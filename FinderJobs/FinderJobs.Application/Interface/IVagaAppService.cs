using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Application.Interface
{
    public interface IVagaAppService : IAppServiceBase<Vaga>
    {
        IEnumerable<Vaga> BuscarPorEmpresa(int id);
    }
}
