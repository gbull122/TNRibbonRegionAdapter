using System;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;

namespace TestApplication.Module1
{
    public class RibbonViewModel : BindableBase
    {
        public DelegateCommand Module1Command { get; private set; }

        public RibbonViewModel()
        {
            Module1Command = new DelegateCommand(DoSomething);
        }

        private void DoSomething()
        {
            MessageBox.Show("Module1");
        }
    }
}
