using FinderJobs.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FinderJobs.MVC.Controllers
{
    public class VagasController : Controller
    {
        //
        // GET: /Vagas/

        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult CadastrarVaga(VagaModel model)
        //{
        //    try
        //    {
        //        model.DataCadastro = DateTime.Now;

        //        VagaDB vagaBanco = new VagaDB();

        //        vagaBanco.Vaga.Add(model);
        //        vagaBanco.SaveChanges();

        //        TempData["mensagem"] = "Realizado!";
        //        return View("VagaCadastro");
        //    }
        //    catch (Exception erro)
        //    {
        //        TempData["mensagem"] = "Ocorreu um erro!";
        //        throw erro;
        //    }
        //}

        //metodos json
        [HttpPost]
        public ActionResult CadastrarVagaJson(VagaViewModel model)
        {
            try
            {
                var vaga = new VagaViewModel();

                vaga.IdEmpresa = model.IdEmpresa;
                vaga.DataCadastro = DateTime.Now;
                vaga.Empresa = model.Empresa;
                vaga.Descricao = model.Descricao;
                vaga.Cep = model.Cep;                 

                if (model.Habilidades != null && model.Habilidades.Length > 0)
                {
                    var lista = model.Habilidades.Split(',');
                    foreach (var item in lista)
                    {
                        if (item.Length > 0)
                        {
                            vaga.Habilidades += item + ",";
                        }
                    }
                    vaga.Habilidades = vaga.Habilidades.Substring(0, vaga.Habilidades.Length - 1);
                }

                //var db = new FinderJobsContext();
                //db.Vagas.Add(vaga);
                //db.SaveChanges();

                var resultado = new object();
                resultado = new { mensagem = "Sucesso" };

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var resultado = new object();
                resultado = new { mensagem = "Ocorreu um erro ao tentar gravar a vaga" };

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListarCliente(string filtro)
        {
            try
            {
                var lista = new List<UsuarioViewModel>()
                {
                    new UsuarioViewModel { Nome = "Eder Lopes0", RgCnpj = "123112313123" },
                    new UsuarioViewModel { Nome = "Eder Lopes1", RgCnpj = "4444444" },
                    new UsuarioViewModel { Nome = "Eder Lopes2", RgCnpj = "123112313123" },
                    new UsuarioViewModel { Nome = "Eder Lopes3", RgCnpj = "66666666" },
                    new UsuarioViewModel { Nome = "Eder Lopes4", RgCnpj = "123112313123" },
                    new UsuarioViewModel { Nome = "Eder Lopes5", RgCnpj = "77777777" },
                    new UsuarioViewModel { Nome = "Eder Lopes6", RgCnpj = "3333333333333" },
                    new UsuarioViewModel { Nome = "Eder Lopes7", RgCnpj = "9999999999" },
                };

                if (!string.IsNullOrWhiteSpace(filtro))
                    lista = lista.Where(x => x.Nome.Contains(filtro)).ToList(); 

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var resultado = new object();
                resultado = new { mensagem = "Ocorreu um erro ao tentar gravar a vaga" };

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
