var DIR_SELECT_EL = document.getElementById("dir-selector");
var absoluteHomeURL = "".concat(location.protocol, "//").concat(location.host);
var dirs = window.location.href.replace(absoluteHomeURL + "/viewfiles", "").split("/");
var path_progression = "";
for (var dirindex = 0; dirindex < dirs.length; dirindex++) {
    path_progression += "".concat(dirs[dirindex], "/");
    if (dirs[dirindex].toString() !== "")
        DIR_SELECT_EL.innerHTML += "<a class=\"dir-select\" href=\"/viewfiles/".concat(path_progression, "\">").concat(dirs[dirindex], "/</a>");
}
//# sourceMappingURL=dirselect.js.map