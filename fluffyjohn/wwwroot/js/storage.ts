﻿function getCookie(keyname) {
    const val = `; ${document.cookie}`;
    const parts = val.split(`; ${keyname}=`);

    if (parts.length === 2)
        return parts.pop().split(";").shift();
}

function eraseCookie(keyname) {
    document.cookie = keyname + "=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;";
}

if (getCookie("toast-content")) {
    var val: string = getCookie("toast-content");

    if (val.match("upload-success\.")) {
        const fCount = val.split(".")[1];
        createToast(`Uploaded ${fCount} file(s)`);
        eraseCookie("toast-content");
    }
}