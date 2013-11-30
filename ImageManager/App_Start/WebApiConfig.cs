using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using ImageManager.Models;
using Microsoft.Data.Edm;
using System.Web.Http.OData.Builder;

namespace ImageManager
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();

            var edmModel = GetEdmModel();
            config.Routes.MapODataRoute("odata", "odata", edmModel);

            //ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            //builder.EntitySet<ImageModel>("Images2");
            //config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        public static IEdmModel GetEdmModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<ImageModel>("Images");
            builder.Namespace = "ImageManager.Models";  // DON'T FORGET THIS! //WebAPIODataWithBreezeConsumer
            return builder.GetEdmModel();
        }
    }
}
