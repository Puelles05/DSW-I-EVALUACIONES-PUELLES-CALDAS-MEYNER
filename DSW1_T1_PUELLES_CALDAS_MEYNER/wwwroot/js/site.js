function confirmarEliminacion(formulario) {
    var titulo = formulario.dataset.titulo;
    return confirm('¿Está seguro de eliminar el libro "' + titulo + '"?\nEsta acción no se puede deshacer.');
}
