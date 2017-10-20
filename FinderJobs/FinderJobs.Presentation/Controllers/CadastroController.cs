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
using System.Threading;
using Uol.PagSeguro;
using Uol.PagSeguro.Exception;
using Uol.PagSeguro.Domain;
using Uol.PagSeguro.Constants;
using Uol.PagSeguro.Resources;
using Uol.PagSeguro.Service;

namespace FinderJobs.Site.Controllers
{
    [Authorize]
    public class CadastroController : Controller
    {

        private readonly ICadastroAppService _cadastroService;
        private readonly IHabilidadeAppService _habilidadeService;
        private readonly IArquivoAppService _arquivoService;
        private readonly IConfiguracaoBoletoAppService _configuracaoBoletoService;
        private readonly IPlanoAppService _planoService;
        private readonly IEmailAppService _emailService;

        public CadastroController(ICadastroAppService cadastroService, IHabilidadeAppService habilidadeService,
                                  IArquivoAppService arquivoAppService, IConfiguracaoBoletoAppService configuracaoBoletoService,
                                  IPlanoAppService planoService, IEmailAppService emailService)
        {
            _cadastroService = cadastroService;
            _habilidadeService = habilidadeService;
            _arquivoService = arquivoAppService;
            _configuracaoBoletoService = configuracaoBoletoService;
            _planoService = planoService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        public ActionResult Index(string tipo)
        {
            return File("~/views/Cadastro.html", "text/html");
        }

        public ActionResult Get()
        {
            var email = Thread.CurrentPrincipal.Identity.Name;
            var cadastro = _cadastroService.GetByEmail(email);
            if (cadastro != null)
            {
                return Json(new { sucesso = true, dados = cadastro }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { sucesso = false }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Empresa(string tipo)
        {
            return File("~/views/Empresa.html", "text/html");
        }

        [AllowAnonymous]
        public ActionResult Candidato(string tipo)
        {
            return File("~/views/Candidato.html", "text/html");
        }

        [HttpPost]
        public ActionResult Gravar(CadastroViewModel model)
        {
            try
            {
                var retorno = string.Empty;
                var sucesso = false;

                if (!string.IsNullOrWhiteSpace(model.Nome))
                {
                    var habilidadeAtual = string.Empty;
                    foreach (var item in model.Habilidades.OrderBy(x => x.Nome))
                    {
                        if (habilidadeAtual != item.Nome)
                        {
                            var hab = _habilidadeService.BuscarPorNome(item.Nome, true);
                            if (hab == null || hab.Count() == 0)
                                _habilidadeService.Insert(new Habilidade { Nome = item.Nome, Ativo = false });
                        }
                        habilidadeAtual = item.Nome;
                    }

                    var cadastro = new Cadastro
                    {
                        Id = model.Id,
                        CpfCnpj = model.CpfCnpj,
                        Celular = model.Celular,
                        Nome = model.Nome,
                        Plano = model.Plano,
                        Endereco = model.Endereco,
                        Email = model.Email,
                        Anonimo = model.Anonimo,
                        Habilidades = (from h in model.Habilidades select h.Nome).ToList(),
                        Ativo = true,
                        IpOrigem = Request.UserHostAddress
                    };

                    if (model.Id == new Guid())
                    {
                        cadastro.DataCadastro = DateTime.Now;
                        cadastro.Id = (Guid)_cadastroService.Insert(cadastro);
                        cadastro.Plano = _planoService.SearchFor(x => x.Nome == model.Plano.Nome).FirstOrDefault();
                        sucesso = true;
                    }
                    else
                    {
                        var cadastroDb = _cadastroService.GetById(model.Id);
                        if (cadastroDb != null)
                        {
                            cadastro.Plano = cadastroDb.Plano;
                            cadastro.DataCadastro = cadastroDb.DataCadastro;
                            cadastro.DataAlteracao = DateTime.Now;
                            sucesso = _cadastroService.Update(cadastro);
                        }
                    }

                    return Json(new { dados = cadastro, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { sucesso = sucesso, mensagem = "Usuário inválido ou indisponível" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GravarArquivo(string id, string tipo, HttpPostedFileBase fileUpload)
        {
            try
            {
                var cadastroId = Guid.Parse(id);
                var retorno = string.Empty;
                var sucesso = false;

                if (tipo != ArquivoLocal.Avatar.ToString())
                {
                    var planoTipo = (PlanoTipo)Enum.Parse(typeof(PlanoTipo), tipo, true);

                    switch (planoTipo)
                    {
                        case PlanoTipo.Candidato:
                            tipo = ArquivoLocal.Curriculo.ToString();
                            break;
                        case PlanoTipo.Empresa:
                            tipo = ArquivoLocal.Vaga.ToString();
                            break;
                        default:
                            tipo = ArquivoLocal.Indefinido.ToString();
                            break;
                    }
                }

                var caminho = string.Concat("~/Arquivo/", cadastroId, "/", tipo, "/");
                var nome = fileUpload.FileName.Length > 50 ? fileUpload.FileName.Substring(fileUpload.FileName.Length - 50) : fileUpload.FileName;

                var arquivo = new Arquivo { CadastroId = cadastroId, Caminho = caminho, Nome = nome, Tipo = tipo, Ativo = true };
                if (!Directory.Exists(Server.MapPath(caminho)))
                    Directory.CreateDirectory(Server.MapPath(caminho));
                fileUpload.SaveAs(Server.MapPath(caminho) + nome);

                if (tipo == ArquivoLocal.Avatar.ToString())
                    sucesso = _cadastroService.UpdateByField(cadastroId, "UrlAvatar", caminho + nome);
                else
                {
                    arquivo.Id = (Guid)_arquivoService.Insert(arquivo);

                    if (arquivo.Id != new Guid())
                        sucesso = true;
                }

                return Json(new { arquivo = arquivo, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CarregarArquivo(string id)
        {
            try
            {
                var cadastroId = Guid.Parse(id);
                var retorno = string.Empty;
                var sucesso = false;

                var arquivo = _arquivoService.GetArquivo(cadastroId, ArquivoTipo.Curriculo.ToString()).ToList();
                if (arquivo != null && arquivo.Count > 0)
                    sucesso = true;

                return Json(new { arquivo = arquivo, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CarregarTodosArquivos(string id)
        {
            try
            {
                var cadastroId = Guid.Parse(id);
                var retorno = string.Empty;
                var sucesso = false;

                var arquivos = _arquivoService.CarregarTodos(cadastroId).ToList();
                arquivos = (from arquivo in arquivos select new Arquivo { Id = arquivo.Id, Ativo = arquivo.Ativo, Nome = arquivo.Nome, Caminho = arquivo.Caminho, Tipo = arquivo.Tipo }).ToList();
                if (arquivos != null && arquivos.Count > 0)
                    sucesso = true;

                return Json(new { arquivos = arquivos, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExcluirArquivo(string id)
        {
            try
            {
                var arquivoId = Guid.Parse(id);
                _arquivoService.Disable(arquivoId);

                return Json(new { sucesso = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Contatar(Guid idVaga, string tipo)
        {
            try
            {
                var email = new Email() { Ativo = true, TipoDestino = tipo == "Empresa" ? "Candidato" : "Empresa", Remetente = Thread.CurrentPrincipal.Identity.Name, Titulo = "Vaga " + idVaga  };                               
                _emailService.Insert(email);

                return Json(new { sucesso = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult GetDashBoard(int ano)
        {
            if (ano > 2016)
            {
                var dados = new List<object>();
                var dadosCandidato = _cadastroService.GetDashboard("Candidato", new DateTime(ano, 1, 1));
                var dadosEmpresas = _cadastroService.GetDashboard("Empresa", new DateTime(ano, 1, 1));

                dados.Add(new { tipo = "Candidato", dashboard = dadosCandidato });
                dados.Add(new { tipo = "Empresa", dashboard = dadosEmpresas });

                if (dados != null)
                    return Json(new { sucesso = true, dados = dados }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { sucesso = false }, JsonRequestBehavior.AllowGet);
        }
    }
}
