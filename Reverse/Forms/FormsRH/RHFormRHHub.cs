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
        private readonly Size _tamanhoMinimo = new Size(800, 600);
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

            this.MinimumSize = _tamanhoMinimo;

            ConfigurarEventos();
        }

        private void ConfigurarEventos()
        {
            picFormFuncionarios.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(FormFuncionarios)))
                {
                    AbrirFormNoPainel(new FormFuncionarios(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar Funcionarios.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picFormFuncionariosInativos.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(FormFuncionariosInativos)))
                {
                    AbrirFormNoPainel(new FormFuncionariosInativos(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar Funcionarios Inativos.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picFormCestaBasica.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(FormCestaBasica)))
                {
                    AbrirFormNoPainel(new FormCestaBasica(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar Cesta Básica.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picFormCurriculosNovos.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(FormCurriculosNovos)))
                {
                    AbrirFormNoPainel(new FormCurriculosNovos(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar Currículos Novos.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picFormParticipantesAptos.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(FormParticipantesAptos)))
                {
                    AbrirFormNoPainel(new FormParticipantesAptos(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar Participantes Aptos.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            ConfigurarHoverPictureBox(picFormFuncionarios);
            ConfigurarHoverPictureBox(picFormFuncionariosInativos);
            ConfigurarHoverPictureBox(picFormCestaBasica);
            ConfigurarHoverPictureBox(picFormCurriculosNovos);
            ConfigurarHoverPictureBox(picFormParticipantesAptos);

            btnSair.Click += (s, e) => Close();
        }

        private void ConfigurarHoverPictureBox(PictureBox pictureBox)
        {
            pictureBox.MouseEnter += Pic_MouseEnter;
            pictureBox.MouseLeave += Pic_MouseLeave;
            pictureBox.Cursor = Cursors.Hand;
        }

        private void Pic_MouseEnter(object sender, EventArgs e)
        {
            if (sender is PictureBox pic)
            {
                pic.BackColor = Color.LightBlue;
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
                _formFilhoAtual.Close();
                _formFilhoAtual.Dispose();
            }

            _formFilhoAtual = formFilho;

            panelConteudo.Controls.Clear();

            formFilho.TopLevel = false;
            formFilho.FormBorderStyle = FormBorderStyle.None;
            formFilho.Dock = DockStyle.None;
            formFilho.Location = Point.Empty;

            panelConteudo.Controls.Add(formFilho);

            panelConteudo.AutoScrollMinSize = formFilho.Size;

            formFilho.Show();
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
            this.WindowState = FormWindowState.Minimized;
        }
    }
}