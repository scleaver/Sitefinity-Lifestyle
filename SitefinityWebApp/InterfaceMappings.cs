using Ninject.Modules;
using SitefinityWebApp.Mvc.Models.ContentFilter;

/// <summary>
/// This class is used to describe the bindings which will be used by the Ninject container when resolving classes
/// </summary>
namespace SitefinityWebApp
{
    public class InterfaceMappings : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            Bind<IContentFilterModel>().To<ContentFilterModel>();
        }
    }
}