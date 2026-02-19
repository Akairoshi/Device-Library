using Device_Library_WPF.ViewModels;
using System.Windows;

namespace Device_Library_WPF.Views
{
	public partial class AddWindow : Window
	{
		public AddWindow(AddDeviceViewModel vm)
		{
			InitializeComponent();
			DataContext = vm;

			vm.RequestClose += ok =>
			{
				DialogResult = ok;
				Close();
			};
		}

	}
}