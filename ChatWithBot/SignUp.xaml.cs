using Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chat;
using System.Windows.Media.Animation;
using System.Threading;
using Microsoft.Win32;
using System.IO;

namespace ChatWithBot
{
    /// <summary>
    /// Логика взаимодействия для SignUp.xaml
    /// </summary>
    public partial class SignUp : Page
    {
        DBConnector con = new DBConnector();
        public SignUp()
        {
            InitializeComponent();
        }

        private void Signup_Click_1(object sender, RoutedEventArgs e)
        {
            if(UserName.Text != String.Empty && Log.Text != String.Empty && Pass.Text != String.Empty)
            {
                if(con.CheckLogin(Log.Text) == true)
                {
                    if(Pass.Text.Length >= 6)
                    {
                        if(File.Exists(avatar.Text) == true)
                        {
                            User user = new User();
                            user.Login = Log.Text;
                            user.Password = Pass.Text;
                            user.Name = UserName.Text;
                            user.Image = File.ReadAllBytes(avatar.Text);
                            con.AddNewUser(user);
                            NavigationService.Navigate(new Login());
                        }
                        else
                        {
                            alerttext.Text = "The image file does not exist!";
                            Thread ShowAnimThread = new Thread(() => ShowAnimation());
                            ShowAnimThread.Start();
                        }
                    }
                    else
                    {
                        alerttext.Text = "The password must contain at least 6 characters!";
                        Thread ShowAnimThread = new Thread(() => ShowAnimation());
                        ShowAnimThread.Start();
                    }
                }
                else
                {
                    alerttext.Text = "This login is already registered!";
                    Thread ShowAnimThread = new Thread(() => ShowAnimation());
                    ShowAnimThread.Start();
                }
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); 
        }
        private void ShowAnimation()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ThicknessAnimation animation = new ThicknessAnimation();
                animation.From = new Thickness(0, 0, 15, -180);
                animation.To = new Thickness(0, 0, 15, 15);
                animation.Duration = TimeSpan.FromMilliseconds(500);
                alertbox.BeginAnimation(Border.MarginProperty, animation);
            }));
        }
        private void CloseAnimation()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {

                ThicknessAnimation animation = new ThicknessAnimation();
                animation.From = new Thickness(0, 0, 15, 15);
                animation.To = new Thickness(0, 0, 15, -180);
                animation.Duration = TimeSpan.FromMilliseconds(500);
                alertbox.BeginAnimation(Border.MarginProperty, animation);
            }));
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() => CloseAnimation());
            thread.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg|JPEG files (*.jpeg)|*.jpeg";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
            if (openFileDialog.ShowDialog() == true)
            {
                avatar.Text = openFileDialog.FileName;
            }
        }
    }
}
