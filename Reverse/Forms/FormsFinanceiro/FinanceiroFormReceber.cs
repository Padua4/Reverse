using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reverse.Forms.FormsFinanceiro
{
    public partial class FormReceber : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private DataTable clientesDt;
        private int? loteAtualId;
        private DateTimePicker _dtp;

        public FormReceber(int _usuarioId)
        {
            InitializeComponent();
            CarregarClientesParaGrid();
            ConfigurarGrid();
            CarregarFiltros();
            ConfigurarDateTimePicker();
            ConfigurarEventos();
            
            AtualizarTextoLabelsResumo();

        }

        private void ConfigurarGrid()
        {
            dgvContasReceber.AutoGenerateColumns = false;
            dgvContasReceber.EnableHeadersVisualStyles = false;
            dgvContasReceber.ReadOnly = false;
            dgvContasReceber.AllowUserToAddRows = false;
            dgvContasReceber.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvContasReceber.MultiSelect = false;

            // Cores e estilo
            dgvContasReceber.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvContasReceber.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvContasReceber.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvContasReceber.DefaultCellStyle.BackColor = Color.White;
            dgvContasReceber.DefaultCellStyle.ForeColor = Color.Black;
            dgvContasReceber.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgvContasReceber.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvContasReceber.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvContasReceber.Columns.Clear();

            // Coluna ID (oculta)
            var colId = new DataGridViewTextBoxColumn
            {
                Name = "ContaId",
                DataPropertyName = "ContaId",
                Visible = false
            };
            dgvContasReceber.Columns.Add(colId);

            // Cliente (ComboBox)
            var colCliente = new DataGridViewComboBoxColumn
            {
                Name = "ClienteId",
                HeaderText = "Cliente",
                DataPropertyName = "ClienteId",
                DisplayMember = "Nome",
                ValueMember = "ClienteId",
                FlatStyle = FlatStyle.Flat,
                Width = 200
            };
            dgvContasReceber.Columns.Add(colCliente);

            // Descrição
            var colDescricao = new DataGridViewTextBoxColumn
            {
                Name = "Descricao",
                HeaderText = "Descrição/Serviço",
                DataPropertyName = "Descricao",
                Width = 250
            };
            dgvContasReceber.Columns.Add(colDescricao);

            // Valor
            var colValor = new DataGridViewTextBoxColumn
            {
                Name = "Valor",
                HeaderText = "Valor",
                DataPropertyName = "Valor",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    FormatProvider = new System.Globalization.CultureInfo("pt-BR")
                },
                Width = 120
            };
            dgvContasReceber.Columns.Add(colValor);

            // Data Vencimento
            var colVencimento = new DataGridViewTextBoxColumn
            {
                Name = "DataVencimento",
                HeaderText = "Vencimento",
                DataPropertyName = "DataVencimento",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy",
                    FormatProvider = System.Globalization.CultureInfo.InvariantCulture
                },
                Width = 110
            };
            dgvContasReceber.Columns.Add(colVencimento);

            // Data Recebimento
            var colRecebimento = new DataGridViewTextBoxColumn
            {
                Name = "DataRecebimento",
                HeaderText = "Recebimento",
                DataPropertyName = "DataRecebimento",
                ValueType = typeof(DateTime?),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy",
                    FormatProvider = System.Globalization.CultureInfo.InvariantCulture,
                    NullValue = ""
                },
                Width = 110
            };
            dgvContasReceber.Columns.Add(colRecebimento);

            // Status (calculado)
            var colStatus = new DataGridViewTextBoxColumn
            {
                Name = "StatusRecebimento",
                HeaderText = "Status",
                DataPropertyName = "StatusRecebimento",
                ReadOnly = true,
                Width = 120
            };
            dgvContasReceber.Columns.Add(colStatus);

            // Observações
            var colObs = new DataGridViewTextBoxColumn
            {
                Name = "Observacoes",
                HeaderText = "Observações",
                DataPropertyName = "Observacoes",
                Width = 200
            };
            dgvContasReceber.Columns.Add(colObs);

            // Define quais colunas são editáveis
            foreach (DataGridViewColumn col in dgvContasReceber.Columns)
            {
                col.ReadOnly = !(col.Name == "ClienteId" ||
                               col.Name == "Descricao" ||
                               col.Name == "Valor" ||
                               col.Name == "DataVencimento" ||
                               col.Name == "DataRecebimento" ||
                               col.Name == "Observacoes");
            }
        }

        private void btnEditarConta_Click(object sender, EventArgs e)
        {
            if (dgvContasReceber.CurrentRow == null || dgvContasReceber.CurrentRow.Index < 0)
            {
                MessageBox.Show("Selecione uma conta para editar.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int colunaEditavel = 1;
                if (dgvContasReceber.CurrentRow.Cells[colunaEditavel].ReadOnly == false)
                {
                    dgvContasReceber.CurrentCell = dgvContasReceber.CurrentRow.Cells[colunaEditavel];
                    dgvContasReceber.BeginEdit(true);
                }
                else
                {
                    foreach (DataGridViewCell cell in dgvContasReceber.CurrentRow.Cells)
                    {
                        if (!cell.ReadOnly && cell.Visible)
                        {
                            dgvContasReceber.CurrentCell = cell;
                            dgvContasReceber.BeginEdit(true);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao editar conta: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ConfigurarDateTimePicker()
        {
            _dtp = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Visible = false,
                ShowCheckBox = true
            };

            _dtp.CloseUp += (s, e) =>
            {
                if (dgvContasReceber.CurrentCell != null)
                {
                    string colName = dgvContasReceber.Columns[dgvContasReceber.CurrentCell.ColumnIndex].Name;

                    if (colName == "DataRecebimento")
                    {
                        dgvContasReceber.CurrentCell.Value = _dtp.Checked ? (object)_dtp.Value : DBNull.Value;
                    }
                    else // DataVencimento sempre precisa de valor
                    {
                        dgvContasReceber.CurrentCell.Value = _dtp.Value;
                    }

                    dgvContasReceber.EndEdit();
                    _dtp.Visible = false;
                }
            };

            _dtp.ValueChanged += (s, e) =>
            {
                if (dgvContasReceber.CurrentCell != null && _dtp.Visible && _dtp.Checked)
                {
                    dgvContasReceber.CurrentCell.Value = _dtp.Value;
                }
            };

            dgvContasReceber.Controls.Add(_dtp);
        }

        private void ConfigurarEventos()
        {
            // Eventos do DataGridView
            dgvContasReceber.CellParsing += dgvContasReceber_CellParsing;
            dgvContasReceber.CellFormatting += dgvContasReceber_CellFormatting;
            dgvContasReceber.CellEndEdit += dgvContasReceber_CellEndEdit;
            dgvContasReceber.DataError += dgvContasReceber_DataError;
            dgvContasReceber.CellBeginEdit += dgvContasReceber_CellBeginEdit;
            dgvContasReceber.CellClick += dgvContasReceber_CellClick;
            dgvContasReceber.CellLeave += (s, e) => _dtp.Visible = false;
            dgvContasReceber.Scroll += (s, e) => _dtp.Visible = false;
            dgvContasReceber.DataBindingComplete += dgvContasReceber_DataBindingComplete;

            // Eventos dos filtros
            cmbCliente.SelectedIndexChanged += FiltroChanged;
            cmbStatus.SelectedIndexChanged += FiltroChanged;

            // Eventos dos botões
            btnFiltrar.Click += btnFiltrar_Click;
            btnLimparFiltro.Click += btnLimparFiltro_Click;
            btnNovoLote.Click += btnNovoLote_Click;
            btnSelecionarLote.Click += btnSelecionarLote_Click;
            btnNovaConta.Click += btnNovaConta_Click;
            btnMarcarRecebido.Click += btnMarcarRecebido_Click;
            btnExcluirConta.Click += btnExcluirConta_Click;
            btnRelatorio.Click += btnRelatorio_Click;
            btnEditarConta.Click += btnEditarConta_Click;
        }

        private void CarregarClientesParaGrid()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var da = new SqlDataAdapter(@"
                    SELECT ClienteId, Nome 
                    FROM Clientes 
                    WHERE Status = 1 
                    ORDER BY Nome", conn);
                clientesDt = new DataTable();
                da.Fill(clientesDt);
            }
        }

        private void CarregarFiltros()
        {
            // Combo Status
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Todos");
            cmbStatus.Items.Add("Pendente");
            cmbStatus.Items.Add("Recebido");
            cmbStatus.Items.Add("Atrasado");
            cmbStatus.Items.Add("Vencimento Próximo");
            cmbStatus.SelectedIndex = 0;

            // Combo Cliente
            cmbCliente.Items.Clear();
            cmbCliente.Items.Add("Todos os Clientes");

            foreach (DataRow row in clientesDt.Rows)
            {
                cmbCliente.Items.Add(row["Nome"].ToString());
            }
            cmbCliente.SelectedIndex = 0;

            // Atualiza o combo da grid
            var colCombo = dgvContasReceber.Columns["ClienteId"] as DataGridViewComboBoxColumn;
            if (colCombo != null)
            {
                colCombo.DataSource = clientesDt;
            }
        }

        private bool IsColunaData(int columnIndex)
        {
            if (columnIndex < 0) return false;
            var colName = dgvContasReceber.Columns[columnIndex].Name;
            return colName == "DataVencimento" || colName == "DataRecebimento";
        }

        private void ShowDatePickerForCell(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || columnIndex < 0) return;
            if (!IsColunaData(columnIndex)) { _dtp.Visible = false; return; }

            var rect = dgvContasReceber.GetCellDisplayRectangle(columnIndex, rowIndex, true);

            // Verificar se a célula está visível
            if (rect.Width == 0 || rect.Height == 0)
            {
                _dtp.Visible = false;
                return;
            }

            _dtp.Bounds = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

            var val = dgvContasReceber.Rows[rowIndex].Cells[columnIndex].Value;

            // Para DataRecebimento, permitir valores nulos
            if (dgvContasReceber.Columns[columnIndex].Name == "DataRecebimento")
            {
                _dtp.ShowCheckBox = true;
                if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()))
                {
                    _dtp.Checked = false;
                    _dtp.Value = DateTime.Today;
                }
                else
                {
                    _dtp.Checked = true;
                    _dtp.Value = Convert.ToDateTime(val);
                }
            }
            else // DataVencimento sempre requer valor
            {
                _dtp.ShowCheckBox = false;
                if (val != null && val != DBNull.Value && !string.IsNullOrWhiteSpace(val.ToString()))
                {
                    _dtp.Value = Convert.ToDateTime(val);
                }
                else
                {
                    _dtp.Value = DateTime.Today;
                }
            }

            _dtp.Visible = true;
            _dtp.BringToFront();
            _dtp.Focus();
        }

        private void dgvContasReceber_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (IsColunaData(e.ColumnIndex))
                ShowDatePickerForCell(e.RowIndex, e.ColumnIndex);
        }

        private void dgvContasReceber_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && IsColunaData(e.ColumnIndex))
                ShowDatePickerForCell(e.RowIndex, e.ColumnIndex);
        }

        private void btnNovoLote_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime hoje = DateTime.Today;

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Verifica se já existe lote para hoje
                    var cmdCheck = new SqlCommand("SELECT LoteId FROM LotesContasReceber WHERE DataLote = @Data", conn);
                    cmdCheck.Parameters.AddWithValue("@Data", hoje);
                    var result = cmdCheck.ExecuteScalar();

                    if (result != null)
                    {
                        MessageBox.Show("Já existe um lote para hoje. Aguarde o dia virar ou selecione o lote existente.",
                                      "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Cria novo lote
                    var cmdInsert = new SqlCommand(@"
                INSERT INTO LotesContasReceber (DataLote) 
                OUTPUT INSERTED.LoteId 
                VALUES (@Data)", conn);
                    cmdInsert.Parameters.AddWithValue("@Data", hoje);
                    loteAtualId = (int)cmdInsert.ExecuteScalar();
                }

                dgvContasReceber.DataSource = CriarDataTableVazio();
                AtualizarLabelLote();
                AtualizarResumoFinanceiro();

                MessageBox.Show("Novo lote criado com sucesso!", "Sucesso",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar novo lote: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable CriarDataTableVazio()
        {
            var dt = new DataTable();
            dt.Columns.Add("ContaId", typeof(int));
            dt.Columns.Add("ClienteId", typeof(int));
            dt.Columns.Add("Descricao", typeof(string));
            dt.Columns.Add("Valor", typeof(decimal));
            dt.Columns.Add("DataVencimento", typeof(DateTime));
            dt.Columns.Add("DataRecebimento", typeof(DateTime));
            dt.Columns.Add("StatusRecebimento", typeof(string));
            dt.Columns.Add("Observacoes", typeof(string));
            return dt;
        }

        private void btnSelecionarLote_Click(object sender, EventArgs e)
        {
            using (var formSelecionar = new FinanceiroFormLotesReceber())
            {
                if (formSelecionar.ShowDialog() == DialogResult.OK && formSelecionar.LoteSelecionadoId.HasValue)
                {
                    loteAtualId = formSelecionar.LoteSelecionadoId.Value;
                    CarregarContasDoLote(loteAtualId.Value);
                    AtualizarLabelLote();
                    AtualizarResumoFinanceiro();
                }
            }
        }

        private void CarregarContasDoLote(int loteId)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                SELECT 
                    cr.ContaId,
                    cr.ClienteId,
                    cr.Descricao,
                    cr.Valor,
                    cr.DataVencimento,
                    cr.DataRecebimento,
                    cr.Observacoes,
                    c.Nome AS NomeCliente
                FROM ContasReceber cr
                INNER JOIN Clientes c ON cr.ClienteId = c.ClienteId
                WHERE cr.LoteId = @lote";

                    // Aplica filtros
                    if (cmbStatus.SelectedIndex > 0)
                    {
                        switch (cmbStatus.SelectedItem.ToString())
                        {
                            case "Pendente":
                                sql += " AND cr.DataRecebimento IS NULL AND cr.DataVencimento >= CONVERT(date, GETDATE())";
                                break;
                            case "Recebido":
                                sql += " AND cr.DataRecebimento IS NOT NULL";
                                break;
                            case "Atrasado":
                                sql += " AND cr.DataRecebimento IS NULL AND cr.DataVencimento < CONVERT(date, GETDATE())";
                                break;
                            case "Vencimento Próximo":
                                sql += " AND cr.DataRecebimento IS NULL AND DATEDIFF(DAY, CONVERT(date, GETDATE()), cr.DataVencimento) <= 3 AND cr.DataVencimento >= CONVERT(date, GETDATE())";
                                break;
                        }
                    }

                    if (cmbCliente.SelectedIndex > 0)
                    {
                        int clienteId = ObterIdClientePorNome(cmbCliente.Text);
                        sql += " AND cr.ClienteId = @clienteId";
                    }

                    sql += " ORDER BY cr.DataVencimento";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@lote", loteId);

                        if (cmbCliente.SelectedIndex > 0)
                        {
                            int clienteId = ObterIdClientePorNome(cmbCliente.Text);
                            cmd.Parameters.AddWithValue("@clienteId", clienteId);
                        }

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            // Verifica se a coluna já existe antes de adicionar
                            if (!dt.Columns.Contains("StatusRecebimento"))
                            {
                                dt.Columns.Add("StatusRecebimento", typeof(string));
                            }

                            foreach (DataRow row in dt.Rows)
                            {
                                row["StatusRecebimento"] = CalcularStatus(row);
                            }

                            dgvContasReceber.DataSource = dt;
                        }
                    }
                }

                AtualizarComboClientes();
                AplicarCoresStatus();
                AtualizarResumoFinanceiro();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar contas: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string CalcularStatus(DataRow row)
        {
            if (row == null || row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached)
                return "Pendente";

            bool recebido = row["DataRecebimento"] != DBNull.Value && row["DataRecebimento"] != null;
            if (recebido) return "Recebido";

            if (row["DataVencimento"] == DBNull.Value || row["DataVencimento"] == null)
                return "Pendente";

            DateTime vencimento = Convert.ToDateTime(row["DataVencimento"]);
            DateTime hoje = DateTime.Today;

            if (vencimento < hoje)
                return "Atrasado";
            else if ((vencimento - hoje).TotalDays <= 3)
                return "Vencimento Próximo";
            else
                return "Pendente";
        }

        private void AtualizarComboClientes()
        {
            var colCombo = dgvContasReceber.Columns["ClienteId"] as DataGridViewComboBoxColumn;
            if (colCombo != null)
            {
                colCombo.DataSource = clientesDt;
            }
        }

        private int ObterIdClientePorNome(string nome)
        {
            foreach (DataRow row in clientesDt.Rows)
            {
                if (row["Nome"].ToString() == nome)
                    return (int)row["ClienteId"];
            }
            return 0;
        }

        private void btnNovaConta_Click(object sender, EventArgs e)
        {
            try
            {
                if (!loteAtualId.HasValue)
                {
                    MessageBox.Show("Selecione um lote antes de criar uma conta.", "Aviso",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (clientesDt == null || clientesDt.Rows.Count == 0)
                {
                    MessageBox.Show("Nenhum cliente encontrado. Cadastre clientes primeiro.", "Aviso",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Garante que edições pendentes sejam salvas
                if (dgvContasReceber.IsCurrentCellInEditMode)
                {
                    dgvContasReceber.EndEdit();
                }

                // Verifica se o DataSource é nulo e inicializa se necessário
                if (dgvContasReceber.DataSource == null)
                {
                    dgvContasReceber.DataSource = CriarDataTableVazio();
                }

                int clientePadraoId = Convert.ToInt32(clientesDt.Rows[0]["ClienteId"]);
                int novoId;

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"
                INSERT INTO ContasReceber (LoteId, ClienteId, Descricao, Valor, DataVencimento, Observacoes)
                VALUES (@lote, @cliente, '', 0, GETDATE(), '');
                SELECT SCOPE_IDENTITY();", conn);

                    cmd.Parameters.AddWithValue("@lote", loteAtualId.Value);
                    cmd.Parameters.AddWithValue("@cliente", clientePadraoId);

                    novoId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                var dataSource = dgvContasReceber.DataSource as DataTable;
                if (dataSource != null)
                {
                    var newRow = dataSource.NewRow();
                    newRow["ContaId"] = novoId;
                    newRow["ClienteId"] = clientePadraoId;
                    newRow["Descricao"] = "";
                    newRow["Valor"] = 0;
                    newRow["DataVencimento"] = DateTime.Today;
                    newRow["DataRecebimento"] = DBNull.Value;
                    newRow["StatusRecebimento"] = "Pendente";
                    newRow["Observacoes"] = "";
                    dataSource.Rows.Add(newRow);

                    // Força a atualização da visualização da grid
                    dgvContasReceber.Refresh();

                    // Seleciona nova linha
                    dgvContasReceber.ClearSelection();
                    int novaLinhaIndex = dgvContasReceber.Rows.Count - 1;
                    if (novaLinhaIndex >= 0)
                    {
                        dgvContasReceber.Rows[novaLinhaIndex].Selected = true;
                        dgvContasReceber.CurrentCell = dgvContasReceber.Rows[novaLinhaIndex].Cells["ClienteId"];
                        dgvContasReceber.BeginEdit(true);
                    }
                }

                AtualizarResumoFinanceiro();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar nova conta: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvContasReceber_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvContasReceber.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            try
            {
                if (!int.TryParse(row.Cells["ContaId"].Value?.ToString(), out int contaId) || contaId <= 0)
                    return;

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // CORREÇÃO: Usar ContaId em vez de Id
                    var cmd = new SqlCommand(@"
                        UPDATE ContasReceber
                        SET ClienteId = @cliente,
                            Descricao = @descricao,
                            Valor = @valor,
                            DataVencimento = @vencimento,
                            DataRecebimento = @recebimento,
                            Observacoes = @observacoes
                        WHERE ContaId = @id", conn); // Alterado de Id para ContaId

                    cmd.Parameters.AddWithValue("@cliente", Convert.ToInt32(row.Cells["ClienteId"].Value));
                    cmd.Parameters.AddWithValue("@descricao", row.Cells["Descricao"].Value?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@valor", Convert.ToDecimal(row.Cells["Valor"].Value ?? 0));
                    cmd.Parameters.AddWithValue("@vencimento", Convert.ToDateTime(row.Cells["DataVencimento"].Value));
                    cmd.Parameters.AddWithValue("@observacoes", row.Cells["Observacoes"].Value?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@id", contaId);

                    // Data recebimento pode ser nula
                    var dataRecebimento = row.Cells["DataRecebimento"].Value;
                    if (dataRecebimento == null || dataRecebimento == DBNull.Value)
                        cmd.Parameters.AddWithValue("@recebimento", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@recebimento", Convert.ToDateTime(dataRecebimento));

                    cmd.ExecuteNonQuery();
                }

                // Atualiza status calculado
                var dt = dgvContasReceber.DataSource as DataTable;
                if (dt != null && e.RowIndex >= 0 && e.RowIndex < dt.Rows.Count)
                {
                    var dataRow = dt.Rows[e.RowIndex];
                    if (dataRow.RowState != DataRowState.Deleted && dataRow.RowState != DataRowState.Detached)
                    {
                        dataRow["StatusRecebimento"] = CalcularStatus(dataRow);
                    }
                }

                this.BeginInvoke(new Action(() =>
                {
                    AplicarCoresStatus();
                    AtualizarResumoFinanceiro();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar alterações: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMarcarRecebido_Click(object sender, EventArgs e)
        {
            if (dgvContasReceber.CurrentRow == null) return;

            try
            {
                int contaId = Convert.ToInt32(dgvContasReceber.CurrentRow.Cells["ContaId"].Value);

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // CORREÇÃO: Usar ContaId em vez de Id
                    var cmd = new SqlCommand(@"
                        UPDATE ContasReceber 
                        SET DataRecebimento = GETDATE() 
                        WHERE ContaId = @Id", conn); // Alterado de Id para ContaId
                    cmd.Parameters.AddWithValue("@Id", contaId);
                    cmd.ExecuteNonQuery();
                }

                // Atualiza a grid
                dgvContasReceber.CurrentRow.Cells["DataRecebimento"].Value = DateTime.Today;
                dgvContasReceber.CurrentRow.Cells["StatusRecebimento"].Value = "Recebido";

                AplicarCoresStatus();
                AtualizarResumoFinanceiro();

                MessageBox.Show("Conta marcada como recebida!", "Sucesso",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao marcar como recebido: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExcluirConta_Click(object sender, EventArgs e)
        {
            if (dgvContasReceber.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione uma conta para excluir.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Deseja realmente excluir esta conta?", "Confirmação",
                              MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                int contaId = Convert.ToInt32(dgvContasReceber.CurrentRow.Cells["ContaId"].Value);

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM ContasReceber WHERE ContaId = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", contaId);
                    cmd.ExecuteNonQuery();
                }

                // Remove da grid
                if (dgvContasReceber.DataSource is DataTable dt)
                {
                    DataRow rowToDelete = dt.Rows.Cast<DataRow>()
                        .FirstOrDefault(r => r.RowState != DataRowState.Deleted &&
                                             Convert.ToInt32(r["ContaId"]) == contaId);
                    if (rowToDelete != null)
                    {
                        rowToDelete.Delete();
                        dt.AcceptChanges();
                    }
                }

                AtualizarResumoFinanceiro();
                MessageBox.Show("Conta excluída com sucesso!", "Sucesso",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir conta: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (loteAtualId.HasValue)
            {
                CarregarContasDoLote(loteAtualId.Value);
            }
            else
            {
                MessageBox.Show("Selecione um lote primeiro.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimparFiltro_Click(object sender, EventArgs e)
        {
            cmbCliente.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;

            if (loteAtualId.HasValue)
            {
                CarregarContasDoLote(loteAtualId.Value);
            }
        }

        private void FiltroChanged(object sender, EventArgs e)
        {
            // Filtro automático quando selecionar um item
            if (loteAtualId.HasValue)
            {
                CarregarContasDoLote(loteAtualId.Value);
            }
        }

        private void AtualizarLabelLote()
        {
            if (loteAtualId.HasValue)
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT DataLote FROM LotesContasReceber WHERE LoteId = @LoteId", conn);
                    cmd.Parameters.AddWithValue("@LoteId", loteAtualId.Value);
                    var dataLote = (DateTime)cmd.ExecuteScalar();
                    lblLoteAtual.Text = $"Lote: {dataLote:dd/MM/yyyy}";
                }
            }
            else
            {
                lblLoteAtual.Text = "Nenhum lote selecionado";
            }
        }

        private void AplicarCoresStatus()
        {
            foreach (DataGridViewRow row in dgvContasReceber.Rows)
            {
                if (row.IsNewRow) continue;

                var status = row.Cells["StatusRecebimento"].Value?.ToString();
                if (string.IsNullOrEmpty(status)) continue;

                switch (status)
                {
                    case "Recebido":
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        break;
                    case "Atrasado":
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                        break;
                    case "Vencimento Próximo":
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                        break;
                    default:
                        row.DefaultCellStyle.BackColor = Color.White;
                        break;
                }
            }
        }

        private void AtualizarResumoFinanceiro()
        {
            try
            {
                decimal totalPendente = 0;
                decimal totalRecebido = 0;
                decimal totalAtrasado = 0;
                decimal totalGeral = 0;

                var dataSource = dgvContasReceber.DataSource as DataTable;
                if (dataSource == null) return;

                foreach (DataRow row in dataSource.Rows)
                {
                    //Adicionar → ignorar linhas removidas ou ainda não anexadas
                    if (row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached)
                        continue;

                    decimal valor = row["Valor"] != DBNull.Value ? Convert.ToDecimal(row["Valor"]) : 0;
                    string status = row["StatusRecebimento"] != DBNull.Value ? row["StatusRecebimento"].ToString() : "";

                    totalGeral += valor;

                    switch (status)
                    {
                        case "Pendente":
                        case "Vencimento Próximo":
                            totalPendente += valor;
                            break;
                        case "Recebido":
                            totalRecebido += valor;
                            break;
                        case "Atrasado":
                            totalAtrasado += valor;
                            break;
                    }
                }

                lblTotalPendente.Text = $"Pendente: {totalPendente:C2}";
                lblTotalRecebido.Text = $"Recebido: {totalRecebido:C2}";
                lblTotalAtrasado.Text = $"Atrasado: {totalAtrasado:C2}";
                lblTotalGeral.Text = $"Total: {totalGeral:C2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar resumo financeiro: {ex.Message}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarTextoLabelsResumo()
        {
            lblTotalPendente.Text = "Pendente: R$ 0,00";
            lblTotalRecebido.Text = "Recebido: R$ 0,00";
            lblTotalAtrasado.Text = "Atrasado: R$ 0,00";
            lblTotalGeral.Text = "Total: R$ 0,00";
        }


        private void btnRelatorio_Click(object sender, EventArgs e)
        {
            if (!loteAtualId.HasValue)
            {
                MessageBox.Show("Selecione um lote primeiro.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var formRelatorio = new formRelatorioReceber(loteAtualId.Value))
                {
                    formRelatorio.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar relatório: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvContasReceber_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            AplicarCoresStatus();
            AtualizarResumoFinanceiro();
        }

        private void dgvContasReceber_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Formatação de valores monetários
            if (dgvContasReceber.Columns[e.ColumnIndex].Name == "Valor" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal valor))
                {
                    e.Value = valor.ToString("C2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void dgvContasReceber_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (dgvContasReceber.Columns[e.ColumnIndex].Name == "Valor")
            {
                if (e.Value != null)
                {
                    string valueStr = e.Value.ToString().Replace("R$", "").Trim();
                    if (decimal.TryParse(valueStr, System.Globalization.NumberStyles.Currency,
                        System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out decimal valor))
                    {
                        e.Value = valor;
                        e.ParsingApplied = true;
                    }
                }
            }
        }

        private void dgvContasReceber_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"Erro ao processar dados: {e.Exception.Message}", "Erro",
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.ThrowException = false;
        }
    }
}