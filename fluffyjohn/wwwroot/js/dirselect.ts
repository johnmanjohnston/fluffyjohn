const DIR_SELECT_EL: HTMLElement = document.getElementById("dir-selector");
const STORAGE_ROUTE: string = `${location.protocol}//${location.host}`;
const PARENT_DIRS: string[] = window.location.href.replace(STORAGE_ROUTE + "/ViewFiles/", "").split("/");

var pathProgression: string = "";

for (var i = 0; i < PARENT_DIRS.length; i++) {
    pathProgression += PARENT_DIRS[i] + "/";
}

DIR_SELECT_EL.innerText = pathProgression;