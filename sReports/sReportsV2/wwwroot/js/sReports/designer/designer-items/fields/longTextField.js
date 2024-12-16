function setCustomLongTextFields(element) {
    if (element) {
        $(element).attr('data-dataextractionenabled', encodeURIComponent($('#enableDataExtraction').is(":checked")));
    }
    setCommonStringFields(element);
}