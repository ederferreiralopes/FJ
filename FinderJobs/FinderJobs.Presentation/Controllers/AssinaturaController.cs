using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinderJobs.MVC.ViewModels;

namespace FinderJobs.MVC.Controllers
{
    public class AssinaturaController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GerarBoleto(UsuarioViewModel usuario)
        {
            var model = new BoletoModel();
            //var ctx = new FinderJobsContext();
            //model = (from b in ctx.Boletos
            //         join c in ctx.Cedentes on b.Cedente.Id equals c.Id
            //         join s in ctx.Sacados on b.Sacado.Id equals s.Id
            //         where c.Id > 0 && s.Id > 0
            //         select new BoletoModel { 
            //         Id = b.Id,
            //         Cedente = c,
            //         Sacado = s

            //         }).FirstOrDefault();

            if (model != null && model.Cedente.Id > 0 && model.Sacado.Id > 0)
                ViewBag.Boleto = new Services.Boleto().GeraBoleto(model);

            return View("Index");
        }
    }
}
