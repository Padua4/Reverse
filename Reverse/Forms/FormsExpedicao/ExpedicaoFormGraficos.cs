using System;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormGraficos : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private int _usuarioId;
        private DataTable dtClientesCache;

        public ExpedicaoFormGraficos(int _usuarioId)
        {
            InitializeComponent();
            this._usuarioId = _usuarioId;

            this.Load += ExpedicaoFormGraficos_Load;
        }

        private async void ExpedicaoFormGraficos_Load(object sender, EventArgs e)
        {
            ConfigurarDateTimePickers();
            ConfigurarRadioButtons();
            ConfigurarDataGridViews();
            ConfigurarChart();
            ConfigurarBotoes();

            await CarregarClientesAsync();

            rbTodos.CheckedChanged += RadioButton_CheckedChanged;
            rbMulti.CheckedChanged += RadioButton_CheckedChanged;
            rbUnico.CheckedChanged += RadioButton_CheckedChanged;

            btnInserir.Click += BtnInserir_Click;
            btnRemover.Click += BtnRemover_Click;
            btnGVeiculos.Click += BtnGVeiculos_Click;
            btnGTickets.Click += BtnGTickets_Click;
            btnGStatusLaudo.Click += BtnGStatusLaudo_Click;
            btnGStatusLogistica.Click += BtnGStatusLogistica_Click;
            btnGStatusServico.Click += BtnGStatusServico_Click;
            btnGMotoristas.Click += BtnGMotoristas_Click;
            btnExportarPDF.Click += BtnExportarPDF_Click;
            btnGValoresMensais.Click += BtnGValoresMensais_Click;
            btnGViagens.Click += BtnGViagens_Click;
            btnGLancamentos.Click += BtnGLancamentos_Click;
            btnGMaterialVendido.Click += BtnGMaterialVendido_Click;
            btnGClientes.Click += BtnGClientes_Click;
            txtFiltro.TextChanged += TxtFiltro_TextChanged;

            rbTodos.Checked = true;
        }

        private void ConfigurarBotoes()
        {
            btnGStatusLogistica.BackColor = Color.FromArgb(52, 73, 94);
            btnGStatusLogistica.ForeColor = Color.White;
            btnGStatusLogistica.FlatStyle = FlatStyle.Flat;
            btnGStatusLogistica.FlatAppearance.BorderSize = 0;
            btnGStatusLogistica.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnGStatusLogistica.Cursor = Cursors.Hand;

            btnGStatusServico.BackColor = Color.FromArgb(52, 73, 94);
            btnGStatusServico.ForeColor = Color.White;
            btnGStatusServico.FlatStyle = FlatStyle.Flat;
            btnGStatusServico.FlatAppearance.BorderSize = 0;
            btnGStatusServico.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnGStatusServico.Cursor = Cursors.Hand;

            btnGStatusLaudo.BackColor = Color.FromArgb(52, 73, 94);
            btnGStatusLaudo.ForeColor = Color.White;
            btnGStatusLaudo.FlatStyle = FlatStyle.Flat;
            btnGStatusLaudo.FlatAppearance.BorderSize = 0;
            btnGStatusLaudo.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnGStatusLaudo.Cursor = Cursors.Hand;

            btnGTickets.BackColor = Color.FromArgb(52, 73, 94);
            btnGTickets.ForeColor = Color.White;
            btnGTickets.FlatStyle = FlatStyle.Flat;
            btnGTickets.FlatAppearance.BorderSize = 0;
            btnGTickets.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnGTickets.Cursor = Cursors.Hand;

            btnGMotoristas.BackColor = Color.FromArgb(52, 73, 94);
            btnGMotoristas.ForeColor = Color.White;
            btnGMotoristas.FlatStyle = FlatStyle.Flat;
            btnGMotoristas.FlatAppearance.BorderSize = 0;
            btnGMotoristas.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnGMotoristas.Cursor = Cursors.Hand;

            btnGVeiculos.BackColor = Color.FromArgb(52, 73, 94);
            btnGVeiculos.ForeColor = Color.White;
            btnGVeiculos.FlatStyle = FlatStyle.Flat;
            btnGVeiculos.FlatAppearance.BorderSize = 0;
            btnGVeiculos.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnGVeiculos.Cursor = Cursors.Hand;

            btnExportarPDF.BackColor = Color.FromArgb(76, 175, 80);
            btnExportarPDF.ForeColor = Color.White;
            btnExportarPDF.FlatStyle = FlatStyle.Flat;
            btnExportarPDF.FlatAppearance.BorderSize = 0;
            btnExportarPDF.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnExportarPDF.Cursor = Cursors.Hand;

            btnGValoresMensais.BackColor = Color.FromArgb(52, 73, 94);
            btnGValoresMensais.ForeColor = Color.White;
            btnGValoresMensais.FlatStyle = FlatStyle.Flat;
            btnGValoresMensais.FlatAppearance.BorderSize = 0;
            btnGValoresMensais.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnGValoresMensais.Cursor = Cursors.Hand;

            btnGViagens.BackColor = Color.FromArgb(52, 73, 94);
            btnGViagens.ForeColor = Color.White;
            btnGViagens.FlatStyle = FlatStyle.Flat;
            btnGViagens.FlatAppearance.BorderSize = 0;
            btnGViagens.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnGViagens.Cursor = Cursors.Hand;

            btnGLancamentos.BackColor = Color.FromArgb(52, 73, 94);
            btnGLancamentos.ForeColor = Color.White;
            btnGLancamentos.FlatStyle = FlatStyle.Flat;
            btnGLancamentos.FlatAppearance.BorderSize = 0;
            btnGLancamentos.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnGLancamentos.Cursor = Cursors.Hand;

            btnGMaterialVendido.BackColor = Color.FromArgb(52, 73, 94);
            btnGMaterialVendido.ForeColor = Color.White;
            btnGMaterialVendido.FlatStyle = FlatStyle.Flat;
            btnGMaterialVendido.FlatAppearance.BorderSize = 0;
            btnGMaterialVendido.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnGMaterialVendido.Cursor = Cursors.Hand;

            btnGClientes.BackColor = Color.FromArgb(52, 73, 94);
            btnGClientes.ForeColor = Color.White;
            btnGClientes.FlatStyle = FlatStyle.Flat;
            btnGClientes.FlatAppearance.BorderSize = 0;
            btnGClientes.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            btnGClientes.Cursor = Cursors.Hand;
        }

        private async void BtnGStatusServico_Click(object sender, EventArgs e)
        {
            HabilitarControlesCliente(true);

            await GerarGraficoStatusServicoAsync();
        }

        private async System.Threading.Tasks.Task<DataTable> BuscarDadosStatusServicoComClienteAsync(DateTime dataInicio, DateTime dataFinal, List<int> clienteIds)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                    SELECT 
                        cl.ClienteId,
                        c.Nome as ClienteNome,
                        cl.Servico
                    FROM ControleLogistico cl WITH (NOLOCK)
                    LEFT JOIN Clientes c ON c.ClienteId = cl.ClienteId
                    WHERE CAST(cl.Data AS DATE) BETWEEN @DataInicio AND @DataFinal
                    AND cl.Servico IS NOT NULL AND LTRIM(RTRIM(cl.Servico)) <> ''";

                if (clienteIds != null && clienteIds.Count > 0)
                {
                    sql += " AND cl.ClienteId IN (" + string.Join(",", clienteIds) + ")";
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFinal", dataFinal);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private async System.Threading.Tasks.Task<DataTable> BuscarDadosStatusLaudoComClienteAsync(DateTime dataInicio, DateTime dataFinal, List<int> clienteIds)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                    SELECT 
                        cl.ClienteId,
                        c.Nome as ClienteNome,
                        cl.StatusLaudo
                    FROM ControleLogistico cl WITH (NOLOCK)
                    LEFT JOIN Clientes c ON c.ClienteId = cl.ClienteId
                    WHERE CAST(cl.Data AS DATE) BETWEEN @DataInicio AND @DataFinal
                    AND cl.StatusLaudo IS NOT NULL AND cl.StatusLaudo <> ''";

                if (clienteIds != null && clienteIds.Count > 0)
                {
                    sql += " AND cl.ClienteId IN (" + string.Join(",", clienteIds) + ")";
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFinal", dataFinal);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private async System.Threading.Tasks.Task<DataTable> BuscarDadosTicketsAsync(DateTime dataInicio, DateTime dataFinal, List<int> clienteIds)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                    SELECT 
                        cl.ClienteId,
                        c.Nome as ClienteNome,
                        cl.Ticket,
                        cl.Data
                    FROM ControleLogistico cl WITH (NOLOCK)
                    LEFT JOIN Clientes c ON c.ClienteId = cl.ClienteId
                    WHERE CAST(cl.Data AS DATE) BETWEEN @DataInicio AND @DataFinal
                    AND cl.Ticket IS NOT NULL AND cl.Ticket <> ''";

                if (clienteIds != null && clienteIds.Count > 0)
                {
                    sql += " AND cl.ClienteId IN (" + string.Join(",", clienteIds) + ")";
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFinal", dataFinal);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private async System.Threading.Tasks.Task<DataTable> BuscarDadosVeiculosAsync(DateTime dataInicio, DateTime dataFinal)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                    SELECT ModeloVeiculo
                    FROM ControleLogistico WITH (NOLOCK)
                    WHERE CAST(Data AS DATE) BETWEEN @DataInicio AND @DataFinal
                    AND ModeloVeiculo IS NOT NULL AND ModeloVeiculo <> ''";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFinal", dataFinal);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private void TxtFiltro_TextChanged(object sender, EventArgs e)
        {
            if (dtClientesCache == null) return;

            string filtro = txtFiltro.Text.Trim();

            if (string.IsNullOrEmpty(filtro))
            {
                dgvClientes.DataSource = dtClientesCache;
            }
            else
            {
                DataView dv = dtClientesCache.DefaultView;
                dv.RowFilter = $"Nome LIKE '%{filtro}%' OR CodigoEmpresa LIKE '%{filtro}%'";
                dgvClientes.DataSource = dv.ToTable();
            }

            if (dgvClientes.Columns.Contains("ClienteId"))
                dgvClientes.Columns["ClienteId"].Visible = false;

            if (dgvClientes.Columns.Contains("CodigoEmpresa"))
            {
                dgvClientes.Columns["CodigoEmpresa"].HeaderText = "CÓDIGO";
                dgvClientes.Columns["CodigoEmpresa"].Width = 100;
            }

            if (dgvClientes.Columns.Contains("Nome"))
            {
                dgvClientes.Columns["Nome"].HeaderText = "NOME";
                dgvClientes.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        #region Configurações Iniciais

        private void ConfigurarDateTimePickers()
        {
            dtpInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpFinal.Value = DateTime.Now;

            dtpInicio.Format = DateTimePickerFormat.Short;
            dtpFinal.Format = DateTimePickerFormat.Short;
        }

        private void ConfigurarRadioButtons()
        {
            rbTodos.Checked = true;
            rbMulti.Checked = false;
            rbUnico.Checked = false;
        }

        private void ConfigurarDataGridViews()
        {
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.MultiSelect = false;
            dgvClientes.ReadOnly = true;
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.RowHeadersVisible = false;
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvClientesInseridos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientesInseridos.MultiSelect = false;
            dgvClientesInseridos.ReadOnly = true;
            dgvClientesInseridos.AllowUserToAddRows = false;
            dgvClientesInseridos.RowHeadersVisible = false;
            dgvClientesInseridos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            AplicarEstiloGrid(dgvClientes);
            AplicarEstiloGrid(dgvClientesInseridos);
        }

        private void AplicarEstiloGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.FromArgb(250, 250, 252);
            dgv.GridColor = Color.FromArgb(230, 230, 235);
            dgv.BorderStyle = BorderStyle.FixedSingle;

            dgv.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersHeight = 35;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            dgv.RowTemplate.Height = 30;
        }

        private void ConfigurarChart()
        {
            if (chartGrafico.ChartAreas.Count == 0)
            {
                var chartArea = new ChartArea("ChartArea1");
                chartArea.AxisX.LabelStyle.Angle = -45;
                chartArea.AxisX.Interval = 1;
                chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
                chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
                chartGrafico.ChartAreas.Add(chartArea);
            }
            else
            {
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chartGrafico.ChartAreas[0].AxisX.Interval = 1;
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            }

            if (chartGrafico.Legends.Count == 0)
            {
                var legend = new Legend("Legend1");
                legend.Docking = Docking.Top;
                legend.Alignment = StringAlignment.Center;
                chartGrafico.Legends.Add(legend);
            }

            chartGrafico.Titles.Clear();
            chartGrafico.Titles.Add(new Title("Selecione um tipo de gráfico", Docking.Top,
                new System.Drawing.Font("Segoe UI", 14F, FontStyle.Bold), Color.FromArgb(52, 73, 94)));
        }

        #endregion

        private async System.Threading.Tasks.Task GerarGraficoVeiculosAsync()
        {
            try
            {
                DateTime dataInicio = dtpInicio.Value.Date;
                DateTime dataFinal = dtpFinal.Value.Date;

                if (dataInicio > dataFinal)
                {
                    MessageBox.Show("A data inicial não pode ser maior que a data final!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dtDados = await BuscarDadosVeiculosAsync(dataInicio, dataFinal);

                if (dtDados.Rows.Count == 0)
                {
                    MessageBox.Show("Não há dados para o período selecionado!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                var topVeiculos = dtDados.AsEnumerable()
                    .Where(row => !string.IsNullOrWhiteSpace(row.Field<string>("ModeloVeiculo")))
                    .GroupBy(row => row.Field<string>("ModeloVeiculo"))
                    .Select(g => new
                    {
                        Veiculo = g.Key,
                        Quantidade = g.Count()
                    })
                    .OrderByDescending(x => x.Quantidade)
                    .Take(10)
                    .ToList();

                if (topVeiculos.Count == 0)
                {
                    MessageBox.Show("Não há veículos registrados no período!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                chartGrafico.Series.Clear();
                chartGrafico.Titles.Clear();
                chartGrafico.Legends.Clear();

                chartGrafico.Titles.Add(new Title(
                    $"Veículos Mais Utilizados\n{dataInicio:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}",
                    Docking.Top,
                    new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                    Color.FromArgb(52, 73, 94)));

                Series serie = new Series("Viagens");
                serie.ChartType = SeriesChartType.Column;
                serie.IsValueShownAsLabel = true;
                serie.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                serie.LabelForeColor = Color.Black;
                serie.BorderWidth = 2;
                serie.BorderColor = Color.Black;

                Color[] cores = new Color[]
                {
                    Color.FromArgb(0, 114, 178),
                    Color.FromArgb(230, 159, 0),
                    Color.FromArgb(0, 158, 115),
                    Color.FromArgb(204, 121, 167),
                    Color.FromArgb(86, 180, 233),
                    Color.FromArgb(213, 94, 0),
                    Color.FromArgb(240, 228, 66),
                    Color.FromArgb(0, 0, 0),
                    Color.FromArgb(128, 128, 128),
                    Color.FromArgb(102, 51, 153)
                };

                for (int i = 0; i < topVeiculos.Count; i++)
                {
                    var veiculo = topVeiculos[i];
                    int idx = serie.Points.AddXY(veiculo.Veiculo, veiculo.Quantidade);
                    serie.Points[idx].Color = cores[i % cores.Length];
                    serie.Points[idx].Label = veiculo.Quantidade.ToString();
                    serie.Points[idx].LabelForeColor = Color.Black;
                    serie.Points[idx].BorderColor = Color.Black;
                    serie.Points[idx].BorderWidth = 2;
                }

                chartGrafico.Series.Add(serie);

                chartGrafico.ChartAreas[0].AxisY.Title = "Quantidade de Viagens";
                chartGrafico.ChartAreas[0].AxisX.Title = "Veículo";
                chartGrafico.ChartAreas[0].AxisY.Minimum = 0;
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

                double maxValue = topVeiculos.Max(v => v.Quantidade);
                if (maxValue > 0)
                {
                    double roundedMax = Math.Ceiling(maxValue * 1.1 / 10) * 10;
                    if (roundedMax < 10) roundedMax = 10;

                    chartGrafico.ChartAreas[0].AxisY.Maximum = roundedMax;
                    chartGrafico.ChartAreas[0].AxisY.Interval = Math.Max(1, roundedMax / 10);
                    chartGrafico.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                }
                else
                {
                    chartGrafico.ChartAreas[0].AxisY.Interval = 1;
                }

                var novaLegenda = new Legend("LegendaVeiculos");
                novaLegenda.Docking = Docking.Top;
                novaLegenda.Alignment = StringAlignment.Near;
                novaLegenda.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                novaLegenda.ForeColor = Color.FromArgb(52, 73, 94);
                novaLegenda.BackColor = Color.Transparent;
                novaLegenda.BorderColor = Color.Transparent;
                novaLegenda.MaximumAutoSize = 25;
                novaLegenda.Position.Auto = false;
                novaLegenda.Position.X = 1;
                novaLegenda.Position.Y = 0.5f;
                novaLegenda.Position.Width = 40;
                novaLegenda.Position.Height = 15;

                chartGrafico.Legends.Add(novaLegenda);

                serie["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}\n\nStack: {ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void BtnGVeiculos_Click(object sender, EventArgs e)
        {
            HabilitarControlesCliente(false);
            await GerarGraficoVeiculosAsync();
        }

        private async System.Threading.Tasks.Task GerarGraficoTicketsAsync()
        {
            try
            {
                DateTime dataInicio = dtpInicio.Value.Date;
                DateTime dataFinal = dtpFinal.Value.Date;

                if (dataInicio > dataFinal)
                {
                    MessageBox.Show("A data inicial não pode ser maior que a data final!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<int> clienteIds = ObterClientesSelecionados();

                DataTable dtDados = await BuscarDadosTicketsAsync(dataInicio, dataFinal, clienteIds);

                if (dtDados.Rows.Count == 0)
                {
                    MessageBox.Show("Não há tickets para o período selecionado!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                chartGrafico.Series.Clear();
                chartGrafico.Titles.Clear();
                chartGrafico.Legends.Clear();

                string tituloCliente = ObterTituloCliente();
                chartGrafico.Titles.Add(new Title(
                    $"Tickets Gerados - {dataInicio:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}\n{tituloCliente}",
                    Docking.Top,
                    new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                    Color.FromArgb(52, 73, 94)));

                if (rbTodos.Checked || (rbUnico.Checked && dgvClientesInseridos.Rows.Count == 1))
                {
                    var ticketsPorData = dtDados.AsEnumerable()
                        .GroupBy(row => Convert.ToDateTime(row.Field<DateTime>("Data")).Date)
                        .Select(g => new
                        {
                            Data = g.Key,
                            Quantidade = g.Count()
                        })
                        .OrderBy(x => x.Data)
                        .ToList();

                    Series serie = new Series("Tickets");
                    serie.ChartType = SeriesChartType.Column;
                    serie.IsValueShownAsLabel = true;
                    serie.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                    serie.LabelForeColor = Color.Black;
                    serie.BorderWidth = 2;
                    serie.BorderColor = Color.Black;
                    serie.Color = Color.FromArgb(0, 114, 178);

                    foreach (var item in ticketsPorData)
                    {
                        int idx = serie.Points.AddXY(item.Data.ToString("dd/MM"), item.Quantidade);
                        serie.Points[idx].Label = item.Quantidade.ToString();
                        serie.Points[idx].LabelForeColor = Color.Black;
                    }

                    chartGrafico.Series.Add(serie);
                }
                else
                {
                    var ticketsPorCliente = dtDados.AsEnumerable()
                        .GroupBy(row => new
                        {
                            ClienteId = row.Field<int>("ClienteId"),
                            ClienteNome = row.Field<string>("ClienteNome") ?? "Cliente Desconhecido"
                        })
                        .Select(g => new
                        {
                            Cliente = g.Key.ClienteNome,
                            Quantidade = g.Count()
                        })
                        .OrderByDescending(x => x.Quantidade)
                        .Take(10)
                        .ToList();

                    if (ticketsPorCliente.Count == 0)
                    {
                        MessageBox.Show("Não há dados para os clientes selecionados!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }

                    Series serie = new Series("Tickets");
                    serie.ChartType = SeriesChartType.Column;
                    serie.IsValueShownAsLabel = true;
                    serie.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                    serie.LabelForeColor = Color.Black;
                    serie.BorderWidth = 2;
                    serie.BorderColor = Color.Black;

                    Color[] cores = new Color[]
                    {
                        Color.FromArgb(0, 114, 178),
                        Color.FromArgb(230, 159, 0),
                        Color.FromArgb(0, 158, 115),
                        Color.FromArgb(204, 121, 167),
                        Color.FromArgb(86, 180, 233),
                        Color.FromArgb(213, 94, 0),
                        Color.FromArgb(240, 228, 66),
                        Color.FromArgb(0, 0, 0),
                        Color.FromArgb(128, 128, 128),
                        Color.FromArgb(102, 51, 153)
                    };

                    for (int i = 0; i < ticketsPorCliente.Count; i++)
                    {
                        var item = ticketsPorCliente[i];
                        string nomeAbreviado = item.Cliente.Length > 20 ?
                            item.Cliente.Substring(0, 17) + "..." :
                            item.Cliente;

                        int idx = serie.Points.AddXY(nomeAbreviado, item.Quantidade);
                        serie.Points[idx].Color = cores[i % cores.Length];
                        serie.Points[idx].Label = item.Quantidade.ToString();
                        serie.Points[idx].LabelForeColor = Color.Black;
                    }

                    chartGrafico.Series.Add(serie);
                }

                chartGrafico.ChartAreas[0].AxisY.Title = "Quantidade de Tickets";
                chartGrafico.ChartAreas[0].AxisX.Title = rbTodos.Checked ? "Data" : "Cliente";
                chartGrafico.ChartAreas[0].AxisY.Minimum = 0;
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

                double maxValue = chartGrafico.Series[0].Points.Max(p => p.YValues[0]);
                if (maxValue > 0)
                {
                    double roundedMax = Math.Ceiling(maxValue * 1.1 / 10) * 10;
                    if (roundedMax < 10) roundedMax = 10;

                    chartGrafico.ChartAreas[0].AxisY.Maximum = roundedMax;
                    chartGrafico.ChartAreas[0].AxisY.Interval = Math.Max(1, roundedMax / 10);
                    chartGrafico.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                }
                else
                {
                    chartGrafico.ChartAreas[0].AxisY.Interval = 1;
                }

                var novaLegenda = new Legend("LegendaTickets");
                novaLegenda.Docking = Docking.Top;
                novaLegenda.Alignment = StringAlignment.Near;
                novaLegenda.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                novaLegenda.ForeColor = Color.FromArgb(52, 73, 94);
                novaLegenda.BackColor = Color.Transparent;
                novaLegenda.BorderColor = Color.Transparent;
                novaLegenda.MaximumAutoSize = 25;
                novaLegenda.Position.Auto = false;
                novaLegenda.Position.X = 1;
                novaLegenda.Position.Y = 0.5f;
                novaLegenda.Position.Width = 40;
                novaLegenda.Position.Height = 15;

                chartGrafico.Legends.Add(novaLegenda);

                chartGrafico.Series[0]["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}\n\nStack: {ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnGTickets_Click(object sender, EventArgs e)
        {
            HabilitarControlesCliente(true);

            await GerarGraficoTicketsAsync();
        }

        private async System.Threading.Tasks.Task GerarGraficoStatusLaudoAsync()
        {
            try
            {
                DateTime dataInicio = dtpInicio.Value.Date;
                DateTime dataFinal = dtpFinal.Value.Date;

                if (dataInicio > dataFinal)
                {
                    MessageBox.Show("A data inicial não pode ser maior que a data final!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<int> clienteIds = ObterClientesSelecionados();

                DataTable dtDados = await BuscarDadosStatusLaudoComClienteAsync(dataInicio, dataFinal, clienteIds);

                if (dtDados.Rows.Count == 0)
                {
                    MessageBox.Show("Não há dados para o período selecionado!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                chartGrafico.Series.Clear();
                chartGrafico.Titles.Clear();
                chartGrafico.Legends.Clear();

                string tituloCliente = ObterTituloCliente();
                chartGrafico.Titles.Add(new Title(
                    $"Status Laudo - {dataInicio:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}\n{tituloCliente}",
                    Docking.Top,
                    new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                    Color.FromArgb(52, 73, 94)));

                List<string> statusFixos = new List<string>
                {
                    "EMITIDO",
                    "FOTOS",
                    "NÃO SE APLICA",
                    "AGUARDANDO",
                    "LIBERADO",
                    "BALANÇO DE MASSA"
                };

                if (rbTodos.Checked || (rbUnico.Checked && dgvClientesInseridos.Rows.Count == 1))
                {
                    var dadosFiltrados = dtDados.AsEnumerable()
                        .Where(row => statusFixos.Contains((row.Field<string>("StatusLaudo") ?? "").ToUpper()))
                        .ToList();

                    if (dadosFiltrados.Count == 0)
                    {
                        MessageBox.Show("Não há dados com os status relevantes para o período!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }

                    var dadosAgrupados = dadosFiltrados
                        .GroupBy(row => row.Field<string>("StatusLaudo") ?? "")
                        .Select(g => new
                        {
                            Status = g.Key.ToUpper(),
                            Quantidade = g.Count()
                        })
                        .OrderBy(x => statusFixos.IndexOf(x.Status))
                        .ToList();

                    Series serie = new Series("Quantidade");
                    serie.ChartType = SeriesChartType.Column;
                    serie.IsValueShownAsLabel = true;
                    serie.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                    serie.LabelForeColor = Color.Black;
                    serie.BorderWidth = 2;
                    serie.BorderColor = Color.Black;

                    Dictionary<string, Color> coresStatus = new Dictionary<string, Color>
                    {
                        { "EMITIDO", Color.FromArgb(76, 175, 80) },           // Verde
                        { "FOTOS", Color.FromArgb(33, 150, 243) },            // Azul
                        { "NÃO SE APLICA", Color.FromArgb(158, 158, 158) },   // Cinza
                        { "AGUARDANDO", Color.FromArgb(255, 193, 7) },        // Amarelo
                        { "LIBERADO", Color.FromArgb(0, 188, 212) },          // Ciano
                        { "BALANÇO DE MASSA", Color.FromArgb(156, 39, 176) }  // Roxo
                    };

                    foreach (var status in statusFixos)
                    {
                        var item = dadosAgrupados.FirstOrDefault(d => d.Status == status);
                        int quantidade = item != null ? item.Quantidade : 0;

                        if (quantidade > 0)
                        {
                            int idx = serie.Points.AddXY(status, quantidade);

                            if (coresStatus.ContainsKey(status))
                            {
                                serie.Points[idx].Color = coresStatus[status];
                            }
                            else
                            {
                                serie.Points[idx].Color = Color.Gray;
                            }

                            serie.Points[idx].Label = quantidade.ToString();
                            serie.Points[idx].LabelForeColor = Color.Black;
                        }
                    }

                    if (serie.Points.Count > 0)
                    {
                        chartGrafico.Series.Add(serie);
                    }
                    else
                    {
                        MessageBox.Show("Não há dados com os status relevantes para o período!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }
                }
                else
                {
                    var dadosFiltrados = dtDados.AsEnumerable()
                        .Where(row => statusFixos.Contains((row.Field<string>("StatusLaudo") ?? "").ToUpper()))
                        .Select(row => new
                        {
                            ClienteId = row.Field<int>("ClienteId"),
                            ClienteNome = row.Field<string>("ClienteNome") ?? "Cliente Desconhecido",
                            Status = row.Field<string>("StatusLaudo") ?? ""
                        })
                        .ToList();

                    if (dadosFiltrados.Count == 0)
                    {
                        MessageBox.Show("Não há dados com os status relevantes para os clientes selecionados!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }

                    var clientes = dadosFiltrados
                        .GroupBy(x => new { x.ClienteId, x.ClienteNome })
                        .ToList();

                    var clientesOrdenados = clientes
                        .Select(c => new
                        {
                            Cliente = c,
                            Total = c.Count(),
                            Nome = c.Key.ClienteNome
                        })
                        .OrderByDescending(x => x.Total)
                        .Take(10)
                        .ToList();

                    if (clientesOrdenados.Count == 0)
                    {
                        MessageBox.Show("Não há dados para os clientes selecionados!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }

                    Color[] coresClientes = new Color[]
                    {
                        Color.FromArgb(0, 114, 178),
                        Color.FromArgb(230, 159, 0),
                        Color.FromArgb(0, 158, 115),
                        Color.FromArgb(204, 121, 167),
                        Color.FromArgb(86, 180, 233),
                        Color.FromArgb(213, 94, 0),
                        Color.FromArgb(240, 228, 66),
                        Color.FromArgb(0, 0, 0),
                        Color.FromArgb(128, 128, 128),
                        Color.FromArgb(102, 51, 153)
                    };

                    for (int i = 0; i < clientesOrdenados.Count; i++)
                    {
                        var clienteInfo = clientesOrdenados[i];
                        var cliente = clienteInfo.Cliente;

                        string nomeAbreviado = clienteInfo.Nome.Length > 20 ?
                            clienteInfo.Nome.Substring(0, 17) + "..." :
                            clienteInfo.Nome;

                        Series serieCliente = new Series(nomeAbreviado);
                        serieCliente.ChartType = SeriesChartType.Column;
                        serieCliente.IsValueShownAsLabel = true;
                        serieCliente.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
                        serieCliente.Color = coresClientes[i % coresClientes.Length];
                        serieCliente.LabelForeColor = Color.Black;
                        serieCliente.BorderColor = Color.Black;
                        serieCliente.BorderWidth = 2;

                        bool clienteTemDados = false;

                        foreach (var status in statusFixos)
                        {
                            int quantidade = cliente.Count(x => x.Status.ToUpper() == status);

                            if (quantidade > 0)
                            {
                                clienteTemDados = true;
                                int idx = serieCliente.Points.AddXY(status, quantidade);
                                serieCliente.Points[idx].Label = quantidade.ToString();
                                serieCliente.Points[idx].LabelForeColor = Color.Black;
                            }
                        }

                        if (clienteTemDados)
                        {
                            chartGrafico.Series.Add(serieCliente);
                        }
                    }

                    if (chartGrafico.Series.Count == 0)
                    {
                        MessageBox.Show("Nenhum dos clientes selecionados possui dados nos status relevantes!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }
                }

                chartGrafico.ChartAreas[0].AxisY.Title = "";
                chartGrafico.ChartAreas[0].AxisX.Title = "";
                chartGrafico.ChartAreas[0].AxisY.Minimum = 0;
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

                double maxValue = 0;
                foreach (var series in chartGrafico.Series)
                {
                    foreach (DataPoint point in series.Points)
                    {
                        if (point.YValues[0] > maxValue)
                        {
                            maxValue = point.YValues[0];
                        }
                    }
                }

                if (maxValue > 0)
                {
                    double roundedMax = Math.Ceiling(maxValue * 1.1 / 10) * 10;
                    if (roundedMax < 10) roundedMax = 10;

                    chartGrafico.ChartAreas[0].AxisY.Maximum = roundedMax;
                    chartGrafico.ChartAreas[0].AxisY.Interval = Math.Max(1, roundedMax / 10);
                    chartGrafico.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                }
                else
                {
                    chartGrafico.ChartAreas[0].AxisY.Interval = 1;
                }

                var novaLegenda = new Legend("LegendaLaudo");
                novaLegenda.Docking = Docking.Top;
                novaLegenda.Alignment = StringAlignment.Near;
                novaLegenda.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                novaLegenda.ForeColor = Color.FromArgb(52, 73, 94);
                novaLegenda.BackColor = Color.Transparent;
                novaLegenda.BorderColor = Color.Transparent;
                novaLegenda.MaximumAutoSize = 25;
                novaLegenda.Position.Auto = false;
                novaLegenda.Position.X = 1;
                novaLegenda.Position.Y = 0.5f;
                novaLegenda.Position.Width = 40;
                novaLegenda.Position.Height = 15;

                chartGrafico.Legends.Add(novaLegenda);

                if (chartGrafico.Series.Count > 1)
                {
                    chartGrafico.ChartAreas[0].AxisX.Interval = 1;

                    foreach (var series in chartGrafico.Series)
                    {
                        series["PointWidth"] = "0.8";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}\n\nStack: {ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnGStatusLaudo_Click(object sender, EventArgs e)
        {
            HabilitarControlesCliente(true);

            await GerarGraficoStatusLaudoAsync();
        }

        private async System.Threading.Tasks.Task GerarGraficoStatusServicoAsync()
        {
            try
            {
                DateTime dataInicio = dtpInicio.Value.Date;
                DateTime dataFinal = dtpFinal.Value.Date;

                if (dataInicio > dataFinal)
                {
                    MessageBox.Show("A data inicial não pode ser maior que a data final!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<int> clienteIds = ObterClientesSelecionados();

                DataTable dtDados = await BuscarDadosStatusServicoComClienteAsync(dataInicio, dataFinal, clienteIds);

                if (dtDados.Rows.Count == 0)
                {
                    MessageBox.Show("Não há dados para o período selecionado!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                chartGrafico.Series.Clear();
                chartGrafico.Titles.Clear();
                chartGrafico.Legends.Clear();

                string tituloCliente = ObterTituloCliente();
                chartGrafico.Titles.Add(new Title(
                    $"Status Serviço - {dataInicio:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}\n{tituloCliente}",
                    Docking.Top,
                    new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                    Color.FromArgb(52, 73, 94)));

                List<string> servicosFixos = new List<string>
                {
                    "COLETA ↓",
                    "ENTREGA →",
                    "RECEBIMENTO ↓",
                    "RETIRADA →",
                    "DESCARTE ↓",
                    "TRANSFERENCIA →"
                };

                if (rbTodos.Checked || (rbUnico.Checked && dgvClientesInseridos.Rows.Count == 1))
                {
                    var dadosFiltrados = dtDados.AsEnumerable()
                        .Where(row => !string.IsNullOrWhiteSpace(row.Field<string>("Servico")))
                        .ToList();

                    if (dadosFiltrados.Count == 0)
                    {
                        MessageBox.Show("Não há dados de serviços para o período!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }

                    var dadosAgrupados = dadosFiltrados
                        .GroupBy(row => row.Field<string>("Servico") ?? "")
                        .Select(g => new
                        {
                            Servico = g.Key,
                            Quantidade = g.Count()
                        })
                        .OrderByDescending(x => x.Quantidade)
                        .ToList();

                    Series serie = new Series("Quantidade");
                    serie.ChartType = SeriesChartType.Column;
                    serie.IsValueShownAsLabel = true;
                    serie.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                    serie.LabelForeColor = Color.Black;
                    serie.BorderWidth = 2;
                    serie.BorderColor = Color.Black;

                    Color[] coresServicos = new Color[]
                    {
                        Color.FromArgb(0, 114, 178),      // Azul
                        Color.FromArgb(230, 159, 0),      // Laranja
                        Color.FromArgb(0, 158, 115),      // Verde azulado
                        Color.FromArgb(204, 121, 167),    // Rosa
                        Color.FromArgb(86, 180, 233),     // Azul céu
                        Color.FromArgb(213, 94, 0)        // Vermelho alaranjado
                    };

                    for (int i = 0; i < dadosAgrupados.Count; i++)
                    {
                        var item = dadosAgrupados[i];
                        int idx = serie.Points.AddXY(item.Servico, item.Quantidade);
                        serie.Points[idx].Color = coresServicos[i % coresServicos.Length];
                        serie.Points[idx].Label = item.Quantidade.ToString();
                        serie.Points[idx].LabelForeColor = Color.Black;
                    }

                    chartGrafico.Series.Add(serie);
                }
                else
                {
                    var dadosFiltrados = dtDados.AsEnumerable()
                        .Where(row => !string.IsNullOrWhiteSpace(row.Field<string>("Servico")))
                        .Select(row => new
                        {
                            ClienteId = row.Field<int>("ClienteId"),
                            ClienteNome = row.Field<string>("ClienteNome") ?? "Cliente Desconhecido",
                            Servico = row.Field<string>("Servico") ?? ""
                        })
                        .ToList();

                    if (dadosFiltrados.Count == 0)
                    {
                        MessageBox.Show("Não há dados de serviços para os clientes selecionados!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }

                    var clientes = dadosFiltrados
                        .GroupBy(x => new { x.ClienteId, x.ClienteNome })
                        .ToList();

                    var clientesOrdenados = clientes
                        .Select(c => new
                        {
                            Cliente = c,
                            Total = c.Count(),
                            Nome = c.Key.ClienteNome
                        })
                        .OrderByDescending(x => x.Total)
                        .Take(10)
                        .ToList();

                    if (clientesOrdenados.Count == 0)
                    {
                        MessageBox.Show("Não há dados para os clientes selecionados!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }

                    Color[] coresClientes = new Color[]
                    {
                        Color.FromArgb(0, 114, 178),
                        Color.FromArgb(230, 159, 0),
                        Color.FromArgb(0, 158, 115),
                        Color.FromArgb(204, 121, 167),
                        Color.FromArgb(86, 180, 233),
                        Color.FromArgb(213, 94, 0),
                        Color.FromArgb(240, 228, 66),
                        Color.FromArgb(0, 0, 0),
                        Color.FromArgb(128, 128, 128),
                        Color.FromArgb(102, 51, 153)
                    };

                    var servicosUnicos = dadosFiltrados
                        .Select(x => x.Servico)
                        .Distinct()
                        .OrderBy(s => s)
                        .ToList();

                    for (int i = 0; i < clientesOrdenados.Count; i++)
                    {
                        var clienteInfo = clientesOrdenados[i];
                        var cliente = clienteInfo.Cliente;

                        string nomeAbreviado = clienteInfo.Nome.Length > 20 ?
                            clienteInfo.Nome.Substring(0, 17) + "..." :
                            clienteInfo.Nome;

                        Series serieCliente = new Series(nomeAbreviado);
                        serieCliente.ChartType = SeriesChartType.Column;
                        serieCliente.IsValueShownAsLabel = true;
                        serieCliente.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
                        serieCliente.Color = coresClientes[i % coresClientes.Length];
                        serieCliente.LabelForeColor = Color.Black;
                        serieCliente.BorderColor = Color.Black;
                        serieCliente.BorderWidth = 2;

                        bool clienteTemDados = false;

                        foreach (var servico in servicosUnicos)
                        {
                            int quantidade = cliente.Count(x => x.Servico == servico);

                            if (quantidade > 0)
                            {
                                clienteTemDados = true;
                                int idx = serieCliente.Points.AddXY(servico, quantidade);
                                serieCliente.Points[idx].Label = quantidade.ToString();
                                serieCliente.Points[idx].LabelForeColor = Color.Black;
                            }
                        }

                        if (clienteTemDados)
                        {
                            chartGrafico.Series.Add(serieCliente);
                        }
                    }

                    if (chartGrafico.Series.Count == 0)
                    {
                        MessageBox.Show("Nenhum dos clientes selecionados possui dados de serviços!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }
                }

                chartGrafico.ChartAreas[0].AxisY.Title = "";
                chartGrafico.ChartAreas[0].AxisX.Title = "";
                chartGrafico.ChartAreas[0].AxisY.Minimum = 0;
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

                double maxValue = 0;
                foreach (var series in chartGrafico.Series)
                {
                    foreach (DataPoint point in series.Points)
                    {
                        if (point.YValues[0] > maxValue)
                        {
                            maxValue = point.YValues[0];
                        }
                    }
                }

                if (maxValue > 0)
                {
                    double roundedMax = Math.Ceiling(maxValue * 1.1 / 10) * 10;
                    if (roundedMax < 10) roundedMax = 10;

                    chartGrafico.ChartAreas[0].AxisY.Maximum = roundedMax;
                    chartGrafico.ChartAreas[0].AxisY.Interval = Math.Max(1, roundedMax / 10);
                    chartGrafico.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                }
                else
                {
                    chartGrafico.ChartAreas[0].AxisY.Interval = 1;
                }

                var novaLegenda = new Legend("LegendaServicos");
                novaLegenda.Docking = Docking.Top;
                novaLegenda.Alignment = StringAlignment.Near;
                novaLegenda.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                novaLegenda.ForeColor = Color.FromArgb(52, 73, 94);
                novaLegenda.BackColor = Color.Transparent;
                novaLegenda.BorderColor = Color.Transparent;
                novaLegenda.MaximumAutoSize = 25;
                novaLegenda.Position.Auto = false;
                novaLegenda.Position.X = 1;
                novaLegenda.Position.Y = 0.5f;
                novaLegenda.Position.Width = 40;
                novaLegenda.Position.Height = 15;

                chartGrafico.Legends.Add(novaLegenda);

                if (chartGrafico.Series.Count > 1)
                {
                    chartGrafico.ChartAreas[0].AxisX.Interval = 1;

                    foreach (var series in chartGrafico.Series)
                    {
                        series["PointWidth"] = "0.8";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}\n\nStack: {ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Carregamento de Dados

        private async System.Threading.Tasks.Task CarregarClientesAsync()
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"
                        SELECT ClienteId, CodigoEmpresa, Nome 
                        FROM Clientes 
                        WHERE Ativo = 1 
                        ORDER BY Nome";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        dtClientesCache = new DataTable();
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dtClientesCache);
                        }
                    }
                }

                dgvClientes.DataSource = dtClientesCache;

                if (dgvClientes.Columns.Contains("ClienteId"))
                    dgvClientes.Columns["ClienteId"].Visible = false;

                if (dgvClientes.Columns.Contains("CodigoEmpresa"))
                {
                    dgvClientes.Columns["CodigoEmpresa"].HeaderText = "CÓDIGO";
                    dgvClientes.Columns["CodigoEmpresa"].Width = 100;
                }

                if (dgvClientes.Columns.Contains("Nome"))
                {
                    dgvClientes.Columns["Nome"].HeaderText = "NOME";
                    dgvClientes.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                DataTable dtInseridos = new DataTable();
                dtInseridos.Columns.Add("ClienteId", typeof(int));
                dtInseridos.Columns.Add("CodigoEmpresa", typeof(string));
                dtInseridos.Columns.Add("Nome", typeof(string));
                dgvClientesInseridos.DataSource = dtInseridos;

                if (dgvClientesInseridos.Columns.Contains("ClienteId"))
                    dgvClientesInseridos.Columns["ClienteId"].Visible = false;

                if (dgvClientesInseridos.Columns.Contains("CodigoEmpresa"))
                {
                    dgvClientesInseridos.Columns["CodigoEmpresa"].HeaderText = "CÓDIGO";
                    dgvClientesInseridos.Columns["CodigoEmpresa"].Width = 100;
                }

                if (dgvClientesInseridos.Columns.Contains("Nome"))
                {
                    dgvClientesInseridos.Columns["Nome"].HeaderText = "NOME";
                    dgvClientesInseridos.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar clientes: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Gráfico - Valores Mensais Frete 

        private async void BtnGValoresMensais_Click(object sender, EventArgs e)
        {
            HabilitarControlesCliente(false);
            await GerarGraficoValoresMensaisAsync();
        }

        private async System.Threading.Tasks.Task GerarGraficoValoresMensaisAsync()
        {
            try
            {
                DateTime dataInicio = dtpInicio.Value.Date;
                DateTime dataFinal = dtpFinal.Value.Date;

                if (dataInicio > dataFinal)
                {
                    MessageBox.Show("A data inicial não pode ser maior que a data final!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dtDados = await BuscarDadosValoresMensaisFreteAsync(dataInicio, dataFinal);

                if (dtDados.Rows.Count == 0)
                {
                    MessageBox.Show("Não há dados de fretes CIF para o período selecionado!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                var valoresPorTransportadora = dtDados.AsEnumerable()
                    .GroupBy(row => new
                    {
                        TransportadoraId = row.Field<int>("TransportadoraId"),
                        Transportadora = row.Field<string>("Transportadora") ?? "Desconhecido"
                    })
                    .Select(g => new
                    {
                        Transportadora = g.Key.Transportadora,
                        ValorTotal = g.Sum(r => r.Field<decimal>("ValorFrete"))
                    })
                    .OrderByDescending(x => x.ValorTotal)
                    .Take(10)
                    .ToList();

                if (valoresPorTransportadora.Count == 0)
                {
                    MessageBox.Show("Não há fretes CIF registrados no período!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                chartGrafico.Series.Clear();
                chartGrafico.Titles.Clear();
                chartGrafico.Legends.Clear();

                chartGrafico.Titles.Add(new Title(
                    $"Valores Mensais - Fretes CIF\n{dataInicio:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}",
                    Docking.Top,
                    new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                    Color.FromArgb(52, 73, 94)));

                Series serie = new Series("Valor Total");
                serie.ChartType = SeriesChartType.Column;
                serie.IsValueShownAsLabel = true;
                serie.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                serie.LabelForeColor = Color.Black;
                serie.BorderWidth = 2;
                serie.BorderColor = Color.Black;

                Color[] cores = new Color[]
                {
                    Color.FromArgb(0, 114, 178),
                    Color.FromArgb(230, 159, 0),
                    Color.FromArgb(0, 158, 115),
                    Color.FromArgb(204, 121, 167),
                    Color.FromArgb(86, 180, 233),
                    Color.FromArgb(213, 94, 0),
                    Color.FromArgb(240, 228, 66),
                    Color.FromArgb(0, 0, 0),
                    Color.FromArgb(128, 128, 128),
                    Color.FromArgb(102, 51, 153)
                };

                for (int i = 0; i < valoresPorTransportadora.Count; i++)
                {
                    var item = valoresPorTransportadora[i];
                    string nomeAbreviado = item.Transportadora.Length > 20 ?
                        item.Transportadora.Substring(0, 17) + "..." :
                        item.Transportadora;

                    int idx = serie.Points.AddXY(nomeAbreviado, item.ValorTotal);
                    serie.Points[idx].Color = cores[i % cores.Length];
                    serie.Points[idx].Label = item.ValorTotal.ToString("C2");
                    serie.Points[idx].LabelForeColor = Color.Black;
                    serie.Points[idx].BorderColor = Color.Black;
                    serie.Points[idx].BorderWidth = 2;
                }

                chartGrafico.Series.Add(serie);

                chartGrafico.ChartAreas[0].AxisY.Title = "Valor Total (R$)";
                chartGrafico.ChartAreas[0].AxisX.Title = "Transportadora";
                chartGrafico.ChartAreas[0].AxisY.Minimum = 0;
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

                double maxValue = valoresPorTransportadora.Max(v => (double)v.ValorTotal);
                if (maxValue > 0)
                {
                    double roundedMax = Math.Ceiling(maxValue * 1.1 / 100) * 100;
                    if (roundedMax < 100) roundedMax = 100;

                    chartGrafico.ChartAreas[0].AxisY.Maximum = roundedMax;
                    chartGrafico.ChartAreas[0].AxisY.Interval = Math.Max(100, roundedMax / 10);
                    chartGrafico.ChartAreas[0].AxisY.LabelStyle.Format = "C2";
                }
                else
                {
                    chartGrafico.ChartAreas[0].AxisY.Interval = 100;
                }

                var novaLegenda = new Legend("LegendaValoresFrete");
                novaLegenda.Docking = Docking.Top;
                novaLegenda.Alignment = StringAlignment.Near;
                novaLegenda.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                novaLegenda.ForeColor = Color.FromArgb(52, 73, 94);

                novaLegenda.BackColor = Color.Transparent;
                novaLegenda.BorderColor = Color.Transparent;
                novaLegenda.MaximumAutoSize = 25;
                novaLegenda.Position.Auto = false;
                novaLegenda.Position.X = 1;
                novaLegenda.Position.Y = 0.5f;
                novaLegenda.Position.Width = 40;
                novaLegenda.Position.Height = 15;

                chartGrafico.Legends.Add(novaLegenda);

                serie["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}\n\nStack: {ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task<DataTable> BuscarDadosValoresMensaisFreteAsync(
            DateTime dataInicio, DateTime dataFinal)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                    SELECT 
                        f.TransportadoraId,
                        m.NomeInterno AS Transportadora,
                        f.ValorFrete
                    FROM Fretes f WITH (NOLOCK)
                    INNER JOIN Motoristas m ON f.TransportadoraId = m.MotoristaId
                    WHERE CAST(f.DataOcorrencia AS DATE) BETWEEN @DataInicio AND @DataFinal
                    AND f.TipoFrete = 'CIF'
                    AND f.ValorFrete IS NOT NULL
                    AND f.ValorFrete > 0";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFinal", dataFinal);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        #endregion

        // ===================================================================
        // ADICIONAR NO ExpedicaoFormGraficos.cs
        // ===================================================================

        #region Gráfico - Material Vendido

        private async void BtnGMaterialVendido_Click(object sender, EventArgs e)
        {
            HabilitarControlesCliente(false);
            await GerarGraficoMaterialVendidoAsync();
        }

        private async System.Threading.Tasks.Task GerarGraficoMaterialVendidoAsync()
        {
            try
            {
                DateTime dataInicio = dtpInicio.Value.Date;
                DateTime dataFinal = dtpFinal.Value.Date;

                if (dataInicio > dataFinal)
                {
                    MessageBox.Show("A data inicial não pode ser maior que a data final!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dtDados = await BuscarDadosMaterialVendidoAsync(dataInicio, dataFinal);

                if (dtDados.Rows.Count == 0)
                {
                    MessageBox.Show("Não há materiais vendidos para o período selecionado!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                var materiaisVendidos = dtDados.AsEnumerable()
                    .Where(row => !string.IsNullOrWhiteSpace(row.Field<string>("Material")))
                    .GroupBy(row => row.Field<string>("Material"))
                    .Select(g => new
                    {
                        Material = g.Key,
                        PesoTotal = g.Sum(r => r.Field<decimal>("Quantidade"))
                    })
                    .OrderByDescending(x => x.PesoTotal)
                    .Take(15)
                    .ToList();

                if (materiaisVendidos.Count == 0)
                {
                    MessageBox.Show("Não há materiais vendidos no período!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                chartGrafico.Series.Clear();
                chartGrafico.Titles.Clear();
                chartGrafico.Legends.Clear();

                chartGrafico.Titles.Add(new Title(
                    $"Materiais Mais Vendidos (kg)\n{dataInicio:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}",
                    Docking.Top,
                    new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                    Color.FromArgb(52, 73, 94)));

                Series serie = new Series("Peso Total");
                serie.ChartType = SeriesChartType.Column;
                serie.IsValueShownAsLabel = true;
                serie.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
                serie.LabelForeColor = Color.Black;
                serie.BorderWidth = 2;
                serie.BorderColor = Color.Black;

                Color[] cores = new Color[]
                {
            Color.FromArgb(0, 114, 178),
            Color.FromArgb(230, 159, 0),
            Color.FromArgb(0, 158, 115),
            Color.FromArgb(204, 121, 167),
            Color.FromArgb(86, 180, 233),
            Color.FromArgb(213, 94, 0),
            Color.FromArgb(240, 228, 66),
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(128, 128, 128),
            Color.FromArgb(102, 51, 153),
            Color.FromArgb(244, 67, 54),
            Color.FromArgb(76, 175, 80),
            Color.FromArgb(33, 150, 243),
            Color.FromArgb(156, 39, 176),
            Color.FromArgb(255, 193, 7)
                };

                for (int i = 0; i < materiaisVendidos.Count; i++)
                {
                    var item = materiaisVendidos[i];
                    string nomeAbreviado = item.Material.Length > 18 ?
                        item.Material.Substring(0, 15) + "..." :
                        item.Material;

                    int idx = serie.Points.AddXY(nomeAbreviado, item.PesoTotal);
                    serie.Points[idx].Color = cores[i % cores.Length];

                    if (item.PesoTotal >= 1000)
                    {
                        serie.Points[idx].Label = (item.PesoTotal / 1000).ToString("N1") + "t";
                    }
                    else
                    {
                        serie.Points[idx].Label = item.PesoTotal.ToString("N0") + "kg";
                    }

                    serie.Points[idx].LabelForeColor = Color.Black;
                    serie.Points[idx].BorderColor = Color.Black;
                    serie.Points[idx].BorderWidth = 2;

                    serie.Points[idx].ToolTip = $"{item.Material}\n{item.PesoTotal:N3} kg";
                }

                chartGrafico.Series.Add(serie);

                chartGrafico.ChartAreas[0].AxisY.Title = "Peso (kg)";
                chartGrafico.ChartAreas[0].AxisX.Title = "Material";
                chartGrafico.ChartAreas[0].AxisY.Minimum = 0;
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

                double maxValue = materiaisVendidos.Max(m => (double)m.PesoTotal);
                if (maxValue > 0)
                {
                    double roundedMax = Math.Ceiling(maxValue * 1.1 / 100) * 100;
                    if (roundedMax < 100) roundedMax = 100;

                    chartGrafico.ChartAreas[0].AxisY.Maximum = roundedMax;
                    chartGrafico.ChartAreas[0].AxisY.Interval = Math.Max(100, roundedMax / 10);
                    chartGrafico.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                }
                else
                {
                    chartGrafico.ChartAreas[0].AxisY.Interval = 100;
                }

                var novaLegenda = new Legend("LegendaMaterialVendido");
                novaLegenda.Docking = Docking.Top;
                novaLegenda.Alignment = StringAlignment.Near;
                novaLegenda.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                novaLegenda.ForeColor = Color.FromArgb(52, 73, 94);
                novaLegenda.BackColor = Color.Transparent;
                novaLegenda.BorderColor = Color.Transparent;
                novaLegenda.MaximumAutoSize = 25;
                novaLegenda.Position.Auto = false;
                novaLegenda.Position.X = 1;
                novaLegenda.Position.Y = 0.5f;
                novaLegenda.Position.Width = 40;
                novaLegenda.Position.Height = 15;

                chartGrafico.Legends.Add(novaLegenda);

                serie["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}\n\nStack: {ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task<DataTable> BuscarDadosMaterialVendidoAsync(
            DateTime dataInicio, DateTime dataFinal)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
            SELECT 
                e.Material,
                e.Quantidade
            FROM Estoques e WITH (NOLOCK)
            WHERE e.Status = 'Vendido'
            AND e.DataSaida IS NOT NULL
            AND CAST(e.DataSaida AS DATE) BETWEEN @DataInicio AND @DataFinal
            AND e.Material IS NOT NULL 
            AND e.Material <> ''
            AND e.Quantidade > 0";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFinal", dataFinal);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        #endregion

        // ===================================================================
        // ADICIONAR NO ExpedicaoFormGraficos.cs
        // ===================================================================

        #region Gráfico - Top 10 Clientes

        private async void BtnGClientes_Click(object sender, EventArgs e)
        {
            HabilitarControlesCliente(false);
            await GerarGraficoTopClientesAsync();
        }

        private async System.Threading.Tasks.Task GerarGraficoTopClientesAsync()
        {
            try
            {
                DateTime dataInicio = dtpInicio.Value.Date;
                DateTime dataFinal = dtpFinal.Value.Date;

                if (dataInicio > dataFinal)
                {
                    MessageBox.Show("A data inicial não pode ser maior que a data final!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dtDados = await BuscarDadosTopClientesAsync(dataInicio, dataFinal);

                if (dtDados.Rows.Count == 0)
                {
                    MessageBox.Show("Não há vendas de materiais para o período selecionado!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                var topClientes = dtDados.AsEnumerable()
                    .Where(row => !string.IsNullOrWhiteSpace(row.Field<string>("ClienteNome")))
                    .GroupBy(row => new
                    {
                        ClienteId = row.Field<int>("ClienteId"),
                        ClienteNome = row.Field<string>("ClienteNome")
                    })
                    .Select(g => new
                    {
                        ClienteNome = g.Key.ClienteNome,
                        PesoTotal = g.Sum(r => r.Field<decimal>("Quantidade")),
                        Volumes = g.Count()
                    })
                    .OrderByDescending(x => x.PesoTotal)
                    .Take(10)
                    .ToList();

                if (topClientes.Count == 0)
                {
                    MessageBox.Show("Não há clientes com compras no período!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                chartGrafico.Series.Clear();
                chartGrafico.Titles.Clear();
                chartGrafico.Legends.Clear();

                chartGrafico.Titles.Add(new Title(
                    $"Top 10 Clientes - Materiais Comprados (kg)\n{dataInicio:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}",
                    Docking.Top,
                    new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                    Color.FromArgb(52, 73, 94)));

                Series serie = new Series("Peso Total");
                serie.ChartType = SeriesChartType.Column;
                serie.IsValueShownAsLabel = true;
                serie.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                serie.LabelForeColor = Color.Black;
                serie.BorderWidth = 2;
                serie.BorderColor = Color.Black;

                Color[] cores = new Color[]
                {
            Color.FromArgb(0, 114, 178),
            Color.FromArgb(230, 159, 0),
            Color.FromArgb(0, 158, 115),
            Color.FromArgb(204, 121, 167),
            Color.FromArgb(86, 180, 233),
            Color.FromArgb(213, 94, 0),
            Color.FromArgb(240, 228, 66),
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(128, 128, 128),
            Color.FromArgb(102, 51, 153)
                };

                for (int i = 0; i < topClientes.Count; i++)
                {
                    var item = topClientes[i];
                    string nomeAbreviado = item.ClienteNome.Length > 20 ?
                        item.ClienteNome.Substring(0, 17) + "..." :
                        item.ClienteNome;

                    int idx = serie.Points.AddXY(nomeAbreviado, item.PesoTotal);
                    serie.Points[idx].Color = cores[i % cores.Length];

                    if (item.PesoTotal >= 1000)
                    {
                        serie.Points[idx].Label = (item.PesoTotal / 1000).ToString("N1") + "t";
                    }
                    else
                    {
                        serie.Points[idx].Label = item.PesoTotal.ToString("N0") + "kg";
                    }

                    serie.Points[idx].LabelForeColor = Color.Black;
                    serie.Points[idx].BorderColor = Color.Black;
                    serie.Points[idx].BorderWidth = 2;

                    serie.Points[idx].ToolTip = $"{item.ClienteNome}\nPeso: {item.PesoTotal:N3} kg\nVolumes: {item.Volumes}";
                }

                chartGrafico.Series.Add(serie);

                chartGrafico.ChartAreas[0].AxisY.Title = "Peso Total (kg)";
                chartGrafico.ChartAreas[0].AxisX.Title = "Cliente";
                chartGrafico.ChartAreas[0].AxisY.Minimum = 0;
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

                double maxValue = topClientes.Max(c => (double)c.PesoTotal);
                if (maxValue > 0)
                {
                    double roundedMax = Math.Ceiling(maxValue * 1.1 / 100) * 100;
                    if (roundedMax < 100) roundedMax = 100;

                    chartGrafico.ChartAreas[0].AxisY.Maximum = roundedMax;
                    chartGrafico.ChartAreas[0].AxisY.Interval = Math.Max(100, roundedMax / 10);
                    chartGrafico.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                }
                else
                {
                    chartGrafico.ChartAreas[0].AxisY.Interval = 100;
                }

                var novaLegenda = new Legend("LegendaTopClientes");
                novaLegenda.Docking = Docking.Top;
                novaLegenda.Alignment = StringAlignment.Near;
                novaLegenda.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                novaLegenda.ForeColor = Color.FromArgb(52, 73, 94);
                novaLegenda.BackColor = Color.Transparent;
                novaLegenda.BorderColor = Color.Transparent;
                novaLegenda.MaximumAutoSize = 25;
                novaLegenda.Position.Auto = false;
                novaLegenda.Position.X = 1;
                novaLegenda.Position.Y = 0.5f;
                novaLegenda.Position.Width = 40;
                novaLegenda.Position.Height = 15;

                chartGrafico.Legends.Add(novaLegenda);

                serie["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}\n\nStack: {ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task<DataTable> BuscarDadosTopClientesAsync(
            DateTime dataInicio, DateTime dataFinal)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
            SELECT 
                e.ClienteId,
                c.Nome as ClienteNome,
                e.Quantidade
            FROM Estoques e WITH (NOLOCK)
            INNER JOIN Clientes c ON e.ClienteId = c.ClienteId
            WHERE e.Status = 'Vendido'
            AND e.DataSaida IS NOT NULL
            AND CAST(e.DataSaida AS DATE) BETWEEN @DataInicio AND @DataFinal
            AND e.ClienteId > 0
            AND e.Quantidade > 0";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFinal", dataFinal);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        #endregion

        #region Gráfico - Viagens por Transportadora

        private async void BtnGViagens_Click(object sender, EventArgs e)
        {
            HabilitarControlesCliente(false);
            await GerarGraficoViagensTransportadoraAsync();
        }

        private async System.Threading.Tasks.Task GerarGraficoViagensTransportadoraAsync()
        {
            try
            {
                DateTime dataInicio = dtpInicio.Value.Date;
                DateTime dataFinal = dtpFinal.Value.Date;

                if (dataInicio > dataFinal)
                {
                    MessageBox.Show("A data inicial não pode ser maior que a data final!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dtDados = await BuscarDadosViagensTransportadoraAsync(dataInicio, dataFinal);

                if (dtDados.Rows.Count == 0)
                {
                    MessageBox.Show("Não há dados de fretes para o período selecionado!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                var viagensPorTransportadora = dtDados.AsEnumerable()
                    .GroupBy(row => new
                    {
                        TransportadoraId = row.Field<int>("TransportadoraId"),
                        Transportadora = row.Field<string>("Transportadora") ?? "Desconhecido"
                    })
                    .Select(g => new
                    {
                        Transportadora = g.Key.Transportadora,
                        Quantidade = g.Count()
                    })
                    .OrderByDescending(x => x.Quantidade)
                    .Take(10)
                    .ToList();

                if (viagensPorTransportadora.Count == 0)
                {
                    MessageBox.Show("Não há transportadoras registradas no período!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                chartGrafico.Series.Clear();
                chartGrafico.Titles.Clear();
                chartGrafico.Legends.Clear();

                chartGrafico.Titles.Add(new Title(
                    $"Viagens por Transportadora\n{dataInicio:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}",
                    Docking.Top,
                    new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                    Color.FromArgb(52, 73, 94)));

                Series serie = new Series("Viagens");
                serie.ChartType = SeriesChartType.Column;
                serie.IsValueShownAsLabel = true;
                serie.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                serie.LabelForeColor = Color.Black;
                serie.BorderWidth = 2;
                serie.BorderColor = Color.Black;

                Color[] cores = new Color[]
                {
                    Color.FromArgb(0, 114, 178),
                    Color.FromArgb(230, 159, 0),
                    Color.FromArgb(0, 158, 115),
                    Color.FromArgb(204, 121, 167),
                    Color.FromArgb(86, 180, 233),
                    Color.FromArgb(213, 94, 0),
                    Color.FromArgb(240, 228, 66),
                    Color.FromArgb(0, 0, 0),
                    Color.FromArgb(128, 128, 128),
                    Color.FromArgb(102, 51, 153)
                };

                for (int i = 0; i < viagensPorTransportadora.Count; i++)
                {
                    var item = viagensPorTransportadora[i];
                    string nomeAbreviado = item.Transportadora.Length > 20 ?
                        item.Transportadora.Substring(0, 17) + "..." :
                        item.Transportadora;

                    int idx = serie.Points.AddXY(nomeAbreviado, item.Quantidade);
                    serie.Points[idx].Color = cores[i % cores.Length];
                    serie.Points[idx].Label = item.Quantidade.ToString();
                    serie.Points[idx].LabelForeColor = Color.Black;
                    serie.Points[idx].BorderColor = Color.Black;
                    serie.Points[idx].BorderWidth = 2;
                }

                chartGrafico.Series.Add(serie);

                chartGrafico.ChartAreas[0].AxisY.Title = "Quantidade de Viagens";
                chartGrafico.ChartAreas[0].AxisX.Title = "Transportadora";
                chartGrafico.ChartAreas[0].AxisY.Minimum = 0;
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

                double maxValue = viagensPorTransportadora.Max(v => v.Quantidade);
                if (maxValue > 0)
                {
                    double roundedMax = Math.Ceiling(maxValue * 1.1 / 10) * 10;
                    if (roundedMax < 10) roundedMax = 10;

                    chartGrafico.ChartAreas[0].AxisY.Maximum = roundedMax;
                    chartGrafico.ChartAreas[0].AxisY.Interval = Math.Max(1, roundedMax / 10);
                    chartGrafico.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                }
                else
                {
                    chartGrafico.ChartAreas[0].AxisY.Interval = 1;
                }

                var novaLegenda = new Legend("LegendaViagensTransportadora");
                novaLegenda.Docking = Docking.Top;
                novaLegenda.Alignment = StringAlignment.Near;
                novaLegenda.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                novaLegenda.ForeColor = Color.FromArgb(52, 73, 94);
                novaLegenda.BackColor = Color.Transparent;
                novaLegenda.BorderColor = Color.Transparent;
                novaLegenda.MaximumAutoSize = 25;
                novaLegenda.Position.Auto = false;
                novaLegenda.Position.X = 1;
                novaLegenda.Position.Y = 0.5f;
                novaLegenda.Position.Width = 40;
                novaLegenda.Position.Height = 15;

                chartGrafico.Legends.Add(novaLegenda);

                serie["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}\n\nStack: {ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task<DataTable> BuscarDadosViagensTransportadoraAsync(
            DateTime dataInicio, DateTime dataFinal)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                    SELECT 
                        f.TransportadoraId,
                        m.NomeInterno AS Transportadora,
                        f.FreteId
                    FROM Fretes f WITH (NOLOCK)
                    INNER JOIN Motoristas m ON f.TransportadoraId = m.MotoristaId
                    WHERE CAST(f.DataOcorrencia AS DATE) BETWEEN @DataInicio AND @DataFinal";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFinal", dataFinal);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        #endregion

        #region Gráfico - Lançamentos de Materiais

        private async void BtnGLancamentos_Click(object sender, EventArgs e)
        {
            HabilitarControlesCliente(false);
            await GerarGraficoLancamentosMateriaisAsync();
        }

        private async System.Threading.Tasks.Task GerarGraficoLancamentosMateriaisAsync()
        {
            try
            {
                DateTime dataInicio = dtpInicio.Value.Date;
                DateTime dataFinal = dtpFinal.Value.Date;

                if (dataInicio > dataFinal)
                {
                    MessageBox.Show("A data inicial não pode ser maior que a data final!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dtDados = await BuscarDadosLancamentosMateriaisAsync(dataInicio, dataFinal);

                if (dtDados.Rows.Count == 0)
                {
                    MessageBox.Show("Não há lançamentos de materiais para o período selecionado!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                var materiaisAgrupados = dtDados.AsEnumerable()
                    .Where(row => !string.IsNullOrWhiteSpace(row.Field<string>("Material")))
                    .GroupBy(row => row.Field<string>("Material"))
                    .Select(g => new
                    {
                        Material = g.Key,
                        PesoTotal = g.Sum(r => r.Field<decimal>("Peso"))
                    })
                    .OrderByDescending(x => x.PesoTotal)
                    .Take(15)
                    .ToList();

                if (materiaisAgrupados.Count == 0)
                {
                    MessageBox.Show("Não há materiais registrados no período!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                chartGrafico.Series.Clear();
                chartGrafico.Titles.Clear();
                chartGrafico.Legends.Clear();

                chartGrafico.Titles.Add(new Title(
                    $"Lançamentos de Materiais (kg)\n{dataInicio:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}",
                    Docking.Top,
                    new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                    Color.FromArgb(52, 73, 94)));

                Series serie = new Series("Peso Total");
                serie.ChartType = SeriesChartType.Column;
                serie.IsValueShownAsLabel = true;
                serie.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
                serie.LabelForeColor = Color.Black;
                serie.BorderWidth = 2;
                serie.BorderColor = Color.Black;

                Color[] cores = new Color[]
                {
                    Color.FromArgb(0, 114, 178),
                    Color.FromArgb(230, 159, 0),
                    Color.FromArgb(0, 158, 115),
                    Color.FromArgb(204, 121, 167),
                    Color.FromArgb(86, 180, 233),
                    Color.FromArgb(213, 94, 0),
                    Color.FromArgb(240, 228, 66),
                    Color.FromArgb(0, 0, 0),
                    Color.FromArgb(128, 128, 128),
                    Color.FromArgb(102, 51, 153),
                    Color.FromArgb(244, 67, 54),
                    Color.FromArgb(76, 175, 80),
                    Color.FromArgb(33, 150, 243),
                    Color.FromArgb(156, 39, 176),
                    Color.FromArgb(255, 193, 7)
                };

                for (int i = 0; i < materiaisAgrupados.Count; i++)
                {
                    var item = materiaisAgrupados[i];
                    string nomeAbreviado = item.Material.Length > 18 ?
                        item.Material.Substring(0, 15) + "..." :
                        item.Material;

                    int idx = serie.Points.AddXY(nomeAbreviado, item.PesoTotal);
                    serie.Points[idx].Color = cores[i % cores.Length];

                    if (item.PesoTotal >= 1000)
                    {
                        serie.Points[idx].Label = (item.PesoTotal / 1000).ToString("N1") + "t";
                    }
                    else
                    {
                        serie.Points[idx].Label = item.PesoTotal.ToString("N0") + "kg";
                    }

                    serie.Points[idx].LabelForeColor = Color.Black;
                    serie.Points[idx].BorderColor = Color.Black;
                    serie.Points[idx].BorderWidth = 2;

                    serie.Points[idx].ToolTip = $"{item.Material}\n{item.PesoTotal:N3} kg";
                }

                chartGrafico.Series.Add(serie);

                chartGrafico.ChartAreas[0].AxisY.Title = "Peso (kg)";
                chartGrafico.ChartAreas[0].AxisX.Title = "Material";
                chartGrafico.ChartAreas[0].AxisY.Minimum = 0;
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

                double maxValue = materiaisAgrupados.Max(m => (double)m.PesoTotal);
                if (maxValue > 0)
                {
                    double roundedMax = Math.Ceiling(maxValue * 1.1 / 100) * 100;
                    if (roundedMax < 100) roundedMax = 100;

                    chartGrafico.ChartAreas[0].AxisY.Maximum = roundedMax;
                    chartGrafico.ChartAreas[0].AxisY.Interval = Math.Max(100, roundedMax / 10);
                    chartGrafico.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                }
                else
                {
                    chartGrafico.ChartAreas[0].AxisY.Interval = 100;
                }

                var novaLegenda = new Legend("LegendaLancamentos");
                novaLegenda.Docking = Docking.Top;
                novaLegenda.Alignment = StringAlignment.Near;
                novaLegenda.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                novaLegenda.ForeColor = Color.FromArgb(52, 73, 94);
                novaLegenda.BackColor = Color.Transparent;
                novaLegenda.BorderColor = Color.Transparent;
                novaLegenda.MaximumAutoSize = 25;
                novaLegenda.Position.Auto = false;
                novaLegenda.Position.X = 1;
                novaLegenda.Position.Y = 0.5f;
                novaLegenda.Position.Width = 40;
                novaLegenda.Position.Height = 15;

                chartGrafico.Legends.Add(novaLegenda);

                serie["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}\n\nStack: {ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task<DataTable> BuscarDadosLancamentosMateriaisAsync(
            DateTime dataInicio, DateTime dataFinal)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
            SELECT 
                lm.Material,
                lm.Peso
            FROM LancamentosMateriais lm WITH (NOLOCK)
            INNER JOIN ControleLogistico cl ON lm.Ticket = cl.Ticket
            WHERE CAST(cl.Data AS DATE) BETWEEN @DataInicio AND @DataFinal
            AND lm.Material IS NOT NULL 
            AND lm.Material <> ''
            AND lm.Peso > 0";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFinal", dataFinal);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        #endregion

        #region Eventos de RadioButtons

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (!rb.Checked) return;

            if (rbTodos.Checked)
            {
                dgvClientes.Enabled = false;
                dgvClientesInseridos.Enabled = false;
                btnInserir.Enabled = false;
                btnRemover.Enabled = false;
            }
            else if (rbMulti.Checked)
            {
                dgvClientes.Enabled = true;
                dgvClientesInseridos.Enabled = true;
                btnInserir.Enabled = true;
                btnRemover.Enabled = true;
            }
            else if (rbUnico.Checked)
            {
                dgvClientes.Enabled = true;
                dgvClientesInseridos.Enabled = true;
                btnInserir.Enabled = true;
                btnRemover.Enabled = true;
            }
        }

        #endregion

        #region Eventos de Botões - Inserir/Remover

        private void BtnInserir_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null) return;

            DataTable dtInseridos = (DataTable)dgvClientesInseridos.DataSource;

            if (rbUnico.Checked && dtInseridos.Rows.Count > 0)
            {
                dtInseridos.Clear();
            }

            int clienteId = Convert.ToInt32(dgvClientes.CurrentRow.Cells["ClienteId"].Value);

            bool jaExiste = false;
            foreach (DataRow row in dtInseridos.Rows)
            {
                if (Convert.ToInt32(row["ClienteId"]) == clienteId)
                {
                    jaExiste = true;
                    break;
                }
            }

            if (!jaExiste)
            {
                DataRow novaLinha = dtInseridos.NewRow();
                novaLinha["ClienteId"] = dgvClientes.CurrentRow.Cells["ClienteId"].Value;
                novaLinha["CodigoEmpresa"] = dgvClientes.CurrentRow.Cells["CodigoEmpresa"].Value;
                novaLinha["Nome"] = dgvClientes.CurrentRow.Cells["Nome"].Value;
                dtInseridos.Rows.Add(novaLinha);
            }
        }

        private void BtnRemover_Click(object sender, EventArgs e)
        {
            if (dgvClientesInseridos.CurrentRow == null) return;

            DataTable dtInseridos = (DataTable)dgvClientesInseridos.DataSource;
            int index = dgvClientesInseridos.CurrentRow.Index;

            if (index >= 0 && index < dtInseridos.Rows.Count)
            {
                dtInseridos.Rows[index].Delete();
            }
        }

        #endregion

        #region Eventos de Gráficos

        private async void BtnGStatusLogistica_Click(object sender, EventArgs e)
        {
            HabilitarControlesCliente(true);

            await GerarGraficoStatusLogisticaAsync();
        }

        private async void BtnGMotoristas_Click(object sender, EventArgs e)
        {
            HabilitarControlesCliente(false);

            await GerarGraficoMotoristasAsync();
        }

        private void HabilitarControlesCliente(bool habilitar)
        {
            rbTodos.Enabled = habilitar;
            rbMulti.Enabled = habilitar;
            rbUnico.Enabled = habilitar;

            if (!habilitar)
            {
                dgvClientes.Enabled = false;
                dgvClientesInseridos.Enabled = false;
                btnInserir.Enabled = false;
                btnRemover.Enabled = false;
            }
            else
            {
                RadioButton_CheckedChanged(rbTodos.Checked ? rbTodos :
                    (rbMulti.Checked ? rbMulti : rbUnico), EventArgs.Empty);
            }
        }

        #endregion

        #region Gráfico - Status Logística

        private async System.Threading.Tasks.Task GerarGraficoStatusLogisticaAsync()
        {
            try
            {
                DateTime dataInicio = dtpInicio.Value.Date;
                DateTime dataFinal = dtpFinal.Value.Date;

                if (dataInicio > dataFinal)
                {
                    MessageBox.Show("A data inicial não pode ser maior que a data final!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<int> clienteIds = ObterClientesSelecionados();

                DataTable dtDados = await BuscarDadosStatusLogisticaComClienteAsync(dataInicio, dataFinal, clienteIds);

                if (dtDados.Rows.Count == 0)
                {
                    MessageBox.Show("Não há dados para o período selecionado!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                chartGrafico.Series.Clear();
                chartGrafico.Titles.Clear();

                // CORREÇÃO: Limpar TODAS as legendas antes de começar
                chartGrafico.Legends.Clear();

                string tituloCliente = ObterTituloCliente();
                chartGrafico.Titles.Add(new Title(
                    $"Status Logística - {dataInicio:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}\n{tituloCliente}",
                    Docking.Top,
                    new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                    Color.FromArgb(52, 73, 94)));

                List<string> statusFixos = new List<string>
        {
            "EM EXECUÇÃO",
            "NÃO EFETUADO",
            "CONCLUÍDO"
        };

                if (rbTodos.Checked || (rbUnico.Checked && dgvClientesInseridos.Rows.Count == 1))
                {
                    var dadosFiltrados = dtDados.AsEnumerable()
                        .Where(row => statusFixos.Contains((row.Field<string>("StatusLogistica") ?? "").ToUpper()))
                        .ToList();

                    if (dadosFiltrados.Count == 0)
                    {
                        MessageBox.Show("Não há dados com os status relevantes para o período!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }

                    var dadosAgrupados = dadosFiltrados
                        .GroupBy(row => row.Field<string>("StatusLogistica") ?? "")
                        .Select(g => new
                        {
                            Status = g.Key.ToUpper(),
                            Quantidade = g.Count()
                        })
                        .OrderBy(x => statusFixos.IndexOf(x.Status))
                        .ToList();

                    // Criar série única
                    Series serie = new Series("Quantidade");
                    serie.ChartType = SeriesChartType.Column;
                    serie.IsValueShownAsLabel = true; // ATIVADO: Mostra valores
                    serie.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                    serie.LabelForeColor = Color.Black;
                    serie.BorderWidth = 2; // NOVO: Borda mais grossa
                    serie.BorderColor = Color.Black; // NOVO: Borda preta para contraste

                    Dictionary<string, Color> coresStatus = new Dictionary<string, Color>
            {
                { "EM EXECUÇÃO", Color.FromArgb(255, 193, 7) },    // Amarelo
                { "NÃO EFETUADO", Color.FromArgb(244, 67, 54) },   // Vermelho
                { "CONCLUÍDO", Color.FromArgb(76, 175, 80) }       // Verde
            };

                    foreach (var status in statusFixos)
                    {
                        var item = dadosAgrupados.FirstOrDefault(d => d.Status == status);
                        int quantidade = item != null ? item.Quantidade : 0;

                        if (quantidade > 0)
                        {
                            int idx = serie.Points.AddXY(status, quantidade);

                            if (coresStatus.ContainsKey(status))
                            {
                                serie.Points[idx].Color = coresStatus[status];
                            }
                            else
                            {
                                serie.Points[idx].Color = Color.Gray;
                            }

                            serie.Points[idx].Label = quantidade.ToString();
                            serie.Points[idx].LabelForeColor = Color.Black;
                        }
                    }

                    if (serie.Points.Count > 0)
                    {
                        chartGrafico.Series.Add(serie);
                    }
                    else
                    {
                        MessageBox.Show("Não há dados com os status relevantes para o período!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }
                }
                else
                {
                    var dadosFiltrados = dtDados.AsEnumerable()
                        .Where(row => statusFixos.Contains((row.Field<string>("StatusLogistica") ?? "").ToUpper()))
                        .Select(row => new
                        {
                            ClienteId = row.Field<int>("ClienteId"),
                            ClienteNome = row.Field<string>("ClienteNome") ?? "Cliente Desconhecido",
                            Status = row.Field<string>("StatusLogistica") ?? ""
                        })
                        .ToList();

                    if (dadosFiltrados.Count == 0)
                    {
                        MessageBox.Show("Não há dados com os status relevantes para os clientes selecionados!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }

                    var clientes = dadosFiltrados
                        .GroupBy(x => new { x.ClienteId, x.ClienteNome })
                        .ToList();

                    var clientesOrdenados = clientes
                        .Select(c => new
                        {
                            Cliente = c,
                            Total = c.Count(),
                            Nome = c.Key.ClienteNome
                        })
                        .OrderByDescending(x => x.Total)
                        .Take(10)
                        .ToList();

                    if (clientesOrdenados.Count == 0)
                    {
                        MessageBox.Show("Não há dados para os clientes selecionados!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }

                    // NOVO: Cores com padrões distintos para daltônicos
                    Color[] coresClientes = new Color[]
                    {
                        Color.FromArgb(0, 114, 178),      // Azul
                        Color.FromArgb(230, 159, 0),      // Laranja
                        Color.FromArgb(0, 158, 115),      // Verde azulado
                        Color.FromArgb(204, 121, 167),    // Rosa
                        Color.FromArgb(86, 180, 233),     // Azul céu
                        Color.FromArgb(213, 94, 0),       // Vermelho alaranjado
                        Color.FromArgb(240, 228, 66),     // Amarelo
                        Color.FromArgb(0, 0, 0),          // Preto
                        Color.FromArgb(128, 128, 128),    // Cinza
                        Color.FromArgb(102, 51, 153)      // Roxo
                    };

                    for (int i = 0; i < clientesOrdenados.Count; i++)
                    {
                        var clienteInfo = clientesOrdenados[i];
                        var cliente = clienteInfo.Cliente;

                        string nomeAbreviado = clienteInfo.Nome.Length > 20 ?
                            clienteInfo.Nome.Substring(0, 17) + "..." :
                            clienteInfo.Nome;

                        Series serieCliente = new Series(nomeAbreviado);
                        serieCliente.ChartType = SeriesChartType.Column;
                        serieCliente.IsValueShownAsLabel = true;
                        serieCliente.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
                        serieCliente.Color = coresClientes[i % coresClientes.Length];
                        serieCliente.LabelForeColor = Color.Black;
                        serieCliente.BorderColor = Color.Black;
                        serieCliente.BorderWidth = 2;

                        bool clienteTemDados = false;

                        foreach (var status in statusFixos)
                        {
                            int quantidade = cliente.Count(x => x.Status.ToUpper() == status);

                            if (quantidade > 0)
                            {
                                clienteTemDados = true;
                                int idx = serieCliente.Points.AddXY(status, quantidade);
                                serieCliente.Points[idx].Label = quantidade.ToString();
                                serieCliente.Points[idx].LabelForeColor = Color.Black;
                            }
                        }

                        if (clienteTemDados)
                        {
                            chartGrafico.Series.Add(serieCliente);
                        }
                    }

                    if (chartGrafico.Series.Count == 0)
                    {
                        MessageBox.Show("Nenhum dos clientes selecionados possui dados nos status relevantes!",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimparGrafico();
                        return;
                    }
                }

                chartGrafico.ChartAreas[0].AxisY.Title = "";
                chartGrafico.ChartAreas[0].AxisX.Title = "";
                chartGrafico.ChartAreas[0].AxisY.Minimum = 0;
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

                double maxValue = 0;
                foreach (var series in chartGrafico.Series)
                {
                    foreach (DataPoint point in series.Points)
                    {
                        if (point.YValues[0] > maxValue)
                        {
                            maxValue = point.YValues[0];
                        }
                    }
                }

                if (maxValue > 0)
                { 
                    double roundedMax = Math.Ceiling(maxValue * 1.1 / 10) * 10;
                    if (roundedMax < 10) roundedMax = 10;

                    chartGrafico.ChartAreas[0].AxisY.Maximum = roundedMax;
                    chartGrafico.ChartAreas[0].AxisY.Interval = Math.Max(1, roundedMax / 10);
                    chartGrafico.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                }
                else
                {
                    chartGrafico.ChartAreas[0].AxisY.Interval = 1;
                }

                var novaLegenda = new Legend("LegendaClientes");
                novaLegenda.Docking = Docking.Top;
                novaLegenda.Alignment = StringAlignment.Near;
                novaLegenda.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                novaLegenda.ForeColor = Color.FromArgb(52, 73, 94);
                novaLegenda.BackColor = Color.Transparent;
                novaLegenda.BorderColor = Color.Transparent;
                novaLegenda.MaximumAutoSize = 25;
                novaLegenda.Position.Auto = false;
                novaLegenda.Position.X = 1;
                novaLegenda.Position.Y = 0.5f;
                novaLegenda.Position.Width = 40;
                novaLegenda.Position.Height = 15;

                chartGrafico.Legends.Add(novaLegenda);

                if (chartGrafico.Series.Count > 1)
                {
                    chartGrafico.ChartAreas[0].AxisX.Interval = 1;

                    foreach (var series in chartGrafico.Series)
                    {
                        series["PointWidth"] = "0.8";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}\n\nStack: {ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task<DataTable> BuscarDadosStatusLogisticaComClienteAsync(
            DateTime dataInicio, DateTime dataFinal, List<int> clienteIds)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
            SELECT 
                cl.ClienteId,
                c.Nome as ClienteNome,
                cl.StatusLogistica
            FROM ControleLogistico cl WITH (NOLOCK)
            LEFT JOIN Clientes c ON c.ClienteId = cl.ClienteId
            WHERE CAST(cl.Data AS DATE) BETWEEN @DataInicio AND @DataFinal";

                if (clienteIds != null && clienteIds.Count > 0)
                {
                    sql += " AND cl.ClienteId IN (" + string.Join(",", clienteIds) + ")";
                }

                // Filtrar apenas os status relevantes
                sql += " AND UPPER(cl.StatusLogistica) IN ('EM EXECUÇÃO', 'NÃO EFETUADO', 'CONCLUÍDO')";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFinal", dataFinal);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        #endregion

        #region Gráfico - Motoristas

        private async System.Threading.Tasks.Task GerarGraficoMotoristasAsync()
        {
            try
            {
                DateTime dataInicio = dtpInicio.Value.Date;
                DateTime dataFinal = dtpFinal.Value.Date;

                if (dataInicio > dataFinal)
                {
                    MessageBox.Show("A data inicial não pode ser maior que a data final!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dtDados = await BuscarDadosMotoristasAsync(dataInicio, dataFinal);

                if (dtDados.Rows.Count == 0)
                {
                    MessageBox.Show("Não há dados para o período selecionado!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                var topMotoristas = dtDados.AsEnumerable()
                    .Where(row => !string.IsNullOrWhiteSpace(row.Field<string>("Motorista")))
                    .GroupBy(row => row.Field<string>("Motorista"))
                    .Select(g => new
                    {
                        Motorista = g.Key,
                        Quantidade = g.Count()
                    })
                    .OrderByDescending(x => x.Quantidade)
                    .Take(10)
                    .ToList();

                if (topMotoristas.Count == 0)
                {
                    MessageBox.Show("Não há motoristas registrados no período!",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparGrafico();
                    return;
                }

                chartGrafico.Series.Clear();
                chartGrafico.Titles.Clear();
                chartGrafico.Legends.Clear();

                chartGrafico.Titles.Add(new Title(
                    $"Motoristas Mais Utilizados\n{dataInicio:dd/MM/yyyy} a {dataFinal:dd/MM/yyyy}",
                    Docking.Top,
                    new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                    Color.FromArgb(52, 73, 94)));

                Series serie = new Series("Viagens");
                serie.ChartType = SeriesChartType.Column;
                serie.IsValueShownAsLabel = true;
                serie.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                serie.LabelForeColor = Color.Black;
                serie.BorderWidth = 2;
                serie.BorderColor = Color.Black;

                Color[] cores = new Color[]
                {
                    Color.FromArgb(0, 114, 178),      // Azul
                    Color.FromArgb(230, 159, 0),      // Laranja
                    Color.FromArgb(0, 158, 115),      // Verde azulado
                    Color.FromArgb(204, 121, 167),    // Rosa
                    Color.FromArgb(86, 180, 233),     // Azul céu
                    Color.FromArgb(213, 94, 0),       // Vermelho alaranjado
                    Color.FromArgb(240, 228, 66),     // Amarelo
                    Color.FromArgb(0, 0, 0),          // Preto
                    Color.FromArgb(128, 128, 128),    // Cinza
                    Color.FromArgb(102, 51, 153)      // Roxo
                };

                for (int i = 0; i < topMotoristas.Count; i++)
                {
                    var motorista = topMotoristas[i];
                    int idx = serie.Points.AddXY(motorista.Motorista, motorista.Quantidade);
                    serie.Points[idx].Color = cores[i % cores.Length];
                    serie.Points[idx].Label = motorista.Quantidade.ToString();
                    serie.Points[idx].LabelForeColor = Color.Black;
                    serie.Points[idx].BorderColor = Color.Black;
                    serie.Points[idx].BorderWidth = 2;
                }

                chartGrafico.Series.Add(serie);

                chartGrafico.ChartAreas[0].AxisY.Title = "Quantidade de Viagens";
                chartGrafico.ChartAreas[0].AxisX.Title = "Motorista";
                chartGrafico.ChartAreas[0].AxisY.Minimum = 0;
                chartGrafico.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chartGrafico.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
                chartGrafico.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

                double maxValue = topMotoristas.Max(m => m.Quantidade);
                if (maxValue > 0)
                {
                    double roundedMax = Math.Ceiling(maxValue * 1.1 / 10) * 10;
                    if (roundedMax < 10) roundedMax = 10;

                    chartGrafico.ChartAreas[0].AxisY.Maximum = roundedMax;
                    chartGrafico.ChartAreas[0].AxisY.Interval = Math.Max(1, roundedMax / 10);
                    chartGrafico.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                }
                else
                {
                    chartGrafico.ChartAreas[0].AxisY.Interval = 1;
                }

                var novaLegenda = new Legend("LegendaMotoristas");
                novaLegenda.Docking = Docking.Top;
                novaLegenda.Alignment = StringAlignment.Near;
                novaLegenda.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
                novaLegenda.ForeColor = Color.FromArgb(52, 73, 94);
                novaLegenda.BackColor = Color.Transparent;
                novaLegenda.BorderColor = Color.Transparent;
                novaLegenda.MaximumAutoSize = 25;
                novaLegenda.Position.Auto = false;
                novaLegenda.Position.X = 1;
                novaLegenda.Position.Y = 0.5f;
                novaLegenda.Position.Width = 40;
                novaLegenda.Position.Height = 15;

                chartGrafico.Legends.Add(novaLegenda);

                serie["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar gráfico: {ex.Message}\n\nStack: {ex.StackTrace}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task<DataTable> BuscarDadosMotoristasAsync(
            DateTime dataInicio, DateTime dataFinal)
        {
            DataTable dt = new DataTable();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                    SELECT Motorista
                    FROM ControleLogistico WITH (NOLOCK)
                    WHERE CAST(Data AS DATE) BETWEEN @DataInicio AND @DataFinal
                    AND Motorista IS NOT NULL AND Motorista <> ''";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFinal", dataFinal);

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        #endregion

        #region Métodos Auxiliares

        private List<int> ObterClientesSelecionados()
        {
            List<int> clienteIds = new List<int>();

            if (rbTodos.Checked)
            {
                // Retornar null para indicar "todos"
                return null;
            }
            else if (rbMulti.Checked || rbUnico.Checked)
            {
                DataTable dtInseridos = (DataTable)dgvClientesInseridos.DataSource;
                foreach (DataRow row in dtInseridos.Rows)
                {
                    clienteIds.Add(Convert.ToInt32(row["ClienteId"]));
                }
            }

            return clienteIds.Count > 0 ? clienteIds : null;
        }

        private string ObterTituloCliente()
        {
            if (rbTodos.Checked)
            {
                return "Todos os Clientes";
            }
            else if (rbUnico.Checked)
            {
                DataTable dtInseridos = (DataTable)dgvClientesInseridos.DataSource;
                if (dtInseridos.Rows.Count > 0)
                {
                    return $"Cliente: {dtInseridos.Rows[0]["Nome"]}";
                }
            }
            else if (rbMulti.Checked)
            {
                DataTable dtInseridos = (DataTable)dgvClientesInseridos.DataSource;
                if (dtInseridos.Rows.Count > 0)
                {
                    return $"{dtInseridos.Rows.Count} Cliente(s) Selecionado(s)";
                }
            }

            return "Nenhum Cliente Selecionado";
        }

        private void LimparGrafico()
        {
            chartGrafico.Series.Clear();
            chartGrafico.Titles.Clear();
            chartGrafico.Titles.Add(new Title("Nenhum dado disponível", Docking.Top,
                new System.Drawing.Font("Segoe UI", 14F, FontStyle.Bold), Color.Gray));
        }

        #endregion

        #region Exportar PDF

        private void BtnExportarPDF_Click(object sender, EventArgs e)
        {
            if (chartGrafico.Series.Count == 0)
            {
                MessageBox.Show("Não há gráfico para exportar!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string nomeArquivo = $"Grafico_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string caminhoCompleto = Path.Combine(desktop, nomeArquivo);

                iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                PdfWriter.GetInstance(doc, new FileStream(caminhoCompleto, FileMode.Create));

                doc.Open();

                string tempImage = Path.Combine(Path.GetTempPath(), "temp_chart.png");
                chartGrafico.SaveImage(tempImage, ChartImageFormat.Png);

                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(tempImage);

                float pageWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
                float pageHeight = doc.PageSize.Height - doc.TopMargin - doc.BottomMargin;

                float scaleWidth = pageWidth / img.Width;
                float scaleHeight = pageHeight / img.Height;
                float scale = Math.Min(scaleWidth, scaleHeight);

                img.ScalePercent(scale * 100);

                doc.Add(img);
                doc.Close();

                if (File.Exists(tempImage))
                    File.Delete(tempImage);

                MessageBox.Show($"PDF exportado com sucesso!\n\nLocal: {caminhoCompleto}",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var result = MessageBox.Show("Deseja abrir o arquivo agora?", "Abrir PDF",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(caminhoCompleto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}