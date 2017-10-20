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
        IEnumerable<Vaga> BuscarPorEmpresa(Guid id);
        IEnumerable<Vaga> BuscarPaginadaPorEmpresa(Guid id, int pagina);
        IEnumerable<Vaga> BuscarVagas(List<string> habilidades, int pagina);
    }
}
