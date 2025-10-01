
function calculateFormula(calculationArgumentsObject) {
    setVariablesAndValues(calculationArgumentsObject);
    executeFormulaCalculation(calculationArgumentsObject);
}

function setVariablesAndValues(calculationArgumentsObject) {
    let shouldResetCalculation = false;
    let identifiersAndVariables = calculationArgumentsObject.identifiersAndVariables;
    let fieldSetInstanceRepetitionId = calculationArgumentsObject.fieldSetInstanceRepetitionId;
    let variablesAndValues = {};
    Object.keys(identifiersAndVariables).forEach(x => {
        let value = '';
        let element = getElementByIdentifier(fieldSetInstanceRepetitionId, x);

        if (isSpecialValueSelected($(element))) {
            shouldResetCalculation = true;
            return;
        }

        if ($(element).attr('type') === 'radio') {
            value = getFormulaVariableValueFromRadio(element);
        } else if ($(element).attr('type') === 'checkbox') {
            value = getFormulaVariableValueFromCheckbox(element);
        } else if ($(element).attr('type') === 'number') {
            value = $(element).val();
        } else if ($(element).is('select')) {
            value = getFormulaVariableValueFromSelect(element);
        } else if (isInputDate($(element))) {
            value = getFormulaVariableValueFromDateAndDateTime(element);
        }

        variablesAndValues[identifiersAndVariables[x]] = value;
    });

    calculationArgumentsObject['shouldResetCalculation'] = shouldResetCalculation;
    calculationArgumentsObject['variablesAndValues'] = variablesAndValues;
}

function executeFormulaCalculation(calculationArgumentsObject) {
    let calculativeField = getElementByIdentifier(calculationArgumentsObject.fieldSetInstanceRepetitionId, calculationArgumentsObject.formulaElement);
    let executedExpression = '';
    try {
        executedExpression = calculationArgumentsObject.shouldResetCalculation || isSpecialValueSelected($(calculativeField)) ? "" : executeExpression(calculationArgumentsObject);
    } catch (e) {
        logError(e);
    }
    $(calculativeField).val(executedExpression);
    $(calculativeField).trigger('change');
    removeFieldErrorIfValid($(calculativeField));
}

function getPossibleFieldCalculations() {
    return {
        0: executeNumericCalculation,
        1: executeDateCalculation
    };
}

function executeExpression(calculationArgumentsObject) {
    return getPossibleFieldCalculations()[calculationArgumentsObject.formulaType](calculationArgumentsObject);
}

function executeNumericCalculation(calculationArgumentsObject) {
    let formula = calculationArgumentsObject.formula;
    for (let variable in calculationArgumentsObject.variablesAndValues) {
        formula = tryReplaceVariableWithValue(formula, calculationArgumentsObject.variablesAndValues[variable], variable);
    }
    return eval(formula).toFixed(4);
}

function executeDateCalculation(calculationArgumentsObject) {
    let date1, date2;
    let executedExpression = '';
    Object.keys(calculationArgumentsObject.variablesAndValues).forEach((value, index) => {
        if (index == 0) {
            date1 = calculationArgumentsObject.variablesAndValues[value];
        } else if (index == 1) {
            date2 = calculationArgumentsObject.variablesAndValues[value];
        }
    });
    if (!isNaN(date1) && !isNaN(date2)) {
        executedExpression = getPossibleDateCalculations()[calculationArgumentsObject.granularityType](date1, date2);
    }
    return executedExpression;
}

function getPossibleDateCalculations() {
    return {
        0: getDifferenceInYears,
        1: getDifferenceInMonths,
        2: getDifferenceInDays,
        3: getDifferenceInYearAndMonths,
        4: getDifferenceInYearMonthAndDays
    };
}

function tryReplaceVariableWithValue(formula, value, variableName) {
    if (value || value === 0) {
        var replace = new RegExp(`\\[${variableName}\\]`, 'g');
        formula = formula.replace(replace, value);
        return formula;
    }
    else {
        throw new Error(`[${x}] not defined in formula ${formula}`);
    }
}

function getFormulaVariableValueFromRadio(element) {
    let result;
    let checked;
    if ($(element).is(":checked")) {
        checked = $(element);
    } else {
        checked = $(element)
            .closest(".radio-container")
            .find("input:checked");
    }
    result = checked.data('numericvalue');
    return result;
}

