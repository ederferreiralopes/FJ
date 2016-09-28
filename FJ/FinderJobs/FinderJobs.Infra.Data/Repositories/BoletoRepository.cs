
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Domain.Interfaces;
using FinderJobs.Infra.Data.Context;

namespace FinderJobs.Infra.Data.Repositories
{
    public class BoletoRepository : RepositoryBase<Boleto>, IBoletoRepository
    {
        public IEnumerable<Boleto> BuscarPorNossoNumero(string nossoNumero)
        {
            using (var session = SessionFactory.AbrirSession())
            {                
                return (from e in session.Query<Boleto>() where e.Cedente.NossoNumero.Equals(nossoNumero) select e);
            }
        }
    }
}
