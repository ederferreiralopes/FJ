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
    public class PlanoController : Controller
    {
        private readonly ICadastroAppService _cadastroService;
        private readonly IPlanoAppService _planoService;
        private readonly IPagamentoAppService _pagamentoService;

        public PlanoController(ICadastroAppService cadastroService, IPagamentoAppService pagamentoService, IPlanoAppService planoService)
        {
            _cadastroService = cadastroService;
            _planoService = planoService;
            _pagamentoService = pagamentoService;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            var planos = _planoService.GetAll();
            if (planos != null)
            {
                return Json(new { sucesso = true, dados = planos }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { sucesso = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Gravar(Plano model)
        {
            try
            {
                var retorno = string.Empty;
                var sucesso = false;

                var plano = new Plano
                {
                    Id = model.Id,
                    Valor = model.Valor,
                    Nome = model.Nome,
                    Pago = model.Pago,
                    Tipo = model.Tipo,
                    Caracteristicas = (from h in model.Caracteristicas select h).ToList(),
                    Ativo = model.Ativo,
                };

                if (model.Id == new Guid())
                {
                    plano.DataCadastro = DateTime.Now;
                    plano.Id = (Guid)_planoService.Insert(plano);
                    sucesso = true;
                }
                else
                {
                    var planoBase = _planoService.GetById(plano.Id);
                    if (planoBase != null)
                    {
                        plano.DataCadastro = planoBase.DataCadastro;
                        sucesso = _planoService.Update(plano);
                    }
                }

                return Json(new { sucesso = sucesso }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infra.CrossCutting.LogService.NotifyException("Plano", ex);
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Excluir(string id)
        {
            try
            {
                _planoService.Disable(Guid.Parse(id));

                return Json(new { sucesso = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Infra.CrossCutting.LogService.NotifyException("Plano", ex);
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult Assinatura(string plano, string nome, string cpfCnpj, string email, string telefone)
        {
            var cadastroDb = _cadastroService.GetByEmail(email);

            try
            {                
                var planoDb = _planoService.SearchFor(x => x.Nome == plano).FirstOrDefault();
                if (cadastroDb == null)
                {
                    _cadastroService.Insert(new Cadastro { Nome = nome, CpfCnpj = cpfCnpj, Email = email, Celular = telefone, Plano = planoDb, IpOrigem = Request.UserHostAddress, DataCadastro = DateTime.Now });
                }
                else
                {
                    cadastroDb.Nome = nome;
                    cadastroDb.CpfCnpj = cpfCnpj;
                    cadastroDb.Plano = planoDb;
                    cadastroDb.Celular = telefone;
                    cadastroDb.DataAlteracao = DateTime.Now;

                    _cadastroService.Update(cadastroDb);
                }

                if (plano != Planos.Standart.ToString())
                {                    
                    var isSandbox = EnvironmentConfiguration.ChangeEnvironment();

                    var pagto = new Pagamento { Referencia = cpfCnpj };

                    try
                    {
                        var credentials = PagSeguroConfiguration.Credentials(isSandbox);
                        var payment = new PaymentRequest();
                        payment.Currency = Currency.Brl;                        
                        if (planoDb != null)
                         {
                            pagto.Plano = planoDb;
                            payment.Items.Add(new Item(planoDb.Id.ToString(), planoDb.Nome, 1, planoDb.Valor));
                            // ref. no pagseguro
                            payment.Reference = cpfCnpj;

                            // customer information.                
                            payment.Sender = new Sender(
                                nome,
                                email,
                                new Phone(telefone.Substring(0, 2), telefone.Substring(2))
                            );

                            var senderCpfCnpj = new SenderDocument(Documents.GetDocumentByType(cpfCnpj.Length == 11 ? "CPF" : "CNPJ"), cpfCnpj);
                            payment.Sender.Documents.Add(senderCpfCnpj);

                            var paymentRedirectUri = payment.Register(credentials);

                            _pagamentoService.Insert(pagto);

                            return Json(new { sucesso = true, mensagem = paymentRedirectUri }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new { sucesso = false, mensagem = "Plano não definido!" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (PagSeguroServiceException ex)
                    {
                        Infra.CrossCutting.LogService.NotifyException("Pagseguro", ex);
                        return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { sucesso = true, mensagem = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Infra.CrossCutting.LogService.NotifyException("Pagseguro", ex);
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }           
        }

    }
}
