using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Configuration;

namespace SeuProjeto
{
    public partial class FinanceiroFormGraficos : Form
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;

        private Timer updateTimer;
        private bool isUpdating = false;

        private Chart chartLucros;
        private Chart chartGastos;
        private Chart chartComparativo;

        public FinanceiroFormGraficos(int _usuarioId)
        {
            InitializeComponent();
            InitializeCharts();
            SetupUpdateTimer();
            _ = LoadInitialDataAsync();

            this.FormClosing += (s, e) => CleanupResources();
        }

        private void InitializeCharts()
        {
            chartLucros = new Chart();
            chartLucros.Dock = DockStyle.Fill;
            chartLucros.BackColor = Color.White;

            ChartArea areaLucros = new ChartArea("MainArea");
            areaLucros.BackColor = Color.FromArgb(240, 248, 255);
            areaLucros.AxisX.MajorGrid.LineColor = Color.LightGray;
            areaLucros.AxisY.MajorGrid.LineColor = Color.LightGray;
            areaLucros.AxisX.Title = "Dias do Mês";
            areaLucros.AxisY.Title = "Lucros (R$)";
            areaLucros.AxisX.TitleFont = new Font("Arial", 10, FontStyle.Bold);
            areaLucros.AxisY.TitleFont = new Font("Arial", 10, FontStyle.Bold);
            chartLucros.ChartAreas.Add(areaLucros);

            Series serieLucros = new Series("Lucros Mensais");
            serieLucros.ChartType = SeriesChartType.Line;
            serieLucros.Color = Color.Green;
            serieLucros.BorderWidth = 3;
            serieLucros.MarkerStyle = MarkerStyle.Circle;
            serieLucros.MarkerSize = 6;
            serieLucros.MarkerColor = Color.DarkGreen;
            chartLucros.Series.Add(serieLucros);

            Title titleLucros = new Title("Lucros do Mês Atual");
            titleLucros.Font = new Font("Arial", 14, FontStyle.Bold);
            titleLucros.ForeColor = Color.DarkBlue;
            chartLucros.Titles.Add(titleLucros);

            chartGastos = new Chart();
            chartGastos.Dock = DockStyle.Fill;
            chartGastos.BackColor = Color.White;

            ChartArea areaGastos = new ChartArea("MainArea");
            areaGastos.BackColor = Color.FromArgb(255, 248, 240);
            areaGastos.AxisX.MajorGrid.LineColor = Color.LightGray;
            areaGastos.AxisY.MajorGrid.LineColor = Color.LightGray;
            areaGastos.AxisX.Title = "Dias do Mês";
            areaGastos.AxisY.Title = "Gastos (R$)";
            areaGastos.AxisX.TitleFont = new Font("Arial", 10, FontStyle.Bold);
            areaGastos.AxisY.TitleFont = new Font("Arial", 10, FontStyle.Bold);
            chartGastos.ChartAreas.Add(areaGastos);

            Series serieGastos = new Series("Gastos Mensais");
            serieGastos.ChartType = SeriesChartType.Line;
            serieGastos.Color = Color.Red;
            serieGastos.BorderWidth = 3;
            serieGastos.MarkerStyle = MarkerStyle.Circle;
            serieGastos.MarkerSize = 6;
            serieGastos.MarkerColor = Color.DarkRed;
            chartGastos.Series.Add(serieGastos);

            Title titleGastos = new Title("Gastos do Mês Atual");
            titleGastos.Font = new Font("Arial", 14, FontStyle.Bold);
            titleGastos.ForeColor = Color.DarkBlue;
            chartGastos.Titles.Add(titleGastos);

            chartComparativo = new Chart();
            chartComparativo.Dock = DockStyle.Fill;
            chartComparativo.BackColor = Color.White;

            ChartArea areaComparativo = new ChartArea("MainArea");
            areaComparativo.BackColor = Color.FromArgb(248, 255, 248);
            areaComparativo.AxisX.MajorGrid.LineColor = Color.LightGray;
            areaComparativo.AxisY.MajorGrid.LineColor = Color.LightGray;
            areaComparativo.AxisX.Title = "Meses";
            areaComparativo.AxisY.Title = "Valor (R$)";
            areaComparativo.AxisX.TitleFont = new Font("Arial", 10, FontStyle.Bold);
            areaComparativo.AxisY.TitleFont = new Font("Arial", 10, FontStyle.Bold);
            chartComparativo.ChartAreas.Add(areaComparativo);

            Series serieAnoAtual = new Series("Ano Atual");
            serieAnoAtual.ChartType = SeriesChartType.Line;
            serieAnoAtual.Color = Color.Blue;
            serieAnoAtual.BorderWidth = 3;
            serieAnoAtual.MarkerStyle = MarkerStyle.Circle;
            serieAnoAtual.MarkerSize = 6;
            serieAnoAtual.MarkerColor = Color.DarkBlue;
            chartComparativo.Series.Add(serieAnoAtual);

            Series serieAnoAnterior = new Series("Ano Anterior");
            serieAnoAnterior.ChartType = SeriesChartType.Line;
            serieAnoAnterior.Color = Color.Orange;
            serieAnoAnterior.BorderWidth = 3;
            serieAnoAnterior.MarkerStyle = MarkerStyle.Square;
            serieAnoAnterior.MarkerSize = 6;
            serieAnoAnterior.MarkerColor = Color.DarkOrange;
            chartComparativo.Series.Add(serieAnoAnterior);

            Title titleComparativo = new Title("Comparativo Anual - Lucro Líquido");
            titleComparativo.Font = new Font("Arial", 14, FontStyle.Bold);
            titleComparativo.ForeColor = Color.DarkBlue;
            chartComparativo.Titles.Add(titleComparativo);

            Legend legend = new Legend();
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            chartComparativo.Legends.Add(legend);

            panelLucros.Controls.Add(chartLucros);
            panelGastos.Controls.Add(chartGastos);
            panelComparativo.Controls.Add(chartComparativo);
        }

