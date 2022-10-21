using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class UserContextLocal : DbContext
    {
        public DbSet<UserLocal> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public UserContextLocal()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=chats.db");
        }
    }
}
