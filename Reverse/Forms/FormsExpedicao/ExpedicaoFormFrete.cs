using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormFrete : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private int? freteAtualId = null;
        private bool isLoadingData = false;
        private int categoriaFreteId = 0;
        private BindingSource _bindingSourceFretes;

        public ExpedicaoFormFrete(int _usuarioId)
        {
            InitializeComponent();
            ConfigurarFormulario();

            btnExportarExcel.Click += btnExportarExcel_Click;
        }

        private void ConfigurarFormulario()
        {
            _bindingSourceFretes = new BindingSource();
            dgvFretes.DataSource = _bindingSourceFretes;

            dgvFretes.FilterStringChanged += (s, ev) =>
            {
                _bindingSourceFretes.Filter = dgvFretes.FilterString;
            };
            dgvFretes.SortStringChanged += (s, ev) =>
            {
                _bindingSourceFretes.Sort = dgvFretes.SortString;
            };

            ConfigurarGridFretes();

            ConfigurarGridValorPendente();

            lblTotal30.Text = "R$ 0,00";
            lblPendente.Text = "R$ 0,00";

            btnNovaLinha.Click += BtnNovaLinha_Click;
            btnExcluirLinha.Click += BtnExcluirLinha_Click;
            btnFinalizado.Click += BtnFinalizado_Click;
            btnSalvar.Click += BtnSalvar_Click;

            dgvFretes.SelectionChanged += DgvFretes_SelectionChanged;

            rbCIF.CheckedChanged += RbTipoFrete_CheckedChanged;
            rbFOB.CheckedChanged += RbTipoFrete_CheckedChanged;

            txtStatus.ReadOnly = true;
            dtpBaixa.Enabled = false;
            btnFinalizado.Enabled = false;

            CarregarTransportadoras();
            ObterCategoriaFreteId();
            CarregarFretes();
        }

        #region Configuração das Grids

        private void AplicarEstiloVisualProducao(DataGridView grid)
        {
            grid.BackgroundColor = Color.FromArgb(250, 250, 252);
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(230, 230, 235);

            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            grid.RowsDefaultCellStyle.BackColor = Color.White;

            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = grid.ColumnHeadersDefaultCellStyle.BackColor;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            grid.DefaultCellStyle.ForeColor = Color.Black;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.RowTemplate.Height = 36;
        }
        private void ConfigurarGridFretes()
        {
            dgvFretes.MultiSelect = false;
            dgvFretes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFretes.ReadOnly = true;
            dgvFretes.AllowUserToAddRows = false;
            dgvFretes.AllowUserToDeleteRows = false;
            dgvFretes.RowHeadersVisible = false;
            dgvFretes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            AplicarEstiloVisualProducao(dgvFretes);
        }

        private void ConfigurarGridValorPendente()
        {
            dgvValorPendente.MultiSelect = false;
            dgvValorPendente.ReadOnly = true;
            dgvValorPendente.AllowUserToAddRows = false;
            dgvValorPendente.RowHeadersVisible = false;
            dgvValorPendente.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            AplicarEstiloVisualProducao(dgvValorPendente);
        }

        #endregion

        #region Carregar Dados

        private void CarregarTransportadoras()
        {
            try
            {
                DataTable dt = ExpedicaoFormMotorista.ObterMotoristas("Transportadora");

                cmbTransportadora.DataSource = dt;
                cmbTransportadora.ValueMember = "MotoristaId";
                cmbTransportadora.DisplayMember = "NomeInterno";
                cmbTransportadora.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar transportadoras: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ObterCategoriaFreteId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id FROM Categorias WHERE Nome = 'Frete'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            categoriaFreteId = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao obter categoria Frete: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarFretes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            f.FreteId,
                            f.Gerador,
                            m.NomeInterno AS Transportadora,
                            f.TransportadoraId,
                            f.ValorFrete,
                            f.Origem,
                            f.Destino,
                            f.DestinatarioFinal,
                            f.DataOcorrencia,
                            f.DataVencimento,
                            f.DataBaixa,
                            f.TipoFrete,
                            f.Status
                        FROM Fretes f
                        INNER JOIN Motoristas m ON f.TransportadoraId = m.MotoristaId
                        ORDER BY f.DataOcorrencia DESC, f.FreteId DESC";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        _bindingSourceFretes.DataSource = dt;
                    }

                    ConfigurarColunasGridFretes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar fretes: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColunasGridFretes()
        {
            if (dgvFretes.Columns.Contains("FreteId"))
                dgvFretes.Columns["FreteId"].Visible = false;

            if (dgvFretes.Columns.Contains("TransportadoraId"))
                dgvFretes.Columns["TransportadoraId"].Visible = false;

            if (dgvFretes.Columns.Contains("Gerador"))
            {
                dgvFretes.Columns["Gerador"].HeaderText = "Gerador";
                dgvFretes.Columns["Gerador"].FillWeight = 80;
            }

            if (dgvFretes.Columns.Contains("Transportadora"))
            {
                dgvFretes.Columns["Transportadora"].HeaderText = "Transportadora";
                dgvFretes.Columns["Transportadora"].FillWeight = 80;
            }

            if (dgvFretes.Columns.Contains("ValorFrete"))
            {
                dgvFretes.Columns["ValorFrete"].HeaderText = "Valor Frete";
                dgvFretes.Columns["ValorFrete"].DefaultCellStyle.Format = "C2";
                dgvFretes.Columns["ValorFrete"].FillWeight = 70;
            }

            if (dgvFretes.Columns.Contains("DataOcorrencia"))
            {
                dgvFretes.Columns["DataOcorrencia"].HeaderText = "Ocorrência";
                dgvFretes.Columns["DataOcorrencia"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvFretes.Columns["DataOcorrencia"].FillWeight = 60;
            }

            if (dgvFretes.Columns.Contains("DataVencimento"))
            {
                dgvFretes.Columns["DataVencimento"].HeaderText = "Vencimento";
                dgvFretes.Columns["DataVencimento"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvFretes.Columns["DataVencimento"].FillWeight = 60;
            }

            if (dgvFretes.Columns.Contains("DataBaixa"))
            {
                dgvFretes.Columns["DataBaixa"].HeaderText = "Data Baixa";
                dgvFretes.Columns["DataBaixa"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvFretes.Columns["DataBaixa"].FillWeight = 60;
            }

            if (dgvFretes.Columns.Contains("TipoFrete"))
            {
                dgvFretes.Columns["TipoFrete"].HeaderText = "Tipo";
                dgvFretes.Columns["TipoFrete"].FillWeight = 40;
            }

            if (dgvFretes.Columns.Contains("Status"))
            {
                dgvFretes.Columns["Status"].HeaderText = "Status";
                dgvFretes.Columns["Status"].FillWeight = 70;
            }

            if (dgvFretes.Columns.Contains("Origem"))
                dgvFretes.Columns["Origem"].Visible = false;
            if (dgvFretes.Columns.Contains("Destino"))
                dgvFretes.Columns["Destino"].Visible = false;
            if (dgvFretes.Columns.Contains("DestinatarioFinal"))
                dgvFretes.Columns["DestinatarioFinal"].Visible = false;
        }

        private void DgvFretes_SelectionChanged(object sender, EventArgs e)
        {
            if (isLoadingData) return;

            if (dgvFretes.CurrentRow == null) return;

            DataGridViewRow row = dgvFretes.CurrentRow;

            if (row.Cells["FreteId"].Value == null || row.Cells["FreteId"].Value == DBNull.Value)
                return;

            CarregarDadosLinha(row);
            AtualizarGridsResumo();
        }

        private void CarregarDadosLinha(DataGridViewRow row)
        {
            isLoadingData = true;

            try
            {
                freteAtualId = Convert.ToInt32(row.Cells["FreteId"].Value);

                txtGerador.Text = row.Cells["Gerador"].Value?.ToString() ?? "";

                if (row.Cells["TransportadoraId"].Value != null && row.Cells["TransportadoraId"].Value != DBNull.Value)
                    cmbTransportadora.SelectedValue = Convert.ToInt32(row.Cells["TransportadoraId"].Value);
                else
                    cmbTransportadora.SelectedIndex = -1;

                txtValorFrete.Text = row.Cells["ValorFrete"].Value != null ?
                    Convert.ToDecimal(row.Cells["ValorFrete"].Value).ToString("F2") : "0.00";

                txtOrigem.Text = row.Cells["Origem"].Value?.ToString() ?? "";
                txtDestino.Text = row.Cells["Destino"].Value?.ToString() ?? "";
                txtDestinatarioFinal.Text = row.Cells["DestinatarioFinal"].Value?.ToString() ?? "";

                if (row.Cells["DataOcorrencia"].Value != null && row.Cells["DataOcorrencia"].Value != DBNull.Value)
                    dtpOcorrencia.Value = Convert.ToDateTime(row.Cells["DataOcorrencia"].Value);

                if (row.Cells["DataVencimento"].Value != null && row.Cells["DataVencimento"].Value != DBNull.Value)
                    dtpVencimento.Value = Convert.ToDateTime(row.Cells["DataVencimento"].Value);

                if (row.Cells["DataBaixa"].Value != null && row.Cells["DataBaixa"].Value != DBNull.Value)
                {
                    dtpBaixa.Value = Convert.ToDateTime(row.Cells["DataBaixa"].Value);
                    dtpBaixa.Enabled = true;
                }
                else
                {
                    dtpBaixa.Value = DateTime.Today;
                    dtpBaixa.Enabled = false;
                }

                string tipoFrete = row.Cells["TipoFrete"].Value?.ToString() ?? "FOB";
                rbCIF.Checked = tipoFrete == "CIF";
                rbFOB.Checked = tipoFrete == "FOB";

                txtStatus.Text = row.Cells["Status"].Value?.ToString() ?? "Em Aberto";

                AtualizarBotaoFinalizado();
            }
            finally
            {
                isLoadingData = false;
            }
        }

        private void AtualizarGridsResumo()
        {
            if (cmbTransportadora.SelectedValue == null) return;

            int transportadoraId = Convert.ToInt32(cmbTransportadora.SelectedValue);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string queryTotal = @"
                        SELECT 
                            ISNULL(SUM(CASE 
                                WHEN DataOcorrencia >= DATEADD(DAY, -30, GETDATE()) 
                                THEN ValorFrete 
                                ELSE 0 
                            END), 0) AS Total30Dias,
                            ISNULL(SUM(CASE 
                                WHEN Status = 'Em Aberto' 
                                THEN ValorFrete 
                                ELSE 0 
                            END), 0) AS TotalPendente
                        FROM Fretes
                        WHERE TransportadoraId = @TransportadoraId";

                    using (SqlCommand cmd = new SqlCommand(queryTotal, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransportadoraId", transportadoraId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblTotal30.Text = ((decimal)reader["Total30Dias"]).ToString("C2");
                                lblPendente.Text = ((decimal)reader["TotalPendente"]).ToString("C2");
                            }
                            else
                            {
                                lblTotal30.Text = "R$ 0,00";
                                lblPendente.Text = "R$ 0,00";
                            }
                        }
                    }

                    string queryPendente = @"
                    SELECT 
                        FreteId,
                        Gerador,
                        ValorFrete,
                        DataOcorrencia,
                        DataVencimento
                    FROM Fretes
                    WHERE TransportadoraId = @TransportadoraId 
                    AND Status = 'Em Aberto'
                    ORDER BY DataOcorrencia ASC";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(queryPendente, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@TransportadoraId", transportadoraId);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvValorPendente.DataSource = dt;
                    }

                    ConfigurarColunasGridPendente();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar resumos: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColunasGridPendente()
        {
            if (dgvValorPendente.Columns.Contains("FreteId"))
                dgvValorPendente.Columns["FreteId"].Visible = false;

            if (dgvValorPendente.Columns.Contains("Gerador"))
            {
                dgvValorPendente.Columns["Gerador"].HeaderText = "Gerador";
                dgvValorPendente.Columns["Gerador"].FillWeight = 80;
            }

            if (dgvValorPendente.Columns.Contains("ValorFrete"))
            {
                dgvValorPendente.Columns["ValorFrete"].HeaderText = "Valor";
                dgvValorPendente.Columns["ValorFrete"].DefaultCellStyle.Format = "C2";
                dgvValorPendente.Columns["ValorFrete"].FillWeight = 60;
            }

            if (dgvValorPendente.Columns.Contains("DataOcorrencia"))
            {
                dgvValorPendente.Columns["DataOcorrencia"].HeaderText = "Ocorrência";
                dgvValorPendente.Columns["DataOcorrencia"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvValorPendente.Columns["DataOcorrencia"].FillWeight = 60;
            }

            if (dgvValorPendente.Columns.Contains("DataVencimento"))
            {
                dgvValorPendente.Columns["DataVencimento"].HeaderText = "Vencimento";
                dgvValorPendente.Columns["DataVencimento"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvValorPendente.Columns["DataVencimento"].FillWeight = 60;
            }
        }

        #endregion

        #region Botões

        private void BtnNovaLinha_Click(object sender, EventArgs e)
        {
            LimparCampos();
            freteAtualId = null;
            txtGerador.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand cmd;

                            if (freteAtualId.HasValue)
                            {
                                int? contaPagarId = null;
                                string tipoFreteAnterior = "";
                                decimal valorFreteAnterior = 0;

                                string queryAnterior = "SELECT ContaPagarId, TipoFrete, ValorFrete FROM Fretes WHERE FreteId = @FreteId";
                                using (SqlCommand cmdAnterior = new SqlCommand(queryAnterior, conn, transaction))
                                {
                                    cmdAnterior.Parameters.AddWithValue("@FreteId", freteAtualId.Value);
                                    using (SqlDataReader reader = cmdAnterior.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            if (reader["ContaPagarId"] != DBNull.Value)
                                                contaPagarId = Convert.ToInt32(reader["ContaPagarId"]);
                                            tipoFreteAnterior = reader["TipoFrete"].ToString();
                                            valorFreteAnterior = Convert.ToDecimal(reader["ValorFrete"]);
                                        }
                                    }
                                }

                                cmd = new SqlCommand(@"
                                UPDATE Fretes SET
                                    Gerador = @Gerador,
                                    TransportadoraId = @TransportadoraId,
                                    ValorFrete = @ValorFrete,
                                    Origem = @Origem,
                                    Destino = @Destino,
                                    DestinatarioFinal = @DestinatarioFinal,
                                    DataOcorrencia = @DataOcorrencia,
                                    DataVencimento = @DataVencimento,
                                    TipoFrete = @TipoFrete,
                                    DataAlteracao = GETDATE()
                                WHERE FreteId = @FreteId", conn, transaction);

                                cmd.Parameters.AddWithValue("@FreteId", freteAtualId.Value);
                                PreencherParametros(cmd);
                                cmd.ExecuteNonQuery();

                                if (contaPagarId.HasValue && tipoFreteAnterior == "CIF" && rbCIF.Checked)
                                {
                                    decimal valorFreteNovo = decimal.Parse(txtValorFrete.Text);

                                    if (valorFreteNovo != valorFreteAnterior)
                                    {
                                        string updateConta = "UPDATE ContasPagar SET ValorReal = @ValorReal, ValorPago = @ValorPago WHERE Id = @Id";
                                        using (SqlCommand cmdUpdateConta = new SqlCommand(updateConta, conn, transaction))
                                        {
                                            cmdUpdateConta.Parameters.AddWithValue("@ValorReal", valorFreteNovo);
                                            cmdUpdateConta.Parameters.AddWithValue("@ValorPago", valorFreteNovo);
                                            cmdUpdateConta.Parameters.AddWithValue("@Id", contaPagarId.Value);
                                            cmdUpdateConta.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                cmd = new SqlCommand(@"
                            INSERT INTO Fretes 
                            (Gerador, TransportadoraId, ValorFrete, Origem, Destino, 
                             DestinatarioFinal, DataOcorrencia, DataVencimento, TipoFrete, 
                             Status, DataCadastro)
                            VALUES 
                            (@Gerador, @TransportadoraId, @ValorFrete, @Origem, @Destino, 
                             @DestinatarioFinal, @DataOcorrencia, @DataVencimento, @TipoFrete, 
                             'Em Aberto', GETDATE());
                            SELECT SCOPE_IDENTITY();", conn, transaction);

                                PreencherParametros(cmd);
                                int novoFreteId = Convert.ToInt32(cmd.ExecuteScalar());

                                if (rbCIF.Checked)
                                {
                                    CriarContaFinanceiro(conn, transaction, novoFreteId);
                                }
                            }

                            transaction.Commit();

                            MessageBox.Show("Frete salvo com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            AtualizarFormsFinanceiroAbertos();

                            CarregarFretes();
                            LimparCampos();
                            freteAtualId = null;
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
                MessageBox.Show($"Erro ao salvar frete: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarFormsFinanceiroAbertos()
        {
            try
            {
                var formPagar = Application.OpenForms.OfType<Reverse.Forms.FormsFinanceiro.FormPagar>().FirstOrDefault();
                if (formPagar != null)
                {
                    var metodo = formPagar.GetType().GetMethod("RecarregarLoteAtual",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    metodo?.Invoke(formPagar, null);
                }

                var formContas = Application.OpenForms.OfType<Reverse.Forms.FormsFinanceiro.FinanceiroFormContasSelecionar>().FirstOrDefault();
                if (formContas != null)
                {
                    formContas.AtualizarTotais();
                }
            }
            catch
            {

            }
        }

        private bool ValidarCampos()
        {
            if (cmbTransportadora.SelectedIndex == -1)
            {
                MessageBox.Show("Selecione uma transportadora!", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTransportadora.Focus();
                return false;
            }

            if (!decimal.TryParse(txtValorFrete.Text, out decimal valor) || valor <= 0)
            {
                MessageBox.Show("Informe um valor válido para o frete!", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValorFrete.Focus();
                return false;
            }

            if (!rbCIF.Checked && !rbFOB.Checked)
            {
                MessageBox.Show("Selecione o tipo de frete (CIF ou FOB)!", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void PreencherParametros(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Gerador",
                string.IsNullOrWhiteSpace(txtGerador.Text) ? DBNull.Value : (object)txtGerador.Text.Trim());
            cmd.Parameters.AddWithValue("@TransportadoraId", cmbTransportadora.SelectedValue);
            cmd.Parameters.AddWithValue("@ValorFrete", decimal.Parse(txtValorFrete.Text));
            cmd.Parameters.AddWithValue("@Origem",
                string.IsNullOrWhiteSpace(txtOrigem.Text) ? DBNull.Value : (object)txtOrigem.Text.Trim());
            cmd.Parameters.AddWithValue("@Destino",
                string.IsNullOrWhiteSpace(txtDestino.Text) ? DBNull.Value : (object)txtDestino.Text.Trim());
            cmd.Parameters.AddWithValue("@DestinatarioFinal",
                string.IsNullOrWhiteSpace(txtDestinatarioFinal.Text) ? DBNull.Value : (object)txtDestinatarioFinal.Text.Trim());
            cmd.Parameters.AddWithValue("@DataOcorrencia", dtpOcorrencia.Value.Date);
            cmd.Parameters.AddWithValue("@DataVencimento", dtpVencimento.Value.Date);
            cmd.Parameters.AddWithValue("@TipoFrete", rbCIF.Checked ? "CIF" : "FOB");
        }

        private void CriarContaFinanceiro(SqlConnection conn, SqlTransaction transaction, int freteId)
        {
            DateTime dataOcorrencia = dtpOcorrencia.Value.Date;
            int loteId;

            string queryLote = "SELECT LoteId FROM LotesContasPagar WHERE DataLote = @DataLote";
            using (SqlCommand cmdLote = new SqlCommand(queryLote, conn, transaction))
            {
                cmdLote.Parameters.AddWithValue("@DataLote", dataOcorrencia);
                var result = cmdLote.ExecuteScalar();

                if (result == null)
                {
                    string insertLote = "INSERT INTO LotesContasPagar (DataLote) VALUES (@DataLote); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand cmdInsertLote = new SqlCommand(insertLote, conn, transaction))
                    {
                        cmdInsertLote.Parameters.AddWithValue("@DataLote", dataOcorrencia);
                        loteId = Convert.ToInt32(cmdInsertLote.ExecuteScalar());
                    }
                }
                else
                {
                    loteId = Convert.ToInt32(result);
                }
            }

            string nomeTransportadora = cmbTransportadora.Text;
            decimal valorFrete = decimal.Parse(txtValorFrete.Text);

            string insertConta = @"
                INSERT INTO ContasPagar 
                (CategoriaId, Observacao, ValorReal, ValorPago, DataVencimento, StatusPagamento, 
                 LoteId, FreteId, DataCadastro)
                VALUES 
                (@CategoriaId, @Observacao, @ValorReal, @ValorPago, @DataVencimento, 0, 
                 @LoteId, @FreteId, GETDATE());
                SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmdConta = new SqlCommand(insertConta, conn, transaction))
            {
                cmdConta.Parameters.AddWithValue("@CategoriaId", categoriaFreteId);
                cmdConta.Parameters.AddWithValue("@Observacao", $"Frete - {nomeTransportadora}");
                cmdConta.Parameters.AddWithValue("@ValorReal", valorFrete);
                cmdConta.Parameters.AddWithValue("@ValorPago", valorFrete);
                cmdConta.Parameters.AddWithValue("@DataVencimento", dtpVencimento.Value.Date);
                cmdConta.Parameters.AddWithValue("@LoteId", loteId);
                cmdConta.Parameters.AddWithValue("@FreteId", freteId);

                int contaPagarId = Convert.ToInt32(cmdConta.ExecuteScalar());

                string updateFrete = "UPDATE Fretes SET ContaPagarId = @ContaPagarId WHERE FreteId = @FreteId";
                using (SqlCommand cmdUpdate = new SqlCommand(updateFrete, conn, transaction))
                {
                    cmdUpdate.Parameters.AddWithValue("@ContaPagarId", contaPagarId);
                    cmdUpdate.Parameters.AddWithValue("@FreteId", freteId);
                    cmdUpdate.ExecuteNonQuery();
                }
            }
        }

        private void BtnExcluirLinha_Click(object sender, EventArgs e)
        {
            if (dgvFretes.CurrentRow == null)
            {
                MessageBox.Show("Selecione uma linha para excluir!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Deseja realmente excluir esta linha?\n\nEsta ação não poderá ser desfeita!",
                "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                int freteId = Convert.ToInt32(dgvFretes.CurrentRow.Cells["FreteId"].Value);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string queryContaId = "SELECT ContaPagarId FROM Fretes WHERE FreteId = @FreteId";
                            using (SqlCommand cmd = new SqlCommand(queryContaId, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@FreteId", freteId);
                                var contaPagarId = cmd.ExecuteScalar();

                                if (contaPagarId != null && contaPagarId != DBNull.Value)
                                {
                                    string deleteConta = "DELETE FROM ContasPagar WHERE Id = @Id";
                                    using (SqlCommand cmdDelete = new SqlCommand(deleteConta, conn, transaction))
                                    {
                                        cmdDelete.Parameters.AddWithValue("@Id", contaPagarId);
                                        cmdDelete.ExecuteNonQuery();
                                    }
                                }
                            }

                            string deleteFrete = "DELETE FROM Fretes WHERE FreteId = @FreteId";
                            using (SqlCommand cmd = new SqlCommand(deleteFrete, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@FreteId", freteId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            MessageBox.Show("Linha excluída com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            CarregarFretes();
                            LimparCampos();
                            freteAtualId = null;
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
                MessageBox.Show($"Erro ao excluir linha: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFinalizado_Click(object sender, EventArgs e)
        {
            if (!freteAtualId.HasValue)
            {
                MessageBox.Show("Selecione uma linha para finalizar!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (rbCIF.Checked)
            {
                MessageBox.Show("Fretes CIF só podem ser finalizados pelo financeiro!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtStatus.Text == "Finalizado")
            {
                MessageBox.Show("Esta linha já está finalizada!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Deseja realmente finalizar esta linha?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE Fretes SET
                            Status = 'Finalizado',
                            DataBaixa = @DataBaixa,
                            DataAlteracao = GETDATE()
                        WHERE FreteId = @FreteId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DataBaixa", DateTime.Today);
                        cmd.Parameters.AddWithValue("@FreteId", freteAtualId.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Linha finalizada com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CarregarFretes();

                if (dgvFretes.CurrentRow != null)
                {
                    CarregarDadosLinha(dgvFretes.CurrentRow);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao finalizar linha: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Métodos Auxiliares

        private void RbTipoFrete_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingData) return;
            AtualizarBotaoFinalizado();
        }

        private void AtualizarBotaoFinalizado()
        {
            btnFinalizado.Enabled = freteAtualId.HasValue &&
                                   rbFOB.Checked &&
                                   txtStatus.Text == "Em Aberto";
        }

        private void LimparCampos()
        {
            txtGerador.Clear();
            cmbTransportadora.SelectedIndex = -1;
            txtValorFrete.Text = "0.00";
            txtOrigem.Clear();
            txtDestino.Clear();
            txtDestinatarioFinal.Clear();
            dtpOcorrencia.Value = DateTime.Today;
            dtpVencimento.Value = DateTime.Today;
            dtpBaixa.Value = DateTime.Today;
            dtpBaixa.Enabled = false;
            rbFOB.Checked = true;
            txtStatus.Text = "Em Aberto";
            freteAtualId = null;
            btnFinalizado.Enabled = false;

            lblTotal30.Text = "R$ 0,00";
            lblPendente.Text = "R$ 0,00";

            dgvValorPendente.DataSource = null;
        }

        #endregion

        #region Método Público para Atualização do Financeiro

        public static void AtualizarStatusFrete(int freteId, DateTime dataPagamento)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE Fretes SET
                            Status = 'Finalizado',
                            DataBaixa = @DataBaixa,
                            DataAlteracao = GETDATE()
                        WHERE FreteId = @FreteId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DataBaixa", dataPagamento);
                        cmd.Parameters.AddWithValue("@FreteId", freteId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar status do frete: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string ObterFretePendentesInfo()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    CONVERT(DATE, DataOcorrencia) AS Data,
                    COUNT(*) AS Quantidade
                FROM Fretes
                WHERE Status = 'Em Aberto' 
                AND TipoFrete = 'CIF'
                AND ContaPagarId IS NOT NULL
                GROUP BY CONVERT(DATE, DataOcorrencia)
                ORDER BY Data";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var listaFretes = new System.Collections.Generic.List<string>();
                            int totalFretes = 0;

                            while (reader.Read())
                            {
                                DateTime data = reader.GetDateTime(0);
                                int quantidade = reader.GetInt32(1);
                                totalFretes += quantidade;

                                string plural = quantidade > 1 ? "fretes" : "frete";
                                listaFretes.Add($"  • {data:dd/MM/yyyy} - {quantidade} {plural}");
                            }

                            if (listaFretes.Count > 0)
                            {
                                string pluralTotal = totalFretes > 1 ? "fretes" : "frete";
                                string mensagem = $"Existem {totalFretes} {pluralTotal} CIF em aberto:\n\n";
                                mensagem += string.Join("\n", listaFretes);
                                return mensagem;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao verificar fretes pendentes: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }
        #endregion

        #region Exportação Excel

        private async void btnExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bindingSourceFretes == null || _bindingSourceFretes.Count == 0)
                {
                    MessageBox.Show("Não há dados para exportar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var resultado = MessageBox.Show(
                    "Deseja exportar apenas os dados FILTRADOS?\n\n" +
                    "SIM = Exportar apenas dados filtrados/visíveis\n" +
                    "NÃO = Exportar TODOS os fretes",
                    "Opção de Exportação",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Cancel)
                    return;

                bool exportarApenasFiltrados = (resultado == DialogResult.Yes);

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Arquivo Excel (*.xlsx)|*.xlsx";
                    sfd.Title = "Salvar Exportação de Fretes";
                    sfd.FileName = $"Fretes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    btnExportarExcel.Enabled = false;
                    btnExportarExcel.Text = "Exportando...";
                    Cursor = Cursors.WaitCursor;

                    try
                    {
                        await Task.Run(() => ExportarFretesParaExcel(sfd.FileName, exportarApenasFiltrados));

                        MessageBox.Show(
                            $"Dados exportados com sucesso!\n\nArquivo: {Path.GetFileName(sfd.FileName)}",
                            "Exportação Concluída",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        var abrirArquivo = MessageBox.Show(
                            "Deseja abrir o arquivo agora?",
                            "Abrir Arquivo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (abrirArquivo == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao exportar: {ex.Message}", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btnExportarExcel.Enabled = true;
                        btnExportarExcel.Text = "Excel";
                        Cursor = Cursors.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportarFretesParaExcel(string caminhoArquivo, bool apenasFiltrados)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Fretes");

                DataTable dadosExportar = ObterDadosFretesParaExportacao(apenasFiltrados);

                if (dadosExportar.Rows.Count == 0)
                {
                    throw new InvalidOperationException("Nenhum dado disponível para exportação.");
                }

                int colIndex = 1;
                var colunasVisiveis = ObterColunasVisiveisFretes();

                foreach (var coluna in colunasVisiveis)
                {
                    var cell = worksheet.Cell(1, colIndex);
                    cell.Value = coluna.HeaderText;
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(52, 73, 94);
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    colIndex++;
                }

                int rowIndex = 2;
                foreach (DataRow row in dadosExportar.Rows)
                {
                    colIndex = 1;
                    foreach (var coluna in colunasVisiveis)
                    {
                        var cell = worksheet.Cell(rowIndex, colIndex);
                        var valor = row[coluna.Name];

                        if (valor != null && valor != DBNull.Value)
                        {
                            if (coluna.Name == "ValorFrete")
                            {
                                cell.Value = Convert.ToDecimal(valor);
                                cell.Style.NumberFormat.Format = "R$ #,##0.00";
                                cell.Style.Font.Bold = true;
                            }
                            else if (coluna.Name == "DataOcorrencia" || coluna.Name == "DataVencimento" || coluna.Name == "DataBaixa")
                            {
                                if (DateTime.TryParse(valor.ToString(), out DateTime data))
                                {
                                    cell.Value = data;
                                    cell.Style.NumberFormat.Format = "dd/MM/yyyy";
                                }
                                else
                                {
                                    cell.Value = valor.ToString();
                                }
                            }
                            else
                            {
                                cell.Value = valor.ToString();
                            }
                        }

                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.OutsideBorderColor = XLColor.Gray;

                        if (rowIndex % 2 == 0)
                        {
                            cell.Style.Fill.BackgroundColor = XLColor.FromArgb(245, 249, 255);
                        }

                        colIndex++;
                    }
                    rowIndex++;
                }

                worksheet.Columns().AdjustToContents(5, 50);

                var range = worksheet.Range(1, 1, rowIndex - 1, colunasVisiveis.Count);
                range.SetAutoFilter();

                worksheet.SheetView.FreezeRows(1);

                int footerRow = rowIndex + 2;
                worksheet.Cell(footerRow, 1).Value = $"Exportado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cell(footerRow, 1).Style.Font.Italic = true;
                worksheet.Cell(footerRow, 1).Style.Font.FontColor = XLColor.Gray;

                if (apenasFiltrados && !string.IsNullOrEmpty(dgvFretes.FilterString))
                {
                    worksheet.Cell(footerRow + 1, 1).Value = "Filtros aplicados: Sim";
                    worksheet.Cell(footerRow + 1, 1).Style.Font.Italic = true;
                    worksheet.Cell(footerRow + 1, 1).Style.Font.FontColor = XLColor.Gray;
                }

                worksheet.Cell(footerRow + 3, 1).Value = $"Total de registros: {dadosExportar.Rows.Count}";
                worksheet.Cell(footerRow + 3, 1).Style.Font.Bold = true;

                if (dadosExportar.Columns.Contains("ValorFrete"))
                {
                    decimal totalValores = 0;
                    foreach (DataRow row in dadosExportar.Rows)
                    {
                        if (row["ValorFrete"] != DBNull.Value)
                        {
                            totalValores += Convert.ToDecimal(row["ValorFrete"]);
                        }
                    }

                    worksheet.Cell(footerRow + 4, 1).Value = $"Total Valor Fretes: R$ {totalValores:N2}";
                    worksheet.Cell(footerRow + 4, 1).Style.Font.Bold = true;
                    worksheet.Cell(footerRow + 4, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 237, 255);
                }

                workbook.SaveAs(caminhoArquivo);
            }
        }
        private DataTable ObterDadosFretesParaExportacao(bool apenasFiltrados)
        {
            if (!apenasFiltrados)
            {
                if (_bindingSourceFretes.DataSource is DataTable dt)
                {
                    return dt.Copy();
                }
            }

            DataTable resultado = (_bindingSourceFretes.DataSource as DataTable)?.Clone();

            if (resultado == null)
                return new DataTable();

            foreach (DataRowView rowView in _bindingSourceFretes)
            {
                resultado.ImportRow(rowView.Row);
            }

            return resultado;
        }

        private List<DataGridViewColumn> ObterColunasVisiveisFretes()
        {
            var colunasVisiveis = new List<DataGridViewColumn>();

            foreach (DataGridViewColumn col in dgvFretes.Columns)
            {
                if (col.Visible && col.Name != "FreteId" && col.Name != "TransportadoraId")
                {
                    colunasVisiveis.Add(col);
                }
            }

            return colunasVisiveis.OrderBy(c => c.DisplayIndex).ToList();
        }

        #endregion
    }
}