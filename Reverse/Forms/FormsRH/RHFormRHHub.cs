using Reverse.Forms.FormsFinanceiro;
using SeuProjeto;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reverse.Forms.FormsRH
{
    public partial class RHFormRHHub : Form
    {
        private readonly int _usuarioId;
        private Form _formFilhoAtual;

        public RHFormRHHub(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            ConfigurarComponentes();
        }

        private void ConfigurarComponentes()
        {
            panelConteudo.AutoScroll = true;
            panelConteudo.AutoScrollMargin = new Size(0, 0);
            panelConteudo.Padding = new Padding(0);
            panelConteudo.Margin = new Padding(0);
            panelConteudo.Dock = DockStyle.Fill;

            this.Resize += (s, e) => AjustarFormFilho();

            ConfigurarEventos();
        }

        private void ConfigurarEventos()
        {
            picFormFuncionarios.MouseEnter += Pic_MouseEnter;
            picFormFuncionarios.MouseLeave += Pic_MouseLeave;
            picFormFuncionarios.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(RHFormFuncionarios)))
                {
                    AbrirFormNoPainel(new RHFormFuncionarios(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar o módulo de Funcionários.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picFormInatividade.MouseEnter += Pic_MouseEnter;
            picFormInatividade.MouseLeave += Pic_MouseLeave;
            picFormInatividade.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(RHFormInatividade)))
                {
                    AbrirFormNoPainel(new RHFormInatividade(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar o módulo de Inatividade.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picFormCestaBasica.MouseEnter += Pic_MouseEnter;
            picFormCestaBasica.MouseLeave += Pic_MouseLeave;
            picFormCestaBasica.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(RHFormCesta)))
                {
                    AbrirFormNoPainel(new RHFormCesta(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar o módulo de Cesta Básica.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

        }

        private void Pic_MouseEnter(object sender, EventArgs e)
        {
            if (sender is PictureBox pic)
            {
                pic.BackColor = Color.LightBlue;
                pic.Cursor = Cursors.Hand;
            }
        }

        private void Pic_MouseLeave(object sender, EventArgs e)
        {
            if (sender is PictureBox pic)
            {
                pic.BackColor = Color.White;
            }
        }

        private void AbrirFormNoPainel(Form formFilho)
        {
            if (_formFilhoAtual != null)
            {
                panelConteudo.Controls.Clear();
                _formFilhoAtual.Close();
                _formFilhoAtual.Dispose();
            }

            _formFilhoAtual = formFilho;

            formFilho.TopLevel = false;
            formFilho.FormBorderStyle = FormBorderStyle.None;
            formFilho.Dock = DockStyle.None;

            panelConteudo.Controls.Add(formFilho);
            formFilho.Show();
            AjustarFormFilho();
        }

        private void AjustarFormFilho()
        {
            if (_formFilhoAtual == null) return;

            _formFilhoAtual.Margin = new Padding(0);
            _formFilhoAtual.Padding = new Padding(0);

            _formFilhoAtual.Location = new Point(0, 0);

            _formFilhoAtual.Size = new Size(
                panelConteudo.ClientSize.Width,
                panelConteudo.ClientSize.Height
            );

            panelConteudo.PerformLayout();
            _formFilhoAtual.PerformLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MaximizarSemCobrirBarraTarefas();
        }

        private void MaximizarSemCobrirBarraTarefas()
        {
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.Manual;

            Rectangle workingArea = Screen.FromControl(this).WorkingArea;

            this.Location = workingArea.Location;
            this.Size = workingArea.Size;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_formFilhoAtual != null)
            {
                _formFilhoAtual.Close();
                _formFilhoAtual.Dispose();
                _formFilhoAtual = null;
            }

            base.OnFormClosed(e);
        }

        public void LimparFormFilho()
        {
            if (_formFilhoAtual != null)
            {
                panelConteudo.Controls.Clear();
                _formFilhoAtual.Close();
                _formFilhoAtual.Dispose();
                _formFilhoAtual = null;
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