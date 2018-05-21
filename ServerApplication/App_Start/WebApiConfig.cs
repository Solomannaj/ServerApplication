using ServerApplication.BussinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Unity;
using Unity.AspNet.WebApi;
using Unity.Lifetime;

namespace ServerApplication
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new {id = RouteParameter.Optional }
            );
            

            var container = new UnityContainer();
            container.RegisterType<IDataPublisher, DataPublisher>(new HierarchicalLifetimeManager());
            container.RegisterType<ICSVParser, CSVParser>(new HierarchicalLifetimeManager());
            container.RegisterType<IXMLGenerator, XMLGenerator>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}
