namespace DSW1_T1_PUELLES_CALDAS_MEYNER.Models;

public class PrestamoAnio
{
    public int IdPrestamo { get; set; }
    public DateTime FechaPrestamo { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public string ApellidosNombres { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string EstadoPrestamo { get; set; } = string.Empty;
    public decimal Multa { get; set; }
}
