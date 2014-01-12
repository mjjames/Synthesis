using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace mjjames.AdminSystem.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // *** This Line ***
            routes.IgnoreRoute("");                                     // Required for the default document in IIS to work
            // *****************
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}"); //todo: remove this and use routing for web form pages

            routes.IgnoreRoute("{folder}/{*pathInfo}", new { folder = "content" });
            routes.IgnoreRoute("{folder}/{*pathInfo}", new { folder = "scripts" });
            routes.IgnoreRoute("{folder}/{*pathInfo}", new { folder = "uploads" });


            //routes.IgnoreRoute("elmah.axd");
            routes.IgnoreRoute("glimpse.axd");

            routes.MapPageRoute("Login", "authentication/login", "~/authentication/default.aspx");

            routes.MapPageRoute("DBEditor", "editor/{type}/{key}/{*fkey}", "~/DBEditor.aspx", false,
                                            new RouteValueDictionary { 
                                                { "fkey", null} 
                                            });

            routes.MapPageRoute("DBListing", "listing/{type}/{*fkey}", "~/DBListing.aspx", false,
                                                new RouteValueDictionary { 
                                                    { "fkey", null } 
                                                });

            routes.MapPageRoute("ImageResizer", "image/{action}/{height}/{width}", "~/loadimage.aspx");

            //default
            routes.MapRoute("Default", // Route name
                            "{controller}/{action}/{id}", // URL with parameters
                            new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );


        }
    }
}