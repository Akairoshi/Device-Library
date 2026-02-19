using System.Windows;
using Device_Library_WPF.ViewModels;

namespace Device_Library_WPF.Views
{
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
            DataContextChanged += InfoWindow_DataContextChanged;
        }

        private void InfoWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is InfoViewModel oldVm)
                oldVm.RequestClose -= OnRequestClose;

            if (e.NewValue is InfoViewModel newVm)
                newVm.RequestClose += OnRequestClose;
        }

        private void OnRequestClose()
        {
            Close();
        }
    }

}
