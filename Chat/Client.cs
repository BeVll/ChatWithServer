using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Chat
{
    public class Client
    {
        private int port = 8888;
        private string address = "127.0.0.1";
        public User user;
        public TcpClient client = new TcpClient();
        public NetworkStream stream;
        public string Connect(User user)
        {
            
            try
            {
                this.user = user;
                client = new TcpClient();    
                client.ConnectAsync(IPAddress.Parse(address), port).Wait(1000);
                stream = client.GetStream();
                Log($"User {user.Login} connected to server");
                SendMessage($"/reg id:{user.Id}");
                return "Correct";
            }
            catch (Exception ex)
            {
                Log($"Connect Error: {ex.Message}");
                return $"Connect error: {ex.Message}";
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
        
        public void SendMessage(string message, int id)
        {
            try
            {
                string mes = $"{message} [from {user.Id}] [to {id}]";
                byte[] data = Encoding.Unicode.GetBytes(mes);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Log($"SendMessage to user Error: {ex.Message}");

            }
        }
        public void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Log($"SendMessage to server Error: {ex.Message}");

            }
        }
       
        public string GetMessage()
        {
            try
            {
                byte[] data = new byte[64];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;

                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                return builder.ToString();
            }
            catch(Exception ex)
            {
                Log(ex.Message);
                return "disconnected";
            }
        }
        public void Close()
        {
            if (client != null)
                client.Close();
            if (stream != null)
                stream.Close();
        }
    }

}

