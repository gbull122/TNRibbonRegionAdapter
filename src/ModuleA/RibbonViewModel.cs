using Prism.Commands;
using Prism.Mvvm;
using System.Windows;

namespace ModuleA
{
    public class RibbonViewModel:BindableBase
    {
        public DelegateCommand Command1 { get; private set; }

        public RibbonViewModel()
        {
            Command1 = new DelegateCommand(DoSomething);
        }

        private void DoSomething()
        {
            MessageBox.Show("ModuleA");
        }
    }
}
