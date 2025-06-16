window.flyToCart = (imgSelector, carritoSelector) => {
    const img = document.querySelector(imgSelector);
    const carrito = document.querySelector(carritoSelector);
    if (!img || !carrito) return;

    const imgRect = img.getBoundingClientRect();
    const carritoRect = carrito.getBoundingClientRect();

    const clone = img.cloneNode(true);
    clone.classList.add('fly-img');
    clone.style.left = imgRect.left + 'px';
    clone.style.top = imgRect.top + 'px';
    clone.style.width = imgRect.width + 'px';
    clone.style.height = imgRect.height + 'px';
    document.body.appendChild(clone);

    const deltaX = carritoRect.left - imgRect.left;
    const deltaY = carritoRect.top - imgRect.top;

    requestAnimationFrame(() => {
        clone.style.transform = `translate(${deltaX}px, ${deltaY}px) scale(0.2)`;
        clone.style.opacity = '0.2';
    });

    setTimeout(() => {
        clone.remove();
    }, 800);
};