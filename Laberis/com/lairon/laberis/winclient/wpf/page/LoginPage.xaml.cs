using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Laberis.com.lairon.laberis.server.exception;
using Laberis.com.lairon.laberis.server.model;
using Laberis.com.lairon.laberis.winclient.wpf.page;

namespace Laberis.com.lairon.laberis.winclient.wpf.login
{
    public partial class LoginPage : Page
    {
        private bool waitingnServer = false;

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void AuthButtonClick(object sender, RoutedEventArgs e)
        {
            if (waitingnServer)
            {
                Application.MainWindow.sendError("Уже идет попытка авторизации...");
                return;
            }

            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;
            if (login.Length == 0)
            {
                Application.MainWindow.sendWarning("Укажите логин");
                LoginTextBox.BorderBrush = Brushes.Red;
                return;
            }

            if (password.Length == 0)
            {
                Application.MainWindow.sendWarning("Укажите пароль");
                PasswordTextBox.BorderBrush = Brushes.Red;
                return;
            }

            LoginTextBox.BorderBrush = Brushes.Gray;
            PasswordTextBox.BorderBrush = Brushes.Gray;
            try
            {
                waitingnServer = true;
                User user = await Application.LaberisServer.Login(login, password);
                Application.User = user;
                Application.MainWindow.sendInfo("Успешная авторизация!");
                Configuration.saveCurrentUser(user);
                Application.MainWindow.changePage(new ProductsPage());
            }
            catch (Exception exception)
            {
                if (exception is UserNotFoundException)
                {
                    Application.MainWindow.sendError("Пользователь " + login + " не найден");
                    LoginTextBox.BorderBrush = Brushes.Red;
                }
                else if (exception is WrongPasswordException)
                {
                    Application.MainWindow.sendError("Неверный пароль");
                    PasswordTextBox.BorderBrush = Brushes.Red;
                }
                else if (exception is ServerErrorException)
                {
                    Application.MainWindow.sendError(
                        "Нейзвестная ошибка сервера. Пожалуйста попробуйте позже.");
                }
                else
                    Application.MainWindow.sendError(
                        "Сервер в данное время недоступен. Пожалуйста попробуйте позже. " + exception.Message);
            }
            finally
            {
                waitingnServer = false;
            }
        }

        private void RegisterButtonClick(object sender, RoutedEventArgs e)
        {
            Application.MainWindow.changePage(new RegistrationPage());
        }
    }
}