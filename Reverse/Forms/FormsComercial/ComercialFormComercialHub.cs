using Reverse.Forms.FormsRH;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reverse.Forms.FormsComercial
{
    public partial class ComercialFormComercialHub : Form
    {
        private readonly int _usuarioId;
        private Form _formFilhoAtual;

        public ComercialFormComercialHub(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            ConfigurarComponentes();

            this.Load += FormComercialHub_Load;
        }

        private void FormComercialHub_Load(object sender, EventArgs e)
        {
            Rectangle areaTrabalho = Screen.PrimaryScreen.WorkingArea;
            this.Location = areaTrabalho.Location;
            this.Size = areaTrabalho.Size;
        }

        private void ConfigurarComponentes()
        {
            pnlConteudo.AutoScroll = true;
            pnlConteudo.AutoScrollMargin = new Size(0, 0);
            pnlConteudo.Padding = new Padding(0);
            this.Resize += (s, e) => AjustarFormFilho();
            ConfigurarEventos();
        }

        private void ConfigurarEventos()
        {
            picVendaPaletes.MouseEnter += Pic_MouseEnter;
            picVendaPaletes.MouseLeave += Pic_MouseLeave;

            picVendaPaletes.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(ComercialFormVenda)))
                {
                    AbrirFormNoPainel(new ComercialFormVenda(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar Vendas.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
        }

        private void Pic_MouseEnter(object sender, EventArgs e)
        {
            var pic = sender as PictureBox;
            pic.BackColor = Color.LightBlue;
            pic.Cursor = Cursors.Hand;
        }

        private void Pic_MouseLeave(object sender, EventArgs e)
        {
            var pic = sender as PictureBox;
            pic.BackColor = Color.White;
        }

        private void AbrirFormNoPainel(Form formFilho)
        {
            _formFilhoAtual = formFilho;
            pnlConteudo.Controls.Clear();

            formFilho.TopLevel = false;
            formFilho.FormBorderStyle = FormBorderStyle.None;
            formFilho.Dock = DockStyle.None;

            pnlConteudo.Controls.Add(formFilho);
            AjustarFormFilho();
            formFilho.Show();
        }
        private void AjustarFormFilho()
        {
            if (_formFilhoAtual == null) return;

            _formFilhoAtual.Location = Point.Empty;
            _formFilhoAtual.Margin = new Padding(0);
            _formFilhoAtual.Padding = new Padding(0);

            pnlConteudo.AutoScrollMinSize = _formFilhoAtual.Size;
            pnlConteudo.PerformLayout();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            Form formPrincipal = this;

            while (formPrincipal.ParentForm != null)
                formPrincipal = formPrincipal.ParentForm;

            formPrincipal.WindowState = FormWindowState.Minimized;
        }
    }
}