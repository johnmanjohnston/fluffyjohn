const DIR_SELECT_EL: HTMLElement = document.getElementById("dir-selector");

const absoluteHomeURL: string = `${location.protocol}//${location.host}`;
const dirs: string[] = window.location.href.replace(absoluteHomeURL + "/viewfiles", "").replace(/#/gm, "").split("/");
var path_progression: string = "";

for (var dirindex = 0; dirindex < dirs.length; dirindex++) {
    path_progression += `${dirs[dirindex]}/`

    if (dirs[dirindex].toString() !== "")
        DIR_SELECT_EL.innerHTML += `<a class="dir-select" href="/viewfiles/${path_progression}">${dirs[dirindex]}/</a>`
}