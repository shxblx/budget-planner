using Microsoft.AspNetCore.Mvc;
using BudgetPlanner.Data;
using System.Security.Cryptography;
using System.Text;
using System;
using BudgetPlanner.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BudgetPlanner.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            var useremail = HttpContext.Session.GetString("UserEmail");
            if (useremail != null) {
              return  RedirectToAction("Dashboard");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            if (_context.Users.Any(x => x.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already registered");
                ViewBag.Error = "Email already registered";
                return View(user);
            }

            user.PasswordHash = HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            _context.SaveChanges();

            return Content($"User saved successfully with Id = {user.Id}");
        }

        public IActionResult Login()
        {

            var useremail = HttpContext.Session.GetString("UserEmail");
            if (useremail != null)
            {
             return    RedirectToAction("Dashboard");
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or Password";
                return View();
            }
            var hashedInput = HashPassword(password);

            if (user.PasswordHash != hashedInput)
            {
                ViewBag.Error = "Incorrect Password";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.FullName),
                new Claim(ClaimTypes.Email,user.Email)
            };

            var identity = new ClaimsIdentity(claims, "BudgetCookie");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("BudgetCookie", principal);

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);
            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.Name = HttpContext.Session.GetString("UserName");

            return View();
        }

        public IActionResult AddExpense()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddExpense(Expense expense)
        {
            var email = HttpContext.Session.GetString("UserEmail");

            if (email == null)
                return RedirectToAction("Login");

            expense.UserEmail = email;

            Console.WriteLine(expense.UserEmail);

            ModelState.Remove("UserEmail");

            if (!ModelState.IsValid)
                return View(expense);

            _context.Expenses.Add(expense);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("BudgetCookie");
            return RedirectToAction("Login");
        }


        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}


