using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Reverse.Forms.FormsRH
{
    public partial class FormCestaBasica : Form
    {
        private readonly string connectionString =
        ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;

        private readonly string[] meses = {
            "Janeiro","Fevereiro","Março","Abril","Maio","Junho",
            "Julho","Agosto","Setembro","Outubro","Novembro","Dezembro"
        };

        private readonly string[] statusOptions = { "Recebido", "Não levou", "Falta sem justificativa" };

        private bool isUpdatingChart = false;

        public FormCestaBasica(int _usuarioId)
        {
            InitializeComponent();
            ConfigurarGrid();
            ConfigurarChart();
            ConfigurarCombos();
        }

        private void FormCestaBasica_Load(object sender, EventArgs e)
        {
            cbMes.SelectedIndex = DateTime.Now.Month - 1;
            cbAno.SelectedItem = DateTime.Now.Year.ToString();
            CarregarFuncionarios(DateTime.Now.Month, DateTime.Now.Year);
        }

        private void ConfigurarCombos()
        {
            cbMes.Items.AddRange(meses);

            int anoAtual = DateTime.Now.Year;
            for (int ano = anoAtual - 5; ano <= anoAtual + 1; ano++)
                cbAno.Items.Add(ano.ToString());

            cbMes.SelectedIndexChanged += (s, e) => AtualizarPeriodo();
            cbAno.SelectedIndexChanged += (s, e) => AtualizarPeriodo();
        }

        private void AtualizarPeriodo()
        {
            if (cbMes.SelectedIndex >= 0 && cbAno.SelectedIndex >= 0)
            {
                int mes = cbMes.SelectedIndex + 1;
                if (int.TryParse(cbAno.SelectedItem?.ToString(), out int ano))
                {
                    CarregarFuncionarios(mes, ano);
                }
            }
        }

        private void ConfigurarGrid()
        {
            dgvCestaBasica.SuspendLayout();

            dgvCestaBasica.Columns.Clear();
            dgvCestaBasica.AutoGenerateColumns = false;
            dgvCestaBasica.AllowUserToAddRows = false;
            dgvCestaBasica.RowHeadersVisible = false;
            dgvCestaBasica.EnableHeadersVisualStyles = false;
            dgvCestaBasica.BackgroundColor = Color.FromArgb(30, 30, 45);
            dgvCestaBasica.GridColor = Color.Black;

            // Estilização
            dgvCestaBasica.DefaultCellStyle.ForeColor = Color.Black;
            dgvCestaBasica.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvCestaBasica.DefaultCellStyle.SelectionBackColor = Color.LightGray;
            dgvCestaBasica.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvCestaBasica.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvCestaBasica.RowTemplate.Height = 35;
            dgvCestaBasica.AllowUserToResizeRows = false;
            dgvCestaBasica.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            // Colunas
            var colId = new DataGridViewTextBoxColumn
            {
                HeaderText = "FuncionarioId",
                Name = "FuncionarioId",
                Visible = false
            };
            dgvCestaBasica.Columns.Add(colId);

            var colNome = new DataGridViewTextBoxColumn
            {
                HeaderText = "Nome",
                Name = "Nome",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 70,
                DisplayIndex = 0
            };
            dgvCestaBasica.Columns.Add(colNome);

            var colStatus = new DataGridViewComboBoxColumn
            {
                HeaderText = "Status",
                Name = "Status",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 30,
                DisplayIndex = 1
            };
            colStatus.Items.AddRange(statusOptions);
            dgvCestaBasica.Columns.Add(colStatus);

            // Eventos otimizados
            dgvCestaBasica.CellValueChanged += DgvCestaBasica_CellValueChanged;
            dgvCestaBasica.CurrentCellDirtyStateChanged += DgvCestaBasica_CurrentCellDirtyStateChanged;

            dgvCestaBasica.ResumeLayout();
        }

        private void DgvCestaBasica_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!isUpdatingChart)
                AtualizarChart();
        }

        private void DgvCestaBasica_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvCestaBasica.IsCurrentCellDirty)
                dgvCestaBasica.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void ConfigurarChart()
        {
            chartResumo.Series.Clear();
            chartResumo.ChartAreas.Clear();
            chartResumo.Legends.Clear();

            var chartArea = new ChartArea();
            chartResumo.ChartAreas.Add(chartArea);

            var series = new Series
            {
                Name = "Cesta",
                IsVisibleInLegend = true,
                ChartType = SeriesChartType.Pie,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                IsValueShownAsLabel = true,
                LabelForeColor = Color.White
            };
            series["PieLabelStyle"] = "Inside";
            chartResumo.Series.Add(series);

            var legend = new Legend
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            chartResumo.Legends.Add(legend);

        }

        private void btnExportarChart_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
                sfd.Title = "Salvar gráfico de Cesta Básica";
                sfd.FileName = $"CestaBasica_{DateTime.Now:yyyyMMdd}.png";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ChartImageFormat formato = sfd.FilterIndex == 2 ? ChartImageFormat.Jpeg : ChartImageFormat.Png;
                    chartResumo.SaveImage(sfd.FileName, formato);

                    MessageBox.Show("Gráfico exportado com sucesso!", "Exportação",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void AtualizarChart()
        {
            if (isUpdatingChart) return;

            isUpdatingChart = true;
            try
            {
                var contadores = new int[3]; // recebidos, naoLevaram, faltas

                foreach (DataGridViewRow row in dgvCestaBasica.Rows)
                {
                    if (row.Cells["Status"].Value?.ToString()?.Trim() is string status)
                    {
                        switch (status)
                        {
                            case "Recebido": contadores[0]++; break;
                            case "Não levou": contadores[1]++; break;
                            case "Falta sem justificativa": contadores[2]++; break;
                        }
                    }
                }

                var series = chartResumo.Series["Cesta"];
                series.Points.Clear();

                int total = contadores.Sum();
                if (total == 0) return;

                var dados = new[]
                {
                    new { Label = "Recebido", Valor = contadores[0], Cor = Color.Blue },
                    new { Label = "Não levou", Valor = contadores[1], Cor = Color.Orange },
                    new { Label = "Falta s/ justificativa", Valor = contadores[2], Cor = Color.Red }
                };

                foreach (var item in dados.Where(x => x.Valor > 0))
                {
                    var point = series.Points.AddXY(item.Label, item.Valor);
                    var addedPoint = series.Points[series.Points.Count - 1];
                    addedPoint.Color = item.Cor;

                    double percentual = (double)item.Valor / total;
                    int fontSize = percentual > 0.4 ? 16 : percentual > 0.2 ? 12 : percentual > 0.1 ? 10 : 8;

                    addedPoint.Label = $"{item.Label}\n{percentual:P0}";
                    addedPoint.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
                    addedPoint.LabelForeColor = Color.White;
                    addedPoint["PieLabelStyle"] = "Inside";
                }
            }
            finally
            {
                isUpdatingChart = false;
            }
        }

        private void CarregarFuncionarios(int mes, int ano)
        {
            dgvCestaBasica.SuspendLayout();
            isUpdatingChart = true;

            try
            {
                dgvCestaBasica.Rows.Clear();

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    const string sql = @"
                        SELECT f.FuncionarioId, f.Nome, cb.Status
                        FROM Funcionarios f
                        LEFT JOIN CestaBasica cb
                            ON cb.FuncionarioId = f.FuncionarioId
                            AND cb.Mes = @Mes
                            AND cb.Ano = @Ano
                        WHERE f.Status = 'Ativo'
                        ORDER BY f.Nome";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Mes", mes);
                        cmd.Parameters.AddWithValue("@Ano", ano);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int funcionarioId = reader.GetInt32(reader.GetOrdinal("FuncionarioId"));
                                string nomeCompleto = reader.GetString(reader.GetOrdinal("Nome"));
                                string nomeReduzido = ObterNomeReduzido(nomeCompleto);
                                object status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")).Trim();

                                dgvCestaBasica.Rows.Add(funcionarioId, nomeReduzido, status);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar funcionários: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isUpdatingChart = false;
                dgvCestaBasica.ResumeLayout();
                AtualizarChart();
            }
        }

        private static string ObterNomeReduzido(string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto)) return string.Empty;

            string[] partes = nomeCompleto.Trim().Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
            return partes.Length >= 2 ? $"{partes[0]} {partes[1]}" : partes[0];
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (cbMes.SelectedIndex < 0 || cbAno.SelectedItem == null)
            {
                MessageBox.Show("Selecione um período válido.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int mes = cbMes.SelectedIndex + 1;
            if (!int.TryParse(cbAno.SelectedItem.ToString(), out int ano))
            {
                MessageBox.Show("Ano inválido.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            const string sql = @"
                                MERGE CestaBasica AS target
                                USING (SELECT @FuncionarioId as FuncionarioId, @Mes as Mes, @Ano as Ano, @Status as Status) AS source
                                ON target.FuncionarioId = source.FuncionarioId AND target.Mes = source.Mes AND target.Ano = source.Ano
                                WHEN MATCHED THEN
                                    UPDATE SET Status = source.Status
                                WHEN NOT MATCHED THEN
                                    INSERT (FuncionarioId, Mes, Ano, Status) VALUES (source.FuncionarioId, source.Mes, source.Ano, source.Status);";

                            foreach (DataGridViewRow row in dgvCestaBasica.Rows)
                            {
                                if (row.Cells["Status"].Value?.ToString()?.Trim() is string status &&
                                    !string.IsNullOrWhiteSpace(status))
                                {
                                    using (var cmd = new SqlCommand(sql, conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@FuncionarioId", row.Cells["FuncionarioId"].Value);
                                        cmd.Parameters.AddWithValue("@Mes", mes);
                                        cmd.Parameters.AddWithValue("@Ano", ano);
                                        cmd.Parameters.AddWithValue("@Status", status);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                            MessageBox.Show("Dados salvos com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Limpar eventos para evitar vazamentos
                if (dgvCestaBasica != null)
                {
                    dgvCestaBasica.CellValueChanged -= DgvCestaBasica_CellValueChanged;
                    dgvCestaBasica.CurrentCellDirtyStateChanged -= DgvCestaBasica_CurrentCellDirtyStateChanged;
                }
            }
            base.Dispose(disposing);
        }
    }
}