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
    public class EmailService : ServiceBase<Email>, IEmailService
    {
        private readonly IEmailRepository _emailRepository;

        public EmailService(IEmailRepository emailRepository)
            : base(emailRepository)
        {
            _emailRepository = emailRepository;
        }
    }
}
