using System.Windows;
using Device_Library_WPF.ViewModels;

namespace Device_Library_WPF.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new MainViewModel();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			await ((MainViewModel)DataContext).LoadAsync();
		}
	}

}
