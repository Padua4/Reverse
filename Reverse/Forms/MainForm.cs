using System;
using System.Windows.Forms;

namespace Reverse
{
    public partial class MainForm : Form
    {
        private readonly string _usuario;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(string usuario) : this()
        {
            _usuario = usuario;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblGreetingg.Text = $"Bem vindo {_usuario}! Escolha seu modulo por favor...";
        }

        private void pnlTriagem_MouseEnter(object sender, EventArgs e)
        {
            lblDescTriagem.Visible = true;
        }

        private void pnlTriagem_MouseLeave(object sender, EventArgs e)
        {
            lblDescTriagem.Visible = false;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void lblDescAlmo_Click(object sender, EventArgs e)
        {

        }

        private void pnlAlmoxarifado_MouseEnter(object sender, EventArgs e)
        {
            lblDescAlmo.Visible = true;
        }

        private void pnlAlmoxarifado_MouseLeave(object sender, EventArgs e)
        {
            lblDescAlmo.Visible = false;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlRH_MouseEnter(object sender, EventArgs e)
        {
            lblDescRH.Visible = true;  
        }

        private void pnlRH_MouseLeave(object sender, EventArgs e)
        {
            lblDescRH.Visible = false; 
        }

        private void pnlFin_MouseEnter(object sender, EventArgs e)
        {
            lblDescFin.Visible = true; 
        }

        private void pnlFin_MouseLeave(object sender, EventArgs e)
        {
            lblDescFin.Visible = false;
        }

        private void picTriagem_Click(object sender, EventArgs e)
        {
            var controle = new ControleTriagemForm(_usuario);
            controle.StartPosition = FormStartPosition.CenterScreen;

            this.Hide();

            controle.FormClosed += (s, args) => this.Show();

            controle.Show();
        }

        private void pnlFin_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
