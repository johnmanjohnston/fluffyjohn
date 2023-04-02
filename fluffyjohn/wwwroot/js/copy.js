function copyFile(fPath) {
    var data = { path: fPath };
    fetch("https://localhost:7111/fcopy/", {
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
            createToast("Coudln't copy item");
        }
    });
}
//# sourceMappingURL=copy.js.map