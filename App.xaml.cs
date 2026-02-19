using System;
using System.Windows;
using System.Windows.Threading;

namespace Device_Library_WPF
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "WPF crash", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true; // чтобы не сдохло сразу
        }

        private void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            MessageBox.Show(ex?.ToString() ?? e.ExceptionObject.ToString() ?? "Unknown", "Fatal crash",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
