﻿using FinderJobs.Manager.Models;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FinderJobs.Manager
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			EnsureAuthIndexes.Exist();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);            
        }
    }
}
