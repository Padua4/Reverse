using System.Windows.Forms;

namespace Reverse
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblSenha;
        private TextBox txtSenha;
        private Button btnLogin;
        private Button btnRegister;
        private PictureBox pictureBox1;
    }
}
