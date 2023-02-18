var toastContainer = document.getElementById("toast-container");
var isShowingToast = false;
var pendingToasts = new Array();
function createToast(msg) {
    // Add pending toast requests to queue
    if (isShowingToast) {
        pendingToasts.push(msg);
        return;
    }
    var toast = document.createElement("div");
    toastContainer.appendChild(toast);
    isShowingToast = true;
    // Animate and show
    toast.classList.add("johntoast");
    toast.classList.add("toast-slideup");
    toast.innerHTML = msg;
    setTimeout(function () {
        toast.classList.remove("toast-slideup");
        toast.classList.add("toast-slidedown");
        setTimeout(function () {
            isShowingToast = false;
            toastContainer.removeChild(toast);
            // Check if we have pending toasts to display
            if (pendingToasts.length > 0) {
                createToast(pendingToasts[0]);
                pendingToasts.shift();
            }
        }, 250);
    }, 1899);
}
//# sourceMappingURL=toast.js.map