function setCustomParagraphFields(element) {
    if (element) {
        var content = tinymce.get('paragraph').getContent();
        $(element).attr('data-paragraph', encodeURIComponent(content));
    }
}