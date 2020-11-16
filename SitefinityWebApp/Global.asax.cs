

using SitefinityWebApp.Custom.Models;
using System;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend;
using Telerik.Sitefinity.Frontend.News.Mvc.Models;
using Telerik.Sitefinity.Services;

namespace SitefinityWebApp
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Bootstrapper.Bootstrapped += Bootstrapper_Bootstrapped;
        }

        private void Bootstrapper_Bootstrapped(object sender, EventArgs e)
        {
            if (IsModuleActive("News"))
            {
                FrontendModule.Current.DependencyResolver.Rebind<INewsModel>().To<CustomNewsModel>();
            }
        }

        public bool IsModuleActive(string name)
        {
            var config = Config.Get<SystemConfig>();
            var module = config.ApplicationModules[name];
            return module.StartupType != StartupType.Disabled;
        }
    }
}