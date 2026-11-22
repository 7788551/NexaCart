using Microsoft.AspNetCore.Mvc;

namespace NexaCart.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
