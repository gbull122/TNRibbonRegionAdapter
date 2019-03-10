using Prism.Mvvm;
using System.Windows.Input;

namespace TestApplication.Module1
{
	public class EditorContextMenuViewModel:BindableBase
	{
		public EditorContextMenuViewModel(Module1HelloCommand cmd)
		{
			HelloCommand = cmd;
		}
		public ICommand HelloCommand { get; set; }
	}
}
