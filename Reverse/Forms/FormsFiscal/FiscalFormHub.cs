using Reverse.Forms.FormsLogin;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reverse.Forms.FormsFiscal
{
    public partial class FiscalFormHub : Form
    {
        private readonly int _usuarioId;
        private Form _formFilhoAtual;

        public FiscalFormHub(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            ConfigurarComponentes();

            this.Load += FormFiscalHub_Load;
        }

        private void FormFiscalHub_Load(object sender, EventArgs e)
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
            picFiscalPedidos.MouseEnter += Pic_MouseEnter;
            picFiscalPedidos.MouseLeave += Pic_MouseLeave;

            picFiscalPedidos.Click += (s, e) =>
            {
                if (FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(FiscalFormPedidos)))
                {
                    AbrirFormNoPainel(new FiscalFormPedidos(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar Pedidos Fiscais.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            pnlConteudo.AutoScrollMinSize = _formFilhoAtual.Size;

            if (pnlConteudo.ClientSize.Width > _formFilhoAtual.Width &&
                pnlConteudo.ClientSize.Height > _formFilhoAtual.Height)
            {
                _formFilhoAtual.Location = new Point(
                    (pnlConteudo.ClientSize.Width - _formFilhoAtual.Width) / 2,
                    (pnlConteudo.ClientSize.Height - _formFilhoAtual.Height) / 2);
            }
            else
            {
                _formFilhoAtual.Location = Point.Empty;
            }
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