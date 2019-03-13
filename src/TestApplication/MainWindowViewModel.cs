using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows;
using TestApplication.Module1;

namespace TestApplication
{
	public class MainWindowViewModel:BindableBase
	{
        public DelegateCommand Button1Command { get; private set; }

        public MainWindowViewModel(IRegionManager regManager)
		{
            Button1Command = new DelegateCommand(DoSomething);
        }

        private void DoSomething()
        {
            MessageBox.Show("Main");
        }
    }
}
