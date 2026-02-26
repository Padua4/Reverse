using Org.BouncyCastle.Asn1.Pkcs;
using PdfiumViewer;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormLaudo : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;

        private int _clienteId;
        private DateTime _mesAno;
        private decimal _pesoTotal;
        private string _numeroLaudo;
        private PdfViewer _pdfViewer;

        private string _razaoSocialGerador;
        private string _enderecoGerador;
        private string _municipioGerador;
        private string _ufGerador;
        private string _cnpjGerador;

        public ExpedicaoFormLaudo(int clienteId, DateTime mesAno, decimal? pesoTotalOpt = null)
        {
            InitializeComponent();
            _clienteId = clienteId;
            _mesAno = new DateTime(mesAno.Year, mesAno.Month, 1);
            _pesoTotal = pesoTotalOpt ?? 0;

            Load += FormLaudo_Load;
            btnExportarLaudo.Click += btnExportarLaudo_Click;
        }

        private async void FormLaudo_Load(object sender, EventArgs e)
        {
            _pdfViewer = new PdfViewer { Dock = DockStyle.Fill };
            pnlVisualizer.Controls.Clear();
            pnlVisualizer.Controls.Add(_pdfViewer);

            txtNomeComum.Text = string.IsNullOrWhiteSpace(txtNomeComum.Text) ? "DESCARTES OBSOLETOS" : txtNomeComum.Text;
            txtConama.Text = string.IsNullOrWhiteSpace(txtConama.Text) ? "A099 – OUTROS RESÍDUOS NÃO PERIGOSOS" : txtConama.Text;
            txtAcondicionamento.Text = string.IsNullOrWhiteSpace(txtAcondicionamento.Text) ? "A GRANEL" : txtAcondicionamento.Text;
            txtClasse.Text = string.IsNullOrWhiteSpace(txtClasse.Text) ? "II. I" : txtClasse.Text;
            txtEstadoFisico.Text = string.IsNullOrWhiteSpace(txtEstadoFisico.Text) ? "SÓLIDOS" : txtEstadoFisico.Text;

            txtRazaoDLD.Text = string.IsNullOrWhiteSpace(txtRazaoDLD.Text) ? "DLD SOLUCOES EM LOGISTICA REVERSA, GESTAO E RECICLAGEM LTDA" : txtRazaoDLD.Text;
            txtEnderecoDLD.Text = string.IsNullOrWhiteSpace(txtEnderecoDLD.Text) ? "AVENIDA MELVIN JONES, 2851 – JARDIM TANGARÁ" : txtEnderecoDLD.Text;
            txtCNPJDLD.Text = string.IsNullOrWhiteSpace(txtCNPJDLD.Text) ? "37.540.504/0001-04" : txtCNPJDLD.Text;
            txtIEDLD.Text = string.IsNullOrWhiteSpace(txtIEDLD.Text) ? "182.261.090.118" : txtIEDLD.Text;
            txtLODLD.Text = string.IsNullOrWhiteSpace(txtLODLD.Text) ? "30006705" : txtLODLD.Text;

            await CarregarDadosGeradorAsync();

            bool temCertificado = !string.IsNullOrWhiteSpace(_numeroLaudo);
            btnExportarLaudo.Enabled = temCertificado;

            if (!temCertificado)
            {
                MessageBox.Show("Laudo só pode ser gerado após emissão do certificado deste cliente no mês selecionado.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            var caminhoPreview = await GerarPDFPreviewTemporal();
            MostrarPreviewPDF(caminhoPreview);
        }

        private async Task CarregarDadosGeradorAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                SELECT c.RazaoSocial, c.RuaEntrega, c.NumeroEntrega, c.BairroEntrega,
                       c.MunicipioEntrega, c.EstadoEntrega, c.CPF_CNPJ,
                       (SELECT SUM(Peso) FROM BalancoMassa 
                        WHERE ClienteId = @ClienteId AND MesAno = @MesAno) AS PesoTotal,
                       (SELECT TOP 1 NumeroCertificado FROM CertificadosEmitidos 
                        WHERE ClienteId = @ClienteId AND MesAno = @MesAno 
                        ORDER BY DataEmissao DESC) AS NumeroLaudo
                FROM Clientes c
                WHERE c.ClienteId = @ClienteId";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClienteId", _clienteId);
                cmd.Parameters.AddWithValue("@MesAno", _mesAno);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        _razaoSocialGerador = reader["RazaoSocial"]?.ToString() ?? "";
                        _enderecoGerador = $"{reader["RuaEntrega"]}, {reader["NumeroEntrega"]} - {reader["BairroEntrega"]}";
                        _municipioGerador = reader["MunicipioEntrega"]?.ToString() ?? "";
                        _ufGerador = reader["EstadoEntrega"]?.ToString() ?? "";
                        _cnpjGerador = FormatarCNPJ(reader["CPF_CNPJ"]?.ToString());

                        if (_pesoTotal <= 0 && reader["PesoTotal"] != DBNull.Value)
                            _pesoTotal = Convert.ToDecimal(reader["PesoTotal"]);

                        _numeroLaudo = reader["NumeroLaudo"]?.ToString();
                    }
                }
            }
        }

        private string FormatarCNPJ(string cnpjRaw)
        {
            string digits = new string((cnpjRaw ?? "").Where(char.IsDigit).ToArray());
            if (digits.Length == 14)
                return Convert.ToUInt64(digits).ToString(@"00\.000\.000\/0000\-00");
            if (digits.Length == 11)
                return Convert.ToUInt64(digits).ToString(@"000\.000\.000\-00");
            return cnpjRaw ?? "";
        }

        private async Task<string> GerarPDFPreviewTemporal()
        {
            string pastaTemp = Path.GetTempPath();
            string file = Path.Combine(pastaTemp, $"Preview_Laudo_{Guid.NewGuid()}.pdf");
            await GerarPDFLaudoEm(file);
            return file;
        }

        private void MostrarPreviewPDF(string caminho)
        {
            _pdfViewer.Document?.Dispose();
            _pdfViewer.Document = PdfiumViewer.PdfDocument.Load(caminho);
        }

        private async Task AtualizarStatusLaudoTicketsAsync()
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    DateTime inicioMes = _mesAno;
                    DateTime fimMes = _mesAno.AddMonths(1).AddDays(-1);

                    string sql = @"
                UPDATE ControleLogistico
                SET StatusLaudo = 'EMITIDO'
                WHERE ClienteId = @ClienteId
                  AND Data >= @InicioMes
                  AND Data <= @FimMes
                  AND (StatusLaudo = 'AGUARDANDO' OR StatusLaudo IS NULL OR StatusLaudo = '')";

                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ClienteId", _clienteId);
                    cmd.Parameters.AddWithValue("@InicioMes", inicioMes);
                    cmd.Parameters.AddWithValue("@FimMes", fimMes);

                    int registrosAtualizados = await cmd.ExecuteNonQueryAsync();

                    if (registrosAtualizados > 0)
                    {
                        MessageBox.Show($"{registrosAtualizados} ticket(s) atualizado(s) para status EMITIDO.",
                            "Atualização", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar status dos tickets: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnExportarLaudo_Click(object sender, EventArgs e)
        {
            try
            {
                string pastaDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string pastaLaudos = Path.Combine(pastaDocs, "DLD_Laudos");

                string nomeEmpresaSanitizado = SanitizarNomeArquivo(_razaoSocialGerador);

                int mesLaudo = _mesAno.Month;
                int anoLaudo = _mesAno.Year;

                string nomeArq = $"Laudo_{nomeEmpresaSanitizado}_{mesLaudo:00}_{anoLaudo}.pdf";

                string caminho = Path.Combine(pastaLaudos, nomeArq);

                string pastaDestino = Path.GetDirectoryName(caminho);
                if (!Directory.Exists(pastaDestino))
                    Directory.CreateDirectory(pastaDestino);

                await GerarPDFLaudoEm(caminho);

                await AtualizarStatusLaudoTicketsAsync();

                MostrarPreviewPDF(caminho);

                var resp = MessageBox.Show($"PDF gerado:\n\n{caminho}\n\nDeseja abrir?", "Sucesso",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (resp == DialogResult.Yes)
                    System.Diagnostics.Process.Start(caminho);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string SanitizarNomeArquivo(string nomeEmpresa)
        {
            if (string.IsNullOrWhiteSpace(nomeEmpresa))
                return "EMPRESA";

            string invalidos = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalidos)
            {
                nomeEmpresa = nomeEmpresa.Replace(c, '_');
            }

            nomeEmpresa = nomeEmpresa.Replace(' ', '_').ToUpper();

            if (nomeEmpresa.Length > 50)
            {
                nomeEmpresa = nomeEmpresa.Substring(0, 50);
            }

            return nomeEmpresa;
        }

        private async Task GerarPDFLaudoEm(string caminhoCompleto)
        {
            PdfSharp.Pdf.PdfDocument doc = new PdfSharp.Pdf.PdfDocument();
            {
                var page = doc.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                string pathPadrao = ExtrairRecursoParaTemp("Reverse.Resources.LaudoPadrao.pdf");
                using (var form = XPdfForm.FromFile(pathPadrao))
                {
                    form.PageNumber = 1;
                    gfx.DrawImage(form, 0, 0, page.Width.Point, page.Height.Point);
                }
            }

            {
                var page = doc.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                string pathBase = ExtrairRecursoParaTemp("Reverse.Resources.LaudoBaseEditable.pdf");
                using (var form = XPdfForm.FromFile(pathBase))
                {
                    form.PageNumber = 1;
                    gfx.DrawImage(form, 0, 0, page.Width.Point, page.Height.Point);
                }

                var fontTitulo = new XFont("Times New Roman", 18, XFontStyleEx.Bold);
                var fontLabel = new XFont("Times New Roman", 12, XFontStyleEx.Bold);
                var fontValor = new XFont("Times New Roman", 12, XFontStyleEx.Regular);

                double margemEsq = 60;
                double larguraUtil = page.Width.Point - 2 * margemEsq;
                double alturaLinha = 25;
                double y = 200;

                gfx.DrawString($"LAUDO: {_numeroLaudo}", fontTitulo, XBrushes.Black,
                    new XRect(215, 150, 340, 40), XStringFormats.TopRight);

                gfx.DrawString("1) Dados dos Resíduos", fontLabel, XBrushes.Black,
                    new XRect(margemEsq, y, larguraUtil, 20), XStringFormats.TopLeft);

                y += 25;

                double alturaSecao = alturaLinha * 4;
                gfx.DrawRectangle(XPens.Black, margemEsq, y, larguraUtil, alturaSecao);

                // Linha 1: Nome Comum
                double larguraLabel = 120;
                double larguraValor = Math.Max(100, larguraUtil - larguraLabel);
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Nome Comum", txtNomeComum.Text,
                    margemEsq, y, larguraLabel, larguraValor, alturaLinha, true);
                y += alturaLinha;

                // Linha 2: Conama 313
                larguraLabel = 120;
                larguraValor = Math.Max(100, larguraUtil - larguraLabel);
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Conama 313", txtConama.Text,
                    margemEsq, y, larguraLabel, larguraValor, alturaLinha, true);
                y += alturaLinha;

                // Linha 3: Acondicionamento | Classe | Estado Físico
                double xAtual = margemEsq;

                // Acondicionamento (correto)
                double larguraAcondLabel = 120, larguraAcondValor = 75;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Acondicionamento", txtAcondicionamento.Text,
                    xAtual, y, larguraAcondLabel, larguraAcondValor, alturaLinha, true);
                xAtual += larguraAcondLabel + larguraAcondValor;

                // Classe
                double larguraClasseLabel = 45, larguraClasseValor = 40;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Classe", txtClasse.Text,
                    xAtual, y, larguraClasseLabel, larguraClasseValor, alturaLinha, true);
                xAtual += larguraClasseLabel + larguraClasseValor;

                // Estado Físico
                double larguraEstadoLabel = 80;
                double larguraEstadoValor = Math.Max(100, larguraUtil - (xAtual - margemEsq + larguraEstadoLabel));
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Estado Físico", txtEstadoFisico.Text,
                    xAtual, y, larguraEstadoLabel, larguraEstadoValor, alturaLinha, true);

                y += alturaLinha;

                // Linha 4: Período Recebimento | Peso Total
                xAtual = margemEsq;

                double larguraPeriodoLabel = 120, larguraPeriodoValor = 160;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Período Recebimento",
                    _mesAno.ToString("MMMM/yyyy", new System.Globalization.CultureInfo("pt-BR")).ToUpper(),
                    xAtual, y, larguraPeriodoLabel, larguraPeriodoValor, alturaLinha, true);
                xAtual += larguraPeriodoLabel + larguraPeriodoValor;

                double larguraPesoLabel = 80;
                double larguraPesoValor = Math.Max(100, larguraUtil - (xAtual - margemEsq + larguraPesoLabel));
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Peso Total", $"{_pesoTotal:N3} kg",
                    xAtual, y, larguraPesoLabel, larguraPesoValor, alturaLinha, true);

                y += alturaLinha;

                y += 40;

                // --- Seção 2: Dados do Gerador ---
                gfx.DrawString("2) Dados do Gerador", fontLabel, XBrushes.Black,
                    new XRect(margemEsq, y, larguraUtil, 20), XStringFormats.TopLeft);

                y += 25;

                // Linha 1: Razão Social (com quebra de linha, padding e middle left)
                larguraLabel = 120;
                larguraValor = larguraUtil - larguraLabel;

                // --- Calcula altura necessária para Razão Social ---
                string razaoGerador = _razaoSocialGerador;
                double lineHeight = fontValor.GetHeight();
                double alturaNecessariaRazaoGerador;

                if (string.IsNullOrWhiteSpace(razaoGerador))
                {
                    alturaNecessariaRazaoGerador = alturaLinha;
                }
                else
                {
                    int linhasRazaoGerador = CalcularLinhas(gfx, razaoGerador, fontValor, larguraValor - 10);
                    alturaNecessariaRazaoGerador = Math.Max(alturaLinha, linhasRazaoGerador * lineHeight + 10);
                }

                // Caixa do rótulo com altura ajustada e padding
                var rectLabelRazaoGerador = new XRect(margemEsq, y, larguraLabel, alturaNecessariaRazaoGerador);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectLabelRazaoGerador);
                gfx.DrawRectangle(XPens.Black, rectLabelRazaoGerador);

                var rectLabelRazaoGeradorTexto = new XRect(rectLabelRazaoGerador.X + 5, rectLabelRazaoGerador.Y,
                    rectLabelRazaoGerador.Width - 5, rectLabelRazaoGerador.Height);
                gfx.DrawString("Razão Social", fontLabel, XBrushes.White, rectLabelRazaoGeradorTexto, XStringFormats.CenterLeft);

                // Caixa do valor com altura ajustada
                var rectValorRazaoGerador = new XRect(margemEsq + larguraLabel, y, larguraValor, alturaNecessariaRazaoGerador);
                gfx.DrawRectangle(XBrushes.White, rectValorRazaoGerador);
                gfx.DrawRectangle(XPens.Black, rectValorRazaoGerador);

                // Centralização vertical manual
                var tfRazaoGerador = new XTextFormatter(gfx);
                tfRazaoGerador.Alignment = XParagraphAlignment.Left;

                int linhasRazao = CalcularLinhas(gfx, razaoGerador, fontValor, larguraValor - 10);
                double alturaTextoRazao = linhasRazao * lineHeight;
                double offsetYRazao = (alturaNecessariaRazaoGerador - alturaTextoRazao) / 2;

                var rectValorRazaoGeradorTexto = new XRect(
                    rectValorRazaoGerador.X + 5,
                    rectValorRazaoGerador.Y + offsetYRazao,
                    rectValorRazaoGerador.Width - 10,
                    alturaTextoRazao
                );
                tfRazaoGerador.DrawString(razaoGerador, fontValor, XBrushes.Black, rectValorRazaoGeradorTexto, XStringFormats.TopLeft);

                y += alturaNecessariaRazaoGerador;

                // Linha 2: Endereço (com quebra de linha, padding e middle left)
                larguraLabel = 120;
                larguraValor = larguraUtil - larguraLabel;

                // --- Calcula altura necessária para Endereço ---
                string enderecoGerador = _enderecoGerador;
                double alturaNecessariaEndereco;

                if (string.IsNullOrWhiteSpace(enderecoGerador))
                {
                    alturaNecessariaEndereco = alturaLinha;
                }
                else
                {
                    int linhasEndereco = CalcularLinhas(gfx, enderecoGerador, fontValor, larguraValor - 10);
                    alturaNecessariaEndereco = Math.Max(alturaLinha, linhasEndereco * lineHeight + 10);
                }

                // Caixa do rótulo com altura ajustada e padding
                var rectLabelEndereco = new XRect(margemEsq, y, larguraLabel, alturaNecessariaEndereco);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectLabelEndereco);
                gfx.DrawRectangle(XPens.Black, rectLabelEndereco);

                var rectLabelEnderecoTexto = new XRect(rectLabelEndereco.X + 5, rectLabelEndereco.Y,
                    rectLabelEndereco.Width - 5, rectLabelEndereco.Height);
                gfx.DrawString("Endereço", fontLabel, XBrushes.White, rectLabelEnderecoTexto, XStringFormats.CenterLeft);

                // Caixa do valor com altura ajustada
                var rectValorEndereco = new XRect(margemEsq + larguraLabel, y, larguraValor, alturaNecessariaEndereco);
                gfx.DrawRectangle(XBrushes.White, rectValorEndereco);
                gfx.DrawRectangle(XPens.Black, rectValorEndereco);

                // Centralização vertical manual
                var tfEndereco = new XTextFormatter(gfx);
                tfEndereco.Alignment = XParagraphAlignment.Left;

                int linhasEnd = CalcularLinhas(gfx, enderecoGerador, fontValor, larguraValor - 10);
                double alturaTextoEnd = linhasEnd * lineHeight;
                double offsetYEnd = (alturaNecessariaEndereco - alturaTextoEnd) / 2;

                var rectValorEnderecoTexto = new XRect(
                    rectValorEndereco.X + 5,
                    rectValorEndereco.Y + offsetYEnd,
                    rectValorEndereco.Width - 10,
                    alturaTextoEnd
                );
                tfEndereco.DrawString(enderecoGerador, fontValor, XBrushes.Black, rectValorEnderecoTexto, XStringFormats.TopLeft);

                y += alturaNecessariaEndereco;

                // Linha 3: Município | UF | CNPJ
                xAtual = margemEsq;

                // --- MUNICÍPIO com quebra de linha ---
                double larguraMunicipioLabel = 120, larguraMunicipioValor = 76;
                string municipioGerador = _municipioGerador;
                double paddingInterno = 4;
                double alturaNecessariaMunicipio;

                XFont fonteMunicipio = fontValor;

                if (!string.IsNullOrWhiteSpace(municipioGerador))
                {
                    var tamanhoTexto = gfx.MeasureString(municipioGerador, fontValor);
                    if (tamanhoTexto.Width > larguraMunicipioValor - (paddingInterno * 2))
                    {
                        fonteMunicipio = new XFont("Times New Roman", 10, XFontStyleEx.Regular);
                    }
                }

                double lineHeightMunicipio = fonteMunicipio.GetHeight();

                if (string.IsNullOrWhiteSpace(municipioGerador))
                {
                    alturaNecessariaMunicipio = alturaLinha;
                }
                else
                {
                    int linhasMunicipio = CalcularLinhas(gfx, municipioGerador, fonteMunicipio, larguraMunicipioValor - (paddingInterno * 2));
                    alturaNecessariaMunicipio = Math.Max(alturaLinha, linhasMunicipio * lineHeightMunicipio + 10);
                }

                // Caixa do rótulo Município
                var rectLabelMunicipio = new XRect(xAtual, y, larguraMunicipioLabel, alturaNecessariaMunicipio);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectLabelMunicipio);
                gfx.DrawRectangle(XPens.Black, rectLabelMunicipio);

                var rectLabelMunicipioTexto = new XRect(rectLabelMunicipio.X + 5, rectLabelMunicipio.Y,
                    rectLabelMunicipio.Width - 5, rectLabelMunicipio.Height);
                gfx.DrawString("Município", fontLabel, XBrushes.White, rectLabelMunicipioTexto, XStringFormats.CenterLeft);

                // Caixa do valor Município
                var rectValorMunicipio = new XRect(xAtual + larguraMunicipioLabel, y, larguraMunicipioValor, alturaNecessariaMunicipio);
                gfx.DrawRectangle(XBrushes.White, rectValorMunicipio);
                gfx.DrawRectangle(XPens.Black, rectValorMunicipio);

                // Desenha o texto com quebra de linha e alinhamento left center
                if (!string.IsNullOrWhiteSpace(municipioGerador))
                {
                    int linhasMunicipio = CalcularLinhas(gfx, municipioGerador, fonteMunicipio, larguraMunicipioValor - (paddingInterno * 2));
                    List<string> linhasMunicipioList = new List<string>();

                    if (linhasMunicipio == 1)
                    {
                        linhasMunicipioList.Add(municipioGerador);
                    }
                    else
                    {
                        var palavrasMunicipio = municipioGerador.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string linhaAtual = "";

                        foreach (var palavra in palavrasMunicipio)
                        {
                            string tentativa = string.IsNullOrEmpty(linhaAtual) ? palavra : (linhaAtual + " " + palavra);
                            var tamanho = gfx.MeasureString(tentativa, fonteMunicipio);

                            if (tamanho.Width <= larguraMunicipioValor - (paddingInterno * 2))
                            {
                                linhaAtual = tentativa;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(linhaAtual))
                                    linhasMunicipioList.Add(linhaAtual);
                                linhaAtual = palavra;
                            }
                        }

                        if (!string.IsNullOrEmpty(linhaAtual))
                            linhasMunicipioList.Add(linhaAtual);
                    }

                    // Calcula a altura total do texto e o offset para centralização vertical
                    double alturaTextoMun = linhasMunicipioList.Count * lineHeightMunicipio;
                    double offsetYMun = (alturaNecessariaMunicipio - alturaTextoMun) / 2;
                    double yAtualMun = rectValorMunicipio.Y + offsetYMun;

                    // Desenha cada linha alinhada à esquerda com padding
                    foreach (var linhaMun in linhasMunicipioList)
                    {
                        gfx.DrawString(linhaMun, fonteMunicipio, XBrushes.Black,
                            new XRect(rectValorMunicipio.X + paddingInterno, yAtualMun,
                                     rectValorMunicipio.Width - (paddingInterno * 2), lineHeightMunicipio),
                            XStringFormats.TopLeft);

                        yAtualMun += lineHeightMunicipio;
                    }
                }

                xAtual += larguraMunicipioLabel + larguraMunicipioValor;

                // --- UF (mantém altura igual ao Município) ---
                double larguraUFLabel = 40, larguraUFValor = 50;

                var rectLabelUF = new XRect(xAtual, y, larguraUFLabel, alturaNecessariaMunicipio);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectLabelUF);
                gfx.DrawRectangle(XPens.Black, rectLabelUF);

                var rectLabelUFTexto = new XRect(rectLabelUF.X + 5, rectLabelUF.Y,
                    rectLabelUF.Width - 5, rectLabelUF.Height);
                gfx.DrawString("UF", fontLabel, XBrushes.White, rectLabelUFTexto, XStringFormats.CenterLeft);

                var rectValorUF = new XRect(xAtual + larguraUFLabel, y, larguraUFValor, alturaNecessariaMunicipio);
                gfx.DrawRectangle(XBrushes.White, rectValorUF);
                gfx.DrawRectangle(XPens.Black, rectValorUF);

                var rectValorUFTexto = new XRect(rectValorUF.X + 5, rectValorUF.Y,
                    rectValorUF.Width - 5, rectValorUF.Height);
                gfx.DrawString(_ufGerador ?? "", fontValor, XBrushes.Black, rectValorUFTexto, XStringFormats.CenterLeft);

                xAtual += larguraUFLabel + larguraUFValor;

                // --- CNPJ (mantém altura igual ao Município) ---
                double larguraCNPJLabel = 60;
                double larguraCNPJValor = Math.Max(120, larguraUtil - (xAtual - margemEsq + larguraCNPJLabel));

                var rectLabelCNPJ = new XRect(xAtual, y, larguraCNPJLabel, alturaNecessariaMunicipio);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectLabelCNPJ);
                gfx.DrawRectangle(XPens.Black, rectLabelCNPJ);

                var rectLabelCNPJTexto = new XRect(rectLabelCNPJ.X + 5, rectLabelCNPJ.Y,
                    rectLabelCNPJ.Width - 5, rectLabelCNPJ.Height);
                gfx.DrawString("CNPJ", fontLabel, XBrushes.White, rectLabelCNPJTexto, XStringFormats.CenterLeft);

                var rectValorCNPJ = new XRect(xAtual + larguraCNPJLabel, y, larguraCNPJValor, alturaNecessariaMunicipio);
                gfx.DrawRectangle(XBrushes.White, rectValorCNPJ);
                gfx.DrawRectangle(XPens.Black, rectValorCNPJ);

                var rectValorCNPJTexto = new XRect(rectValorCNPJ.X + 5, rectValorCNPJ.Y,
                    rectValorCNPJ.Width - 5, rectValorCNPJ.Height);
                gfx.DrawString(_cnpjGerador ?? "", fontValor, XBrushes.Black, rectValorCNPJTexto, XStringFormats.CenterLeft);

                y += alturaNecessariaMunicipio;

                y += alturaLinha;
                y += 40;

                // --- Seção 3: Dados do Receptor ---
                gfx.DrawString("3) Dados do Receptor", fontLabel, XBrushes.Black,
                    new XRect(margemEsq, y, larguraUtil, 20), XStringFormats.TopLeft);

                y += 25;

                // Linha 1: Razão Social (com quebra de linha e padding no label)
                larguraLabel = 120;
                larguraValor = larguraUtil - larguraLabel;

                string razao = txtRazaoDLD.Text;

                // Calcula altura necessária
                var tf = new XTextFormatter(gfx);
                tf.Alignment = XParagraphAlignment.Left;

                int linhas = CalcularLinhas(gfx, razao, fontValor, larguraValor - 10);
                double alturaNecessaria = Math.Max(alturaLinha, linhas * lineHeight + 10);

                // Caixa do rótulo com altura ajustada e padding
                var rectLabel = new XRect(margemEsq, y, larguraLabel, alturaNecessaria);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectLabel);
                gfx.DrawRectangle(XPens.Black, rectLabel);

                var rectLabelTexto = new XRect(rectLabel.X + 5, rectLabel.Y, rectLabel.Width - 5, rectLabel.Height);
                gfx.DrawString("Razão Social", fontLabel, XBrushes.White, rectLabelTexto, XStringFormats.CenterLeft);

                // Caixa do valor com altura ajustada
                var rectValor = new XRect(margemEsq + larguraLabel, y, larguraValor, alturaNecessaria);
                gfx.DrawRectangle(XBrushes.White, rectValor);
                gfx.DrawRectangle(XPens.Black, rectValor);

                // Centralização vertical manual
                double alturaTexto = linhas * lineHeight;
                double offsetY = (alturaNecessaria - alturaTexto) / 2;

                var rectValorTexto = new XRect(rectValor.X + 5, rectValor.Y + offsetY, rectValor.Width - 10, alturaTexto);
                tf.DrawString(razao, fontValor, XBrushes.Black, rectValorTexto, XStringFormats.TopLeft);

                y += alturaNecessaria;

                // Linha 2: Endereço
                larguraLabel = 120;
                larguraValor = larguraUtil - larguraLabel;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Endereço", txtEnderecoDLD.Text,
                    margemEsq, y, larguraLabel, larguraValor, alturaLinha, true);
                y += alturaLinha;

                // Linha 3: CNPJ | IE | LO
                xAtual = margemEsq;

                larguraCNPJLabel = 120;
                larguraCNPJValor = 110;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "CNPJ", txtCNPJDLD.Text,
                    xAtual, y, larguraCNPJLabel, larguraCNPJValor, alturaLinha, true);
                xAtual += larguraCNPJLabel + larguraCNPJValor;

                double larguraIELabel = 40, larguraIEValor = 93;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "IE", txtIEDLD.Text,
                    xAtual, y, larguraIELabel, larguraIEValor, alturaLinha, true);
                xAtual += larguraIELabel + larguraIEValor;

                double larguraLOLabel = 40;
                double larguraLOValor = Math.Max(70, larguraUtil - (xAtual - margemEsq + larguraLOLabel));
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "LO", txtLODLD.Text,
                    xAtual, y, larguraLOLabel, larguraLOValor, alturaLinha, true);

                y += alturaLinha;
                y += 40;

                // --- Seção 4: Descrição e Quantidades ---
                page = doc.AddPage();
                gfx = XGraphics.FromPdfPage(page);

                string pathBase2 = ExtrairRecursoParaTemp("Reverse.Resources.LaudoBaseEditable.pdf");
                using (var form = XPdfForm.FromFile(pathBase2))
                {
                    form.PageNumber = 1;
                    gfx.DrawImage(form, 0, 0, page.Width.Point, page.Height.Point);
                }

                double yTop = 180;
                gfx.DrawString("4) Descrição e Quantidades de Produtos Obsoletos Coletados",
                    fontLabel, XBrushes.Black,
                    new XRect(margemEsq, yTop, larguraUtil, 20), XStringFormats.TopLeft);

                y = yTop + 30;
                alturaLinha = 25;

                // Definição das larguras das colunas
                double[] larguras = { 65, 70, 80, 70, 155, 80 };
                string[] headers = { "Data", "Ticket", "MTR", "NFe", "Descrição", "Quantidades" };

                // Soma total da largura da tabela
                double larguraTabela = larguras.Sum();

                // Calcula posição inicial para centralizar
                double xInicial = (page.Width.Point - larguraTabela) / 2;

                // Cabeçalho
                double x = xInicial;
                for (int i = 0; i < headers.Length; i++)
                {
                    var rect = new XRect(x, y, larguras[i], alturaLinha);
                    gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rect);
                    gfx.DrawRectangle(XPens.Black, rect);
                    gfx.DrawString(headers[i], fontLabel, XBrushes.White,
                        new XRect(rect.X, rect.Y, rect.Width, rect.Height),
                        XStringFormats.Center);
                    x += larguras[i];
                }
                y += alturaLinha;

                // Carrega tickets reais
                var tickets = await CarregarTicketsAsync();
                decimal total = 0;

                // Limite de página
                double yLimite = page.Height.Point - 80;

                foreach (var t in tickets)
                {
                    // Quebra de página
                    if (y + alturaLinha > yLimite)
                    {
                        page = doc.AddPage();
                        gfx = XGraphics.FromPdfPage(page);

                        using (var form = XPdfForm.FromFile(pathBase2))
                        {
                            form.PageNumber = 1;
                            gfx.DrawImage(form, 0, 0, page.Width.Point, page.Height.Point);
                        }

                        // Repetir cabeçalho
                        y = 60;
                        x = xInicial;
                        for (int i = 0; i < headers.Length; i++)
                        {
                            var rect = new XRect(x, y, larguras[i], alturaLinha);
                            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rect);
                            gfx.DrawRectangle(XPens.Black, rect);
                            gfx.DrawString(headers[i], fontLabel, XBrushes.White,
                                new XRect(rect.X, rect.Y, rect.Width, rect.Height),
                                XStringFormats.Center);
                            x += larguras[i];
                        }
                        y += alturaLinha;
                    }

                    // Linhas de dados
                    string[] valoresTickets = {
                        t.Data.ToString("dd/MM/yyyy"),
                        t.Ticket,
                        string.IsNullOrWhiteSpace(t.MTR) ? "*" : t.MTR,
                        string.IsNullOrWhiteSpace(t.NF) ? "*" : t.NF,
                        "Descartes Resíduos Obsoletos",
                        $"{t.Peso:N3} kg"
                    };

                    x = xInicial;
                    double alturaLinhaAtual = alturaLinha;

                    // Ajusta altura se MTR ou NF forem grandes
                    if (!string.IsNullOrWhiteSpace(t.MTR))
                    {
                        int linhasMTR = CalcularLinhasComEspacos(t.MTR, fontValor, larguras[2]);
                        alturaLinhaAtual = Math.Max(alturaLinhaAtual, (linhasMTR * fontValor.GetHeight()) + 10);
                    }
                    if (!string.IsNullOrWhiteSpace(t.NF))
                    {
                        int linhasNF = CalcularLinhasComEspacos(t.NF, fontValor, larguras[3]);
                        alturaLinhaAtual = Math.Max(alturaLinhaAtual, (linhasNF * fontValor.GetHeight()) + 10);
                    }

                    for (int i = 0; i < valoresTickets.Length; i++)
                    {
                        var rect = new XRect(x, y, larguras[i], alturaLinhaAtual);
                        gfx.DrawRectangle(XBrushes.White, rect);
                        gfx.DrawRectangle(XPens.Black, rect);

                        if (i == 2 || i == 3) // MTR ou NFe
                        {
                            DesenharTextoQuebradoPorEspacos(gfx, valoresTickets[i], fontValor, XBrushes.Black, rect);
                        }
                        else
                        {
                            gfx.DrawString(valoresTickets[i], fontValor, XBrushes.Black,
                                new XRect(rect.X, rect.Y, rect.Width, rect.Height),
                                XStringFormats.Center);
                        }

                        x += larguras[i];
                    }

                    total += t.Peso;
                    y += alturaLinhaAtual;
                }

                // --- Linha TOTAL ---
                x = xInicial;
                for (int i = 0; i < 4; i++)
                    x += larguras[i];

                // Coluna "TOTAL"
                var rectTotal = new XRect(x, y, larguras[4], alturaLinha);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectTotal);
                gfx.DrawRectangle(XPens.Black, rectTotal);
                gfx.DrawString("TOTAL", fontLabel, XBrushes.White,
                    new XRect(rectTotal.X, rectTotal.Y, rectTotal.Width, rectTotal.Height),
                    XStringFormats.Center);
                x += larguras[4];

                // Coluna com o valor total
                var rectValorTotal = new XRect(x, y, larguras[5], alturaLinha);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectValorTotal);
                gfx.DrawRectangle(XPens.Black, rectValorTotal);
                gfx.DrawString($"{total:N3} kg", fontLabel, XBrushes.White,
                    new XRect(rectValorTotal.X, rectValorTotal.Y, rectValorTotal.Width, rectValorTotal.Height),
                    XStringFormats.Center);

                y += 50;


                // --- Seção 5: Segregação Final ---
                // Definição das larguras das colunas (mantendo seu layout)
                var segregacoes = await CarregarSegregacaoAsync();

                var segregacoesAgrupadas = segregacoes
                    .GroupBy(s => new { s.Tipo, s.Tratamento })
                    .Select(g => new
                    {
                        Tipo = g.Key.Tipo,
                        Peso = g.Sum(s => s.Peso),
                        Tratamento = g.Key.Tratamento
                    })
                    .OrderBy(s => s.Tipo)
                    .ToList();

                // Definições de layout
                double[] larguras5 = { 50, 190, 100, 180 };
                string[] headers5 = { "Item", "Material", "Quantidades", "Tipo de Tratamento" };
                double larguraTabela5 = larguras5.Sum();
                double xInicial5 = (page.Width.Point - larguraTabela5) / 2;

                // Limite seguro
                double yLimitePagina = page.Height.Point - 120;

                // 2. PRÉ-CÁLCULO: Calcular a altura TOTAL que essa seção vai gastar
                double alturaTitulo = 50;
                double alturaCabecalho = 25;
                double alturaLinhaTotal = 25;

                double alturaTotalNecessaria = alturaTitulo + alturaCabecalho + alturaLinhaTotal;

                foreach (var s in segregacoesAgrupadas)
                {
                    double hLinha = 25; // Altura mínima

                    if (!string.IsNullOrWhiteSpace(s.Tipo))
                    {
                        int linhasMat = CalcularLinhas(gfx, s.Tipo, fontValor, larguras5[1] - 10);
                        hLinha = Math.Max(hLinha, linhasMat * fontValor.GetHeight() + 10);
                    }

                    if (!string.IsNullOrWhiteSpace(s.Tratamento))
                    {
                        int linhasTrat = CalcularLinhas(gfx, s.Tratamento, fontValor, larguras5[3] - 10);
                        hLinha = Math.Max(hLinha, linhasTrat * fontValor.GetHeight() + 10);
                    }
                    alturaTotalNecessaria += hLinha;
                }

                if (y + alturaTotalNecessaria > yLimitePagina)
                {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);

                    using (var form = XPdfForm.FromFile(pathBase2))
                    {
                        form.PageNumber = 1;
                        gfx.DrawImage(form, 0, 0, page.Width.Point, page.Height.Point);
                    }
                    y = 180;
                }
                else
                {
                    y += 40;
                }

                // 4. DESENHO: Título e Tabela
                gfx.DrawString("5) Segregação Final dos Materiais e/ou Resíduos Após a Manufatura Reversa",
                        fontLabel, XBrushes.Black,
                        new XRect(margemEsq, y, larguraUtil, 20), XStringFormats.TopLeft);

                y += 30;
                alturaLinha = 25;

                // Cabeçalho da Tabela
                double x5 = xInicial5;
                for (int i = 0; i < headers5.Length; i++)
                {
                    var rect = new XRect(x5, y, larguras5[i], alturaLinha);
                    gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rect);
                    gfx.DrawRectangle(XPens.Black, rect);
                    gfx.DrawString(headers5[i], fontLabel, XBrushes.White,
                        new XRect(rect.X, rect.Y, rect.Width, rect.Height),
                        XStringFormats.Center);
                    x5 += larguras5[i];
                }
                y += alturaLinha;

                // Dados da Tabela
                decimal totalPeso = 0;
                int item = 1;

                foreach (var s in segregacoesAgrupadas)
                {
                    double alturaLinhaAtual = 25;

                    if (!string.IsNullOrWhiteSpace(s.Tipo))
                    {
                        int linhasMat = CalcularLinhas(gfx, s.Tipo, fontValor, larguras5[1] - 10);
                        alturaLinhaAtual = Math.Max(alturaLinhaAtual, linhasMat * fontValor.GetHeight() + 10);
                    }
                    if (!string.IsNullOrWhiteSpace(s.Tratamento))
                    {
                        int linhasTrat = CalcularLinhas(gfx, s.Tratamento, fontValor, larguras5[3] - 10);
                        alturaLinhaAtual = Math.Max(alturaLinhaAtual, linhasTrat * fontValor.GetHeight() + 10);
                    }

                    if (y + alturaLinhaAtual > yLimitePagina)
                    {
                        page = doc.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        using (var form = XPdfForm.FromFile(pathBase2))
                        {
                            form.PageNumber = 1;
                            gfx.DrawImage(form, 0, 0, page.Width.Point, page.Height.Point);
                        }
                        y = 180;

                        x5 = xInicial5;
                        for (int i = 0; i < headers5.Length; i++)
                        {
                            var rect = new XRect(x5, y, larguras5[i], alturaLinha);
                            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rect);
                            gfx.DrawRectangle(XPens.Black, rect);
                            gfx.DrawString(headers5[i], fontLabel, XBrushes.White,
                                new XRect(rect.X, rect.Y, rect.Width, rect.Height),
                                XStringFormats.Center);
                            x5 += larguras5[i];
                        }
                        y += alturaLinha;
                    }

                    string[] valores = {
                        item.ToString(),
                        s.Tipo,
                        $"{s.Peso:N3} kg",
                        s.Tratamento
                    };

                    x5 = xInicial5;
                    for (int i = 0; i < valores.Length; i++)
                    {
                        var rect = new XRect(x5, y, larguras5[i], alturaLinhaAtual);
                        gfx.DrawRectangle(XBrushes.White, rect);
                        gfx.DrawRectangle(XPens.Black, rect);

                        if (i == 0 || i == 2)
                            gfx.DrawString(valores[i], fontValor, XBrushes.Black, rect, XStringFormats.Center);
                        else
                            DesenharTextoQuebradoCentralizado(gfx, valores[i], fontValor, XBrushes.Black, rect);

                        x5 += larguras5[i];
                    }

                    totalPeso += s.Peso;
                    item++;
                    y += alturaLinhaAtual;
                }

                // Linha TOTAL
                x5 = xInicial5 + larguras5[0];

                if (y + alturaLinha > yLimitePagina)
                {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    using (var form = XPdfForm.FromFile(pathBase2))
                    {
                        form.PageNumber = 1;
                        gfx.DrawImage(form, 0, 0, page.Width.Point, page.Height.Point);
                    }
                    y = 180;
                }

                var rectTotalLabel = new XRect(x5, y, larguras5[1], alturaLinha);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectTotalLabel);
                gfx.DrawRectangle(XPens.Black, rectTotalLabel);
                gfx.DrawString("TOTAL", fontLabel, XBrushes.White, rectTotalLabel, XStringFormats.Center);

                x5 += larguras5[1];
                var rectTotalValor = new XRect(x5, y, larguras5[2], alturaLinha);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectTotalValor);
                gfx.DrawRectangle(XPens.Black, rectTotalValor);
                gfx.DrawString($"{totalPeso:N3} kg", fontLabel, XBrushes.White, rectTotalValor, XStringFormats.Center);

                y += alturaLinha;

                // --- Seção 6: Declaração Final + Assinatura ---
                double margemRodape = 100; // Espaço reservado do rodapé
                double alturaAssinatura = 150; // Altura total da assinatura (img + nome + margem)
                double alturaTextos = 130; // Altura aproximada dos 2 parágrafos
                double espacoNecessarioSecao6 = 50 + alturaTextos; // Título + textos (SEM assinatura)

                double yLimiteTexto = page.Height.Point - margemRodape - alturaAssinatura;

                // Se não couber os textos, cria nova página
                if (y + espacoNecessarioSecao6 > yLimiteTexto)
                {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);

                    string pathBase3 = ExtrairRecursoParaTemp("Reverse.Resources.LaudoBaseEditable.pdf");
                    using (var form = XPdfForm.FromFile(pathBase3))
                    {
                        form.PageNumber = 1;
                        gfx.DrawImage(form, 0, 0, page.Width.Point, page.Height.Point);
                    }
                    y = 180;
                    yLimiteTexto = page.Height.Point - margemRodape - alturaAssinatura;
                }
                else
                {
                    y += 40;
                }

                // Título
                gfx.DrawString("6) Declaração Final", fontLabel, XBrushes.Black,
                    new XRect(margemEsq, y, larguraUtil, 20), XStringFormats.TopLeft);
                y += 30;

                // PRIMEIRO PARÁGRAFO
                string textoDeclaracao1 = "A DLD SOLUÇÕES EM LOGISTICA REVERSA, GESTÃO E RECICLAGEM LTDA, inscrita no CNPJ nº 37.540.504/0001-04 declara que gerenciou de forma ambientalmente correta e sustentável, através dos processos de logística/manufatura reversa e destinação final, seguindo a legislação vigente neste país, os resíduos eletroeletrônicos obsoletos e descartes em geral descritos neste relatório.";

                var tf1 = new XTextFormatter(gfx);
                tf1.Alignment = XParagraphAlignment.Justify;
                var rectTexto1 = new XRect(margemEsq, y, larguraUtil, 65);
                tf1.DrawString(textoDeclaracao1, fontValor, XBrushes.Black, rectTexto1, XStringFormats.TopLeft);
                y += 70;

                // SEGUNDO PARÁGRAFO
                string textoDeclaracao2 = "A DLD SOLUÇÕES EM LOGISTICA REVERSA, GESTÃO E RECICLAGEM LTDA atesta a veracidade de todas as informações contidas neste relatório de rastreamento e destinação final de resíduos eletroeletrônicos obsoletos e descartes em geral.";

                var tf2 = new XTextFormatter(gfx);
                tf2.Alignment = XParagraphAlignment.Justify;
                var rectTexto2 = new XRect(margemEsq, y, larguraUtil, 55);
                tf2.DrawString(textoDeclaracao2, fontValor, XBrushes.Black, rectTexto2, XStringFormats.TopLeft);

                // ASSINATURA: Sempre fixada acima do rodapé
                double yAssinatura = page.Height.Point - margemRodape - alturaAssinatura + 20;

                // Data e local
                var mesAtual = DateTime.Now.ToString("MMMM", new System.Globalization.CultureInfo("pt-BR"));
                mesAtual = char.ToUpper(mesAtual[0]) + mesAtual.Substring(1);
                string dataTexto = $"Araras/SP, {mesAtual} de {DateTime.Now:yyyy}";
                var fontDataLocal = new XFont("Times New Roman", 16, XFontStyleEx.Bold);

                gfx.DrawString(dataTexto, fontDataLocal, XBrushes.Black,
                    new XRect(0, yAssinatura, page.Width.Point, 20), XStringFormats.Center);
                yAssinatura += 25;

                // Imagem da assinatura
                try
                {
                    string pathAssinatura = ExtrairRecursoParaTemp("Reverse.Resources.AssinaturaLaudo.jpg");
                    using (XImage imgAssinatura = XImage.FromFile(pathAssinatura))
                    {
                        double larguraImg = 180;
                        double alturaImg = (imgAssinatura.PointHeight / imgAssinatura.PointWidth) * larguraImg;

                        if (alturaImg > 70)
                        {
                            alturaImg = 70;
                            larguraImg = (imgAssinatura.PointWidth / imgAssinatura.PointHeight) * alturaImg;
                        }

                        double xImg = (page.Width.Point - larguraImg) / 2;
                        gfx.DrawImage(imgAssinatura, xImg, yAssinatura, larguraImg, alturaImg);
                        yAssinatura += alturaImg + 3;
                    }
                }
                catch
                {
                    yAssinatura += 15;
                }

                // Linha e nome
                gfx.DrawString("________________________________", fontValor, XBrushes.Black,
                    new XRect(0, yAssinatura, page.Width.Point, 12), XStringFormats.Center);
                yAssinatura += 12;

                gfx.DrawString("DLD Soluções em Logística Reversa", fontLabel, XBrushes.Black,
                    new XRect(0, yAssinatura, page.Width.Point, 12), XStringFormats.Center);
            }
            doc.Save(caminhoCompleto);
        }

        private async Task<List<(string Tipo, decimal Peso, string Tratamento)>> CarregarSegregacaoAsync()
        {
            var lista = new List<(string, decimal, string)>();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string sql = @"
                    SELECT Tipo, Peso, Tratamento
                    FROM BalancoMassa
                    WHERE ClienteId = @ClienteId
                      AND MesAno = @MesAno
                    ORDER BY Tipo";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClienteId", _clienteId);
                cmd.Parameters.AddWithValue("@MesAno", _mesAno);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string tipo = reader["Tipo"]?.ToString();
                        decimal peso = reader["Peso"] != DBNull.Value ? Convert.ToDecimal(reader["Peso"]) : 0;
                        string tratamento = reader["Tratamento"]?.ToString();

                        lista.Add((tipo, peso, tratamento));
                    }
                }
            }

            return lista;
        }

        private void DesenharTextoQuebradoPorEspacos(XGraphics gfx, string texto, XFont fonte, XBrush brush, XRect rect)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return;

            // Quebra por espaços
            var linhas = texto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            double alturaLinha = fonte.GetHeight();
            double alturaTotal = linhas.Length * alturaLinha;
            double offsetY = rect.Y + Math.Max(0, (rect.Height - alturaTotal) / 2);

            double yAtual = offsetY;

            foreach (var linha in linhas)
            {
                var tamanho = gfx.MeasureString(linha, fonte);
                double xCentralizado = rect.X + (rect.Width - tamanho.Width) / 2;

                gfx.DrawString(linha, fonte, brush,
                    new XRect(xCentralizado, yAtual, rect.Width, alturaLinha),
                    XStringFormats.TopLeft);

                yAtual += alturaLinha;

                if (yAtual > rect.Y + rect.Height)
                    break;
            }
        }

        private void DesenharTextoQuebradoCentralizado(XGraphics gfx, string texto, XFont fonte, XBrush brush, XRect rect)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return;

            var palavras = texto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> linhas = new List<string>();
            string linhaAtual = "";

            foreach (var palavra in palavras)
            {
                string tentativa = string.IsNullOrEmpty(linhaAtual) ? palavra : (linhaAtual + " " + palavra);
                var tamanho = gfx.MeasureString(tentativa, fonte);

                if (tamanho.Width <= rect.Width - 4)
                {
                    linhaAtual = tentativa;
                }
                else
                {
                    if (!string.IsNullOrEmpty(linhaAtual))
                        linhas.Add(linhaAtual);
                    linhaAtual = palavra;
                }
            }

            if (!string.IsNullOrEmpty(linhaAtual))
                linhas.Add(linhaAtual);

            double alturaLinha = fonte.GetHeight();
            double alturaTotal = linhas.Count * alturaLinha;
            double offsetY = rect.Y + Math.Max(0, (rect.Height - alturaTotal) / 2);

            double yAtual = offsetY;

            foreach (var linha in linhas)
            {
                var tamanho = gfx.MeasureString(linha, fonte);
                double xCentralizado = rect.X + (rect.Width - tamanho.Width) / 2;

                gfx.DrawString(linha, fonte, brush,
                    new XRect(xCentralizado, yAtual, rect.Width, alturaLinha),
                    XStringFormats.TopLeft);

                yAtual += alturaLinha;

                if (yAtual > rect.Y + rect.Height)
                    break;
            }
        }

        private int CalcularLinhasComEspacos(string texto, XFont fonte, double largura)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return 1;

            var partes = texto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return Math.Max(1, partes.Length);
        }

        private int CalcularLinhas(XGraphics gfx, string texto, XFont fonte, double largura)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return 1;

            var palavras = texto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string linhaAtual = "";
            int linhas = 1;

            foreach (var p in palavras)
            {
                string tentativa = string.IsNullOrEmpty(linhaAtual) ? p : (linhaAtual + " " + p);
                var tam = gfx.MeasureString(tentativa, fonte);
                if (tam.Width <= largura)
                {
                    linhaAtual = tentativa;
                }
                else
                {
                    linhas++;
                    linhaAtual = p;
                }
            }

            return linhas;
        }

        private async Task<List<(DateTime Data, string Ticket, string MTR, string NF, decimal Peso)>>
            CarregarTicketsAsync()
        {
            var lista = new List<(DateTime, string, string, string, decimal)>();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string sql = @"
                    SELECT Data, Ticket, MTR, NF, Peso
                    FROM ControleLogistico
                    WHERE ClienteId = @ClienteId
                      AND MONTH(Data) = MONTH(@MesAno)
                      AND YEAR(Data) = YEAR(@MesAno)
                    ORDER BY Data";
                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClienteId", _clienteId);
                cmd.Parameters.AddWithValue("@MesAno", _mesAno);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        DateTime data = reader.GetDateTime(0);
                        string ticket = reader["Ticket"]?.ToString();
                        string mtr = reader["MTR"]?.ToString();
                        string nf = reader["NF"]?.ToString();
                        decimal peso = reader["Peso"] != DBNull.Value ? Convert.ToDecimal(reader["Peso"]) : 0;

                        lista.Add((data, ticket, mtr, nf, peso));
                    }
                }
            }

            return lista;
        }
        private void DesenharCaixaComLabel(
            XGraphics gfx, XFont labelFont, XFont valueFont,
            string label, string value,
            double x, double y, double larguraLabel, double larguraValor, double alturaLinha,
            bool aplicarPadding = false)
        {
            var rectLabel = new XRect(x, y, larguraLabel, alturaLinha);
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectLabel);
            gfx.DrawRectangle(XPens.Black, rectLabel);

            var rectLabelTexto = aplicarPadding
                ? new XRect(rectLabel.X + 5, rectLabel.Y, rectLabel.Width - 5, rectLabel.Height)
                : rectLabel;

            gfx.DrawString(label, labelFont, XBrushes.White, rectLabelTexto, XStringFormats.CenterLeft);

            var rectValor = new XRect(x + larguraLabel, y, larguraValor, alturaLinha);
            gfx.DrawRectangle(XBrushes.White, rectValor);
            gfx.DrawRectangle(XPens.Black, rectValor);

            var rectValorTexto = aplicarPadding
                ? new XRect(rectValor.X + 5, rectValor.Y, rectValor.Width - 5, rectValor.Height)
                : rectValor;

            gfx.DrawString(value ?? "", valueFont, XBrushes.Black, rectValorTexto, XStringFormats.CenterLeft);
        }

        private string ExtrairRecursoParaTemp(string resourceName)
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException("Recurso não encontrado: " + resourceName);

                using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                    stream.CopyTo(fs);
            }
            return tempFile;
        }
        private void btnSair_Click(object sender, EventArgs e) => Close();
    }
}