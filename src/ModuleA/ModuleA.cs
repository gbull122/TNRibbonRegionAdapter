using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModuleA
{
    public class ModuleA : IModule
    {
        IRegionManager regionManager;

        public ModuleA(IRegionManager regManager)
        {
            regionManager = regManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            regionManager.RegisterViewWithRegion("MainMenu", typeof(RibbonView));
        }
    }
}
