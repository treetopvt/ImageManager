using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.IO;

[assembly: OwinStartup(typeof(ImageManager.Startup))]

namespace ImageManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
         //   log4net.Config.XmlConfigurator.Configure()
            ConfigureAuth(app);
        }
    }
}
