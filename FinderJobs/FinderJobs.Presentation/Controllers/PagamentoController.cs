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
using System.Net;
using System.Text;

namespace FinderJobs.Site.Controllers
{
    public class PagamentoController : Controller
    {        
        private readonly IConfiguracaoBoletoAppService _configuracaoBoletoService;
        private readonly IPagamentoAppService _pagamentoService;        
        private readonly IArquivoAppService _arquivoService;

        public PagamentoController(IConfiguracaoBoletoAppService configuracaoBoletoService,
                                  IPagamentoAppService pagamentoService, IArquivoAppService arquivoService)
        {            
            _configuracaoBoletoService = configuracaoBoletoService;
            _pagamentoService = pagamentoService;            
            _arquivoService = arquivoService;
        }

        public ActionResult Index(int mes, int ano)
        {
            try
            {
                var pagtos = _pagamentoService.GetAll();
                return Json(new { sucesso = true, dados = pagtos }, JsonRequestBehavior.AllowGet);
            }
            catch (PagSeguroServiceException exception)
            {
                var errorMsg = exception.Message + "\n";

                foreach (ServiceError element in exception.Errors)
                {
                    errorMsg += element + "\n";
                }

                return Json(new { sucesso = false, dados = errorMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GerarBoleto(CadastroViewModel cadastro)
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
                Id = cadastro.Id,
                Nome = cadastro.Nome,
                CpfCnpj = cadastro.CpfCnpj,
                Cep = cadastro.Endereco.Cep,
                Endereco = cadastro.Endereco.Logradouro,
                Bairro = cadastro.Endereco.Bairro,
                Cidade = cadastro.Endereco.Cidade,
                UF = cadastro.Endereco.UF
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

                    caminho = "~/Arquivo/" + cadastro.Id + "/Boleto/";
                    nome = boleto.Boleto.NumeroDocumento + ".pdf";
                    byte[] arquivoPDF = boleto.MontaBytesPDF();
                    var arquivo = new Domain.Entities.Arquivo { CadastroId = cadastro.Id, Caminho = caminho, Nome = nome, Tipo = "Boleto", Ativo = true };

                    if (!System.IO.Directory.Exists(Server.MapPath(caminho)))
                        System.IO.Directory.CreateDirectory(Server.MapPath(caminho));

                    FileStream Stream = new FileStream(Server.MapPath(caminho) + nome, FileMode.Create);
                    Stream.Write(arquivoPDF, 0, arquivoPDF.Length);

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

        public ActionResult redirecionar(string transasao)
        {
            return File("~/views/redireciona.html", "text/html");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Notificacao(string notificationType, string notificationCode)
        {
            try
            {
                bool isSandbox = true;
                var credentials = PagSeguroConfiguration.Credentials(isSandbox);
                var url = PagSeguroConfiguration.NotificationUri + "/" + notificationCode;

                var request = (HttpWebRequest)WebRequest.Create(url + "email=" + credentials.Email + "&token=" + credentials.Token);
                var response = request.GetResponse();
                var resposta = string.Empty;
                using (var stream = response.GetResponseStream())
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    resposta = reader.ReadToEnd();
                }

                // atualizar base de dados finderjobs.pagamento   
            }
            catch (Exception erro)
            {
                //new Infra.Log.Gerar().LogErro("Notificação pagseguro: " + notificationCode, erro.Message);
            }

            return null;
        }
    }
}
