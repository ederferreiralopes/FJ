
using System;
using System.Collections.Generic;
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
        public string CpfCnpj { get; set; }
        public string EnderecoCep { get; set; }
        public string EnderecoLogradouro { get; set; }
        public string EnderecoNumero { get; set; }
        public string EnderecoBairro { get; set; }
        public string EnderecoCidade { get; set; }
        public string EnderecoUF { get; set; }
        public string DataCadastro { get; set; }
        public string Habilidades { get; set; }
        public string CaminhoArquivo { get; set; }
    }
}