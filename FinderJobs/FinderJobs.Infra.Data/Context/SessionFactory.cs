using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using FinderJobs.Domain.Entities;

namespace FinderJobs.Infra.Data.Context
{
    public class SessionFactory
    {
        private static ISessionFactory session;

        public static ISessionFactory CrateSessionFactory()
        {
            if (session != null)
                return session;
            //conexão para MS SqlServer
            //IPersistenceConfigurer configDB = MsSqlConfiguration.MsSql2012.ConnectionString(x => x.FromConnectionStringWithKey("conexao"));

            //conexão para MySQL            
            IPersistenceConfigurer configDB = MySQLConfiguration.Standard.ConnectionString(x => x.FromConnectionStringWithKey("conexao"));

            var configMap = Fluently.Configure().Database(configDB)                
                //.ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, true)) 
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ArquivoMap>())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ConfiguracaoBoletoMap>())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UsuarioMap>())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<VagaMap>());

            session = configMap.BuildSessionFactory();

            return session;
        }

        public static ISession AbrirSession()
        {
            return CrateSessionFactory().OpenSession();
        }
    }
}
