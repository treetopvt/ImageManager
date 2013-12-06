using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.IO;
using log4net.Repository.Hierarchy;

[assembly: OwinStartup(typeof(ImageManager.Startup))]

namespace ImageManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            log4net.Config.XmlConfigurator.Configure();
            ConfigureAuth(app);
        }

        //public static void SetAdoNetAppenderConnectionStrings(string connectionStringKey)
        //{
        //    var hier = (Hierarchy)LogManager.GetRepository();
        //    if (hier != null)
        //    {
        //        var appenders = hier.GetAppenders().OfType<log4net.Appender.AdoNetAppender>();
        //        foreach (var appender in appenders)
        //        {
        //            appender.ConnectionString = 
        //            appender.ActivateOptions();
        //        }
        //    }
        //}
    }
}
