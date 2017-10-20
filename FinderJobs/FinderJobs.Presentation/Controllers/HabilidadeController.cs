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

        public ActionResult Index(string parametro, bool ativo = true)
        {
            try
            {                
                var habilidades = _habilidadeService.BuscarPorNome(parametro, ativo);

                var lista = (from h in habilidades select new ViewModels.HabilidadeViewModel { Id = h.Id.ToString(), Nome = h.Nome, Ativo = h.Ativo }).ToList();

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var resultado = new object();
                resultado = new { mensagem = "Ocorreu um erro: " + ex.Message };

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult Salvar(Guid? id, string nome, bool ativo)
        {
            try
            {
                if (id.Value == new Guid())
                {
                    var habilidade = _habilidadeService.Insert(new Habilidade { Id = id.Value, Nome = nome, Ativo = ativo });

                    return Json(new { sucesso = true, habilidade = habilidade }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var habilidade = _habilidadeService.Update(new Habilidade { Id = id.Value, Nome = nome, Ativo = ativo });

                    return Json(new { sucesso = true, dados = habilidade }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var resultado = new object();
                resultado = new { sucesso = false, mensagem = "Ocorreu um erro: " + ex.Message };

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDashBoard()
        {
            var dashBoard = _habilidadeService.GetDashboard();                            

            if (dashBoard != null)
            {
                return Json(new { sucesso = true, dados = dashBoard }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { sucesso = false }, JsonRequestBehavior.AllowGet);
        }

    }
}
