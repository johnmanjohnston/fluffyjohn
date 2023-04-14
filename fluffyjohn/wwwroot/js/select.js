var currentlySelected = []; // String array of all paths
var normalBG = "";
function handleSelect(path, id) {
    if (currentlySelected.indexOf(path) == -1) {
        currentlySelected.push(path);
        normalBG = document.getElementById(id).style.background;
        document.getElementById(id).style.background = "rgba(255, 255, 255, 0.3)";
    }
    else {
        currentlySelected.splice(currentlySelected.indexOf(path), 1);
        document.getElementById(id).style.background = normalBG;
    }
    console.log(currentlySelected);
}
function selectCopy() {
    if (currentlySelected.length == 0) {
        createToast("No items selected");
        return;
    }
    var data = { paths: currentlySelected };
    fetch("https://localhost:7111/selectcopy/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(data)
    }).then(function (res) {
        res.json().then(function (failedCopies) {
            if (failedCopies > 0) {
                createToast("Couldn't copy ".concat(failedCopies, " file(s)"));
            }
        });
    });
}
function deleteCopy() {
    if (currentlySelected.length == 0) {
        createToast("No items selected");
        return;
    }
    var data = { paths: currentlySelected };
    fetch("https://localhost:7111/deletecopy/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(data)
    }).then(function (res) {
        if (res.ok)
            location.reload();
    });
}
//# sourceMappingURL=select.js.map