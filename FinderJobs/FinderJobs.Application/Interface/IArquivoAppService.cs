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

        IEnumerable<Arquivo> GetArquivo(int UsuarioId, string tipo);        
    }
}
