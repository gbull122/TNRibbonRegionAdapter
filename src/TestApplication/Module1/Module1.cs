using CommonServiceLocator;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace TestApplication.Module1
{
	class Module1 : IModule
	{

    private readonly IRegionManager _regionManager;

		public Module1(IRegionManager regionManager)
    {
      _regionManager = regionManager;
    }

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion("EditorContextMenu", GetContextMenu);
			//_regionManager.RegisterViewWithRegion(ShellRegions.MainMenu, typeof(Ribbon));
		}

        public void OnInitialized(IContainerProvider containerProvider)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new System.NotImplementedException();
        }

        private object GetContextMenu()
		{
			var cmv = ServiceLocator.Current.GetInstance<EditorContextMenuView>();
			return cmv.ContextMenu;
		}
	}
}
