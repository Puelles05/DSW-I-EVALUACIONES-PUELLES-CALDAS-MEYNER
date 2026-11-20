using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using DSW1_T1_PUELLES_CALDAS_MEYNER.Data;
using DSW1_T1_PUELLES_CALDAS_MEYNER.Models;

namespace DSW1_T1_PUELLES_CALDAS_MEYNER.Controllers;

public class LibroController : Controller
{
    private const int FilasPorPagina = 10;
    private const int AnioMinimo = 1900;
    private const int AnioMaximo = 2100;

    private readonly ConexionBD _bd;

    public LibroController(ConexionBD bd) => _bd = bd;

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is SqlException ex)
        {
            TempData["Error"] = $"No se pudo completar la operación con la base de datos. Detalle: {ex.Message}";
            context.ExceptionHandled = true;
            context.Result = RedirectToAction("Index", "Home");
        }
        base.OnActionExecuted(context);
    }

    private IEnumerable<Libro> ListarLibros(string? buscar = null)
    {
        var lista = new List<Libro>();

        using var cn = _bd.ObtenerConexion();
        using var cmd = new SqlCommand("usp_ListarLibros", cn) { CommandType = CommandType.StoredProcedure };
        if (!string.IsNullOrWhiteSpace(buscar))
            cmd.Parameters.AddWithValue("@Buscar", buscar.Trim());
        cn.Open();

        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            lista.Add(new Libro
            {
                CodLibro = dr.GetString(0).Trim(),
                CodCategoria = dr.GetString(1).Trim(),
                NombreCategoria = dr.GetString(2),
                Titulo = dr.GetString(3),
                ISBN = dr.GetString(4),
                AnioPublicacion = dr.GetInt32(5),
                Stock = dr.GetInt32(6),
                Estado = dr.GetByte(7)
            });
        }
        return lista;
    }

    private IEnumerable<CategoriaLibro> ListarCategorias()
    {
        var lista = new List<CategoriaLibro>();

        using var cn = _bd.ObtenerConexion();
        using var cmd = new SqlCommand("usp_ListarCategorias", cn) { CommandType = CommandType.StoredProcedure };
        cn.Open();

        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            lista.Add(new CategoriaLibro
            {
                CodCategoria = dr.GetString(0).Trim(),
                NombreCategoria = dr.GetString(1),
                Descripcion = dr.IsDBNull(2) ? null : dr.GetString(2)
            });
        }
        return lista;
    }

    private IEnumerable<PrestamoAnio> ListarPrestamosAnio(int anio)
    {
        var lista = new List<PrestamoAnio>();

        using var cn = _bd.ObtenerConexion();
        using var cmd = new SqlCommand("usp_PrestamosPorAnio", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Anio", anio);
        cn.Open();

        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            lista.Add(new PrestamoAnio
            {
                IdPrestamo = dr.GetInt32(0),
                FechaPrestamo = dr.GetDateTime(1),
                FechaDevolucion = dr.IsDBNull(2) ? null : dr.GetDateTime(2),
                ApellidosNombres = dr.GetString(3),
                Titulo = dr.GetString(4),
                EstadoPrestamo = dr.GetString(5),
                Multa = dr.GetDecimal(6)
            });
        }
        return lista;
    }

    private void RegistrarLibro(Libro libro)
    {
        using var cn = _bd.ObtenerConexion();
        using var cmd = new SqlCommand("usp_RegistrarLibro", cn) { CommandType = CommandType.StoredProcedure };
        AgregarParametros(cmd, libro);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    private void ActualizarLibro(Libro libro)
    {
        using var cn = _bd.ObtenerConexion();
        using var cmd = new SqlCommand("usp_ActualizarLibro", cn) { CommandType = CommandType.StoredProcedure };
        AgregarParametros(cmd, libro);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    private void EliminarLibro(string codLibro)
    {
        using var cn = _bd.ObtenerConexion();
        using var cmd = new SqlCommand("usp_EliminarLibro", cn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@CodLibro", codLibro);
        cn.Open();
        cmd.ExecuteNonQuery();
    }

    private static void AgregarParametros(SqlCommand cmd, Libro libro)
    {
        cmd.Parameters.AddWithValue("@CodLibro", libro.CodLibro.Trim());
        cmd.Parameters.AddWithValue("@CodCategoria", libro.CodCategoria.Trim());
        cmd.Parameters.AddWithValue("@Titulo", libro.Titulo.Trim());
        cmd.Parameters.AddWithValue("@ISBN", NormalizarIsbn(libro.ISBN));
        cmd.Parameters.AddWithValue("@AnioPublicacion", libro.AnioPublicacion);
        cmd.Parameters.AddWithValue("@Stock", libro.Stock);
        cmd.Parameters.AddWithValue("@Estado", libro.Estado);
    }

    private static string NormalizarIsbn(string isbn) =>
        new string(isbn.Where(c => c is (>= '0' and <= '9') or 'X' or 'x').ToArray()).ToUpperInvariant();

    private void CargarCategorias()
    {
        ViewBag.Categorias = new SelectList(
            ListarCategorias(),
            nameof(CategoriaLibro.CodCategoria),
            nameof(CategoriaLibro.NombreCategoria));
    }

    private int PaginaDeLibro(string codLibro)
    {
        int posicion = ListarLibros().ToList().FindIndex(l =>
            string.Equals(l.CodLibro, codLibro.Trim(), StringComparison.OrdinalIgnoreCase));
        return posicion < 0 ? 1 : posicion / FilasPorPagina + 1;
    }

    private void AgregarErrorSql(SqlException ex)
    {
        string campo = ex.Number switch
        {
            50001 => nameof(Libro.CodLibro),
            50002 => nameof(Libro.ISBN),
            50003 => nameof(Libro.CodCategoria),
            _ => string.Empty
        };
        ModelState.AddModelError(campo, ex.Message);
    }

    public IActionResult Index(string? buscar, int pagina = 1)
    {
        var libros = ListarLibros(buscar).ToList();

        int total = libros.Count;
        int totalPaginas = (int)Math.Ceiling(total / (double)FilasPorPagina);
        pagina = Math.Clamp(pagina, 1, Math.Max(totalPaginas, 1));

        var filas = libros
            .Skip((pagina - 1) * FilasPorPagina)
            .Take(FilasPorPagina)
            .ToList();

        ViewData["Titulo"] = "Mantenimiento de libros";
        ViewBag.Buscar = buscar;
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = totalPaginas;
        ViewBag.Total = total;
        ViewBag.Desde = total == 0 ? 0 : (pagina - 1) * FilasPorPagina + 1;
        ViewBag.Hasta = (pagina - 1) * FilasPorPagina + filas.Count;

        return View(filas);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        CargarCategorias();
        return View(new Libro { AnioPublicacion = DateTime.Now.Year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Libro modelo)
    {
        if (ModelState.IsValid)
        {
            try
            {
                RegistrarLibro(modelo);
                TempData["Exito"] = $"Libro '{modelo.Titulo.Trim()}' registrado correctamente.";
                return RedirectToAction("Index", new { pagina = PaginaDeLibro(modelo.CodLibro) });
            }
            catch (SqlException ex) when (ex.Number >= 50000)
            {
                AgregarErrorSql(ex);
            }
        }

        CargarCategorias();
        return View(modelo);
    }

    [HttpGet]
    public IActionResult Editar(string id)
    {
        var libro = ListarLibros().FirstOrDefault(l => string.Equals(l.CodLibro, id, StringComparison.OrdinalIgnoreCase));
        if (libro == null)
        {
            TempData["Info"] = $"No se encontró el libro con código '{id}'.";
            return RedirectToAction("Index");
        }

        CargarCategorias();
        return View(libro);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Libro modelo)
    {
        if (ModelState.IsValid)
        {
            try
            {
                ActualizarLibro(modelo);
                TempData["Exito"] = $"Libro '{modelo.Titulo.Trim()}' actualizado correctamente.";
                return RedirectToAction("Index", new { pagina = PaginaDeLibro(modelo.CodLibro) });
            }
            catch (SqlException ex) when (ex.Number >= 50000)
            {
                AgregarErrorSql(ex);
            }
        }

        CargarCategorias();
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(string id, int pagina = 1, string? buscar = null)
    {
        try
        {
            EliminarLibro(id);
            TempData["Exito"] = $"Libro '{id}' eliminado correctamente.";
        }
        catch (SqlException ex) when (ex.Number >= 50000)
        {
            TempData["Info"] = ex.Message;
        }
        return RedirectToAction("Index", new { buscar, pagina });
    }

    [HttpGet]
    public IActionResult PrestamosPorAnio(int? anio, int pagina = 1)
    {
        int anioConsulta = anio ?? DateTime.Now.Year;
        var prestamos = new List<PrestamoAnio>();

        if (anioConsulta < AnioMinimo || anioConsulta > AnioMaximo)
        {
            ViewBag.Error = $"Ingrese un año válido entre {AnioMinimo} y {AnioMaximo}.";
        }
        else
        {
            prestamos = ListarPrestamosAnio(anioConsulta).ToList();
        }

        int total = prestamos.Count;
        int totalPaginas = (int)Math.Ceiling(total / (double)FilasPorPagina);
        pagina = Math.Clamp(pagina, 1, Math.Max(totalPaginas, 1));

        var filas = prestamos
            .Skip((pagina - 1) * FilasPorPagina)
            .Take(FilasPorPagina)
            .ToList();

        ViewBag.Anio = anioConsulta;
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = totalPaginas;
        ViewBag.Total = total;
        ViewBag.Desde = total == 0 ? 0 : (pagina - 1) * FilasPorPagina + 1;
        ViewBag.Hasta = (pagina - 1) * FilasPorPagina + filas.Count;

        return View(filas);
    }
}