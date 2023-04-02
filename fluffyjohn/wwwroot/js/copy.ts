function copyFile(fPath: string)
{
    var data = { path: fPath };

    alert("Sending data...");

    fetch("https://localhost:7111/fcopy/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },

        body: JSON.stringify(data)
    }).then(res => {
        if (res.ok) {
            location.reload();
        } else { createToast("Coudln't copy item"); }
    });
}