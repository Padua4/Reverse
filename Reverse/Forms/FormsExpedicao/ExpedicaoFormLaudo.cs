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

            // Defaults (mantém como está)
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

        private async void btnExportarLaudo_Click(object sender, EventArgs e)
        {
            try
            {
                string pastaDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string pastaLaudos = Path.Combine(pastaDocs, "DLD_Laudos");

                string nomeArq = $"Laudo_{_numeroLaudo}_{_mesAno:yyyy_MM}.pdf";
                string caminho = Path.Combine(pastaLaudos, nomeArq);

                string pastaDestino = Path.GetDirectoryName(caminho);
                if (!Directory.Exists(pastaDestino))
                    Directory.CreateDirectory(pastaDestino);

                await GerarPDFLaudoEm(caminho);

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

                // Linha 1: Razão Social
                larguraLabel = 120;
                larguraValor = larguraUtil - larguraLabel;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Razão Social", _razaoSocialGerador,
                    margemEsq, y, larguraLabel, larguraValor, alturaLinha, true);
                y += alturaLinha;

                // Linha 2: Endereço
                larguraLabel = 120;
                larguraValor = larguraUtil - larguraLabel;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Endereço", _enderecoGerador,
                    margemEsq, y, larguraLabel, larguraValor, alturaLinha, true);
                y += alturaLinha;

                // Linha 3: Município | UF | CNPJ
                xAtual = margemEsq;

                // Município
                double larguraMunicipioLabel = 120, larguraMunicipioValor = 70;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Município", _municipioGerador,
                    xAtual, y, larguraMunicipioLabel, larguraMunicipioValor, alturaLinha, true);
                xAtual += larguraMunicipioLabel + larguraMunicipioValor;

                // UF
                double larguraUFLabel = 40, larguraUFValor = 50;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "UF", _ufGerador,
                    xAtual, y, larguraUFLabel, larguraUFValor, alturaLinha, true);
                xAtual += larguraUFLabel + larguraUFValor;

                // CNPJ (usa o restante da linha)
                double larguraCNPJLabel = 60;
                double larguraCNPJValor = Math.Max(120, larguraUtil - (xAtual - margemEsq + larguraCNPJLabel));
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "CNPJ", _cnpjGerador,
                    xAtual, y, larguraCNPJLabel, larguraCNPJValor, alturaLinha, true);

                y += alturaLinha;
                y += 40;

                // --- Seção 3: Dados do Receptor ---
                gfx.DrawString("3) Dados do Receptor", fontLabel, XBrushes.Black,
                    new XRect(margemEsq, y, larguraUtil, 20), XStringFormats.TopLeft);

                y += 25;

                // Linha 1: Razão Social (com quebra de linha, padding e centralizado)
                larguraLabel = 120;
                larguraValor = larguraUtil - larguraLabel;

                // Caixa do rótulo
                var rectLabel = new XRect(margemEsq, y, larguraLabel, alturaLinha);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectLabel);
                gfx.DrawRectangle(XPens.Black, rectLabel);
                gfx.DrawString("Razão Social", fontLabel, XBrushes.White,
                    new XRect(rectLabel.X + 5, rectLabel.Y, rectLabel.Width - 5, rectLabel.Height),
                    XStringFormats.CenterLeft);

                // --- Agora o valor com quebra dinâmica ---
                string razao = txtRazaoDLD.Text;

                // Usa XTextFormatter para quebrar
                var tf = new XTextFormatter(gfx);
                tf.Alignment = XParagraphAlignment.Left;

                // Mede altura necessária (quantas linhas cabem na largura)
                double lineHeight = fontValor.GetHeight();
                int linhas = (int)Math.Ceiling(gfx.MeasureString(razao, fontValor).Width / (larguraValor - 5));
                double alturaNecessaria = Math.Max(alturaLinha, linhas * lineHeight);

                // Caixa do valor com altura ajustada
                var rectValor = new XRect(margemEsq + larguraLabel, y, larguraValor, alturaNecessaria);
                gfx.DrawRectangle(XBrushes.White, rectValor);
                gfx.DrawRectangle(XPens.Black, rectValor);

                // Padding lateral de 5px
                var rectValorTexto = new XRect(rectValor.X + 5, rectValor.Y, rectValor.Width - 5, rectValor.Height);

                // Desenha o texto quebrado
                tf.DrawString(razao, fontValor, XBrushes.Black, rectValorTexto, XStringFormats.TopLeft);

                // Avança Y pela altura usada
                y += alturaNecessaria;

                // Linha 2: Endereço
                larguraLabel = 120;
                larguraValor = larguraUtil - larguraLabel;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "Endereço", txtEnderecoDLD.Text,
                    margemEsq, y, larguraLabel, larguraValor, alturaLinha, true);
                y += alturaLinha;

                // Linha 3: CNPJ | IE | LO
                xAtual = margemEsq;

                // CNPJ
                larguraCNPJLabel = 120;
                larguraCNPJValor = 110;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "CNPJ", txtCNPJDLD.Text,
                    xAtual, y, larguraCNPJLabel, larguraCNPJValor, alturaLinha, true);
                xAtual += larguraCNPJLabel + larguraCNPJValor;

                // IE
                double larguraIELabel = 40, larguraIEValor = 93;
                DesenharCaixaComLabel(gfx, fontLabel, fontValor, "IE", txtIEDLD.Text,
                    xAtual, y, larguraIELabel, larguraIEValor, alturaLinha, true);
                xAtual += larguraIELabel + larguraIEValor;

                // LO (usa o restante da linha)
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
                        int linhasMTR = CalcularLinhas(gfx, t.MTR, fontValor, larguras[2]);
                        alturaLinhaAtual = Math.Max(alturaLinhaAtual, linhasMTR * fontValor.GetHeight());
                    }
                    if (!string.IsNullOrWhiteSpace(t.NF))
                    {
                        int linhasNF = CalcularLinhas(gfx, t.NF, fontValor, larguras[3]);
                        alturaLinhaAtual = Math.Max(alturaLinhaAtual, linhasNF * fontValor.GetHeight());
                    }

                    for (int i = 0; i < valoresTickets.Length; i++)
                    {
                        var rect = new XRect(x, y, larguras[i], alturaLinhaAtual);
                        gfx.DrawRectangle(XBrushes.White, rect);
                        gfx.DrawRectangle(XPens.Black, rect);

                        if (i == 2 || i == 3) // MTR ou NFe
                        {
                            DesenharTextoQuebradoCentralizado(gfx, valoresTickets[i], fontValor, XBrushes.Black, rect);
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

                // --- Linha TOTAL (apenas uma vez, fora do loop) ---
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
                // Verifica se precisa de nova página ANTES de começar a seção 5
                double espacoNecessarioSecao5 = 100; // Espaço mínimo para título + cabeçalho da tabela
                if (y + espacoNecessarioSecao5 > yLimite)
                {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);

                    using (var form = XPdfForm.FromFile(pathBase2))
                    {
                        form.PageNumber = 1;
                        gfx.DrawImage(form, 0, 0, page.Width.Point, page.Height.Point);
                    }

                    y = 180; // Começa em uma posição mais baixa, igual às outras seções
                }
                else
                {
                    y += 50; // Se estiver na mesma página, dá um espaçamento maior
                }

                gfx.DrawString("5) Segregação Final dos Materiais e/ou Resíduos Após a Manufatura Reversa",
                        fontLabel, XBrushes.Black,
                        new XRect(margemEsq, y, larguraUtil, 20), XStringFormats.TopLeft);

                y += 30;
                alturaLinha = 25;

                // Definição das larguras das colunas
                double[] larguras5 = { 50, 190, 100, 180 };
                string[] headers5 = { "Item", "Material", "Quantidades", "Tipo de Tratamento" };

                // Soma total da largura da tabela
                double larguraTabela5 = larguras5.Sum();

                // Calcula posição inicial para centralizar
                double xInicial5 = (page.Width.Point - larguraTabela5) / 2;

                // Controle de limite da página
                double yLimite5 = page.Height.Point - 80;

                // Cabeçalho
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

                var segregacoes = await CarregarSegregacaoAsync();
                decimal totalPeso = 0;
                int item = 1;

                foreach (var s in segregacoes)
                {
                    // Calcular altura necessária baseado nos textos longos
                    double alturaLinhaAtual = alturaLinha;

                    // Verifica quantas linhas o Material precisa
                    if (!string.IsNullOrWhiteSpace(s.Tipo))
                    {
                        int linhasMaterial = CalcularLinhas(gfx, s.Tipo, fontValor, larguras5[1] - 10); // -10 para padding
                        alturaLinhaAtual = Math.Max(alturaLinhaAtual, linhasMaterial * fontValor.GetHeight() + 10);
                    }

                    // Verifica quantas linhas o Tratamento precisa
                    if (!string.IsNullOrWhiteSpace(s.Tratamento))
                    {
                        int linhasTratamento = CalcularLinhas(gfx, s.Tratamento, fontValor, larguras5[3] - 10); // -10 para padding
                        alturaLinhaAtual = Math.Max(alturaLinhaAtual, linhasTratamento * fontValor.GetHeight() + 10);
                    }

                    // Verifica se a linha cabe na página com a altura calculada
                    if (y + alturaLinhaAtual > yLimite5)
                    {
                        page = doc.AddPage();
                        gfx = XGraphics.FromPdfPage(page);

                        using (var form = XPdfForm.FromFile(pathBase2))
                        {
                            form.PageNumber = 1;
                            gfx.DrawImage(form, 0, 0, page.Width.Point, page.Height.Point);
                        }

                        y = 180; // Começa mais abaixo
                        x5 = xInicial5;

                        // Repetir cabeçalho da tabela
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

                        // Para Item (coluna 0) e Quantidades (coluna 2): texto simples centralizado
                        if (i == 0 || i == 2)
                        {
                            gfx.DrawString(valores[i], fontValor, XBrushes.Black,
                                new XRect(rect.X, rect.Y, rect.Width, rect.Height),
                                XStringFormats.Center);
                        }
                        // Para Material (coluna 1) e Tipo de Tratamento (coluna 3): texto com quebra
                        else if (i == 1 || i == 3)
                        {
                            DesenharTextoQuebradoCentralizado(gfx, valores[i], fontValor, XBrushes.Black, rect);
                        }

                        x5 += larguras5[i];
                    }

                    totalPeso += s.Peso;
                    item++;
                    y += alturaLinhaAtual; // Usa a altura calculada dinamicamente
                }

                // --- Linha TOTAL ---
                // Verifica se a linha TOTAL cabe na página atual
                if (y + alturaLinha > yLimite5)
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

                // TOTAL embaixo da coluna "Material"
                x5 = xInicial5 + larguras5[0]; // pula só a coluna "Item"
                var rectTotalLabel = new XRect(x5, y, larguras5[1], alturaLinha);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectTotalLabel);
                gfx.DrawRectangle(XPens.Black, rectTotalLabel);
                gfx.DrawString("TOTAL", fontLabel, XBrushes.White,
                    new XRect(rectTotalLabel.X, rectTotalLabel.Y, rectTotalLabel.Width, rectTotalLabel.Height),
                    XStringFormats.Center);

                // Valor embaixo da coluna "Quantidades"
                x5 += larguras5[1];
                var rectTotalValor = new XRect(x5, y, larguras5[2], alturaLinha);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(0x54, 0x8D, 0xD4)), rectTotalValor);
                gfx.DrawRectangle(XPens.Black, rectTotalValor);
                gfx.DrawString($"{totalPeso:N3} kg", fontLabel, XBrushes.White,
                    new XRect(rectTotalValor.X, rectTotalValor.Y, rectTotalValor.Width, rectTotalValor.Height),
                    XStringFormats.Center);

                y += alturaLinha;

                // --- Seção 6: Declaração Final + Assinatura ---
                // Calcular espaço necessário para seção 6 + assinatura
                double espacoNecessarioSecao6 = 280; // AUMENTADO: texto com quebra + assinatura + margens
                double espacoDisponivel = page.Height.Point - y - 80; // 80 = margem inferior

                // Se não couber na página atual, cria nova
                if (espacoDisponivel < espacoNecessarioSecao6)
                {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);

                    string pathBase3 = ExtrairRecursoParaTemp("Reverse.Resources.LaudoBaseEditable.pdf");
                    using (var form = XPdfForm.FromFile(pathBase3))
                    {
                        form.PageNumber = 1;
                        gfx.DrawImage(form, 0, 0, page.Width.Point, page.Height.Point);
                    }

                    y = 180; // Começa do topo da nova página
                }
                else
                {
                    y += 40; // Espaçamento na mesma página
                }

                // Título da seção
                gfx.DrawString("6) Declaração Final", fontLabel, XBrushes.Black,
                    new XRect(margemEsq, y, larguraUtil, 20), XStringFormats.TopLeft);

                y += 30;

                // PRIMEIRO PARÁGRAFO
                string textoDeclaracao1 = "A DLD SOLUÇÕES EM LOGISTICA REVERSA, GESTÃO E RECICLAGEM LTDA, inscrita no CNPJ nº 37.540.504/0001-04 declara que gerenciou de forma ambientalmente correta e sustentável, através dos processos de logística/manufatura reversa e destinação final, seguindo a legislação vigente neste país, os resíduos eletroeletrônicos obsoletos e descartes em geral descritos neste relatório.";

                var tf1 = new XTextFormatter(gfx);
                tf1.Alignment = XParagraphAlignment.Justify;

                var rectTexto1 = new XRect(margemEsq, y, larguraUtil, 60); // Altura ajustada para 1 parágrafo
                tf1.DrawString(textoDeclaracao1, fontValor, XBrushes.Black, rectTexto1, XStringFormats.TopLeft);

                y += 70; // Espaço entre parágrafos (inclui a quebra de linha)

                // SEGUNDO PARÁGRAFO
                string textoDeclaracao2 = "A DLD SOLUÇÕES EM LOGISTICA REVERSA, GESTÃO E RECICLAGEM LTDA atesta a veracidade de todas as informações contidas neste relatório de rastreamento e destinação final de resíduos eletroeletrônicos obsoletos e descartes em geral.";

                var tf2 = new XTextFormatter(gfx);
                tf2.Alignment = XParagraphAlignment.Justify;

                var rectTexto2 = new XRect(margemEsq, y, larguraUtil, 50);
                tf2.DrawString(textoDeclaracao2, fontValor, XBrushes.Black, rectTexto2, XStringFormats.TopLeft);

                // Pular para o final da página para assinatura
                double yAssinatura = page.Height.Point - 220;
                // Data e local centralizados (TUDO EM NEGRITO e FONTE MAIOR)
                var mes = _mesAno.ToString("MMMM", new System.Globalization.CultureInfo("pt-BR"));
                mes = char.ToUpper(mes[0]) + mes.Substring(1); // primeira letra maiúscula

                string dataTexto = $"Araras/SP, {mes} de {_mesAno:yyyy}";

                var fontDataLocal = new XFont("Times New Roman", 18, XFontStyleEx.Bold);

                gfx.DrawString(dataTexto, fontDataLocal, XBrushes.Black,
                    new XRect(0, yAssinatura, page.Width.Point, 20),
                    XStringFormats.Center);

                yAssinatura += 35; // Aumentado de 30 para 35 para acomodar a fonte maior

                // Carregar e inserir imagem da assinatura (centralizada e REDIMENSIONADA)
                try
                {
                    string pathAssinatura = ExtrairRecursoParaTemp("Reverse.Resources.AssinaturaLaudo.jpg");
                    using (XImage imgAssinatura = XImage.FromFile(pathAssinatura))
                    {
                        // REDUZIDO: de 150 para 120 (largura menor)
                        double larguraImg = 120;
                        double alturaImg = (imgAssinatura.PointHeight / imgAssinatura.PointWidth) * larguraImg;

                        // LIMITADOR: se a altura calculada for muito grande, reduz proporcionalmente
                        if (alturaImg > 60)
                        {
                            alturaImg = 60;
                            larguraImg = (imgAssinatura.PointWidth / imgAssinatura.PointHeight) * alturaImg;
                        }

                        double xImg = (page.Width.Point - larguraImg) / 2;

                        gfx.DrawImage(imgAssinatura, xImg, yAssinatura, larguraImg, alturaImg);
                        yAssinatura += alturaImg + 5; // Reduzido espaço de 10 para 5
                    }
                }
                catch (Exception ex)
                {
                    // Se não encontrar a imagem, apenas loga (não quebra o PDF)
                    System.Diagnostics.Debug.WriteLine($"Assinatura não encontrada: {ex.Message}");
                    yAssinatura += 20; // Espaço reservado caso não haja imagem
                }

                // Nome abaixo da assinatura (centralizado)
                gfx.DrawString("________________________________", fontValor, XBrushes.Black,
                    new XRect(0, yAssinatura, page.Width.Point, 15),
                    XStringFormats.Center);

                yAssinatura += 15;

                gfx.DrawString("DLD Soluções em Logística Reversa", fontLabel, XBrushes.Black,
                    new XRect(0, yAssinatura, page.Width.Point, 15),
                    XStringFormats.Center);
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

        private void DesenharTextoQuebradoCentralizado(
            XGraphics gfx, string texto, XFont fonte, XBrush brush, XRect rect)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return;

            var tf = new XTextFormatter(gfx)
            {
                Alignment = XParagraphAlignment.Center
            };

            double alturaTexto = CalcularLinhas(gfx, texto, fonte, rect.Width) * fonte.GetHeight();

            double offsetY = rect.Y + (rect.Height - alturaTexto) / 2;

            var destino = new XRect(rect.X, offsetY, rect.Width, alturaTexto);

            tf.DrawString(texto, fonte, brush, destino, XStringFormats.TopLeft);
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