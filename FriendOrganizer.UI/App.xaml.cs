using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.ViewModel;
using System.Windows;
using Autofac;
using FriendOrganizer.UI.Startup;

namespace FriendOrganizer.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Bootstrap();
//            var mainWindow = new MainWindow(new MainViewModel(new FriendDataService()));
            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }
    }
}
