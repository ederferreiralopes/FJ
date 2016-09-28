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
        public virtual string RgCnpj { get; set; }
        public virtual string Cep { get; set; }
        public virtual string DataCadastro { get; set; }
        public virtual string Habilidades { get; set; }
        public virtual string CaminhoArquivo { get; set; }
    }

    public class UsuarioMap :  ClassMap<Usuario>
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
            Map(x => x.Celular).Not.Nullable().Column("Celular");
            Map(x => x.RgCnpj).Not.Nullable().Column("RgCnpj");
            Map(x => x.Cep).Not.Nullable().Column("Cep");
            Map(x => x.Habilidades).Not.Nullable().Column("Habilidades");
            Map(x => x.CaminhoArquivo).Not.Nullable().Column("CaminhoArquivo");
            Map(x => x.DataCadastro).Not.Nullable().Column("DataCadastro");
        }        
    }
}