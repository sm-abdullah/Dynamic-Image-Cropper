using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CropImages.Controllers;

namespace CropImages
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //register the rout for handler with a URL Pattern like http:/localhost/cropImages/x-y-h-w-image.jpg
            //so all the similar url Request are processed by Our Generic Handler
            //our generic handler will parse x as Left , y as Top, H as Height, w as Width
            //So it will start croping from  Point(x,y) and end up point(width,height) of given image
            //where ImageName is the Image path resides on the server http://localhost/Images Directory



            routes.Add(new Route("cropImages/{x}-{y}-{h}-{w}-{imgName}", new ImageCroperRoutHandler()));
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
            
        }
    }
}
