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
    });
}
//# sourceMappingURL=revert.js.map