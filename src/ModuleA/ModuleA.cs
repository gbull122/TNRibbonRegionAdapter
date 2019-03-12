using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModuleA
{
    public class ModuleA : IModule
    {

        public ModuleA(IRegionManager regManager)
        {
            regManager.RegisterViewWithRegion("RibbonRegion", typeof(RibbonView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
         
        }
    }
}
