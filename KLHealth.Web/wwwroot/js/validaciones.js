document.addEventListener('DOMContentLoaded', function () {

    // 1. Validación de fortaleza de contraseña en tiempo real
    const passwordInput = document.getElementById('Password');
    const passwordFeedback = document.getElementById('password-feedback');

    if (passwordInput && passwordFeedback) {
        passwordInput.addEventListener('input', function () {
            const password = this.value;
            let errors = [];

            if (password.length < 8) errors.push("Mínimo 8 caracteres.");
            if (!/[A-Z]/.test(password)) errors.push("Al menos una mayúscula.");
            if (!/[0-9]/.test(password)) errors.push("Al menos un número.");
            if (!/[^A-Za-z0-9]/.test(password)) errors.push("Al menos un carácter especial.");

            if (errors.length === 0 && password.length > 0) {
                passwordFeedback.innerHTML = '<span style="color: #166534;"><i class="bi bi-check-circle-fill"></i> Contraseña segura</span>';
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else if (password.length > 0) {
                passwordFeedback.innerHTML = '<span style="color: #dc2626;"><i class="bi bi-exclamation-circle-fill"></i> ' + errors.join(' ') + '</span>';
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            } else {
                passwordFeedback.innerHTML = '';
                this.classList.remove('is-valid', 'is-invalid');
            }
        });
    }

    // 2. Validación de coincidencia de contraseñas
    const confirmPasswordInput = document.getElementById('ConfirmPassword');
    const confirmFeedback = document.getElementById('confirm-password-feedback');

    if (confirmPasswordInput && passwordInput && confirmFeedback) {
        confirmPasswordInput.addEventListener('input', function () {
            if (this.value !== passwordInput.value) {
                confirmFeedback.innerHTML = '<span style="color: #dc2626;"><i class="bi bi-exclamation-circle-fill"></i> Las contraseñas no coinciden.</span>';
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            } else if (this.value.length > 0) {
                confirmFeedback.innerHTML = '<span style="color: #166534;"><i class="bi bi-check-circle-fill"></i> Las contraseñas coinciden.</span>';
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else {
                confirmFeedback.innerHTML = '';
                this.classList.remove('is-valid', 'is-invalid');
            }
        });
    }

    // 3. Validación de fecha mínima (no permitir fechas pasadas)
    const fechaInput = document.getElementById('FechaHoraInicio');
    if (fechaInput) {
        // Obtener fecha actual en formato YYYY-MM-DDTHH:MM
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');
        const minDate = `${year}-${month}-${day}T${hours}:${minutes}`;

        fechaInput.setAttribute('min', minDate);

        fechaInput.addEventListener('change', function () {
            if (this.value < minDate) {
                this.setCustomValidity('No puedes agendar citas en fechas pasadas.');
                this.classList.add('is-invalid');
            } else {
                this.setCustomValidity('');
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            }
        });
    }
});