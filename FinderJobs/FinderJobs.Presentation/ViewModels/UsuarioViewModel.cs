
using System;
using System.ComponentModel.DataAnnotations;

namespace FinderJobs.MVC.ViewModels
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public bool Pago { get; set; }
        public bool Anonimo { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Tipo { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string RgCnpj { get; set; }
        public string Cep { get; set; }        
        public string DataCadastro { get; set; }
        public string Habilidades { get; set; }
        public string CaminhoArquivo { get; set; }
    }
}