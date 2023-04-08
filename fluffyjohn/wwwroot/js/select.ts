﻿var currentlySelected: string[] = [] // String array of all paths
var normalBG = "";

function handleSelect(path: string, id: string)
{
    if (currentlySelected.indexOf(path) == -1) {
        currentlySelected.push(path);

        normalBG = document.getElementById(id).style.background;
        document.getElementById(id).style.background = "rgba(255, 255, 255, 0.3)";
    } else {
        currentlySelected.splice(currentlySelected.indexOf(path), 1);
        document.getElementById(id).style.background = normalBG;
    }

    console.log(currentlySelected);
}

function selectCopy() {
    var data = { paths: currentlySelected };

    fetch("https://localhost:7111/selectcopy/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },

        body: JSON.stringify(data)
    }).then(res => {
    });
}