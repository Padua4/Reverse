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

namespace Reverse.Forms.FormsProducao
{
    public partial class ProducaoFormHub : Form
    {
        private readonly int _usuarioId;
        private Form _formFilhoAtual;

        public ProducaoFormHub(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            ConfigurarComponentes();
        }

        private void ProducaoFormHub_Load(object sender, EventArgs e)
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
            picProducao.MouseEnter += Pic_MouseEnter;
            picProducao.MouseLeave += Pic_MouseLeave;
            picProducao.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(ProducaoForm)))
                {
                    AbrirFormNoPainel(new ProducaoForm(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar o Controle de Produção.",
                        "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            picGrafico.MouseEnter += Pic_MouseEnter;
            picGrafico.MouseLeave += Pic_MouseLeave;
            picGrafico.Click += (s, e) =>
            {
                if (Reverse.Forms.FormsLogin.FormConfigU.PermissaoHelper.TemPermissao(_usuarioId, nameof(ProducaoFormGrafico)))
                {
                    AbrirFormNoPainel(new ProducaoFormGrafico(_usuarioId));
                }
                else
                {
                    MessageBox.Show("Você não tem permissão para acessar os Gráficos de Produção.",
                        "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
        }

        private void Pic_MouseEnter(object sender, EventArgs e)
        {
            var pic = sender as PictureBox;
            if (pic != null)
            {
                pic.BackColor = Color.LightBlue;
                pic.Cursor = Cursors.Hand;
            }
        }

        private void Pic_MouseLeave(object sender, EventArgs e)
        {
            var pic = sender as PictureBox;
            if (pic != null)
            {
                pic.BackColor = Color.White;
            }
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

            // Centraliza o form filho se ele for menor que o painel
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