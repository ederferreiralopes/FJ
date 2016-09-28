
using System;
using System.ComponentModel.DataAnnotations;

namespace FinderJobs.MVC.ViewModels
{
    public class BoletoModel
    {

        public BoletoModel()
        {
            this.Cedente = new CedenteModel();
            this.Sacado = new SacadoModel();
        }

        public int Id { get; set; }
        public short CodigoBanco { get; set; }
        public DateTime Vencimento { get; set; }
        public decimal ValorBoleto { get; set; }
        public string NumeroDocumento { get; set; }
        public string Descricao { get; set; }
        public string CodigoCarteira { get; set; }
        public string CodigoEspecieDocumento { get; set; }
        public bool MostrarCodigoCarteira { get; set; }
        public bool MostrarComprovanteEntrega { get; set; }        
        public virtual CedenteModel Cedente { get; set; }
        public virtual SacadoModel Sacado { get; set; }
    }

    public class CedenteModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string NossoNumero { get; set; }
        public string CpfCnpj { get; set; }
        public string Nome { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string DigitoConta { get; set; }
    }

    public class SacadoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }        
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Cep { get; set; }
        public string Uf { get; set; }
    }
}