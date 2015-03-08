using System.Web.Mvc;
using System.Web.Routing;

namespace FidoU2f.Demo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { action = "Index", controller = "Fido", id = UrlParameter.Optional }
            );
        }
    }
}

