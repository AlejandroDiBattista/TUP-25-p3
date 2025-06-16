// Funciones helper para mejorar la experiencia de usuario
window.tiendaHelpers = {
    // Mostrar loading en botones
    setButtonLoading: function(buttonElement, isLoading) {
        if (isLoading) {
            buttonElement.disabled = true;
            buttonElement.setAttribute('data-original-text', buttonElement.innerHTML);
            buttonElement.innerHTML = '<span class="loading-spinner"></span> Cargando...';
        } else {
            buttonElement.disabled = false;
            buttonElement.innerHTML = buttonElement.getAttribute('data-original-text');
        }
    },

    // Smooth scroll to element
    scrollToElement: function(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    },

    // Agregar clase con delay para animaciones
    addClassWithDelay: function(selector, className, delay = 100) {
        setTimeout(() => {
            const elements = document.querySelectorAll(selector);
            elements.forEach(el => el.classList.add(className));
        }, delay);
    },

    // Focus en input con delay
    focusInput: function(selector, delay = 100) {
        setTimeout(() => {
            const input = document.querySelector(selector);
            if (input) {
                input.focus();
            }
        }, delay);
    },

    // Vibración en móviles (si está soportado)
    vibrate: function(pattern = [100]) {
        if (navigator.vibrate) {
            navigator.vibrate(pattern);
        }
    },

    // Mostrar confirmación antes de eliminar
    confirmDelete: function(message = '¿Estás seguro de que quieres eliminar este elemento?') {
        return confirm(message);
    },

    // Copiar texto al portapapeles
    copyToClipboard: function(text) {
        navigator.clipboard.writeText(text).then(() => {
            return true;
        }).catch(() => {
            // Fallback para navegadores más antiguos
            const textArea = document.createElement('textarea');
            textArea.value = text;
            document.body.appendChild(textArea);
            textArea.select();
            const successful = document.execCommand('copy');
            document.body.removeChild(textArea);
            return successful;
        });
    },

    // Disparar evento personalizado para actualización del carrito
    triggerCartUpdate: function() {
        const event = new CustomEvent('cartUpdated');
        window.dispatchEvent(event);
    },    // Escuchar evento de actualización del carrito
    onCartUpdate: function(dotnetHelper) {
        window.addEventListener('cartUpdated', function() {
            dotnetHelper.invokeMethodAsync('UpdateCartCounter');
        });
    },
};

// Variable global para la referencia del MainLayout
window.mainLayoutReference = null;

// Función para establecer la referencia del MainLayout
window.setMainLayoutReference = function(dotnetRef) {
    window.mainLayoutReference = dotnetRef;
};

// Función para actualizar el contador del carrito desde cualquier lugar
window.updateCartCounter = function() {
    if (window.mainLayoutReference) {
        window.mainLayoutReference.invokeMethodAsync('UpdateCartCounter');
    }
};
