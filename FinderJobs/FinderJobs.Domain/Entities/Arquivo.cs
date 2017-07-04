using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Arquivo : EntityBase
    {        
        public Guid CadastroId { get; set; }
        public string Caminho { get; set; }
        public string Tipo { get; set; }
        public string Nome { get; set; }               
    }
}
