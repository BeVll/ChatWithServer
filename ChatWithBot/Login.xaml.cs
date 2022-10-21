using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ChatWithBot
{
    /// <summary>
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Login : Page
    {
        DBConnector con = new DBConnector();
        
        public Login()
        {
            InitializeComponent();
        }
        

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            User user = con.db.Users.Where(s => s.Login == Log.Text).FirstOrDefault();
            if(user != null)
            {
                if (user.Password == Pass.Text)
                    NavigationService.Navigate(new Chat(user));
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SignUp());
        }
    }
}
