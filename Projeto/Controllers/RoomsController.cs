using Microsoft.AspNetCore.Mvc;
using Noitcua.Models;
using System.Diagnostics;

namespace Noitcua.Controllers
{
    public class RoomsController : Controller
    {
        private readonly bdContext _context;

        public RoomsController(bdContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Sales()
        {
            var salas = _context.sala;
            //Console.WriteLine(salas.Count);
            //ViewData["Salas"] = salas;
            return View(salas);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
