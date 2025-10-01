$(window).on('scroll', function () {
    hideOpenedDropdowns();
});

$('.chapters-container').on('scroll', function () {
    hideOpenedDropdowns();
});

function clickDropdownButton($target) {
    let $dropdown = $target.closest('.dropdown');
    let $tr = $target.closest('tr');
    if ($dropdown.hasClass('show')) {
        dropdownIsHidding($target);
        hideOpenedDropdown($dropdown, $tr);
    } else {
        hideOpenedDropdowns();
        dropdownIsShowing($target);
        showDropdown($dropdown, $tr);
    }
}

function showDropdown($dropdown, $tr) {
    let $dropdownMenu = $dropdown.find('.dropdown-menu');
    $dropdown.addClass('show');
    $dropdownMenu.addClass('show');
    if (!$dropdown.hasClass('input-dropdown')) {
        $tr.addClass('grey-background');
    }
    relocateDropdown($dropdown, $dropdownMenu);
}

function relocateDropdown($dropdown, $dropdownMenu) {
    let $td = $dropdown.closest('td');
    let offsets = getDropdownOffsets($td, $dropdownMenu);
    const { top, left } = getPosition($td);

    const viewportHeight = $(window).height();
    const dropdownMenuHeight = $dropdownMenu.outerHeight();
    const dropdownTop = top + offsets.topOffset;

    if (dropdownTop + dropdownMenuHeight + 10 > viewportHeight) {
        $dropdownMenu.css(
            "cssText",
            `left: ${left - offsets.leftOffset}px !important; 
             top: ${top - dropdownMenuHeight}px; 
             position: fixed
             `
        );
    } else {
        $dropdownMenu.css(
            "cssText",
            `left: ${left - offsets.leftOffset}px !important; 
             top: ${top + offsets.topOffset}px; 
             position: fixed
             `
        );
    }

    if ($td) {
        $td.css("z-index", "10");
    }
}

function getDropdownOffsets($td, $dropdownMenu) {
    let dropdownMenuWidth = getWidth($dropdownMenu);
    let tdWidth = getWidth($td);
    let elementsWidthDiff = dropdownMenuWidth - tdWidth;
    let staticLeftOffset = 10;
    let leftOffset = elementsWidthDiff + staticLeftOffset;
    let topOffset = 15;

    return {
        leftOffset,
        topOffset
    }
}

$(document).on('click', function (e) {
    clickOutsideDropdown($(e.target));
});

function clickOutsideDropdown($target) {
    if ($('.fieldsets').length > 0 && !($target.hasClass('dots') && $target.parent().is('.dropdown-matrix'))) {
        hideOpenedDropdowns();
    }
}

function hideOpenedDropdowns() {
    $('.fieldsets .dropdown.show').each(function () {
        let $dropdown = $(this);
        let $tr = $dropdown.closest('tr');
        dropdownIsHidding($(this).find('.dropdown-matrix'));
        hideOpenedDropdown($dropdown, $tr);
    });
}

function hideOpenedDropdown($dropdown, $tr) {
    $dropdown.removeClass('show');
    $dropdownMenu = $dropdown.children('.dropdown-menu');
    $dropdownMenu.removeClass('show');
    $tr.removeClass('grey-background');
    $dropdownMenu
        .css(
            "cssText",
            `left: ; 
                top: ; 
                position: 
                `
        );

    let $td = $dropdown.closest('td');
    if ($td) {
        $td.css("z-index", "");
    }
}

function inactivateAllMatrixFields(inputElements) {
    inputElements.forEach(otherInput => {
        const dropdown = otherInput.querySelector(".dropdown");
        dropdown.style.display = "none";
        const otherTd = otherInput.closest("td");
        clearStyles(otherTd);
    });
}

function clearStyles(otherTd) {
    if (otherTd) {
        otherTd.style.outline = "";
        otherTd.style.border = "";
        otherTd.style.background = "";
    }
}

$('.fieldsets').on('scroll', function () {
    hideOpenedDropdowns();
    const inputElements = document.querySelectorAll(".fieldset-matrix-td");

    inputElements.forEach(input => {
        const dropdown = input.querySelector(".dropdown");
        input.isInputClicked = false;
        dropdown.style.display = "none";
        inactivateAllMatrixFields(inputElements);
    });
});

function initializeInputDropdown() {
    $(document).off('click', '.dropdown-matrix').on('click', '.dropdown-matrix', function (event) {
        event.preventDefault();
        let $target = $(event.currentTarget);
        clickDropdownButton($target);
    });

    const inputElements = document.querySelectorAll(".fieldset-matrix-td");

    inputElements.forEach(input => {
        const dropdown = input.querySelector(".dropdown");
        const td = input;
        input.isInputClicked = false;

        input.addEventListener("mouseenter", function () {
            if (!input.isInputClicked) {
                dropdown.style.display = "block";
                td.style.background = "#F9F9F9";
            }
        });

        input.addEventListener("click", function (event) {
            if (!event.target.closest(".dropdown")) {
                inputElements.forEach(otherInput => {
                    const otherDropdown = otherInput.querySelector(".dropdown");
                    const otherTd = otherInput;

                    if (otherInput !== input) {
                        if (otherDropdown) {
                            otherDropdown.style.display = "none";
                        }
                        clearStyles(otherTd);
                        otherInput.isInputClicked = false;
                    }
                });

                input.isInputClicked = true;
                dropdown.style.display = "block";

                if (!td.querySelector('span.show-missing-value')) {
                    td.style.outline = "1px solid #1C94A3";
                    td.style.outlineOffset = "-1px";
                    td.style.background = "";
                }
                else {
                    td.style.background = "#F9F9F9";
                }
            }
            else {
                inputElements.forEach(otherInput => {
                    const otherDropdown = otherInput.querySelector(".dropdown");
                    const otherTd = otherInput;

                    if (otherInput !== input) {
                        if (otherDropdown) {
                            otherDropdown.style.display = "none";
                        }
                        clearStyles(otherTd);
                        otherInput.isInputClicked = false;
                    }
                });
            }
        });

        input.addEventListener("mouseleave", function () {
            if (!dropdown.classList.contains("show")) {
                if (!input.isInputClicked && !dropdown.matches(":hover")) {
                    dropdown.style.display = "none";
                }
                if (input.isInputClicked && td.querySelector('span.show-missing-value')) {
                    td.style.background = "#F9F9F9";
                }
                else {
                    td.style.background = "";
                }
            }
        });

        dropdown.addEventListener("mouseenter", function () {
            dropdown.style.display = "block";
        });


        document.addEventListener("click", function (event) {
            const isOutsideClick = !event.target.closest(".fieldset-matrix-td") && !event.target.closest(".dropdown");

            if (isOutsideClick) {
                input.isInputClicked = false;
                dropdown.style.display = "none";
                inactivateAllMatrixFields(inputElements);
            }
        });
    });

    $("tr[data-dependables='True']").hover(
        function () {
            $(this).find("td").css("background", "#F9F9F9");
        },
        function () {
            $(this).find("td").css("background", "");
        }
    );
}