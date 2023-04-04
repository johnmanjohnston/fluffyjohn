function confirmDelete(contentName: string, contentPath: string, isFile: boolean) {
    if (confirm(`Are you sure you want to delete ${contentName}? This is not reversable.`) === true) {

        let data = {
            path: contentPath,
            isFile: isFile
        }

        fetch("https://localhost:7111/delete/", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            },

            body: JSON.stringify(data)
        }).then(res => {
            if (res.ok) {
                location.reload();
            } else { createToast("Couldn't delete item"); }
        });
    }
}