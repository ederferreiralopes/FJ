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
    public class PlanoAppService : AppServiceBase<Plano>, IPlanoAppService
    {
        private readonly IPlanoService _planoService;

        public PlanoAppService(IPlanoService planoService)
            : base(planoService)       
        {
            _planoService = planoService;
        }
    }
}
