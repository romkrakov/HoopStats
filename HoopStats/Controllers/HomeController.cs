using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HoopStats.Models;
using Microsoft.EntityFrameworkCore;

namespace HoopStats.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Users.FirstOrDefault(u => 
                u.Username == model.Username && 
                u.Password == model.Password);

            if (user != null)
            {
                TempData["Action"] = "Login";
                return RedirectToAction("ThankYou", new { id = user.Id });
            }
            
            ModelState.AddModelError("", "שם משתמש או סיסמה שגויים");
        }
        return View(model);
    }
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Email = model.Email,
                    Gender = model.Gender,
                    Password = model.Password
                };

                _context.Users.Add(user);
                _context.SaveChanges();
                TempData["Action"] = "Register";
                return RedirectToAction("ThankYou", new { id = user.Id });
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "שם המשתמש או כתובת האימייל כבר קיימים במערכת");
            }
        }
        return View(model);
    }

    public IActionResult ThankYou(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}