using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            return View(salas);
        }

        public IActionResult Create()
        {
            var user = _context.utilizador.Find(HttpContext.Session.GetInt32("Id"));
            if (user == null) return RedirectToAction("Login", "Account");
            ViewData["NumeroSalas"] = _context.sala.Where(s => s.id_comprador == user.id && s.estado == 0).Count().ToString();
            return View();
        }
                
        [HttpGet]
        public async Task<IActionResult> MyRooms()
        {
            var userId = HttpContext.Session.GetInt32("Id");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var vendedor = await _context.vendedor.FirstOrDefaultAsync(v => v.id_user == userId);
            var comprador = await _context.comprador.FirstOrDefaultAsync(c => c.id_user == userId);

            bool isVendedor = vendedor != null;
            bool isComprador = comprador != null;

            if (!isComprador && !isVendedor)
            {
                return NotFound("O utilizador ainda não participa em nenhuma sala!");
            }

            IQueryable<sala> salasAsComprador = isComprador
                ? _context.sala.Where(s => s.id_comprador == comprador.id && s.estado != 2)
                : Enumerable.Empty<sala>().AsQueryable();

            IQueryable<sala> salasAsVendedor = isVendedor
                ? _context.vendedor_has_sala
                    .Where(vs => vs.id_vendedor == vendedor.id)
                    .Join(_context.sala, vs => vs.id_sala, s => s.id, (vs, s) => s)
                    .Where(s => s.estado != 2)
                : Enumerable.Empty<sala>().AsQueryable();

            var salas = await salasAsComprador.Union(salasAsVendedor).ToListAsync();

            return View(salas);
        }




        [HttpGet]
        public IActionResult View(int id)
        {
            ViewData["Id_sala"] = id;
            var sala = _context.sala.Where(s => s.id == id);

            if ( sala != null ) {
                return View(sala);
            }
            else
            {
                return RedirectToAction("Sales", "Rooms");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(sala info)
        {
            
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("Id");

                if (userId != null)
                {
                    var comprador = _context.comprador.Where(a => a.id_user == (int)userId);
                    if(comprador == null)
                    {
                        comprador novo = new comprador();
                        novo.id_user = (int)userId;
                        _context.comprador.Add(novo);
                        _context.SaveChanges();
                    }

                    sala nova = new sala();
                    nova.estado = 0;
                    nova.titulo = info.titulo;
                    nova.descricao = info.descricao;
                    nova.id_comprador = _context.comprador.FirstOrDefault(a => a.id_user == (int)userId).id;
                    _context.sala.Add(nova);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("View", "Rooms", new { id = nova.id });
                }
                else 
                {
                    return RedirectToAction("Login","Account");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Não foi possivel criar a sala...");
            }
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}