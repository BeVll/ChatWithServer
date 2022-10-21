using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ChatWithBot
{
    public class MessageNew
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public int Sender_Id { get; set; }
        public int Destination_Id { get; set; }
        public HorizontalAlignment Alig { get; set; }
        public SolidColorBrush Color { get; set; }
    }
}
