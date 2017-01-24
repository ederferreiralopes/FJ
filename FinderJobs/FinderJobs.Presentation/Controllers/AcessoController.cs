using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FinderJobs.Site.ViewModels;
using Newtonsoft.Json;
using FinderJobs.Application.Interface;
using System.IO;
using FinderJobs.Domain.Entities;
using System.Net;
using System.Text;

namespace FinderJobs.Site.Controllers
{
    public class AcessoController : Controller
    {

        private readonly IUsuarioAppService _usuarioService;
        private readonly IHabilidadeAppService _habilidadeService;
        private readonly IArquivoAppService _arquivoService;
        private readonly IConfiguracaoBoletoAppService _configuracaoBoletoService;

        public AcessoController(IUsuarioAppService usuarioService, IHabilidadeAppService habilidadeService, IArquivoAppService arquivoAppService, IConfiguracaoBoletoAppService configuracaoBoletoService)
        {
            _usuarioService = usuarioService;
            _habilidadeService = habilidadeService;
            _arquivoService = arquivoAppService;
            _configuracaoBoletoService = configuracaoBoletoService;
        }

        public ActionResult Index()
        {
            return File("~/views/Index.html", "text/html");
        }


        public ActionResult Cadastro()
        {
            return File("~/views/Cadastro.html", "text/html");
        }

        public ActionResult UsuarioDisponivel(string email)
        {
            var disponivel = false;
            var url = "http://localhost:53432/Account/UsuarioDisponivelApi?";            
            var parametros = string.Concat("Email=", email);
            var respostaJson = string.Empty;

            var request = (HttpWebRequest)WebRequest.Create(url + parametros);
            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                var reader = new StreamReader(stream, Encoding.UTF8);
                respostaJson = reader.ReadToEnd();
            }

            var resposta = JsonConvert.DeserializeAnonymousType(respostaJson, new { sucesso = false, mensagem = "" });

            return Json(new { sucesso = resposta.sucesso, mensagem = resposta }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Registrar(string email, string senha)
        {            
            var url = "http://localhost:53432/Account/RegisterApi";
            var urlRetorno = "http://localhost:2175";            
            var respostaJson = string.Empty;            

            ASCIIEncoding encoder = new ASCIIEncoding();
            var dados = JsonConvert.SerializeObject(new { Email = email, Password = senha, Url = urlRetorno });
            byte[] data = encoder.GetBytes(dados);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            request.Expect = "application/json";

            request.GetRequestStream().Write(data, 0, data.Length);

            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                var reader = new StreamReader(stream, Encoding.UTF8);
                respostaJson = reader.ReadToEnd();
            }

            var resposta = JsonConvert.DeserializeAnonymousType(respostaJson, new { sucesso = false, mensagem = "" });

            return Json(new { sucesso = true, mensagem = resposta }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EsqueceuSenha(string email)
        {
            var url = "http://localhost:53432/Account/ForgotPasswordApi?";
            var urlRetorno = "http://localhost:2175";
            var parametros = string.Concat("Email=", email, "&Url=", urlRetorno);
            var respostaJson = string.Empty;

            var request = (HttpWebRequest)WebRequest.Create(url + parametros);            
            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                var reader = new StreamReader(stream, Encoding.UTF8);
                respostaJson = reader.ReadToEnd();
            }

            var resposta = JsonConvert.DeserializeAnonymousType(respostaJson, new { sucesso = false, mensagem = "" });

            return Json(new { sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NovaSenha(string email, string password, string code)
        {
            var respostaJson = string.Empty;
            var url = "http://localhost:53432/Account/ResetPasswordApi";

            ASCIIEncoding encoder = new ASCIIEncoding();
            var dados = JsonConvert.SerializeObject(new { Email = email, Password = password, Code = code });
            byte[] data = encoder.GetBytes(dados); 

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            request.Expect = "application/json";

            request.GetRequestStream().Write(data, 0, data.Length);

            var response = request.GetResponse();


            using (var stream = response.GetResponseStream())
            {
                var reader = new StreamReader(stream, Encoding.UTF8);
                respostaJson = reader.ReadToEnd();
            }

            var resposta = JsonConvert.DeserializeAnonymousType(respostaJson, new { sucesso = false, mensagem = "" });

            return Json(new { sucesso = true }, JsonRequestBehavior.AllowGet);
        }
    }

}
