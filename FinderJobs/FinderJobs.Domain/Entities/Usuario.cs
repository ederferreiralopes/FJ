using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;

namespace FinderJobs.Domain.Entities
{
    public class Usuario : EntityBase
    {        
        public bool Pago { get; set; }
        public bool Anonimo { get; set; }
        public string Nome { get; set; }
        public string UrlAvatar { get; set; }
        public string Tipo { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string CpfCnpj { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public List<string> Habilidades { get; set; }
        public Endereco Endereco { get; set; }        
    }
}