using Microsoft.AspNetCore.Mvc;

namespace csharp.Controllers
{
    public class testController : Controller
    {
        public ActionResult Index()
        {
            return Json(new { message = "Hello World!" });
        }
    }
}