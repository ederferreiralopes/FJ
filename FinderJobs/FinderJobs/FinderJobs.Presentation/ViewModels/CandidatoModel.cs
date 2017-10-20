
using System;

namespace FinderJobs.MVC.ViewModels
{
    public class CandidatoModel
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Profissao { get; set; }
        public string Habilidades { get; set; }
        public string Curriculo { get; set; }
    }
}