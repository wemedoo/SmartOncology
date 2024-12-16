///////////
// Modal - Add new medicaction dose
///////////  medication_day
let modalEl;
//Show modal - Standardised Dosing Time
function showMedicationDoseInstanceModal(el) {
    resetDoseModalState();

    $('body').addClass('modal-active');
    $('.modal-med-day').css('display', 'block').trigger('lowZIndex');
    let curEl = $(el).attr('id');
    modalEl = curEl;

    $("#day-item-modal").removeClass("green");
    $("#day-item-modal").removeClass("orange");

    let dataSet = $('#' + modalEl).attr("data-set");
    let dose = {};
    if (dataSet) {
        dose = getDataFromElement(dataSet);
    } else {
        dose['medicationInstanceId'] = getMedicationInstanceRow().attr('data-medicationInstance');
    }
    populateDoseModalWithData(dose);

    let medicationName = $(el).attr('data-medication-name');
    $("#medicationDoseModalTitle").text(medicationName);
    let daynumber = getDayNumber(modalEl);
    $("#day-item-modal").children('span').text(daynumber);

    // Set modal title day-item background style
    if (daynumber > 0) {
        $("#day-item-modal").addClass("green");
    } else {
        $("#day-item-modal").addClass("orange");
    }
}

function populateDoseModalWithData(dose) {
    callServer({
        type: "POST",
        data: dose,
        url: `/SmartOncology/EditMedicationDoseInstanceContent`,
        success: function (data) {
            $("#doseModalContent").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function setSelect2(id, selector, getElementAction, getAutocompleteAction, minimumInputLength = 3) {
    let initDataSource = [];
    callServer({
        type: 'GET',
        url: `/SmartOncology/${getElementAction}?id=${id}`,
        success: function (data) {
            if (!jQuery.isEmptyObject(data)) {
                initDataSource.push(data);
            }
            
            $(`#${selector}`).initSelect2(
                getSelect2Object(
                    {
                        minimumInputLength: minimumInputLength,
                        url: `/SmartOncology/${getAutocompleteAction}`,
                        initialDataSource: initDataSource
                    }
                )
            );
            if (initDataSource.length > 0) {
                $(`#${selector}`).val(initDataSource[0].id.toString()).trigger('change');
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function setMedicationReplacementSelect2() {
    $("#inputMedicationReplacements").initSelect2(
        getSelect2Object(
            {
                url: '/SmartOncology/GetAutoCompleteMedicationData',
                customAjaxData: function (params) {
                    return {
                        Term: params.term,
                        Page: params.page,
                        ChemotherapySchemaInstanceId: getSchemaInstanceId(),
                        IsSupportiveMedication: $("input[name=inputSupportiveMedication]:checked").val()
                    };
                }
            }
        )
    );
}

//Hide modals
$(document).on('click', '.modal-med-day .close-modal', function () {
    closeModal('.modal-med-day');
});

$(document).on('click', '.modal-delay-dose .close-modal', function () {
    closeModal('.modal-delay-dose');
});

function closeModal(modalName) {
    $('body').removeClass('modal-active');
    $(modalName).css('display', 'none').trigger('defaultZIndex');
}

// Handle #interval input on change
$(document).on('change', "#interval", function (e) {
    //Initial - clear inputs
    var intervalStartAt = $('option:selected', this).attr('startAt');
    $('#cycle-starts').val(intervalStartAt);

    let intervalVal = $(this).val();
    setDosingTimeUI(intervalVal);
});

function setDosingTimeUI(intervalId) {
    callServer({
        type: "GET",
        url: `/SmartOncology/EditMedicationDoseTimes?interval=${intervalId}`,
        success: function (data) {
            $(".dose-time-wrapper").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

// Handle modal SAVE button - set data-set to right element;
$(document).on('click', '#save-dose', function () {
    $('#' + modalEl).attr('data-set', '');

    let intervalVal = $('#interval').val();

    if (!intervalVal) {
        toastr.warning("Please fill out data.");
        return;
    }

    let daynumber = getDayNumber(modalEl);
    let elementObj = {
        dayNumber: daynumber,
        date: getDoseDate(),
        unitId: $('#dose-time-unit').val(),
        id: getMedicationDoseInstanceId(),
        medicationInstanceId: getMedicationInstanceId(),
        intervalId: intervalVal,
        startsAt: $('#cycle-starts').val()
    };

    var doses = [];
    $('.dose-time-wrapper .dose-time-value').each(function () {
        var dose = $(this).children('input').val();
        var time = $(this).children('.label').text().trim();
        var doseTimeId = $(this).attr("data-doseTime");
        doses.push({
            'time': time,
            'dose': dose,
            'medicationDoseInstanceId': getMedicationDoseInstanceId(),
            'id': doseTimeId
        });
    });
    elementObj.medicationDoseTimes = doses;

    // Set data to element
    updateMedicationDoseInstance(elementObj);

    toastr.success('Standardised Dosing Time [Day ' + daynumber + '] is updated!');
});

function getDoseDate() {
    var doseDate = $('#' + modalEl).attr('data-date');
    return toDateStringIfValue(doseDate);
}

function updateMedicationDoseInstance(request) {
    if (request.MedicationInstanceId > 0) {
        request['chemotherapySchemaInstanceId'] = getSchemaInstanceId();
        request['medicationName'] = $("#medicationDoseModalTitle").text().trim();
        request['rowVersion'] = getRowVersionMedicationInstance();
        callServer({
            type: "POST",
            url: "/SmartOncology/UpdateSchemaMedicationDoseInstance",
            data: request,
            success: function (data) {
                updateRowVersionMedicationInstance(data.rowVersion);
                updateMedicationDoseInstanceOnUI(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    } else {
        updateMedicationDoseInstanceOnUI(request);
    }
}

function getRowVersionMedicationInstance() {
    return getMedicationInstanceRow().attr('data-rowVersion');
}

function updateRowVersionMedicationInstance(rowVersion) {
    return getMedicationInstanceRow().attr('data-rowVersion', rowVersion);
}

function getMedicationInstanceRow() {
    return $('#' + modalEl).parent('tr');
}

function updateMedicationDoseInstanceOnUI(elementObj) {
    let elementObjStr = JSON.stringify(elementObj);
    let dataSet = encodeURIComponent(elementObjStr);
    $('#' + modalEl).attr('data-set', dataSet);
    setUpdatedValuesInCell(modalEl, elementObj);
    setDoseFlag(modalEl, true);

    // Reset state
    closeModal('.modal-med-day');
    resetDoseModalState();
}

function getMedicationInstanceId() {
    return $("#medicationInstanceId").val();
}

function getMedicationDoseInstanceId() {
    return $('#doseInstanceId').val();
}

function resetDoseModalState() {
    $('#doseId').val("");
    $('#doseInstanceId').val("");
    $('#medicationInstanceId').val("");
    $('#interval').val('');
    $('#cycle-starts').val('');
    $('.dose-time-wrapper').empty();
}

function resetCell(inputEl) {
    var cell = $(inputEl).closest('.input-field');
    var elId = $(cell).attr('id');

    resetCellAction(elId);
}

function resetCellAction(elId) {
    var medicationInstanceRow = $('#' + elId).closest('tr');
    var medicationInstanceId = $(medicationInstanceRow).attr('data-medicationInstance');
    if (medicationInstanceId) {
        callServer({
            type: "DELETE",
            url: "/SmartOncology/DeleteMedicationDoseInstance",
            data: getDeleteDoseObject(elId),
            success: function (data) {
                removeCellOnUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    } else {
        removeCellOnUI();
    }
}

function getDeleteDoseObject(elId) {
    var request = {};

    let dose = getDose($('#' + elId));
    let isEmptyObject = jQuery.isEmptyObject(dose);
    request['Id'] = isEmptyObject ? 0 : dose.Id;

    var medicationInstanceRow = $('#' + elId).closest('tr');
    request['RowVersion'] = $(medicationInstanceRow).attr('data-rowVersion');
    request['MedicationId'] = $(medicationInstanceRow).attr('data-medicationInstance');

    if (!isEmptyObject) {
        request['DayNumber'] = dose.DayNumber;
        request['MedicationName'] = $(medicationInstanceRow).find(".input-field").attr('data-medication-name');
        request['ChemotherapySchemaInstanceId'] = getSchemaInstanceId();
    }

    return request;
}

function removeCellOnUI(elId) {
    $('#' + elId).attr("data-set", '');
    setUpdatedValuesInCell(elId, {});
    setDoseFlag(elId, false);
}

function setUpdatedValuesInCell(elId, obj) {
    $(`#${elId}`).children('input').val(formatValueOutput(obj));
    $(`#${elId}`).children('input').attr('data-original-title', formatValueOutputTooltip(obj));
}

function formatValueOutput(obj) {
    if (jQuery.isEmptyObject(obj)) return '';
    var unit = getSelectedUnit();
    let doseTimeValues = [];
    for (var doseTime of obj.medicationDoseTimes) {
        let value = doseTime.time + "h : " + formatDose(doseTime.dose);
        doseTimeValues.push(value);
    }
    return `${doseTimeValues.join(";\n")} ${unit ? '[' + unit + ']' : ''}`;
}

function formatValueOutputTooltip(obj) {
    if (jQuery.isEmptyObject(obj)) return '';
    var unit = getSelectedUnit();
    let doseTimeValues = [];
    for (var doseTime of obj.medicationDoseTimes) {
        let value = `<li>${doseTime.time}h : ${formatDose(doseTime.dose)} ${unit ? unit : ''}</li>`;
        doseTimeValues.push(value);
    }
    return `<div><h4>Dosing time:</h4><ol>${doseTimeValues.join("")}</ol></div>`;
}

function formatDose(dose) {
    return dose ? dose : 'N/A';
}

function getSelectedUnit() {
    return getSelectedSelect2Label("dose-time-unit");
}

function setDoseFlag(elId, val) {
    $(`#${elId}`).attr('data-hasValue', val);
}

function getDayNumber(el) {
    return el.split('_')[1];
}


// Medication (Instance) form
$(document).on('click', '.add-medication', function (event) {
    var tableExpanded = $(this).closest('tr').attr("aria-expanded");
    if (tableExpanded === "true") {
        event.stopPropagation();
    }
    var tableName = $(this).closest('tr').attr('data-target').substring(1);

    callServer({
        type: "GET",
        url: `/SmartOncology/GetSchemaMedicationInstance?id=0`,
        success: function (data) {
            showMedicationModal(data, tableName);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
});

function showMedicationModal(content, tableName) {
    $('#medication-modal-content').html(content);
    var isSupportiveMedication = tableName == "table-supportive-therapy";
    $(`input[name="inputSupportiveMedication"][value="${isSupportiveMedication}"]`).prop("checked", true);
    $(`input[name="inputSupportiveMedication"]`).prop("disabled", true);
    $('#medicationModal').modal("show");
}

$(document).on('click', 'input[name="inputHasMaximulaCumulativeDose"]', function (e) {
    shouldCumulativeDoseContainerBeDisplayed();
});

function shouldCumulativeDoseContainerBeDisplayed() {
    if ($('#hasCumulativeDose').prop('checked')) {
        $('.cumulative-dose-wrapper').fadeIn(150);
    } else {
        $('.cumulative-dose-wrapper :input').val("");
        $('.cumulative-dose-wrapper').fadeOut(150);
    }
}

$(document).on('click', 'input[name="inputAsReserve"]', function (e) {
    shouldMedicationReplacementContainerBeDisplayed();
});

function shouldMedicationReplacementContainerBeDisplayed() {
    if ($('#inputAsReserve').prop('checked')) {
        $('.replacement-container').fadeIn(150);
    } else {
        $('.replacement-container').fadeOut(150);
    }
}

function updateMedication(form, event) {
    event.preventDefault();
    event.stopPropagation();

    $(form).validate({
        ignore: []
    });

    if ($(form).valid()) {
        var openedElements = getCollapsedTables();
        let request = getMedicationInstanceObject();
        callServer({
            type: "POST",
            url: "/SmartOncology/UpdateSchemaMedicationInstance",
            data: request,
            success: function (data) {
                $('#medicationModal').modal('hide');
                $(".modal-backdrop").remove();
                $("#schemaData").html(data);
                collapsePrevioslyOpenedTables(openedElements);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function getMedicationInstanceObject() {
    let medicationInstance = {};

    let medicationInstanceId = getMedicationInstanceFormId();
    let schemaInstanceId = getSchemaInstanceId();
    let medication = getMedicationObject();

    medicationInstance['Id'] = medicationInstanceId;
    medicationInstance['ChemotherapySchemaInstanceId'] = schemaInstanceId;
    medicationInstance['Medication'] = medication;
    medicationInstance['MedicationId'] = medication.Id;
    medicationInstance['MedicationIdsToReplace'] = $("#inputMedicationReplacements").val();

    return medicationInstance;
}

function getMedicationInstanceFormId() {
    return 0;
}

function getMedicationObject() {
    let request = {};

    request['Id'] = getMedicationId();
    request['Name'] = $("#inputMedicationName").val();
    request['Dose'] = $("#inputAmount").val();
    request['UnitId'] = $("#inputUnit").val();
    request['PreparationInstruction'] = $("#inputPreparationInstruction").val();
    request['ApplicationInstruction'] = $("#inputApplicationInstruction").val();
    request['AdditionalInstruction'] = $("#inputAdditionalInstruction").val();
    request['RouteOfAdministration'] = $("#inputRouteOfAdministration").val();
    request['BodySurfaceCalculationFormula'] = $("#inputDoseCalculationFormula").val();
    request['SameDoseForEveryAplication'] = $("input[name=inputSameDose]:checked").val();
    request['HasMaximalCumulativeDose'] = $("input[name=inputHasMaximulaCumulativeDose]:checked").val();
    request['CumulativeDose'] = $("#inputMaxCumulativeDose").val();
    request['CumulativeDoseUnitId'] = $("#inputMaxCumulativeDoseUnit").val();
    request['WeekendHolidaysExcluded'] = $("input[name=inputHolidaysExcluded]:checked").val();
    request['MaxDayNumberOfApplicationiDelay'] = $("#inputMaxDaysOfDelay").val();
    request['IsSupportiveMedication'] = $("input[name=inputSupportiveMedication]:checked").val();
    request['SupportiveMedicationReserve'] = $("#inputAsReserve:checked").val();
    request['SupportiveMedicationAlternative'] = $("#inputHasAlternative:checked").val();

    return request;
}

function getMedicationId() {
    return $("#inputMedicationId").val();
}

function deleteMedication(event) {
    event.preventDefault();
    event.stopPropagation();

    var id = document.getElementById("buttonSubmitDelete").getAttribute('data-id');

    var medicationInstanceTr = $(`tr[data-medicationinstance=${id}]`);

    var request = {
        id: id,
        chemotherapySchemaInstanceId: getSchemaInstanceId(),
        rowVersion: getRowVersion(),
        rowVersionMedication: $(medicationInstanceTr).attr('data-rowversion'),
    };

    var openedElements = getCollapsedTables();

    callServer({
        type: "DELETE",
        data: request,
        url: `/SmartOncology/DeleteMedicationInstance`,
        success: function (data) {
            $('#deleteModal').modal('hide');
            $(".modal-backdrop").remove();
            toastr.success(`Success`);
            $("#schemaData").html(data);
            collapsePrevioslyOpenedTables(openedElements);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('click', '.replace-icon', function (e) {
    var medicationInstanceId = $(this).closest('tr').attr('data-medicationinstance');
    callServer({
        type: "GET",
        url: `/SmartOncology/ViewMedicationReplacements?medicationInstanceId=${medicationInstanceId}`,
        success: function (data) {
            showMedicationReplacementModal(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
});

function showMedicationReplacementModal(content) {
    $('#medication-replacement-modal-content').html(content);
    $('#medicationReplacementModal').modal("show");
}