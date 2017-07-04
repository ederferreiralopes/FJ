using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Email : EntityBase
    {
        public string Remetente { get; set; }
        public string Mensagem { get; set; }
        public string Destino { get; set; }
        public string Titulo { get; set; }
    }
}
