if (!localStorage.getItem("drop")) {
    localStorage.setItem("drop", JSON.stringify(true));
}
else {
    if (JSON.parse(localStorage.getItem("drop")) == true) {
        var dropdown = document.getElementById("upload-section");
        dropdown.classList.add("show");
    }
}
function toggleDropdownShowProperty() {
    var dropStatus = JSON.parse(localStorage.getItem("drop"));
    localStorage.setItem("drop", JSON.stringify(!dropStatus));
}
//# sourceMappingURL=uploaddropdown.js.map