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

namespace FinderJobs.Site.Controllers
{
    [Authorize]
    public class CadastroController : Controller
    {

        private readonly IUsuarioAppService _usuarioService;
        private readonly IHabilidadeAppService _habilidadeService;
        private readonly IArquivoAppService _arquivoService;
        private readonly IConfiguracaoBoletoAppService _configuracaoBoletoService;

        public CadastroController(IUsuarioAppService usuarioService, IHabilidadeAppService habilidadeService, IArquivoAppService arquivoAppService, IConfiguracaoBoletoAppService configuracaoBoletoService)
        {
            _usuarioService = usuarioService;
            _habilidadeService = habilidadeService;
            _arquivoService = arquivoAppService;
            _configuracaoBoletoService = configuracaoBoletoService;
        }

        public ActionResult GetCadastro()
        {
            var email = Thread.CurrentPrincipal.Identity.Name;
            var usuario = _usuarioService.GetByEmail(email);
            if (usuario != null)
            {
                return Json(new { sucesso = true, usuario = usuario }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { sucesso = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Gravar(UsuarioViewModel model)
        {
            try
            {
                var retorno = string.Empty;
                var sucesso = false;

                if (!string.IsNullOrWhiteSpace(model.Nome))
                {                    
                    var habilidadesExistentes = (from h in model.Habilidades
                                                 where h.Nome != h.Id
                                                 select new Habilidade { Id = Guid.Parse(h.Id), Nome = h.Nome }).ToList();

                    // Grava Habilidades Novas
                    foreach (var item in model.Habilidades.Where(x => x.Id.Equals(x.Nome)))
                    {
                        var id = (Guid)_habilidadeService.Insert(new Domain.Entities.Habilidade { Nome = item.Nome });
                        habilidadesExistentes.Add(new Habilidade { Id = id, Nome = item.Nome });
                    }

                    var usuario = new Domain.Entities.Usuario
                    {
                        Id = model.Id,
                        CpfCnpj = model.CpfCnpj,
                        Celular = model.Celular,
                        Nome = model.Nome,
                        Pago = model.Pago,
                        Tipo = model.Tipo,
                        Endereco = model.Endereco, 
                        Email = model.Email,
                        Anonimo = model.Anonimo,
                        Habilidades = habilidadesExistentes,                                           
                    };

                    if (model.Id == new Guid())
                    {
                        usuario.DataCadastro = DateTime.Now.ToString();
                        usuario.Id = (Guid)_usuarioService.Insert(usuario);
                        sucesso = true;
                    }
                    else
                    {                        
                        usuario.DataAlteracao = DateTime.Now.ToString();
                        sucesso = _usuarioService.Update(usuario);
                    }                       

                    return Json(new { usuario = usuario, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
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
                var usuarioId = Guid.Parse(id);
                var retorno = string.Empty;
                var sucesso = false;

                var usuarioTipo = (UsuarioTipo)Enum.Parse(typeof(UsuarioTipo), tipo, true);

                switch (usuarioTipo)
                {
                    case UsuarioTipo.Candidato:
                        tipo = ArquivoLocal.Curriculo.ToString();
                        break;
                    case UsuarioTipo.Empresa:
                        tipo = ArquivoLocal.Vaga.ToString();
                        break;
                    default:
                        tipo = ArquivoLocal.Indefinido.ToString();
                        break;
                }

                var caminho = string.Concat("~/Arquivo/", id, "/", tipo, "/");
                var nome = fileUpload.FileName.Length > 50 ? fileUpload.FileName.Substring(fileUpload.FileName.Length - 50) : fileUpload.FileName;

                var arquivo = new Domain.Entities.Arquivo { Usuario = new Domain.Entities.Usuario { Id = usuarioId }, Caminho = caminho, Nome = nome, Tipo = tipo, Ativo = true };
                if (!System.IO.Directory.Exists(Server.MapPath(caminho)))
                    System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                fileUpload.SaveAs(Server.MapPath(caminho) + nome);

                _arquivoService.Desativar(usuarioId, tipo);
                var resultado = _arquivoService.Insert(arquivo);

                if (resultado != null)
                    sucesso = true;

                return Json(new { arquivo = resultado, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
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
                var usuarioId = Guid.Parse(id);
                var retorno = string.Empty;
                var sucesso = false;

                var arquivo = _arquivoService.GetArquivo(usuarioId, ArquivoTipo.Curriculo.ToString()).ToList();
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
                var usuarioId = Guid.Parse(id);
                var retorno = string.Empty;
                var sucesso = false;

                var arquivos = _arquivoService.CarregarTodos(usuarioId).ToList();
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
                _arquivoService.Desativar(arquivoId);

                return Json(new { sucesso = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GerarBoleto(UsuarioViewModel usuario)
        {
            var boleto = new BoletoNet.BoletoBancario();
            var model = new BoletoModel();
            var configuracaoBoleto = _configuracaoBoletoService.GetAll().FirstOrDefault();
            model.Id = configuracaoBoleto.Id;
            model.CodigoBanco = configuracaoBoleto.CodigoBanco;
            model.CodigoCarteira = configuracaoBoleto.CodigoCarteira;
            model.MostrarCodigoCarteira = configuracaoBoleto.MostrarCodigoCarteira;
            model.MostrarComprovanteEntrega = configuracaoBoleto.MostrarComprovanteEntrega;
            model.NumeroDocumento = configuracaoBoleto.NumeroDocumento;
            model.ValorBoleto = configuracaoBoleto.ValorBoleto;
            model.Vencimento = configuracaoBoleto.Vencimento;

            model.Cedente = new CedenteModel
            {
                Id = configuracaoBoleto.Cedente.Id,
                Nome = configuracaoBoleto.Cedente.Nome,
                CpfCnpj = configuracaoBoleto.Cedente.CpfCnpj,
                NossoNumero = configuracaoBoleto.Cedente.NossoNumero,
                Agencia = configuracaoBoleto.Cedente.Agencia,
                Conta = configuracaoBoleto.Cedente.Conta,
                DigitoConta = configuracaoBoleto.Cedente.DigitoConta,
                Codigo = configuracaoBoleto.Cedente.Codigo

            };

            model.Sacado = new SacadoModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                CpfCnpj = usuario.CpfCnpj,
                Cep = usuario.Endereco.Cep,
                Endereco = usuario.Endereco.Logradouro,
                Bairro = usuario.Endereco.Bairro,
                Cidade = usuario.Endereco.Cidade,
                UF = usuario.Endereco.UF
            };

            // criar entidade Assinatura
            // migrar isso para MongoDB
            // depois de gerar o boleto gravar o html no MongoDB

            if (model != null && model.Cedente.Id != new Guid() && model.Sacado.Id != null)
                boleto = new Services.Boleto().GeraBoleto(model);

            var retorno = string.Empty;
            var sucesso = false;
            var idArquivo = 0;
            var caminho = string.Empty; ;
            var nome = string.Empty; ;

            if (boleto != null)
            {
                try
                {

                    caminho = "~/Arquivo/" + usuario.Id + "/Boleto/";
                    nome = boleto.Boleto.NumeroDocumento + ".pdf";
                    byte[] arquivoPDF = boleto.MontaBytesPDF();
                    var arquivo = new Domain.Entities.Arquivo { Usuario = new Domain.Entities.Usuario { Id = usuario.Id }, Caminho = caminho, Nome = nome, Tipo = "Boleto", Ativo = true };

                    if (!System.IO.Directory.Exists(Server.MapPath(caminho)))
                        System.IO.Directory.CreateDirectory(Server.MapPath(caminho));

                    //Crio o arquivo em disco e um fluxo
                    FileStream Stream = new FileStream(Server.MapPath(caminho) + nome, FileMode.Create);
                    Stream.Write(arquivoPDF, 0, arquivoPDF.Length);

                    //_arquivoService.Desativar(id);
                    idArquivo = (int)_arquivoService.Insert(arquivo);

                    if (idArquivo > 0)
                        sucesso = true;
                }
                catch (Exception ex)
                {
                    return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { id = idArquivo, url = caminho + "/" + nome, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
        }

        public RedirectToRouteResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();

            return RedirectToAction("Index");
        }
    }
}
