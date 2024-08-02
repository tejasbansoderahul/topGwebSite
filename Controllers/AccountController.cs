using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Top_G_Web.Entities;
using Top_G_Web.Models;

namespace Top_G_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }


        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserAccount account = new UserAccount();

                account.Email = model.Email;
                account.FullName = model.FullName;
                account.Gender = model.Gender;
                account.Age = model.Age;
                account.Password = model.Password;


                try
                {
                    _context.UserAccounts.Add(account);
                    _context.SaveChanges();

                    ModelState.Clear();
                    ViewBag.Message = $"{account.FullName}Registered successfully,Pls LogIn";

                }
                catch (DbUpdateConcurrencyException ex)
                {

                    ModelState.AddModelError("","Pls Enter Email and Password");
                    return View(model);
                }
                // Optionally, you might want to redirect to another action or display a success message
                return View();
            }

            return View(model);
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
                // Check email and password
                var user = _context.UserAccounts
                    .FirstOrDefault(x => x.Password == model.Password && x.Email == model.Email);

                if (user != null)
                {
                    // Generate cookie
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("Name", user.FullName),
                new Claim("Password", user.Password)
            };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)).Wait();

                    return RedirectToAction("CourseDetails","Info");
                }
                else
                {
                    ModelState.AddModelError("", "Email or password not correct");
                }
            }
            return View(model);
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction("Index");
        }

        
    }
}
