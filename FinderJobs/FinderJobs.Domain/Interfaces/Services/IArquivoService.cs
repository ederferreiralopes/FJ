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
        void Desativar(int id);

        void Desativar(int id, string tipo);

        IEnumerable<Arquivo> GetArquivo(int usuarioId, string tipo);

        IEnumerable<Arquivo> CarregarTodos(int usuarioId);
    }
}
