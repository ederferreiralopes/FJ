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

        public void Desativar(int id)
        {
            _arquivoRepository.Desativar(id);
        }

        public IEnumerable<Arquivo> GetArquivo(int usuarioId, string tipo)
        {
            return _arquivoRepository.GetAll().Where(u => u.Usuario.Id == usuarioId && u.Tipo.Equals(tipo) && u.Ativo);
        }
    }
}
