[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(FinderJobs.Site.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(FinderJobs.Site.App_Start.NinjectWebCommon), "Stop")]

namespace FinderJobs.Site.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Domain.Interfaces.Services;
    using Application;
    using Application.Interface;
    using Domain.Services;
    using Infra.Data.Repositories;
    using Domain.Interfaces.Repositories;
    using Infra.Data;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(typeof(IAppServiceBase<>)).To(typeof(AppServiceBase<>));
            kernel.Bind<ICadastroAppService>().To<CadastroAppService>();
            kernel.Bind<IHabilidadeAppService>().To<HabilidadeAppService>();            
            kernel.Bind<IVagaAppService>().To<VagaAppService>();
            kernel.Bind<IConfiguracaoBoletoAppService>().To<ConfiguracaoBoletoAppService>();
            kernel.Bind<IArquivoAppService>().To<ArquivoAppService>();
            kernel.Bind<IPlanoAppService>().To<PlanoAppService>();
            kernel.Bind<IPagamentoAppService>().To<PagamentoAppService>();
            kernel.Bind<IEmailAppService>().To<EmailAppService>();

            kernel.Bind(typeof(IServiceBase<>)).To(typeof(ServiceBase<>));            
            kernel.Bind<ICadastroService>().To<CadastroService>();
            kernel.Bind<IHabilidadeService>().To<HabilidadeService>();            
            kernel.Bind<IVagaService>().To<VagaService>();
            kernel.Bind<IConfiguracaoBoletoService>().To<ConfiguracaoBoletoService>();
            kernel.Bind<IArquivoService>().To<ArquivoService>();
            kernel.Bind<IPlanoService>().To<PlanoService>();
            kernel.Bind<IPagamentoService>().To<PagamentoService>();
            kernel.Bind<IEmailService>().To<EmailService>();

            kernel.Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBaseMongoDb<>));            
            kernel.Bind<ICadastroRepository>().To<CadastroRepository>();
            kernel.Bind<IHabilidadeRepository>().To<HabilidadeRepository>();            
            kernel.Bind<IVagaRepository>().To<VagaRepository>();
            kernel.Bind<IConfiguracaoBoletoRepository>().To<ConfiguracaoBoletoRepository>();
            kernel.Bind<IArquivoRepository>().To<ArquivoRepository>();
            kernel.Bind<IPlanoRepository>().To<PlanoRepository>();
            kernel.Bind<IPagamentoRepository>().To<PagamentoRepository>();
            kernel.Bind<IEmailRepository>().To<EmailRepository>();
        }        
    }
}
