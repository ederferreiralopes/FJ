using FluentNHibernate.Mapping;

namespace FinderJobs.Domain.Entities
{
    public class Usuario
    {
        public virtual int Id { get; set; }
        public virtual bool Pago { get; set; }
        public virtual bool Anonimo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Login { get; set; }
        public virtual string Senha { get; set; }
        public virtual string Tipo { get; set; }
        public virtual string Email { get; set; }
        public virtual string Celular { get; set; }
        public virtual string CpfCnpj { get; set; }
        public virtual string EnderecoCep { get; set; }
        public virtual string EnderecoLogradouro { get; set; }
        public virtual string EnderecoNumero { get; set; }
        public virtual string EnderecoBairro { get; set; }
        public virtual string EnderecoCidade { get; set; }
        public virtual string EnderecoUF { get; set; }
        public virtual string DataCadastro { get; set; }
        public virtual string Habilidades { get; set; }
        public virtual bool Ativo { get; set; }
    }

    public class UsuarioMap : ClassMap<Usuario>
    {

        public UsuarioMap()
        {
            Table("Usuario");

            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.Pago).Not.Nullable().Column("Pago");
            Map(x => x.Anonimo).Not.Nullable().Column("Anonimo");
            Map(x => x.Nome).Not.Nullable().Column("Nome");
            Map(x => x.Login).Not.Nullable().Column("Login");
            Map(x => x.Senha).Not.Nullable().Column("Senha");
            Map(x => x.Tipo).Not.Nullable().Column("Tipo");
            Map(x => x.Email).Not.Nullable().Column("Email");
            Map(x => x.Celular).Column("Celular");
            Map(x => x.CpfCnpj).Not.Nullable().Column("CpfCnpj");
            Map(x => x.EnderecoCep).Not.Nullable().Column("EnderecoCep");
            Map(x => x.EnderecoLogradouro).Not.Nullable().Column("EnderecoLogradouro");
            Map(x => x.EnderecoNumero).Not.Nullable().Column("EnderecoNumero");
            Map(x => x.EnderecoBairro).Not.Nullable().Column("EnderecoBairro");
            Map(x => x.EnderecoCidade).Not.Nullable().Column("EnderecoCidade");
            Map(x => x.EnderecoUF).Not.Nullable().Column("EnderecoUF");
            Map(x => x.Habilidades).Not.Nullable().Column("Habilidades");
            Map(x => x.DataCadastro).Not.Nullable().Column("DataCadastro");
            Map(x => x.Ativo).Not.Nullable().Column("Ativo");
        }
    }
}