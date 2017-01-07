using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Interfaces.Services
{
    public interface IVagaService: IServiceBase<Vaga>
    {
        IEnumerable<Vaga> BuscarPorEmpresa(int id);
    }
}
