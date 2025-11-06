using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormControleMes : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        public DateTime? SelectedDate { get; private set; }

        private enum TipoGrafico
        {
            ServicosGeral,
            VeiculosMaisUsados,
            StatusLogisticaMensal,
            ServicosMensal
        }

        public ExpedicaoFormControleMes()
        {
            InitializeComponent();
            this.Load += FormControleMes_Load;
        }

        private void FormControleMes_Load(object sender, EventArgs e)
        {
            dtpDataControle.Value = DateTime.Now.Date;
            ConfigurarComboGraficos();
            ConfigurarChart();

            cbGraficos.SelectedIndexChanged += cbGraficos_SelectedIndexChanged;
            dtpDataControle.ValueChanged += dtpDataControle_ValueChanged;
        }

        private void ConfigurarComboGraficos()
        {
            cbGraficos.Items.Clear();
            cbGraficos.Items.Add("Serviços - Período Geral");
            cbGraficos.Items.Add("Veículos Mais Usados - Mensal");
            cbGraficos.Items.Add("Status Logística - Mensal");
            cbGraficos.Items.Add("Serviços - Mensal");
            cbGraficos.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGraficos.SelectedIndex = 0;
        }

        private void ConfigurarChart()
        {
            ChartGraficos.Series.Clear();
            ChartGraficos.ChartAreas.Clear();
            ChartGraficos.Legends.Clear();

            var chartArea = new ChartArea("MainArea");
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            chartArea.BackColor = Color.White;
            chartArea.BackSecondaryColor = Color.LightGray;
            chartArea.BackGradientStyle = GradientStyle.TopBottom;

            chartArea.BorderColor = Color.Silver;
            chartArea.BorderDashStyle = ChartDashStyle.Solid;

            chartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
            chartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);

            ChartGraficos.ChartAreas.Add(chartArea);

            var legend = new Legend("MainLegend");
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Regular);
            ChartGraficos.Legends.Add(legend);

            ChartGraficos.BackColor = Color.WhiteSmoke;
        }

        private async void cbGraficos_SelectedIndexChanged(object sender, EventArgs e)
        {
            await CarregarGraficoAsync();
        }

        private async void dtpDataControle_ValueChanged(object sender, EventArgs e)
        {
            if (cbGraficos.SelectedIndex > 0)
            {
                await CarregarGraficoAsync();
            }
        }

        private async Task CarregarGraficoAsync()
        {
            if (cbGraficos.SelectedIndex == -1) return;

            try
            {
                ChartGraficos.Series.Clear();
                ChartGraficos.Titles.Clear();

                switch (cbGraficos.SelectedIndex)
                {
                    case 0:
                        await CarregarGraficoServicosGeralAsync();
                        break;
                    case 1:
                        await CarregarGraficoVeiculosMaisUsadosAsync();
                        break;
                    case 2:
                        await CarregarGraficoStatusLogisticaMensalAsync();
                        break;
                    case 3:
                        await CarregarGraficoServicosMensalAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar gráfico: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CarregarGraficoServicosGeralAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                    SELECT 
                        Servico,
                        COUNT(*) as Total
                    FROM ControleLogistico WITH (NOLOCK)
                    WHERE Servico IS NOT NULL AND Servico <> ''
                    GROUP BY Servico
                    ORDER BY Total DESC";

                var cmd = new SqlCommand(sql, conn);
                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Nenhum dado encontrado para o período.", "Aviso");
                    return;
                }

                var series = new Series("Serviços");
                series.ChartType = SeriesChartType.Column;
                series.IsValueShownAsLabel = true;
                series.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);

                series.BorderWidth = 1;
                series.BorderColor = Color.DarkGray;
                series.LabelForeColor = Color.Black;
                series.LabelFormat = "N0";

                var cores = new Color[]
                {
                    Color.FromArgb(52, 152, 219),   // Azul
                    Color.FromArgb(46, 204, 113),   // Verde
                    Color.FromArgb(155, 89, 182),   // Roxo
                    Color.FromArgb(241, 196, 15),   // Amarelo
                    Color.FromArgb(231, 76, 60),    // Vermelho
                    Color.FromArgb(230, 126, 34)    // Laranja
                };

                int colorIndex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    string servico = row["Servico"].ToString();
                    int total = Convert.ToInt32(row["Total"]);

                    int pointIndex = series.Points.AddXY(servico, total);
                    series.Points[pointIndex].Color = cores[colorIndex % cores.Length];
                    series.Points[pointIndex].LabelForeColor = Color.Black;

                    colorIndex++;
                }

                ChartGraficos.Series.Add(series);

                var title = new Title("Serviços Realizados - Período Geral");
                title.Font = new System.Drawing.Font("Segoe UI", 14F, FontStyle.Bold);
                title.ForeColor = Color.FromArgb(44, 62, 80);
                ChartGraficos.Titles.Add(title);

                ChartGraficos.ChartAreas[0].AxisX.LabelStyle.Angle = 0;
                ChartGraficos.ChartAreas[0].AxisX.Interval = 1;
                ChartGraficos.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
                ChartGraficos.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.WordWrap;
            }
        }

        private async Task CarregarGraficoVeiculosMaisUsadosAsync()
        {
            DateTime dataInicio;
            DateTime dataFim;

            if (chkPeriodo.Checked)
            {
                dataInicio = dtpInicio.Value.Date;
                dataFim = dtpFim.Value.Date;
            }
            else
            {
                dataInicio = new DateTime(dtpDataControle.Value.Year, dtpDataControle.Value.Month, 1);
                dataFim = dataInicio.AddMonths(1).AddDays(-1);
            }

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                SELECT 
                    ModeloVeiculo,
                    COUNT(*) as Total
                FROM ControleLogistico WITH (NOLOCK)
                WHERE ModeloVeiculo IS NOT NULL 
                    AND ModeloVeiculo <> ''
                    AND Data >= @DataInicio 
                    AND Data <= @DataFim
                GROUP BY ModeloVeiculo
                ORDER BY Total DESC";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                cmd.Parameters.AddWithValue("@DataFim", dataFim);

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"Nenhum veículo utilizado entre {dataInicio:dd/MM/yyyy} e {dataFim:dd/MM/yyyy}.", "Aviso");
                    return;
                }

                var series = new Series("Veículos");
                series.ChartType = SeriesChartType.Pie;
                series.IsValueShownAsLabel = true;
                series.Label = "#VALX: #PERCENT{P1} (#VALY)";
                series.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);

                series.BorderColor = Color.White;
                series.BorderWidth = 2;
                series["PieLabelStyle"] = "Outside";

                foreach (DataRow row in dt.Rows)
                {
                    string modelo = row["ModeloVeiculo"].ToString();
                    int total = Convert.ToInt32(row["Total"]);
                    series.Points.AddXY(modelo, total);
                }

                if (series.Points.Count > 0)
                    series.Points[0]["Exploded"] = "true";

                ChartGraficos.Series.Add(series);

                var title = new Title($"Veículos Mais Usados - {dataInicio:dd/MM/yyyy} até {dataFim:dd/MM/yyyy}");
                title.Font = new System.Drawing.Font("Segoe UI", 14F, FontStyle.Bold);
                title.ForeColor = Color.FromArgb(44, 62, 80);
                ChartGraficos.Titles.Add(title);
            }
        }

        private async Task CarregarGraficoStatusLogisticaMensalAsync()
        {
            DateTime dataInicio;
            DateTime dataFim;

            if (chkPeriodo.Checked)
            {
                dataInicio = dtpInicio.Value.Date;
                dataFim = dtpFim.Value.Date;
            }
            else
            {
                dataInicio = new DateTime(dtpDataControle.Value.Year, dtpDataControle.Value.Month, 1);
                dataFim = dataInicio.AddMonths(1).AddDays(-1);
            }

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                SELECT 
                    StatusLogistica,
                    COUNT(*) as Total
                FROM ControleLogistico WITH (NOLOCK)
                WHERE StatusLogistica IS NOT NULL 
                    AND StatusLogistica <> ''
                    AND Data >= @DataInicio 
                    AND Data <= @DataFim
                GROUP BY StatusLogistica
                ORDER BY 
                    CASE StatusLogistica
                        WHEN 'Programado' THEN 1
                        WHEN 'Em execução' THEN 2
                        WHEN 'Concluído' THEN 3
                        WHEN 'Não Efetuado' THEN 4
                        ELSE 5
                    END";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                cmd.Parameters.AddWithValue("@DataFim", dataFim);

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"Nenhum status encontrado entre {dataInicio:dd/MM/yyyy} e {dataFim:dd/MM/yyyy}.", "Aviso");
                    return;
                }

                var series = new Series("Status");
                series.ChartType = SeriesChartType.Column;
                series.IsValueShownAsLabel = true;
                series.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);

                series.BorderWidth = 1;
                series.BorderColor = Color.DarkGray;
                series.LabelForeColor = Color.Black;
                series.LabelFormat = "N0";

                var coresStatus = new System.Collections.Generic.Dictionary<string, Color>
        {
            { "Programado", Color.FromArgb(52, 152, 219) },      // Azul
            { "Em execução", Color.FromArgb(241, 196, 15) },     // Amarelo
            { "Concluído", Color.FromArgb(46, 204, 113) },       // Verde
            { "Não Efetuado", Color.FromArgb(231, 76, 60) }      // Vermelho
        };

                foreach (DataRow row in dt.Rows)
                {
                    string status = row["StatusLogistica"].ToString();
                    int total = Convert.ToInt32(row["Total"]);

                    int pointIndex = series.Points.AddXY(status, total);

                    if (coresStatus.ContainsKey(status))
                        series.Points[pointIndex].Color = coresStatus[status];
                    else
                        series.Points[pointIndex].Color = Color.Gray;

                    series.Points[pointIndex].LabelForeColor = Color.Black;
                }

                ChartGraficos.Series.Add(series);

                var title = new Title($"Status Logística - {dataInicio:dd/MM/yyyy} até {dataFim:dd/MM/yyyy}");
                title.Font = new System.Drawing.Font("Segoe UI", 14F, FontStyle.Bold);
                title.ForeColor = Color.FromArgb(44, 62, 80);
                ChartGraficos.Titles.Add(title);

                ChartGraficos.ChartAreas[0].AxisX.LabelStyle.Angle = 0;
                ChartGraficos.ChartAreas[0].AxisX.Interval = 1;
                ChartGraficos.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
                ChartGraficos.ChartAreas[0].AxisX.LabelAutoFitStyle =
                    LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.WordWrap;
            }
        }

        private async Task CarregarGraficoServicosMensalAsync()
        {
            DateTime dataInicio;
            DateTime dataFim;

            if (chkPeriodo.Checked)
            {
                dataInicio = dtpInicio.Value.Date;
                dataFim = dtpFim.Value.Date;
            }
            else
            {
                dataInicio = new DateTime(dtpDataControle.Value.Year, dtpDataControle.Value.Month, 1);
                dataFim = dataInicio.AddMonths(1).AddDays(-1);
            }

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                SELECT 
                    Servico,
                    COUNT(*) as Total
                FROM ControleLogistico WITH (NOLOCK)
                WHERE Servico IS NOT NULL 
                    AND Servico <> ''
                    AND Data >= @DataInicio 
                    AND Data <= @DataFim
                GROUP BY Servico
                ORDER BY Total DESC";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                cmd.Parameters.AddWithValue("@DataFim", dataFim);

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"Nenhum serviço realizado entre {dataInicio:dd/MM/yyyy} e {dataFim:dd/MM/yyyy}.", "Aviso");
                    return;
                }

                var series = new Series("Serviços");
                series.ChartType = SeriesChartType.Column;
                series.IsValueShownAsLabel = true;
                series.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);

                series.BorderWidth = 1;
                series.BorderColor = Color.DarkGray;
                series.LabelForeColor = Color.Black;
                series.LabelFormat = "N0";

                var cores = new Color[]
                    {
                Color.FromArgb(52, 152, 219),
                Color.FromArgb(46, 204, 113),
                Color.FromArgb(155, 89, 182),
                Color.FromArgb(241, 196, 15),
                Color.FromArgb(231, 76, 60),
                Color.FromArgb(230, 126, 34)
                    };

                int colorIndex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    string servico = row["Servico"].ToString();
                    int total = Convert.ToInt32(row["Total"]);

                    int pointIndex = series.Points.AddXY(servico, total);
                    series.Points[pointIndex].Color = cores[colorIndex % cores.Length];
                    series.Points[pointIndex].LabelForeColor = Color.Black;

                    colorIndex++;
                }

                ChartGraficos.Series.Add(series);

                var title = new Title($"Serviços Realizados - {dataInicio:dd/MM/yyyy} até {dataFim:dd/MM/yyyy}");
                title.Font = new System.Drawing.Font("Segoe UI", 14F, FontStyle.Bold);
                title.ForeColor = Color.FromArgb(44, 62, 80);
                ChartGraficos.Titles.Add(title);

                ChartGraficos.ChartAreas[0].AxisX.LabelStyle.Angle = 0;
                ChartGraficos.ChartAreas[0].AxisX.Interval = 1;
                ChartGraficos.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
                ChartGraficos.ChartAreas[0].AxisX.LabelAutoFitStyle =
                    LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.WordWrap;
            }
        }

        private void btnSelecionar_Click(object sender, EventArgs e)
        {
            SelectedDate = dtpDataControle.Value.Date;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChartGraficos.Series.Count == 0)
                {
                    MessageBox.Show("Nenhum gráfico carregado para exportar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Arquivo PDF (*.pdf)|*.pdf";
                    sfd.FileName = $"Grafico_{cbGraficos.Text.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        string tempImg = Path.GetTempFileName() + ".png";
                        ChartGraficos.SaveImage(tempImg, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);

                        using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 25, 25);
                            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                            doc.Open();

                            var titulo = new Paragraph($"Relatório - {cbGraficos.Text}\n\n",
                                new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD));
                            titulo.Alignment = Element.ALIGN_CENTER;
                            doc.Add(titulo);

                            iTextSharp.text.Image chartImg = iTextSharp.text.Image.GetInstance(tempImg);
                            chartImg.Alignment = Element.ALIGN_CENTER;

                            chartImg.ScaleToFit(PageSize.A4.Rotate().Width - 50, PageSize.A4.Rotate().Height - 100);

                            doc.Add(chartImg);

                            doc.Close();
                            writer.Close();
                        }

                        if (File.Exists(tempImg))
                            File.Delete(tempImg);

                        MessageBox.Show("Gráfico exportado com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAplicarPeriodo_Click(object sender, EventArgs e)
        {
            if (cbGraficos.SelectedIndex == 0)
            {
                MessageBox.Show("O gráfico de Período Geral não usa intervalo de datas.");
                return;
            }

            await CarregarGraficoAsync();
        }
    }
}