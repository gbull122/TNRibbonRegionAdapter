using CommonServiceLocator;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace TestApplication.Module1
{
    class Module1 : IModule
    {

        private readonly IRegionManager regionManager;

        public Module1(IRegionManager regManager)
        {
            regionManager = regManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            regionManager.RegisterViewWithRegion("MainMenu", typeof(RibbonView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
           
        }

    }
}
