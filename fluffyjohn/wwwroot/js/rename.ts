function renameFile(fullPath: string)
{
    var brokenPath = fullPath.split("/");
    var filename: string = brokenPath[brokenPath.length - 1]
    var newName: string = prompt(`What do you want to rename ${filename} to?`, filename);
    brokenPath[brokenPath.length - 1] = newName;

    var reconstructedPath = "";

    for (var i = 0; i < brokenPath.length; i++)
    {   
        reconstructedPath += brokenPath[i] + "/";
    }

    let data = {
        orginalpath: fullPath,
        newpath: reconstructedPath
    }

    console.log(data);

    const MIME_TYPE = 'application/json';

    fetch("https://localhost:7111/renamef/", {
        method: "POST",
        headers: {
            'Content-Type': MIME_TYPE,
            'Accept': MIME_TYPE,
        },
        body: JSON.stringify(data)

    }).then(res => {
        console.log("Request complete! response:", res);
    });
}