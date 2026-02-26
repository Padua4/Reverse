using ADGV;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using ClosedXML.Excel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormControle : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private int? controleAtualId;
        private DateTime? dataFiltro;
        private DataTable cacheVeiculos;
        private DateTime ultimaAtualizacaoVeiculos = DateTime.MinValue;
        private bool isFilteringEmpresa = false;
        private bool isLoadingEmpresa = false;
        private System.Windows.Forms.Timer searchTimer;
        private string statusLogisticaOriginal = string.Empty;
        private bool statusLogisticaAutomatico = false;
        private bool carregandoRegistro = false;
        private string statusLaudoOriginal = string.Empty;
        private bool statusLaudoAutomatico = false;
        private readonly object cacheLock = new object();
        private CancellationTokenSource selectionCancellation;

        private bool isBindingGrid = false;

        public ExpedicaoFormControle(int _usuarioId)
        {
            InitializeComponent();
            this.Load += FormControle_Load;

            searchTimer = new System.Windows.Forms.Timer();
            searchTimer.Interval = 300;
            searchTimer.Tick += SearchTimer_Tick;

            txtKM.TextChanged += (s, e) => AtualizarCustoTotal();
            txtCombustivel.TextChanged += (s, e) => AtualizarCustoTotal();
            txtCafeManha.TextChanged += (s, e) => AtualizarCustoTotal();
            txtAlimentacao.TextChanged += (s, e) => AtualizarCustoTotal();
            txtHoraExtra.TextChanged += (s, e) => AtualizarCustoTotal();

            cbLogistica.SelectedIndexChanged += cbLogistica_SelectedIndexChanged;
            cbLaudo.SelectedIndexChanged += cbLaudo_SelectedIndexChanged;
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
            ConfigurarGridControle();

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

            dtpData.ValueChanged += dtpData_ValueChanged;
            cmEmpresa.KeyUp += CmEmpresa_KeyUp;
            cmEmpresa.SelectedIndexChanged += cmEmpresa_SelectedIndexChanged;
            cbModeloVeiculo.SelectedIndexChanged += cbModeloVeiculo_SelectedIndexChanged;
        }

        private void ConfigurarGridControle()
        {
            dgvControle.RowHeadersVisible = false;
            dgvControle.BorderStyle = BorderStyle.FixedSingle;
            dgvControle.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvControle.EnableHeadersVisualStyles = false;
            dgvControle.MultiSelect = false;
            dgvControle.ReadOnly = true;
            dgvControle.AllowUserToAddRows = false;
            dgvControle.AllowUserToDeleteRows = false;
            dgvControle.AllowUserToResizeRows = false;
            dgvControle.EditMode = DataGridViewEditMode.EditProgrammatically;

            dgvControle.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvControle.DefaultCellStyle.ForeColor = Color.Black;
            dgvControle.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvControle.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvControle.BackgroundColor = Color.FromArgb(250, 250, 252);
            dgvControle.GridColor = Color.FromArgb(230, 230, 235);

            dgvControle.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvControle.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvControle.ColumnHeadersHeight = 40;
            dgvControle.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvControle.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvControle.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 3, 0, 3);

            dgvControle.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            dgvControle.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgvControle.RowsDefaultCellStyle.BackColor = Color.White;
            dgvControle.RowsDefaultCellStyle.ForeColor = Color.Black;

            dgvControle.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            dgvControle.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvControle.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgvControle.ColumnHeadersDefaultCellStyle.BackColor;
            dgvControle.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgvControle.DefaultCellStyle.Padding = new Padding(3, 5, 3, 5);

            dgvControle.RowTemplate.Height = 35;
            dgvControle.RowTemplate.MinimumHeight = 34;

            dgvControle.EnableHeadersVisualStyles = false;

            dgvControle.GridColor = Color.FromArgb(230, 230, 240);

            dgvControle.CellMouseEnter += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    dgvControle.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                    dgvControle.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
            };

            dgvControle.CellMouseLeave += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (e.RowIndex % 2 == 0)
                    {
                        dgvControle.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                        dgvControle.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        dgvControle.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
                        dgvControle.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            };


            dgvControle.SelectionChanged += (sender, e) =>
            {
                if (dgvControle.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvControle.Rows)
                    {
                        row.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    dgvControle.SelectedRows[0].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    dgvControle.SelectedRows[0].DefaultCellStyle.ForeColor = Color.Black;
                }
            };
        }

        private string CalcularStatusLogisticaAutomatico(DateTime dataRegistro)
        {
            DateTime hoje = DateTime.Today;

            if (dataRegistro.Date > hoje)
                return "PROGRAMADO";
            else if (dataRegistro.Date == hoje)
                return "EM EXECUÇÃO";
            else
                return "CONCLUÍDO";
        }

        private void cbLogistica_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!carregandoRegistro && statusLogisticaAutomatico)
            {
                if (cbLogistica.Text.ToUpper() != statusLogisticaOriginal.ToUpper())
                {
                    statusLogisticaAutomatico = false;
                }
            }
        }

        private void cbLaudo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!carregandoRegistro && statusLaudoAutomatico)
            {
                if (cbLaudo.Text.ToUpper() != statusLaudoOriginal.ToUpper())
                {
                    statusLaudoAutomatico = false;
                }
            }
        }

        private void CmEmpresa_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up ||
                e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab ||
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                return;
            }

            searchTimer.Stop();
            searchTimer.Start();
        }

        private void dtpData_ValueChanged(object sender, EventArgs e)
        {
            if (statusLogisticaAutomatico && !carregandoRegistro)
            {
                DateTime dataRegistro = dtpData.Value.Date;
                string statusAuto = CalcularStatusLogisticaAutomatico(dataRegistro);

                carregandoRegistro = true;
                cbLogistica.Text = statusAuto;
                statusLogisticaOriginal = statusAuto;
                carregandoRegistro = false;
            }
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            searchTimer.Stop();
            FiltrarEmpresas();
        }

        private void btnVeiculos_Click(object sender, EventArgs e)
        {
            var formVeiculos = new ExpedicaoFormVeiculos();
            formVeiculos.ShowDialog();
        }

        private void FiltrarEmpresas()
        {
            if (isFilteringEmpresa || cacheEmpresas == null) return;

            try
            {
                isFilteringEmpresa = true;

                string filtro = cmEmpresa.Text.Trim();

                if (string.IsNullOrWhiteSpace(filtro))
                {
                    isLoadingEmpresa = true;
                    cmEmpresa.DataSource = cacheEmpresas;
                    cmEmpresa.DisplayMember = "Nome";
                    cmEmpresa.ValueMember = "ClienteId";
                    cmEmpresa.SelectedIndex = -1;
                    isLoadingEmpresa = false;
                    return;
                }

                var resultados = cacheEmpresas.AsEnumerable()
                    .Where(row =>
                    {
                        var nome = row.Field<string>("Nome");
                        return nome != null && nome.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0;
                    })
                    .ToList();

                if (resultados.Any())
                {
                    DataTable dtFiltrado = cacheEmpresas.Clone();
                    foreach (var row in resultados)
                    {
                        dtFiltrado.ImportRow(row);
                    }

                    isLoadingEmpresa = true;

                    cmEmpresa.DataSource = dtFiltrado;
                    cmEmpresa.DisplayMember = "Nome";
                    cmEmpresa.ValueMember = "ClienteId";

                    cmEmpresa.SelectedIndex = -1;

                    isLoadingEmpresa = false;

                    cmEmpresa.Text = filtro;
                    cmEmpresa.SelectionStart = filtro.Length;
                    cmEmpresa.SelectionLength = 0;

                    if (!cmEmpresa.DroppedDown)
                        cmEmpresa.DroppedDown = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao filtrar empresas: {ex.Message}");
            }
            finally
            {
                isFilteringEmpresa = false;
            }
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

            if (cmbMotorista != null) cmbMotorista.SelectedIndex = -1;
            if (cmbAjudante1 != null) cmbAjudante1.SelectedIndex = -1;
            if (cmbAjudante2 != null) cmbAjudante2.SelectedIndex = -1;

            controleAtualId = null;
        }

        private DataTable cacheEmpresas;
        private DateTime ultimaAtualizacaoEmpresas = DateTime.MinValue;

        private async Task CarregarEmpresasAsync()
        {
            lock (cacheLock)
            {
                if (cacheEmpresas != null && (DateTime.Now - ultimaAtualizacaoEmpresas).TotalMinutes < 2)
                {
                    ConfigurarComboBoxEmpresa();
                    return;
                }
            }

            DataTable novoCache;

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand(
                    "SELECT TOP 500 ClienteId, Nome, Cidade, CodigoEmpresa " +
                    "FROM Clientes WITH (NOLOCK) " +
                    "WHERE Ativo = 1 " +
                    "ORDER BY Nome",
                    conn);
                cmd.CommandTimeout = 15;

                novoCache = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(novoCache);
                }
            }

            lock (cacheLock)
            {
                cacheEmpresas = novoCache;
                ultimaAtualizacaoEmpresas = DateTime.Now;
            }

            if (InvokeRequired)
            {
                Invoke(new Action(() => ConfigurarComboBoxEmpresa()));
            }
            else
            {
                ConfigurarComboBoxEmpresa();
            }
        }

        private void ConfigurarComboBoxEmpresa()
        {
            cmEmpresa.DataSource = cacheEmpresas;
            cmEmpresa.DisplayMember = "Nome";
            cmEmpresa.ValueMember = "ClienteId";

            cmEmpresa.AutoCompleteMode = AutoCompleteMode.None;
            cmEmpresa.AutoCompleteSource = AutoCompleteSource.None;
        }

        private void cmEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadingEmpresa || isFilteringEmpresa) return;

            if (cmEmpresa.SelectedItem is DataRowView row)
            {
                txtLocalidade.Text = row["Cidade"].ToString();
                txtCodigo.Text = row["CodigoEmpresa"].ToString();
            }
        }

        private async Task CarregarModelosVeiculoAsync()
        {
            if (cacheVeiculos != null && (DateTime.Now - ultimaAtualizacaoVeiculos).TotalMinutes < 5)
            {
                ConfigurarComboBoxVeiculo();
                return;
            }

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "SELECT VeiculoId, Modelo, Categoria FROM Veiculos WITH (NOLOCK) WHERE Ativo = 1 ORDER BY Modelo",
                    conn);

                cacheVeiculos = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(cacheVeiculos);
                }

                ultimaAtualizacaoVeiculos = DateTime.Now;
            }

            ConfigurarComboBoxVeiculo();
        }

        private void ConfigurarComboBoxVeiculo()
        {
            cbModeloVeiculo.DataSource = cacheVeiculos;
            cbModeloVeiculo.DisplayMember = "Modelo";
            cbModeloVeiculo.ValueMember = "VeiculoId";
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
                "EM EXECUÇÃO","CONCLUÍDO","NÃO EFETUADO","PROGRAMADO"
            });

            cbLaudo.Items.Clear();
            cbLaudo.Items.AddRange(new string[]
            {
                "EMITIDO","FOTOS","NÃO SE APLICA","AGUARDANDO","LIBERADO","BALANÇO DE MASSA"
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
                        SELECT TOP 500 
                            Id, Gerador, Localidade, Data, Ticket, ModeloVeiculo, 
                            Motorista, Ajudante1, Servico, StatusLogistica, StatusLaudo
                        FROM ControleLogistico WITH (NOLOCK)
                        WHERE 
                            (@DataFiltro IS NULL AND Data >= DATEADD(DAY, -50, CAST(GETDATE() AS DATE)))
                            OR (@DataFiltro IS NOT NULL AND CAST(Data AS DATE) = @DataFiltro)
                        ORDER BY Data DESC, Id DESC";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.Add("@DataFiltro", SqlDbType.Date).Value =
                            dataFiltro.HasValue ? (object)dataFiltro.Value.Date : DBNull.Value;

                        var dt = new DataTable();
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            isBindingGrid = true;
                            adapter.Fill(dt);
                        }

                        await AtualizarStatusAutomaticosEmLoteAsync(dt);

                        dgvControle.SuspendLayout();
                        _bindingSource.DataSource = dt;
                        ConfigurarColunasGrid();
                        dgvControle.ClearSelection();
                        dgvControle.ResumeLayout();

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


        private async Task AtualizarStatusAutomaticosEmLoteAsync(DataTable dt)
        {
            if (dt.Rows.Count == 0) return;

            var registrosParaAtualizar = new List<(int Id, string NovoStatus)>();
            DateTime hoje = DateTime.Today;

            foreach (DataRow row in dt.Rows)
            {
                if (row["Data"] == DBNull.Value) continue;

                DateTime dataRegistro = Convert.ToDateTime(row["Data"]).Date;
                string statusAtual = row["StatusLogistica"]?.ToString()?.ToUpper() ?? "";

                if (statusAtual == "NÃO EFETUADO")
                    continue;

                string statusEsperado = CalcularStatusLogisticaAutomatico(dataRegistro);

                if ((statusAtual == "PROGRAMADO" || statusAtual == "EM EXECUÇÃO" || statusAtual == "CONCLUÍDO")
                    && statusAtual != statusEsperado.ToUpper())
                {
                    registrosParaAtualizar.Add((Convert.ToInt32(row["Id"]), statusEsperado));
                    row["StatusLogistica"] = statusEsperado; 
                }
            }

            if (registrosParaAtualizar.Count > 0)
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    foreach (var (id, novoStatus) in registrosParaAtualizar)
                    {
                        var cmd = new SqlCommand(
                            "UPDATE ControleLogistico SET StatusLogistica = @Status WHERE Id = @Id",
                            conn);
                        cmd.Parameters.AddWithValue("@Status", novoStatus);
                        cmd.Parameters.AddWithValue("@Id", id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        private void ConfigurarColunasGrid()
        {
            if (dgvControle.Columns.Contains("Id"))
                dgvControle.Columns["Id"].Visible = false;
            if (dgvControle.Columns.Contains("Localidade"))
                dgvControle.Columns["Localidade"].Visible = false;

            var centerStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(2),
                ForeColor = Color.Black
            };

            var leftStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(5, 0, 0, 0),
                ForeColor = Color.Black
            };

            if (dgvControle.Columns.Contains("Gerador"))
            {
                dgvControle.Columns["Gerador"].HeaderText = "GERADOR";
                dgvControle.Columns["Gerador"].DefaultCellStyle = leftStyle;
                dgvControle.Columns["Gerador"].Width = 140;
            }

            if (dgvControle.Columns.Contains("Data"))
            {
                dgvControle.Columns["Data"].HeaderText = "DATA";
                dgvControle.Columns["Data"].DefaultCellStyle = centerStyle;
                dgvControle.Columns["Data"].Width = 90;
                dgvControle.Columns["Data"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }

            if (dgvControle.Columns.Contains("Ticket"))
            {
                dgvControle.Columns["Ticket"].HeaderText = "TICKET";
                dgvControle.Columns["Ticket"].DefaultCellStyle = centerStyle;
                dgvControle.Columns["Ticket"].Width = 90;
            }

            if (dgvControle.Columns.Contains("ModeloVeiculo"))
            {
                dgvControle.Columns["ModeloVeiculo"].HeaderText = "MODELO VEÍCULO";
                dgvControle.Columns["ModeloVeiculo"].DefaultCellStyle = leftStyle;
                dgvControle.Columns["ModeloVeiculo"].Width = 120;
            }

            if (dgvControle.Columns.Contains("Motorista"))
            {
                dgvControle.Columns["Motorista"].HeaderText = "MOTORISTA";
                dgvControle.Columns["Motorista"].DefaultCellStyle = leftStyle;
                dgvControle.Columns["Motorista"].Width = 120;
            }

            if (dgvControle.Columns.Contains("Ajudante1"))
            {
                dgvControle.Columns["Ajudante1"].HeaderText = "1º AJUDANTE";
                dgvControle.Columns["Ajudante1"].DefaultCellStyle = leftStyle;
                dgvControle.Columns["Ajudante1"].Width = 120;
            }

            if (dgvControle.Columns.Contains("Servico"))
            {
                dgvControle.Columns["Servico"].HeaderText = "SERVIÇO";
                dgvControle.Columns["Servico"].DefaultCellStyle = centerStyle;
                dgvControle.Columns["Servico"].Width = 110;
            }

            if (dgvControle.Columns.Contains("StatusLogistica"))
            {
                dgvControle.Columns["StatusLogistica"].HeaderText = "STATUS LOGÍSTICA";
                dgvControle.Columns["StatusLogistica"].DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    Padding = new Padding(2),
                    ForeColor = Color.Black
                };
                dgvControle.Columns["StatusLogistica"].Width = 130;
            }

            if (dgvControle.Columns.Contains("StatusLaudo"))
            {
                dgvControle.Columns["StatusLaudo"].HeaderText = "STATUS LAUDO";
                dgvControle.Columns["StatusLaudo"].DefaultCellStyle = centerStyle;
                dgvControle.Columns["StatusLaudo"].Width = 120;
            }

            if (dgvControle.Columns.Contains("CombustivelPedagio"))
            {
                dgvControle.Columns["CombustivelPedagio"].DefaultCellStyle.Format = "C2";
                dgvControle.Columns["CombustivelPedagio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvControle.Columns["CombustivelPedagio"].DefaultCellStyle.Padding = new Padding(0, 0, 5, 0);
                dgvControle.Columns["CombustivelPedagio"].DefaultCellStyle.ForeColor = Color.Black;
            }

            if (dgvControle.Columns.Contains("CafeManha"))
            {
                dgvControle.Columns["CafeManha"].DefaultCellStyle.Format = "C2";
                dgvControle.Columns["CafeManha"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvControle.Columns["CafeManha"].DefaultCellStyle.Padding = new Padding(0, 0, 5, 0);
                dgvControle.Columns["CafeManha"].DefaultCellStyle.ForeColor = Color.Black;
            }

            if (dgvControle.Columns.Contains("Alimentacao"))
            {
                dgvControle.Columns["Alimentacao"].DefaultCellStyle.Format = "C2";
                dgvControle.Columns["Alimentacao"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvControle.Columns["Alimentacao"].DefaultCellStyle.Padding = new Padding(0, 0, 5, 0);
                dgvControle.Columns["Alimentacao"].DefaultCellStyle.ForeColor = Color.Black;
            }

            if (dgvControle.Columns.Contains("HoraExtra"))
            {
                dgvControle.Columns["HoraExtra"].DefaultCellStyle.Format = "C2";
                dgvControle.Columns["HoraExtra"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvControle.Columns["HoraExtra"].DefaultCellStyle.Padding = new Padding(0, 0, 5, 0);
                dgvControle.Columns["HoraExtra"].DefaultCellStyle.ForeColor = Color.Black;
            }

            if (dgvControle.Columns.Contains("CustoTotal"))
            {
                dgvControle.Columns["CustoTotal"].DefaultCellStyle.Format = "C2";
                dgvControle.Columns["CustoTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvControle.Columns["CustoTotal"].DefaultCellStyle.Padding = new Padding(0, 0, 5, 0);
                dgvControle.Columns["CustoTotal"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                dgvControle.Columns["CustoTotal"].DefaultCellStyle.ForeColor = Color.Black;
            }

            if (dgvControle.Columns.Contains("KmPercorrido"))
            {
                dgvControle.Columns["KmPercorrido"].DefaultCellStyle.Format = "N0";
                dgvControle.Columns["KmPercorrido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvControle.Columns["KmPercorrido"].DefaultCellStyle.Padding = new Padding(0, 0, 5, 0);
                dgvControle.Columns["KmPercorrido"].DefaultCellStyle.ForeColor = Color.Black;
            }

            dgvControle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            if (dgvControle.Columns.Contains("Gerador"))
            {
                dgvControle.Columns["Gerador"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvControle.Columns["Gerador"].FillWeight = 25;
            }

            if (dgvControle.Columns.Contains("Observacoes"))
            {
                dgvControle.Columns["Observacoes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvControle.Columns["Observacoes"].FillWeight = 30;
                dgvControle.Columns["Observacoes"].DefaultCellStyle.ForeColor = Color.Black;
            }

            dgvControle.CellFormatting += DgvControle_CellFormatting;
        }

        private void DgvControle_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvControle.Columns[e.ColumnIndex].Name == "KmPercorrido" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out var km))
                    e.Value = $"{km:N0} km";
            }

            if (dgvControle.Columns[e.ColumnIndex].Name == "Data" && e.Value != null)
            {
                if (DateTime.TryParse(e.Value.ToString(), out DateTime data))
                {
                    e.Value = data.ToString("dd/MM/yyyy");
                    e.FormattingApplied = true;
                }
            }

            if (dgvControle.Columns[e.ColumnIndex].Name == "StatusLogistica" && e.Value != null)
            {
                string status = e.Value.ToString().ToUpper();
                Color backColor;

                switch (status)
                {
                    case "EM EXECUÇÃO":
                        backColor = Color.FromArgb(255, 255, 200);
                        break;
                    case "CONCLUÍDO":
                        backColor = Color.FromArgb(200, 255, 200);
                        break;
                    case "PROGRAMADO":
                        backColor = Color.FromArgb(200, 220, 255);
                        break;
                    case "NÃO EFETUADO":
                        backColor = Color.FromArgb(255, 200, 200);
                        break;
                    default:
                        backColor = Color.White;
                        break;
                }

                e.CellStyle.BackColor = backColor;
                e.CellStyle.ForeColor = Color.Black;
            }
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
            if (!string.IsNullOrWhiteSpace(cmEmpresa.Text) &&
                (cmEmpresa.SelectedValue == null || cmEmpresa.SelectedValue == DBNull.Value))
            {
                var resultado = MessageBox.Show(
                    $"A empresa '{cmEmpresa.Text}' foi digitada mas não foi selecionada da lista.\n\n" +
                    "Deseja continuar mesmo assim? (O ClienteId ficará NULL)",
                    "Atenção - Empresa não selecionada",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (resultado == DialogResult.No)
                    return;
            }

            DateTime dataSalva = dtpData.Value.Date;
            string statusParaSalvar;

            if (statusLogisticaAutomatico && cbLogistica.Text == statusLogisticaOriginal)
            {
                statusParaSalvar = CalcularStatusLogisticaAutomatico(dataSalva);
            }
            else
            {
                statusParaSalvar = cbLogistica.Text;
            }

            try
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

                    PreencherParametros(cmd, statusParaSalvar);
                    await cmd.ExecuteNonQueryAsync();
                }

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
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar registro: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PreencherParametros(SqlCommand cmd, string statusLogistica)
        {
            object clienteId = DBNull.Value;

            if (controleAtualId.HasValue)
            {
                using (var connTemp = new SqlConnection(connectionString))
                {
                    connTemp.Open();
                    var cmdBusca = new SqlCommand(
                        "SELECT ClienteId FROM ControleLogistico WHERE Id = @Id",
                        connTemp);
                    cmdBusca.Parameters.AddWithValue("@Id", controleAtualId.Value);

                    var resultado = cmdBusca.ExecuteScalar();
                    if (resultado != null && resultado != DBNull.Value)
                    {
                        clienteId = resultado;
                        System.Diagnostics.Debug.WriteLine(
                            $"🔒 UPDATE - ClienteId BLOQUEADO: {clienteId} (Id: {controleAtualId.Value})");
                    }
                    else
                    {
                        clienteId = DBNull.Value;
                    }
                }
            }
            else
            {
                if (cmEmpresa.SelectedValue != null && cmEmpresa.SelectedValue != DBNull.Value)
                {
                    clienteId = cmEmpresa.SelectedValue;
                }
                else if (!string.IsNullOrWhiteSpace(cmEmpresa.Text))
                {
                    if (cacheEmpresas != null)
                    {
                        var empresa = cacheEmpresas.AsEnumerable()
                            .FirstOrDefault(row =>
                                row.Field<string>("Nome")?.Equals(cmEmpresa.Text, StringComparison.OrdinalIgnoreCase) == true);

                        if (empresa != null)
                        {
                            clienteId = empresa["ClienteId"];
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine(
                    $"➕ INSERT - Novo ClienteId: {clienteId}");
            }

            cmd.Parameters.AddWithValue("@Gerador", (object)cmEmpresa.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ClienteId", clienteId);

            cmd.Parameters.AddWithValue("@Localidade", (object)txtLocalidade.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Data", dtpData.Value);
            cmd.Parameters.AddWithValue("@Ticket", (object)txtTicket.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NF", (object)txtNF.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MTR", (object)txtMTR.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Codigo", (object)txtCodigo.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Lote", (object)txtLote.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Volume", (object)txtVolume.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Obs", (object)txtObs.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ModeloVeiculo", (object)cbModeloVeiculo.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@VeiculoId",
                cbModeloVeiculo.SelectedValue != null && cbModeloVeiculo.SelectedValue != DBNull.Value
                    ? cbModeloVeiculo.SelectedValue
                    : DBNull.Value);
            cmd.Parameters.AddWithValue("@TipoVeiculo", (object)txtVeiculo.Text ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@Motorista",
                !string.IsNullOrWhiteSpace(cmbMotorista?.Text) ? cmbMotorista.Text : string.Empty);
            cmd.Parameters.AddWithValue("@Ajudante1",
                !string.IsNullOrWhiteSpace(cmbAjudante1?.Text) ? cmbAjudante1.Text : string.Empty);
            cmd.Parameters.AddWithValue("@Ajudante2",
                !string.IsNullOrWhiteSpace(cmbAjudante2?.Text) ? cmbAjudante2.Text : string.Empty);

            cmd.Parameters.AddWithValue("@Servico", (object)cbServico.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Destino", (object)txtDestino.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Peso", ParseDecimalNullable(txtPeso.Text));
            cmd.Parameters.AddWithValue("@StatusLogistica", (object)statusLogistica ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StatusLaudo", (object)cbLaudo.Text ?? DBNull.Value);
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

            statusLogisticaAutomatico = true;

            carregandoRegistro = true;
            DateTime dataRegistro = dtpData.Value.Date;
            string statusAuto = CalcularStatusLogisticaAutomatico(dataRegistro);
            statusLogisticaOriginal = statusAuto;
            cbLogistica.Text = statusAuto;

            statusLaudoAutomatico = true;
            statusLaudoOriginal = "AGUARDANDO";
            cbLaudo.Text = "AGUARDANDO";

            carregandoRegistro = false;
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
            selectionCancellation?.Cancel();
            selectionCancellation = new CancellationTokenSource();
            var token = selectionCancellation.Token;

            try
            {
                await Task.Delay(50, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            if (isBindingGrid) return;

            var row = dgvControle.CurrentRow;
            if (row == null || row.IsNewRow) return;

            var idValue = row.Cells["Id"].Value;
            if (idValue == null || idValue == DBNull.Value) return;

            int novoId = Convert.ToInt32(idValue);

            if (controleAtualId.HasValue && controleAtualId.Value == novoId)
                return;

            controleAtualId = novoId;

            try
            {
                await CarregarRegistroCompletoAsync(controleAtualId.Value, token);
            }
            catch (TaskCanceledException)
            {

            }
        }

        private async Task CarregarRegistroCompletoAsync(int id, CancellationToken cancellationToken = default)
        {
            carregandoRegistro = true;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync(cancellationToken);

                    string sql = @"SELECT * FROM ControleLogistico WITH (NOLOCK) WHERE Id = @Id";
                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.CommandTimeout = 10;

                    using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                    {
                        if (await reader.ReadAsync(cancellationToken))
                        {
                            if (cancellationToken.IsCancellationRequested)
                                return;

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

                            DateTime dataRegistro = reader["Data"] != DBNull.Value
                                ? Convert.ToDateTime(reader["Data"]).Date
                                : DateTime.Now.Date;
                            string statusBanco = reader["StatusLogistica"]?.ToString()?.ToUpper() ?? "";

                            string statusAuto = CalcularStatusLogisticaAutomatico(dataRegistro);
                            var statusAutomaticos = new[] { "PROGRAMADO", "EM EXECUÇÃO", "CONCLUÍDO" };

                            if (statusAutomaticos.Contains(statusBanco))
                            {
                                statusLogisticaAutomatico = true;
                                statusLogisticaOriginal = statusAuto;
                                cbLogistica.Text = statusAuto;
                            }
                            else
                            {
                                statusLogisticaAutomatico = false;
                                statusLogisticaOriginal = statusBanco;
                                cbLogistica.Text = statusBanco;
                            }

                            cbModeloVeiculo.Text = reader["ModeloVeiculo"]?.ToString() ?? "";
                            txtVeiculo.Text = reader["TipoVeiculo"]?.ToString() ?? "";

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

                            string statusLaudoBanco = reader["StatusLaudo"]?.ToString() ?? "";

                            if (statusLaudoBanco.ToUpper() == "AGUARDANDO" || statusLaudoBanco.ToUpper() == "EMITIDO")
                            {
                                statusLaudoAutomatico = true;
                                statusLaudoOriginal = statusLaudoBanco;
                                cbLaudo.Text = statusLaudoBanco;
                            }
                            else
                            {
                                statusLaudoAutomatico = false;
                                statusLaudoOriginal = statusLaudoBanco;
                                cbLaudo.Text = statusLaudoBanco;
                            }

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
            catch (OperationCanceledException)
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar registro: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                carregandoRegistro = false;
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
                DataTable dtMotoristas = new DataTable();

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        SELECT MotoristaId, NomeInterno, NomeCompleto, Funcao
                        FROM Motoristas
                        WHERE Ativo = 1 
                        AND Funcao IN ('Motorista', 'Transportadora')
                        ORDER BY Funcao, NomeInterno";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dtMotoristas);
                        }
                    }
                }

                if (cmbMotorista != null)
                {
                    DataRow rowVazio = dtMotoristas.NewRow();
                    rowVazio["NomeInterno"] = "";
                    rowVazio["MotoristaId"] = DBNull.Value;
                    dtMotoristas.Rows.InsertAt(rowVazio, 0);

                    cmbMotorista.DataSource = dtMotoristas;
                    cmbMotorista.DisplayMember = "NomeInterno";
                    cmbMotorista.ValueMember = "MotoristaId";
                    cmbMotorista.SelectedIndex = -1;
                }

                var dtAjudantes = ExpedicaoFormMotorista.ObterMotoristas("Ajudante");

                DataRow rowVazioAjudante = dtAjudantes.NewRow();
                rowVazioAjudante["NomeInterno"] = "";
                rowVazioAjudante["MotoristaId"] = DBNull.Value;
                dtAjudantes.Rows.InsertAt(rowVazioAjudante, 0);

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

        private async void btnExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bindingSource == null || _bindingSource.Count == 0)
                {
                    MessageBox.Show("Não há dados para exportar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var resultado = MessageBox.Show(
                    "Deseja exportar apenas os dados FILTRADOS?\n\n" +
                    "SIM = Exportar apenas dados filtrados/visíveis\n" +
                    "NÃO = Exportar TODOS os dados",
                    "Opção de Exportação",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Cancel)
                    return;

                bool exportarApenasFiltrados = (resultado == DialogResult.Yes);

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Arquivo Excel (*.xlsx)|*.xlsx";
                    sfd.Title = "Salvar Exportação";
                    sfd.FileName = $"ControleLogistico_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    btnExportarExcel.Enabled = false;
                    btnExportarExcel.Text = "Exportando...";
                    Cursor = Cursors.WaitCursor;

                    try
                    {
                        await Task.Run(() => ExportarParaExcel(sfd.FileName, exportarApenasFiltrados));

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

        private void ExportarParaExcel(string caminhoArquivo, bool apenasFiltrados)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Controle Logístico");

                DataTable dadosExportar = ObterDadosParaExportacao(apenasFiltrados);

                if (dadosExportar.Rows.Count == 0)
                {
                    throw new InvalidOperationException("Nenhum dado disponível para exportação.");
                }

                int colIndex = 1;
                var colunasVisiveis = ObterColunasVisiveis();

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
                            if (coluna.Name == "Data" && valor is DateTime data)
                            {
                                cell.Value = data;
                                cell.Style.DateFormat.Format = "dd/MM/yyyy";
                            }
                            else if (coluna.Name == "KmPercorrido" && decimal.TryParse(valor.ToString(), out var km))
                            {
                                cell.Value = km;
                                cell.Style.NumberFormat.Format = "#,##0";
                            }
                            else if ((coluna.Name == "CombustivelPedagio" ||
                                      coluna.Name == "CafeManha" ||
                                      coluna.Name == "Alimentacao" ||
                                      coluna.Name == "HoraExtra" ||
                                      coluna.Name == "CustoTotal") &&
                                     decimal.TryParse(valor.ToString(), out var vlrMoeda))
                            {
                                cell.Value = vlrMoeda;
                                cell.Style.NumberFormat.Format = "R$ #,##0.00";
                            }
                            else if (coluna.Name == "Peso" && decimal.TryParse(valor.ToString(), out var peso))
                            {
                                cell.Value = peso;
                                cell.Style.NumberFormat.Format = "#,##0.000";
                            }
                            else
                            {
                                cell.Value = valor.ToString();
                            }

                            if (coluna.Name == "StatusLogistica")
                            {
                                string status = valor.ToString().ToUpper();
                                switch (status)
                                {
                                    case "EM EXECUÇÃO":
                                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 200);
                                        break;
                                    case "CONCLUÍDO":
                                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(200, 255, 200);
                                        break;
                                    case "PROGRAMADO":
                                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(200, 220, 255);
                                        break;
                                    case "NÃO EFETUADO":
                                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 200, 200);
                                        break;
                                }
                                cell.Style.Font.Bold = true;
                            }
                        }

                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.OutsideBorderColor = XLColor.Gray;

                        if (rowIndex % 2 == 0)
                        {
                            if (coluna.Name != "StatusLogistica")
                            {
                                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(245, 249, 255);
                            }
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

                if (apenasFiltrados && !string.IsNullOrEmpty(dgvControle.FilterString))
                {
                    worksheet.Cell(footerRow + 1, 1).Value = $"Filtros aplicados: Sim";
                    worksheet.Cell(footerRow + 1, 1).Style.Font.Italic = true;
                    worksheet.Cell(footerRow + 1, 1).Style.Font.FontColor = XLColor.Gray;
                }

                worksheet.Cell(footerRow + 2, 1).Value = $"Total de registros: {dadosExportar.Rows.Count}";
                worksheet.Cell(footerRow + 2, 1).Style.Font.Bold = true;

                workbook.SaveAs(caminhoArquivo);
            }
        }

        private DataTable ObterDadosParaExportacao(bool apenasFiltrados)
        {
            if (!apenasFiltrados)
            {
                if (_bindingSource.DataSource is DataTable dt)
                {
                    return dt.Copy();
                }
            }

            DataTable resultado = (_bindingSource.DataSource as DataTable)?.Clone();

            if (resultado == null)
                return new DataTable();

            foreach (DataRowView rowView in _bindingSource)
            {
                resultado.ImportRow(rowView.Row);
            }

            return resultado;
        }

        private List<DataGridViewColumn> ObterColunasVisiveis()
        {
            var colunasVisiveis = new List<DataGridViewColumn>();

            foreach (DataGridViewColumn col in dgvControle.Columns)
            {
                if (col.Visible && col.Name != "Id")
                {
                    colunasVisiveis.Add(col);
                }
            }

            return colunasVisiveis.OrderBy(c => c.DisplayIndex).ToList();
        }
    }
}