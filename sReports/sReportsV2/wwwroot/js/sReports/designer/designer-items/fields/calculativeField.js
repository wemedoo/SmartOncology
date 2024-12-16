$(document).on('change', '#calculationFormulaType', function (e) {
    if (isDateCalculationType($(this).val())) {
        $("#calculationGranularityTypeContainer").removeClass("d-none");
    } else {
        $("#calculationGranularityTypeContainer").addClass("d-none");
    }
    reloadCalculativeTree(null);
    $("#formula").val('');
    $("#calculationGranularityType").val('');
});

function isDateCalculationType(value) {
    return value == dateCalculationType;
}

function reloadCalculativeTree(identifierTypes) {
    let fields = getAvailableFieldsForCalculation(identifierTypes);
    loadCalculativeTree({ data: fields });
}

function getAvailableFieldsForCalculation(identifierTypes) {
    let fields = [];
    getOpenedElementId();
    $.each(getPossibleFieldTypesForCalculations(), function (ind, value) {
        $(`#nestable li.dd-item[data-itemtype=field][data-type=${value}]`).each(function (index, element) {
            fields.push(generateIdentifierVariableObject(element, identifierTypes));
        });
    })

    return fields;
}

let possibleFieldTypes = {
    0: ['number', 'radio', 'checkbox', 'select'],
    1: ['date', 'datetime']
};
function getPossibleFieldTypesForCalculations() {
    let calculationFormulaType = +$("#calculationFormulaType").val();
    return possibleFieldTypes[calculationFormulaType];
}

function generateIdentifierVariableObject(element, identifierTypes) {
    let dataProps = generateObjectFromDataProperties(element, configByType['field'].excludeProperties);
    return {
        Id: dataProps['id'],
        Label: dataProps['label'],
        Type: dataProps['type'],
        VariableName: identifierTypes ? identifierTypes[dataProps['id']] : ''
    };
}

function loadCalculativeTree(data) {
    setIsReadOnlyViewModeInRequest(data);
    callServer({
        method: 'POST',
        data: data,
        url: '/Form/GetCalculativeTree',
        contentType: 'application/json',
        success: function (data) {
            $('#calculativeTree').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function setCustomCalculativeFields(element) {
    if (element) {
        let formula = $('#formula').val();
        let fieldsForCalculationWithVariables = getFieldsForCalculationWithVariables(formula);
        if (fieldsForCalculationWithVariables['valid']) {
            $(element).attr('data-identifiersandvariables', encodeURIComponent(JSON.stringify(fieldsForCalculationWithVariables['fieldsAndVariables'])));
            $(element).attr('data-formula', encodeURIComponent(formula));
            $(element).attr('data-formulatype', encodeURIComponent($("#calculationFormulaType").val()));
            $(element).attr('data-granularitytype', encodeURIComponent($("#calculationGranularityType").val()));
            setCommonStringFields(element);
        }
    }
}

function getFieldsForCalculationWithVariables(formula) {
    let result = {};
    result['valid'] = true;
    result['allVariablesAssignedToField'] = true;
    result['duplicateVariableAssignment'] = true;
    let variables = getVariablesFromFormula(formula);
    if (variables) {
        result['fieldsAndVariables'] = {};
        variables.forEach(x => {
            let variableName = getVariableName(x);
            let fieldsWithAssignedVariables = getFieldForVariable(variableName);

            if (fieldsWithAssignedVariables.length == 0) {
                result['allVariablesAssignedToField'] = false;
                result['valid'] = false;
                return result;
            } else if (fieldsWithAssignedVariables.length > 1) {
                result['duplicateVariableAssignment'] = false;
                result['valid'] = false;
                return result;
            } else {
                let fieldId = $(fieldsWithAssignedVariables).attr('data-fieldid');
                result['fieldsAndVariables'][fieldId] = variableName;
            }
        });
    }
    return result;
}

function getFieldForVariable(variableName) {
    let result = [];
    $('.calculative-field-variable-input').each(function (index, element) {
        if ($(this).val().trim() == variableName) {
            result.push(element);
        }
    });

    return result;
}

function getVariableName(variable) {
    return variable.replace("[", "").replace("]", "").trim()
}

function getVariablesFromFormula(formula) {
    return formula.match(/\[[^[]+\]/g);
}

function doesFormulaMatchDateCaculation(formula, fieldsAndVariables) {
    return /^\[([^\]]+)\]-\[([^\]]+)\]$/.test(formula) && fieldsAndVariables && Object.keys(fieldsAndVariables).length == 2;
}

$(document).on('focusin', '.calculative-variable-input-container .designer-form-input', function (e) {
    let variableName = $(this).parent().siblings('.calculative-variable-name');
    $(variableName).addClass('active');
})

$(document).on('focusout', '.calculative-variable-input-container .designer-form-input', function (e) {
    let variableName = $(this).parent().siblings('.calculative-variable-name');
    $(variableName).removeClass('active');
})

let dateCalculationType = 1;
$(document).ready(function () {
    jQuery.validator.addMethod("allVariablesAssignedToField", function (value, element) {
        let result = getFieldsForCalculationWithVariables(value);
        return result['allVariablesAssignedToField'];//error
    }, "*Not all variables are assigned to fields");

    jQuery.validator.addMethod("duplicateVariableAssignment", function (value, element) {
        let result = getFieldsForCalculationWithVariables(value);
        return result['duplicateVariableAssignment'];//error
    }, "Some of variables are assigned to more than one field");

    jQuery.validator.addMethod("granularityType", function (value, element) {
        let isDateCalculationFormula = isDateCalculationType($("#calculationFormulaType").val());
        return (isDateCalculationFormula && value) || !isDateCalculationFormula;
    }, "Please choose granularity type for date(time) calculation");

    jQuery.validator.addMethod("datetimeCalculationFormula", function (value, element) {
        let isDateCalculationFormula = isDateCalculationType($("#calculationFormulaType").val());
        let result = getFieldsForCalculationWithVariables(value);
        return (isDateCalculationFormula && doesFormulaMatchDateCaculation(value, result.fieldsAndVariables)) || !isDateCalculationFormula;
    }, "Date(time) calculation should be in this format: [date_n1] - [date_n2]");
})