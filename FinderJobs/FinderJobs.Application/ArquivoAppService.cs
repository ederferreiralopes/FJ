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

        public void Desativar(Guid id)
        {
            _arquivoService.Desativar(id);
        }

        public void Desativar(Guid id, string tipo)
        {
            _arquivoService.Desativar(id, tipo);
        }

        public IEnumerable<Arquivo> GetArquivo(Guid UsuarioId, string tipo)            
        {
            return _arquivoService.GetArquivo(UsuarioId, tipo);
        }

        public IEnumerable<Arquivo> CarregarTodos(Guid UsuarioId)
        {
            return _arquivoService.CarregarTodos(UsuarioId);
        }
    }
}
