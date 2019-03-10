using CommonServiceLocator;
using Prism.Ioc;
using Prism.Regions;
using Prism.Unity;
using System.Windows;
using System.Windows.Controls.Ribbon;
using TNRibbonRegionAdapter;

namespace TestApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            regionAdapterMappings.RegisterMapping(typeof(Ribbon), new RibbonRegionAdapter(ServiceLocator.Current.GetInstance<IRegionBehaviorFactory>()));

            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
