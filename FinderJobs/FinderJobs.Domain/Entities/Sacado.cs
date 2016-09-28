using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinderJobs.Domain.Entities
{
    public class Sacado
    {
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual string CpfCnpj { get; set; }
        public virtual string Endereco { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Cidade { get; set; }
        public virtual string Cep { get; set; }
        public virtual string Uf { get; set; }
    }
}
