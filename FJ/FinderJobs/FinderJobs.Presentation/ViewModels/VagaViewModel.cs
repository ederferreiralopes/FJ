using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinderJobs.MVC.ViewModels
{
    public class Vaga
    {
        public int Id { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Empresa { get; set; }
        public string Descricao { get; set; }
        public string Cep { get; set; }
        public string Habilidades { get; set; }
    }

    public class VagaViewModel
    {
        public int Id { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Empresa { get; set; }
        public string Descricao { get; set; }
        public string Cep { get; set; }
        public string Habilidades { get; set; }
        public Vaga Vaga { get; set; }
        public UsuarioViewModel Candidato { get; set; }
        public DistanciaModel Pesquisa { get; set; }
    }

    public class EmpresaViewModel
    {
        public UsuarioViewModel Usuario { get; set; }
        public List<VagaViewModel> Vagas { get; set; }        
    }
}