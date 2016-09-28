using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinderJobs.MVC.ViewModels
{
    public class DistanciaModel
    {
        public string CepVaga { get; set; }
        public string NomeCandidato { get; set; }
        public string CepCandidato { get; set; }
        public string Distancia { get; set; }
        public string Porcentagem { get; set; }
        public string Unidade { get; set; }
    }
}