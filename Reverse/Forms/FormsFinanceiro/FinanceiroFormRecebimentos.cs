using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms.FormsFinanceiro
{
    public partial class FinanceiroFormRecebimentos : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private DataTable dtClientes;
        private DataTable dtClientesComCpfCnpj;
        private bool isLoading = false;

        public FinanceiroFormRecebimentos(int _usuarioId)
        {
            InitializeComponent();

            btnNovaLinha.ForeColor = Color.Black;
            btnExcluir.ForeColor = Color.Black;

            ConfigurarGrid();

            dtpRecebimentos.Value = DateTime.Today;

            dgvRecebimento.CellFormatting += dgvRecebimento_CellFormatting;

            _ = InicializarFormAsync();
        }

        private async Task InicializarFormAsync()
        {
            isLoading = true;

            await CarregarClientesAsync();

            await CarregarRecebimentosAsync(dtpRecebimentos.Value);

            isLoading = false;
        }

        private void ConfigurarGrid()
        {
            dgvRecebimento.AutoGenerateColumns = false;
            dgvRecebimento.AllowUserToAddRows = false;
            dgvRecebimento.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecebimento.MultiSelect = false;
            dgvRecebimento.BackgroundColor = Color.FromArgb(242, 243, 244);

            dgvRecebimento.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvRecebimento.DefaultCellStyle.ForeColor = Color.Black;
            dgvRecebimento.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRecebimento.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvRecebimento.EnableHeadersVisualStyles = false;

            // Coluna ID (oculta)
            var colId = new DataGridViewTextBoxColumn
            {
                Name = "ContaId",
                DataPropertyName = "ContaId",
                Visible = false
            };
            dgvRecebimento.Columns.Add(colId);

            // Coluna Cliente (ComboBox)
            var colCliente = new DataGridViewComboBoxColumn
            {
                Name = "Cliente",
                HeaderText = "Cliente",
                DataPropertyName = "ClienteId",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                Width = 200,
                FlatStyle = FlatStyle.Flat
            };
            dgvRecebimento.Columns.Add(colCliente);

            // Coluna CPF/CNPJ (ComboBox)
            var colCpfCnpj = new DataGridViewComboBoxColumn
            {
                Name = "CPF_CNPJ",
                HeaderText = "CPF/CNPJ",
                DataPropertyName = "ClienteId",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                Width = 150,
                FlatStyle = FlatStyle.Flat,
                DisplayMember = "CPF_CNPJ",
                ValueMember = "ClienteId"
            };
            dgvRecebimento.Columns.Add(colCpfCnpj);

            // Coluna Valor
            var colValor = new DataGridViewTextBoxColumn
            {
                Name = "Valor",
                HeaderText = "Valor Recebido",
                DataPropertyName = "Valor",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                }
            };
            dgvRecebimento.Columns.Add(colValor);

            // Coluna Forma Pagamento (ComboBox)
            var colFormaPgto = new DataGridViewComboBoxColumn
            {
                Name = "FormaPagamento",
                HeaderText = "Forma de Pagamento",
                DataPropertyName = "FormaPagamento",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                Width = 180,
                FlatStyle = FlatStyle.Flat
            };
            colFormaPgto.Items.AddRange("PIX", "Cheque", "Dinheiro", "Cartão de Crédito", "Cartão de Débito");
            dgvRecebimento.Columns.Add(colFormaPgto);

            // Coluna Observação
            var colObs = new DataGridViewTextBoxColumn
            {
                Name = "Observacoes",
                HeaderText = "Observações",
                DataPropertyName = "Observacoes",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            dgvRecebimento.Columns.Add(colObs);

            // Eventos
            dgvRecebimento.CellValueChanged += DgvRecebimento_CellValueChanged;
            dgvRecebimento.CurrentCellDirtyStateChanged += DgvRecebimento_CurrentCellDirtyStateChanged;
            dgvRecebimento.EditingControlShowing += DgvRecebimento_EditingControlShowing;
            dgvRecebimento.DataError += DgvRecebimento_DataError;
        }

        private void dgvRecebimento_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRecebimento.Columns[e.ColumnIndex].Name == "CPF_CNPJ")
            {
                if (e.Value == null || e.Value == DBNull.Value)
                {
                    e.Value = "CPF/CNPJ não informado";
                    e.CellStyle.ForeColor = Color.Gray;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Italic);
                    e.FormattingApplied = true;
                }
            }
        }

        private void DgvRecebimento_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvRecebimento.IsCurrentCellDirty &&
                dgvRecebimento.CurrentCell is DataGridViewComboBoxCell)
            {
                dgvRecebimento.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvRecebimento_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (isLoading) return;

            Console.WriteLine($"DataError: {e.Exception.Message}");

            e.ThrowException = false;
        }

        private void DgvRecebimento_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvRecebimento.CurrentCell.ColumnIndex == dgvRecebimento.Columns["CPF_CNPJ"].Index)
            {
                var combo = e.Control as ComboBox;
                if (combo != null)
                {
                    combo.DropDownStyle = ComboBoxStyle.DropDown;
                    combo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    combo.AutoCompleteSource = AutoCompleteSource.ListItems;
                }
            }
        }

        private async void DgvRecebimento_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isLoading || e.RowIndex < 0) return;

            var row = dgvRecebimento.Rows[e.RowIndex];
            var contaId = row.Cells["ContaId"].Value;

            // Sincronizar as combobox Cliente e CPF/CNPJ
            if (e.ColumnIndex == dgvRecebimento.Columns["Cliente"].Index)
            {
                // Quando seleciona cliente, atualiza CPF/CNPJ automaticamente
                var clienteId = row.Cells["Cliente"].Value;
                if (clienteId != null && clienteId != DBNull.Value)
                {
                    int id = Convert.ToInt32(clienteId);

                    // Verificar se este cliente tem CPF/CNPJ válido
                    bool temCpfCnpjValido = await ClienteTemCpfCnpjValidoAsync(id);

                    isLoading = true;
                    if (temCpfCnpjValido)
                    {
                        row.Cells["CPF_CNPJ"].Value = clienteId;
                    }
                    else
                    {
                        // Cliente sem CPF/CNPJ válido - definir valor nulo
                        row.Cells["CPF_CNPJ"].Value = DBNull.Value;
                        // Opcional: mostrar mensagem
                        MessageBox.Show("Este cliente não possui CPF/CNPJ cadastrado.",
                                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    isLoading = false;
                }
            }
            else if (e.ColumnIndex == dgvRecebimento.Columns["CPF_CNPJ"].Index)
            {
                // Quando seleciona por CPF/CNPJ, atualiza Cliente automaticamente
                var cpfCnpjCell = row.Cells["CPF_CNPJ"];
                if (cpfCnpjCell.Value != null && cpfCnpjCell.Value != DBNull.Value)
                {
                    int clienteId = Convert.ToInt32(cpfCnpjCell.Value);

                    // Verificar se é o item especial (cliente sem CPF/CNPJ)
                    if (clienteId == 0)
                    {
                        MessageBox.Show("Este cliente não possui CPF/CNPJ cadastrado. " +
                                       "Selecione o cliente pelo nome na coluna 'Cliente'.",
                                       "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        row.Cells["CPF_CNPJ"].Value = DBNull.Value;
                        return;
                    }

                    // Verificar se o cliente existe
                    bool clienteExiste = await VerificarClienteExiste(clienteId);

                    if (clienteExiste)
                    {
                        isLoading = true;
                        row.Cells["Cliente"].Value = clienteId;
                        isLoading = false;
                    }
                    else
                    {
                        MessageBox.Show($"Nenhum cliente cadastrado com este CPF/CNPJ.\nPor favor, cadastre o cliente primeiro.",
                                        "Cliente não encontrado",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Limpar a seleção
                        row.Cells["CPF_CNPJ"].Value = DBNull.Value;
                        return;
                    }
                }
            }

            if (contaId != null && contaId != DBNull.Value && !isLoading)
            {
                await Task.Delay(100);
                if (!isLoading)
                {
                    await SalvarAlteracaoAsync(row);
                }
            }
        }

        private async Task<bool> ClienteTemCpfCnpjValidoAsync(int clienteId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT CPF_CNPJ FROM Clientes WHERE ClienteId = @id", conn);
                cmd.Parameters.AddWithValue("@id", clienteId);

                var result = await cmd.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                    return false;

                string cpfCnpj = result.ToString();
                string digits = new string(cpfCnpj.Where(char.IsDigit).ToArray());

                return digits.Length == 11 || digits.Length == 14;
            }
        }



        private async Task<bool> VerificarClienteExiste(int clienteId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Clientes WHERE ClienteId = @id AND Status = 1", conn);
                cmd.Parameters.AddWithValue("@id", clienteId);
                int count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
        }

        private async Task CarregarClientesAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT ClienteId, Nome, CPF_CNPJ FROM Clientes WHERE Status = 1 ORDER BY Nome", conn);
                dtClientes = new DataTable();
                dtClientesComCpfCnpj = new DataTable();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dtClientes.Load(reader);

                    cmd.CommandText = "SELECT ClienteId, Nome, CPF_CNPJ FROM Clientes WHERE Status = 1 ORDER BY Nome";
                    using (var reader2 = await cmd.ExecuteReaderAsync())
                    {
                        dtClientesComCpfCnpj.Load(reader2);
                    }
                }
            }

            // Configurar coluna de Cliente
            var colCliente = (DataGridViewComboBoxColumn)dgvRecebimento.Columns["Cliente"];
            colCliente.DataSource = dtClientes;
            colCliente.DisplayMember = "Nome";
            colCliente.ValueMember = "ClienteId";

            var colCpfCnpj = (DataGridViewComboBoxColumn)dgvRecebimento.Columns["CPF_CNPJ"];

            DataTable dtCpfCnpjFormatado = dtClientesComCpfCnpj.Clone();
            foreach (DataRow row in dtClientesComCpfCnpj.Rows)
            {
                string cpfCnpjOriginal = row["CPF_CNPJ"].ToString();

                // Remover todos os espaços em branco
                string cpfCnpjSemEspacos = new string(cpfCnpjOriginal.Where(c => !char.IsWhiteSpace(c)).ToArray());

                // Verificar se após remover espaços ainda tem conteúdo
                if (string.IsNullOrWhiteSpace(cpfCnpjSemEspacos))
                    continue; // Pula clientes sem CPF/CNPJ

                // Extrair apenas os dígitos
                string digits = new string(cpfCnpjSemEspacos.Where(char.IsDigit).ToArray());

                // Verificar se tem 11 ou 14 dígitos (CPF ou CNPJ válido)
                if (digits.Length != 11 && digits.Length != 14)
                    continue;

                DataRow newRow = dtCpfCnpjFormatado.NewRow();
                newRow["ClienteId"] = row["ClienteId"];
                newRow["Nome"] = row["Nome"];

                // Formatar o CPF/CNPJ
                if (digits.Length == 11)
                {
                    newRow["CPF_CNPJ"] = Convert.ToUInt64(digits).ToString(@"000\.000\.000\-00");
                }
                else if (digits.Length == 14)
                {
                    newRow["CPF_CNPJ"] = Convert.ToUInt64(digits).ToString(@"00\.000\.000\/0000\-00");
                }

                dtCpfCnpjFormatado.Rows.Add(newRow);
            }

            if (dtCpfCnpjFormatado.Rows.Count == 0)
            {
                DataRow newRow = dtCpfCnpjFormatado.NewRow();
                newRow["ClienteId"] = 0;
                newRow["Nome"] = "Nenhum CPF/CNPJ cadastrado";
                newRow["CPF_CNPJ"] = "CPF/CNPJ não informado";
                dtCpfCnpjFormatado.Rows.Add(newRow);
            }

            colCpfCnpj.DataSource = dtCpfCnpjFormatado;
            colCpfCnpj.DisplayMember = "CPF_CNPJ";
            colCpfCnpj.ValueMember = "ClienteId";
        }

        private async Task CarregarRecebimentosAsync(DateTime data)
        {
            isLoading = true;
            dgvRecebimento.Rows.Clear();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand(@"
                    SELECT r.ContaId, r.ClienteId, c.Nome, c.CPF_CNPJ, r.Valor, r.FormaPagamento, r.Observacoes,
                           r.StatusPagamento, r.DataVencimento, r.DataRecebimento
                    FROM ContasReceber r
                    INNER JOIN Clientes c ON r.ClienteId = c.ClienteId
                    WHERE (r.StatusPagamento = 1 AND CAST(r.DataRecebimento AS DATE) = @data)
                       OR (r.StatusPagamento = 0 AND CAST(r.DataVencimento AS DATE) = @data)
                    ORDER BY r.ContaId", conn);

                cmd.Parameters.AddWithValue("@data", data.Date);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dgvRecebimento.Rows.Add(
                            reader["ContaId"],
                            reader["ClienteId"],
                            reader["ClienteId"], // Mesmo clienteId para a coluna CPF/CNPJ
                            reader["Valor"],
                            reader["FormaPagamento"] == DBNull.Value ? "PIX" : reader["FormaPagamento"],
                            reader["Observacoes"] == DBNull.Value ? "" : reader["Observacoes"]
                        );
                    }
                }
            }

            isLoading = false;
        }

        private async void dtpRecebimentos_ValueChanged(object sender, EventArgs e)
        {
            if (isLoading) return;

            await CarregarRecebimentosAsync(dtpRecebimentos.Value);
        }

        private async void btnNovaLinha_Click(object sender, EventArgs e)
        {
            if (dtClientes == null || dtClientes.Rows.Count == 0)
            {
                MessageBox.Show("Nenhum cliente ativo disponível.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(@"
                        INSERT INTO ContasReceber (ClienteId, DataVencimento, Valor, StatusPagamento, FormaPagamento, Observacoes)
                        OUTPUT INSERTED.ContaId
                        VALUES (@clienteId, @data, 0, 0, 'PIX', '')", conn);

                    cmd.Parameters.AddWithValue("@clienteId", dtClientes.Rows[0]["ClienteId"]);
                    cmd.Parameters.AddWithValue("@data", dtpRecebimentos.Value.Date);

                    int novoId = (int)await cmd.ExecuteScalarAsync();

                    isLoading = true;
                    dgvRecebimento.Rows.Add(
                        novoId,
                        dtClientes.Rows[0]["ClienteId"],
                        dtClientes.Rows[0]["ClienteId"], // CPF/CNPJ
                        0m,
                        "PIX",
                        ""
                    );
                    isLoading = false;

                    dgvRecebimento.CurrentCell = dgvRecebimento.Rows[dgvRecebimento.Rows.Count - 1].Cells["Cliente"];
                    dgvRecebimento.BeginEdit(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar novo recebimento: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvRecebimento.CurrentRow == null)
            {
                MessageBox.Show("Selecione uma linha para excluir.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var contaId = dgvRecebimento.CurrentRow.Cells["ContaId"].Value;
            if (contaId == null || contaId == DBNull.Value)
                return;

            if (MessageBox.Show("Deseja realmente excluir este recebimento?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand("DELETE FROM ContasReceber WHERE ContaId = @id", conn);
                    cmd.Parameters.AddWithValue("@id", contaId);
                    await cmd.ExecuteNonQueryAsync();
                }

                dgvRecebimento.Rows.Remove(dgvRecebimento.CurrentRow);
                MessageBox.Show("Recebimento excluído com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir recebimento: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SalvarAlteracaoAsync(DataGridViewRow row)
        {
            try
            {
                var contaId = row.Cells["ContaId"].Value;
                var clienteId = row.Cells["Cliente"].Value;
                var valor = row.Cells["Valor"].Value;
                var formaPgto = row.Cells["FormaPagamento"].Value;
                var obs = row.Cells["Observacoes"].Value ?? "";

                if (clienteId == null || clienteId == DBNull.Value)
                {
                    MessageBox.Show("Selecione um cliente.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal valorDecimal = 0;
                if (valor != null && valor != DBNull.Value)
                {
                    if (valor is decimal dec)
                        valorDecimal = dec;
                    else if (!decimal.TryParse(valor.ToString(), NumberStyles.Any,
                        CultureInfo.GetCultureInfo("pt-BR"), out valorDecimal))
                    {
                        MessageBox.Show("Valor inválido.", "Validação",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(@"
                        UPDATE ContasReceber 
                        SET ClienteId = @clienteId, 
                            Valor = @valor, 
                            FormaPagamento = @forma,
                            Observacoes = @obs,
                            StatusPagamento = 1,
                            DataRecebimento = @dataReceb
                        WHERE ContaId = @id", conn);

                    cmd.Parameters.AddWithValue("@id", contaId);
                    cmd.Parameters.AddWithValue("@clienteId", clienteId);
                    cmd.Parameters.AddWithValue("@valor", valorDecimal);
                    cmd.Parameters.AddWithValue("@forma", formaPgto?.ToString() ?? "PIX");
                    cmd.Parameters.AddWithValue("@obs", obs?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@dataReceb", dtpRecebimentos.Value.Date);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar alteração: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}