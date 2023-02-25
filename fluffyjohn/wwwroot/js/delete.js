// cName -- content name
function confirmDelete(contentName, contentPath, isFile) {
    if (confirm("Are you sure you want to delete ".concat(contentName, "? This is not reversable.")) === true) {
        if (isFile) {
            document.location = "/delfile/".concat(contentPath);
        }
        else {
            document.location = "/deldir/".concat(contentPath);
        }
    }
}
//# sourceMappingURL=delete.js.map