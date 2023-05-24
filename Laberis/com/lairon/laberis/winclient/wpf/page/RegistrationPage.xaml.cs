using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Laberis.com.lairon.laberis.server.exception;
using Laberis.com.lairon.laberis.server.model;
using Laberis.com.lairon.laberis.winclient.wpf.login;

namespace Laberis.com.lairon.laberis.winclient.wpf.page
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            Application.MainWindow.changePage(new LoginPage());
        }

        private async void RegistrButtonClick(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text,
                password = PasswordTextBox.Password,
                firstname = FirstnameTextBox.Text,
                lastname = LastnameTextBox.Text;
            if (login.Length == 0)
            {
                LoginTextBox.BorderBrush = Brushes.Red;
                Application.MainWindow.sendWarning("Поле логина не должно быть пустым");
                return;
            }
            if (password.Length == 0)
            {
                PasswordTextBox.BorderBrush = Brushes.Red;
                Application.MainWindow.sendWarning("Поле пароля не должно быть пустым");
                return;
            }
            if (firstname.Length == 0)
            {
                FirstnameTextBox.BorderBrush = Brushes.Red;
                Application.MainWindow.sendWarning("Поле имени не должно быть пустым");
                return;
            }
            if (lastname.Length == 0)
            {
                LastnameTextBox.BorderBrush = Brushes.Red;
                Application.MainWindow.sendWarning("Поле фамилии не должно быть пустым");
                return;
            }
        
            LastnameTextBox.BorderBrush = Brushes.Gray;
            FirstnameTextBox.BorderBrush = Brushes.Gray;
            PasswordTextBox.BorderBrush = Brushes.Gray;
            LoginTextBox.BorderBrush = Brushes.Gray;

            
            try
            {
                User user = await Application.LaberisServer.Register(new User(login, firstname, lastname, null, password));
                Application.MainWindow.sendInfo("Регистрация успешно завершена!");
                Application.User = user;
                Configuration.saveCurrentUser(user);
                Application.MainWindow.changePage(new ProductsPage());
            }
            catch (Exception ex)
            {
                if (ex is SimplePasswordException)
                {
                    PasswordTextBox.BorderBrush = Brushes.Red;
                    Application.MainWindow.sendError(
                        "Ваш пароль слишком простой. Используйте другой");
                }
                else if (ex is LoginAlreadyExistsException)
                {
                    LoginTextBox.BorderBrush = Brushes.Red;
                    Application.MainWindow.sendError(
                        "Логин " + login + " уже используется. Используйте другой");
                }
                else if (ex is ServerErrorException)
                {
                    Application.MainWindow.sendError(
                        "Нейзвестная ошибка сервера. Пожалуйста попробуйте позже.");
                }
                else
                    Application.MainWindow.sendError(
                        "Сервер в данное время недоступен. Пожалуйста попробуйте позже. ");
            }
            

        }
    }
}