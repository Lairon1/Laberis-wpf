using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Laberis.com.lairon.laberis.server.exception;
using Laberis.com.lairon.laberis.server.model;
using Laberis.com.lairon.laberis.winclient.wpf.login;

namespace Laberis.com.lairon.laberis.winclient.wpf.page
{
    public partial class ProductsPage : Page
    {
        public String[] sortsTypes =
        {
            "Нет сортировки",
            "Цена по убыванию",
            "Цена по возрастанию"
        };

        private Collection<Product> currentProducts;

        public IEnumerable<Product> viewedProducts
        {
            get
            {
                if (currentProducts == null) return null;
                IEnumerable<Product> result = new Collection<Product>(currentProducts);

                if (SearchBox.Text.Length != 0)
                {
                    result = result.Where(product =>
                        product.titleWithType.ToLower().Contains(SearchBox.Text.ToLower()));
                }

                if (SortBox.SelectedIndex != 0)
                {
                    switch (SortBox.SelectedIndex)
                    {
                        case 1:
                            result = result.OrderByDescending(product => product.price);
                            break;
                        case 2:
                            result = result.OrderBy(product => product.price);
                            break;
                    }
                }

                if (FillterComboBox.SelectedIndex != 0)
                {
                    ProductType fillter;
                    ProductType.TryParse(FillterComboBox.SelectedIndex - 1 + "", out fillter);
                    result = result.Where(product => product.productType == fillter);
                }

                return result;
            }
        }

        public ProductsPage()
        {
            InitializeComponent();
            SortBox.ItemsSource = sortsTypes;

            Collection<string> fillters = new Collection<string>();
            fillters.Add("Нет фильтра");
            foreach (ProductType type in Enum.GetValues(typeof(ProductType)))
            {
                fillters.Add(typeToRus(type));
            }

            FillterComboBox.ItemsSource = fillters;
            FillterComboBox.SelectedIndex = 0;
            SortBox.SelectedIndex = 0;
            reloadProductsFromServer();
        }

        public string typeToRus(ProductType productType)
        {
            switch (productType)
            {
                case ProductType.ORM:
                    return "Оперативная память";
                case ProductType.DISK:
                    return "Жесткий диск";
                case ProductType.FRAME:
                    return "Корпус";
                case ProductType.COOLER:
                    return "Охлаждение";
                case ProductType.MONITOR:
                    return "Монитор";
                case ProductType.POWER_UNIT:
                    return "Блок питания";
                case ProductType.PROCESSOR:
                    return "Процессор";
                case ProductType.VIDEO_CARD:
                    return "Видео карта";
                case ProductType.MOTHER_BOARD:
                    return "Материнская плата";
            }

            return "";
        }

        private async void reloadProductsFromServer()
        {
            Application.MainWindow.sendInfo("Загрузка товаров");
            try
            {
                currentProducts = await Application.LaberisServer.getAllProducts();
                Application.MainWindow.sendInfo("Товары успешно обновлены");

                updateList(viewedProducts);
            }
            catch (Exception e)
            {
                Application.MainWindow.sendError("Произошла неизвестная ошибка сервера. Попробуйте позже." +
                                                 e.GetType().Name + " " + e.Message);
            }
        }

        private void updateList(IEnumerable<Product> list)
        {
            if (list == null) return;
            ProductsListView.ItemsSource = list;
        }


        private void profileButtonClick(object sender, MouseButtonEventArgs e)
        {
            Configuration.saveCurrentUser(null);
            Application.User = null;
            Application.MainWindow.changePage(new LoginPage());
            Application.MainWindow.sendInfo("Вы вышли из аккаунта");
        }

        private void reloadButtonClick(object sender, MouseButtonEventArgs e) => reloadProductsFromServer();

        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e) => updateList(viewedProducts);

        private void SortBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            updateList(viewedProducts);

        private void FillterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            updateList(viewedProducts);

        private void ProductsListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductsListView.SelectedItem == null) return;
            Product selectedItem = (Product)ProductsListView.SelectedItem;
            Application.MainWindow.changePage(new ViewProductPage(selectedItem, this));
        }

        private void createProductButtonClick(object sender, MouseButtonEventArgs e)
        {
            Application.MainWindow.changePage(new SaveCreateProductPage(this, product =>
            {
                currentProducts.Add(product);
                updateList(viewedProducts);
            }));
        }

        public void deleteProduct(Product product)
        {
            try
            {
                Application.LaberisServer.deleteProduct(Application.User, product);
                currentProducts.Remove(product);
                updateList(viewedProducts);
            }
            catch (DontHavePermissionsException)
            {
                Application.MainWindow.sendError("У вас недостаточно прав для удаления товаров.");
                return;
            }
            catch (Exception e)
            {
                Application.MainWindow.sendError("Произошла неизвестная ошибка сервера. Попробуйте позже." +
                                                 e.GetType().Name + " " + e.Message);
                return;
            }
        }

        private void deleteContextMenuButtonClick(object sender, RoutedEventArgs e) =>
            deleteProduct((Product)ProductsListView.SelectedItem);

        private void deletButtonClick(object sender, MouseButtonEventArgs e)
        {
            Product selectedItem = (Product)ProductsListView.SelectedItem;
            if (selectedItem == null)
            {
                Application.MainWindow.sendError("Товар не выбран");
                return;
            }

            deleteProduct(selectedItem);
        }

        private void editProduct(Product product)
        {
            Application.MainWindow.changePage(new SaveCreateProductPage(this, product1 =>
            {
                updateList(viewedProducts);
            }, product));
        }
        
        private void editProductContextMenuButtonClick(object sender, RoutedEventArgs e)
        {
            if (ProductsListView.SelectedItem == null) return;
            editProduct((Product) ProductsListView.SelectedItem);
            
        }

        private void editProductButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductsListView.SelectedItem == null)
            {
                Application.MainWindow.sendError("Товар не выбран");
                return;
            }
            editProduct((Product) ProductsListView.SelectedItem);
        }
    }
}