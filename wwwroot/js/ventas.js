let productosVenta = [];

function agregarProducto() {
    const idProducto = document.getElementById("idProducto").value.trim();
    const cantidad = parseInt(document.getElementById("cantidad").value.trim(), 10);

    if (!idProducto || isNaN(cantidad) || cantidad <= 0) {
        mostrarError("⚠️ Ingresa un ID válido y cantidad mayor a cero.");
        return;
    }

    productosVenta.push({
        idProducto,
        nombre: "Nombre Simulado", // ⚠️ Luego lo reemplazamos cuando consultemos a la base
        cantidad,
        precioVenta: 99.99, // ⚠️ También se reemplaza luego por el precio real
        total: cantidad * 99.99
    });

    renderizarTabla();
    limpiarCampos();
}

function renderizarTabla() {
    const tabla = document.getElementById("tablaVenta");
    tabla.innerHTML = "";

    productosVenta.forEach((prod, index) => {
        const fila = `
            <tr>
                <td>${prod.idProducto}</td>
                <td>${prod.nombre}</td>
                <td>${prod.cantidad}</td>
                <td>$${prod.precioVenta.toFixed(2)}</td>
                <td>$${prod.total.toFixed(2)}</td>
                <td><button class="btn btn-danger btn-sm" onclick="eliminarProducto(${index})">Eliminar</button></td>
            </tr>
        `;
        tabla.innerHTML += fila;
    });
}

function eliminarProducto(index) {
    productosVenta.splice(index, 1);
    renderizarTabla();
}

function finalizarVenta() {
    if (productosVenta.length === 0) {
        mostrarError("⚠️ No hay productos para vender.");
        return;
    }

    alert("✅ Venta realizada exitosamente. (Simulado)");
    productosVenta = [];
    renderizarTabla();
}

function limpiarCampos() {
    document.getElementById("idProducto").value = "";
    document.getElementById("cantidad").value = "";
    ocultarError();
}

function mostrarError(mensaje) {
    const errorDiv = document.getElementById("mensajeError");
    errorDiv.textContent = mensaje;
    errorDiv.classList.remove("d-none");
}

function ocultarError() {
    const errorDiv = document.getElementById("mensajeError");
    errorDiv.classList.add("d-none");
}
