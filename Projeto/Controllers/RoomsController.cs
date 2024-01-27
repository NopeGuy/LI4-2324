using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
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
            if (TempData["ModelState"] is ModelStateDictionary modelState)
            {
                ModelState.Merge(modelState);
            }
            return View(salas);
        }

        public IActionResult Exit(int salaId,int userId)
        {
            var id_comprador = _context.comprador.First(c => c.id_user == userId).id;

            var sala = _context.sala.First(s => s.id == salaId);

            if (sala.id_comprador == id_comprador)
            {
                if (sala != null)
                {
                    sala.estado = 2;
                    _context.SaveChanges();
                }
            }
            else
            {
                var ven = _context.vendedor.First(v => v.id_user == userId);
                if (ven != null)
                {
                    if (sala != null)
                    {
                        var vhs = _context.vendedor_has_sala.First(v => v.id_sala == salaId && v.id_vendedor == ven.id);
                        var sql = "DELETE from vendedor_has_sala WHERE id_sala = @salaId AND id_vendedor = @venId";
                        _context.Database.ExecuteSqlRaw(sql,
                                                       new SqlParameter("@salaId", salaId),
                                                                                  new SqlParameter("@venId", ven.id));

                        var uid = userId;
                        var salaIDDD = salaId;
                        var chats = _context.chat.Where(c => c.id_utilizador == userId && c.id_sala == salaId).ToList();
                        foreach(var chat in chats )
                        {
                            _context.chat.Remove(chat);
                        }
                        _context.SaveChanges();
                        
                    }
                }
            }
            return RedirectToAction("Profile", "Account");
        }

        public IActionResult Sold(int salaId,int userId, string handle,float price,string method)
        {
            var comp = _context.comprador.FirstOrDefault(c => c.id_user == userId);
            if (comp == null)
            {
                ModelState.AddModelError(string.Empty, "Comando válido apenas para o comprador da sala.");
                TempData["ModelState"] = ModelState;
                return RedirectToAction("Room", "Rooms", new { id = salaId });
            }

            var userVen = _context.utilizador.FirstOrDefault(v => v.handle == handle);
            if(userVen==null)
            {
                ModelState.AddModelError(string.Empty, "Handle inválida. Por favor utilize uma handle válida.");
                TempData["ModelState"] = ModelState;
                return RedirectToAction("Room", "Rooms", new { id = salaId });
            }

            var venHasSala = _context.vendedor_has_sala.Where(vhs => vhs.id_sala == salaId);
            foreach(var v in venHasSala)
            {
                if (v.id_vendedor != userVen.id) _context.vendedor_has_sala.Remove(v);
            }

            venda venda = new();
            venda.date = DateTime.Now;
            venda.value = price;
            venda.id_sala = salaId;
            venda.id_vendedor = userVen.id;
            venda.payment_method = method;
            venda.verified = true;
            _context.venda.Add(venda);

            sala sala = _context.sala.Find(salaId);
            sala.estado = 1;

            _context.SaveChanges();
            return RedirectToAction("Room", "Rooms", new { id = salaId });
        }

        public IActionResult Create()
        {
            var user = _context.utilizador.Find(HttpContext.Session.GetInt32("Id"));
            if (user == null) return RedirectToAction("Login", "Account");
            comprador comp;
            comp = _context.comprador.FirstOrDefault(c => c.id_user == user.id);
            if (comp == null)
            {
                comprador novo = new();
                novo.id_user = user.id;
                _context.comprador.Add(novo);
                _context.SaveChanges();
                comp = novo;
            }
            ViewData["NumeroSalas"] = _context.sala.Where(s => s.id_comprador == comp.id && s.estado == 0).Count().ToString();
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

            var salas = new List<sala>();
            
            if (isComprador)
            {
                ViewData["IdComp"] = comprador.id;
                var salasAsComprador = await _context.sala
                    .Where(s => s.id_comprador == comprador.id && s.estado != 2)
                    .ToListAsync();
                salas.AddRange(salasAsComprador);
            }

            if (isVendedor)
            {
                ViewData["IdVend"] = vendedor.id;
                var salasAsVendedor = await _context.vendedor_has_sala
                    .Where(vs => vs.id_vendedor == vendedor.id)
                    .Join(_context.sala, vs => vs.id_sala, s => s.id, (vs, s) => s)
                    .Where(s => s.estado != 2)
                    .ToListAsync();
                salas.AddRange(salasAsVendedor);
            }

            salas = salas.Distinct().ToList();

            return View(salas);
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = HttpContext.Session.GetInt32("Id");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var comprador = await _context.comprador.FirstOrDefaultAsync(c => c.id_user == userId);

            if (comprador == null)
            {
                return NotFound("O utilizador ainda não possui nenhuma sala com uma venda em processo ou terminada.");
            }

            var vendaProcesso = await _context.sala.FirstOrDefaultAsync(s => s.id_comprador == comprador.id && s.estado != 0);

            if (vendaProcesso.id == 0)
            {
                return NotFound("O utilizador ainda não possui nenhuma sala com uma venda em processo ou terminada.");
            }

            var salasAsComprador = _context.sala
                    .Where(s => s.id_comprador == comprador.id && s.estado != 0).ToList();


            var vendas = new List<venda>();
            foreach (var salaAtual in salasAsComprador) { 
                int id_sala = salaAtual.id;
                venda Venda = await _context.venda.FirstOrDefaultAsync(v => v.id_sala == id_sala);
                var vendedor = _context.vendedor.FirstOrDefault(v => v.id == Venda.id_vendedor);
                string nome_utilizador = _context.utilizador.FirstOrDefault(u => u.id == vendedor.id_user).handle;
                ViewData["User" + vendedor.id] = nome_utilizador;
                ViewData["Sala" + id_sala] = salaAtual.titulo;
                vendas.Add(Venda);
            }
            return View(vendas);
        }



        [HttpGet]
        public IActionResult Room(int id)
        {
            var user = HttpContext.Session.GetInt32("Id");
            if(user==null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (TempData["ModelState"] is ModelStateDictionary modelState)
            {
                ModelState.Merge(modelState);
            }

            if (_context.sala.Where(s => s.id == id).Count() == 0)
                return RedirectToAction("Sales", "Rooms");

            bool isComp;
            var comprador = _context.comprador.FirstOrDefault(c => c.id_user == user);
            if(comprador==null)
            {
                isComp = false;
            }
            else
            {
                var owner = _context.sala.FirstOrDefault(os => os.id == id && os.id_comprador == comprador.id);
                isComp = owner != null;
            }

            if(!isComp)
            {
                var vendedor = _context.vendedor.FirstOrDefault(v => v.id_user == user);
                if (vendedor == null)
                {
                    vendedor vens = new vendedor();
                    vens.id_user = (int)user;
                    _context.vendedor.Add(vens);
                    _context.SaveChanges();
                }
                var ven = _context.vendedor.FirstOrDefault(v => v.id_user == user);

                var vendedorInSala = _context.vendedor_has_sala.FirstOrDefault(vs => vs.id_sala == id && vs.id_vendedor == ven.id);
                if (vendedorInSala == null)
                {

                    var sql = "INSERT INTO vendedor_has_sala (id_vendedor, id_sala) VALUES (@vendedorId, @salaId)";
                    _context.Database.ExecuteSqlRaw(sql,
                        new SqlParameter("@vendedorId", ven.id),
                        new SqlParameter("@salaId", id));

                }
            }


            ViewData["Id_sala"] = id;
            ViewData["Id_user"] = user;

            // Todas as mensagens deste chat
            var chatMessages = _context.chat.Where(c => c.id_sala == id && (comprador != null ? c.id_utilizador != comprador.id : true));

            // Todos os ids unicos do chat
            var userIds =  chatMessages.Select(c => c.id_utilizador).Distinct().ToList();

            // Handles dos users
            var usersHandles = _context.utilizador
                .Where(u => userIds.Contains(u.id))
                .Select(u => u.handle)
                .ToList();

            // Em Lista
            ViewData["UsersHandles"] = usersHandles;

            ViewData["Last_time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var sala = _context.sala.Where(s => s.id == id).First();
            var compUserId = _context.comprador.Find(sala.id_comprador).id_user;
            var compHandle = _context.utilizador.Find(compUserId).handle;
            ViewData["Handle_Comprador"] = "C " + compHandle;
            var chat = _context.chat.Where(c => c.id_sala == id);
            ViewData["Descricao"] = sala.descricao;


            if (sala != null)
            {
                return View(chat);
            }
            else
            {
                return RedirectToAction("Sales", "Rooms");
            }
        }

        [HttpGet]
        public IActionResult CheckChatChanges(int id, string last_time)
        {
            var data_atual = DateTime.Parse(last_time);
            var chat = _context.chat.Where(c => c.id_sala == id).Where(c => c.data > data_atual).ToList();

            var json = new chatResp();
            json.date_verified = last_time; //TODO: Adicionar 1 segundo

            if (chat.Any())
            {
                json.hasUpdates = true;
            }

            json.error = false;


            return Json(json);


        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string id_utilizador, string id_sala, string msg)
        {
            int salaId = int.Parse(id_sala);
            if (_context.sala.Find(salaId)==null)
            {
                ModelState.AddModelError(string.Empty, "Comprador eliminou a sala.");
                TempData["ModelState"] = ModelState;
                return RedirectToAction("Sales", "Rooms");
            }

            int userId = int.Parse(id_utilizador);
            var venInSala = _context.vendedor.FirstOrDefault(v => v.id_user == userId);
            if(venInSala == null)
            {
                ModelState.AddModelError(string.Empty, "O Comprador já escolheu um vendedor. Foi excluído da sala.");
                TempData["ModelState"] = ModelState;
                return RedirectToAction("Sales", "Rooms");
            }

            chat c = new();
            if (msg[0]=='/')
            {
                msg = msg[1..];
                if (!msg.Any(a => a == '/'))
                {
                    if (msg.ToLower().Equals("exit") && _context.sala.Find(salaId)!=null)
                    {
                        Exit(salaId,userId);
                    }
                }

                else
                {
                    string cmd = msg[..msg.IndexOf("@")];
                    if (cmd == "Sold")
                    {
                        //sold@handle/price/method
                    string handle = msg.Substring(msg.IndexOf("@") + 1, msg.IndexOf("/")); // handle

                    msg = msg[(msg.IndexOf("/") + 1)..]; // price/method

                    string prc = msg[..msg.IndexOf("/")]; //price
                    float price = -1;
                    float.TryParse(prc, out price);
                    if (price < 0)
                    {
                        ModelState.AddModelError(string.Empty, "Valor inválido, por favor utilize um valor positivo com '.' como separador decimal.");
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("Room", "Rooms", new { id = c.id_sala });
                    }

                        string method = msg[(msg.IndexOf("/") + 1)..]; // method
                        Sold(salaId,userId,handle,price,method);
                    }
                }
            }
            else { 
            var nome = _context.utilizador.Where(u => u.id == userId).FirstOrDefault().handle;
            c.mensagem = nome + ": " + msg;
            c.id_sala = salaId;
            c.id_utilizador = userId;
            c.data = DateTime.Now;
            _context.chat.Add(c);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Room", "Rooms", new { id = c.id_sala });
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(sala info)
        {
            if (ModelState.IsValid)
            {
                var user = _context.utilizador.Find(HttpContext.Session.GetInt32("Id"));

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                comprador comp = _context.comprador.FirstOrDefault(a => a.id_user == user.id);

                var numSalas = _context.sala.Where(ns => ns.id_comprador == comp.id  && ns.estado == 0);
                if (numSalas.Count() >= 3)
                {
                    ModelState.AddModelError(string.Empty, "Não pode possuir mais de três salas simultaneamente.");
                    TempData["ModelState"] = ModelState;
                    return RedirectToAction("Sales", "Rooms");
                }

                sala nova = new sala();
                nova.estado = 0;
                nova.titulo = info.titulo;
                nova.descricao = info.descricao;
                nova.id_comprador = comp.id;
                _context.sala.Add(nova);
                await _context.SaveChangesAsync();
                return RedirectToAction("Room", "Rooms", new { id = nova.id });
            }

            ModelState.AddModelError(string.Empty, "Não foi possivel criar a sala...");
            TempData["ModelState"] = ModelState;

            return RedirectToAction("Sales","Rooms");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmDelivery(string id_sala, string id_venda)
        {
            _context.Database.ExecuteSqlRaw("UPDATE sala SET estado = 2 WHERE id = @id_sala",
                               new SqlParameter("@id_sala", id_sala));
            _context.Database.ExecuteSqlRaw("UPDATE venda SET verified = 0 WHERE id = @id_venda",
                                              new SqlParameter("@id_venda", id_venda));
            await _context.SaveChangesAsync();
            return RedirectToAction("History", "Rooms");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}