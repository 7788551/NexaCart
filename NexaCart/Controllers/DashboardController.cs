using Microsoft.AspNetCore.Mvc;

namespace NexaCart.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
