using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Vaga : EntityBase
    {        
        public Guid EmpresaId { get; set; }
        public string EmpresaNome { get; set; }
        public string EmpresaUrlAvatar { get; set; }
        public string Descricao { get; set; }
        public string Cep { get; set; }
        public List<string> Habilidades { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public DateTime DataExpiracao { get; set; }
    }
}
