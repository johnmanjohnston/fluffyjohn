function renameItem(fullPath: string, isFile: boolean = true)
{
    var brokenPath = fullPath.split("/");
    var filename: string = brokenPath[brokenPath.length - 1]
    var newName: string = prompt(`What do you want to rename ${filename} to?`, filename);

    if (newName == null) { return; }

    brokenPath[brokenPath.length - 1] = newName;

    var reconstructedPath = "";

    for (var i = 0; i < brokenPath.length; i++)
    {   
        reconstructedPath += brokenPath[i] + "/";
    }

    let data = {
        orginalpath: fullPath,
        newpath: reconstructedPath,
        isFile: isFile
    }

    fetch("https://localhost:7111/rename", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
        },
        body: JSON.stringify(data)

    }).then(res => {
        if (res.ok) {
            location.reload();
        } else { createToast("Couldn't rename item"); }
    });
}