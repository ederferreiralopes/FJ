using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinderJobs.MVC.ViewModels;
using FinderJobs.Application.Interface;

namespace FinderJobs.MVC.Controllers
{
    public class AssinaturaController : Controller
    {

        private readonly IConfiguracaoBoletoAppService _configuracaoBoletoService;

        public AssinaturaController(IConfiguracaoBoletoAppService configuracaoBoletoService)
        {
            _configuracaoBoletoService = configuracaoBoletoService;            
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GerarBoleto(UsuarioViewModel usuario)
        {
            usuario.Id = 1;
            usuario.Nome = "Usuario Teste";
            usuario.CpfCnpj = "12312312312";
            usuario.EnderecoCep = "11680000";

            var model = new BoletoModel();
            var configuracaoBoleto = _configuracaoBoletoService.GetById(1);
            model.Id = configuracaoBoleto.Id;
            model.CodigoBanco = configuracaoBoleto.CodigoBanco;
            model.CodigoCarteira = configuracaoBoleto.CodigoCarteira;
            model.MostrarCodigoCarteira = configuracaoBoleto.MostrarCodigoCarteira;
            model.MostrarComprovanteEntrega = configuracaoBoleto.MostrarComprovanteEntrega;
            model.NumeroDocumento = configuracaoBoleto.NumeroDocumento;
            model.ValorBoleto = configuracaoBoleto.ValorBoleto;
            model.Vencimento = configuracaoBoleto.Vencimento;

            model.Cedente = new CedenteModel { 
                Id = configuracaoBoleto.Cedente.Id,
                Nome = configuracaoBoleto.Cedente.Nome, 
                CpfCnpj = configuracaoBoleto.Cedente.CpfCnpj, 
                NossoNumero = configuracaoBoleto.Cedente.NossoNumero, 
                Agencia = configuracaoBoleto.Cedente.Agencia,
                Conta = configuracaoBoleto.Cedente.Conta,
                DigitoConta = configuracaoBoleto.Cedente.DigitoConta,
                Codigo = configuracaoBoleto.Cedente.Codigo
            
            };
            
            model.Sacado = new SacadoModel { 
                Id = usuario.Id,
                Nome = usuario.Nome, 
                CpfCnpj = usuario.CpfCnpj, 
                Cep = usuario.EnderecoCep,
                Endereco = usuario.EnderecoLogradouro,
                Bairro = usuario.EnderecoBairro,
                Cidade = usuario.EnderecoCidade,
                Uf = usuario.EnderecoUF
            };

            // criar entidade Assinatura
            // migrar isso para MongoDB
            // depois de gerar o boleto gravar o html no MongoDB

            if (model != null && model.Cedente.Id > 0 && model.Sacado.Id > 0)
                ViewBag.Boleto = new Services.Boleto().GeraBoleto(model);

            return View("Index");
        }
    }
}
