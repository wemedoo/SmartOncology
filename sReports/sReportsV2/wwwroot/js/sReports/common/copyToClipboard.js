
function copyRichTextToClipboard(html) {
    const tempElement = document.createElement('div');
    tempElement.innerHTML = html;
    document.body.appendChild(tempElement); // Append to the body before selecting
    const range = document.createRange();
    range.selectNodeContents(tempElement);
    const selection = window.getSelection();
    selection.removeAllRanges();
    selection.addRange(range);

    document.execCommand('copy');

    selection.removeAllRanges();
    document.body.removeChild(tempElement);
}

function showTooltipOnCopy(event) {
    let target = event.target;
    let copyString = $(target).attr('title');
    let copiedString = $(target).attr('data-copied');

    $(target).attr('title', copiedString);
    $(target).tooltip('enable');
    $(target).tooltip('show');

    setTimeout(function () {
        $(target).tooltip('hide');
        $(target).tooltip('disable');
        $(target).attr('title', copyString);
    }, 3000);
}


function copyToClipBoard(event, selector) {
    var richText = $(selector).html();  
    copyRichTextToClipboard(richText);
    showTooltipOnCopy(event); 
}

