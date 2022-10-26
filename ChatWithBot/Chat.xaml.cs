using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chat;

namespace ChatWithBot
{
    /// <summary>
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Chat : Page
    {
        Client client = new Client();
        UserLocal current_user;
        User User { get; set; }
        DBConnector con = new DBConnector();
        bool server_connected = false;
        List<Thread> threads = new List<Thread>();
        public Chat(User user)
        {
            this.User = user;
            InitializeComponent();
            
            lv.ItemsSource = con.GetUsesrLocal(user.Id);
            if(lv.Items.Count > 0)
                lv.SelectedIndex = 0;
            if (client.Connect(user) == "Correct")
            {
                server_connected = true;
                Thread read = new Thread(ReadServer);
                read.Start();
                threads.Add(read);
            }
            else
            {
                Thread ShowAnimAlert = new Thread(ShowNoCon);
                ShowAnimAlert.Start();
                Thread reconnect = new Thread(Reconnect);
                reconnect.Start();
                threads.Add(reconnect);
                threads.Add(ShowAnimAlert);
            }

        }
        private void Reconnect()
        {
            while(server_connected == false)
            {
                if (client != null)
                {
                    if (client.Connect(User) == "Correct")
                    {
                        server_connected = true;
                        Thread read = new Thread(ReadServer);
                        read.Start();
                        Thread ShowAnimAlert = new Thread(CloseNoCon);
                        ShowAnimAlert.Start();
                        threads.Add(read);
                        threads.Add(ShowAnimAlert);
                    }
                }
                Thread.Sleep(500);
            }
        }
        private static BitmapImage LoadImage(byte[] imageData)
        {
            BitmapImage image = new BitmapImage();
            using (MemoryStream mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void lv_Selected(object sender, RoutedEventArgs e)
        {

        }
        
        private void UpdateMessage()
        {
            if (current_user != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    List<MessageNew> messageNews = new List<MessageNew>();

                    List<Message> messages = con.GetMessage(User.Id, current_user.Id);
                    foreach (Message message in messages)
                    {
                        MessageNew messageNew = new MessageNew();
                        messageNew.Sender_Id = message.Sender_Id;
                        messageNew.Created = message.Created;
                        messageNew.Destination_Id = message.Destination_Id;
                        messageNew.Id = message.Id;
                        messageNew.Title = message.Title;
                        if (message.Sender_Id == User.Id)
                        {
                            messageNew.Alig = HorizontalAlignment.Right;
                            messageNew.Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#163B60"));
                        }
                        else
                        {
                            messageNew.Alig = HorizontalAlignment.Left;
                            messageNew.Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#393E46"));
                        }
                        messageNews.Add(messageNew);
                    }
                    List<UserLocal> ul = lv.Items.Cast<UserLocal>().ToList();
                    UserLocal userLocal = ul.Where(s => s.Id == current_user.Id).First();
                    userLocal.LastMessage = messageNews[messageNews.Count - 1].Title;
                    ul[ul.IndexOf(userLocal)] = userLocal;

                    mes.ItemsSource = messageNews;
                    mes.SelectedIndex = mes.Items.Count - 1;
                    mes.ScrollIntoView(mes.SelectedItem);
                }));

            }
        }
        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            current_user = lv.SelectedItem as UserLocal;
            UpdateMessage();
            
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (Key.Enter == e.Key)
                SendMessage();
        }
        private void SendMessage()
        {
            if (client.stream != null)
            {
                if (client.client.Connected)
                {
                    UserLocal local = lv.SelectedItem as UserLocal;
                    if (local != null && TBMes.Text != string.Empty && server_connected == true)
                    {
                        Message message2 = new Message();
                        message2.Sender_Id = User.Id;
                        message2.Created = DateTime.Now;
                        message2.Title = TBMes.Text;
                        message2.Destination_Id = local.Id;
                        client.SendMessage(message2.Title, message2.Destination_Id);
                        con.localdb.Messages.Add(message2);
                        con.localdb.SaveChanges();
                        UpdateMessage();
                        TBMes.Text = "";
                    }
                }
                else
                {
                    server_connected = false;
                    Thread ShowAnimAlert = new Thread(ShowNoCon);
                    ShowAnimAlert.Start();
                    Thread reconnect = new Thread(Reconnect);
                    reconnect.Start();
                    threads.Add(reconnect);
                    threads.Add(ShowAnimAlert);
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            User user = con.GetUser(SearchUser.Text);
            if(user != null && user.Id != User.Id)
            {
                con.AddLocalUser(user, User.Id);
                lv.ItemsSource = con.GetUsesrLocal(User.Id);
            }
            else
            {
                alerttext.Text = $"User {SearchUser.Text} not found";
                Thread ShowAnimAlert = new Thread(ShowAnimation);
                ShowAnimAlert.Start();
                threads.Add(ShowAnimAlert);
            }
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

        private void ShowNoCon()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ThicknessAnimation animation = new ThicknessAnimation();
                animation.From = new Thickness(0, -45, 0, 0);
                animation.To = new Thickness(0, 0, 0, 0);
                animation.Duration = TimeSpan.FromMilliseconds(500);
                NoCon.BeginAnimation(Border.MarginProperty, animation);
            }));
        }
        private void CloseNoCon()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {

                ThicknessAnimation animation = new ThicknessAnimation();
                animation.From = new Thickness(0, 0, 0, 0);
                animation.To = new Thickness(0, -45, 0, 0);
                animation.Duration = TimeSpan.FromMilliseconds(500);
                NoCon.BeginAnimation(Border.MarginProperty, animation);
            }));
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() => CloseAnimation());
            thread.Start();
            threads.Add(thread);
        }

        private void SendMes_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }
        private void ReadServer()
        {
            while (server_connected)
            {

                string mes = client.GetMessage();
                mes = mes.Replace("\0", "");
                if (mes == "Server is closed")
                {
                    server_connected = false;
                    alerttext.Text = "Server is closed. Sending and receiving messages does not work!";
                    Thread ShowAnimAlert = new Thread(ShowAnimation);
                    ShowAnimAlert.Start();
                    threads.Add(ShowAnimAlert);
                }
                else if (mes == "disconnected")
                {
                    Thread show = new Thread(WriteThreads);
                    show.Start();
                    server_connected = false;
                    Thread ShowAnimAlert = new Thread(ShowNoCon);
                    ShowAnimAlert.Start();
                    Thread reconnect = new Thread(Reconnect);
                    reconnect.Start();
                    threads.Add(ShowAnimAlert);
                    threads.Add(reconnect);
                }
                else if (mes == "clossing")
                {
                    server_connected = false;
                }
                else
                {

                    Regex regex = new Regex("(.*) \\[to (\\w+)] \\[from (\\w+)]");

                    Match match = regex.Match(mes);
                    string message = match.Groups[1].Value;
                    int id_from = Convert.ToInt32(match.Groups[3].Value);
                    int id_to = Convert.ToInt32(match.Groups[2].Value);
                    Message message2 = new Message();
                    message2.Sender_Id = id_from;
                    message2.Created = DateTime.Now;
                    message2.Title = message;
                    message2.Destination_Id = id_to;
                    con.localdb.Messages.Add(message2);
                    con.localdb.SaveChanges();
                    //if(con.GetUsesrLocal(User.Id).Where(s => s.Id == id_from).FirstOrDefault() == null)
                    //{
                    //    con.AddLocalUser(con.GetUserFromId(id_from), User.Id);

                    //}
                    UpdateMessage();
                }

            }
        }
        

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow w = Application.Current.MainWindow as MainWindow;
            w.DragMove();
        }
       

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainWindow w = Application.Current.MainWindow as MainWindow;
            client.SendMessage("clossing");
            
            client.Close();
            client.stream.Close();
            client = null;
            w.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MainWindow w = Application.Current.MainWindow as MainWindow;
            if (w.WindowState == WindowState.Maximized)
                w.WindowState = WindowState.Normal;
            else
                w.WindowState = WindowState.Maximized;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MainWindow w = Application.Current.MainWindow as MainWindow;
            w.WindowState = WindowState.Minimized;
        }
        private void WriteThreads()
        {
            ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;

            foreach (ProcessThread thread in currentThreads)
            {
                string str = thread.StartAddress + " " + thread.Id + "" + thread.ThreadState;
                File.WriteAllText("/log/log.txt", str);

            }
        }
    }
}
