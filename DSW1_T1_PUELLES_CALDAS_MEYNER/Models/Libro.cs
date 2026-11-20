using System.ComponentModel.DataAnnotations;

namespace DSW1_T1_PUELLES_CALDAS_MEYNER.Models;

public class Libro
{
    [Required(ErrorMessage = "El código del libro es obligatorio")]
    [RegularExpression("^[A-Za-z0-9]{6}$", ErrorMessage = "El código debe tener 6 caracteres alfanuméricos (ej. LIB013)")]
    [Display(Name = "Código")]
    public string CodLibro { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione una categoría")]
    [Display(Name = "Categoría")]
    public string CodCategoria { get; set; } = string.Empty;

    [Display(Name = "Categoría")]
    public string? NombreCategoria { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(120, ErrorMessage = "El título no puede superar los 120 caracteres")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ISBN es obligatorio")]
    [RegularExpression(@"^(?:[0-9][ -]?){9}[0-9Xx]$|^(?:[0-9][ -]?){12}[0-9]$",
        ErrorMessage = "El ISBN debe tener 10 o 13 dígitos (se permiten guiones)")]
    [Display(Name = "ISBN")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "El año de publicación es obligatorio")]
    [Range(1450, 2100, ErrorMessage = "El año de publicación debe estar entre 1450 y 2100")]
    [Display(Name = "Año de publicación")]
    public int AnioPublicacion { get; set; }

    [Required(ErrorMessage = "El stock es obligatorio")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    [Range(0, 1, ErrorMessage = "Seleccione un estado válido")]
    public byte Estado { get; set; } = 1;
}
