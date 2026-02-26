using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace Reverse.Forms.FormsAtendimento
{
    public partial class AtendimentoChamadoForm : Form
    {
        private int usuarioId;
        private string connectionString;

        public AtendimentoChamadoForm(int _usuarioId)
        {
            InitializeComponent();
            this.usuarioId = _usuarioId;

            connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;

            ConfigurarGrid();
        }

        private void ConfigurarGrid()
        {
            dgvChamados.AutoGenerateColumns = false;
            dgvChamados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvChamados.MultiSelect = false;
            dgvChamados.AllowUserToAddRows = false;
            dgvChamados.AllowUserToDeleteRows = false;
            dgvChamados.ReadOnly = true;
            dgvChamados.RowHeadersVisible = false;

            dgvChamados.BorderStyle = BorderStyle.FixedSingle;
            dgvChamados.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvChamados.EnableHeadersVisualStyles = false;
            dgvChamados.AllowUserToResizeRows = false;
            dgvChamados.EditMode = DataGridViewEditMode.EditProgrammatically;

            // Fonte
            dgvChamados.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvChamados.DefaultCellStyle.ForeColor = Color.Black;
            dgvChamados.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvChamados.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft; // Já está left

            // Cores de fundo
            dgvChamados.BackgroundColor = Color.FromArgb(250, 250, 252);
            dgvChamados.GridColor = Color.FromArgb(230, 230, 235);

            // Cabeçalho
            dgvChamados.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvChamados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvChamados.ColumnHeadersHeight = 40;
            dgvChamados.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94); // Azul escuro
            dgvChamados.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvChamados.ColumnHeadersDefaultCellStyle.Padding = new Padding(5, 3, 0, 3); // Padding à esquerda

            // Linhas alternadas
            dgvChamados.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            dgvChamados.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgvChamados.RowsDefaultCellStyle.BackColor = Color.White;
            dgvChamados.RowsDefaultCellStyle.ForeColor = Color.Black;

            // Seleção
            dgvChamados.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            dgvChamados.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvChamados.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgvChamados.ColumnHeadersDefaultCellStyle.BackColor;
            dgvChamados.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // Padding das células
            dgvChamados.DefaultCellStyle.Padding = new Padding(5, 5, 5, 5);

            // Altura das linhas
            dgvChamados.RowTemplate.Height = 35;
            dgvChamados.RowTemplate.MinimumHeight = 34;

            // Alinhamento
            dgvChamados.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Limpar colunas existentes
            dgvChamados.Columns.Clear();

            // Nº Chamado
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ChamadoId",
                HeaderText = "Nº CHAMADO",
                DataPropertyName = "ChamadoId",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.Black,
                    Padding = new Padding(5, 0, 0, 0)
                }
            });

            // Assunto
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Assunto",
                HeaderText = "ASSUNTO",
                DataPropertyName = "Assunto",
                Width = 400, // Maior coluna
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    Padding = new Padding(5, 0, 0, 0),
                    ForeColor = Color.Black
                }
            });

            // Data
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DataCriacao",
                HeaderText = "DATA",
                DataPropertyName = "DataCriacao",
                Width = 160,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy HH:mm",
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.Black,
                    Padding = new Padding(5, 0, 0, 0)
                }
            });

            // Prioridade
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Prioridade",
                HeaderText = "PRIORIDADE",
                DataPropertyName = "Prioridade",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.Black,
                    Padding = new Padding(5, 0, 0, 0)
                }
            });

            // Suporte
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SuporteNome",
                HeaderText = "SUPORTE",
                DataPropertyName = "SuporteNome",
                Width = 250,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    Padding = new Padding(5, 0, 0, 0),
                    ForeColor = Color.Black
                }
            });

            // Resposta
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StatusResposta",
                HeaderText = "RESPOSTA",
                DataPropertyName = "StatusResposta",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.Black,
                    Padding = new Padding(5, 0, 0, 0)
                }
            });

            // Status
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "STATUS",
                DataPropertyName = "Status",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.Black,
                    Padding = new Padding(5, 0, 0, 0)
                }
            });

            // Coluna oculta Atrasado
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Atrasado",
                HeaderText = "Atrasado",
                DataPropertyName = "Atrasado",
                Visible = false
            });

            dgvChamados.Dock = DockStyle.Fill;
            dgvChamados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dgvChamados.Columns.Contains("Assunto"))
            {
                dgvChamados.Columns["Assunto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvChamados.Columns["Assunto"].FillWeight = 50;
            }

            if (dgvChamados.Columns.Contains("SuporteNome"))
            {
                dgvChamados.Columns["SuporteNome"].FillWeight = 25;
            }

            if (dgvChamados.Columns.Contains("DataCriacao"))
            {
                dgvChamados.Columns["DataCriacao"].FillWeight = 15;
            }

            if (dgvChamados.Columns.Contains("ChamadoId"))
            {
                dgvChamados.Columns["ChamadoId"].MinimumWidth = 110;
                dgvChamados.Columns["ChamadoId"].FillWeight = 8;
            }

            if (dgvChamados.Columns.Contains("Status"))
            {
                dgvChamados.Columns["Status"].MinimumWidth = 100;
                dgvChamados.Columns["Status"].FillWeight = 8;
            }

            if (dgvChamados.Columns.Contains("StatusResposta"))
            {
                dgvChamados.Columns["StatusResposta"].MinimumWidth = 100;
                dgvChamados.Columns["StatusResposta"].FillWeight = 8;
            }

            if (dgvChamados.Columns.Contains("Prioridade"))
            {
                dgvChamados.Columns["Prioridade"].MinimumWidth = 100;
                dgvChamados.Columns["Prioridade"].FillWeight = 8;
            }
            dgvChamados.CellFormatting += DgvChamados_CellFormatting;
        }

        private void CarregarChamados()
        {
            try
            {
                AtualizarChamadosAtrasados();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"SELECT c.ChamadoId, c.Assunto, c.Prioridade, c.Status, c.StatusResposta,
                                          c.DataCriacao, c.Atrasado,
                                          ISNULL(s.UsuarioNome, 'Não atribuído') AS SuporteNome
                                   FROM Chamados c
                                   LEFT JOIN Usuarios s ON c.SuporteId = s.Id
                                   WHERE c.UsuarioId = @UsuarioId
                                   ORDER BY c.DataCriacao DESC";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvChamados.DataSource = dt;
                    dgvChamados.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar chamados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarChamadosAtrasados()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"UPDATE Chamados
                                  SET Atrasado = 1
                                  WHERE Status = 'Aberto'
                                  AND SuporteId IS NULL
                                  AND DATEDIFF(DAY, DataCriacao, GETDATE()) >= 1";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private void DgvChamados_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridView grid = (DataGridView)sender;
                DataGridViewRow row = grid.Rows[e.RowIndex];

                string statusResposta = row.Cells["StatusResposta"].Value?.ToString() ?? "";
                string status = row.Cells["Status"].Value?.ToString() ?? "";
                bool atrasado = row.Cells["Atrasado"] != null &&
                               Convert.ToBoolean(row.Cells["Atrasado"].Value ?? false);

                if (statusResposta == "Respondido" && status == "Aberto")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else if (atrasado && status == "Aberto")
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                }
                else if (status == "Finalizado")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }

        private void btnChamado_Click(object sender, EventArgs e)
        {
            AtendimentoFormProtocolo formProtocolo = new AtendimentoFormProtocolo(usuarioId, connectionString);
            formProtocolo.ShowDialog();

            CarregarChamados();
        }

        private void btnAnalise_Click(object sender, EventArgs e)
        {
            if (dgvChamados.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um chamado para analisar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int chamadoId = Convert.ToInt32(dgvChamados.SelectedRows[0].Cells["ChamadoId"].Value);

            AtendimentoFormProtocolo formProtocolo = new AtendimentoFormProtocolo(
                usuarioId,
                connectionString,
                chamadoId
            );
            formProtocolo.ShowDialog();

            CarregarChamados();
        }

        private void AtendimentoChamadoForm_Load(object sender, EventArgs e)
        {
            CarregarChamados();
        }

        public void AtualizarGrid()
        {
            CarregarChamados();
        }

        private void AdicionarColunaAtrasado()
        {
            if (!dgvChamados.Columns.Contains("Atrasado"))
            {
                dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Atrasado",
                    HeaderText = "Atrasado",
                    DataPropertyName = "Atrasado",
                    Visible = false
                });
            }
        }
    }   
}