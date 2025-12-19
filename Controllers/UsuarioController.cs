using Microsoft.AspNetCore.Mvc;
using VoteAqui.DTOs;

namespace VoteAqui.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

   
    }
}
