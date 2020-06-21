using Rad302FinalPrj_GTeam.Models;
using Rad302FinalPrj_GTeam.Models.Items;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Rad302FinalPrj_GTeam
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            Database.SetInitializer(new DbInitializer());
            GeneralDbContext gen_db = new GeneralDbContext();
            gen_db.Database.Initialize(true);

            Database.SetInitializer(new ApplicationDbInitializer());
            ApplicationDbContext db = new ApplicationDbContext();
            db.Database.Initialize(true);



            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
