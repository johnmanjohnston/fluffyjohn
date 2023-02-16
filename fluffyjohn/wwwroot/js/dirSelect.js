var DIR_SELECT_EL = document.getElementById("dir-selector");
var absoluteHomeURL = "".concat(location.protocol, "//").concat(location.host);
var dirs = window.location.href.replace(absoluteHomeURL + "/ViewFiles/", "").split("/");
var path_progression = "";
for (var dirindex = 0; dirindex < dirs.length; dirindex++) {
    path_progression += "".concat(dirs[dirindex], "/");
    DIR_SELECT_EL.innerHTML += path_progression + "<br>";
}
//# sourceMappingURL=dirselect.js.map