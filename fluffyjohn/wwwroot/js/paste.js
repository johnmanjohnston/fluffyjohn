function paste() {
    var route = window.location.href.replace("https://localhost:7111/viewfiles/", "");
    var data = {
        route: route
    };
    fetch("https://localhost:7111/paste/", {
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
            createToast("Couldn't paste item(s)");
        }
    });
}
//# sourceMappingURL=paste.js.map