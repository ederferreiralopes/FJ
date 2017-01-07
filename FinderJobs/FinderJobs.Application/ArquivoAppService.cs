using FinderJobs.Application.Interface;
using FinderJobs.Domain.Entities;
using FinderJobs.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Application
{
    public class ArquivoAppService : AppServiceBase<Arquivo>, IArquivoAppService
    {
        private readonly IArquivoService _arquivoService;

        public ArquivoAppService(IArquivoService arquivoService)
            : base(arquivoService)       
        {
            _arquivoService = arquivoService;
        }

        public void Desativar(int id)
        {
            _arquivoService.Desativar(id);
        }

        public IEnumerable<Arquivo> GetArquivo(int UsuarioId, string tipo)            
        {
            return _arquivoService.GetArquivo(UsuarioId, tipo);
        }
    }
}
