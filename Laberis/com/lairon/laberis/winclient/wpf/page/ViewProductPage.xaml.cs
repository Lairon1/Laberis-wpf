using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Laberis.com.lairon.laberis.server.model;

namespace Laberis.com.lairon.laberis.winclient.wpf.page
{
    public partial class ViewProductPage : Page
    {
        private Page previousPage;
        public Product product { get; set; }

        public ViewProductPage(Product product, Page previousPage)
        {
            this.previousPage = previousPage;
            this.product = product;
            DataContext = this;
            InitializeComponent();
            
            
        }

        private void previousPageButtonClick(object sender, MouseButtonEventArgs e)
        {
            Application.MainWindow.changePage(previousPage);
        }

        private bool descriptionCollapsed = true;
        
        
        private void DescriptionCollapseButtonClick(object sender, RoutedEventArgs e)
        {
            if (descriptionCollapsed)
            {
                descriptionCollapsed = false;
                DescriptionText.Text = product.description;
                DescriptionCollapseButton.Content = "Свернуть 🢁";
            }
            else
            {
                descriptionCollapsed = true;
                DescriptionText.Text = product.descriptionCutted;
                DescriptionCollapseButton.Content = "Развернуть 🢃";

            }
        }
    }
}