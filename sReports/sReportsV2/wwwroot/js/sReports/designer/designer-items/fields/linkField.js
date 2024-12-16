function setCustomLinkFields(element) {
    if (element) {
        $(element).attr('data-link', encodeURIComponent($('#link').val()));
    }
}