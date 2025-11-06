using Reverse.Forms.FormsExpedicao;
using Reverse.Forms.FormsFinanceiro;
using Reverse.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SeuProjeto
{
    public partial class FinanceiroFormFinanceiroHub : Form
    {
        private readonly int _usuarioId;
        private Form _formFilhoAtual;

        public FinanceiroFormFinanceiroHub()
        {
            InitializeComponent();
            ConfigurarComponentes();
        }

        public FinanceiroFormFinanceiroHub(int usuarioId) : this()
        {
            _usuarioId = usuarioId;
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
            picGraficos.MouseEnter += Pic_MouseEnter;
            picGraficos.MouseLeave += Pic_MouseLeave;
            picGraficos.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(FormGraficos)))
                {
                    AbrirFormNoPainel(new FormGraficos(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar os Gráficos.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picClientes.MouseEnter += Pic_MouseEnter;
            picClientes.MouseLeave += Pic_MouseLeave;
            picClientes.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(FinanceiroFormClientes)))
                {
                    AbrirFormNoPainel(new FinanceiroFormClientes(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar os Clientes.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picPagar.MouseEnter += Pic_MouseEnter;
            picPagar.MouseLeave += Pic_MouseLeave;
            picPagar.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(FormPagar)))
                {
                    AbrirFormNoPainel(new FormPagar(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar Contas a Pagar.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picReceber.MouseEnter += Pic_MouseEnter;
            picReceber.MouseLeave += Pic_MouseLeave;
            picReceber.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(FormReceber)))
                {
                    AbrirFormNoPainel(new FormReceber(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar Contas a Receber.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picExportar.MouseEnter += Pic_MouseEnter;
            picExportar.MouseLeave += Pic_MouseLeave;
            picExportar.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(FinanceiroFormExportarPDF)))
                {
                    AbrirFormNoPainel(new FinanceiroFormExportarPDF(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar a Exportação de Dados Financeiros.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnSair.Click += (s, e) => Close();
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

        private void MaximizarSemCobrirBarra()
        {
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.Manual;

            var workingArea = Screen.FromControl(this).WorkingArea;
            this.Location = workingArea.Location;
            this.Size = workingArea.Size;
            this.MinimumSize = new Size(800, 600);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MaximizarSemCobrirBarra();
            AjustarFormFilho();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}