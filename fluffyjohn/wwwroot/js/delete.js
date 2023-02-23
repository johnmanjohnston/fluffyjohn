// cName -- content name
function confirmDelete(contentName, fPath, isFile) {
    if (confirm("Are you sure you want to delete ".concat(contentName, "? This is not reversable.")) === true) {
        if (isFile) {
            document.location = "/delfile/".concat(fPath);
        }
    }
}
//# sourceMappingURL=delete.js.map