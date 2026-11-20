using Microsoft.AspNetCore.Mvc;

namespace DSW1_T1_PUELLES_CALDAS_MEYNER.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
