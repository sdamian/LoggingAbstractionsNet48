using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Extensions.Logging;
using Serilog;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;

namespace WebApplication11
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var loggerFactory = new Microsoft.Extensions.Logging.LoggerFactory();
            Log.Logger = new Serilog.LoggerConfiguration()
                .Enrich.WithHttpRequestId()
                .Enrich.WithHttpRequestType()
                .Enrich.WithHttpRequestRawUrl()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("CPU", Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"))
                .WriteTo.File(Server.MapPath("~/App_Data/logs.txt"))
                .WriteTo.Seq("http://localhost:5341")
                .MinimumLevel.Debug()
                .CreateLogger();

            loggerFactory.AddSerilog(Log.Logger);

            // create logger and put it to work.
            var logProvider = loggerFactory.CreateLogger<MvcApplication>();
            logProvider.LogInformation("Hi Serilog!");
            
            var container = new Container
            {
                Options = {DefaultScopedLifestyle = new WebRequestLifestyle()}
            };
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            
            // register logger factory and generic logger
            container.RegisterInstance<ILoggerFactory>(loggerFactory);
            container.RegisterSingleton(typeof(ILogger<>), typeof(Logger<>));

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End()
        {
            Log.Logger.ForContext<MvcApplication>().Information("Bye Serilog!");
            Log.CloseAndFlush();
        }
    }
}
