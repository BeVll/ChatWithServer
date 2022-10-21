using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chat
{
    public class DBConnector
    {
        public UserContext db = new UserContext();
        public UserContextLocal localdb = new UserContextLocal();

        public List<UserLocal> GetUsesrLocal(int user_id)
        {
            List<UserLocal> users = localdb.Users.Where(s => s.ForUserId == user_id).ToList();
            users = users.OrderBy(s => s.LastTime).ToList();
            return users;
        }
        public User GetUser(string login)
        {
            return db.Users.Where(s => s.Login == login).FirstOrDefault();
        }
        public User GetUserFromId(int id)
        {
            return db.Users.Where(s => s.Id == id).FirstOrDefault();
        }
        public List<Message> GetMessage(int user1_id, int user2_id)
        {
            List<Message> messages = localdb.Messages.Where(s => s.Sender_Id == user1_id && s.Destination_Id == user2_id).ToList();
            messages.AddRange(localdb.Messages.Where(s => s.Sender_Id == user2_id && s.Destination_Id == user1_id).ToList());
            messages = messages.OrderBy(s => s.Created).ToList();
            return messages;
        }
        public bool CheckLogin(string login)
        {
            List<User> users = db.Users.ToList();
            if (users.Exists(s => s.Login == login) == true)
                return false;
            else
                return true;
        }
        public void AddNewUser(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
        }
        public void AddLocalUser(User user, int MyId)
        {
            UserLocal userLocal = new UserLocal();
            userLocal.ForUserId = MyId;
            userLocal.LastMessage = "New User";
            userLocal.Login = user.Login;
            userLocal.LastTime = DateTime.Now;
            userLocal.Id = user.Id;
            userLocal.Name = user.Name;
            userLocal.Image = user.Image;
            localdb.Users.Add(userLocal);
            localdb.SaveChanges();
        }
    }
}
