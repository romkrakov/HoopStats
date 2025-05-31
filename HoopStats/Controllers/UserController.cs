using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HoopStats.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace HoopStats.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly ApplicationDbContext _context;

    public UserController(ILogger<UserController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
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
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());
                TempData["Action"] = "Login";
                return RedirectToAction("ThankYou", "Home", new { id = user.Id });
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
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());
                TempData["Action"] = "Register";
                return RedirectToAction("ThankYou", "Home", new { id = user.Id });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "שם המשתמש או כתובת האימייל כבר קיימים במערכת");
            }
        }
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult ManageUsers()
    {
        if (!IsAdmin())
        {
            return Forbid();
        }
        
        var users = _context.Users.ToList();
        return View(users);
    }

    [HttpPost]
    public IActionResult DeleteUser(int id)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        var user = _context.Users.Find(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
        
        return RedirectToAction("ManageUsers");
    }

    [HttpPost]
    public IActionResult ToggleAdmin(int id)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        var user = _context.Users.Find(id);
        if (user != null)
        {
            user.IsAdmin = !user.IsAdmin;
            _context.SaveChanges();
        }
        
        return RedirectToAction("ManageUsers");
    }

    public IActionResult EditUser(int id)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }
        
        return View(user);
    }

    [HttpPost]
    public IActionResult EditUser(User model)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            var user = _context.Users.Find(model.Id);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Username = model.Username;
                user.Email = model.Email;
                user.Gender = model.Gender;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.Password = model.Password;
                }
                _context.SaveChanges();
                return RedirectToAction("ManageUsers");
            }
        }
        
        return View(model);
    }

    private bool IsAdmin()
    {
        var isAdminString = HttpContext.Session.GetString("IsAdmin");
        return bool.TryParse(isAdminString, out bool isAdmin) && isAdmin;
    }
}