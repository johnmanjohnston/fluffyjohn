function confirmDelete(contentName, contentPath, isFile) {
    if (confirm("Are you sure you want to delete ".concat(decodeURIComponent(contentName), "? This is not reversable.")) === true) {
        var data = {
            path: contentPath,
            isFile: isFile
        };
        fetch("https://localhost:7111/delete/", {
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
                createToast("Couldn't delete item");
            }
        });
    }
}
//# sourceMappingURL=delete.js.map