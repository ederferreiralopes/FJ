
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Infra.Data.Repositories;
using FinderJobs.Infra.Data.Context;
using FinderJobs.Domain.Interfaces.Repositories;

namespace FinderJobs.Infra.Data
{
    public class UsuarioHabilidadeRepository : RepositoryBase<UsuarioHabilidade>, IUsuarioHabilidadeRepository
    {
        public IEnumerable<UsuarioHabilidade> GetByUserId(int id)
        {
            using (var session = SessionFactory.AbrirSession())
            {
                return (from e in session.Query<UsuarioHabilidade>() where e.Usuario.Id.Equals(id) select e);
            }
        }
    }
}
