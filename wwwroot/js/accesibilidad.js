// 🔁 Mostrar/Ocultar el menú de accesibilidad
function toggleMenu() {
    const menu = document.getElementById("menuAccesibilidad");
    menu.classList.toggle("show");
}

// 🎨 Cambiar tema accesible
function setTheme(theme) {
    document.body.classList.remove("modo-estandar", "modo-alto-contraste", "modo-daltonismo");
    document.body.classList.add(theme);
    localStorage.setItem("modoAccesible", theme);
}

// 🔠 Aumentar o reducir fuente
function cambiarTamanioFuente(cambio) {
    const html = document.documentElement;
    const estilo = window.getComputedStyle(html);
    const actual = parseFloat(estilo.fontSize);
    const nuevo = actual + cambio;

    html.style.fontSize = nuevo + "px";
    localStorage.setItem("tamanoFuente", nuevo);
}

// 🔄 Restaurar accesibilidad por defecto
function resetAccesibilidad() {
    setTheme("modo-estandar");
    document.documentElement.style.fontSize = "16px";
    localStorage.removeItem("modoAccesible");
    localStorage.removeItem("tamanoFuente");
}

// 🚀 Aplicar configuraciones guardadas al cargar
document.addEventListener("DOMContentLoaded", function () {
    const temaGuardado = localStorage.getItem("modoAccesible");
    const fuenteGuardada = localStorage.getItem("tamanoFuente");

    if (temaGuardado) setTheme(temaGuardado);
    if (fuenteGuardada) {
        document.documentElement.style.fontSize = fuenteGuardada + "px";
    }
});
