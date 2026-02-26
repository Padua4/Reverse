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
    public partial class AtendimentoForm : Form
    {
        private int usuarioId;
        private string connectionString;

        public AtendimentoForm(int _usuarioId)
        {
            InitializeComponent();
            this.usuarioId = _usuarioId;

            connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;

            if (!VerificarPermissaoSuporte())
            {
                MessageBox.Show("Você não tem permissão para acessar o módulo de suporte!", "Acesso Negado",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            ConfigurarGrid();
        }

        #region Verificação de Permissão

        private bool VerificarPermissaoSuporte()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"SELECT COUNT(*) 
                                  FROM Permissoes 
                                  WHERE UsuarioId = @UsuarioId 
                                  AND FormName = 'AtendimentoForm' 
                                  AND PodeAcessar = 1";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Configuração da Grid

        private void ConfigurarGrid()
        {
            dgvAnalise.AutoGenerateColumns = false;
            dgvAnalise.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAnalise.MultiSelect = false;
            dgvAnalise.AllowUserToAddRows = false;
            dgvAnalise.AllowUserToDeleteRows = false;
            dgvAnalise.ReadOnly = true;
            dgvAnalise.RowHeadersVisible = false;

            dgvAnalise.BorderStyle = BorderStyle.FixedSingle;
            dgvAnalise.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvAnalise.EnableHeadersVisualStyles = false;
            dgvAnalise.AllowUserToResizeRows = false;
            dgvAnalise.EditMode = DataGridViewEditMode.EditProgrammatically;

            // Fonte
            dgvAnalise.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvAnalise.DefaultCellStyle.ForeColor = Color.Black;
            dgvAnalise.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvAnalise.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft; // Já está left

            // Cores de fundo
            dgvAnalise.BackgroundColor = Color.FromArgb(250, 250, 252);
            dgvAnalise.GridColor = Color.FromArgb(230, 230, 235);

            // Cabeçalho
            dgvAnalise.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvAnalise.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvAnalise.ColumnHeadersHeight = 40;
            dgvAnalise.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvAnalise.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAnalise.ColumnHeadersDefaultCellStyle.Padding = new Padding(5, 3, 0, 3); // Padding à esquerda

            // Linhas alternadas
            dgvAnalise.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            dgvAnalise.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgvAnalise.RowsDefaultCellStyle.BackColor = Color.White;
            dgvAnalise.RowsDefaultCellStyle.ForeColor = Color.Black;

            // Seleção
            dgvAnalise.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            dgvAnalise.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvAnalise.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgvAnalise.ColumnHeadersDefaultCellStyle.BackColor;
            dgvAnalise.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // Padding das células
            dgvAnalise.DefaultCellStyle.Padding = new Padding(5, 5, 5, 5);

            // Altura das linhas
            dgvAnalise.RowTemplate.Height = 35;
            dgvAnalise.RowTemplate.MinimumHeight = 34;

            // Alinhamento
            dgvAnalise.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvAnalise.Columns.Clear();

            dgvAnalise.Columns.Add(new DataGridViewTextBoxColumn
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

            // Usuario
            dgvAnalise.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UsuarioNome",
                HeaderText = "USUÁRIO",
                DataPropertyName = "UsuarioNome",
                Width = 200,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    Padding = new Padding(5, 0, 0, 0),
                    ForeColor = Color.Black
                }
            });

            // Assunto
            dgvAnalise.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Assunto",
                HeaderText = "ASSUNTO",
                DataPropertyName = "Assunto",
                Width = 450,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    Padding = new Padding(5, 0, 0, 0),
                    ForeColor = Color.Black
                }
            });

            // Data Criação
            dgvAnalise.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DataCriacao",
                HeaderText = "DATA CRIAÇÃO",
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
            dgvAnalise.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Prioridade",
                HeaderText = "PRIORIDADE",
                DataPropertyName = "Prioridade",
                Width = 200,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.Black,
                    Padding = new Padding(5, 0, 0, 0)
                }
            });

            // Atribuído
            dgvAnalise.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SuporteNome",
                HeaderText = "ATRIBUÍDO A",
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

            // Status
            dgvAnalise.Columns.Add(new DataGridViewTextBoxColumn
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

            // Status Resposta (oculta)
            dgvAnalise.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StatusResposta",
                HeaderText = "Status Resposta",
                DataPropertyName = "StatusResposta",
                Visible = false
            });

            // Coluna Atrasado (oculta)
            dgvAnalise.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Atrasado",
                HeaderText = "Atrasado",
                DataPropertyName = "Atrasado",
                Visible = false
            });

            // Coluna SuporteId (oculta)
            dgvAnalise.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SuporteId",
                HeaderText = "SuporteId",
                DataPropertyName = "SuporteId",
                Visible = false
            });

            dgvAnalise.Dock = DockStyle.Fill;
            dgvAnalise.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvAnalise.Dock = DockStyle.Fill;
            dgvAnalise.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dgvAnalise.Columns.Contains("Assunto"))
            {
                dgvAnalise.Columns["Assunto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvAnalise.Columns["Assunto"].FillWeight = 40;
            }

            if (dgvAnalise.Columns.Contains("UsuarioNome"))
            {
                dgvAnalise.Columns["UsuarioNome"].FillWeight = 20;
            }

            if (dgvAnalise.Columns.Contains("SuporteNome"))
            {
                dgvAnalise.Columns["SuporteNome"].FillWeight = 20;
            }

            if (dgvAnalise.Columns.Contains("DataCriacao"))
            {
                dgvAnalise.Columns["DataCriacao"].FillWeight = 12;
            }

            if (dgvAnalise.Columns.Contains("Prioridade"))
            {
                dgvAnalise.Columns["Prioridade"].FillWeight = 15;
            }

            if (dgvAnalise.Columns.Contains("ChamadoId"))
            {
                dgvAnalise.Columns["ChamadoId"].MinimumWidth = 110;
                dgvAnalise.Columns["ChamadoId"].FillWeight = 8;
            }

            if (dgvAnalise.Columns.Contains("Status"))
            {
                dgvAnalise.Columns["Status"].MinimumWidth = 100;
                dgvAnalise.Columns["Status"].FillWeight = 8;
            }

            dgvAnalise.CellFormatting += DgvAnalise_CellFormatting;
        }

        #endregion

        #region Carregar Dados

        private void CarregarChamados()
        {
            try
            {
                AtualizarChamadosAtrasados();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"SELECT c.ChamadoId, c.Assunto, c.Prioridade, c.Status, c.StatusResposta,
                                          c.DataCriacao, c.Atrasado, c.SuporteId,
                                          u.UsuarioNome AS UsuarioNome,
                                          ISNULL(s.UsuarioNome, 'Não atribuído') AS SuporteNome
                                   FROM Chamados c
                                   INNER JOIN Usuarios u ON c.UsuarioId = u.Id
                                   LEFT JOIN Usuarios s ON c.SuporteId = s.Id
                                   ORDER BY 
                                       CASE WHEN c.Status = 'Aberto' THEN 0 ELSE 1 END,
                                       CASE WHEN c.Atrasado = 1 THEN 0 ELSE 1 END,
                                       CASE c.Prioridade 
                                           WHEN 'Alta' THEN 0 
                                           WHEN 'Media' THEN 1 
                                           WHEN 'Baixa' THEN 2 
                                           WHEN 'Sugestao' THEN 3 
                                       END,
                                       c.DataCriacao DESC";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvAnalise.DataSource = dt;
                    dgvAnalise.ClearSelection();

                    AtualizarContadores();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar chamados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarContadores()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlNovo = @"SELECT COUNT(*) FROM Chamados 
                                      WHERE Status = 'Aberto' AND SuporteId IS NULL";
                    SqlCommand cmdNovo = new SqlCommand(sqlNovo, conn);
                    int novos = Convert.ToInt32(cmdNovo.ExecuteScalar());
                    lblNovo.Text = $"Novo: {novos}";

                    string sqlAndamento = @"SELECT COUNT(*) FROM Chamados 
                                           WHERE Status = 'Aberto' AND SuporteId IS NOT NULL AND StatusResposta = 'Enviado'";
                    SqlCommand cmdAndamento = new SqlCommand(sqlAndamento, conn);
                    int andamento = Convert.ToInt32(cmdAndamento.ExecuteScalar());
                    lblAndamento.Text = $"Em andamento: {andamento}";

                    string sqlAguardando = @"SELECT COUNT(*) FROM Chamados 
                                            WHERE Status = 'Aberto' AND StatusResposta = 'Respondido'";
                    SqlCommand cmdAguardando = new SqlCommand(sqlAguardando, conn);
                    int aguardando = Convert.ToInt32(cmdAguardando.ExecuteScalar());
                    lblAguardando.Text = $"Aguardando: {aguardando}";

                    string sqlResolvido = @"SELECT COUNT(*) FROM Chamados WHERE Status = 'Finalizado'";
                    SqlCommand cmdResolvido = new SqlCommand(sqlResolvido, conn);
                    int resolvidos = Convert.ToInt32(cmdResolvido.ExecuteScalar());
                    lblResolvido.Text = $"Resolvido: {resolvidos}";

                    string sqlAtrasado = @"SELECT COUNT(*) FROM Chamados 
                                          WHERE Status = 'Aberto' AND Atrasado = 1";
                    SqlCommand cmdAtrasado = new SqlCommand(sqlAtrasado, conn);
                    int atrasados = Convert.ToInt32(cmdAtrasado.ExecuteScalar());
                    lblAtrasado.Text = $"Atrasado: {atrasados}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar contadores: {ex.Message}", "Erro",
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

        private void DgvAnalise_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridView grid = (DataGridView)sender;
                DataGridViewRow row = grid.Rows[e.RowIndex];

                string status = row.Cells["Status"].Value?.ToString() ?? "";
                string statusResposta = row.Cells["StatusResposta"] != null ?
                    row.Cells["StatusResposta"].Value?.ToString() ?? "" : "";
                bool atrasado = row.Cells["Atrasado"] != null &&
                               Convert.ToBoolean(row.Cells["Atrasado"].Value ?? false);
                string prioridade = row.Cells["Prioridade"].Value?.ToString() ?? "";

                if (atrasado && status == "Aberto")
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
                else if (prioridade == "Alta" && status == "Aberto")
                {
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                }
                else if (statusResposta == "Respondido" && status == "Aberto")
                {
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
                }
                else if (status == "Finalizado")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }

        #endregion

        #region Eventos de Botões

        private void btnAnalise_Click(object sender, EventArgs e)
        {
            if (dgvAnalise.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um chamado para analisar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int chamadoId = Convert.ToInt32(dgvAnalise.SelectedRows[0].Cells["ChamadoId"].Value);

            AtendimentoFormProtocolo formProtocolo = new AtendimentoFormProtocolo(
                usuarioId,
                connectionString,
                chamadoId
            );
            formProtocolo.ShowDialog();

            CarregarChamados();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvAnalise.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um chamado para excluir.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int chamadoId = Convert.ToInt32(dgvAnalise.SelectedRows[0].Cells["ChamadoId"].Value);
            string assunto = dgvAnalise.SelectedRows[0].Cells["Assunto"].Value.ToString();

            DialogResult result = MessageBox.Show(
                $"Deseja realmente excluir o chamado #{chamadoId}?\n\nAssunto: {assunto}\n\nEsta ação não pode ser desfeita!",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string sql = "DELETE FROM Chamados WHERE ChamadoId = @ChamadoId";

                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@ChamadoId", chamadoId);

                        conn.Open();
                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                        {
                            MessageBox.Show($"Chamado #{chamadoId} excluído com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            CarregarChamados();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir chamado: {ex.Message}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnConcluido_Click(object sender, EventArgs e)
        {
            if (dgvAnalise.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um chamado para concluir.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int chamadoId = Convert.ToInt32(dgvAnalise.SelectedRows[0].Cells["ChamadoId"].Value);
            string status = dgvAnalise.SelectedRows[0].Cells["Status"].Value.ToString();
            string assunto = dgvAnalise.SelectedRows[0].Cells["Assunto"].Value.ToString();

            if (status == "Finalizado")
            {
                MessageBox.Show("Este chamado já está finalizado!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Deseja marcar o chamado #{chamadoId} como concluído?\n\nAssunto: {assunto}",
                "Confirmar Conclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string sql = @"UPDATE Chamados 
                                      SET Status = 'Finalizado', 
                                          DataFinalizacao = GETDATE(), 
                                          DataUltimaAtualizacao = GETDATE()
                                      WHERE ChamadoId = @ChamadoId";

                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@ChamadoId", chamadoId);

                        conn.Open();
                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                        {
                            MessageBox.Show($"Chamado #{chamadoId} concluído com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            CarregarChamados();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao concluir chamado: {ex.Message}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Eventos do Form

        private void AtendimentoForm_Load(object sender, EventArgs e)
        {
            CarregarChamados();
        }

        #endregion

        #region Métodos Públicos
        public void AtualizarGrid()
        {
            CarregarChamados();
        }

        #endregion

        #region Eventos dos Labels (Opcional - Filtros)

        private void lblNovo_Click(object sender, EventArgs e)
        {
            FiltrarChamados("Novo");
        }

        private void lblAndamento_Click(object sender, EventArgs e)
        {
            FiltrarChamados("Andamento");
        }

        private void lblAguardando_Click(object sender, EventArgs e)
        {
            FiltrarChamados("Aguardando");
        }

        private void lblResolvido_Click(object sender, EventArgs e)
        {
            FiltrarChamados("Resolvido");
        }

        private void lblAtrasado_Click(object sender, EventArgs e)
        {
            FiltrarChamados("Atrasado");
        }

        private void FiltrarChamados(string filtro)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "";

                    switch (filtro)
                    {
                        case "Novo":
                            sql = @"SELECT c.ChamadoId, c.Assunto, c.Prioridade, c.Status, c.StatusResposta,
                                           c.DataCriacao, c.Atrasado, c.SuporteId,
                                           u.UsuarioNome AS UsuarioNome,
                                           ISNULL(s.UsuarioNome, 'Não atribuído') AS SuporteNome
                                    FROM Chamados c
                                    INNER JOIN Usuarios u ON c.UsuarioId = u.Id
                                    LEFT JOIN Usuarios s ON c.SuporteId = s.Id
                                    WHERE c.Status = 'Aberto' AND c.SuporteId IS NULL
                                    ORDER BY c.DataCriacao DESC";
                            break;

                        case "Andamento":
                            sql = @"SELECT c.ChamadoId, c.Assunto, c.Prioridade, c.Status, c.StatusResposta,
                                           c.DataCriacao, c.Atrasado, c.SuporteId,
                                           u.UsuarioNome AS UsuarioNome,
                                           ISNULL(s.UsuarioNome, 'Não atribuído') AS SuporteNome
                                    FROM Chamados c
                                    INNER JOIN Usuarios u ON c.UsuarioId = u.Id
                                    LEFT JOIN Usuarios s ON c.SuporteId = s.Id
                                    WHERE c.Status = 'Aberto' AND c.SuporteId IS NOT NULL AND c.StatusResposta = 'Enviado'
                                    ORDER BY c.DataCriacao DESC";
                            break;

                        case "Aguardando":
                            sql = @"SELECT c.ChamadoId, c.Assunto, c.Prioridade, c.Status, c.StatusResposta,
                                           c.DataCriacao, c.Atrasado, c.SuporteId,
                                           u.UsuarioNome AS UsuarioNome,
                                           ISNULL(s.UsuarioNome, 'Não atribuído') AS SuporteNome
                                    FROM Chamados c
                                    INNER JOIN Usuarios u ON c.UsuarioId = u.Id
                                    LEFT JOIN Usuarios s ON c.SuporteId = s.Id
                                    WHERE c.Status = 'Aberto' AND c.StatusResposta = 'Respondido'
                                    ORDER BY c.DataCriacao DESC";
                            break;

                        case "Resolvido":
                            sql = @"SELECT c.ChamadoId, c.Assunto, c.Prioridade, c.Status, c.StatusResposta,
                                           c.DataCriacao, c.Atrasado, c.SuporteId,
                                           u.UsuarioNome AS UsuarioNome,
                                           ISNULL(s.UsuarioNome, 'Não atribuído') AS SuporteNome
                                    FROM Chamados c
                                    INNER JOIN Usuarios u ON c.UsuarioId = u.Id
                                    LEFT JOIN Usuarios s ON c.SuporteId = s.Id
                                    WHERE c.Status = 'Finalizado'
                                    ORDER BY c.DataFinalizacao DESC";
                            break;

                        case "Atrasado":
                            sql = @"SELECT c.ChamadoId, c.Assunto, c.Prioridade, c.Status, c.StatusResposta,
                                           c.DataCriacao, c.Atrasado, c.SuporteId,
                                           u.UsuarioNome AS UsuarioNome,
                                           ISNULL(s.UsuarioNome, 'Não atribuído') AS SuporteNome
                                    FROM Chamados c
                                    INNER JOIN Usuarios u ON c.UsuarioId = u.Id
                                    LEFT JOIN Usuarios s ON c.SuporteId = s.Id
                                    WHERE c.Status = 'Aberto' AND c.Atrasado = 1
                                    ORDER BY c.DataCriacao ASC";
                            break;

                        default:
                            CarregarChamados();
                            return;
                    }

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvAnalise.DataSource = dt;
                    dgvAnalise.ClearSelection();

                    this.Text = $"Atendimento - Suporte - {filtro}: {dt.Rows.Count} chamado(s)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao filtrar chamados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LimparFiltro()
        {
            this.Text = "Atendimento - Suporte";
            CarregarChamados();
        }

        #endregion
    }
}