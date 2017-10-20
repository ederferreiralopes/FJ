using System;
using System.Collections.Generic;

namespace FinderJobs.Domain.Entities
{
    public class Cadastro : EntityBase
    {                
        public bool Anonimo { get; set; }
        public string Nome { get; set; }
        public string UrlAvatar { get; set; }
        public Plano Plano { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string CpfCnpj { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public List<string> Habilidades { get; set; }
        public Endereco Endereco { get; set; }       
        public string IpOrigem { get; set; } 
    }
}