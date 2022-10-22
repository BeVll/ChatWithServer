using Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerControl
    {
        List<Read> reads = new List<Read>();
        Chat.Server server = new Chat.Server();
        public void Start()
        {
            server.Start();
            Thread WaitUsers = new Thread(server.WaitConnect);
            WaitUsers.Start();
            Thread read = new Thread(Read);
            read.Start();
        }
        public void Read()
        {
            while (server.working)
            {
                foreach (UserServer user in server.users.ToList())
                {

                    Read r = reads.Where(s => s.User == user).FirstOrDefault();
                    if (r == null)
                    {
                        Thread thread = new Thread(() => ReadUser(user));
                        thread.Start();
                        reads.Add(new Read(thread, user));
                    }
                    Thread.Sleep(50); // Щоб ненавантажувало процесор
                }
            }
        }
        public void ReadUser(UserServer user)
        {
            while (server.working && server.users.Contains(user))
            {
                string mes = user.GetMessage();
                Console.WriteLine($"[{DateTime.Now.ToString()}] {user.Id}: {mes}");
            }
            reads.Remove(reads.Where(s => s.User.Id == user.Id).FirstOrDefault());
        }
    }
}
