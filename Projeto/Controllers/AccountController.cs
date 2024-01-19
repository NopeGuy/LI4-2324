using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noitcua.Models;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly bdContext _context;

    public AccountController(bdContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new utilizador());
    }
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string email, string handle)
    {
        var model = new ResetPasswordViewModel { Email = email, Handle = handle };
        return View(model);
    }

    [HttpGet]
    /*public IActionResult Profile()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if(userId != null)
        {

        }
        else
        {
            ModelState.AddModelError(string.Empty, "Session Timeout.");
        }
    }
    */
    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }




    [HttpPost]
    public async Task<IActionResult> Login(utilizador user)
    {
        if (ModelState.IsValid)
        {
            var authenticatedUser = await _context.utilizador
                .FirstOrDefaultAsync(u => u.email == user.email && u.password == user.password);

            if (authenticatedUser != null)
            {
                HttpContext.Session.SetInt32("UserId", user.id);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Dados de login inválidos. Tente Novamente.");
        }

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Register(utilizador newUser)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _context.utilizador
                .FirstOrDefaultAsync(u => u.email == newUser.email || u.handle == newUser.handle);

            if (existingUser == null)
            {
                _context.Add(newUser);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Já existe um utilizador com o mesmo email ou handle.");
            }
        }
        return View(newUser);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.utilizador
                .FirstOrDefaultAsync(u => u.email == model.Email && u.handle == model.Handle);

            if (user != null)
            {
                if (model.Password == model.ConfirmPassword)
                {
                    user.password = model.Password;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Passwords do not match.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid request.");
            }
        }
        return View(model);
    }

}
