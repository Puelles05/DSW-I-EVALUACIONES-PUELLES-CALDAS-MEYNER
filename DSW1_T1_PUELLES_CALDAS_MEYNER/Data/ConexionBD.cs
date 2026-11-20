using Microsoft.Data.SqlClient;

namespace DSW1_T1_PUELLES_CALDAS_MEYNER.Data;

public class ConexionBD
{
    private readonly string _cadena;

    public ConexionBD(IConfiguration configuration)
    {
        _cadena = configuration.GetConnectionString("BDBIBLIOTECA2026")
                  ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'BDBIBLIOTECA2026'.");
    }

    public SqlConnection ObtenerConexion() => new SqlConnection(_cadena);

    public bool ProbarConexion(out string mensaje)
    {
        try
        {
            using var cn = ObtenerConexion();
            cn.Open();
            mensaje = $"Conexión exitosa a '{cn.Database}' en '{cn.DataSource}'.";
            return true;
        }
        catch (SqlException ex)
        {
            mensaje = $"Error de SQL Server: {ex.Message}";
            return false;
        }
        catch (Exception ex)
        {
            mensaje = $"Error inesperado: {ex.Message}";
            return false;
        }
    }
}
