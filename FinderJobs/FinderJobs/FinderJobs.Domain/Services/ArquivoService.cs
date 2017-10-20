using FinderJobs.Domain.Entities;
using FinderJobs.Domain.Interfaces.Repositories;
using FinderJobs.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Services
{
    public class ArquivoService : ServiceBase<Arquivo>, IArquivoService
    {
        private readonly IArquivoRepository _arquivoRepository;

        public ArquivoService(IArquivoRepository arquivoRepository)
            : base(arquivoRepository)
        {
            _arquivoRepository = arquivoRepository;
        }

        public IEnumerable<Arquivo> GetArquivo(Guid usuarioId, string tipo)
        {            
            return _arquivoRepository.SearchFor(x => x.CadastroId == usuarioId && x.Tipo == tipo && x.Ativo);
        }

        public IEnumerable<Arquivo> CarregarTodos(Guid usuarioId)
        {            
            return _arquivoRepository.SearchFor(x => x.CadastroId == usuarioId && x.Ativo);
        }
    }
}
