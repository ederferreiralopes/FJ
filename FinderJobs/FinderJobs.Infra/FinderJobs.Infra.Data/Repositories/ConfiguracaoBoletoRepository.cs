
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Domain.Interfaces.Repositories;
using FinderJobs.Infra.Data.Context;
using System;
using System.Linq.Expressions;

namespace FinderJobs.Infra.Data.Repositories
{
    public class ConfiguracaoBoletoRepository : RepositoryBaseMongoDb<ConfiguracaoBoleto>, IConfiguracaoBoletoRepository
    {
        public IEnumerable<ConfiguracaoBoleto> BuscarPorNossoNumero(string nossoNumero)
        {
            using (var session = SessionFactory.AbrirSession())
            {                
                return (from e in session.Query<ConfiguracaoBoleto>() where e.Cedente.NossoNumero.Equals(nossoNumero) select e);
            }
        }
    }
}
