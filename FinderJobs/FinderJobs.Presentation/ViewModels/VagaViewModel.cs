using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinderJobs.Site.ViewModels
{
    public class Vaga
    {
        public int Id { get; set; }
        public Guid IdEmpresa { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Empresa { get; set; }
        public string Descricao { get; set; }
        public string Cep { get; set; }
        public string Habilidades { get; set; }
    }

    public class VagaViewModel : EntityBase
    {        
        public Guid IdEmpresa { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Empresa { get; set; }
        public string Descricao { get; set; }
        public string Cep { get; set; }
        public List<Habilidade> Habilidades { get; set; }
        public Vaga Vaga { get; set; }
        public UsuarioViewModel Candidato { get; set; }
        public DistanciaViewModel Pesquisa { get; set; }
    }

    public class CandidatoVagaViewModel
    {
        public UsuarioViewModel Origem { get; set; }
        public List<VagaDistanciaViewModel> Destinos { get; set; }
    }

    public class EmpresaViewModel
    {
        public UsuarioViewModel Usuario { get; set; }
        public List<VagaViewModel> Vagas { get; set; }        
    }

    public class EmpresaVagaViewModel
    {
        public List<CandidatoDistanciaViewModel> Candidatos { get; set; }
        public List<EmpresaDistanciaViewModel> Vagas { get; set; }
    }

    public class CandidatoDistanciaViewModel : EntityBase
    {        
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string EnderecoCep { get; set; }
        public string Habilidades { get; set; }        
    }

    public class EmpresaDistanciaViewModel : EntityBase
    {
        public string Cep { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Descricao { get; set; }
        public List<Habilidade> Habilidades { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioCep { get; set; }
        public string Distancia { get; set; }
        public string Porcentagem { get; set; }
        public string Unidade { get; set; }

        public List<CalculosVaga> CalculosVaga { get; set; }
    }

    public class CalculosVaga
    {
        public Guid CandidatoId { get; set; }       
        public decimal Aderencia { get; set; }
        public string Distancia { get; set; }
    }

    public class VagaDistanciaViewModel : EntityBase
    {        
        public Guid IdEmpresa { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Empresa { get; set; }
        public string Descricao { get; set; }
        public string Cep { get; set; }
        public string Distancia { get; set; }
        public string Porcentagem { get; set; }
        public string Unidade { get; set; }
        public List<HabilidadeViewModel> Habilidades { get; set; }
    }
}