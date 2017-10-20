
using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinderJobs.Site.ViewModels
{
    public class CadastroViewModel : EntityBase
    {        
        public bool Pago { get; set; }
        public bool Anonimo { get; set; }
        public string Nome { get; set; }
        public string UrlAvatar { get; set; }
        public Plano Plano { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string CpfCnpj { get; set; }
        public Endereco Endereco { get; set; }        
        public List<HabilidadeViewModel> Habilidades { get; set; }
        public string CaminhoArquivo { get; set; }
        public DateTime? DataCadastro { get; set; }
    }
}