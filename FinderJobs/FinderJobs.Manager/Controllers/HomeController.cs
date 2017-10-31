using System.Web.Mvc;

namespace FinderJobs.Manager.Controllers
{    
    public class HomeController : Controller {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Plano()
        {
            return View();
            //return File("~/views/Index.html", "text/html");
        }

        public ActionResult Habilidade()
        {
            return View();
        }
        public ActionResult Pagamento()
        {
            return View();
        }

        public ActionResult Log()
        {
            return View();
        }
    }
}
