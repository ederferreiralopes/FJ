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
    public class PlanoService : ServiceBase<Plano>, IPlanoService
    {
        private readonly IPlanoRepository _planoRepository;

        public PlanoService(IPlanoRepository planoRepository)
            : base(planoRepository)
        {
            _planoRepository = planoRepository;
        }
    }
}
