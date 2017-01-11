
using System;
using System.IO;
using System.Web.Mvc;
using FinderJobs.Site.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using FinderJobs.Application.Interface;
using FinderJobs.Domain.Entities;

namespace FinderJobs.Site.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUsuarioAppService _usuarioService;
        private readonly IVagaAppService _vagaAppService;        

        public HomeController(IUsuarioAppService usuarioService, IVagaAppService vagaAppService)
        {
            _vagaAppService = vagaAppService;
            _usuarioService = usuarioService;            
        }

        public ActionResult Index(string tipo)
        {
            if (!string.IsNullOrWhiteSpace(tipo))
            {
                if (tipo.Equals("Empresa"))
                    return File("~/views/Empresa.html", "text/html");
                else
                    return File("~/views/Candidato.html", "text/html");
            }
            else
                return File("~/views/Index.html", "text/html");
        }        
    }
}
