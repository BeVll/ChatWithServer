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
        public UserServer(int id, TcpClient tcpClient, NetworkStream networkStream, Server server)
        {
            Id = id;
            Client = tcpClient;
            Stream = networkStream;
            this.server = server;
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
                    if (str == "disconnecting")
                    {
                        server.SendMessage("disconnecting", Id);
                        Client.Close();
                        Stream.Close();
                        server.users.Remove(this);
                        Log($"Server: User {Id} is disconnected!");
                        return $"Server: User {Id} is disconnected!";
                    }
                    else if (str == "clossing")
                    {
                        server.SendMessage("clossing", Id);
                        Client.Close();
                        Stream.Close();
                        server.users.Remove(this);
                        Log($"Server: User {Id} is disconnected!");
                        return $"Server: User {Id} is disconnected!";
                    }
                    else
                    {
                        Regex regex = new Regex("(.*) \\[from (\\w+)\\] \\[to (\\w+)\\]");

                        Match match = regex.Match(str);
                        string mes = match.Groups[1].Value;
                        int id_from = Convert.ToInt32(match.Groups[2].Value);
                        int id_to = Convert.ToInt32(match.Groups[3].Value);
                        server.SendMessage(mes, id_from, id_to);
                        return str;
                    }
                }
                else
                {
                    Client.Close();
                    Stream.Close();
                    server.users.Remove(this);
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
