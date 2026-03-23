using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MOM.Services;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly DataService? _dataService;

        public AccountController()
        {
            try
            {
                _dataService = DataService.Instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing DataService in AccountController: {ex.Message}");
                _dataService = null;
            }
        }

        // GET: /Account/Profile
        public IActionResult Profile()
        {
            try
            {
                var username = HttpContext.Session.GetString("Username") ?? "user";
                var fullName = HttpContext.Session.GetString("FullName") ?? "User";
                var email = HttpContext.Session.GetString("Email") ?? "user@company.com";
                
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var userModel = new UserModel
                {
                    Username = username,
                    FullName = fullName,
                    Email = email,
                    IsActive = true,
                    LastLogin = DateTime.Now.AddHours(-2), // Mock last login
                    Created = DateTime.Now.AddMonths(-6) // Mock creation date
                };

                return View(userModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Profile action: {ex.Message}");
                
                // Return with fallback data
                var fallbackModel = new UserModel
                {
                    Username = "user",
                    FullName = "User",
                    Email = "user@company.com",
                    IsActive = true,
                    Created = DateTime.Now
                };
                
                ViewBag.ErrorMessage = "Unable to load complete profile information.";
                return View(fallbackModel);
            }
        }

        // GET: /Account/Settings
        public IActionResult Settings()
        {
            try
            {
                var username = HttpContext.Session.GetString("Username") ?? "user";
                var fullName = HttpContext.Session.GetString("FullName") ?? "User";
                var email = HttpContext.Session.GetString("Email") ?? "user@company.com";
                
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var userModel = new UserModel
                {
                    Username = username,
                    FullName = fullName,
                    Email = email,
                    IsActive = true,
                    LastLogin = DateTime.Now.AddHours(-2),
                    Created = DateTime.Now.AddMonths(-6)
                };

                return View(userModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Settings action: {ex.Message}");
                
                var fallbackModel = new UserModel
                {
                    Username = "user",
                    FullName = "User",
                    Email = "user@company.com",
                    IsActive = true,
                    Created = DateTime.Now
                };
                
                ViewBag.ErrorMessage = "Unable to load settings.";
                return View(fallbackModel);
            }
        }

        // POST: /Account/UpdateProfile
        [HttpPost]
        public IActionResult UpdateProfile(UserModel model)
        {
            try
            {
                if (ModelState.IsValid && model != null)
                {
                    // Update session with new values
                    HttpContext.Session.SetString("FullName", model.FullName ?? "User");
                    HttpContext.Session.SetString("Email", model.Email ?? "user@company.com");
                    
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("Profile");
                }
                
                return View("Settings", model ?? new UserModel());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                ViewBag.ErrorMessage = "Unable to update profile at this time.";
                return View("Settings", model ?? new UserModel());
            }
        }
    }
}
