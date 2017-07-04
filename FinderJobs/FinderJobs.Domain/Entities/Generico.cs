using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public enum Planos
    {
        Standart, Advanced, Premium, Gold
    }

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

    public enum PlanoTipo
    {
        Candidato,
        Empresa,        
    }

    public enum Mes
    {
        Jan, Fev, Mar, Abr, Mai, Jun, Jul, Ago, Set, Out, Nov, Dez
    }
}
