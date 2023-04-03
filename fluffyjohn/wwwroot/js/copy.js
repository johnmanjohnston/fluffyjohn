function copy(fPath, isFile) {
    if (isFile === void 0) { isFile = true; }
    var data = { path: fPath, isFile: isFile };
    fetch("https://localhost:7111/copy/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(data)
    }).then(function (res) {
        if (res.ok) {
            location.reload();
        }
        else {
            createToast("Couldn't copy item");
        }
    });
}
//# sourceMappingURL=copy.js.map