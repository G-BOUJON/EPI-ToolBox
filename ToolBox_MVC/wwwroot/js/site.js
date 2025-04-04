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