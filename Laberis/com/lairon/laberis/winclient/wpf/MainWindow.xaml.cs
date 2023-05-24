using System;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Laberis.com.lairon.laberis.data;
using Laberis.com.lairon.laberis.server;
using Laberis.com.lairon.laberis.server.model;
using Laberis.com.lairon.laberis.winclient.wpf.login;
using Laberis.com.lairon.laberis.winclient.wpf.page;
using Application = Laberis.com.lairon.laberis.winclient.Application;

namespace Laberis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private Page currentPage;
        
        public MainWindow()
        {
            InitializeComponent();
            Application.MainWindow = this;
            try
            {
                Application.LaberisServer = new RestAPIDataProvider("http://localhost:8080");
            }
            catch(Exception)
            {
                sendError("Сервер в данное время недоступен. Пожалуйста попробуйте позже");
            }
            
            
            loadPage();
        }

        private async void loadPage()
        {
            User user = Configuration.loadCurrentUser();
            if (user == null)
            {
                currentPage = new LoginPage();
            }
            else
            {
                bool session = await Application.LaberisServer.checkSession(user);
                if (session)
                {
                    Application.User = user;

                    currentPage = new ProductsPage();
                    sendInfo("Добро пожаловать " + user.firstname + " " + user.lastname);
                }
                else
                {
                    currentPage = new LoginPage();
                    Configuration.saveCurrentUser(null);
                    sendError("К сожелению время вашей сессии закончилось, пожалуйста войдите еще раз.");
                }
            }
            
            MainFrame.Content = currentPage;

            DoubleAnimation opacityAnimationFrom = new DoubleAnimation();
            opacityAnimationFrom.From = 0f;
            opacityAnimationFrom.To = 1;
            opacityAnimationFrom.Duration = TimeSpan.FromSeconds(3);
            opacityAnimationFrom.EasingFunction = new QuadraticEase();
            
            currentPage.BeginAnimation(OpacityProperty, opacityAnimationFrom);
        }
        
        public async void changePage(Page page)
        {
            MainFrame.IsHitTestVisible = false;
            DoubleAnimation opacityAnimationFrom = new DoubleAnimation();
            opacityAnimationFrom.From = 1f;
            opacityAnimationFrom.To = 0;
            opacityAnimationFrom.Duration = TimeSpan.FromSeconds(0.2);
            opacityAnimationFrom.EasingFunction = new QuadraticEase();
            
            DoubleAnimation opacityAnimationTo = new DoubleAnimation();
            opacityAnimationTo.From = 0;
            opacityAnimationTo.To = 1f;
            opacityAnimationTo.Duration = TimeSpan.FromSeconds(0.3);
            opacityAnimationTo.EasingFunction = new QuadraticEase();
            
            currentPage.BeginAnimation(OpacityProperty, opacityAnimationFrom);
            
            await Task.Delay(200);

            MainFrame.Content = page;
            currentPage = page;
            
            currentPage.BeginAnimation(OpacityProperty, opacityAnimationTo);
            await Task.Delay(300);
            MainFrame.IsHitTestVisible = true;
        }
        

        public async void sendNotification(string titleText, Brush titleColor, string contentText, SystemSound sound)
        {
            Border border = new Border();
            border.MaxWidth = 400;
            border.Margin = new Thickness(0, 7, 7, 0);
            border.Background = Brushes.White;
            border.VerticalAlignment = VerticalAlignment.Top;
            border.HorizontalAlignment = HorizontalAlignment.Right;
            border.Padding = new Thickness(20);
            border.CornerRadius = new CornerRadius(20);
            border.Opacity = 0.75f;
            border.IsHitTestVisible = false;
            
            StackPanel stackPanel = new StackPanel();
            stackPanel.Opacity = 0.75f;


            TextBlock title = new TextBlock();
            title.Text = titleText;
            title.Foreground = titleColor;
            title.FontSize = 15;
            title.FontWeight = FontWeights.Bold;


            TextBlock content = new TextBlock();
            content.Text = contentText;
            content.FontSize = 15;
            content.FontWeight = FontWeights.Bold;
            content.Margin = new Thickness(0, 10, 0, 0);
            content.TextWrapping = TextWrapping.Wrap;
            
            stackPanel.Children.Add(title);
            stackPanel.Children.Add(content);
            border.Child = stackPanel;
            

            NotificationStackPanel.Children.Insert(0, border);
            DoubleAnimation maxHeightAnimationFrom = new DoubleAnimation();
            maxHeightAnimationFrom.From = 0;
            maxHeightAnimationFrom.To = 300;
            maxHeightAnimationFrom.Duration = TimeSpan.FromSeconds(0.5f);
            maxHeightAnimationFrom.EasingFunction = new QuadraticEase();
            
            DoubleAnimation opacityAnimationFrom = new DoubleAnimation();
            opacityAnimationFrom.From = 0f;
            opacityAnimationFrom.To = 0.75f;
            opacityAnimationFrom.Duration = TimeSpan.FromSeconds(0.5f);
            opacityAnimationFrom.EasingFunction = new QuadraticEase();
            
            
            border.BeginAnimation(MaxHeightProperty, maxHeightAnimationFrom);
            border.BeginAnimation(OpacityProperty, opacityAnimationFrom);
            
            if (sound != null)
                sound.Play();
            
            await Task.Delay(5000);
            
            DoubleAnimation maxHeightAnimationTo = new DoubleAnimation();
            maxHeightAnimationTo.From = 300;
            maxHeightAnimationTo.To = 0;
            maxHeightAnimationTo.Duration = TimeSpan.FromSeconds(0.5f);
            maxHeightAnimationTo.EasingFunction = new QuadraticEase();
            
            DoubleAnimation opacityAnimationTo = new DoubleAnimation();
            opacityAnimationTo.From = 0.75f;
            opacityAnimationTo.To = 0;
            opacityAnimationTo.Duration = TimeSpan.FromSeconds(0.5f);
            opacityAnimationTo.EasingFunction = new QuadraticEase();
            
            border.BeginAnimation(MaxHeightProperty, maxHeightAnimationTo);
            border.BeginAnimation(OpacityProperty, opacityAnimationTo);
            
            await Task.Delay(500);
            
            NotificationStackPanel.Children.Remove(border);


            

        }

        public void sendInfo(string info) =>
            sendNotification("Информация", Brushes.Aqua, info, null);

        public void sendWarning(string warning) =>
            sendNotification("Предупреждение", Brushes.Orange, warning, SystemSounds.Beep);

        public void sendError(string error) => sendNotification("Ошибка", Brushes.Red, error, SystemSounds.Beep);

        private void DragWin(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            DragMove();
        }

        private void CloseButtonClick(object sender, MouseButtonEventArgs e) => Close();

        private void MinButtonClick(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;

        private void MaxWindowButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                return;                
            }

            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                return;
            }
        }
    }
}