function renameFile(fullPath) {
    var brokenPath = fullPath.split("/");
    var filename = brokenPath[brokenPath.length - 1];
    var newName = prompt("What do you want to rename ".concat(filename, " to?"), filename);
    brokenPath[brokenPath.length - 1] = newName;
    var reconstructedPath = "";
    for (var i = 0; i < brokenPath.length; i++) {
        reconstructedPath += brokenPath[i] + "/";
    }
    var data = {
        orginalpath: fullPath,
        newpath: reconstructedPath
    };
    console.log(data);
    var MIME_TYPE = 'application/json';
    fetch("https://localhost:7111/renamef/", {
        method: "POST",
        headers: {
            'Content-Type': MIME_TYPE,
            'Accept': MIME_TYPE,
        },
        body: JSON.stringify(data)
    }).then(function (res) {
        if (res.ok) {
            location.reload();
        }
        else {
            createToast("Couldn't rename file");
        }
    });
}
//# sourceMappingURL=rename.js.map