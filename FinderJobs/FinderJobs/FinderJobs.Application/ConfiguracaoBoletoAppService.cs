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
    public class ConfiguracaoBoletoAppService : AppServiceBase<ConfiguracaoBoleto>, IConfiguracaoBoletoAppService
    {
        private readonly IConfiguracaoBoletoService _configuracaoBoletoService;

        public ConfiguracaoBoletoAppService(IConfiguracaoBoletoService configuracaoBoletoService)     
            : base(configuracaoBoletoService)       
        {
            _configuracaoBoletoService = configuracaoBoletoService;
        }
    }
}
