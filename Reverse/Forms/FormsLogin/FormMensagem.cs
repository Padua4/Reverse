using Reverse.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reverse.Forms.FormsNotificacao
{
    public class FormMensagem : Form
    {
        private Panel panelTopo;
        private Label lblTitulo;
        private Label lblDestinatario;
        private RadioButton rdTodos;
        private RadioButton rdEspecifico;
        private ComboBox cmbUsuario;
        private Label lblMensagem;
        private TextBox txtMensagem;
        private Button btnEnviar;
        private Button btnCancelar;

        private readonly int _remetenteId;

        public FormMensagem(int remetenteId)
        {
            _remetenteId = remetenteId;
            ConstruirUI();
            CarregarUsuarios();
        }

        private void ConstruirUI()
        {
            this.Text = "Enviar Mensagem";
            this.Size = new Size(480, 395);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 45);

            panelTopo = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(20, 20, 35) };
            lblTitulo = new Label
            {
                Text = "✉  Nova Mensagem",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };
            panelTopo.Controls.Add(lblTitulo);
            this.Controls.Add(panelTopo);

            lblDestinatario = Label("Destinatário:", 70);

            rdTodos = new RadioButton
            {
                Text = "Todos os usuários",
                Location = new Point(30, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                Checked = true,
                AutoSize = true
            };
            rdTodos.CheckedChanged += (s, e) => cmbUsuario.Enabled = !rdTodos.Checked;

            rdEspecifico = new RadioButton
            {
                Text = "Usuário específico",
                Location = new Point(200, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                AutoSize = true
            };

            cmbUsuario = new ComboBox
            {
                Location = new Point(30, 128),
                Size = new Size(410, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false,
                BackColor = Color.FromArgb(50, 50, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };

            lblMensagem = Label("Mensagem:", 168);

            txtMensagem = new TextBox
            {
                Location = new Point(30, 193),
                Size = new Size(410, 105),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                MaxLength = 500,
                BackColor = Color.FromArgb(50, 50, 70),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnEnviar = new Button
            {
                Text = "Enviar",
                Location = new Point(250, 315),
                Size = new Size(90, 32),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnEnviar.FlatAppearance.BorderSize = 0;
            btnEnviar.Click += BtnEnviar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(350, 315),
                Size = new Size(90, 32),
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblDestinatario, rdTodos, rdEspecifico,
                cmbUsuario, lblMensagem, txtMensagem,
                btnEnviar, btnCancelar
            });
        }

        private Label Label(string texto, int top) => new Label
        {
            Text = texto,
            Location = new Point(30, top),
            ForeColor = Color.FromArgb(180, 180, 220),
            Font = new Font("Segoe UI", 9),
            AutoSize = true
        };

        private void CarregarUsuarios()
        {
            try
            {
                using (var ctx = new ReverseContext())
                {
                    var lista = ctx.Usuarios
                        .OrderBy(u => u.UsuarioNome)
                        .Select(u => new { u.Id, u.UsuarioNome })
                        .ToList();

                    cmbUsuario.DisplayMember = "UsuarioNome";
                    cmbUsuario.ValueMember = "Id";
                    cmbUsuario.DataSource = lista;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuários: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMensagem.Text))
            {
                MessageBox.Show("Digite uma mensagem antes de enviar.",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int? destinatarioId = null;
                string nomeDestinatario = "Todos";

                if (rdEspecifico.Checked)
                {
                    if (cmbUsuario.SelectedValue == null)
                    {
                        MessageBox.Show("Selecione um usuário destinatário.",
                            "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    destinatarioId = (int)cmbUsuario.SelectedValue;
                    nomeDestinatario = cmbUsuario.Text;
                }

                using (var ctx = new ReverseContext())
                {
                    ctx.Notificacoes.Add(new Notificacao
                    {
                        UsuarioRemetenteId = _remetenteId,
                        UsuarioDestinatarioId = destinatarioId,
                        Mensagem = txtMensagem.Text.Trim(),
                        Lida = false,
                        DataCriacao = DateTime.Now
                    });
                    ctx.SaveChanges();
                }

                MessageBox.Show($"Mensagem enviada para: {nomeDestinatario}.",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}