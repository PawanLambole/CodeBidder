using Microsoft.AspNetCore.Mvc;

namespace CodeBidder.Controllers
{
    public class DeveloperDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
