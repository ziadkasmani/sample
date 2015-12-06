using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;

namespace HeyVoteWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultVideo",
                routeTemplate: "api/{controller}/{action}/{id}/{ff}",
                defaults: new { controller = "Values", action = UrlParameter.Optional, id = UrlParameter.Optional, ff = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{ff}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, ff = UrlParameter.Optional }
            );
        }
    }
}
