using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Reverse.Forms.FormsRH
{
    public partial class RHFormCesta : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private int usuarioId;
        private DataTable dtFuncionarios;

        public RHFormCesta(int _usuarioId)
        {
            InitializeComponent();
            this.usuarioId = _usuarioId;
        }

        private void RHFormCesta_Load(object sender, EventArgs e)
        {
            InicializarComboBoxes();
            ConfigurarDataGridView();
            ConfigurarGrafico();

            CarregarDados();
        }

        #region Inicialização

        private void InicializarComboBoxes()
        {
            cmbMes.SelectedIndexChanged -= cmbMes_SelectedIndexChanged;
            cmbAno.SelectedIndexChanged -= cmbAno_SelectedIndexChanged;

            cmbMes.Items.Clear();
            cmbMes.Items.AddRange(new object[] {
                new { Text = "Janeiro",   Value = 1  },
                new { Text = "Fevereiro", Value = 2  },
                new { Text = "Março",     Value = 3  },
                new { Text = "Abril",     Value = 4  },
                new { Text = "Maio",      Value = 5  },
                new { Text = "Junho",     Value = 6  },
                new { Text = "Julho",     Value = 7  },
                new { Text = "Agosto",    Value = 8  },
                new { Text = "Setembro",  Value = 9  },
                new { Text = "Outubro",   Value = 10 },
                new { Text = "Novembro",  Value = 11 },
                new { Text = "Dezembro",  Value = 12 }
            });
            cmbMes.DisplayMember = "Text";
            cmbMes.ValueMember = "Value";
            cmbMes.SelectedIndex = DateTime.Now.Month - 1;

            cmbAno.Items.Clear();
            int anoAtual = DateTime.Now.Year;
            for (int i = anoAtual - 5; i <= anoAtual + 1; i++)
                cmbAno.Items.Add(i);
            cmbAno.SelectedItem = anoAtual;

            cmbMes.SelectedIndexChanged += cmbMes_SelectedIndexChanged;
            cmbAno.SelectedIndexChanged += cmbAno_SelectedIndexChanged;
        }

        private void ConfigurarDataGridView()
        {
            dgvFuncionarios.AutoGenerateColumns = false;
            dgvFuncionarios.AllowUserToAddRows = false;
            dgvFuncionarios.AllowUserToDeleteRows = false;
            dgvFuncionarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFuncionarios.MultiSelect = false;
            dgvFuncionarios.RowHeadersVisible = false;
            dgvFuncionarios.BorderStyle = BorderStyle.FixedSingle;
            dgvFuncionarios.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvFuncionarios.EnableHeadersVisualStyles = false;
            dgvFuncionarios.AllowUserToResizeRows = false;

            // Estilo
            dgvFuncionarios.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvFuncionarios.DefaultCellStyle.ForeColor = Color.Black;
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvFuncionarios.BackgroundColor = Color.FromArgb(250, 250, 252);
            dgvFuncionarios.GridColor = Color.FromArgb(230, 230, 235);

            dgvFuncionarios.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvFuncionarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvFuncionarios.ColumnHeadersHeight = 40;
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 3, 0, 3);

            dgvFuncionarios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            dgvFuncionarios.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgvFuncionarios.RowsDefaultCellStyle.BackColor = Color.White;
            dgvFuncionarios.RowsDefaultCellStyle.ForeColor = Color.Black;

            dgvFuncionarios.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            dgvFuncionarios.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvFuncionarios.DefaultCellStyle.Padding = new Padding(3, 5, 3, 5);
            dgvFuncionarios.RowTemplate.Height = 35;

            // Colunas
            dgvFuncionarios.Columns.Clear();

            // Coluna oculta com ID do funcionário
            DataGridViewTextBoxColumn colFuncionarioID = new DataGridViewTextBoxColumn
            {
                Name = "FuncionarioID",
                DataPropertyName = "FuncionarioID",
                Visible = false
            };
            dgvFuncionarios.Columns.Add(colFuncionarioID);

            // Coluna com nome do funcionário (somente leitura)
            DataGridViewTextBoxColumn colNome = new DataGridViewTextBoxColumn
            {
                Name = "Funcionario",
                DataPropertyName = "Nome",
                HeaderText = "Funcionário",
                Width = 250,
                ReadOnly = true
            };
            dgvFuncionarios.Columns.Add(colNome);

            // Coluna com ComboBox de Status (editável)
            DataGridViewComboBoxColumn colStatus = new DataGridViewComboBoxColumn
            {
                Name = "Status",
                DataPropertyName = "Status",
                HeaderText = "Status",
                Width = 160,
                Items = { "Recebido", "Não pegou", "Falta s/ Justificativa" }
            };
            dgvFuncionarios.Columns.Add(colStatus);

            // Eventos
            dgvFuncionarios.Resize += DgvFuncionarios_Resize;
            AjustarColunas();

            dgvFuncionarios.CellValueChanged += DgvFuncionarios_CellValueChanged;
            dgvFuncionarios.CurrentCellDirtyStateChanged += DgvFuncionarios_CurrentCellDirtyStateChanged;
        }

        private void DgvFuncionarios_Resize(object sender, EventArgs e)
        {
            AjustarColunas();
        }
        private void AjustarColunas()
        {
            if (dgvFuncionarios.Columns.Count < 2) return;

            int larguraTotal = dgvFuncionarios.ClientSize.Width;
            int larguraStatus = 160;
            int larguraNome = larguraTotal - larguraStatus;

            dgvFuncionarios.Columns["Funcionario"].Width = larguraNome > 0 ? larguraNome : 200;
            dgvFuncionarios.Columns["Status"].Width = larguraStatus;
        }

        private void ConfigurarGrafico()
        {
            chGrafico.Series.Clear();
            chGrafico.Titles.Clear();
            chGrafico.ChartAreas.Clear();
            chGrafico.Legends.Clear();

            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.BackColor = Color.Transparent;
            chartArea.Position = new ElementPosition(0, 0, 100, 100);
            chartArea.InnerPlotPosition = new ElementPosition(5, 10, 90, 80);
            chGrafico.ChartAreas.Add(chartArea);

            Title title = new Title
            {
                Text = "Estatísticas de Cesta Básica",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Docking = Docking.Top
            };
            chGrafico.Titles.Add(title);

            Series series = new Series
            {
                Name = "Status",
                ChartType = SeriesChartType.Doughnut,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                LabelForeColor = Color.White,
                ["PieDrawingStyle"] = "Concave",
                ["DoughnutRadius"] = "35",
                ["PieLabelStyle"] = "Inside"
            };
            chGrafico.Series.Add(series);

            chGrafico.BackColor = Color.Transparent;

            Legend legend = new Legend("MainLegend")
            {
                Enabled = false
            };
            chGrafico.Legends.Add(legend);

            LegendItem liRecebido = new LegendItem();
            liRecebido.Name = "Recebido";
            liRecebido.Color = Color.FromArgb(46, 204, 113);
            liRecebido.BorderColor = Color.Transparent;
            legend.CustomItems.Add(liRecebido);

            LegendItem liNaoPegou = new LegendItem();
            liNaoPegou.Name = "Não Pegou";
            liNaoPegou.Color = Color.FromArgb(52, 152, 219);
            liNaoPegou.BorderColor = Color.Transparent;
            legend.CustomItems.Add(liNaoPegou);

            LegendItem liFalta = new LegendItem();
            liFalta.Name = "Falta s/ Justificativa";
            liFalta.Color = Color.FromArgb(231, 76, 60);
            liFalta.BorderColor = Color.Transparent;
            legend.CustomItems.Add(liFalta);
        }

        #endregion

        #region Eventos do DataGridView

        private void DgvFuncionarios_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvFuncionarios.IsCurrentCellDirty)
            {
                dgvFuncionarios.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvFuncionarios_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (dgvFuncionarios.Columns[e.ColumnIndex].Name == "Status")
                {
                    
                }
            }
        }

        #endregion

        #region Eventos dos ComboBox

        private void cmbMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMes.SelectedIndex >= 0 && cmbAno.SelectedItem != null)
            {
                CarregarDados();
            }
        }

        private void cmbAno_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMes.SelectedIndex >= 0 && cmbAno.SelectedItem != null)
            {
                CarregarDados();
            }
        }

        #endregion

        #region Carregar Dados

        private void CarregarDados()
        {
            try
            {
                int mes = Convert.ToInt32(((dynamic)cmbMes.SelectedItem).Value);
                int ano = Convert.ToInt32(cmbAno.SelectedItem);

                CarregarFuncionarios(mes, ano);
                AtualizarGrafico(mes, ano);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarFuncionarios(int mes, int ano)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            f.FuncionarioID,
                            f.Nome,
                            ISNULL(cb.Status, '') AS Status
                        FROM RHFuncionarios f
                        LEFT JOIN RHCestaBasica cb ON f.FuncionarioID = cb.FuncionarioID 
                            AND cb.Mes = @Mes 
                            AND cb.Ano = @Ano
                        WHERE f.Ativo = 1
                        ORDER BY f.Nome";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Mes", mes);
                        cmd.Parameters.AddWithValue("@Ano", ano);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        dtFuncionarios = new DataTable();
                        adapter.Fill(dtFuncionarios);

                        dgvFuncionarios.DataSource = dtFuncionarios;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar funcionários: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarGrafico(int mes, int ano)
        {
            try
            {
                if (chGrafico.Series.Count == 0) return;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ObterEstatisticasCestaBasica", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mes", SqlDbType.Int).Value = mes;
                        cmd.Parameters.Add("@Ano", SqlDbType.Int).Value = ano;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        chGrafico.Series[0].Points.Clear();
                        chGrafico.Titles[0].Text = "Estatísticas de Cesta Básica";

                        if (dt.Rows.Count == 0) return;

                        int total = dt.AsEnumerable().Sum(r => Convert.ToInt32(r["Quantidade"]));

                        foreach (DataRow row in dt.Rows)
                        {
                            string status = row["Status"].ToString();
                            int quantidade = Convert.ToInt32(row["Quantidade"]);
                            double percentual = total > 0 ? quantidade * 100.0 / total : 0;

                            DataPoint point = new DataPoint();
                            point.SetValueXY(status, quantidade);

                            point.Label = $"{status}\n{quantidade} ({percentual:F1}%)";
                            point.LabelForeColor = Color.White;

                            switch (status)
                            {
                                case "Recebido":
                                    point.Color = Color.FromArgb(46, 204, 113);
                                    break;
                                case "Não pegou":
                                    point.Color = Color.FromArgb(52, 152, 219);
                                    break;
                                case "Falta s/ Justificativa":
                                    point.Color = Color.FromArgb(231, 76, 60);
                                    break;
                                default:
                                    point.Color = Color.Gray;
                                    break;
                            }

                            chGrafico.Series[0].Points.Add(point);
                        }

                        VerificarFuncionariosInativosNoGrafico(mes, ano);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar gráfico: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Botões

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbMes.SelectedIndex < 0 || cmbAno.SelectedItem == null)
                {
                    MessageBox.Show("Selecione o mês e o ano antes de salvar.", "Atenção",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int mes = Convert.ToInt32(((dynamic)cmbMes.SelectedItem).Value);
                int ano = Convert.ToInt32(cmbAno.SelectedItem);

                foreach (DataGridViewRow row in dgvFuncionarios.Rows)
                {
                    if (row.Cells["Status"].Value == null ||
                        string.IsNullOrEmpty(row.Cells["Status"].Value.ToString()))
                    {
                        MessageBox.Show("Por favor, preencha o status de todos os funcionários antes de salvar.",
                            "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand("sp_SalvarCestaBasica", conn, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@FuncionarioID", SqlDbType.Int);
                                cmd.Parameters.Add("@Mes", SqlDbType.Int);
                                cmd.Parameters.Add("@Ano", SqlDbType.Int);
                                cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50);
                                cmd.Parameters.Add("@UsuarioID", SqlDbType.Int);

                                foreach (DataGridViewRow row in dgvFuncionarios.Rows)
                                {
                                    string status = row.Cells["Status"].Value?.ToString() ?? "";
                                    if (string.IsNullOrEmpty(status)) continue;

                                    cmd.Parameters["@FuncionarioID"].Value = Convert.ToInt32(row.Cells["FuncionarioID"].Value);
                                    cmd.Parameters["@Mes"].Value = mes;
                                    cmd.Parameters["@Ano"].Value = ano;
                                    cmd.Parameters["@Status"].Value = status;
                                    cmd.Parameters["@UsuarioID"].Value = usuarioId;

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();

                            MessageBox.Show("Registros salvos com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            AtualizarGrafico(mes, ano);
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar registros: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Deseja realmente cancelar? Todas as alterações não salvas serão perdidas.",
                "Confirmação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (cmbMes.SelectedIndex >= 0 && cmbAno.SelectedItem != null)
                {
                    CarregarDados();
                }
                else
                {
                    dgvFuncionarios.DataSource = null;
                    chGrafico.Series[0].Points.Clear();
                }
            }
        }

        private void VerificarFuncionariosInativosNoGrafico(int mes, int ano)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT f.Nome, cb.Status
                        FROM RHCestaBasica cb
                        INNER JOIN RHFuncionarios f ON cb.FuncionarioID = f.FuncionarioID
                        WHERE cb.Mes = @Mes
                          AND cb.Ano = @Ano
                          AND f.Ativo = 0
                        ORDER BY f.Nome";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@Mes", SqlDbType.Int).Value = mes;
                        cmd.Parameters.Add("@Ano", SqlDbType.Int).Value = ano;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            var nomes = string.Join("\n  • ", dt.AsEnumerable()
                                .Select(r => $"{r["Nome"]} ({r["Status"]})"));

                            MessageBox.Show(
                                $"Atenção: os funcionários abaixo foram desativados, mas possuem registros de cesta básica neste período e ainda constam no gráfico:\n\n  • {nomes}",
                                "Funcionários Inativos no Gráfico",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao verificar inativos: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}