using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Boleto
    {

        public Boleto()
        {
            this.Cedente = new Cedente();
            this.Sacado = new Sacado();
        }

        public virtual int Id { get; set; }
        public virtual short CodigoBanco { get; set; }
        public virtual DateTime Vencimento { get; set; }
        public virtual decimal ValorBoleto { get; set; }
        public virtual string NumeroDocumento { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string CodigoCarteira { get; set; }
        public virtual string CodigoEspecieDocumento { get; set; }
        public virtual bool MostrarCodigoCarteira { get; set; }
        public virtual bool MostrarComprovanteEntrega { get; set; }
        public virtual Cedente Cedente { get; set; }
        public virtual Sacado Sacado { get; set; }
    }
}
