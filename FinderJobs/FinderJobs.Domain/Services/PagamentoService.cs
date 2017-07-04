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
    public class PagamentoService : ServiceBase<Pagamento>, IPagamentoService
    {
        private readonly IPagamentoRepository _pagamentoRepository;

        public PagamentoService(IPagamentoRepository pagamentoRepository)
            : base(pagamentoRepository)
        {
            _pagamentoRepository = pagamentoRepository;
        }
    }
}
