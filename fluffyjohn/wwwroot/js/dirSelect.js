const STORAGE_ROUTE = `${location.protocol}//${location.host}`;
const PARENT_DIRS = window.location.href.replace(STORAGE_ROUTE + "/ViewFiles/", "").split("/");

const dirSelectorEl = document.getElementById("dir-selector");
var pathProgression = "";

for (var dirIndex = 0; dirIndex < PARENT_DIRS.length; dirIndex++) {
    const dirName = PARENT_DIRS[dirIndex];
    pathProgression += dirName;

    dirSelectorEl.innerHTML += "/" + pathProgression + "/ <br />"
}

