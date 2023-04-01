function renameItem(fullPath, isFile) {
    if (isFile === void 0) { isFile = true; }
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
    var MIME_TYPE = 'application/json';
    var fetchURL;
    if (isFile) {
        fetchURL = "https://localhost:7111/renamef";
    }
    else {
        fetchURL = "https://localhost:7111/renamed";
    }
    fetch(fetchURL, {
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