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
    public class EmailAppService : AppServiceBase<Email>, IEmailAppService
    {
        private readonly IEmailService _emailService;

        public EmailAppService(IEmailService emailService)
            : base(emailService)       
        {
            _emailService = emailService;
        }
    }
}
