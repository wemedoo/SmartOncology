function scrollArrows(arrow, container) {
    if (arrow == 'left-arrow') {
        $(`#${container}`).animate({
            scrollLeft: "-=500px"
        }, "slow");
    } else if (arrow == 'right-arrow') {
        $(`#${container}`).animate({
            scrollLeft: "+=500px"
        }, "slow");
    }
}