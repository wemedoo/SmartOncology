//date extension methods
var defaultDateFormat = 'dd/mm/yy';

$.fn.initDatePicker = function (chooseSepareteMonthYear = false, yearRange = false, beforeShow = undefined, onClose = undefined) {
	var datePickerOptions = {};
	datePickerOptions['dateFormat'] = defaultDateFormat;

	if (shouldPreventFutureDates(this)) {
		datePickerOptions['maxDate'] = new Date();
	}

	if (shouldRestrictAge(this)) {
		const today = new Date();
		const minDateFor18 = new Date(today.setFullYear(today.getFullYear() - 18));
		datePickerOptions['maxDate'] = minDateFor18;

		if (!yearRange) {
			datePickerOptions['yearRange'] = `c-100:${minDateFor18.getFullYear()}`;
		}
	}

	if (chooseSepareteMonthYear) {
		datePickerOptions['changeMonth'] = true;
		datePickerOptions['changeYear'] = true;
	}

	if (yearRange) {
		datePickerOptions['yearRange'] = 'c-100:c+100';
	}

	if (beforeShow) {
		datePickerOptions['beforeShow'] = beforeShow;
	}

	if (onClose) {
		datePickerOptions['onClose'] = onClose;
	}

    return this.datepicker(datePickerOptions);
}

$(document).ready(function () {
	$("[data-date-validation]").on("change blur", function () {
		const $input = $(this);

		if (!validateDateInput($input)) {
			markDateInputInvalid($input, `Please put your date in [${getDateFormatDisplay()}] format.`);
		} else if ($input.is("[data-preventfuturedates]") && !validateFutureDates($input)) {
			markDateInputInvalid($input, `Your date cannot be greater than current date for this field.`);
		} else {
			clearDateInputError($input);
		}
	});
});

//date helpers methods
function toDateStringIfValue(value) {
	return value ? initDate(value).toDateString() : value;
}

function toDateISOStringIfValue(value) {
	var date = initDate(value);
	var [day, month, year] = extractDate(date);
	
	return `${year}-${formatTo2Digits(month)}-${formatTo2Digits(day)}`;
}

function dateForComparison(value) {
	var date = initDate(value);
	var [day, month, year] = extractDate(date);

	return `${year}${formatTo2Digits(month)}${formatTo2Digits(day)}`;
}

function initDate(value) {
	return new Date(formatDateToValid(value));
}

function toLocaleDateStringIfValue(valueInUtc, timeZoneOffset, returnInUtcTimeZone) {
	timeZoneOffset = timeZoneOffset ? timeZoneOffset : userTimeZoneOffset;
	return valueInUtc ? toValidTimezoneFormat(convertToCustomTimeZone(valueInUtc, userTimeZoneOffset, returnInUtcTimeZone)) : '';
}

function toValidTimezoneFormat(value) {
	return value.replace(' ', '+');
}

