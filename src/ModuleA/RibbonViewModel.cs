using Prism.Commands;
using Prism.Mvvm;
using System.Windows;

namespace ModuleA
{
    public class RibbonViewModel:BindableBase
    {
        private string _title = "ModuleA";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand Command1 { get; private set; }

        public DelegateCommand CustomPopupCommand { get; private set; }

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
