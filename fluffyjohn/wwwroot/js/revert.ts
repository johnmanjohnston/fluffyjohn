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

    }).then(res => {
        if (res.ok) {
            createToast("File successfully reverted");
        } else if (res.status == 404) {
            createToast("Previous file version not found");
        } else {
            createToast(`Something went wrong when trying to revert (HTTP ${res.status})`);
        }
    })
}