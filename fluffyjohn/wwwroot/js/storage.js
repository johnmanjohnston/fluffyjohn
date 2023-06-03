function getCookie(keyname) {
    var val = "; ".concat(document.cookie);
    var parts = val.split("; ".concat(keyname, "="));
    if (parts.length === 2)
        return parts.pop().split(";").shift();
}
function eraseCookie(keyname) {
    document.cookie = keyname + "=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;";
}
if (getCookie("toast-content")) {
    var val = getCookie("toast-content");
    if (val.match("upload-success\.")) {
        var fCount = val.split(".")[1];
        createToast("Uploaded ".concat(fCount, " file(s)"));
        eraseCookie("toast-content");
    }
    if (val.match("delete-success\.")) {
        var fName = val.split(".")[1];
        createToast("Deleted ".concat(fName));
        eraseCookie("toast-content");
    }
    if (val.match("invalid-dirname")) {
        createToast("Invalid directory name");
        eraseCookie("toast-content");
    }
    if (val.match("dircreate-exception")) {
        createToast("An error occured when creating that folder");
        eraseCookie("toast-content");
    }
}
//# sourceMappingURL=storage.js.map