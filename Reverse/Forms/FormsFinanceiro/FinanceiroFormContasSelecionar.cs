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

namespace Reverse.Forms.FormsFinanceiro
{
    public partial class FinanceiroFormContasSelecionar : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;

        public int LoteIdSelecionado { get; private set; }
        public DateTime DataLoteSelecionado { get; private set; }

        public FinanceiroFormContasSelecionar()
        {
            InitializeComponent();
            ConfigurarFormulario();
            BtnFiltrar_Click(null, null);
        }

        private void ConfigurarFormulario()
        {
            CarregarMeses();
            CarregarAnos();

            cmbMes.SelectedValue = DateTime.Now.Month;
            cmbAno.SelectedValue = DateTime.Now.Year;

            btnContasSelecionar.Click += BtnCriarLoteData_Click;
            btnFiltrar.Click += BtnFiltrar_Click;
            btnAbrir.Click += BtnAbrir_Click;
            btnSair.Click += BtnSair_Click;

            dgvLotes.MultiSelect = false;
            dgvLotes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLotes.ReadOnly = true;
            dgvLotes.RowHeadersVisible = false;
            dgvLotes.AllowUserToAddRows = false;
            dgvLotes.AllowUserToDeleteRows = false;
            dgvLotes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLotes.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvLotes.Dock = DockStyle.Fill;

            dgvLotes.DefaultCellStyle.ForeColor = Color.Black;
            dgvLotes.DefaultCellStyle.BackColor = Color.White;
            dgvLotes.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvLotes.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgvLotes.EnableHeadersVisualStyles = false;
            dgvLotes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
            dgvLotes.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;

        }


        private void CarregarMeses()
        {
            var meses = new[]
            {
                new { Valor = 1, Nome = "Janeiro" },
                new { Valor = 2, Nome = "Fevereiro" },
                new { Valor = 3, Nome = "Março" },
                new { Valor = 4, Nome = "Abril" },
                new { Valor = 5, Nome = "Maio" },
                new { Valor = 6, Nome = "Junho" },
                new { Valor = 7, Nome = "Julho" },
                new { Valor = 8, Nome = "Agosto" },
                new { Valor = 9, Nome = "Setembro" },
                new { Valor = 10, Nome = "Outubro" },
                new { Valor = 11, Nome = "Novembro" },
                new { Valor = 12, Nome = "Dezembro" }
            };

            cmbMes.DataSource = meses;
            cmbMes.ValueMember = "Valor";
            cmbMes.DisplayMember = "Nome";
        }

        private void CarregarAnos()
        {
            var anos = Enumerable.Range(DateTime.Now.Year - 5, 11)
                                .Select(a => new { Valor = a, Nome = a.ToString() })
                                .ToList();

            cmbAno.DataSource = anos;
            cmbAno.ValueMember = "Valor";
            cmbAno.DisplayMember = "Nome";
        }

