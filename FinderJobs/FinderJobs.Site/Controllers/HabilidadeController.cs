using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FinderJobs.Site.ViewModels;
using FinderJobs.Domain.Entities;
using FinderJobs.Application.Interface;

namespace FinderJobs.Site.Controllers
{
    public class HabilidadeController : Controller
    {

        private readonly IHabilidadeAppService _habilidadeService;

        public HabilidadeController(IHabilidadeAppService habilidadeService)
        {
            _habilidadeService = habilidadeService;
        }

        public ActionResult Index(string parametro)
        {
            try
            {
                IEnumerable<Habilidade> habilidades = null;
                if (string.IsNullOrWhiteSpace(parametro))
                    habilidades = _habilidadeService.GetAll();
                else                
                    habilidades = _habilidadeService.BuscarPorNome(parametro);

                var lista = (from h in habilidades select new ViewModels.HabilidadeViewModel { Id = h.Id, Nome = h.Nome }).ToList();

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var resultado = new object();
                resultado = new { mensagem = "Ocorreu um erro: " + ex.Message };

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
