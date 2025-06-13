function cerrarModal(modalId) {
    var modalElement = document.getElementById(modalId);
    if (modalElement) {
        var modalInstance = bootstrap.Modal.getInstance(modalElement);
        if (modalInstance) {
            modalInstance.hide();
        }
    }
}