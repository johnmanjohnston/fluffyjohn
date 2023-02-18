var toastContainer: HTMLElement = document.getElementById("toast-container")
var isShowingToast: boolean = false;
var pendingToasts: string[] = new Array();

function createToast(msg: string) {
    // Add pending toast requests to queue
    if (isShowingToast) {
        pendingToasts.push(msg)
        return;
    }

    var toast: HTMLElement = document.createElement("div");
    toastContainer.appendChild(toast);

    isShowingToast = true;

    // Animate and show
    toast.classList.add("johntoast")
    toast.classList.add("toast-slideup")
    toast.innerHTML = msg

    setTimeout(() => {
        toast.classList.remove("toast-slideup")
        toast.classList.add("toast-slidedown")
        setTimeout(() => {
            isShowingToast = false;
            toastContainer!.removeChild(toast);

            // Check if we have pending toasts to display
            if (pendingToasts.length > 0) {
                createToast(pendingToasts[0])
                pendingToasts.shift()
            }
        }, 250)
    }, 1899)
}
