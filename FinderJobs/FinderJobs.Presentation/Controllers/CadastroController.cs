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

                    var habilidadesModel = (from h in model.Habilidades where h.Id.Equals(h.Nome) select h.Nome).ToList();

                    foreach (var item in habilidadesModel)
                    {
                        var hab = _habilidadeService.BuscarPorNome(item);
                        if (hab == null || hab.Count() == 0)
                            _habilidadeService.Insert(new Habilidade { Nome = item });
                    }

                    var usuario = new Usuario
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
                        Habilidades = (from h in model.Habilidades select h.Nome).ToList(),                        
                        Ativo = true,
                    };

                    if (model.Id == new Guid())
                    {
                        usuario.DataCadastro = DateTime.Now;
                        usuario.Id = (Guid)_usuarioService.Insert(usuario);
                        sucesso = true;
                    }
                    else
                    {
                        usuario.DataAlteracao = DateTime.Now;
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

                if (tipo != ArquivoLocal.Avatar.ToString())
                {
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
                }

                var caminho = string.Concat("~/Arquivo/", usuarioId, "/", tipo, "/");
                var nome = fileUpload.FileName.Length > 50 ? fileUpload.FileName.Substring(fileUpload.FileName.Length - 50) : fileUpload.FileName;

                var arquivo = new Arquivo { UsuarioId = usuarioId, Caminho = caminho, Nome = nome, Tipo = tipo, Ativo = true };
                if (!Directory.Exists(Server.MapPath(caminho)))
                    Directory.CreateDirectory(Server.MapPath(caminho));
                fileUpload.SaveAs(Server.MapPath(caminho) + nome);

                if (tipo == ArquivoLocal.Avatar.ToString())
                    sucesso = _usuarioService.UpdateByField(usuarioId, "UrlAvatar", caminho + nome);
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
                _arquivoService.Disable(arquivoId);

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
                    var arquivo = new Domain.Entities.Arquivo { UsuarioId = usuario.Id, Caminho = caminho, Nome = nome, Tipo = "Boleto", Ativo = true };

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

        [AllowAnonymous]
        public ActionResult Pagamento(string meio)
        {
            bool isSandbox = true;

            EnvironmentConfiguration.ChangeEnvironment(isSandbox);

            try
            {
                AccountCredentials credentials = PagSeguroConfiguration.Credentials(isSandbox);

                // Instantiate a new payment request
                PaymentRequest payment = new PaymentRequest();

                // Sets the currency
                payment.Currency = Currency.Brl;

                // Add an item for this payment request
                payment.Items.Add(new Item("0001", "Assinatura FinderJobs", 1, 100.00m));

                // Sets a reference code for this payment request, it is useful to identify this payment in future notifications.
                payment.Reference = "REFteste1234";

                // Sets your customer information.
                payment.Sender = new Sender(
                    "Fulano Comprador",
                    "comprador@teste.com.br",
                    new Phone("12", "81389219")
                );

                var senderCPF = new SenderDocument(Documents.GetDocumentByType("CPF"), "12345678909");
                payment.Sender.Documents.Add(senderCPF);

                var paymentRedirectUri = payment.Register(credentials);

                return Json(new { sucesso = true, mensagem = paymentRedirectUri }, JsonRequestBehavior.AllowGet);
            }
            catch (PagSeguroServiceException exception)
            {
                var errorMsg = exception.Message + "\n";

                foreach (ServiceError element in exception.Errors)
                {
                    errorMsg += element + "\n";
                }

                return Json(new { sucesso = false, mensagem = errorMsg }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
