using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Interfaces.Services
{
    public interface IHabilidadeService : IServiceBase<Habilidade>
    {
        IEnumerable<Habilidade> BuscarPorNome(string nome, bool ativo);

        Dictionary<string, string> GetDashboard();
    }
}
