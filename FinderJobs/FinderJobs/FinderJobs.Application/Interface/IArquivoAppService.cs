using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Application.Interface
{
    public interface IArquivoAppService : IAppServiceBase<Arquivo>
    {
        IEnumerable<Arquivo> GetArquivo(Guid UsuarioId, string tipo);
        IEnumerable<Arquivo> CarregarTodos(Guid UsuarioId);
    }
}
