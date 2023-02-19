var DIR_SELECT_EL = document.getElementById("dir-selector");
var absoluteHomeURL = "".concat(location.protocol, "//").concat(location.host).toLowerCase();
var dirs = window.location.href.toLowerCase().replace(absoluteHomeURL + "/viewfiles/", "").split("/");
var path_progression = "";
for (var dirindex = 0; dirindex < dirs.length; dirindex++) {
    dirs[dirindex] = dirs[dirindex].toLowerCase();
    path_progression += "".concat(dirs[dirindex], "/");
    // DIR_SELECT_EL.innerHTML += path_progression + "<br>"
    if (dirs[dirindex].toString() !== "")
        DIR_SELECT_EL.innerHTML += "<a class=\"dir-select\" href=\"/viewfiles/".concat(path_progression, "\">").concat(dirs[dirindex], "/</a>");
}
//# sourceMappingURL=dirselect.js.map