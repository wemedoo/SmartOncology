function setCustomDateFields(element) {
    if (element) {
        $(element).attr('data-preventfuturedates', encodeURIComponent($('#preventFutureDates').is(":checked")));
    }
    setCommonStringFields(element);
}