function getFormulaVariableValueFromCheckbox(element) {
    let result = 0;
    let checkedCheckboxes = $(element)
        .closest(".checkbox-container")
        .find("input:checked");
    $.each(checkedCheckboxes, function (index, checkbox) {
        if ($(checkbox).data('numericvalue')) {
            result += $(checkbox).data('numericvalue');
        }
    });

    return result;
}

function getFormulaVariableValueFromSelect(element) {
    let result = '';
    let selected = $(element).find(':selected');
    if (selected) {
        result = $(selected).data('numericvalue');
    }

    return result;
}

function getFormulaVariableValueFromDateAndDateTime(element) {
    let result = initDate($(element).val());

    return result;
}

function getElementByIdentifier(fieldSetInstanceRepetitionId, fieldId) {
    let matchedElement = $();
    $(`[data-fieldid=${fieldId}]`).not("[spec-value]").each(function () {
        if ($(matchedElement).length == 0 || $(this).attr('data-fieldsetinstancerepetitionid') === fieldSetInstanceRepetitionId) {
            matchedElement = $(this);
        }
    });

    return matchedElement;
}

//date calculation helper methods
function getDifferenceInDays(date1, date2) {
    const diffTime = Math.abs(date2 - date1);
    const dateDiffrence = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
    return dateDiffrence + ' Day' + addPlural(dateDiffrence);
}

function getDifferenceInMonths(date1, date2) {
    const monthDifference = calculateDifferenceInMonths(date1, date2);
    return monthDifference + ' Month' + addPlural(monthDifference);
}

function calculateDifferenceInMonths(date1, date2) {
    const dateDifferenceObject = getCompoundDateDifference(date1, date2);
    const monthDifference = dateDifferenceObject.months + dateDifferenceObject.years * 12;
    return monthDifference;
}

function getDifferenceInYears(date1, date2) {
    const dateDifferenceObject = getCompoundDateDifference(date1, date2);
    return `${dateDifferenceObject.years} Year${addPlural(dateDifferenceObject.years)}`;
}

function getDifferenceInYearAndMonths(date1, date2) {
    const dateDifferenceObject = getCompoundDateDifference(date1, date2);
    return `${dateDifferenceObject.years} Year${addPlural(dateDifferenceObject.years)}, ${dateDifferenceObject.months} Month${addPlural(dateDifferenceObject.months)}`;
}

function getDifferenceInYearMonthAndDays(date1, date2) {
    const dateDifferenceObject = getCompoundDateDifference(date1, date2);
    return `${dateDifferenceObject.years} Year${addPlural(dateDifferenceObject.years)}, ${dateDifferenceObject.months} Month${addPlural(dateDifferenceObject.months)}, ${dateDifferenceObject.days} Day${addPlural(dateDifferenceObject.days)}`;
}

function addPlural(number) {
    return number > 1 ? 's' : '';
}

function getCompoundDateDifference(date1, date2) {
    let dateRange = orderDateRange(date1, date2);
    return getDateDifference(dateRange.start, dateRange.end);
}

function orderDateRange(date1, date2) {
    let start, end;
    if (date1 >= date2) {
        start = date2;
        end = date1;
    } else {
        start = date1;
        end = date2;
    }
    return {
        start: start,
        end: end
    };
}

function getDateDifference(start, end) {
    var startYear = start.getFullYear();
    var endYear = end.getFullYear();
    var startMonth = start.getMonth() + 1;
    var endMonth = end.getMonth() + 1;
    var startDateOfMonth = start.getDate();
    var endDateOfMonth = end.getDate();

    var yearDiff = endYear - startYear;
    var monthDiff = endMonth - startMonth;
    var dayDiff = endDateOfMonth - startDateOfMonth;

    // Adjust for negative month difference
    if (monthDiff < 0) {
        monthDiff += 12;
        yearDiff--;
    }

    // Adjust for negative day difference
    if (dayDiff < 0) {
        var prevMonthEndDate = new Date(endYear, endMonth - 1, 0).getDate();
        dayDiff = prevMonthEndDate - startDateOfMonth + endDateOfMonth;
        monthDiff--;

        // Adjust for negative month difference after adjusting for day difference
        if (monthDiff < 0) {
            monthDiff += 12;
            yearDiff--;
        }
    }

    return {
        years: yearDiff,
        months: monthDiff,
        days: dayDiff
    };
}