using System.Web.Mvc;

namespace FinderJobs.Manager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller {
        public ActionResult Index()
        {
            return View();
        }
    }
}
