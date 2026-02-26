using Reverse.Forms.FormsAtendimento;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reverse.Forms.FormsAtendimento
{
    public partial class AtendimentoFormHub : Form
    {
        private readonly int _usuarioId;
        private Form _formFilhoAtual;

        public AtendimentoFormHub(int usuarioId)
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
            if (picAtendimento != null)
            {
                picAtendimento.MouseEnter += Pic_MouseEnter;
                picAtendimento.MouseLeave += Pic_MouseLeave;
                picAtendimento.Click += (s, e) =>
                {
                    if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(AtendimentoChamadoForm)))
                    {
                        AbrirFormNoPainel(new AtendimentoChamadoForm(_usuarioId));
                    }
                    else
                    {
                        MessageBox.Show("Você não tem permissão para acessar o módulo de Atendimentos.",
                            "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };
            }

            if (picChamado != null)
            {
                picChamado.MouseEnter += Pic_MouseEnter;
                picChamado.MouseLeave += Pic_MouseLeave;
                picChamado.Click += (s, e) =>
                {
                    if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(AtendimentoForm)))
                    {
                        AbrirFormNoPainel(new AtendimentoForm(_usuarioId));
                    }
                    else
                    {
                        MessageBox.Show("Você não tem permissão para acessar o módulo de Atendimentos.",
                            "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };
            }

            if (btnSair != null)
            {
                btnSair.Click += (s, e) => Close();
            }

            if (btnMinimizar != null)
            {
                btnMinimizar.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            }
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
            LimparFormFilho();
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
    }
}