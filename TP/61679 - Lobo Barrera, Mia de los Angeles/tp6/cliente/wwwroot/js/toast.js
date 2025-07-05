window.mostrarToast = (mensaje) => {
    let toast = document.createElement("div");
    toast.innerText = mensaje;
    toast.style.position = "fixed";
    toast.style.bottom = "30px";
    toast.style.right = "30px";
    toast.style.backgroundColor = "#e10600";
    toast.style.color = "white";
    toast.style.padding = "12px 20px";
    toast.style.borderRadius = "8px";
    toast.style.boxShadow = "0 0 12px rgba(0,0,0,0.3)";
    toast.style.zIndex = "10000";
    toast.style.fontWeight = "600";
    toast.style.fontFamily = "Poppins, sans-serif";
    toast.style.opacity = "0.95";
    toast.style.transition = "opacity 0.5s ease";

    document.body.appendChild(toast);

    setTimeout(() => {
        toast.style.opacity = "0";
        setTimeout(() => document.body.removeChild(toast), 500);
    }, 3000);
};