//success.js
console.log('success.js loaded');
window.showAlert = {
    successAlert: function (title, message) {
        Swal.fire({
            title: title,
            text: message,
            icon: 'success', // Use 'error' for error icon, 'info' for info icon, etc.
            confirmButtonText: 'OK'
        });
    }
};
