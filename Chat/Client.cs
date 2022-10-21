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
        NetworkStream stream;
        public string Connect(User user)
        {
            try
            {
                client = new TcpClient();
                client.Connect(IPAddress.Parse(address), port);
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
                byte[] data = Encoding.Unicode.GetBytes(message);
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
        public string GetConnect()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            bytes = stream.Read(data, 0, data.Length);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            return builder.ToString();
        }

        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;

            bytes = stream.Read(data, 0, data.Length);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));


            return builder.ToString();
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
}
