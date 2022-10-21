using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chat
{
    public class UserServer
    {
        public int Id { get; set; }
        public TcpClient Client { get; set; }
        public NetworkStream Stream { get; set; }
        private Server server = new Server();
        public UserServer() { }
        public UserServer(int id, TcpClient tcpClient, NetworkStream networkStream)
        {
            Id = id;
            Client = tcpClient;
            Stream = networkStream;
        }
        public string GetMessage()
        {
            try
            {
                if (Client.Connected == true)
                {
                    string message = string.Empty;
                    byte[] data_from_client = new byte[100];
                    int bytes = Stream.Read(data_from_client, 0, data_from_client.Length);
                    string str = Encoding.Unicode.GetString(data_from_client);
                    str = str.Replace("\0", "");
                    Regex regex = new Regex("(\\w+) //from (\\w+) to (\\w+)");

                    Match match = regex.Match(str);

                    int id = Convert.ToInt32(match.Groups[3].Value);

                    return "Disconnecting...The maximum number of requests has been reached";
                }
                else
                {
                    Client.Close();
                    Stream.Close();
                    server.RemoveClient(this);
                    Log($"Server: User {Id} is disconnected!");
                    return $"Server: User {Id} is disconnected!";
                }
            }
            catch (Exception ex)
            {
                Log($"GetMessage Error: {ex.Message}");
                return ex.Message;
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
    }
}
