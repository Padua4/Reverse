using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADGV;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormCadastro : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private int? clienteAtualId;
        private System.Threading.Timer debounceTimer;

        public ExpedicaoFormCadastro(int _usuarioId)
        {
            InitializeComponent();
            this.Load += FormClientes_Load;
            dgvClientes.SelectionChanged += dgvClientes_SelectionChanged;
            dgvClientes.CellFormatting += dgvClientes_CellFormatting;
            txtFiltro.TextChanged += txtFiltro_TextChanged;
            btnExportarExcel.Click += btnExportarExcel_Click;

            CarregarCombos();
            HabilitarCampos(false);
        }
        private BindingSource _bindingSourceClientes;

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
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            debounceTimer?.Dispose();
            base.OnFormClosing(e);
        }
        private void FormClientes_Load(object sender, EventArgs e)
        {
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.MultiSelect = false;
            dgvClientes.ReadOnly = true;
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.AllowUserToDeleteRows = false;
            dgvClientes.AllowUserToResizeRows = false;
            dgvClientes.EditMode = DataGridViewEditMode.EditProgrammatically;

            AplicarEstiloVisualProducao(dgvClientes);

            if (dgvClientes.Columns.Contains("ClienteId"))
                dgvClientes.Columns["ClienteId"].ReadOnly = true;

            _bindingSourceClientes = new BindingSource();
            dgvClientes.DataSource = _bindingSourceClientes;

            dgvClientes.FilterStringChanged += (s, ev) =>
            {
                _bindingSourceClientes.Filter = dgvClientes.FilterString;
            };

            dgvClientes.SortStringChanged += (s, ev) =>
            {
                _bindingSourceClientes.Sort = dgvClientes.SortString;
            };

            _ = CarregarClientesAsync();
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            debounceTimer?.Dispose();

            debounceTimer = new System.Threading.Timer(async _ =>
            {
                try
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(async () => await CarregarClientesAsync(txtFiltro.Text.Trim())));
                    }
                    else
                    {
                        await CarregarClientesAsync(txtFiltro.Text.Trim());
                    }
                }
                catch (ObjectDisposedException)
                {
                   
                }
                catch (Exception ex)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                            MessageBox.Show($"Erro ao filtrar: {ex.Message}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }
                }
            }, null, 500, System.Threading.Timeout.Infinite);
        }

        private void dgvClientes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvClientes.Columns[e.ColumnIndex].Name == "Codigo" && e.Value != null)
            {
                e.Value = e.Value.ToString().PadLeft(3, '0');
                e.FormattingApplied = true;
            }

            if (dgvClientes.Columns[e.ColumnIndex].Name == "CPF_CNPJ" && e.Value != null)
            {
                string digits = new string(e.Value.ToString().Where(char.IsDigit).ToArray());

                if (digits.Length == 14)
                {
                    e.Value = Convert.ToUInt64(digits).ToString(@"00\.000\.000\/0000\-00");
                    e.FormattingApplied = true;
                }
                else if (digits.Length == 11)
                {
                    e.Value = Convert.ToUInt64(digits).ToString(@"000\.000\.000\-00");
                    e.FormattingApplied = true;
                }
            }
        }

        private void HabilitarCampos(bool habilitar, Control parent = null)
        {
            if (parent == null) parent = this;

            foreach (Control ctrl in parent.Controls)
            {
                if ((ctrl is TextBox || ctrl is MaskedTextBox || ctrl is ComboBox)
                    && ctrl.Name != "txtFiltro")
                {
                    ctrl.Enabled = habilitar;
                }

                if (ctrl.HasChildren)
                    HabilitarCampos(habilitar, ctrl);
            }

            cbStatus.Enabled = habilitar; 
        }

        private async Task CarregarClientesAsync(string filtro = "")
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                SqlCommand cmd;

                if (string.IsNullOrEmpty(filtro))
                {
                    cmd = new SqlCommand(@"
                SELECT 
                    ClienteId,
                    CodigoEmpresa AS Codigo,
                    Nome,
                    CPF_CNPJ,
                    Cidade,
                    Estado
                FROM Clientes WITH (NOLOCK)
                ORDER BY CodigoEmpresa", conn);
                }
                else
                {
                    cmd = new SqlCommand(@"
                SELECT 
                    ClienteId,
                    CodigoEmpresa AS Codigo,
                    Nome,
                    CPF_CNPJ,
                    Cidade,
                    Estado
                FROM Clientes WITH (NOLOCK)
                WHERE CodigoEmpresa LIKE @Filtro 
                   OR Nome LIKE @Filtro 
                   OR CPF_CNPJ LIKE @Filtro
                   OR Cidade LIKE @Filtro 
                   OR Estado LIKE @Filtro
                ORDER BY CodigoEmpresa", conn);
                    cmd.Parameters.AddWithValue("@Filtro", $"%{filtro}%");
                }

                cmd.CommandTimeout = 30;

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                _bindingSourceClientes.DataSource = dt;

                ConfigurarColunasDataGridView();

                if (dgvClientes.Rows.Count > 0)
                    dgvClientes.Rows[0].Selected = true;
            }
        }


        private void ConfigurarColunasDataGridView()
        {
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dgvClientes.Columns.Contains("ClienteId"))
                dgvClientes.Columns["ClienteId"].Visible = false;

            if (dgvClientes.Columns.Contains("Codigo"))
            {
                dgvClientes.Columns["Codigo"].HeaderText = "Código";
                dgvClientes.Columns["Codigo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvClientes.Columns["Codigo"].Width = 90;
                dgvClientes.Columns["Codigo"].DefaultCellStyle.Font =
                    new Font("Segoe UI", 10F, FontStyle.Bold);
                dgvClientes.Columns["Codigo"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
                dgvClientes.Columns["Codigo"].HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
                dgvClientes.Columns["Codigo"].Resizable = DataGridViewTriState.False;
            }

            // Configurar coluna Nome
            if (dgvClientes.Columns.Contains("Nome"))
            {
                dgvClientes.Columns["Nome"].HeaderText = "Nome";
                dgvClientes.Columns["Nome"].FillWeight = 50;
                dgvClientes.Columns["Nome"].MinimumWidth = 140;
            }

            // Configurar coluna CNPJ
            if (dgvClientes.Columns.Contains("CPF_CNPJ"))
            {
                dgvClientes.Columns["CPF_CNPJ"].HeaderText = "CNPJ";
                dgvClientes.Columns["CPF_CNPJ"].FillWeight = 25;
                dgvClientes.Columns["CPF_CNPJ"].MinimumWidth = 130;
            }

            // Configurar coluna Cidade
            if (dgvClientes.Columns.Contains("Cidade"))
            {
                dgvClientes.Columns["Cidade"].HeaderText = "Município";
                dgvClientes.Columns["Cidade"].FillWeight = 20;
                dgvClientes.Columns["Cidade"].MinimumWidth = 120;
            }

            // Configurar coluna Estado
            if (dgvClientes.Columns.Contains("Estado"))
            {
                dgvClientes.Columns["Estado"].HeaderText = "UF";
                dgvClientes.Columns["Estado"].FillWeight = 10;
                dgvClientes.Columns["Estado"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleLeft;
                dgvClientes.Columns["Estado"].MinimumWidth = 35;
                dgvClientes.Columns["Estado"].HeaderCell.Style.Alignment =
                    DataGridViewContentAlignment.MiddleLeft;
            }

            dgvClientes.AllowUserToResizeColumns = true;
        }

        private void CarregarCombos()
        {
            cbStatus.Items.Clear();
            cbStatus.Items.AddRange(new string[] { "Ativo", "Inativo" });
            cbStatus.SelectedIndex = 0;
        }

        private async void dgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                var row = dgvClientes.CurrentRow;
                if (row == null || row.IsNewRow)
                {
                    LimparCampos();
                    clienteAtualId = null;
                    return;
                }
                var valorId = row.Cells["ClienteId"]?.Value;
                if (valorId == null || valorId == DBNull.Value)
                {
                    LimparCampos();
                    clienteAtualId = null;
                    return;
                }

                clienteAtualId = Convert.ToInt32(valorId);
                await CarregarDadosClienteAsync(clienteAtualId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados da seleção:\n{ex.Message}",
                                "SelectionChanged", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CarregarDadosClienteAsync(int clienteId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    SELECT 
                        Nome, RazaoSocial, CPF_CNPJ, RG_IE, ResponsavelComercial, Telefone, Celular, Email,
                        Rua, Numero, Bairro, Complemento, Cidade, Estado, Status,
                        CodigoEmpresa, NomeContato, Setor,
                        RuaEntrega, BairroEntrega, NumeroEntrega, ComplementoEntrega, MunicipioEntrega, EstadoEntrega
                    FROM Clientes WITH (NOLOCK)
                    WHERE ClienteId = @Id", conn);

                cmd.Parameters.AddWithValue("@Id", clienteId);
                cmd.CommandTimeout = 15; 

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        txtNome.Text = reader["Nome"]?.ToString() ?? "";
                        txtRazao.Text = reader["RazaoSocial"]?.ToString() ?? "";
                        txtCNPJ.Text = reader["CPF_CNPJ"]?.ToString() ?? "";
                        txtIE.Text = reader["RG_IE"]?.ToString() ?? "";
                        txtResponsavel.Text = reader["ResponsavelComercial"]?.ToString() ?? "";
                        txtTelefone.Text = reader["Telefone"]?.ToString() ?? "";
                        txtCelular.Text = reader["Celular"]?.ToString() ?? "";
                        txtEmail.Text = reader["Email"]?.ToString() ?? "";
                        txtCod.Text = reader["CodigoEmpresa"]?.ToString() ?? "";
                        txtNomeContato.Text = reader["NomeContato"]?.ToString() ?? "";
                        txtSetor.Text = reader["Setor"]?.ToString() ?? "";

                        txtRuaCad.Text = reader["Rua"]?.ToString() ?? "";
                        txtNumCad.Text = reader["Numero"]?.ToString() ?? "";
                        txtBairroCad.Text = reader["Bairro"]?.ToString() ?? "";
                        txtCompCad.Text = reader["Complemento"]?.ToString() ?? "";
                        txtMunCad.Text = reader["Cidade"]?.ToString() ?? "";
                        txtEstCad.Text = reader["Estado"]?.ToString() ?? "";

                        txtRuaEnt.Text = reader["RuaEntrega"]?.ToString() ?? "";
                        txtBairroEnt.Text = reader["BairroEntrega"]?.ToString() ?? "";
                        txtNumEnt.Text = reader["NumeroEntrega"]?.ToString() ?? "";
                        txtCompEnt.Text = reader["ComplementoEntrega"]?.ToString() ?? "";
                        txtMunEnt.Text = reader["MunicipioEntrega"]?.ToString() ?? "";
                        txtEstEnt.Text = reader["EstadoEntrega"]?.ToString() ?? "";

                        bool statusAtivo = reader["Status"] != DBNull.Value &&
                                           Convert.ToBoolean(reader["Status"]);
                        cbStatus.SelectedItem = statusAtivo ? "Ativo" : "Inativo";
                    }
                }
            }
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            LimparCampos();
            clienteAtualId = null;
            HabilitarCampos(true);
            txtNome.Focus();
        }

        private string GetOnlyDigits(string s)
        {
            return new string((s ?? "").Where(char.IsDigit).ToArray());
        }
        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            string cnpjDigits = GetOnlyDigits(txtCNPJ.Text.Trim());

            if (await CpfCnpjJaExisteAsync(cnpjDigits, clienteAtualId))
            {
                MessageBox.Show("Já existe um cliente com este CPF/CNPJ.",
                                "Duplicação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (clienteAtualId.HasValue)
            {
                await AtualizarClienteAsync();
                MessageBox.Show("Cliente atualizado com sucesso!");
            }
            else
            {
                clienteAtualId = await InserirClienteAsync();
                MessageBox.Show("Cliente cadastrado com sucesso!");
            }

            await CarregarClientesAsync();

            if (clienteAtualId.HasValue)
                await CarregarDadosClienteAsync(clienteAtualId.Value);

            HabilitarCampos(false);

        }
        private async Task<bool> CpfCnpjJaExisteAsync(string cpfCnpjDigits, int? clienteIdIgnorar = null)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpjDigits))
                return false;
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string sql = @"
            SELECT COUNT(*)
            FROM Clientes
            WHERE CPF_CNPJ = @cpf";

                if (clienteIdIgnorar.HasValue)
                    sql += " AND ClienteId != @id";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cpf", cpfCnpjDigits);
                if (clienteIdIgnorar.HasValue)
                    cmd.Parameters.AddWithValue("@id", clienteIdIgnorar.Value);

                int count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
        }
        private bool ValidarCampos()
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(txtNome.Text))
                erros.Add("O campo Nome é obrigatório.");

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
        private void LimparCampos(Control parent = null)
        {
            if (parent == null) parent = this;

            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox txt) txt.Clear();
                else if (ctrl is MaskedTextBox mtxt) mtxt.Clear();
                else if (ctrl is ComboBox cmb) cmb.SelectedIndex = -1;

                if (ctrl.HasChildren) LimparCampos(ctrl);
            }

            cbStatus.SelectedItem = "Ativo";
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos();
            clienteAtualId = null;

            dgvClientes.ReadOnly = true;
            dgvClientes.EndEdit();
            dgvClientes.ClearSelection();
            HabilitarCampos(false);
        }
        private async void btnExcluir_Click(object sender, EventArgs e)
        {
            var row = dgvClientes.CurrentRow ?? (dgvClientes.SelectedRows.Count > 0 ? dgvClientes.SelectedRows[0] : null);
            if (row == null)
            {
                MessageBox.Show("Selecione um cliente para excluir.");
                return;
            }

            if (row.Cells["ClienteId"].Value == null || row.Cells["ClienteId"].Value == DBNull.Value)
            {
                MessageBox.Show("Registro sem ID não pode ser excluído.");
                return;
            }

            int id = Convert.ToInt32(row.Cells["ClienteId"].Value);

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    var checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM ContasReceber WITH (NOLOCK) WHERE ClienteId = @Id", conn);
                    checkCmd.Parameters.AddWithValue("@Id", id);
                    checkCmd.CommandTimeout = 10;

                    int count = (int)await checkCmd.ExecuteScalarAsync();
                    if (count > 0)
                    {
                        MessageBox.Show(
                            "Não é possível excluir este cliente, pois ele possui registros em Contas a Receber.",
                            "Exclusão bloqueada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                var confirm = MessageBox.Show("Deseja realmente excluir este cliente?",
                                              "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand("DELETE FROM Clientes WHERE ClienteId=@Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.CommandTimeout = 10;
                    await cmd.ExecuteNonQueryAsync();
                }

                await CarregarClientesAsync();
                MessageBox.Show("Cliente excluído com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir cliente: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null || dgvClientes.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Selecione um cliente válido para editar.");
                return;
            }

            var valorId = dgvClientes.CurrentRow.Cells["ClienteId"]?.Value;
            if (valorId == null || valorId == DBNull.Value)
            {
                MessageBox.Show("Cliente sem ID não pode ser editado.");
                return;
            }

            clienteAtualId = Convert.ToInt32(valorId);
            await CarregarDadosClienteAsync(clienteAtualId.Value);

            HabilitarCampos(true);
            txtNome.Focus();
        }
        private async Task<int> InserirClienteAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var cmd = new SqlCommand(@"
                    INSERT INTO Clientes 
                        (Nome, RazaoSocial, CPF_CNPJ, RG_IE, ResponsavelComercial, Telefone, Celular, Email,
                         Rua, Numero, Bairro, Complemento, Cidade, Estado, Status, DataCadastro,
                         CodigoEmpresa, NomeContato, Setor,
                         RuaEntrega, BairroEntrega, NumeroEntrega, ComplementoEntrega, MunicipioEntrega, EstadoEntrega)
                        OUTPUT INSERTED.ClienteId
                        VALUES (@Nome, @RazaoSocial, @CNPJ, @IE, @Resp, @Tel, @Cel, @Email,
                                @Rua, @Numero, @Bairro, @Complemento, @Cidade, @Estado, @Status, GETDATE(),
                                @Cod, @NomeContato, @Setor,
                                @RuaEnt, @BairroEnt, @NumEnt, @CompEnt, @MunEnt, @EstEnt)", conn, transaction);

                        PreencherParametros(cmd);
                        int novoId = (int)await cmd.ExecuteScalarAsync();

                        transaction.Commit();
                        return novoId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        private async Task AtualizarClienteAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var cmd = new SqlCommand(@"
                    UPDATE Clientes SET 
                        Nome=@Nome, RazaoSocial=@RazaoSocial, CPF_CNPJ=@CNPJ, RG_IE=@IE, ResponsavelComercial=@Resp,
                        Telefone=@Tel, Celular=@Cel, Email=@Email,
                        Rua=@Rua, Numero=@Numero, Bairro=@Bairro, Complemento=@Complemento, Cidade=@Cidade, Estado=@Estado,
                        Status=@Status, CodigoEmpresa=@Cod, NomeContato=@NomeContato, Setor=@Setor,
                        RuaEntrega=@RuaEnt, BairroEntrega=@BairroEnt, NumeroEntrega=@NumEnt, ComplementoEntrega=@CompEnt, 
                        MunicipioEntrega=@MunEnt, EstadoEntrega=@EstEnt
                    WHERE ClienteId=@Id", conn, transaction);

                        PreencherParametros(cmd);
                        cmd.Parameters.AddWithValue("@Id", clienteAtualId.Value);
                        await cmd.ExecuteNonQueryAsync();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        private async void btnExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bindingSourceClientes == null || _bindingSourceClientes.Count == 0)
                {
                    MessageBox.Show("Não há dados para exportar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var resultado = MessageBox.Show(
                    "Deseja exportar apenas os dados FILTRADOS?\n\n" +
                    "SIM = Exportar apenas dados filtrados/visíveis\n" +
                    "NÃO = Exportar TODOS os clientes",
                    "Opção de Exportação",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Cancel)
                    return;

                bool exportarApenasFiltrados = (resultado == DialogResult.Yes);

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Arquivo Excel (*.xlsx)|*.xlsx";
                    sfd.Title = "Salvar Exportação de Clientes";
                    sfd.FileName = $"Clientes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    btnExportarExcel.Enabled = false;
                    btnExportarExcel.Text = "Exportando...";
                    Cursor = Cursors.WaitCursor;

                    try
                    {
                        await Task.Run(() => ExportarClientesParaExcel(sfd.FileName, exportarApenasFiltrados));

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
                        btnExportarExcel.Text = "Exportar Excel";
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

        private void ExportarClientesParaExcel(string caminhoArquivo, bool apenasFiltrados)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Clientes");

                DataTable dadosExportar = ObterDadosClientesParaExportacao(apenasFiltrados);

                if (dadosExportar.Rows.Count == 0)
                {
                    throw new InvalidOperationException("Nenhum dado disponível para exportação.");
                }

                int colIndex = 1;
                var colunasVisiveis = ObterColunasVisiveisClientes();

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
                            if (coluna.Name == "Codigo")
                            {
                                cell.Value = valor.ToString().PadLeft(3, '0');
                                cell.Style.Font.Bold = true;
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }
                            else if (coluna.Name == "CPF_CNPJ")
                            {
                                string digits = new string(valor.ToString().Where(char.IsDigit).ToArray());

                                if (digits.Length == 14)
                                {
                                    cell.Value = Convert.ToUInt64(digits).ToString(@"00\.000\.000\/0000\-00");
                                }
                                else if (digits.Length == 11)
                                {
                                    cell.Value = Convert.ToUInt64(digits).ToString(@"000\.000\.000\-00");
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

                // ===== AJUSTAR LARGURAS DAS COLUNAS =====
                worksheet.Columns().AdjustToContents(5, 50);

                // ===== ADICIONAR FILTROS =====
                var range = worksheet.Range(1, 1, rowIndex - 1, colunasVisiveis.Count);
                range.SetAutoFilter();

                // ===== CONGELAR PRIMEIRA LINHA =====
                worksheet.SheetView.FreezeRows(1);

                // ===== ADICIONAR INFORMAÇÕES DE RODAPÉ =====
                int footerRow = rowIndex + 2;
                worksheet.Cell(footerRow, 1).Value = $"Exportado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cell(footerRow, 1).Style.Font.Italic = true;
                worksheet.Cell(footerRow, 1).Style.Font.FontColor = XLColor.Gray;

                if (apenasFiltrados && !string.IsNullOrEmpty(dgvClientes.FilterString))
                {
                    worksheet.Cell(footerRow + 1, 1).Value = "Filtros aplicados: Sim";
                    worksheet.Cell(footerRow + 1, 1).Style.Font.Italic = true;
                    worksheet.Cell(footerRow + 1, 1).Style.Font.FontColor = XLColor.Gray;
                }

                if (!string.IsNullOrWhiteSpace(txtFiltro.Text))
                {
                    worksheet.Cell(footerRow + 2, 1).Value = $"Busca aplicada: {txtFiltro.Text}";
                    worksheet.Cell(footerRow + 2, 1).Style.Font.Italic = true;
                    worksheet.Cell(footerRow + 2, 1).Style.Font.FontColor = XLColor.Gray;
                }

                worksheet.Cell(footerRow + 3, 1).Value = $"Total de registros: {dadosExportar.Rows.Count}";
                worksheet.Cell(footerRow + 3, 1).Style.Font.Bold = true;

                // Salvar arquivo
                workbook.SaveAs(caminhoArquivo);
            }
        }

        private DataTable ObterDadosClientesParaExportacao(bool apenasFiltrados)
        {
            if (!apenasFiltrados)
            {
                if (_bindingSourceClientes.DataSource is DataTable dt)
                {
                    return dt.Copy();
                }
            }

            DataTable resultado = (_bindingSourceClientes.DataSource as DataTable)?.Clone();

            if (resultado == null)
                return new DataTable();

            foreach (DataRowView rowView in _bindingSourceClientes)
            {
                resultado.ImportRow(rowView.Row);
            }

            return resultado;
        }

        private List<DataGridViewColumn> ObterColunasVisiveisClientes()
        {
            var colunasVisiveis = new List<DataGridViewColumn>();

            foreach (DataGridViewColumn col in dgvClientes.Columns)
            {
                if (col.Visible && col.Name != "ClienteId")
                {
                    colunasVisiveis.Add(col);
                }
            }

            return colunasVisiveis.OrderBy(c => c.DisplayIndex).ToList();
        }

        private void PreencherParametros(SqlCommand cmd)
        {
            string cnpjDigits = GetOnlyDigits(txtCNPJ.Text.Trim());

            cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
            cmd.Parameters.AddWithValue("@RazaoSocial", txtRazao.Text.Trim());
            if (string.IsNullOrWhiteSpace(cnpjDigits))
                cmd.Parameters.AddWithValue("@CNPJ", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@CNPJ", cnpjDigits);
            cmd.Parameters.AddWithValue("@IE", txtIE.Text.Trim());
            cmd.Parameters.AddWithValue("@Resp", txtResponsavel.Text.Trim());
            cmd.Parameters.AddWithValue("@Tel", txtTelefone.Text.Trim());
            cmd.Parameters.AddWithValue("@Cel", txtCelular.Text.Trim());
            cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@Cod", txtCod.Text.Trim());
            cmd.Parameters.AddWithValue("@NomeContato", txtNomeContato.Text.Trim());
            cmd.Parameters.AddWithValue("@Setor", txtSetor.Text.Trim());

            cmd.Parameters.AddWithValue("@Rua", txtRuaCad.Text.Trim());
            cmd.Parameters.AddWithValue("@Numero", txtNumCad.Text.Trim());
            cmd.Parameters.AddWithValue("@Bairro", txtBairroCad.Text.Trim());
            cmd.Parameters.AddWithValue("@Complemento", txtCompCad.Text.Trim());
            cmd.Parameters.AddWithValue("@Cidade", txtMunCad.Text.Trim());
            cmd.Parameters.AddWithValue("@Estado", txtEstCad.Text.Trim());

            cmd.Parameters.AddWithValue("@RuaEnt", txtRuaEnt.Text.Trim());
            cmd.Parameters.AddWithValue("@BairroEnt", txtBairroEnt.Text.Trim());
            cmd.Parameters.AddWithValue("@NumEnt", txtNumEnt.Text.Trim());
            cmd.Parameters.AddWithValue("@CompEnt", txtCompEnt.Text.Trim());
            cmd.Parameters.AddWithValue("@MunEnt", txtMunEnt.Text.Trim());
            cmd.Parameters.AddWithValue("@EstEnt", txtEstEnt.Text.Trim());

            bool status = cbStatus.SelectedItem != null && cbStatus.SelectedItem.ToString() == "Ativo";
            cmd.Parameters.AddWithValue("@Status", status);
        }

    }
}
