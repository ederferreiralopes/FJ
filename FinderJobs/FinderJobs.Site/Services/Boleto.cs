using FinderJobs.Site.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinderJobs.Site.Services
{
    public class Boleto
    {
        public BoletoNet.BoletoBancario GeraBoleto(BoletoModel model)
        {                        
            var cedente = new BoletoNet.Cedente(model.Cedente.CpfCnpj,
                            model.Cedente.Nome,
                            model.Cedente.Agencia,
                            model.Cedente.Conta,
                            model.Cedente.DigitoConta
                            );

            cedente.Codigo = model.Cedente.Codigo;

            var boleto = new BoletoNet.Boleto(
                model.Vencimento,
                model.ValorBoleto, 
                model.CodigoCarteira, 
                model.Cedente.NossoNumero, 
                cedente
                );

            boleto.NumeroDocumento = model.NumeroDocumento;

            var sacado = new BoletoNet.Sacado(model.Sacado.CpfCnpj, model.Sacado.Nome);
            boleto.Sacado = sacado;
            boleto.Sacado.Endereco.End = model.Sacado.Endereco;
            boleto.Sacado.Endereco.Bairro = model.Sacado.Bairro;
            boleto.Sacado.Endereco.Cidade = model.Sacado.Cidade;
            boleto.Sacado.Endereco.CEP = model.Sacado.Cep;
            boleto.Sacado.Endereco.UF = model.Sacado.Uf;

            var instrucaoItau = new BoletoNet.Instrucao_Itau();
            instrucaoItau.Descricao = model.Descricao;

            boleto.Instrucoes.Add(instrucaoItau);
            var especieDocumentoItau = new BoletoNet.EspecieDocumento_Itau(model.CodigoEspecieDocumento);
            boleto.EspecieDocumento = especieDocumentoItau;

            var boletBancario = new BoletoNet.BoletoBancario();
            boletBancario.CodigoBanco = model.CodigoBanco;
            boletBancario.Boleto = boleto;
            boletBancario.MostrarCodigoCarteira = model.MostrarCodigoCarteira;
            boletBancario.Boleto.Valida();
            boletBancario.MostrarComprovanteEntrega = model.MostrarComprovanteEntrega;

            return boletBancario;
        }
    }
}