        private void SetupUpdateTimer()
        {
            updateTimer = new Timer();
            updateTimer.Interval = 600000;
            updateTimer.Tick += async (s, e) => await UpdateTimer_TickAsync();
            updateTimer.Start();
        }

        private async Task UpdateTimer_TickAsync()
        {
            if (!isUpdating)
            {
                await LoadInitialDataAsync();
            }
        }

        private async Task LoadInitialDataAsync()
        {
            if (isUpdating) return;

            try
            {
                isUpdating = true;

                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    await LoadAllChartsDataAsync(conn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isUpdating = false;
            }
        }

        private async Task LoadAllChartsDataAsync(SqlConnection conn)
        {
            const string query = @"
                -- Dados diários do mês atual (CORRIGIDO: usando DataVencimento igual ao FormExportarPDF)
                WITH DadosDiarios AS (
                    SELECT 
                        'Receita' as Tipo,
                        DAY(DataVencimento) as Dia,
                        SUM(Valor) as Total
                    FROM ContasReceber
                    WHERE YEAR(DataVencimento) = YEAR(GETDATE())
                        AND MONTH(DataVencimento) = MONTH(GETDATE())
                        AND DataVencimento IS NOT NULL
                    GROUP BY DAY(DataVencimento)
                    
                    UNION ALL
                    
                    SELECT 
                        'Gasto' as Tipo,
                        DAY(DataVencimento) as Dia,
                        SUM(Valor) as Total
                    FROM ContasPagar
                    WHERE YEAR(DataVencimento) = YEAR(GETDATE())
                        AND MONTH(DataVencimento) = MONTH(GETDATE())
                        AND DataVencimento IS NOT NULL
                    GROUP BY DAY(DataVencimento)
                ),
                -- Dados mensais dos últimos 2 anos (CORRIGIDO: usando DataVencimento)
                DadosMensais AS (
                    SELECT 
                        'Receita' as Tipo,
                        YEAR(DataVencimento) as Ano,
                        MONTH(DataVencimento) as Mes,
                        SUM(Valor) as Total
                    FROM ContasReceber
                    WHERE DataVencimento IS NOT NULL
                        AND YEAR(DataVencimento) IN (YEAR(GETDATE()), YEAR(GETDATE()) - 1)
                    GROUP BY YEAR(DataVencimento), MONTH(DataVencimento)
                    
                    UNION ALL
                    
                    SELECT 
                        'Gasto' as Tipo,
                        YEAR(DataVencimento) as Ano,
                        MONTH(DataVencimento) as Mes,
                        SUM(Valor) as Total
                    FROM ContasPagar
                    WHERE DataVencimento IS NOT NULL
                        AND YEAR(DataVencimento) IN (YEAR(GETDATE()), YEAR(GETDATE()) - 1)
                    GROUP BY YEAR(DataVencimento), MONTH(DataVencimento)
                )
                SELECT 'DIARIO' as Dataset, Tipo, Dia as Periodo, 0 as Ano, 0 as Mes, Total
                FROM DadosDiarios
                UNION ALL
                SELECT 'MENSAL' as Dataset, Tipo, 0 as Periodo, Ano, Mes, Total
                FROM DadosMensais
                ORDER BY Dataset, Ano, Mes, Periodo";

            using (var cmd = new SqlCommand(query, conn))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    chartLucros.Series["Lucros Mensais"].Points.Clear();
                    chartGastos.Series["Gastos Mensais"].Points.Clear();
                    chartComparativo.Series["Ano Atual"].Points.Clear();
                    chartComparativo.Series["Ano Anterior"].Points.Clear();

                    var receitasDiarias = new decimal[32];
                    var gastosDiarios = new decimal[32];
                    var receitasMensais = new decimal[2, 13];
                    var gastosMensais = new decimal[2, 13];

                    int anoAtual = DateTime.Now.Year;

                    while (await reader.ReadAsync())
                    {
                        string dataset = reader["Dataset"].ToString();
                        string tipo = reader["Tipo"].ToString();
                        decimal total = Convert.ToDecimal(reader["Total"]);

                        if (dataset == "DIARIO")
                        {
                            int dia = Convert.ToInt32(reader["Periodo"]);
                            if (tipo == "Receita")
                                receitasDiarias[dia] += total;
                            else
                                gastosDiarios[dia] += total;
                        }
                        else
                        {
                            int ano = Convert.ToInt32(reader["Ano"]);
                            int mes = Convert.ToInt32(reader["Mes"]);
                            int anoIndex = ano == anoAtual ? 1 : 0;

                            if (tipo == "Receita")
                                receitasMensais[anoIndex, mes] = total;
                            else
                                gastosMensais[anoIndex, mes] = total;
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"=== DEBUG GRÁFICOS ===");
                    for (int i = 1; i <= 31; i++)
                    {
                        if (receitasDiarias[i] > 0 || gastosDiarios[i] > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Dia {i}: Receitas={receitasDiarias[i]:C}, Gastos={gastosDiarios[i]:C}");
                        }
                    }

                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate {
                            UpdateChartsWithData(receitasDiarias, gastosDiarios, receitasMensais, gastosMensais, anoAtual);
                        });
                    }
                    else
                    {
                        UpdateChartsWithData(receitasDiarias, gastosDiarios, receitasMensais, gastosMensais, anoAtual);
                    }
                }
            }
        }

        private void UpdateChartsWithData(decimal[] receitasDiarias, decimal[] gastosDiarios,
                                        decimal[,] receitasMensais, decimal[,] gastosMensais, int anoAtual)
        {
            decimal lucroAcumulado = 0;
            for (int dia = 1; dia <= 31; dia++)
            {
                if (receitasDiarias[dia] > 0 || gastosDiarios[dia] > 0)
                {
                    decimal lucroNoDia = receitasDiarias[dia] - gastosDiarios[dia];
                    lucroAcumulado += lucroNoDia;
                    chartLucros.Series["Lucros Mensais"].Points.AddXY(dia, lucroAcumulado);

                    System.Diagnostics.Debug.WriteLine($"Dia {dia}: Receita={receitasDiarias[dia]:C}, Gasto={gastosDiarios[dia]:C}, Lucro Acumulado={lucroAcumulado:C}");
                }
            }

            decimal gastoAcumulado = 0;
            for (int dia = 1; dia <= 31; dia++)
            {
                if (gastosDiarios[dia] > 0)
                {
                    gastoAcumulado += gastosDiarios[dia];
                    chartGastos.Series["Gastos Mensais"].Points.AddXY(dia, gastoAcumulado);
                }
            }

            string[] meses = {"Jan", "Fev", "Mar", "Abr", "Mai", "Jun",
                             "Jul", "Ago", "Set", "Out", "Nov", "Dez"};

            for (int mes = 1; mes <= 12; mes++)
            {
                decimal lucroAnoAtual = receitasMensais[1, mes] - gastosMensais[1, mes];
                if (lucroAnoAtual != 0)
                {
                    chartComparativo.Series["Ano Atual"].Points.AddXY(meses[mes - 1], lucroAnoAtual);
                }

                decimal lucroAnoAnterior = receitasMensais[0, mes] - gastosMensais[0, mes];
                if (lucroAnoAnterior != 0)
                {
                    chartComparativo.Series["Ano Anterior"].Points.AddXY(meses[mes - 1], lucroAnoAnterior);
                }
            }

            chartLucros.Invalidate();
            chartGastos.Invalidate();
            chartComparativo.Invalidate();
        }

        public async Task RefreshChartsAsync()
        {
            await LoadInitialDataAsync();
        }

        public void RefreshCharts()
        {
            _ = RefreshChartsAsync();
        }

        private void CleanupResources()
        {
            updateTimer?.Stop();
            updateTimer?.Dispose();
        }
    }
}