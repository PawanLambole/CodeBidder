using Microsoft.AspNetCore.Mvc;

namespace CodeBidder.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
