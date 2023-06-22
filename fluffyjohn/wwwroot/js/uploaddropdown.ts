if (!localStorage.getItem("drop")) {
    localStorage.setItem("drop", JSON.stringify(true));
}

else {
    if (JSON.parse(localStorage.getItem("drop")) == true) {
        const dropdown = document.getElementById("upload-section");
        dropdown.classList.add("show")
    }
}

function toggleDropdownShowProperty() {
    var dropStatus: boolean = JSON.parse(localStorage.getItem("drop"));
    localStorage.setItem("drop", JSON.stringify(!dropStatus));
}