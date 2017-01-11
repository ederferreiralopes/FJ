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
        void Desativar(int id);

        void Desativar(int id, string tipo);

        IEnumerable<Arquivo> GetArquivo(int UsuarioId, string tipo);
        IEnumerable<Arquivo> CarregarTodos(int UsuarioId);
    }
}
