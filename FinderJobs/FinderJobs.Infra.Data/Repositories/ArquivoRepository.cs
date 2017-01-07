
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Domain.Interfaces.Repositories;
using FinderJobs.Infra.Data.Context;
using System.Text;

namespace FinderJobs.Infra.Data.Repositories
{
    public class ArquivoRepository : RepositoryBase<Arquivo>, IArquivoRepository
    {
        public void Desativar(int id)
        {
            using (var session = SessionFactory.AbrirSession())
            {
                try
                {
                    var query = string.Concat("update arquivo set Ativo = 0 where id = ", id);
                    session.CreateSQLQuery(query)
                    .ExecuteUpdate();
                }
                catch (System.Exception ex)
                {
                }
            }
        }
    }
}
