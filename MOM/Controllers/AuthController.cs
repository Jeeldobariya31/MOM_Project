using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Services;
using System.Data;

namespace MOM.Controllers
{
    public class AuthController : Controller
    {
        private readonly DataService _dataService;

        public AuthController()
        {
            try
            {
                _dataService = DataService.Instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing DataService in AuthController: {ex.Message}");
                // Initialize with fallback if needed
                DataService.InitializeWithFallback();
                _dataService = DataService.Instance;
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If user is already logged in, redirect to home
            var userID = HttpContext.Session.GetString("UserID");
            if (!string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Simple validation for demo users
                var validUsers = new Dictionary<string, (string password, string fullName, string email)>
                {
                    { "admin", ("admin123", "System Administrator", "admin@company.com") },
                    { "manager", ("manager123", "Department Manager", "manager@company.com") },
                    { "user", ("user123", "Regular User", "user@company.com") }
                };

                if (validUsers.ContainsKey(model.Username.ToLower()) && 
                    validUsers[model.Username.ToLower()].password == model.Password)
                {
                    var userInfo = validUsers[model.Username.ToLower()];
                    
                    // Set session variables
                    HttpContext.Session.SetString("UserID", "1");
                    HttpContext.Session.SetString("Username", model.Username);
                    HttpContext.Session.SetString("FullName", userInfo.fullName);
                    HttpContext.Session.SetString("Email", userInfo.email);

                    TempData["SuccessMessage"] = $"Welcome back, {userInfo.fullName}!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid username or password. Please use the demo credentials provided.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Login failed: {ex.Message}";
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            ViewBag.Message = "Access Denied. You don't have permission to access this resource.";
            return View();
        }
    }
}
