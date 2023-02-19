﻿const DIR_SELECT_EL: HTMLElement = document.getElementById("dir-selector");

const absoluteHomeURL: string = `${location.protocol}//${location.host}`.toLowerCase();
const dirs: string[] = window.location.href.toLowerCase().replace(absoluteHomeURL + "/viewfiles/", "").split("/");
var path_progression: string = "";

for (var dirindex = 0; dirindex < dirs.length; dirindex++) {
    dirs[dirindex] = dirs[dirindex].toLowerCase();
    path_progression += `${dirs[dirindex]}/`
    // DIR_SELECT_EL.innerHTML += path_progression + "<br>"

    if (dirs[dirindex].toString() !== "")
        DIR_SELECT_EL.innerHTML += `<a class="dir-select" href="/viewfiles/${path_progression}">${dirs[dirindex]}/</a>`
}