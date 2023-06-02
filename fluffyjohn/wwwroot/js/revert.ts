function revert(orginalFilePath: string, oldFilePath: string) {
    console.log(orginalFilePath);
    console.log(oldFilePath);

    const data = {
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

    })
}