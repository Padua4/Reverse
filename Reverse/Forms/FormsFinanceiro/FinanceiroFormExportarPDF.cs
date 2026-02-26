using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Reverse.Forms.FormsFinanceiro
{
    public partial class FinanceiroFormExportarPDF : Form
    {
        private readonly string connectionString =
        ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;

        public FinanceiroFormExportarPDF(int _usuarioId)
        {
            InitializeComponent();
        }

        private async void FormExportaPDF_Load(object sender, EventArgs e)
        {
            try
            {
                var taskAnos = CarregarAnosAsync();
                var taskCategorias = CarregarCategoriasAsync();
                var taskVerificarCategoria = VerificarCategoriasAsync();

                await Task.WhenAll(taskAnos, taskCategorias);

                int anoAtual = DateTime.Now.Year;
                if (cmbAno.Items.Contains(anoAtual))
                {
                    cmbAno.SelectedItem = anoAtual;
                }
                else if (cmbAno.Items.Count > 0)
                {
                    cmbAno.SelectedIndex = 0;
                }

                if (cmbAno.SelectedItem != null)
                {
                    int anoSelecionado = Convert.ToInt32(cmbAno.SelectedItem);
                    await CarregarGraficoAsync(anoSelecionado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar: {ex.Message}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CarregarAnosAsync()
        {
            cmbAno.Items.Clear();

            const string query = @"
        SELECT DISTINCT YEAR(DataVencimento) AS Ano 
        FROM (
            SELECT DataVencimento FROM ContasPagar
            UNION ALL
            SELECT DataVencimento FROM ContasReceber
        ) AS TodasDatas
        WHERE YEAR(DataVencimento) IS NOT NULL
        ORDER BY Ano DESC";

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            cmbAno.Items.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            if (cmbAno.Items.Count > 0)
                cmbAno.SelectedIndex = 0;
        }
        private async Task CarregarCategoriasAsync()
        {
            const string query = "SELECT Id, Nome FROM Categorias ORDER BY Nome";

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var table = new DataTable();
                        table.Load(reader);

                        cmbCategoria.DataSource = table;
                        cmbCategoria.DisplayMember = "Nome";
                        cmbCategoria.ValueMember = "Id";
                    }
                }
            }

            if (cmbCategoria.Items.Count > 0)
            {
                cmbCategoria.SelectedIndex = 0;
            }
        }

        private async void cmbAno_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAno.SelectedItem != null)
            {
                int anoSelecionado = Convert.ToInt32(cmbAno.SelectedItem);

                try
                {
                    cmbAno.Enabled = false;
                    this.Cursor = Cursors.WaitCursor;

                    await CarregarGraficoAsync(anoSelecionado);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar o gráfico: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    cmbAno.Enabled = true; // Reabilita ao finalizar
                    this.Cursor = Cursors.Default; // Restaura o cursor
                }
            }
        }

        private async Task VerificarCategoriasAsync()
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    var query = "SELECT Id, Nome FROM Categorias WHERE Nome LIKE '%prolabore%' OR Nome LIKE '%pró-labore%' OR Nome LIKE '%PROLABORE%'";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            System.Diagnostics.Debug.WriteLine($"Categoria encontrada: ID={reader["Id"]}, Nome={reader["Nome"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar categorias: {ex.Message}");
            }
        }

        private async Task CarregarGraficoAsync(int ano)
        {
            crtGrafico.Series.Clear();
            crtGrafico.Titles.Clear();
            crtGrafico.ChartAreas.Clear();
            crtGrafico.Legends.Clear();
            crtGrafico.Titles.Add($"Lucros x Gastos - {ano}");
            var area = new ChartArea("MainArea");
            area.BackColor = System.Drawing.Color.WhiteSmoke;
            area.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            area.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            area.AxisX.Title = "Meses";
            area.AxisY.Title = "Valor (R$)";
            area.AxisX.TitleFont = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
            area.AxisY.TitleFont = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
            crtGrafico.ChartAreas.Add(area);
            var serieLucros = new Series("Lucros") { ChartType = SeriesChartType.Line, Color = System.Drawing.Color.RoyalBlue, BorderWidth = 3, MarkerStyle = MarkerStyle.Circle, MarkerSize = 8, MarkerColor = System.Drawing.Color.DarkBlue };
            crtGrafico.Series.Add(serieLucros);
            var serieGastos = new Series("Gastos") { ChartType = SeriesChartType.Line, Color = System.Drawing.Color.OrangeRed, BorderWidth = 3, MarkerStyle = MarkerStyle.Square, MarkerSize = 8, MarkerColor = System.Drawing.Color.DarkOrange };
            crtGrafico.Series.Add(serieGastos);
            var serieProlabore = new Series("Prolabore")
            {
                ChartType = SeriesChartType.Line,
                Color = System.Drawing.Color.Purple,
                BorderWidth = 3,
                MarkerStyle = MarkerStyle.Triangle,
                MarkerSize = 8,
                MarkerColor = System.Drawing.Color.DarkMagenta
            };
            crtGrafico.Series.Add(serieProlabore);
            var legend = new Legend { Docking = Docking.Bottom, Alignment = StringAlignment.Center, Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold), BackColor = System.Drawing.Color.WhiteSmoke, BorderColor = System.Drawing.Color.Gray };
            crtGrafico.Legends.Add(legend);

            decimal[] lucros = new decimal[12];
            decimal[] gastos = new decimal[12];
            decimal[] prolabore = new decimal[12];

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                var query = @"
            WITH DadosMensais AS (
                -- Lucros (ContasReceber)
                SELECT 
                    MONTH(DataVencimento) AS Mes, 
                    SUM(Valor) as Total, 
                    'Lucro' AS Tipo 
                FROM ContasReceber 
                WHERE YEAR(DataVencimento) = @ano 
                GROUP BY MONTH(DataVencimento)

                UNION ALL

                -- Gastos (excluindo Prolabore)
                SELECT 
                    MONTH(cp.DataVencimento) AS Mes, 
                    SUM(cp.ValorPago) AS Total, 
                    'Gasto' AS Tipo 
                FROM ContasPagar cp
                WHERE YEAR(cp.DataVencimento) = @ano 
                  AND cp.CategoriaId <> 49
                GROUP BY MONTH(cp.DataVencimento)

                UNION ALL

                -- Prolabore (apenas a categoria específica)
                SELECT 
                    MONTH(cp.DataVencimento) AS Mes, 
                    SUM(cp.ValorPago) AS Total, 
                    'Prolabore' AS Tipo 
                FROM ContasPagar cp
                WHERE YEAR(cp.DataVencimento) = @ano 
                  AND cp.CategoriaId = 49
                GROUP BY MONTH(cp.DataVencimento)
            )
            SELECT Mes, Tipo, Total FROM DadosMensais;";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ano", ano);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int mes = reader.GetInt32(0);
                            string tipo = reader.GetString(1);
                            decimal total = reader.GetDecimal(2);

                            if (tipo == "Lucro")
                            {
                                lucros[mes - 1] = total;
                            }
                            else if (tipo == "Gasto")
                            {
                                gastos[mes - 1] = total;
                            }
                            else if (tipo == "Prolabore")
                            {
                                prolabore[mes - 1] = total;
                            }
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"=== DADOS CARREGADOS PARA O ANO {ano} ===");
            for (int i = 0; i < 12; i++)
            {
                System.Diagnostics.Debug.WriteLine($"Mês {i + 1}: Lucros={lucros[i]}, Gastos={gastos[i]}, Prolabore={prolabore[i]}");
            }

            string[] meses = { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun",
                       "Jul", "Ago", "Set", "Out", "Nov", "Dez" };
            for (int i = 0; i < 12; i++)
            {
                serieLucros.Points.AddXY(meses[i], lucros[i]);
                serieGastos.Points.AddXY(meses[i], gastos[i]);
                serieProlabore.Points.AddXY(meses[i], prolabore[i]);
            }
        }

        private void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCategoria.SelectedValue == null || cmbCategoria.SelectedValue is DataRowView)
                return;

            int categoriaId = Convert.ToInt32(cmbCategoria.SelectedValue);

            // Limpa a imagem anterior e libera os recursos CORRETAMENTE
            if (imgCategoria.Image != null)
            {
                var oldImage = imgCategoria.Image;
                imgCategoria.Image = null; // Remove a referência ANTES do dispose
                oldImage.Dispose(); // Agora faz o dispose
            }

            int[] categoriasComImagem = { 2,3,8,11,15,17,23,27,28,29,
                          35,36,41,42,43,44,45,47,48,
                          50,53,56,57 };

            if (categoriasComImagem.Contains(categoriaId))
            {
                string pasta = Path.Combine(Application.StartupPath, "ImagensCategorias");
                string caminhoPng = Path.Combine(pasta, $"{categoriaId}.png");
                string caminhoJpg = Path.Combine(pasta, $"{categoriaId}.jpg");

                string caminho = File.Exists(caminhoPng) ? caminhoPng :
                                 File.Exists(caminhoJpg) ? caminhoJpg : null;

                if (caminho != null)
                {
                    using (var fileStream = new FileStream(caminho, FileMode.Open, FileAccess.Read))
                    using (var originalImage = System.Drawing.Image.FromStream(fileStream))
                    {
                        imgCategoria.Image = new Bitmap(originalImage);
                        imgCategoria.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
        }

        private async void btnGraficoPDF_Click(object sender, EventArgs e)
        {
            if (cmbAno.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione um ano para gerar o gráfico.", "Atenção",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF File|*.pdf";
                sfd.FileName = $"Grafico_{cmbAno.SelectedItem}.pdf";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        btnGraficoPDF.Enabled = false;

                        Bitmap chartBitmap = null;
                        chartBitmap = new Bitmap(crtGrafico.Width, crtGrafico.Height);
                        crtGrafico.DrawToBitmap(chartBitmap, new System.Drawing.Rectangle(0, 0, chartBitmap.Width, chartBitmap.Height));

                        await Task.Run(() =>
                        {
                            try
                            {
                                using (var doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10))
                                using (var stream = new FileStream(sfd.FileName, FileMode.Create))
                                {
                                    PdfWriter.GetInstance(doc, stream);
                                    doc.Open();

                                    var img = iTextSharp.text.Image.GetInstance(chartBitmap, System.Drawing.Imaging.ImageFormat.Png);
                                    img.ScaleToFit(doc.PageSize.Width - 20, doc.PageSize.Height - 20);
                                    img.Alignment = Element.ALIGN_CENTER;

                                    doc.Add(img);
                                    doc.Close();
                                }
                            }
                            finally
                            {
                                chartBitmap?.Dispose();
                            }
                        });

                        MessageBox.Show("PDF do gráfico gerado com sucesso!", "Sucesso",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                        btnGraficoPDF.Enabled = true;
                    }
                }
            }
        }


        public class BordaPreta : iTextSharp.text.pdf.PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                PdfContentByte cb = writer.DirectContent;
                cb.SetColorStroke(BaseColor.BLACK);
                cb.SetLineWidth(2f);

                cb.Rectangle(
                    document.LeftMargin - 20,
                    document.BottomMargin - 20,
                    document.PageSize.Width - (document.LeftMargin + document.RightMargin) + 40,
                    document.PageSize.Height - (document.TopMargin + document.BottomMargin) + 40
                );

                cb.Stroke();
            }
        }

        private void btnCategoriaExp_Click(object sender, EventArgs e)
        {
            if (cmbCategoria.SelectedItem == null)
            {
                MessageBox.Show("Selecione uma categoria.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtValor.Text) ||
            !decimal.TryParse(txtValor.Text, NumberStyles.Currency, new CultureInfo("pt-BR"), out decimal valorDecimal))
            {
                MessageBox.Show("Por favor, insira um valor numérico válido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string valor = valorDecimal.ToString("C", new CultureInfo("pt-BR"));

            string categoria = cmbCategoria.Text;
            string data = dtpData.Value.ToString("dd/MM/yyyy");
            string pix = txtPix.Text;
            string referencia = chbFechamento.Checked ? "FECHAMENTO DE MÊS" : txtREF.Text.ToUpper();

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF File|*.pdf";
                sfd.FileName = $"Relatorio_{categoria}.pdf";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var doc = new iTextSharp.text.Document(PageSize.A4, 40, 40, 40, 40))
                    {
                        PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));

                        writer.PageEvent = new BordaPreta();

                        doc.Open();

                        var titulo = new Paragraph(categoria, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, BaseColor.BLACK));
                        titulo.Alignment = Element.ALIGN_CENTER;
                        doc.Add(titulo);

                        var linha = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -2)));
                        doc.Add(linha);
                        doc.Add(new Paragraph(" "));

                        if (imgCategoria.Image != null)
                        {
                            using (var ms = new MemoryStream())
                            {
                                imgCategoria.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                var pdfImg = iTextSharp.text.Image.GetInstance(ms.ToArray());

                                pdfImg.ScaleToFit(doc.PageSize.Width - 80, 350);
                                pdfImg.Alignment = Element.ALIGN_CENTER;
                                doc.Add(pdfImg);
                            }
                            doc.Add(new Paragraph(" "));
                        }

                        var refFonte = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLUE);
                        var refParagrafo = new Paragraph(referencia, refFonte);
                        refParagrafo.Alignment = Element.ALIGN_CENTER;
                        doc.Add(refParagrafo);

                        var linhaRef = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 60f, BaseColor.BLUE, Element.ALIGN_CENTER, -2)));
                        doc.Add(linhaRef);
                        doc.Add(new Paragraph(" "));

                        var fonteCaixa = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);

                        void AddCaixa(string conteudo)
                        {
                            PdfPTable tabela = new PdfPTable(1);
                            tabela.WidthPercentage = 100;

                            PdfPCell cell = new PdfPCell(new Phrase(conteudo, fonteCaixa))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                Padding = 12,
                                BorderWidth = 1,
                                BorderColor = BaseColor.BLACK,
                                BackgroundColor = new BaseColor(240, 240, 240)
                            };

                            tabela.AddCell(cell);
                            doc.Add(tabela);
                            doc.Add(new Paragraph(" "));
                        }

                        AddCaixa($"Valor: {valor}");
                        AddCaixa($"Data: {data}");
                        AddCaixa($"Chave Pix: {pix}");

                        doc.Close();
                    }
                    MessageBox.Show("PDF exportado com sucesso!",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void txtValor_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtValor.Text, out decimal valor))
            {
                txtValor.Text = valor.ToString("C", new System.Globalization.CultureInfo("pt-BR"));
            }
            else
            {
                txtValor.Text = "R$ 0,00";
            }
        }

        private void chbFechamento_CheckedChanged(object sender, EventArgs e)
        {
            if (chbFechamento.Checked)
            {
                txtREF.Text = "Fechamento de mês";
                txtREF.Enabled = false;
            }
            else
            {
                txtREF.Text = string.Empty;
                txtREF.Enabled = true;
            }
        }

    }
}