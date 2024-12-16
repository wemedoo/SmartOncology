//date extension methods
var defaultDateFormat = 'dd/mm/yy';

$.fn.initDatePicker = function (chooseSepareteMonthYear = false, yearRange = false, beforeShow = undefined, onClose = undefined) {
	var datePickerOptions = {};
	datePickerOptions['dateFormat'] = defaultDateFormat;

	if (shouldPreventFutureDates(this)) {
		datePickerOptions['maxDate'] = new Date();
	}

	if (chooseSepareteMonthYear) {
		datePickerOptions['changeMonth'] = true;
		datePickerOptions['changeYear'] = true;
	}

	if (yearRange) {
		datePickerOptions['yearRange'] = 'c-100:9999';
	}

	if (beforeShow) {
		datePickerOptions['beforeShow'] = beforeShow;
	}

	if (onClose) {
		datePickerOptions['onClose'] = onClose;
	}

    return this.datepicker(datePickerOptions);
}

$(document).on("change", ".field-date-input", function () {
	removeFieldErrorIfValid($(this), $(this).attr("id"));
	removeFieldErrorIfValidForTimeInput($(this));
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

function toISO8601DateTimeWithOffset(value, timeZoneOffset) {
	return `${value}${timeZoneOffset}`
}

function dateForComparison(value) {
	var date = initDate(value);
	var [day, month, year] = extractDate(date);

	return `${year}${formatTo2Digits(month)}${formatTo2Digits(day)}`;
}

function initDate(value) {
	return new Date(formatDateToValid(value));
}

function toLocaleDateStringIfValue(valueInUtc) {
	return valueInUtc ? `${valueInUtc}${getUtcTimezoneOffset()}` : valueInUtc;
}

function toValidTimezoneFormat(value) {
	return value.replace(' ', '+')
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

function getUtcTimezoneOffset() {
	var offsetInMins = new Date().getTimezoneOffset();
	var offsetSign = offsetInMins * (-1) > 0 ? '+' : '-';
	var offsetHours = Math.abs(offsetInMins / 60);
	var offset = offsetSign + formatTo2Digits(offsetHours) + ":00";

	return offset;
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
			$("#entryDatetimeDefault").val(dateTime);
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

$(document).on("blur", "input[data-date-input]:not(.field-date-input)", function (e) {
	validateDateInput($(this));
	validateFutureDates($(this));
});

$(document).on("change", "input[data-date-input]:not(.field-date-input)", function (e) {
	var form = $(this).closest("form");
	form.validate().element(this);
});

function shouldPreventFutureDates($input) {
	return $input.attr('data-preventfuturedates') === "True";
}

function validateDateInput($input) {
	var inputValue = $input.val();
	var inputLength = inputValue.length;
	var parsedDate = toDateStringIfValue(inputValue);
	var invalid = inputLength > 0 && (inputLength !== getAllowedDateLength() || parsedDate === "Invalid Date");
	if (invalid) {
		$input.addClass("error");
		return false;
	} else {
		clearErrorLabel($input);
		return true;
	}
}

function validateFutureDates($input) {
	if (shouldPreventFutureDates($input)) {
		var inputValue = $input.val();
		var parsedDate = toDateStringIfValue(inputValue);
		var date = new Date(parsedDate);
		var currentDate = new Date();
		if (date > currentDate) {
			$input.addClass("error");
			return false;
		}
		else {
			clearErrorLabel($input);
			return true;
		}
	}
	else {
		return true;
	}
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

	$.validator.addMethod(
		"timeInputValidation",
		function (value, element) {
			if ($(element).hasClass("time-part")) {
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
			validateFutureDates: true
		});
		let allowedDateLength = getAllowedDateLength();
		$(this)
			.attr("maxlength", allowedDateLength)
			.attr("data-maxlength", allowedDateLength);
	});

	$("input.time-part").each(function () {
		$(this).rules('add', {
			timeInputValidation: true
		});
	});
}

function isDateOrTimeInput(element) {
	return element.hasClass("time-part") || element.is("[data-date-input]");
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
	organizationOffset = decodeHtmlEntity(organizationOffset);
	if (activeDateTime.getTime() === maxDate.getTime())
		return getActiveDate(dateFieldId, timeFieldId) + "+00:00";
	else {
		var activeDate = getActiveDate(dateFieldId, timeFieldId);
		return activeDate != "" ? activeDate + organizationOffset : "";
	}
}

function calculateDateWithOffset(dateFieldId) {
	organizationOffset = decodeHtmlEntity(organizationOffset);
	var activeDate = toDateStringIfValue(dateFieldId);
	if (activeDate != "")
		return activeDate + organizationOffset;
	else
		return activeDate;
}

function decodeHtmlEntity(str) {
	var txt = document.createElement('textarea');
	txt.innerHTML = str;
	return txt.value;
}

function getActiveDate(dateFieldId, timeFieldId) {
	var activeDate = toDateStringIfValue($(dateFieldId).val());
	var activeTime = timeFieldId != null ? $(timeFieldId).val() : "00:00";

	if (activeDate != "")
		return activeDate + ' ' + activeTime;
	else
		return "";
}