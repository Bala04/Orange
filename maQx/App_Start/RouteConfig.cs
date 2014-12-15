using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace maQx
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "AppLogin",
                url: "Login",
                defaults: new { controller = "App", action = "Login" }
            );

            routes.MapRoute(
                name: "AppLogout",
                url: "Logout",
                defaults: new { controller = "App", action = "Logout" }
            );

            routes.MapRoute(
               name: "CurrentUser",
               url: "get/current-user",
               defaults: new { controller = "App", action = "CurrentUser" }
            );

          //  routes.MapRoute(
          //    name: "Menus",
          //    url: "get/menus",
          //    defaults: new { controller = "App", action = "Menus" }
          // );

          //  routes.MapRoute(
          //    name: "OrganizationList",
          //    url: "get/organizations",
          //    defaults: new { controller = "Organizations", action = "List" }
          // );

          //  routes.MapRoute(
          //   name: "InviteList",
          //   url: "get/invites",
          //   defaults: new { controller = "Invites", action = "List" }
          //);

            routes.MapRoute(
                name: "Application",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "App", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
