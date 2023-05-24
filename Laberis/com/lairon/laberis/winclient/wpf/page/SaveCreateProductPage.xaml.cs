using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Laberis.com.lairon.laberis.server.exception;
using Laberis.com.lairon.laberis.server.model;
using Microsoft.Win32;

namespace Laberis.com.lairon.laberis.winclient.wpf.page
{
    public partial class SaveCreateProductPage : Page
    {
        private Page previousPage;
        private Action<Product> callback;
        private string imagePath;
        private Product edit;

        public SaveCreateProductPage(Page previousPage, Action<Product> callback, Product edit = null)
        {
            this.previousPage = previousPage;
            this.callback = callback;
            this.edit = edit;
            InitializeComponent();

            Collection<string> types = new Collection<string>();
            foreach (ProductType type in Enum.GetValues(typeof(ProductType)))
            {
                types.Add(typeToRus(type));
            }

            TypeComboBox.ItemsSource = types;
            TypeComboBox.SelectedIndex = 0;

            if (edit != null)
            {
                TitleTextBox.Text = this.edit.title;
                DescriptionTextBox.Text = this.edit.description;
                PriceTextBox.Text = this.edit.price + "";
                TypeComboBox.SelectedIndex = (int)this.edit.productType;
                ProductImage.Source = this.edit.image;
                StringBuilder specificationsText = new StringBuilder();
                foreach (var keyValuePair in edit.specifications)
                {
                    specificationsText.Append(keyValuePair.Key + "\n" + keyValuePair.Value + "\n");
                }

                SpecificationsTextBox.Text = specificationsText.ToString();
            }
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

        private void cancelButtonClick(object sender, RoutedEventArgs e)
        {
            Application.MainWindow.changePage(previousPage);
        }

        private async void saveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Product product = generateProduct();
                if (product == null) return;
                Product saveProduct = await Application.LaberisServer.saveProduct(Application.User, product);
                if (imagePath != null && File.Exists(imagePath))
                {
                    Application.LaberisServer.SaveImage(Application.User, saveProduct.id, imagePath);
                }

                callback.Invoke(saveProduct);
                Application.MainWindow.changePage(previousPage);
            }
            catch (DontHavePermissionsException)
            {
                Application.MainWindow.sendError("У вас недостаточно прав чтобы создавать свои товары.");
                return;
            }
            catch (ServerErrorException)
            {
                Application.MainWindow.sendError("Сервер сейчас недоступен, попробуйте позже.");
                return;
            }
            catch (Exception ex)
            {
                Application.MainWindow.sendError("Неизветсная ошибка " + ex.GetType().Name + "  " + ex.Message + " " +
                                                 ex.StackTrace);
            }
        }

        private Product generateProduct()
        {
            Product product = new Product();
            if (edit != null)
            {
                product.id = edit.id;
            }

            string title = TitleTextBox.Text,
                description = DescriptionTextBox.Text,
                specificationsString = SpecificationsTextBox.Text,
                priceString = PriceTextBox.Text;
            if (title.Length == 0)
            {
                Application.MainWindow.sendError("Имя не может быть пустым!");
                return null;
            }

            if (description.Length == 0)
            {
                Application.MainWindow.sendError("Описание не может быть пустым!");
                return null;
            }

            if (specificationsString.Length == 0)
            {
                Application.MainWindow.sendError("Характеристики не могут быть пустыми!");
                return null;
            }

            if (priceString.Length == 0)
            {
                Application.MainWindow.sendError("Цена не может быть пустой!");
                return null;
            }

            decimal price = 0;
            try
            {
                price = decimal.Parse(priceString);
            }
            catch
            {
                Application.MainWindow.sendError("Цена должна быть числом");
                return null;
            }

            Dictionary<string, string> specifications = null;

            try
            {
                specifications = parseSpecifications(specificationsString);
            }
            catch (Exception exception)
            {
                Application.MainWindow.sendError("У каждой характеристики должно быть значение");
                return null;
            }

            product.image = (BitmapSource)ProductImage.Source;

            ProductType type;
            ProductType.TryParse(TypeComboBox.SelectedIndex + "", out type);

            product.title = title;
            product.description = description;
            product.price = price;
            product.productType = type;
            product.specifications = specifications;

            return product;
        }

        private Dictionary<string, string> parseSpecifications(string str)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            string[] strings = str.Split('\n');
            if (strings.Length % 2 != 0)
                throw new ArgumentException();
            for (var i = 0; i < strings.Length; i++)
            {
                string key = strings[i].Replace("\r", "");
                string value = strings[++i].Replace("\r", "");
                map.Add(key, value);
            }

            return map;
        }


        private async void setImageButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Files|*.jpg;*.jpeg;*.png;*.webp;";
            openFileDialog.Title = "Выберете картинку";
            if (openFileDialog.ShowDialog() == false) return;
            BitmapImage image = new BitmapImage(new Uri(openFileDialog.FileName));
            ProductImage.Source = image;
            imagePath = openFileDialog.FileName;
        }

        private void previewButtonClick(object sender, RoutedEventArgs e)
        {
            Product product = generateProduct();
            if (product == null) return;
            Application.MainWindow.changePage(new ViewProductPage(product, this));
        }
    }
}