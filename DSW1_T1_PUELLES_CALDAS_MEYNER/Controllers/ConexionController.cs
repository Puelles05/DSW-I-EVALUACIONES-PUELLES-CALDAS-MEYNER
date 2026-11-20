using Microsoft.AspNetCore.Mvc;
using DSW1_T1_PUELLES_CALDAS_MEYNER.Data;

namespace DSW1_T1_PUELLES_CALDAS_MEYNER.Controllers;

public class ConexionController : Controller
{
    private readonly ConexionBD _bd;

    public ConexionController(ConexionBD bd)
    {
        _bd = bd;
    }

    public IActionResult Index()
    {
        ViewBag.Exito = _bd.ProbarConexion(out string mensaje);
        ViewBag.Mensaje = mensaje;
        return View();
    }
}
