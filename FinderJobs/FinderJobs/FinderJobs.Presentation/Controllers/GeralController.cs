using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinderJobs.Site.ViewModels;
using FinderJobs.Application.Interface;
using System.IO;
using FinderJobs.Domain.Entities;
using Uol.PagSeguro.Exception;
using Uol.PagSeguro.Domain;
using Uol.PagSeguro.Constants;
using Uol.PagSeguro.Resources;
using Uol.PagSeguro.Service;

namespace FinderJobs.Site.Controllers
{    
    public class GeralController : Controller
    {
        public ActionResult Log(string tipo)
        {
            try
            {
                var logs = Infra.CrossCutting.LogService.Get(tipo);
                return Json(new { sucesso = true, dados = logs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception erro)
            {
                Infra.CrossCutting.LogService.NotifyException("Geral/Log", erro);
                return Json(new {sucesso = false, dados = "Ocorreu um erro: " + erro.Message }, JsonRequestBehavior.AllowGet);
            }       
        }

        public ActionResult LogDesativar(Guid id)
        {
            try
            {
                var sucesso = Infra.CrossCutting.LogService.Desativar(id);
                return Json(new { sucesso = sucesso }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception erro)
            {
                Infra.CrossCutting.LogService.NotifyException("Geral/Log", erro);
                return Json(new { sucesso = false, dados = "Ocorreu um erro: " + erro.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Erro400()
        {
            return File("~/views/BadRequest.html", "text/html");
        }
        public ActionResult Erro401()
        {
            return File("~/views/Unauthorized.html", "text/html");
        }

        public ActionResult Erro404()
        {
            return File("~/views/NotFound.html", "text/html");
        }

        public ActionResult Erro408()
        {
            return File("~/views/RequestTimeout.html", "text/html");
        }
    }
}
