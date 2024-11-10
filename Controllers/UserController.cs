using Microsoft.AspNetCore.Http; // Required for session handling
using Microsoft.AspNetCore.Mvc;
using CodeBidder.Data; // Your database context namespace
using CodeBidder.Models; // Your models namespace
using System.Linq;
using System.Threading.Tasks;

namespace CodeBidder.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        // Constructor that initializes the DbContext
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // Action method for user registration (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Action method to handle registration post request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if the username or email already exists
                var existingUser = _context.Users
                    .Any(u => u.Username == user.Username || u.Email == user.Email);

                if (existingUser)
                {
                    ModelState.AddModelError(string.Empty, "Username or email already in use.");
                    return View(user);
                }

                // Add new user to the Users table
                _context.Users.Add(user);
                await _context.SaveChangesAsync(); // Save changes to the database

                return RedirectToAction("Index", "Home"); // Redirect to home after registration
            }

            return View(user);
        }

        // Action method for user login (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Action method to handle login post request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user, string LoginType)
        {
            // Find the user based on username and password
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            if (existingUser != null)
            {
                // Check if the selected login type matches the user's type in the database
                if (existingUser.UserType != LoginType)
                {
                    // Redirect to NoUser page if the login type does not match
                    return RedirectToAction("NoUser", "User");
                }

                // If login type matches, proceed with login
                TempData["ClientName"] = existingUser.FirstName;

                // Store the user's ID in Session
                HttpContext.Session.SetInt32("UserId", existingUser.Id);

                // Redirect based on the user type
                switch (existingUser.UserType)
                {
                    case "Developer":
                        return RedirectToAction("Index", "DeveloperDashboard");
                    case "Client":
                        return RedirectToAction("Index", "ClientDashboard");
                    case "Admin":
                        return RedirectToAction("Index", "AdminDashboard");
                    default:
                        // Redirect to NoUser if the UserType is invalid or not recognized
                        return RedirectToAction("NoUser", "User");
                }
            }
            else
            {
                // If user not found, redirect to the "NoUser" page
                return RedirectToAction("NoUser", "User");
            }

            // If invalid login attempt, return the login page with an error
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(user);
        }

        // Optional: Action to handle logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear all session data
            return RedirectToAction("Index", "Home");
        }

        // Fallback action in case of invalid user session
        public IActionResult NoUser()
        {
            return View();
        }
    }
}
