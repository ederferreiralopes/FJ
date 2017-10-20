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
    public class ConfiguracaoBoletoService : ServiceBase<ConfiguracaoBoleto>, IConfiguracaoBoletoService
    {
        private readonly IConfiguracaoBoletoRepository _configuracaoBoletoRepository;

        public ConfiguracaoBoletoService(IConfiguracaoBoletoRepository configuracaoBoletoRepository)
            : base(configuracaoBoletoRepository)
        {
            _configuracaoBoletoRepository = configuracaoBoletoRepository;
        }
    }
}
