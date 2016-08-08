using BlessTheWeb.Data.NHibernate;
using BlessTheWeb.Email.Azure;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(BlessTheWeb.MVC5.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(BlessTheWeb.MVC5.App_Start.NinjectWebCommon), "Stop")]

namespace BlessTheWeb.MVC5.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Core;
    using Core.Repository;
    using NHibernate;
    using Storage.AzureCdn;
    using log4net;
    using Email;
    using Core.Twitter;
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
            kernel.Bind<ILog>().ToMethod(x=>log4net.LogManager.GetLogger(x.Request.Target != null ? x.Request.Target.Type.FullName : "BlessTheWeb"));
            kernel.Bind<IFileStorage>().To<AzureFileStorage>();
            kernel.Bind<IIndulgenceGenerator>().To<IndulgenceGeneratoriTextSharp>();
            kernel.Bind<ISession>().ToMethod(x =>SessionFactory.Instance.OpenSession()).InRequestScope();
            kernel.Bind<IIndulgeMeService>().To<NHibernateIndulgeMeService>();
            kernel.Bind<IIndulgenceEmailer>().To<AzureIndulgenceEmailer>();
            kernel.Bind<ITweeter>().To<Tweeter>();
        }
    }
}
