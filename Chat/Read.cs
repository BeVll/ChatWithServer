using Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    public class Read
    {
        public Thread Thread { get; set; }
        public UserServer User { get; set; }
        public Read(Thread thread, UserServer user)
        {
            Thread = thread;
            User = user;
        }   
    }
}
