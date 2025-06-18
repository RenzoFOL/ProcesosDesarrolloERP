let historialVentas = [
    { idProducto: "P001", cantidad: 3, fecha: "2024-05-15" },
    { idProducto: "P002", cantidad: 2, fecha: "2024-05-16" },
    { idProducto: "P003", cantidad: 5, fecha: "2024-05-17" }
]; // ⚠️ Simulado por ahora

function renderizarHistorial(filtrado = historialVentas) {
    const tabla = document.getElementById("tablaHistorial");
    tabla.innerHTML = "";

    filtrado.forEach(item => {
        const fila = `
            <tr>
                <td>${item.idProducto}</td>
                <td>${item.cantidad}</td>
                <td>${item.fecha}</td>
            </tr>
        `;
        tabla.innerHTML += fila;
    });
}

function filtrarHistorial() {
    const busqueda = document.getElementById("buscarHistorial").value.trim().toLowerCase();

    if (!busqueda) {
        renderizarHistorial();
        return;
    }

    const filtrado = historialVentas.filter(item =>
        item.idProducto.toLowerCase().includes(busqueda)
    );

    renderizarHistorial(filtrado);
}

// Inicial
document.addEventListener("DOMContentLoaded", () => {
    renderizarHistorial();
});
