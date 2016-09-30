using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FinderJobs.MVC.ViewModels;
using FinderJobs.Infra.Data;
using AutoMapper;

namespace FinderJobs.MVC.Controllers
{
    public class AcessoController : Controller
    {

        private readonly UsuarioRepository _usuarioRepository = new UsuarioRepository();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UsuarioViewModel model)
        {
            //var db = new FinderJobsContext();
            //var model = db.Usuarios.Where(usu => usu.Login == usuario.Login && usu.Senha == usuario.Senha).ToList().FirstOrDefault();
            
            var usuario = _usuarioRepository.ValidarAcesso(model.Login, model.Senha);
            if (usuario != null)
            {
                var usuarioViewModel = Mapper.Map<Domain.Entities.Usuario, UsuarioViewModel>(usuario);
                var authenticationTicket = new FormsAuthenticationTicket(usuarioViewModel.Login, false, 60);
                string encryptTicket = FormsAuthentication.Encrypt(authenticationTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptTicket);
                Response.Cookies.Add(authCookie);

                TempData["Usuario"] = usuarioViewModel;
                TempData["Nome"] = usuarioViewModel.Nome;

                if (usuarioViewModel.Tipo.Equals("Empresa"))
                {
                    return RedirectToAction("../Home/IndexEmpresa");
                }
                else
                {
                    return RedirectToAction("../Home/IndexCandidato");
                }
            }
            else
            {
                TempData["mensagemErro"] = "Usuario ou senha invalidos!";
                return View("Index");
            }

        }

        public ActionResult Cadastro(int? id)
        {
            if (id == 1)
            {
                return View("CadastroCandidato");
            }
            else
            {
                return View("CadastroEmpresa");
            }
        }

        [HttpPost]
        public ActionResult Cadastrar(CadastroViewModel model)
        {
            model.Usuario.DataCadastro = DateTime.Now.ToString();
            model.Usuario.Habilidades = string.Empty;
            if (Request.Form["Usuario.Habilidades"] != null)
            {
                var lista = Request.Form["Usuario.Habilidades"].Split(',');
                foreach (var item in lista)
                {
                    if (item.Length > 0)
                    {
                        model.Usuario.Habilidades += item + ",";
                    }
                }
                model.Usuario.Habilidades = model.Usuario.Habilidades.Substring(0, model.Usuario.Habilidades.Length - 1);
            }

            //var db = new FinderJobsContext();
            //db.Usuarios.Add(model.Usuario);
            //var id = db.SaveChanges();

            //if (id > 0)
            //    TempData["mensagem"] = "Cadastro realizado!";
            //else
            //    TempData["mensagem"] = "";

            return View("Cadastro" + model.Usuario.Tipo);
        }
        public RedirectToRouteResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();

            return RedirectToAction("Index");
        }
    }
}
