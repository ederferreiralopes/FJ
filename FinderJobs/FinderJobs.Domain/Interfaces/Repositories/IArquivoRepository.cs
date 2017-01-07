using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinderJobs.Domain.Entities;

namespace FinderJobs.Domain.Interfaces.Repositories
{
    public interface IArquivoRepository : IRepositoryBase<Arquivo>
    {
        void Desativar(int id);
    }
}