function formatDateToValid(value) {
	if (df == defaultDateFormat) {
		return value.replace(/^(\d{1,2})\/(\d{1,2})\//, '$2/$1/');
	} else {
		return value;
	}
}

function formatUtcDateToClientFormat(utcDateValue) {
	let retVal = '';
	if (utcDateValue) {
		let splittedParts = utcDateValue.split('-');
		if (df == defaultDateFormat && splittedParts.length == 3) {
			retVal = `${splittedParts[2]}/${splittedParts[1]}/${splittedParts[0]}`;
		} else {
			retVal = utcDateValue;
		}
	}
	return retVal;
}

function extractUtcDatePart(utcDatetimeValue) {
	let utcDatePart;
	if (utcDatetimeValue) {
		utcDatePart = utcDatetimeValue.split("T")[0];
	}
	return utcDatePart;
}

function extractUtcTimePart(utcDatetimeValue) {
	let utcTimePart;
	if (utcDatetimeValue) {
		utcTimePart = utcDatetimeValue.split("T")[1].substring(0, 5);
	}
	return utcTimePart;
}

function formatTo2Digits(inputDigit) {
	return ('0' + inputDigit).slice(-2);
}

function extractDate(date) {
	var day = date.getDate();
	var month = date.getMonth() + 1;
	var year = date.getFullYear();
	return [day, month, year];
}

function setValueForDateTime(paramName, paramValue) {
	const dateTime = paramValue.slice(0, 16);
	let formattedDate = toDateFormatDisplay(dateTime.split("T")[0]);

	switch (paramName) {
		case "DateTimeFrom":
		case "DateTimeTo":
		case "RequestTimestampFrom":
		case "RequestTimestampTo": {
			const time = dateTime.split("T")[1].slice(0, 5);
			const lowerParamName = firstLetterToLower(paramName);
			const container = $(`#${lowerParamName}`).closest('.datetime-picker-container');
			$(`#${lowerParamName}`).val(dateTime);
			container.find('.time-helper').val(time);
			container.find('input:first').val(formattedDate);
			break;
		}
		case "BirthDate":
			$("#birthDate, #BirthDateTemp").val(formattedDate);
			$("#birthDateDefault").val(dateTime);
			break;
		case "EntryDatetime":
			$("#entryDatetime").val(formattedDate);
			$("#entryDatetimeDefault").val(toValidTimezoneFormat(paramValue));
			break;
		case "AdmissionDate":
			$("#admissionDate").val(formattedDate);
			$("#admissionDateDefault").val(dateTime);
			break;
		case "DischargeDate":
			$("#dischargeDate").val(formattedDate);
			$("#dischargeDateDefault").val(dateTime);
			break;
		default:
			break;
	}
}

function toDateFormatDisplay(utcDate) {
	var [day, month, year] = extractDate(new Date(utcDate));
	return `${formatTo2Digits(day)}/${formatTo2Digits(month)}/${year}`;
}

function isDateTimeFilter(paramName) {
	const datetimeParams = ["DateTimeFrom", "DateTimeTo", 'RequestTimestampFrom', 'RequestTimestampTo', "BirthDate", "EntryDatetime", "AdmissionDate", "DischargeDate"];
	return datetimeParams.includes(paramName);
}

function getDateTimeFilterTag(params, param) {
	const partsOfDate = params[param].split('T')[0].split('-');

	let fromFilters = ["DateTimeFrom", "RequestTimestampFrom"];
	let toFilters = ["DateTimeTo", "RequestTimestampTo"];

	if (fromFilters.includes(param)) {
		return `From: ${partsOfDate.reverse().join('/')} ${params[param].split('T')[1].slice(0, 5)}`;
	} else if (toFilters.includes(param)) {
		return `To: ${partsOfDate.reverse().join('/')} ${params[param].split('T')[1].slice(0, 5)}`;
	} else if (isDateTimeFilter(param)) {
		return partsOfDate.reverse().join('/');
	}

	return null;
}

var dateDelimiter = '/';
$(document).on("keypress", "input[data-date-input]", function (e) {
	if (isForbiddenDateInputCharacter(e.key)) {
		e.preventDefault();
	}
	$(this).data("key-action", true);

	var value = $(this).val();
	var inputLength = value.length;


	if (isNotTimeForDateDelimiter(inputLength) ) {
		if (e.key == dateDelimiter) {
			e.preventDefault();
		}
	}

	if (isTimeForDateDelimiter(inputLength)) {
		value += dateDelimiter;
	}

	$(this).val(value);
});

$(document).on("blur", "input[data-date-input]", function (e) {
	if (!$(this).val() && !$(this).data('key-action')) {
		$(this).val($(this).data("initial-date-value"));
		updateDatetimeIfNeeded($(this));
	}
	if (!$(this).attr('skip-blur')) {
		const $input = $(this);

		const isDateValid = validateDateInput($input);
		const isFutureValid = validateFutureDates($input);
		const isAgeValid = validateAge18($input);

		if (isDateValid && isFutureValid && isAgeValid) {
			clearErrorLabel($input);
		} else {
			$input.addClass("error");
		}
	}
});

$(document).on("change", "input[data-date-input]", function (e) {
	if ($(this).hasClass('field-date-input')) {
		removeFieldErrorIfValid($(this), $(this).attr("id"));
		removeFieldErrorIfValidForTimeInput($(this));
	} else {
		if (!$(this).attr('skip-change')) {
			var form = $(this).closest("form");
			if (form.length) {
				form.validate().element(this);
			}
		}
	}
});

$(document).on("focus", "input[data-date-input]", function (e) {
	$(this).data("initial-date-value", $(this).val());
	$(this).data("key-action", false);
	$(this).val("");
	updateDatetimeIfNeeded($(this));
});

function updateDatetimeIfNeeded($dateInput) {
	if (typeof handleDateHelper === 'function' && $dateInput.hasClass('date-helper')) {
		handleDateHelper($dateInput, true);
	}
}

function shouldPreventFutureDates($input) {
	return $input.attr('data-preventfuturedates') === "True";
}

function shouldRestrictAge($input) {
	return $input.attr('data-restrictage18') === "True";
}

function validateDateInput($input) {
	var inputValue = $input.val();
	var inputLength = inputValue.length;

	if (inputLength === 0) {
		return true;
	}

	var parsedDate = toDateStringIfValue(inputValue);
	var invalid = inputLength !== getAllowedDateLength() || parsedDate === "Invalid Date";
	return !invalid;
}

function validateFutureDates($input) {
	var inputValue = $input.val();

	if (inputValue.length === 0 || !shouldPreventFutureDates($input)) {
		return true;
	}

	var parsedDate = toDateStringIfValue(inputValue);
	var date = new Date(parsedDate);
	var currentDate = new Date();
	return date <= currentDate;
}

function validateAge18($input) {
	var inputValue = $input.val();

	if (inputValue.length === 0 || !shouldRestrictAge($input)) {
		return true;
	}

	const inputDate = $input.datepicker('getDate');
	if (!inputDate || isNaN(inputDate)) {
		return false;
	}

	const today = new Date();
	const minDateFor18 = new Date(today.setFullYear(today.getFullYear() - 18));
	return inputDate <= minDateFor18;
}

function clearErrorLabel($input) {
	$input.removeClass("error");
	let inputId = $input.attr('id');
	let $errorLabel = $(`#${inputId}-error`);
	$errorLabel.text('').css('display', 'none');
}

function isForbiddenDateInputCharacter(inputCharacter) {
	return isNaN(inputCharacter) && inputCharacter !== dateDelimiter && inputCharacter != "Enter";
}

function isTimeForDateDelimiter(inputLength) {
	return inputLength === 2 || inputLength === 5;
}

function isNotTimeForDateDelimiter(inputLength) {
	return inputLength !== 1 || inputLength !== 3;
}

function getAllowedDateLength() {
	return getDateFormatDisplay().length;
}

function getDateFormatDisplay() {
	return typeof dateFormatDisplay === 'undefined' ? 'dd/mm/yyyy' : dateFormatDisplay;
}

function getTimeFormatDisplay() {
	return typeof timeFormatDisplay === 'undefined' ? 'hh:mm' : timeFormatDisplay;
}

var defaultTimePart = "00:00";
function toFullDateISO(dateInputValue) {
	let datePart = toDateISOStringIfValue(dateInputValue);
	return `${datePart}T${defaultTimePart}`;
}

function copyDateToHiddenField(value, hiddenField) {
	if (value) {
		$(`#${hiddenField}`).val(toFullDateISO(value));
	}
}

function setDateTimeValidatorMethods() {
	$.validator.addMethod(
		"dateInputValidation",
		function (value, element) {
			if ($(element).is("[data-date-input]")) {
				return validateDateInput($(element));
			} else {
				return true;
			}
		},
		`Please put your date in [${getDateFormatDisplay()}] format.`
	);

	$.validator.addMethod(
		"validateFutureDates",
		function (value, element) {
			if ($(element).is("[data-date-input]")) {
				return validateFutureDates($(element));
			} else {
				return true;
			}
		},
		`Your date cannot be greater than current date for this field.`
	);

	$.validator.addMethod("validateAge18", function (value, element) {
		if ($(element).is("[data-restrictage18]")) {
			return validateAge18($(element));
		} else {
			return true;
		}
	}, "Date must indicate you are 18 or older as of today.");

	$.validator.addMethod(
		"timeInputValidation",
		function (value, element) {
			if ($(element).hasClass("time-helper")) {
				return validateTimeInput($(element));
			} else {
				return true;
			}
		},
		`Please put your time in [${getTimeFormatDisplay()}] format.`
	);

	$('form:has([data-date-input])').each(function () {
		$(this).validate({
			errorPlacement: function (error, element) {
				handleErrorPlacement(error, element);
			}
		});
	}); 

	$("input[data-date-input]").each(function () {
		$(this).rules('add', {
			dateInputValidation: true,
			validateFutureDates: true,
			validateAge18: true
		});
		let allowedDateLength = getAllowedDateLength();
		$(this)
			.attr("maxlength", allowedDateLength)
			.attr("data-maxlength", allowedDateLength);
	});

	$("input.time-helper").each(function () {
		$(this).rules('add', {
			timeInputValidation: true
		});
	});
}

function isDateOrTimeInput(element) {
	return element.hasClass("time-helper") || element.is("[data-date-input]");
}

function handleErrorPlacement(error, element) {
	if (isDateOrTimeInput(element)) {
		handleErrorPlacementForDateOrTime(error, element);
	} else if (isRadioOrCheckbox(element)) {
		handleErrorPlacementForRadioOrCheckbox(error, element);
	} else {
		handleErrorPlacementForOther(error, element);
	}
}

function handleErrorPlacementForDateOrTime(error, element) {
	var targetContainerForErrors = getElementWhereErrorShouldBeAdded(element);
	modifyIfSecondError(targetContainerForErrors, error);
	error.appendTo(targetContainerForErrors);
}

function convertTimeFormat(time) {
	const parts = time.split(':');
	const hours = parts[0];
	const minutes = parts[1];
	const result = hours + minutes;

	return result;
}

function compareActiveDate(activeFromId, activeToId) {
	var activeToDate = document.getElementById(activeToId);
	var activeTo = dateForComparison(activeToDate.value);
	var activeFrom = dateForComparison(document.getElementById(activeFromId).value);

	if (activeFrom > activeTo) {
		activeToDate.classList.add("error");
		return false;
	}

	activeToDate.classList.remove("error");
	return true;
}

function compareActiveDateTime(activeFromId, activeToId, activeFromTimeId, activeToTimeId, activeToTimeWrapperId) {
	var activeToDate = document.getElementById(activeToId);
	var activeToTimeWrapper = document.getElementById(activeToTimeWrapperId);
	var activeFromTime = document.getElementById(activeFromTimeId);
	var activeToTime = document.getElementById(activeToTimeId);
	var activeTo = dateForComparison(activeToDate.value) + convertTimeFormat(activeToTime.value);
	var activeFrom = dateForComparison(document.getElementById(activeFromId).value) + convertTimeFormat(activeFromTime.value);

	if (activeFrom > activeTo) {
		addErrorClass(activeToDate, activeToTimeWrapper, activeToTime);
		return false;
	}

	removeErrorClass(activeToDate, activeToTimeWrapper, activeToTime);
	removeActiveToError(activeToId + "-error");
	removeActiveToError(activeToTimeId + "-error");
	return true;
}

function removeActiveToError(id) {
	var errorLabel = document.getElementById(id);

	if (errorLabel) {
		errorLabel.parentNode.removeChild(errorLabel);
	}
}

function removeErrorClass(activeToDate, activeToTimeWrapper, activeToTime) {
	activeToDate.classList.remove("date-error");
	activeToTimeWrapper.classList.remove("date-error");
	activeToTime.classList.remove("time-error");
}

function addErrorClass(activeToDate, activeToTimeWrapper, activeToTime) {
	activeToDate.classList.add("date-error");
	activeToTimeWrapper.classList.add("date-error");
	activeToTime.classList.add("time-error");
}

function calculateDateTimeWithOffset(dateFieldId, timeFieldId) {
	var activeDateTime = new Date(getActiveDate(dateFieldId, timeFieldId));
	var maxDate = new Date("Fri Dec 31 9999 23:59");
	if (activeDateTime.getTime() === maxDate.getTime())
		return getActiveDate(dateFieldId, timeFieldId) + "+00:00";
	else {
		var dateValue = $(dateFieldId).siblings('.date-time-local').val();
		return dateValue ? convertToCustomTimeZone(dateValue, organizationTimeZoneOffset) : '';
	}
}

function calculateDateWithOffset(dateFieldId) {
	let value = $(dateFieldId).val();
	return value ? convertToCustomTimeZone(toFullDateISO(value), organizationTimeZoneOffset) : '';
}

function convertToCustomTimeZone(utcDateTime, timeZoneIanaCode, skipTimeZone = false) {
	if (skipTimeZone) return `${utcDateTime}+00:00`;
	var momentDate = moment.tz(utcDateTime, timeZoneIanaCode);
	return momentDate.toISOString(true);
}

function getActiveDate(dateFieldId, timeFieldId) {
	var activeDate = toDateStringIfValue($(dateFieldId).val());
	var activeTime = timeFieldId != null ? $(timeFieldId).val() : "00:00";

	if (activeDate != "")
		return activeDate + ' ' + activeTime;
	else
		return "";
}

function markDateInputInvalid($input, errorMessage) {
	clearDateInputError($input);
	$input.addClass("error").attr("aria-invalid", "true");

	const errorId = $input.attr("id") + "-error";
	if ($(`#${errorId}`).length === 0) {
		$(`<label id="${errorId}" class="error" for="${$input.attr("id")}">${errorMessage}</label>`)
			.insertAfter($input);
	}
}

function clearDateInputError($input) {
	$input.removeClass("error").removeAttr("aria-invalid");

	const errorId = $input.attr("id") + "-error";
	$(`#${errorId}`).remove();
}