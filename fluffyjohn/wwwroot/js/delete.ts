function confirmDelete(contentName: string, contentPath: string, isFile: boolean) {
    if (confirm(`Are you sure you want to delete ${contentName}? This is not reversable.`) === true) {
        if (isFile) {
            document.location = `/delfile/${contentPath}`;
        } else {
            document.location = `/deldir/${contentPath}`;
        }
    }
}