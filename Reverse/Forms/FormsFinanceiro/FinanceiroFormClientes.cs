using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms.FormsFinanceiro
{
    public partial class FinanceiroFormClientes : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private readonly Dictionary<string, bool> _cacheExisteCpfCnpj = new Dictionary<string, bool>();
        private int? clienteAtualId;

        public FinanceiroFormClientes(int _usuarioId)
        {
            InitializeComponent();
            CarregarCombos();
            ConfigurarGridClientes();
            ConfigurarGrid(dgvClientes);
            ConfigurarGrid(dgvCompras);
            ConfigurarGrid(dgvAtendimentos);
            ConfigurarGrid(dgvProdutosComprados);
            dgvClientes.CellFormatting += dgvClientes_CellFormatting;

            _ = CarregarClientesAsync();
        }

        private void ConfigurarGridClientes()
        {
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.MultiSelect = false;
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.Columns.Clear();

            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "ClienteId", DataPropertyName = "ClienteId", Visible = false });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome", HeaderText = "Nome/Razão Social", DataPropertyName = "Nome" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "CPF_CNPJ", HeaderText = "CPF/CNPJ", DataPropertyName = "CPF_CNPJ" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefone", HeaderText = "Telefone", DataPropertyName = "Telefone" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cidade", HeaderText = "Cidade", DataPropertyName = "Cidade" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", DataPropertyName = "Status" });
        }
        private void ConfigurarGrid(DataGridView dgv)
        {
            // Ajuste de colunas e linhas
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = false;

            // Cores e fontes
            dgv.BackgroundColor = Color.FromArgb(242, 243, 244);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            // Cabeçalho
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
        }


        private async Task CarregarClientesAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT ClienteId, Nome, CPF_CNPJ, Telefone, Cidade, Status FROM Clientes ORDER BY Nome", conn);
                var dt = new DataTable();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
                dgvClientes.DataSource = dt;
            }
            dgvClientes.ClearSelection();
        }

        private void dgvClientes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvClientes.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                bool statusAtivo;

                if (e.Value is bool b)
                {
                    statusAtivo = b;
                }
                else
                {
                    string valor = e.Value.ToString().Trim().ToLower();
                    statusAtivo = (valor == "true" || valor == "ativo");
                }

                if (statusAtivo)
                {
                    e.Value = "Ativo";
                    e.CellStyle.ForeColor = Color.Green;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }
                else
                {
                    e.Value = "Inativo";
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }

                e.FormattingApplied = true;
            }
        }

        private void CarregarCombos()
        {
                    cmbEstado.Items.Clear();
                    cmbEstado.Items.AddRange(new string[]
                    {
                "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA",
                "MT","MS","MG","PA","PB","PR","PE","PI","RJ","RN",
                "RS","RO","RR","SC","SP","SE","TO"
                    });

                    cmbStatus.Items.Clear();
                    cmbStatus.Items.AddRange(new string[] { "Ativo", "Inativo" });

                    cmbPagamento.Items.Clear();
                    cmbPagamento.Items.AddRange(new string[]
                    {
                "À vista",
                "7 dias",
                "15 dias",
                "21 dias",
                "30 dias",
                "45 dias",
                "60 dias",
                "Parcelado 2x",
                "Parcelado 3x",
                "Parcelado 4x"
                    });

                    cmbFormaPagamento.Items.Clear();
                    cmbFormaPagamento.Items.AddRange(new string[]
                    {
                "Dinheiro",
                "PIX",
                "Cartão de Débito",
                "Boleto Bancário",
                "Transferência Bancária"
                    });


                    cmbRisco.Items.Clear();
                    cmbRisco.Items.AddRange(new string[]
                    {
                "Baixa",
                "Média",
                "Alta"
                    });

            if (cmbEstado.Items.Count > 0) cmbEstado.SelectedIndex = 0;
            if (cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0;
            if (cmbPagamento.Items.Count > 0) cmbPagamento.SelectedIndex = 0;
            if (cmbFormaPagamento.Items.Count > 0) cmbFormaPagamento.SelectedIndex = 0;
            if (cmbRisco.Items.Count > 0) cmbRisco.SelectedIndex = 0;
        }

        private async void dgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null || dgvClientes.CurrentRow.IsNewRow)
            {
                clienteAtualId = null;
                LimparCampos();
                return;
            }

            var valorId = dgvClientes.CurrentRow.Cells["ClienteId"].Value;

            if (valorId == null || valorId == DBNull.Value || string.IsNullOrWhiteSpace(valorId.ToString()))
            {
                clienteAtualId = null;
                LimparCampos();
            }
            else
            {
                clienteAtualId = Convert.ToInt32(valorId);
                await CarregarDadosClienteAsync(clienteAtualId.Value);
            }
        }

        private async Task CarregarDadosClienteAsync(int clienteId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // Query única com múltiplos resultsets
                var cmd = new SqlCommand(@"
            SELECT Nome, CPF_CNPJ, RG_IE, ResponsavelComercial, Telefone, Celular, Email, Site, 
                   Rua, Numero, Bairro, Cidade, Estado, CEP, DataCadastro, Status, LimiteCredito, 
                   CondicaoPagamento, FormaPagamentoPreferida, SaldoAberto, ValorAtrasado, QtdAtrasos, 
                   UltimaCompraData, UltimaCompraValor, TicketMedio, Observacoes, ClassificacaoRisco, 
                   RankingCliente, PercentualParticipacao
            FROM Clientes WHERE ClienteId = @Id;
            
            SELECT DataCompra, ProdutoServico, Valor, StatusPagamento
            FROM Compras WHERE ClienteId = @Id ORDER BY DataCompra DESC;
            
            SELECT NumeroChamado, DataAbertura, Assunto, Status
            FROM Atendimentos WHERE ClienteId = @Id ORDER BY DataAbertura DESC;
            
            SELECT TOP 10 ProdutoServico, COUNT(*) AS Quantidade, SUM(Valor) AS Total
            FROM Compras WHERE ClienteId = @Id
            GROUP BY ProdutoServico ORDER BY Quantidade DESC", conn);

                cmd.Parameters.AddWithValue("@Id", clienteId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    // CORREÇÃO: Verificar se há dados antes de ler
                    if (reader.HasRows && await reader.ReadAsync())
                    {
                        txtNome.Text = reader["Nome"]?.ToString();
                        mtbCPF.Text = reader["CPF_CNPJ"]?.ToString();
                        txtRG.Text = reader["RG_IE"]?.ToString();
                        txtResposavel.Text = reader["ResponsavelComercial"]?.ToString();
                        mtbTelefone.Text = reader["Telefone"]?.ToString();
                        mtbCelular.Text = reader["Celular"]?.ToString();
                        txtEmail.Text = reader["Email"]?.ToString();
                        txtSite.Text = reader["Site"]?.ToString();
                        txtRua.Text = reader["Rua"]?.ToString();
                        txtNumero.Text = reader["Numero"]?.ToString();
                        txtBairro.Text = reader["Bairro"]?.ToString();
                        txtCidade.Text = reader["Cidade"]?.ToString();

                        string estado = reader["Estado"]?.ToString();
                        cmbEstado.SelectedItem = !string.IsNullOrWhiteSpace(estado) && cmbEstado.Items.Contains(estado) ? estado : "SP";

                        mtbCEP.Text = reader["CEP"]?.ToString();
                        dtpCadastro.Value = reader["DataCadastro"] != DBNull.Value ? Convert.ToDateTime(reader["DataCadastro"]) : DateTime.Today;

                        if (reader["Status"] != DBNull.Value)
                        {
                            if (reader["Status"] is bool statusBool)
                                cmbStatus.SelectedItem = statusBool ? "Ativo" : "Inativo";
                            else
                                cmbStatus.SelectedItem = reader["Status"].ToString();
                        }
                        else
                        {
                            cmbStatus.SelectedItem = "Ativo";
                        }

                        nudLimite.Value = reader["LimiteCredito"] != DBNull.Value ? Convert.ToDecimal(reader["LimiteCredito"]) : 0;

                        string condPag = reader["CondicaoPagamento"]?.ToString();
                        cmbPagamento.SelectedItem = !string.IsNullOrWhiteSpace(condPag) && cmbPagamento.Items.Contains(condPag) ? condPag : "À vista";

                        string formaPag = reader["FormaPagamentoPreferida"]?.ToString();
                        cmbFormaPagamento.SelectedItem = !string.IsNullOrWhiteSpace(formaPag) && cmbFormaPagamento.Items.Contains(formaPag) ? formaPag : "Dinheiro";

                        txtSaldo.Text = reader["SaldoAberto"] != DBNull.Value ? Convert.ToDecimal(reader["SaldoAberto"]).ToString("N2") : "0,00";
                        txtValorAtrasado.Text = reader["ValorAtrasado"] != DBNull.Value ? Convert.ToDecimal(reader["ValorAtrasado"]).ToString("N2") : "0,00";
                        txtQuantAtra.Text = reader["QtdAtrasos"] != DBNull.Value ? reader["QtdAtrasos"].ToString() : "0";
                        dtpDataUltima.Value = reader["UltimaCompraData"] != DBNull.Value ? Convert.ToDateTime(reader["UltimaCompraData"]) : DateTime.Today;
                        txtValorUltima.Text = reader["UltimaCompraValor"] != DBNull.Value ? Convert.ToDecimal(reader["UltimaCompraValor"]).ToString("N2") : "0,00";
                        lblTicket.Text = reader["TicketMedio"] != DBNull.Value ? Convert.ToDecimal(reader["TicketMedio"]).ToString("N2") : "0,00";

                        txtObsInt.Text = reader["Observacoes"]?.ToString();

                        string risco = reader["ClassificacaoRisco"]?.ToString();
                        cmbRisco.SelectedItem = !string.IsNullOrWhiteSpace(risco) && cmbRisco.Items.Contains(risco) ? risco : "Baixa";

                        lblRanking.Text = reader["RankingCliente"] != DBNull.Value ? reader["RankingCliente"].ToString() : "0";
                        prbPercentual.Value = reader["PercentualParticipacao"] != DBNull.Value ? Convert.ToInt32(reader["PercentualParticipacao"]) : 0;
                    }

                    // CORREÇÃO: Só avança se não estiver fechado
                    if (!reader.IsClosed && await reader.NextResultAsync())
                    {
                        var dtCompras = new DataTable();
                        dtCompras.Load(reader);
                        dgvCompras.DataSource = dtCompras;

                        // Terceiro resultset - Atendimentos
                        if (!reader.IsClosed && await reader.NextResultAsync())
                        {
                            var dtAtendimentos = new DataTable();
                            dtAtendimentos.Load(reader);
                            dgvAtendimentos.DataSource = dtAtendimentos;

                            // Quarto resultset - Produtos
                            if (!reader.IsClosed && await reader.NextResultAsync())
                            {
                                var dtProdutos = new DataTable();
                                dtProdutos.Load(reader);
                                dgvProdutosComprados.DataSource = dtProdutos;
                            }
                        }
                    }
                }
            }
        }

        private async Task<bool> CpfCnpjJaExisteAsync(string cpfCnpjMask, int? clienteIdIgnorar = null)
        {
            var cpfCnpj = new string((cpfCnpjMask ?? "").Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(cpfCnpj)) return false;

            string cacheKey = $"{cpfCnpj}_{clienteIdIgnorar}";
            if (_cacheExisteCpfCnpj.ContainsKey(cacheKey))
                return _cacheExisteCpfCnpj[cacheKey];

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string sql = "SELECT COUNT(*) FROM Clientes WHERE CPF_CNPJ = @cpf";

                if (clienteIdIgnorar.HasValue)
                    sql += " AND ClienteId != @clienteId";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cpf", cpfCnpj);

                if (clienteIdIgnorar.HasValue)
                    cmd.Parameters.AddWithValue("@clienteId", clienteIdIgnorar.Value);

                bool existe = (int)await cmd.ExecuteScalarAsync() > 0;
                _cacheExisteCpfCnpj[cacheKey] = existe;
                return existe;
            }
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            try
            {
                clienteAtualId = null;
                LimparCampos();
                dgvClientes.ClearSelection();
                txtNome.Focus();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao preparar novo cliente: {ex.Message}", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string GetCpfCnpjNumerico()
        {
            var originalText = mtbCPF.Text;
            var originalMaskFormat = mtbCPF.TextMaskFormat;

            try
            {
                mtbCPF.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                var numericoOnly = mtbCPF.Text;
                return new string(numericoOnly.Where(char.IsDigit).ToArray());
            }
            finally
            {
                mtbCPF.TextMaskFormat = originalMaskFormat;
            }
        }


        private bool ValidarCampos()
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(txtNome.Text))
                erros.Add("O campo Nome é obrigatório.");

            var cpfCnpj = GetCpfCnpjNumerico();
            if (string.IsNullOrWhiteSpace(cpfCnpj))
                erros.Add("CPF/CNPJ é obrigatório.");
            else if (!(cpfCnpj.Length == 11 || cpfCnpj.Length == 14))
                erros.Add("CPF/CNPJ inválido. Informe 11 dígitos (CPF) ou 14 dígitos (CNPJ).");

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) &&
                !System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                erros.Add("O e-mail informado não é válido.");

            if (erros.Any())
            {
                MessageBox.Show(string.Join("\n", erros), "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        private void SelecionarClienteNaGrid(int clienteId)
        {
            foreach (DataGridViewRow row in dgvClientes.Rows)
            {
                if (row.Cells["ClienteId"].Value != null &&
                    row.Cells["ClienteId"].Value != DBNull.Value &&
                    Convert.ToInt32(row.Cells["ClienteId"].Value) == clienteId)
                {
                    row.Selected = true;
                    dgvClientes.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCampos()) return;

                var cpfCnpj = GetCpfCnpjNumerico();

                if (await CpfCnpjJaExisteAsync(cpfCnpj, clienteAtualId))
                {
                    MessageBox.Show("Já existe um cliente com este CPF/CNPJ.",
                                    "Duplicação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idNovoOuEditado;

                if (clienteAtualId.HasValue)
                {
                    await AtualizarClienteAsync();
                    idNovoOuEditado = clienteAtualId.Value;
                    MessageBox.Show("Cliente atualizado com sucesso!", "Sucesso",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    idNovoOuEditado = await InserirClienteAsync();
                    clienteAtualId = idNovoOuEditado;
                    MessageBox.Show("Cliente criado com sucesso!", "Sucesso",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await CarregarClientesAsync();
                SelecionarClienteNaGrid(idNovoOuEditado);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar cliente: {ex.Message}", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<int> InserirClienteAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
            INSERT INTO Clientes (
                Nome, CPF_CNPJ, RG_IE, ResponsavelComercial, Telefone, Celular, Email, Site, 
                Rua, Numero, Bairro, Cidade, Estado, CEP, DataCadastro, Status, 
                LimiteCredito, CondicaoPagamento, FormaPagamentoPreferida, 
                SaldoAberto, ValorAtrasado, QtdAtrasos, UltimaCompraData, UltimaCompraValor, 
                TicketMedio, Observacoes, ClassificacaoRisco, RankingCliente, PercentualParticipacao
            )
            OUTPUT INSERTED.ClienteId
            VALUES (
                @Nome, @CPF, @RG, @Resp, @Tel, @Cel, @Email, @Site, 
                @Rua, @Num, @Bairro, @Cidade, @Estado, @CEP, @DataCad, @Status, 
                @Limite, @CondPag, @FormaPag, @Saldo, @ValAtraso, @QtdAtraso, 
                @UltData, @UltValor, @Ticket, @Observacoes, @ClassificacaoRisco, 
                @RankingCliente, @PercentualParticipacao
            )", conn);

                PreencherParametros(cmd);
                return (int)await cmd.ExecuteScalarAsync();
            }
        }
        private async Task AtualizarClienteAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                UPDATE Clientes SET 
                    Nome=@Nome, CPF_CNPJ=@CPF, RG_IE=@RG, ResponsavelComercial=@Resp, 
                    Telefone=@Tel, Celular=@Cel, Email=@Email, Site=@Site, Rua=@Rua, Numero=@Num, 
                    Bairro=@Bairro, Cidade=@Cidade, Estado=@Estado, CEP=@CEP, DataCadastro=@DataCad, 
                    Status=@Status, LimiteCredito=@Limite, CondicaoPagamento=@CondPag, 
                    FormaPagamentoPreferida=@FormaPag, SaldoAberto=@Saldo, ValorAtrasado=@ValAtraso, 
                    QtdAtrasos=@QtdAtraso, UltimaCompraData=@UltData, UltimaCompraValor=@UltValor, 
                    TicketMedio=@Ticket, 
                    ClassificacaoRisco=@ClassificacaoRisco
                WHERE ClienteId=@Id", conn);

                PreencherParametros(cmd);
                cmd.Parameters.AddWithValue("@Id", clienteAtualId.Value);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private void PreencherParametros(SqlCommand cmd)
        {
            var cultura = CultureInfo.GetCultureInfo("pt-BR");

            decimal ConverterParaDecimal(string texto)
            {
                if (string.IsNullOrWhiteSpace(texto))
                    return 0m;

                if (decimal.TryParse(texto, NumberStyles.Any, cultura, out decimal valor))
                    return valor;

                MessageBox.Show($"Valor inválido: \"{texto}\". Será considerado 0.",
                                "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0m;
            }

            int ConverterParaInt(string texto)
            {
                if (string.IsNullOrWhiteSpace(texto))
                    return 0;

                if (int.TryParse(texto, out int valor))
                    return valor;

                MessageBox.Show($"Número inválido: \"{texto}\". Será considerado 0.",
                                "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }

            object DbNullIfEmpty(string valor) =>
                string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor.Trim();

            cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
            var cpfCnpj = GetCpfCnpjNumerico();
            cmd.Parameters.AddWithValue("@CPF", cpfCnpj);

            cmd.Parameters.AddWithValue("@RG", DbNullIfEmpty(txtRG.Text));
            cmd.Parameters.AddWithValue("@Resp", DbNullIfEmpty(txtResposavel.Text));
            cmd.Parameters.AddWithValue("@Tel", DbNullIfEmpty(mtbTelefone.Text));
            cmd.Parameters.AddWithValue("@Cel", DbNullIfEmpty(mtbCelular.Text));
            cmd.Parameters.AddWithValue("@Email", DbNullIfEmpty(txtEmail.Text));
            cmd.Parameters.AddWithValue("@Site", DbNullIfEmpty(txtSite.Text));
            cmd.Parameters.AddWithValue("@Rua", DbNullIfEmpty(txtRua.Text));
            cmd.Parameters.AddWithValue("@Num", DbNullIfEmpty(txtNumero.Text));
            cmd.Parameters.AddWithValue("@Bairro", DbNullIfEmpty(txtBairro.Text));
            cmd.Parameters.AddWithValue("@Cidade", DbNullIfEmpty(txtCidade.Text));

            cmd.Parameters.AddWithValue("@Estado",
                cmbEstado.SelectedItem != null ? cmbEstado.SelectedItem.ToString() : "SP");

            cmd.Parameters.AddWithValue("@CEP", DbNullIfEmpty(mtbCEP.Text));

            cmd.Parameters.AddWithValue("@DataCad", dtpCadastro.Value);

            cmd.Parameters.AddWithValue("@Status",
                cmbStatus.SelectedItem != null
                    ? (cmbStatus.SelectedItem.ToString() == "Ativo")
                    : true);

            cmd.Parameters.AddWithValue("@Limite", nudLimite.Value);
            cmd.Parameters.AddWithValue("@CondPag",
                cmbPagamento.SelectedItem != null ? cmbPagamento.SelectedItem.ToString() : "À vista");
            cmd.Parameters.AddWithValue("@FormaPag",
                cmbFormaPagamento.SelectedItem != null ? cmbFormaPagamento.SelectedItem.ToString() : "Dinheiro");

            cmd.Parameters.AddWithValue("@Saldo", ConverterParaDecimal(txtSaldo.Text));
            cmd.Parameters.AddWithValue("@ValAtraso", ConverterParaDecimal(txtValorAtrasado.Text));
            cmd.Parameters.AddWithValue("@QtdAtraso", ConverterParaInt(txtQuantAtra.Text));

            cmd.Parameters.AddWithValue("@UltData", dtpDataUltima.Value);
            cmd.Parameters.AddWithValue("@UltValor", ConverterParaDecimal(txtValorUltima.Text));
            cmd.Parameters.AddWithValue("@Ticket", ConverterParaDecimal(lblTicket.Text));

            cmd.Parameters.AddWithValue("@Observacoes", DbNullIfEmpty(txtObsInt.Text));
            cmd.Parameters.AddWithValue("@ClassificacaoRisco",
                cmbRisco.SelectedItem != null ? cmbRisco.SelectedItem.ToString() : "Baixa");
            cmd.Parameters.AddWithValue("@RankingCliente", ConverterParaInt(lblRanking.Text));
            cmd.Parameters.AddWithValue("@PercentualParticipacao", prbPercentual.Value);
        }


        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (!clienteAtualId.HasValue)
            {
                MessageBox.Show("Selecione um cliente para excluir.");
                return;
            }

            if (MessageBox.Show("Deseja realmente excluir este cliente?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM Clientes WHERE ClienteId = @id", conn);
                    cmd.Parameters.AddWithValue("@id", clienteAtualId.Value);
                    cmd.ExecuteNonQuery();
                }

                _ =CarregarClientesAsync();
                LimparCampos();
            }
        }

        private void LimparCampos(Control parent = null)
        {
            if (parent == null) parent = this;

            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox txt)
                    txt.Clear();
                else if (ctrl is MaskedTextBox mtxt)
                    mtxt.Clear();
                else if (ctrl is ComboBox cmb)
                {
                    if (cmb.Name == "cmbEstado")
                        cmb.SelectedItem = "SP";
                    else if (cmb.Name == "cmbStatus")
                        cmb.SelectedItem = "Ativo";
                    else if (cmb.Name == "cmbPagamento")
                        cmb.SelectedItem = "À vista";
                    else if (cmb.Name == "cmbFormaPagamento")
                        cmb.SelectedItem = "Dinheiro";
                    else if (cmb.Name == "cmbRisco")
                        cmb.SelectedItem = "Baixa";
                    else
                        cmb.SelectedIndex = -1;
                }
                else if (ctrl is NumericUpDown nud)
                    nud.Value = 0;
                else if (ctrl is Label lbl &&
                        (lbl.Name == "lblTicket" || lbl.Name == "lblRanking"))
                    lbl.Text = "0";
                else if (ctrl is DataGridView dgv && dgv.Name != "dgvClientes")
                    dgv.DataSource = null;
                else if (ctrl is ProgressBar pb)
                    pb.Value = 0;

                // Continua percorrendo controles filhos
                if (ctrl.HasChildren)
                    LimparCampos(ctrl);
            }

            dtpCadastro.Value = DateTime.Today;
            dtpDataUltima.Value = DateTime.Today;
        }
    }
}