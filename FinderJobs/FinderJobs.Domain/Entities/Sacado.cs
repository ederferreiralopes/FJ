using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinderJobs.Domain.Entities
{
    public class Sacado
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Cep { get; set; }
        public string UF { get; set; }
    }
}
