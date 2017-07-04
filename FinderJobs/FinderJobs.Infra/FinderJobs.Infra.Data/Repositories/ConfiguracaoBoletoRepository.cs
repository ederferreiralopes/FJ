
using System.Collections.Generic;
using System.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Domain.Interfaces.Repositories;
using System;
using System.Linq.Expressions;

namespace FinderJobs.Infra.Data.Repositories
{
    public class ConfiguracaoBoletoRepository : RepositoryBaseMongoDb<ConfiguracaoBoleto>, IConfiguracaoBoletoRepository
    {
        public IEnumerable<ConfiguracaoBoleto> BuscarPorNossoNumero(string nossoNumero)
        {
            return null;
        }
    }
}
