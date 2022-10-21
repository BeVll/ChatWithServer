using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class UserLocal
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Login { get; set; }
        public byte[] Image { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastTime { get; set; }
        public int ForUserId { get; set; }
    }
}
