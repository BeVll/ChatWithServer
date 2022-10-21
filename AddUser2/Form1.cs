namespace AddUser2
{
    public partial class Form1 : Form
    {
        UserContext userContext = new UserContext();
        string filename = string.Empty;
        byte[] data = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPG files (*.jpg)|*.jpg|PNG files (*.png)|*.png|JPEG files (*.jpeg)|*.jpeg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filename = ofd.FileName;
            }
        }
        public byte[] FileToByteArray(string fileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

       

        private void button2_Click_1(object sender, EventArgs e)
        {
            data = FileToByteArray(filename);
            User user = new User();
            user.Name = textBox1.Text;
            user.Login = textBox2.Text;
            user.Password = textBox3.Text;
            user.Image = data;
            userContext.Users.Add(user);
            userContext.SaveChanges();
        }
    }
}