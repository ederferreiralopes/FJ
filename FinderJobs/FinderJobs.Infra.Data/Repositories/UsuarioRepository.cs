
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Infra.Data.Repositories;
using FinderJobs.Infra.Data.Context;
using FinderJobs.Domain.Interfaces.Repositories;

namespace FinderJobs.Infra.Data
{
    public class UsuarioRepository : RepositoryBaseMongoDb<Usuario>, IUsuarioRepository
    {
        public List<Usuario> BuscarPorTipo(string tipo)
        {
            using (var session = SessionFactory.AbrirSession())
            {
                return (from e in session.Query<Usuario>() where e.Tipo.Equals(tipo) select e).ToList();
            }
        }
    }
}
