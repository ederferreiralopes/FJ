using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Interfaces.Services
{
    public interface IUsuarioHabilidadeService : IServiceBase<UsuarioHabilidade>
    {
        IEnumerable<UsuarioHabilidade> GetByUserId(int id);
    }
}