        private void CarregarDados()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT
                    l.LoteId,
                    l.DataLote,
                    COUNT(cp.Id) AS QuantidadeContas,
                    ISNULL(SUM(cp.Valor), 0) AS ValorTotal,
                    CASE 
                        WHEN SUM(CASE WHEN cp.StatusPagamento = 0 THEN 1 ELSE 0 END) > 0 THEN 'Pendente'
                        ELSE 'Tudo pago'
                    END AS StatusLote
                FROM LotesContasPagar l
                LEFT JOIN ContasPagar cp ON cp.LoteId = l.LoteId
                GROUP BY l.LoteId, l.DataLote
                ORDER BY l.DataLote DESC";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvLotes.DataSource = dt;
                    }

                    ConfigurarGridLotes();
                }

                CalcularTotais();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarGridLotes()
        {
            if (dgvLotes.Columns.Count > 0)
            {
                if (dgvLotes.Columns.Contains("LoteId"))
                {
                    dgvLotes.Columns["LoteId"].Visible = false;
                }

                if (dgvLotes.Columns.Contains("DataLote"))
                {
                    dgvLotes.Columns["DataLote"].HeaderText = "Data do lote";
                    dgvLotes.Columns["DataLote"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvLotes.Columns["DataLote"].FillWeight = 25;
                }

                if (dgvLotes.Columns.Contains("QuantidadeContas"))
                {
                    dgvLotes.Columns["QuantidadeContas"].HeaderText = "Quantidade";
                    dgvLotes.Columns["QuantidadeContas"].FillWeight = 20;
                    dgvLotes.Columns["QuantidadeContas"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (dgvLotes.Columns.Contains("ValorTotal"))
                {
                    dgvLotes.Columns["ValorTotal"].HeaderText = "Valor do lote";
                    dgvLotes.Columns["ValorTotal"].DefaultCellStyle.Format = "C2";
                    dgvLotes.Columns["ValorTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvLotes.Columns["ValorTotal"].FillWeight = 25;
                }

                if (dgvLotes.Columns.Contains("StatusLote"))
                {
                    dgvLotes.Columns["StatusLote"].HeaderText = "Status";
                    dgvLotes.Columns["StatusLote"].FillWeight = 30;
                }

                dgvLotes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }
        private void BtnCriarLoteData_Click(object sender, EventArgs e)
        {
            using (var formCalendar = new FormSelecionarData())
            {
                if (formCalendar.ShowDialog() == DialogResult.OK)
                {
                    DateTime dataSelecionada = formCalendar.DataSelecionada;

                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            string checkQuery = "SELECT COUNT(*) FROM LotesContasPagar WHERE DataLote = @DataLote";
                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                            {
                                checkCmd.Parameters.AddWithValue("@DataLote", dataSelecionada);
                                int exists = (int)checkCmd.ExecuteScalar();

                                if (exists > 0)
                                {
                                    MessageBox.Show("Já existe um lote para esta data!", "Aviso",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }

                            string insertQuery = "INSERT INTO LotesContasPagar (DataLote) VALUES (@DataLote)";
                            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@DataLote", dataSelecionada);
                                cmd.ExecuteNonQuery();
                            }

                            MessageBox.Show("Lote criado com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CarregarDados();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao criar lote: {ex.Message}", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnFiltrar_Click(object sender, EventArgs e)
        {
            if (cmbMes.SelectedValue == null || cmbAno.SelectedValue == null)
            {
                MessageBox.Show("Selecione o mês e ano para filtrar!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int mes = (int)cmbMes.SelectedValue;
                int ano = (int)cmbAno.SelectedValue;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT
                    l.LoteId,
                    l.DataLote,
                    COUNT(cp.Id) AS QuantidadeContas,
                    ISNULL(SUM(cp.Valor), 0) AS ValorTotal,
                    CASE 
                        WHEN SUM(CASE WHEN cp.StatusPagamento = 0 THEN 1 ELSE 0 END) > 0 THEN 'Pendente'
                        ELSE 'Tudo pago'
                    END AS StatusLote
                FROM LotesContasPagar l
                LEFT JOIN ContasPagar cp ON cp.LoteId = l.LoteId
                WHERE MONTH(l.DataLote) = @Mes AND YEAR(l.DataLote) = @Ano
                GROUP BY l.LoteId, l.DataLote
                ORDER BY l.DataLote DESC";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@Mes", mes);
                        adapter.SelectCommand.Parameters.AddWithValue("@Ano", ano);

                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvLotes.DataSource = dt;
                    }

                    ConfigurarGridLotes();
                    CalcularTotais();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao filtrar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void BtnAbrir_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = null;

            if (dgvLotes.SelectedRows.Count > 0)
                row = dgvLotes.SelectedRows[0];
            else
                row = dgvLotes.CurrentRow;

            if (row == null)
            {
                MessageBox.Show("Selecione um lote para abrir!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoteIdSelecionado = Convert.ToInt32(row.Cells["LoteId"].Value);
            DataLoteSelecionado = Convert.ToDateTime(row.Cells["DataLote"].Value);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void BtnSair_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void AtualizarTotais()
        {
            CalcularTotais();
        }

        private void CalcularTotais()
        {
            try
            {
                int mesSelecionado = cmbMes.SelectedValue != null ? (int)cmbMes.SelectedValue : DateTime.Today.Month;
                int anoSelecionado = cmbAno.SelectedValue != null ? (int)cmbAno.SelectedValue : DateTime.Today.Year;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
            SELECT 
                -- Pendente: tudo que está aberto (status 0), incluindo vencimento próximo
                SUM(CASE WHEN cp.StatusPagamento = 0 THEN cp.Valor ELSE 0 END) AS TotalPendente,

                -- Vencido: status 2
                SUM(CASE WHEN cp.StatusPagamento = 2 THEN cp.Valor ELSE 0 END) AS TotalVencido,

                -- Pago: status 1
                SUM(CASE WHEN cp.StatusPagamento = 1 THEN cp.Valor ELSE 0 END) AS TotalPago
            FROM ContasPagar cp
            INNER JOIN LotesContasPagar l ON cp.LoteId = l.LoteId
            WHERE MONTH(l.DataLote) = @Mes AND YEAR(l.DataLote) = @Ano";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Mes", mesSelecionado);
                        cmd.Parameters.AddWithValue("@Ano", anoSelecionado);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal totalPendente = reader["TotalPendente"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPendente"]) : 0;
                                decimal totalVencido = reader["TotalVencido"] != DBNull.Value ? Convert.ToDecimal(reader["TotalVencido"]) : 0;
                                decimal totalPago = reader["TotalPago"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPago"]) : 0;

                                lblTotalPendente.Text = $"Total Pendente: {totalPendente:C}";
                                lblTotalPago.Text = $"Total Pago: {totalPago:C}";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao calcular totais: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}