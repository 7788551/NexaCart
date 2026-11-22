using Microsoft.AspNetCore.Mvc;

namespace NexaCart.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
