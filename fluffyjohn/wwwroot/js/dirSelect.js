var DIR_SELECT_EL = document.getElementById("dir-selector");
var STORAGE_ROUTE = "".concat(location.protocol, "//").concat(location.host);
var PARENT_DIRS = window.location.href.replace(STORAGE_ROUTE + "/ViewFiles/", "").split("/");
var pathProgression = "";
for (var i = 0; i < PARENT_DIRS.length; i++) {
    pathProgression += PARENT_DIRS[i] + "/";
}
DIR_SELECT_EL.innerText = pathProgression;
//# sourceMappingURL=dirselect.js.map