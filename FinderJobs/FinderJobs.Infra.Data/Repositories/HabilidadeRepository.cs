
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Infra.Data.Repositories;
using FinderJobs.Infra.Data.Context;
using FinderJobs.Domain.Interfaces.Repositories;

namespace FinderJobs.Infra.Data
{
    public class HabilidadeRepository : RepositoryBase<Habilidade>, IHabilidadeRepository
    {
        public List<Habilidade> BuscarPorNome(string nome)
        {
            using (var session = SessionFactory.AbrirSession())
            {
                return (from e in session.Query<Habilidade>() where e.Nome.Contains(nome) select e).ToList();
            }
        }
    }
}
