using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Interfaces.Services
{
    public interface IArquivoService : IServiceBase<Arquivo>
    {
        IEnumerable<Arquivo> GetArquivo(Guid usuarioId, string tipo);

        IEnumerable<Arquivo> CarregarTodos(Guid usuarioId);
    }
}
