document.getElementById("registerForm").addEventListener("submit", function (event) {
    const email = event.target.Email.value;
    const password = event.target.PasswordHash.value;

    if (!email.includes("@") || password.length < 6) {
        alert("Por favor, ingrese un email válido y una contraseña de al menos 6 caracteres.");
        event.preventDefault();
    }
});

document.getElementById("paymentForm").addEventListener("submit", function (event) {
    const cardNumber = event.target.cardNumber.value;
    const cvv = event.target.cvv.value;

    if (cardNumber.length !== 16 || isNaN(cardNumber)) {
        alert("Número de tarjeta inválido.");
        event.preventDefault();
    }
    if (cvv.length !== 3 || isNaN(cvv)) {
        alert("CVV inválido.");
        event.preventDefault();
    }
});

document.getElementById("searchForm").addEventListener("submit", function (event) {
    const origen = event.target.origen.value.trim();
    const destino = event.target.destino.value.trim();

    if (!origen || !destino) {
        alert("Por favor, ingrese origen y destino.");
        event.preventDefault();
    }
});

document.querySelectorAll("form[action='/Reserva/CancelarReserva']").forEach(form => {
    form.addEventListener("submit", function (event) {
        if (!confirm("¿Estás seguro de que deseas cancelar esta reserva?")) {
            event.preventDefault();
        }
    });
});

