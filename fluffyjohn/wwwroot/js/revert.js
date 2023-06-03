function revert(orginalFilePath, oldFilePath) {
    console.log(orginalFilePath);
    console.log(oldFilePath);
    var data = {
        currentFilePath: orginalFilePath,
        oldFilePath: oldFilePath
    };
    fetch("https://localhost:7111/revert", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
        },
        body: JSON.stringify(data)
    }).then(function (res) {
        if (res.ok) {
            createToast("File successfully reverted");
        }
        else if (res.status == 404) {
            createToast("Previous file version not found");
        }
        else {
            createToast("Something went wrong when trying to revert (HTTP ".concat(res.status, ")"));
        }
    });
}
//# sourceMappingURL=revert.js.map