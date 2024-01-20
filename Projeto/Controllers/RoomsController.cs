using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noitcua.Models;

namespace Noitcua.Controllers
{
    public class RoomsController : Controller
    {
        private readonly bdContext _context;

        public RoomsController(bdContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult RoomCriation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoomCriation(string tit, string desc)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("Id");
                if (userId != null)
                {
                    var userRooms = _context.sala.Where(s => s.id_comprador == userId);
                    if (userRooms.Count() < 3)
                    {
                        // Se o comprador já existe, senão cria um
                        var comprador = await _context.comprador.FirstOrDefaultAsync(c => c.id_user == userId);
                        if (comprador == null)
                        {
                            comprador = new comprador { id_user = (int)userId };
                            _context.comprador.Add(comprador);
                        }
                        // Criar Nova Sala
                        sala newSala = new sala
                        {
                            titulo = tit,
                            descricao = desc,
                            estado = true,
                            id_comprador = comprador.id
                        };

                        _context.sala.Add(newSala);

                        // Adicionar sala ao comprador
                        comprador.sala.Add(newSala);

                        await _context.SaveChangesAsync();

                        return View("MyRooms"); //TODO: Redirecionar para view das "Minhas Salas"
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Não consegue criar mais de 3 salas.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Session timeout.");
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }
    }
    }
}
