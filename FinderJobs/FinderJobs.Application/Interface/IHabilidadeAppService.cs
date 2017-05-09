using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Application.Interface
{
    public interface IHabilidadeAppService : IAppServiceBase<Habilidade>
    {
        IEnumerable<Habilidade> BuscarPorNome(string nome, bool ativo);
    }
}
