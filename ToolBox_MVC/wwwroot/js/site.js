// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function changeServer() {
    let currentForm = document.getElementById("changeServer");
    let currentSelect = document.getElementById("selectServer");
    let newServer = currentSelect.options[currentSelect.selectedIndex].value;

    currentForm.getAttributeNode("action").value += "/" + newServer;
    currentForm.submit();
}

/**
 * Checks all <account-check> elements
 */
function checkAllAccountCheckBoxes() {
    var accountCheckBoxes = document.getElementsByClassName("account-check");

    for (let i = 0; i < accountCheckBoxes.length; i++) {
        accountCheckBoxes[i].checked = true;
    }

    updateViewSelectAll();
}

/**
 * Unchecks all <account-check> elements
 */
function uncheckAllAccountCheckBoxes() {
    var accountCheckBoxes = document.getElementsByClassName("account-check");

    for (let i = 0; i < accountCheckBoxes.length; i++) {
        accountCheckBoxes[i].checked = false;
    }

    updateViewSelectAll();
}

/**
 * Checks the states of all account checkboxes. Returns true if all are checked, false otherwise
 * @returns {boolean}
 */
function verifyAllCheckBoxes() {
    var accountCheckBoxes = document.getElementsByClassName("account-check");

    for (let i = 0; i < accountCheckBoxes.length; i++) {
        if (!accountCheckBoxes[i].checked) {
            return false;
        }
    }

    return true;
}

/**
 * Count the number of checked boxes
 * @returns {int}
 */
function countSelectedBoxes() {
    var accountCheckBoxes = document.getElementsByClassName("account-check");
    var count = 0;

    for (let i = 0; i < accountCheckBoxes.length; i++) {
        if (accountCheckBoxes[i].checked) {
            count += 1;
        }
    } 

    return count;
}

function updateViewSelectAll() {
    var selectAll = document.getElementById("account-select-all");
    var allBoxesChecked = verifyAllCheckBoxes();

    if (allBoxesChecked) {
        selectAll.checked = true;
        selectAll.onclick = uncheckAllAccountCheckBoxes;
    }
    else {
        selectAll.checked = false;
        selectAll.onclick = checkAllAccountCheckBoxes;
    }

    var numberChecked = countSelectedBoxes();
    var numberDisplayers = document.getElementsByClassName("selectionNumber");

    for (let i = 0; i < numberDisplayers.length; i++) {
        numberDisplayers[i].textContent = numberChecked;
    }

    var selectionsButtons = document.getElementsByClassName("selectionButton");

    for (let i = 0; i < selectionsButtons.length; i++) {
        selectionsButtons[i].disabled = (numberChecked == 0);
    }


}