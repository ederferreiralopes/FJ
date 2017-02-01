using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public enum ArquivoTipo
    {
        Boleto,
        Curriculo,
        Vaga,
    }

    public enum ArquivoLocal
    {
        Avatar,
        Boleto,
        Curriculo,
        Vaga,
        Indefinido,
    }

    public enum UsuarioTipo
    {
        Candidato,
        Empresa,        
    }
}
