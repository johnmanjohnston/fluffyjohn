var currentlySelected = []; // String array of all paths
var normalBG = "";
function handleSelect(path, id) {
    if (currentlySelected.indexOf(path) == -1) {
        currentlySelected.push(path);
        normalBG = document.getElementById(id).style.background;
        document.getElementById(id).style.background = "red";
    }
    else {
        currentlySelected.splice(currentlySelected.indexOf(path), 1);
        document.getElementById(id).style.background = normalBG;
    }
    console.log(currentlySelected);
}
//# sourceMappingURL=select.js.map