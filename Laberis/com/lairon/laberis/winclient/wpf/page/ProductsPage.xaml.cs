using System.Windows;
using System.Windows.Controls;
using Laberis.com.lairon.laberis.winclient.wpf.login;

namespace Laberis.com.lairon.laberis.winclient.wpf.page
{
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
        }

        private void AccountButtonClick(object sender, RoutedEventArgs e)
        {
            Application.MainWindow.sendInfo("Вы вышли из аккаунта");
            Application.MainWindow.changePage(new LoginPage());
        }
    }
}