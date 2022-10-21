using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chat
{
    public class Server
    {
        static TcpListener tcpListener; 
        private string Ip = "127.0.0.1";
        private int port = 8888;
        public List<UserServer> users = new List<UserServer>();
        public bool working = false;
        private DBConnector con = new DBConnector();

        public void Start()
        {
            tcpListener = new TcpListener(IPAddress.Parse(Ip), port);
            tcpListener.Start();
            working = true;
            Log("=============Server started=============");
            Thread thread = new Thread(() => WaitConnect());
            thread.Start();

        }
        public void WaitConnect()
        {
            try
            {
                while (working)
                {

                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    NetworkStream tmpStream = tcpClient.GetStream();

                    string message = string.Empty;
                    byte[] data_from_client = new byte[100];
                    StringBuilder string_from_client = new StringBuilder();

                    int bytes = tmpStream.Read(data_from_client, 0, data_from_client.Length);

                    string str = Encoding.Unicode.GetString(data_from_client);
                    str = str.Replace("\0", "");
                    Regex regex = new Regex("/reg id:(\\w+)");

                    Match match = regex.Match(str);
                    int id = Convert.ToInt32(match.Groups[1].Value);
                    UserServer userLocal = new UserServer(id, tcpClient, tmpStream);
                    users.Add(userLocal);   
                    Log($"User {id} is connected!");
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        
        


        public void SendMessage(string message, int id)
        {
            try
            {
                Log($"Server: {message} [to {clients.Where(s => s.Id == id).First().Login}]!");
                byte[] data = Encoding.Unicode.GetBytes(message);
                clients.Where(s => s.Id == id).First().Stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Log($"SendMessage Error: {ex.Message}");
            }
        }
        public void Log(string str)
        {
            lock (this)
            {
                string log = $"[{DateTime.Now.ToString()}] {str}";
                using (FileStream fs = new FileStream("log.txt", FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(log);
                    sw.Close();
                    fs.Close();
                }
            }
        }
        public void RemoveClient(UserServer user)
        {
            Thread.Sleep(100);
            users.Remove(user);
        }
        public void Disconnect()
        {
            working = false;
            for (int i = 0; i < clients.Count; i++)
            {
                SendMessage("Server is closed!", clients[i].Id);
            }
            tcpListener.Stop();

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Stream.Close();
                clients[i].Client.Close();
            }
            Log("Server was stopped!");
            Environment.Exit(0);
        }


    }
}
