using csharp.Data;
using csharp.Models;
using Microsoft.AspNetCore.Mvc;

namespace csharp.Controllers;

[Route("auth/")]
public class AuthController : Controller {

    private readonly AppDbContext _context;

    public AuthController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    [Route("logout")]
    public ActionResult Logout() {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("login")]
    public ActionResult Login() {
        if (HttpContext.Session.GetInt32("userId") != null){
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [Route("login")]
    public ActionResult LoginProcess(string email, string password){
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            HttpContext.Session.SetString("error", "Os campos não podem estar vazios.");
            return RedirectToAction("Login", "Auth");
        }

        var user = _context.Users.SingleOrDefault(u => u.Email == email);

        if (user == null)
        {
            HttpContext.Session.SetString("error", "Utilizador não encontrado.");
            return RedirectToAction("Login", "Auth");
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            HttpContext.Session.SetString("error", "Password incorreta.");
            return RedirectToAction("Login", "Auth");
        }

        HttpContext.Session.SetInt32("userId", user.Id);
        HttpContext.Session.SetString("username", user.Username);
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    [Route("register")]
    public ActionResult Register() {
        if (HttpContext.Session.GetInt32("userId") != null){
            return RedirectToAction("Index", "Home");
        }

        return View();
    }


    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterProcess(string username, string email, string password){
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            HttpContext.Session.SetString("error", "Os campos não podem estar vazios.");
            return RedirectToAction("RegisterProcess", "Auth");
        }

        var checkUserEmail = _context.Users.SingleOrDefault(u => u.Email == email);

        if (checkUserEmail != null)
        {
            HttpContext.Session.SetString("error", "Um utilizador com este email já existe.");
            return RedirectToAction("RegisterProcess", "Auth");
        }


        var checkUsername = _context.Users.SingleOrDefault(u => u.Username == username);
        if (checkUsername != null)
        {
            HttpContext.Session.SetString("error", "Já existe alguem com este nome de utilizador.");
            return RedirectToAction("RegisterProcess", "Auth");
        }

        var user = new UserModel(
            username: username,
            email: email,
            passwordHash: BCrypt.Net.BCrypt.HashPassword(password),
            ProfileDescription: ""
        );

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        HttpContext.Session.SetInt32("userId", user.Id);
        HttpContext.Session.SetString("username", user.Username);
        
        return RedirectToAction("Index", "Home");
    }

}