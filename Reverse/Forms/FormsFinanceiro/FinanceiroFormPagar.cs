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
    public partial class FormPagar : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private int loteAtualId = 0;
        private DateTime dataLoteAtual = DateTime.Today;
        private DataTable dtCategorias;

        public FormPagar(int _usuarioId)
        {
            InitializeComponent();
            ConfigurarFormulario();
            CarregarCategorias();
        }

        private void ConfigurarFormulario()
        {
            ConfigurarGrid();

            btnContasSelecionar.Click += BtnContasSelecionar_Click;
            btnCriar.Click += BtnCriar_Click;
            btnExcluir.Click += BtnExcluir_Click;
            btnPago.Click += BtnPago_Click;
            dgvContasPagar.CellValueChanged += DgvContasPagar_CellValueChanged;
            dgvContasPagar.CellClick += DgvContasPagar_CellClick;
            dgvContasPagar.DataBindingComplete += DgvContasPagar_DataBindingComplete;
            dgvContasPagar.DataError += DgvContasPagar_DataError;
            dgvContasPagar.CellFormatting += DgvContasPagar_CellFormatting_Simple;

            dgvContasPagar.DefaultCellStyle.ForeColor = Color.Black;
            dgvContasPagar.DefaultCellStyle.BackColor = Color.White;
            dgvContasPagar.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvContasPagar.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgvContasPagar.EnableHeadersVisualStyles = false;
            dgvContasPagar.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
            dgvContasPagar.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
        }

        private void ConfigurarGrid()
        {
            dgvContasPagar.Columns.Clear();

            dgvContasPagar.MultiSelect = false;
            dgvContasPagar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvContasPagar.RowHeadersVisible = false;
            dgvContasPagar.AllowUserToAddRows = false;
            dgvContasPagar.AllowUserToDeleteRows = false;
            dgvContasPagar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvContasPagar.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvContasPagar.Dock = DockStyle.Fill;

            // Coluna ID (oculta)
            var colId = new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                Visible = false
            };
            dgvContasPagar.Columns.Add(colId);

            // Coluna Categoria (ComboBox)
            var colCategoria = new DataGridViewComboBoxColumn
            {
                Name = "CategoriaId",
                HeaderText = "Categoria",
                ValueMember = "Id",
                DisplayMember = "Nome",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                FlatStyle = FlatStyle.Flat,
                FillWeight = 20
            };
            dgvContasPagar.Columns.Add(colCategoria);

            // Coluna Observações
            var colObservacao = new DataGridViewTextBoxColumn
            {
                Name = "Observacao",
                HeaderText = "Observações",
                FillWeight = 30
            };
            dgvContasPagar.Columns.Add(colObservacao);

            var colValor = new DataGridViewTextBoxColumn
            {
                Name = "Valor",
                HeaderText = "Valor",
                FillWeight = 15
            };
            colValor.DefaultCellStyle.Format = "C2";
            colValor.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colValor.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            dgvContasPagar.Columns.Add(colValor);

            // Coluna Data Vencimento
            var colDataVencimento = new DataGridViewTextBoxColumn
            {
                Name = "DataVencimento",
                HeaderText = "Data Vencimento",
                FillWeight = 12
            };
            colDataVencimento.DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvContasPagar.Columns.Add(colDataVencimento);

            // Coluna Data Pagamento
            var colDataPagamento = new DataGridViewTextBoxColumn
            {
                Name = "DataPagamento",
                HeaderText = "Data Pagamento",
                FillWeight = 12
                //EXPLICITO: removido ReadOnly = true para permitir edição manual
            };
            colDataPagamento.DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvContasPagar.Columns.Add(colDataPagamento);

            // Coluna Status (oculta)
            var colStatus = new DataGridViewTextBoxColumn
            {
                Name = "StatusPagamento",
                HeaderText = "Status",
                Visible = false
            };
            dgvContasPagar.Columns.Add(colStatus);

            // Coluna Status Display
            var colStatusDisplay = new DataGridViewTextBoxColumn
            {
                Name = "StatusDisplay",
                HeaderText = "Status",
                ReadOnly = true,
                FillWeight = 11
            };
            dgvContasPagar.Columns.Add(colStatusDisplay);
        }
        private void DgvContasPagar_CellFormatting_Simple(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvContasPagar.Columns[e.ColumnIndex].Name == "Valor" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal valor))
                {
                    e.Value = valor.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
                    e.FormattingApplied = true;
                }
            }
        }

        private void DgvContasPagar_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;

            if (dgvContasPagar.Columns[e.ColumnIndex].Name == "Valor")
            {
                dgvContasPagar.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
            }
        }

        private void CarregarCategorias()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT Id, Nome FROM Categorias ORDER BY Nome";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        dtCategorias = new DataTable();
                        adapter.Fill(dtCategorias);

                        // Configurar a coluna categoria com os dados
                        if (dgvContasPagar.Columns["CategoriaId"] is DataGridViewComboBoxColumn colCategoria)
                        {
                            colCategoria.DataSource = dtCategorias;
                            colCategoria.ValueMember = "Id";
                            colCategoria.DisplayMember = "Nome";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar categorias: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnContasSelecionar_Click(object sender, EventArgs e)
        {
            using (FinanceiroFormContasSelecionar form = new FinanceiroFormContasSelecionar())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    loteAtualId = form.LoteIdSelecionado;
                    dataLoteAtual = form.DataLoteSelecionado;
                    lblLoteAtual.Text = $"Lote: {dataLoteAtual:dd/MM/yyyy}";
                    CarregarContasLote();
                }
            }
        }

        private void CarregarContasLote()
        {
            if (loteAtualId == 0) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                    SELECT 
                        cp.Id,
                        cp.CategoriaId,
                        cp.Observacao,
                        cp.Valor,
                        cp.DataVencimento,
                        cp.DataPagamento,
                        cp.StatusPagamento
                    FROM ContasPagar cp
                    WHERE cp.LoteId = @LoteId
                    ORDER BY cp.DataVencimento";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@LoteId", loteAtualId);

                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Limpar grid
                        dgvContasPagar.Rows.Clear();

                        // Adicionar linhas
                        foreach (DataRow row in dt.Rows)
                        {
                            int rowIndex = dgvContasPagar.Rows.Add();
                            DataGridViewRow gridRow = dgvContasPagar.Rows[rowIndex];

                            gridRow.Cells["Id"].Value = row["Id"];
                            gridRow.Cells["CategoriaId"].Value = row["CategoriaId"];
                            gridRow.Cells["Observacao"].Value = row["Observacao"];
                            gridRow.Cells["Valor"].Value = row["Valor"];
                            gridRow.Cells["DataVencimento"].Value = row["DataVencimento"];
                            gridRow.Cells["DataPagamento"].Value = row.IsNull("DataPagamento") ? null : row["DataPagamento"];
                            gridRow.Cells["StatusPagamento"].Value = row["StatusPagamento"];

                            // Calcular e definir status display e cor da linha
                            AtualizarStatusLinha(gridRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar contas: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarStatusLinha(DataGridViewRow row)
        {
            if (row == null) return;

            int statusPagamento = Convert.ToInt32(row.Cells["StatusPagamento"].Value);
            DateTime hoje = DateTime.Today;
            DateTime? dataVencimento = null;

            if (row.Cells["DataVencimento"].Value != null &&
                DateTime.TryParse(row.Cells["DataVencimento"].Value.ToString(), out DateTime dv))
            {
                dataVencimento = dv;
            }

            if (statusPagamento == 1)
            {
                row.Cells["StatusDisplay"].Value = "Pago";
                row.DefaultCellStyle.BackColor = Color.LightGreen;
            }
            else if (statusPagamento == 0 && dataVencimento.HasValue)
            {
                int diasParaVencimento = (dataVencimento.Value - hoje).Days;

                if (diasParaVencimento < 0)
                {
                    row.Cells["StatusDisplay"].Value = "Vencido";
                    row.DefaultCellStyle.BackColor = Color.LightPink;
                }
                else if (diasParaVencimento <= 3)
                {
                    row.Cells["StatusDisplay"].Value = "Vencimento Próximo";
                    row.DefaultCellStyle.BackColor = Color.Khaki;
                }
                else
                {
                    row.Cells["StatusDisplay"].Value = "Pendente";
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void BtnCriar_Click(object sender, EventArgs e)
        {
            if (loteAtualId == 0)
            {
                MessageBox.Show("Selecione um lote primeiro!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtCategorias == null || dtCategorias.Rows.Count == 0)
            {
                MessageBox.Show("Não há categorias disponíveis!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string query = @"
                    INSERT INTO ContasPagar (CategoriaId, Observacao, Valor, DataVencimento, StatusPagamento, LoteId, DataCadastro)
                    VALUES (@CategoriaId, @Observacao, @Valor, @DataVencimento, @StatusPagamento, @LoteId, @DataCadastro);
                    SELECT SCOPE_IDENTITY();";

                            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CategoriaId", dtCategorias.Rows[0]["Id"]);
                                cmd.Parameters.AddWithValue("@Observacao", "");
                                cmd.Parameters.AddWithValue("@Valor", 0);
                                cmd.Parameters.AddWithValue("@DataVencimento", DateTime.Today);
                                cmd.Parameters.AddWithValue("@StatusPagamento", 0);
                                cmd.Parameters.AddWithValue("@LoteId", loteAtualId);
                                cmd.Parameters.AddWithValue("@DataCadastro", DateTime.Now);

                                int novoId = Convert.ToInt32(cmd.ExecuteScalar());

                                transaction.Commit();

                                // Adicionar linha no grid
                                int rowIndex = dgvContasPagar.Rows.Add();
                                DataGridViewRow row = dgvContasPagar.Rows[rowIndex];

                                row.Cells["Id"].Value = novoId;
                                row.Cells["CategoriaId"].Value = dtCategorias.Rows[0]["Id"];
                                row.Cells["Observacao"].Value = "";
                                row.Cells["Valor"].Value = 0m;
                                row.Cells["DataVencimento"].Value = DateTime.Today;
                                row.Cells["DataPagamento"].Value = null;
                                row.Cells["StatusPagamento"].Value = 0;

                                AtualizarStatusLinha(row);

                                // Selecionar a nova linha
                                dgvContasPagar.CurrentCell = row.Cells["CategoriaId"];
                                dgvContasPagar.BeginEdit(true);
                            }
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
                MessageBox.Show($"Erro ao criar nova linha: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvContasPagar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione uma linha para excluir!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Deseja realmente excluir esta linha?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                DataGridViewRow row = dgvContasPagar.SelectedRows[0];
                int id = Convert.ToInt32(row.Cells["Id"].Value);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM ContasPagar WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                dgvContasPagar.Rows.RemoveAt(row.Index);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir linha: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPago_Click(object sender, EventArgs e)
        {
            if (dgvContasPagar.CurrentRow == null) return;

            DataGridViewRow row = dgvContasPagar.CurrentRow;
            int id = Convert.ToInt32(row.Cells["Id"].Value);
            DateTime dataPagamento = DateTime.Today;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                UPDATE ContasPagar
                SET StatusPagamento = 1, DataPagamento = @DataPagamento
                WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@DataPagamento", dataPagamento);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Atualiza a grid local
                row.Cells["StatusPagamento"].Value = 1;
                row.Cells["DataPagamento"].Value = dataPagamento;
                AtualizarStatusLinha(row);

                // Atualiza totais no outro form
                var formContas = Application.OpenForms.OfType<FinanceiroFormContasSelecionar>().FirstOrDefault();
                if (formContas != null)
                    formContas.AtualizarTotais();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao marcar como pago: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvContasPagar_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                var row = dgvContasPagar.Rows[e.RowIndex];
                int id = Convert.ToInt32(row.Cells["Id"].Value);

                string query = "";
                object valorParametro = null;
                bool executarQuery = false;

                string coluna = dgvContasPagar.Columns[e.ColumnIndex].Name;

                switch (coluna)
                {
                    case "CategoriaId":
                        query = "UPDATE ContasPagar SET CategoriaId = @Valor WHERE Id = @Id";
                        valorParametro = row.Cells["CategoriaId"].Value;
                        executarQuery = true;
                        break;

                    case "DataVencimento":
                        query = "UPDATE ContasPagar SET DataVencimento = @Valor WHERE Id = @Id";
                        if (DateTime.TryParse(row.Cells["DataVencimento"].Value?.ToString(), out DateTime dataVenc))
                            valorParametro = dataVenc;
                        else
                        {
                            row.Cells["DataVencimento"].Value = DateTime.Today;
                            valorParametro = DateTime.Today;
                        }
                        executarQuery = true;
                        break;

                    case "DataPagamento":
                        if (row.Cells["DataPagamento"].Value != null &&
                            row.Cells["DataPagamento"].Value != DBNull.Value &&
                            DateTime.TryParse(row.Cells["DataPagamento"].Value.ToString(), out DateTime dataPag))
                        {
                            query = "UPDATE ContasPagar SET DataPagamento = @Valor, StatusPagamento = 1 WHERE Id = @Id";
                            valorParametro = dataPag;
                            row.Cells["StatusPagamento"].Value = 1;
                        }
                        else
                        {
                            query = "UPDATE ContasPagar SET DataPagamento = NULL, StatusPagamento = 0 WHERE Id = @Id";
                            valorParametro = DBNull.Value;
                            row.Cells["StatusPagamento"].Value = 0;
                        }
                        executarQuery = true;
                        break;

                    case "Valor":
                        if (decimal.TryParse(row.Cells["Valor"].Value?.ToString(), out decimal valor))
                        {
                            query = "UPDATE ContasPagar SET Valor = @Valor WHERE Id = @Id";
                            valorParametro = valor;
                            executarQuery = true;
                        }
                        break;

                    case "Observacao":
                        query = "UPDATE ContasPagar SET Observacao = @Valor WHERE Id = @Id";
                        valorParametro = row.Cells["Observacao"].Value?.ToString() ?? "";
                        executarQuery = true;
                        break;
                }

                if (!string.IsNullOrEmpty(query) && executarQuery) // CORREÇÃO: usar flag em vez de verificar null
                {
                    using (SqlConnection conexao = new SqlConnection(connectionString))
                    {
                        conexao.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conexao))
                        {
                            cmd.Parameters.AddWithValue("@Valor", valorParametro ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                AtualizarStatusLinha(row);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvContasPagar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            try
            {
                string columnName = dgvContasPagar.Columns[e.ColumnIndex].Name;

                if (columnName == "DataVencimento" || columnName == "DataPagamento")
                {
                    using (FormSelecionarData formData = new FormSelecionarData())
                    {
                        DateTime dataAtual = DateTime.Today;

                        if (dgvContasPagar.Rows[e.RowIndex].Cells[columnName].Value != null &&
                            dgvContasPagar.Rows[e.RowIndex].Cells[columnName].Value != DBNull.Value)
                        {
                            DateTime.TryParse(dgvContasPagar.Rows[e.RowIndex].Cells[columnName].Value.ToString(), out dataAtual);
                        }

                        formData.DataSelecionada = dataAtual;

                        if (formData.ShowDialog() == DialogResult.OK)
                        {
                            dgvContasPagar.Rows[e.RowIndex].Cells[columnName].Value = formData.DataSelecionada;
                            DgvContasPagar_CellValueChanged(sender, e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao selecionar data: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void DgvContasPagar_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvContasPagar.Rows)
            {
                if (!row.IsNewRow)
                {
                    AtualizarStatusLinha(row);
                }
            }
        }
    }
}