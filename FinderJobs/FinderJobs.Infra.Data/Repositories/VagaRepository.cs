
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Infra.Data.Repositories;
using FinderJobs.Infra.Data.Context;
using FinderJobs.Domain.Interfaces;

namespace FinderJobs.Infra.Data
{
    public class VagaRepository : RepositoryBase<Vaga>, IVagaRepository
    {
        public List<Vaga> BuscarVagas()
        {
            using (var session = SessionFactory.AbrirSession())
            {
                return (from e in session.Query<Vaga>() select e).ToList();
            }
        }

        public List<Vaga> BuscarPorEmpresa(int idEmpresa)
        {
            using (var session = SessionFactory.AbrirSession())
            {
                return (from e in session.Query<Vaga>() where e.IdEmpresa.Equals(idEmpresa) select e).ToList();
            }
        }
    }
}
