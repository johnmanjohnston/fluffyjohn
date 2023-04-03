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

    const MIME_TYPE = 'application/json';

    var fetchURL: string; 

    fetchURL = "https://localhost:7111/rename";


    fetch(fetchURL, {
        method: "POST",
        headers: {
            'Content-Type': MIME_TYPE,
            'Accept': MIME_TYPE,
        },
        body: JSON.stringify(data)

    }).then(res => {
        if (res.ok) {
            location.reload();
        } else { createToast("Couldn't rename item"); }
    });
}