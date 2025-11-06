using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADGV;


namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormControle : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private int? controleAtualId;
        private DateTime? dataFiltro;

        private bool isBindingGrid = false;

        public ExpedicaoFormControle(int _usuarioId)
        {
            InitializeComponent();
            this.Load += FormControle_Load;

            txtKM.TextChanged += (s, e) => AtualizarCustoTotal();
            txtCombustivel.TextChanged += (s, e) => AtualizarCustoTotal();
            txtCafeManha.TextChanged += (s, e) => AtualizarCustoTotal();
            txtAlimentacao.TextChanged += (s, e) => AtualizarCustoTotal();
            txtHoraExtra.TextChanged += (s, e) => AtualizarCustoTotal();

            dgvControle.SelectionChanged += dgvControle_SelectionChanged;

        }

        public void SetFiltro(DateTime dataSelecionada)
        {
            dataFiltro = dataSelecionada.Date;
            _ = CarregarControleAsync();
        }

        private BindingSource _bindingSource;

        private async void FormControle_Load(object sender, EventArgs e)
        {
            dgvControle.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvControle.MultiSelect = false;
            dgvControle.ReadOnly = true;
            dgvControle.AllowUserToAddRows = false;
            dgvControle.AllowUserToDeleteRows = false;
            dgvControle.AllowUserToResizeRows = false;
            dgvControle.EditMode = DataGridViewEditMode.EditProgrammatically;

            txtCustoTotal.ReadOnly = true;
            txtCustoTotal.BackColor = SystemColors.Control;

            _bindingSource = new BindingSource();
            dgvControle.DataSource = _bindingSource;

            dgvControle.FilterStringChanged += (s, ev) =>
            {
                _bindingSource.Filter = dgvControle.FilterString;
            };
            dgvControle.SortStringChanged += (s, ev) =>
            {
                _bindingSource.Sort = dgvControle.SortString;
            };

            await CarregarEmpresasAsync();
            await CarregarModelosVeiculoAsync();
            await CarregarControleAsync();
            CarregarCombosFixos();
            CarregarCombosMotoristas();

            AtualizarLabelFiltro();

            cmEmpresa.SelectedIndexChanged += cmEmpresa_SelectedIndexChanged;
            cbModeloVeiculo.SelectedIndexChanged += cbModeloVeiculo_SelectedIndexChanged;
        }

        private void btnVeiculos_Click(object sender, EventArgs e)
        {
            var formVeiculos = new ExpedicaoFormVeiculos();
            formVeiculos.ShowDialog();
        }

        private void btnGerarTicket_Click(object sender, EventArgs e)
        {
            if (controleAtualId.HasValue)
            {
                var formTicket = new ExpedicaoFormTickets(controleAtualId.Value);
                formTicket.TicketGerado += async () => await CarregarControleAsync();
                formTicket.ShowDialog();
            }
            else
            {
                MessageBox.Show("Selecione um registro na grid para gerar o ticket.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSelecionarMes_Click(object sender, EventArgs e)
        {
            using (var controleMes = new ExpedicaoFormControleMes())
            {
                if (controleMes.ShowDialog() == DialogResult.OK && controleMes.SelectedDate.HasValue)
                {
                    dataFiltro = controleMes.SelectedDate.Value.Date;
                    _ = CarregarControleAsync();
                    AtualizarLabelFiltro();
                }
            }
        }

        private void AtualizarLabelFiltro()
        {
            if (dataFiltro.HasValue)
            {
                lblFiltroAtivo.Text = $"Visualizando: {dataFiltro.Value:dd/MM/yyyy}";
                lblFiltroAtivo.Visible = true;
            }
            else
            {
                lblFiltroAtivo.Text = "Visualizando: Todos os registros";
                lblFiltroAtivo.Visible = true;
            }
        }

        private void LimparCampos(Control parent = null)
        {
            if (parent == null) parent = this;

            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox txt) txt.Clear();
                else if (ctrl is MaskedTextBox mtxt) mtxt.Clear();
                else if (ctrl is ComboBox cmb && cmb != cmEmpresa && cmb != cbModeloVeiculo &&
                         cmb != cbServico && cmb != cbLogistica && cmb != cbLaudo &&
                         cmb != cmbMotorista && cmb != cmbAjudante1 && cmb != cmbAjudante2)
                {
                    cmb.SelectedIndex = -1;
                }
                else if (ctrl is DataGridView) continue;

                if (ctrl.HasChildren) LimparCampos(ctrl);
            }

            cmEmpresa.SelectedIndex = -1;
            cbModeloVeiculo.SelectedIndex = -1;
            cbServico.SelectedIndex = -1;
            cbLogistica.SelectedIndex = -1;
            cbLaudo.SelectedIndex = -1;

            // Limpar as novas ComboBoxes
            if (cmbMotorista != null) cmbMotorista.SelectedIndex = -1;
            if (cmbAjudante1 != null) cmbAjudante1.SelectedIndex = -1;
            if (cmbAjudante2 != null) cmbAjudante2.SelectedIndex = -1;

            controleAtualId = null;
        }

        private DataTable cacheEmpresas;
        private DateTime ultimaAtualizacaoEmpresas = DateTime.MinValue;

        private async Task CarregarEmpresasAsync()
        {
            if (cacheEmpresas != null && (DateTime.Now - ultimaAtualizacaoEmpresas).TotalMinutes < 5)
            {
                cmEmpresa.DataSource = cacheEmpresas;
                cmEmpresa.DisplayMember = "Nome";
                cmEmpresa.ValueMember = "ClienteId";
                return;
            }

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "SELECT TOP 1000 ClienteId, Nome, Cidade, CodigoEmpresa FROM Clientes WHERE Ativo = 1 ORDER BY Nome",
                    conn);

                cacheEmpresas = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(cacheEmpresas);
                }

                cmEmpresa.DataSource = cacheEmpresas;
                cmEmpresa.DisplayMember = "Nome";
                cmEmpresa.ValueMember = "ClienteId";

                ultimaAtualizacaoEmpresas = DateTime.Now;
            }
        }

        private void cmEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmEmpresa.SelectedItem is DataRowView row)
            {
                txtLocalidade.Text = row["Cidade"].ToString();
                txtCodigo.Text = row["CodigoEmpresa"].ToString();
            }
        }

        private async Task CarregarModelosVeiculoAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT VeiculoId, Modelo, Categoria FROM Veiculos ORDER BY Modelo", conn);

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                cbModeloVeiculo.DataSource = dt;
                cbModeloVeiculo.DisplayMember = "Modelo";
                cbModeloVeiculo.ValueMember = "VeiculoId";
            }
        }

        private void cbModeloVeiculo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbModeloVeiculo.SelectedItem is DataRowView row)
            {
                txtVeiculo.Text = row["Categoria"].ToString();
            }
        }

        private void CarregarCombosFixos()
        {
            cbServico.Items.Clear();
            cbServico.Items.AddRange(new string[]
            {
                "COLETA ↓","ENTREGA →","RECEBIMENTO ↓","RETIRADA →","DESCARTE ↓","TRANSFERENCIA →"
            });

            cbLogistica.Items.Clear();
            cbLogistica.Items.AddRange(new string[]
            {
                "Em execução","Concluído","Não Efetuado","Programado"
            });

            cbLaudo.Items.Clear();
            cbLaudo.Items.AddRange(new string[]
            {
                "Emitido","Fotos","Não se aplica","Aguardando","Liberado","Balanço de massa"
            });
        }

        private async Task CarregarControleAsync()
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"
                    SELECT Id, Gerador, Localidade, Data, Ticket, ModeloVeiculo, 
                           Motorista, Ajudante1, Servico, StatusLogistica, StatusLaudo
                    FROM ControleLogistico WITH (NOLOCK)
                    WHERE (@DataFiltro IS NULL OR Data >= @DataInicio AND Data < @DataFim)
                    ORDER BY Data DESC, Id DESC";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        if (dataFiltro.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@DataInicio", dataFiltro.Value.Date);
                            cmd.Parameters.AddWithValue("@DataFim", dataFiltro.Value.Date.AddDays(1));
                            cmd.Parameters.AddWithValue("@DataFiltro", dataFiltro.Value.Date);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@DataInicio", DBNull.Value);
                            cmd.Parameters.AddWithValue("@DataFim", DBNull.Value);
                            cmd.Parameters.AddWithValue("@DataFiltro", DBNull.Value);
                        }

                        var dt = new DataTable();
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            isBindingGrid = true;
                            adapter.Fill(dt);
                        }

                        _bindingSource.DataSource = dt;

                        ConfigurarColunasGrid();

                        dgvControle.ClearSelection();
                        isBindingGrid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                isBindingGrid = false;
            }
        }

        private void ConfigurarColunasGrid()
        {
            if (dgvControle.Columns.Contains("Id"))
                dgvControle.Columns["Id"].Visible = false;
            if (dgvControle.Columns.Contains("Localidade"))
                dgvControle.Columns["Localidade"].Visible = false;

            dgvControle.Columns["Gerador"].HeaderText = "Gerador";
            dgvControle.Columns["Data"].HeaderText = "Data";
            dgvControle.Columns["Ticket"].HeaderText = "Ticket";
            dgvControle.Columns["ModeloVeiculo"].HeaderText = "Modelo Veículo";
            dgvControle.Columns["Motorista"].HeaderText = "Motorista";
            dgvControle.Columns["Ajudante1"].HeaderText = "1º Ajudante";
            dgvControle.Columns["Servico"].HeaderText = "Serviço";
            dgvControle.Columns["StatusLogistica"].HeaderText = "Status Logística";
            dgvControle.Columns["StatusLaudo"].HeaderText = "Status Laudo";

            // 🔹 Formatação de moeda
            if (dgvControle.Columns.Contains("CombustivelPedagio"))
                dgvControle.Columns["CombustivelPedagio"].DefaultCellStyle.Format = "C2";
            if (dgvControle.Columns.Contains("CafeManha"))
                dgvControle.Columns["CafeManha"].DefaultCellStyle.Format = "C2";
            if (dgvControle.Columns.Contains("Alimentacao"))
                dgvControle.Columns["Alimentacao"].DefaultCellStyle.Format = "C2";
            if (dgvControle.Columns.Contains("HoraExtra"))
                dgvControle.Columns["HoraExtra"].DefaultCellStyle.Format = "C2";
            if (dgvControle.Columns.Contains("CustoTotal"))
                dgvControle.Columns["CustoTotal"].DefaultCellStyle.Format = "C2";

            // 🔹 Formatação de KM
            if (dgvControle.Columns.Contains("KmPercorrido"))
                dgvControle.Columns["KmPercorrido"].DefaultCellStyle.Format = "N0";

            dgvControle.DefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            dgvControle.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvControle.DefaultCellStyle.ForeColor = Color.Black;
            dgvControle.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgvControle.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvControle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void dgvControle_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvControle.Columns[e.ColumnIndex].Name == "KmPercorrido" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out var km))
                    e.Value = $"{km:N0} km";
            }
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                SqlCommand cmd;
                if (controleAtualId.HasValue)
                {
                    cmd = new SqlCommand(@"
                UPDATE ControleLogistico SET
                    Gerador=@Gerador, ClienteId=@ClienteId, Localidade=@Localidade, Data=@Data, 
                    Ticket=@Ticket, NF=@NF, MTR=@MTR, Codigo=@Codigo, Lote=@Lote, Volume=@Volume, Observacoes=@Obs,
                    ModeloVeiculo=@ModeloVeiculo, VeiculoId=@VeiculoId, TipoVeiculo=@TipoVeiculo,
                    Motorista=@Motorista, Ajudante1=@Ajudante1, Ajudante2=@Ajudante2,
                    Servico=@Servico, Destino=@Destino, Peso=@Peso,
                    StatusLogistica=@StatusLogistica, StatusLaudo=@StatusLaudo,
                    KmPercorrido=@Km, CombustivelPedagio=@Combustivel, CafeManha=@Cafe,
                    Alimentacao=@Alimentacao, HoraExtra=@HoraExtra, CustoTotal=@CustoTotal
                WHERE Id=@Id", conn);
                    cmd.Parameters.AddWithValue("@Id", controleAtualId.Value);
                }
                else
                {
                    cmd = new SqlCommand(@"
                INSERT INTO ControleLogistico
                (Gerador, ClienteId, Localidade, Data, Ticket, NF, MTR, Codigo, Lote, Volume, Observacoes,
                 ModeloVeiculo, VeiculoId, TipoVeiculo, Motorista, Ajudante1, Ajudante2,
                 Servico, Destino, Peso, StatusLogistica, StatusLaudo,
                 KmPercorrido, CombustivelPedagio, CafeManha, Alimentacao, HoraExtra, CustoTotal)
                VALUES
                (@Gerador, @ClienteId, @Localidade, @Data, @Ticket, @NF, @MTR, @Codigo, @Lote, @Volume, @Obs,
                 @ModeloVeiculo, @VeiculoId, @TipoVeiculo, @Motorista, @Ajudante1, @Ajudante2,
                 @Servico, @Destino, @Peso, @StatusLogistica, @StatusLaudo,
                 @Km, @Combustivel, @Cafe, @Alimentacao, @HoraExtra, @CustoTotal)", conn);
                }

                PreencherParametros(cmd);
                await cmd.ExecuteNonQueryAsync();
            }

            DateTime dataSalva = dtpData.Value.Date;

            if (dataFiltro.HasValue && dataFiltro.Value.Date != dataSalva)
            {
                var resultado = MessageBox.Show(
                    $"Registro salvo com data {dataSalva:dd/MM/yyyy}.\n\n" +
                    $"O filtro atual está em {dataFiltro.Value:dd/MM/yyyy}.\n\n" +
                    "Deseja ajustar o filtro para ver o registro salvo?",
                    "Filtro Diferente",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    dataFiltro = dataSalva;
                    AtualizarLabelFiltro();
                }
            }
            else
            {
                MessageBox.Show("Registro salvo com sucesso!");
            }

            await CarregarControleAsync();
            LimparCampos();
        }

        private void PreencherParametros(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Gerador", cmEmpresa.Text);
            cmd.Parameters.AddWithValue("@ClienteId",
                cmEmpresa.SelectedValue != null ? cmEmpresa.SelectedValue : DBNull.Value);

            cmd.Parameters.AddWithValue("@Localidade", txtLocalidade.Text);
            cmd.Parameters.AddWithValue("@Data", dtpData.Value);
            cmd.Parameters.AddWithValue("@Ticket", txtTicket.Text);
            cmd.Parameters.AddWithValue("@NF", txtNF.Text);
            cmd.Parameters.AddWithValue("@MTR", txtMTR.Text);
            cmd.Parameters.AddWithValue("@Codigo", txtCodigo.Text);
            cmd.Parameters.AddWithValue("@Lote", txtLote.Text);
            cmd.Parameters.AddWithValue("@Volume", txtVolume.Text);
            cmd.Parameters.AddWithValue("@Obs", txtObs.Text);
            cmd.Parameters.AddWithValue("@ModeloVeiculo", cbModeloVeiculo.Text);

            cmd.Parameters.AddWithValue("@VeiculoId",
                cbModeloVeiculo.SelectedValue != null ? cbModeloVeiculo.SelectedValue : DBNull.Value);

            cmd.Parameters.AddWithValue("@TipoVeiculo", txtVeiculo.Text);

            cmd.Parameters.AddWithValue("@Motorista",
                cmbMotorista != null && cmbMotorista.Text != "" ? cmbMotorista.Text : "");
            cmd.Parameters.AddWithValue("@Ajudante1",
                cmbAjudante1 != null && cmbAjudante1.Text != "" ? cmbAjudante1.Text : "");
            cmd.Parameters.AddWithValue("@Ajudante2",
                cmbAjudante2 != null && cmbAjudante2.Text != "" ? cmbAjudante2.Text : "");

            cmd.Parameters.AddWithValue("@Servico", cbServico.Text);
            cmd.Parameters.AddWithValue("@Destino", txtDestino.Text);
            cmd.Parameters.AddWithValue("@Peso", ParseDecimalNullable(txtPeso.Text));
            cmd.Parameters.AddWithValue("@StatusLogistica", cbLogistica.Text);
            cmd.Parameters.AddWithValue("@StatusLaudo", cbLaudo.Text);
            cmd.Parameters.AddWithValue("@Km", ParseDecimalNullable(txtKM.Text));
            cmd.Parameters.AddWithValue("@Combustivel", ParseDecimalNullable(txtCombustivel.Text));
            cmd.Parameters.AddWithValue("@Cafe", ParseDecimalNullable(txtCafeManha.Text));
            cmd.Parameters.AddWithValue("@Alimentacao", ParseDecimalNullable(txtAlimentacao.Text));
            cmd.Parameters.AddWithValue("@HoraExtra", ParseDecimalNullable(txtHoraExtra.Text));
            cmd.Parameters.AddWithValue("@CustoTotal", ParseDecimalNullable(txtCustoTotal.Text));
        }

        private object ParseDecimalNullable(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return 0m;

            valor = valor.Replace("R$", "")
                         .Replace("km", "")
                         .Trim();

            if (decimal.TryParse(valor,
                System.Globalization.NumberStyles.Any,
                new System.Globalization.CultureInfo("pt-BR"),
                out var d))
            {
                return d;
            }
            return 0m;
        }

        private void txtPeso_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtPeso.Text, out decimal valor))
            {
                txtPeso.Text = valor.ToString("N3");
            }
            else
            {
                txtPeso.Text = "0,000";
            }
        }

        private void AtualizarCustoTotal()
        {
            decimal km = decimal.TryParse(txtKM.Text, out var v1) ? v1 : 0;

            decimal combustivel = ConverterParaDecimal(txtCombustivel.Text);
            decimal cafe = ConverterParaDecimal(txtCafeManha.Text);
            decimal alimentacao = ConverterParaDecimal(txtAlimentacao.Text);
            decimal horaExtra = ConverterParaDecimal(txtHoraExtra.Text);

            decimal total = combustivel + cafe + alimentacao + horaExtra;
            txtCustoTotal.Text = total.ToString("C2", new System.Globalization.CultureInfo("pt-BR"));
        }

        private decimal ConverterParaDecimal(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            valor = valor.Replace("R$", "")
                         .Replace("km", "")
                         .Trim();

            if (decimal.TryParse(valor,
                System.Globalization.NumberStyles.Any,
                new System.Globalization.CultureInfo("pt-BR"),
                out var d))
            {
                return d;
            }
            return 0;
        }

        private void FormatarComoMoeda(TextBox txt)
        {
            if (decimal.TryParse(txt.Text, out decimal valor))
                txt.Text = valor.ToString("C2", new System.Globalization.CultureInfo("pt-BR"));
            else
                txt.Text = 0m.ToString("C2", new System.Globalization.CultureInfo("pt-BR"));
        }

        private void txtKM_Leave(object sender, EventArgs e)
        {
            string texto = txtKM.Text.ToLower().Replace("km", "").Trim();

            if (decimal.TryParse(texto,
                System.Globalization.NumberStyles.Any,
                new System.Globalization.CultureInfo("pt-BR"),
                out decimal valor))
            {
                txtKM.Text = $"{valor:N0} km";
            }
            else
            {
                txtKM.Text = "0 km";
            }
        }

        private void txtCombustivel_Leave(object sender, EventArgs e) => FormatarComoMoeda(txtCombustivel);
        private void txtCafeManha_Leave(object sender, EventArgs e) => FormatarComoMoeda(txtCafeManha);
        private void txtAlimentacao_Leave(object sender, EventArgs e) => FormatarComoMoeda(txtAlimentacao);
        private void txtHoraExtra_Leave(object sender, EventArgs e) => FormatarComoMoeda(txtHoraExtra);

        private void btnNovo_Click(object sender, EventArgs e)
        {
            LimparCampos();
            controleAtualId = null;

            dtpData.Value = DateTime.Now.Date;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos();
            controleAtualId = null;
        }

        private async void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvControle.CurrentRow == null)
            {
                MessageBox.Show("Selecione um registro para excluir.");
                return;
            }

            var valorId = dgvControle.CurrentRow.Cells["Id"].Value;
            if (valorId == null || valorId == DBNull.Value)
            {
                MessageBox.Show("Registro inválido.");
                return;
            }

            int id = Convert.ToInt32(valorId);

            var confirm = MessageBox.Show("Deseja realmente excluir este registro?",
                                          "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM ControleLogistico WHERE Id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                await cmd.ExecuteNonQueryAsync();
            }

            await CarregarControleAsync();
            MessageBox.Show("Registro excluído com sucesso!");
        }

        private async void dgvControle_SelectionChanged(object sender, EventArgs e)
        {
            if (isBindingGrid) return;

            var row = dgvControle.CurrentRow;
            if (row == null || row.IsNewRow) return;

            var idValue = row.Cells["Id"].Value;
            if (idValue == null || idValue == DBNull.Value) return;

            controleAtualId = Convert.ToInt32(idValue);

            await CarregarRegistroCompletoAsync(controleAtualId.Value);
        }

        private async Task CarregarRegistroCompletoAsync(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"SELECT * FROM ControleLogistico WHERE Id = @Id";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cmEmpresa.Text = reader["Gerador"]?.ToString() ?? "";
                        txtLocalidade.Text = reader["Localidade"]?.ToString() ?? "";
                        dtpData.Value = reader["Data"] != DBNull.Value
                            ? Convert.ToDateTime(reader["Data"])
                            : DateTime.Now;

                        txtTicket.Text = reader["Ticket"]?.ToString() ?? "";
                        txtNF.Text = reader["NF"]?.ToString() ?? "";
                        txtMTR.Text = reader["MTR"]?.ToString() ?? "";
                        txtCodigo.Text = reader["Codigo"]?.ToString() ?? "";
                        txtLote.Text = reader["Lote"]?.ToString() ?? "";
                        txtVolume.Text = reader["Volume"]?.ToString() ?? "";
                        txtObs.Text = reader["Observacoes"]?.ToString() ?? "";

                        cbModeloVeiculo.Text = reader["ModeloVeiculo"]?.ToString() ?? "";
                        txtVeiculo.Text = reader["TipoVeiculo"]?.ToString() ?? "";

                        // MODIFICAR estas linhas para usar as ComboBoxes
                        if (cmbMotorista != null)
                            cmbMotorista.Text = reader["Motorista"]?.ToString() ?? "";
                        if (cmbAjudante1 != null)
                            cmbAjudante1.Text = reader["Ajudante1"]?.ToString() ?? "";
                        if (cmbAjudante2 != null)
                            cmbAjudante2.Text = reader["Ajudante2"]?.ToString() ?? "";

                        cbServico.Text = reader["Servico"]?.ToString() ?? "";
                        txtDestino.Text = reader["Destino"]?.ToString() ?? "";
                        txtPeso.Text = reader["Peso"] != DBNull.Value
                            ? Convert.ToDecimal(reader["Peso"]).ToString("N3")
                            : "";

                        cbLogistica.Text = reader["StatusLogistica"]?.ToString() ?? "";
                        cbLaudo.Text = reader["StatusLaudo"]?.ToString() ?? "";

                        txtKM.Text = reader["KmPercorrido"] != DBNull.Value
                            ? $"{Convert.ToDecimal(reader["KmPercorrido"]):N0} km"
                            : "0 km";

                        txtCombustivel.Text = reader["CombustivelPedagio"] != DBNull.Value
                            ? ((decimal)reader["CombustivelPedagio"]).ToString("C2", new System.Globalization.CultureInfo("pt-BR"))
                            : "R$ 0,00";

                        txtCafeManha.Text = reader["CafeManha"] != DBNull.Value
                            ? ((decimal)reader["CafeManha"]).ToString("C2", new System.Globalization.CultureInfo("pt-BR"))
                            : "R$ 0,00";

                        txtAlimentacao.Text = reader["Alimentacao"] != DBNull.Value
                            ? ((decimal)reader["Alimentacao"]).ToString("C2", new System.Globalization.CultureInfo("pt-BR"))
                            : "R$ 0,00";

                        txtHoraExtra.Text = reader["HoraExtra"] != DBNull.Value
                            ? ((decimal)reader["HoraExtra"]).ToString("C2", new System.Globalization.CultureInfo("pt-BR"))
                            : "R$ 0,00";

                        txtCustoTotal.Text = reader["CustoTotal"] != DBNull.Value
                            ? ((decimal)reader["CustoTotal"]).ToString("C2", new System.Globalization.CultureInfo("pt-BR"))
                            : "R$ 0,00";
                    }
                }
            }
        }

        private void btnMotorista_Click(object sender, EventArgs e)
        {
            var formMotorista = new ExpedicaoFormMotorista();

            formMotorista.MotoristasAtualizados += () =>
            {
                CarregarCombosMotoristas();
            };

            formMotorista.ShowDialog();
        }


        private void CarregarCombosMotoristas()
        {
            try
            {
                // Carregar Motoristas
                var dtMotoristas = ExpedicaoFormMotorista.ObterMotoristas("Motorista");

                if (cmbMotorista != null)
                {
                    cmbMotorista.DataSource = dtMotoristas;
                    cmbMotorista.DisplayMember = "NomeInterno";
                    cmbMotorista.ValueMember = "MotoristaId";
                    cmbMotorista.SelectedIndex = -1;
                }

                // Carregar Ajudantes
                var dtAjudantes = ExpedicaoFormMotorista.ObterMotoristas("Ajudante");

                if (cmbAjudante1 != null)
                {
                    cmbAjudante1.DataSource = dtAjudantes.Copy();
                    cmbAjudante1.DisplayMember = "NomeInterno";
                    cmbAjudante1.ValueMember = "MotoristaId";
                    cmbAjudante1.SelectedIndex = -1;
                }

                if (cmbAjudante2 != null)
                {
                    cmbAjudante2.DataSource = dtAjudantes.Copy();
                    cmbAjudante2.DisplayMember = "NomeInterno";
                    cmbAjudante2.ValueMember = "MotoristaId";
                    cmbAjudante2.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar motoristas/ajudantes: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}