using Reverse.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms.FormsTriagem
{
    public partial class TriagemFormHub : Form
    {

        private readonly int _usuarioId;
        private Form _formFilhoAtual;

        public TriagemFormHub(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            ConfigurarComponentes();
            this.AutoScaleMode = AutoScaleMode.None;
        }

        private void FormTriagemHub_Load(object sender, EventArgs e)
        {
            var areaTrabalho = Screen.FromControl(this).WorkingArea;

            this.Location = areaTrabalho.Location;
            this.Size = areaTrabalho.Size;

            this.Margin = new Padding(0);
            this.Padding = new Padding(0);
        }

        private void ConfigurarComponentes()
        {
            pnlConteudo.AutoScroll = true;
            pnlConteudo.AutoScrollMargin = new Size(0, 0);
            pnlConteudo.Padding = new Padding(0);
            pnlConteudo.Margin = new Padding(0);
            this.Resize += (s, e) => AjustarFormFilho();

            ConfigurarEventos();
        }

        private void ConfigurarEventos()
        {
            picTriagemForm.MouseEnter += Pic_MouseEnter;
            picTriagemForm.MouseLeave += Pic_MouseLeave;
            picTriagemForm.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(TriagemForm)))
                {
                    AbrirFormNoPainel(new TriagemForm(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar esta tela.",
                                    "Acesso Negado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }
            };

            picControleTriagemForm.MouseEnter += Pic_MouseEnter;
            picControleTriagemForm.MouseLeave += Pic_MouseLeave;
            picControleTriagemForm.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(TriagemControleForm)))
                {
                    AbrirFormNoPainel(new TriagemControleForm(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar esta tela.",
                                    "Acesso Negado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }
            };

            picTriagemPalete.MouseEnter += Pic_MouseEnter;
            picTriagemPalete.MouseLeave += Pic_MouseLeave;
            picTriagemPalete.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(TriagemFormPalete)))
                {
                    AbrirFormNoPainel(new TriagemFormPalete(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar esta tela.",
                                    "Acesso Negado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
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
            if (_formFilhoAtual != null)
            {
                _formFilhoAtual.Close();
                _formFilhoAtual.Dispose();
                _formFilhoAtual = null;
            }

            pnlConteudo.Controls.Clear();

            var host = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            _formFilhoAtual = formFilho;
            formFilho.TopLevel = false;
            formFilho.FormBorderStyle = FormBorderStyle.None;
            formFilho.AutoScaleMode = AutoScaleMode.None;

            formFilho.Dock = DockStyle.Fill;
            host.Controls.Add(formFilho);
            pnlConteudo.Controls.Add(host);
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
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
