using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace FinderJobs.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            AutoMapper.AutoMapperConfig.RegisterMappings();
        }
    }
}