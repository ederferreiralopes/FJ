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
    public class PagamentoAppService : AppServiceBase<Pagamento>, IPagamentoAppService
    {
        private readonly IPagamentoService _pagamentoService;

        public PagamentoAppService(IPagamentoService pagamentoService)
            : base(pagamentoService)       
        {
            _pagamentoService = pagamentoService;
        }
    }
}
