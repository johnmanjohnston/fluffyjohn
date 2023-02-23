                       // cName -- content name
function confirmDelete(contentName: string, fPath: string, isFile: boolean) {
    if (confirm(`Are you sure you want to delete ${contentName}? This is not reversable.`) === true) {
        if (isFile) {
            document.location = `/delfile/${fPath}`;
        }
    }
}