using Reverse.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reverse.Forms.FormsNotificacao
{
    public class FormNotificacoes : Form
    {
        private Panel panelTopo;
        private Label lblTitulo;
        private Button btnMarcarTodas;
        private ListView listNotificacoes;
        private Label lblDica;
        private Button btnFechar;

        private readonly int _usuarioId;

        public event EventHandler NotificacoesAlteradas;

        public FormNotificacoes(int usuarioId)
        {
            _usuarioId = usuarioId;
            ConstruirUI();
            CarregarNotificacoes();
        }

        private void ConstruirUI()
        {
            this.Text = "Notificações";
            this.Size = new Size(600, 470);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 45);

            panelTopo = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.FromArgb(20, 20, 35) };

            lblTitulo = new Label
            {
                Text = "🔔  Suas Notificações",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(320, 55),
                Location = new Point(15, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnMarcarTodas = new Button
            {
                Text = "Marcar todas como lidas",
                Location = new Point(355, 13),
                Size = new Size(178, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnMarcarTodas.FlatAppearance.BorderSize = 0;
            btnMarcarTodas.Click += BtnMarcarTodas_Click;

            panelTopo.Controls.AddRange(new Control[] { lblTitulo, btnMarcarTodas });
            this.Controls.Add(panelTopo);

            listNotificacoes = new ListView
            {
                Location = new Point(15, 70),
                Size = new Size(558, 330),
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(40, 40, 58),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };
            listNotificacoes.Columns.Add("", 22);
            listNotificacoes.Columns.Add("Remetente", 120);
            listNotificacoes.Columns.Add("Mensagem", 280);
            listNotificacoes.Columns.Add("Recebido", 130);
            listNotificacoes.MouseDoubleClick += ListNotificacoes_DoubleClick;
            this.Controls.Add(listNotificacoes);

            lblDica = new Label
            {
                Text = "Dica: duplo clique em uma mensagem para lê-la completa.",
                Location = new Point(15, 410),
                AutoSize = true,
                ForeColor = Color.FromArgb(130, 130, 160),
                Font = new Font("Segoe UI", 8)
            };

            btnFechar = new Button
            {
                Text = "Fechar",
                Location = new Point(490, 405),
                Size = new Size(83, 30),
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnFechar.FlatAppearance.BorderSize = 0;
            btnFechar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblDica, btnFechar });
        }

        private void CarregarNotificacoes()
        {
            listNotificacoes.Items.Clear();

            try
            {
                using (var ctx = new ReverseContext())
                {
                    var lista = (from n in ctx.Notificacoes
                                 join r in ctx.Usuarios on n.UsuarioRemetenteId equals r.Id
                                 where n.UsuarioDestinatarioId == _usuarioId
                                    || n.UsuarioDestinatarioId == null
                                 orderby n.DataCriacao descending
                                 select new
                                 {
                                     n.Id,
                                     n.Mensagem,
                                     n.DataCriacao,
                                     NomeRemetente = r.UsuarioNome,
                                     Lida = ctx.NotificacoesLidas.Any(
                                                l => l.NotificacaoId == n.Id
                                                  && l.UsuarioId == _usuarioId)
                                 })
                                .Take(100)
                                .ToList();

                    foreach (var n in lista)
                    {
                        var item = new ListViewItem(n.Lida ? "✓" : "●");
                        item.SubItems.Add(n.NomeRemetente);
                        item.SubItems.Add(n.Mensagem);
                        item.SubItems.Add(n.DataCriacao.ToString("dd/MM/yyyy HH:mm"));
                        item.Tag = n.Id;
                        item.ForeColor = n.Lida ? Color.FromArgb(120, 120, 150) : Color.White;
                        if (!n.Lida)
                            item.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                        listNotificacoes.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar notificações: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListNotificacoes_DoubleClick(object sender, MouseEventArgs e)
        {
            if (listNotificacoes.SelectedItems.Count == 0) return;

            var item = listNotificacoes.SelectedItems[0];
            int id = (int)item.Tag;
            string remetente = item.SubItems[1].Text;
            string mensagem = item.SubItems[2].Text;
            string recebido = item.SubItems[3].Text;

            AbrirDetalhe(remetente, mensagem, recebido);

            MarcarComoLida(id);
            CarregarNotificacoes();
            NotificacoesAlteradas?.Invoke(this, EventArgs.Empty);
        }

        private void AbrirDetalhe(string remetente, string mensagem, string recebido)
        {
            using (var detalhe = new Form())
            {
                detalhe.Text = "Mensagem";
                detalhe.Size = new Size(480, 320);
                detalhe.FormBorderStyle = FormBorderStyle.FixedDialog;
                detalhe.MaximizeBox = false;
                detalhe.MinimizeBox = false;
                detalhe.StartPosition = FormStartPosition.CenterParent;
                detalhe.BackColor = Color.FromArgb(30, 30, 45);

                var panelHead = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 55,
                    BackColor = Color.FromArgb(20, 20, 35)
                };

                var lblCabecalho = new Label
                {
                    Text = "✉  Mensagem recebida",
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    AutoSize = false,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(15, 0, 0, 0)
                };
                panelHead.Controls.Add(lblCabecalho);
                detalhe.Controls.Add(panelHead);

                var lblMeta = new Label
                {
                    Text = $"De: {remetente}     |     Recebido: {recebido}",
                    Location = new Point(20, 65),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(160, 160, 200),
                    Font = new Font("Segoe UI", 8, FontStyle.Italic)
                };

                var separador = new Panel
                {
                    Location = new Point(20, 88),
                    Size = new Size(425, 1),
                    BackColor = Color.FromArgb(60, 60, 90)
                };

                var txtCorpo = new RichTextBox
                {
                    Location = new Point(20, 100),
                    Size = new Size(425, 150),
                    Text = mensagem,
                    ReadOnly = true,
                    BackColor = Color.FromArgb(40, 40, 58),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10),
                    BorderStyle = BorderStyle.None,
                    ScrollBars = RichTextBoxScrollBars.Vertical,
                    WordWrap = true
                };

                var btnOk = new Button
                {
                    Text = "Fechar",
                    Location = new Point(370, 262),
                    Size = new Size(75, 28),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    DialogResult = DialogResult.OK
                };
                btnOk.FlatAppearance.BorderSize = 0;

                detalhe.Controls.AddRange(new Control[]
                {
                    lblMeta, separador, txtCorpo, btnOk
                });

                detalhe.AcceptButton = btnOk;
                detalhe.ShowDialog(this);
            }
        }

        private void BtnMarcarTodas_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ctx = new ReverseContext())
                {
                    var idsNaoLidas = (from n in ctx.Notificacoes
                                       where (n.UsuarioDestinatarioId == _usuarioId
                                              || n.UsuarioDestinatarioId == null)
                                          && !ctx.NotificacoesLidas.Any(
                                                  l => l.NotificacaoId == n.Id
                                                    && l.UsuarioId == _usuarioId)
                                       select n.Id)
                                      .ToList();

                    foreach (int nId in idsNaoLidas)
                    {
                        ctx.NotificacoesLidas.Add(new NotificacaoLida
                        {
                            NotificacaoId = nId,
                            UsuarioId = _usuarioId,
                            DataLeitura = DateTime.Now
                        });
                    }

                    ctx.SaveChanges();
                }

                CarregarNotificacoes();
                NotificacoesAlteradas?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MarcarComoLida(int notificacaoId)
        {
            try
            {
                using (var ctx = new ReverseContext())
                {
                    bool jaLida = ctx.NotificacoesLidas.Any(
                        l => l.NotificacaoId == notificacaoId
                          && l.UsuarioId == _usuarioId);

                    if (!jaLida)
                    {
                        ctx.NotificacoesLidas.Add(new NotificacaoLida
                        {
                            NotificacaoId = notificacaoId,
                            UsuarioId = _usuarioId,
                            DataLeitura = DateTime.Now
                        });
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao marcar notificação: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}