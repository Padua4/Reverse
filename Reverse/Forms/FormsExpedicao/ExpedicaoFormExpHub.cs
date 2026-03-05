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

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormExpHub : Form
    {
        private readonly int _usuarioId;
        private Form _formFilhoAtual;
        public ExpedicaoFormExpHub(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            ConfigurarComponentes();
        }
        private void FormExpHub_Load(object sender, EventArgs e)
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
            picControle.MouseEnter += Pic_MouseEnter;
            picControle.MouseLeave += Pic_MouseLeave;
            picControle.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(ExpedicaoFormControle)))
                {
                    AbrirFormNoPainel(new ExpedicaoFormControle(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar o Controle.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picCadastro.MouseEnter += Pic_MouseEnter;
            picCadastro.MouseLeave += Pic_MouseLeave;
            picCadastro.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(ExpedicaoFormCadastro)))
                {
                    AbrirFormNoPainel(new ExpedicaoFormCadastro(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar o Cadastro da Expedição.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picFrete.MouseEnter += Pic_MouseEnter;
            picFrete.MouseLeave += Pic_MouseLeave;
            picFrete.Click += (s, e) =>
            {
                 if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(ExpedicaoFormFrete)))
                 {
                        AbrirFormNoPainel(new ExpedicaoFormFrete(_usuarioId));
                 }
                 else
                 {
                     MessageBox.Show("Você não tem permissão para acessar o Frete.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 }
            };

            picDesc.MouseEnter += Pic_MouseEnter;
            picDesc.MouseLeave += Pic_MouseLeave;
            picDesc.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(ExpedicaoFormBalanco)))
                {
                    AbrirFormNoPainel(new ExpedicaoFormBalanco(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar o Balanço de Massa.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picEstoque.MouseEnter += Pic_MouseEnter;
            picEstoque.MouseLeave += Pic_MouseLeave;
            picEstoque.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(ExpedicaoFormEstoque)))
                {
                    AbrirFormNoPainel(new ExpedicaoFormEstoque(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar o Estoque da Expedição.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picGraficos.MouseEnter += Pic_MouseEnter;
            picGraficos.MouseLeave += Pic_MouseLeave;
            picGraficos.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(ExpedicaoFormGraficos)))
                {
                    AbrirFormNoPainel(new ExpedicaoFormGraficos(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar os Gráficos da Expedição.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
        }

        private void PicGraficos_MouseEnter(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
