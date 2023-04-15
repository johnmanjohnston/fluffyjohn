var currentlySelected: string[] = [] // String array of all paths
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
    if (currentlySelected.length == 0) { createToast("No items selected"); return; }

    var data = { paths: currentlySelected };

    fetch("https://localhost:7111/selectcopy/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },

        body: JSON.stringify(data)
    }).then(res => {
        res.json().then(failedCopies => {
            if (Number(failedCopies) > 0) {
                createToast(`Couldn't copy ${failedCopies} item(s)`);
            }
        });
    });
}

function deleteCopy() {
    if (currentlySelected.length == 0) { createToast("No items selected"); return; }

    var data = { paths: currentlySelected };

    fetch("https://localhost:7111/deletecopy/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },

        body: JSON.stringify(data)
    }).then(res => {
        if (res.ok) location.reload();
    });
}