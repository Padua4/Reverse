using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Font = System.Drawing.Font;

namespace Reverse.Forms.FormsFinanceiro
{
    public partial class formRelatorioReceber : Form
    {
        #region Campos e Propriedades
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private readonly int loteId;
        private DataTable dadosRelatorio;
        private DateTime dataLote;
        private int totalContas;
        private decimal valorTotal, valorRecebido, valorPendente, valorAtrasado;

        // Estrutura para definir colunas
        private readonly struct ColumnConfig
        {
            public string Name { get; }
            public string DataProperty { get; }
            public int Width { get; }
            public string Header { get; }
            public string Format { get; }
            public DataGridViewContentAlignment Alignment { get; }

            public ColumnConfig(string name, string dataProperty, int width, string header,
                              string format = null, DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft)
            {
                Name = name;
                DataProperty = dataProperty;
                Width = width;
                Header = header;
                Format = format;
                Alignment = alignment;
            }
        }
        #endregion

        #region Construtor e Inicialização
        public formRelatorioReceber(int loteId)
        {
            if (loteId <= 0)
                throw new ArgumentException("ID do lote deve ser maior que zero", nameof(loteId));

            InitializeComponent();
            this.loteId = loteId;

            // Usar async/await para não travar a UI
            _ = InicializarFormularioAsync();
        }

        private async Task InicializarFormularioAsync()
        {
            try
            {
                // Mostrar indicador de carregamento
                Cursor = Cursors.WaitCursor;

                await Task.Run(() =>
                {
                    CarregarDadosRelatorio();
                });

                ConfigurarRelatorio();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar relatório: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        #endregion

        #region Configuração do Relatório
        private void ConfigurarRelatorio()
        {
            // Configurar DataGridView
            ConfigurarDataGridView();
            ConfigurarColunas();
        }

        private void ConfigurarDataGridView()
        {
            dgvRelatorio.AutoGenerateColumns = false;
            dgvRelatorio.ReadOnly = true;
            dgvRelatorio.AllowUserToAddRows = false;
            dgvRelatorio.AllowUserToDeleteRows = false;
            dgvRelatorio.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRelatorio.MultiSelect = false;
            dgvRelatorio.RowHeadersVisible = false;
            dgvRelatorio.EnableHeadersVisualStyles = false;

            // Melhorar aparência
            dgvRelatorio.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvRelatorio.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(64, 64, 64);
            dgvRelatorio.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRelatorio.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        }

        private void ConfigurarColunas()
        {
            dgvRelatorio.Columns.Clear();

            var colunas = new[]
            {
                new ColumnConfig("Cliente", "NomeCliente", 200, "Cliente"),
                new ColumnConfig("Descricao", "Descricao", 250, "Descrição"),
                new ColumnConfig("Valor", "Valor", 100, "Valor", "C2", DataGridViewContentAlignment.MiddleRight),
                new ColumnConfig("Vencimento", "DataVencimento", 100, "Vencimento", "dd/MM/yyyy", DataGridViewContentAlignment.MiddleCenter),
                new ColumnConfig("Recebimento", "DataRecebimento", 100, "Recebimento", "dd/MM/yyyy", DataGridViewContentAlignment.MiddleCenter),
                new ColumnConfig("Status", "Status", 120, "Status", null, DataGridViewContentAlignment.MiddleCenter),
                new ColumnConfig("Observacoes", "Observacoes", 200, "Observações")
            };

            foreach (var col in colunas)
            {
                var dataGridViewColumn = new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    DataPropertyName = col.DataProperty,
                    HeaderText = col.Header,
                    Width = col.Width
                };

                if (!string.IsNullOrEmpty(col.Format))
                {
                    dataGridViewColumn.DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = col.Format,
                        Alignment = col.Alignment,
                        NullValue = col.Name.Contains("Data") ? "" : null
                    };
                }
                else
                {
                    dataGridViewColumn.DefaultCellStyle.Alignment = col.Alignment;
                }

                // Colorir coluna de status
                if (col.Name == "Status")
                {
                    dataGridViewColumn.DefaultCellStyle.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
                }

                dgvRelatorio.Columns.Add(dataGridViewColumn);
            }

            // Configurar cores por status após o binding
            dgvRelatorio.DataBindingComplete += DgvRelatorio_DataBindingComplete;
        }

        private void DgvRelatorio_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvRelatorio.Rows)
            {
                if (row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();
                    Color corStatus = GetCorPorStatus(status);

                    row.Cells["Status"].Style.ForeColor = corStatus;
                }
            }
        }

        private Color GetCorPorStatus(string status)
        {
            return status switch
            {
                "Recebido" => Color.Green,
                "Atrasado" => Color.Red,
                "Vencimento Próximo" => Color.Orange,
                "Pendente" => Color.Blue,
                _ => Color.Black
            };
        }
        #endregion

        #region Carregamento de Dados
        private void CarregarDadosRelatorio()
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();

                // Usar transação para garantir consistência
                using var transaction = conn.BeginTransaction();

                try
                {
                    CarregarDadosLote(conn, transaction);
                    CarregarContasLote(conn, transaction);
                    CarregarTotais(conn, transaction);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Erro na base de dados: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar relatório: {ex.Message}", ex);
            }
        }

        private void CarregarDadosLote(SqlConnection conn, SqlTransaction transaction)
        {
            const string sqlLote = @"
                SELECT DataLote, COUNT(cr.ContaId) as TotalContas
                FROM LotesContasReceber lcr
                LEFT JOIN ContasReceber cr ON lcr.LoteId = cr.LoteId
                WHERE lcr.LoteId = @LoteId
                GROUP BY DataLote";

            using var cmd = new SqlCommand(sqlLote, conn, transaction);
            cmd.Parameters.AddWithValue("@LoteId", loteId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                dataLote = Convert.ToDateTime(reader["DataLote"]);
                totalContas = Convert.ToInt32(reader["TotalContas"]);

                // Thread safety
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                        lblTitulo.Text = $"RELATÓRIO - LOTE {dataLote:dd/MM/yyyy} ({totalContas} contas)"
                    ));
                }
                else
                {
                    lblTitulo.Text = $"RELATÓRIO - LOTE {dataLote:dd/MM/yyyy} ({totalContas} contas)";
                }
            }
            else
            {
                throw new Exception($"Lote {loteId} não encontrado.");
            }
        }

        private void CarregarContasLote(SqlConnection conn, SqlTransaction transaction)
        {
            const string sqlContas = @"
                SELECT 
                    ISNULL(c.Nome, 'Cliente não informado') as NomeCliente,
                    ISNULL(cr.Descricao, 'Sem descrição') as Descricao,
                    cr.Valor,
                    cr.DataVencimento,
                    cr.DataRecebimento,
                    ISNULL(cr.Observacoes, '') as Observacoes,
                    CASE 
                        WHEN cr.DataRecebimento IS NOT NULL THEN 'Recebido'
                        WHEN cr.DataVencimento < CAST(GETDATE() AS DATE) THEN 'Atrasado'
                        WHEN DATEDIFF(DAY, CAST(GETDATE() AS DATE), cr.DataVencimento) <= 3 THEN 'Vencimento Próximo'
                        ELSE 'Pendente'
                    END as Status
                FROM ContasReceber cr
                LEFT JOIN Clientes c ON cr.ClienteId = c.ClienteId
                WHERE cr.LoteId = @LoteId
                ORDER BY 
                    CASE 
                        WHEN cr.DataRecebimento IS NOT NULL THEN 4
                        WHEN cr.DataVencimento < CAST(GETDATE() AS DATE) THEN 1
                        WHEN DATEDIFF(DAY, CAST(GETDATE() AS DATE), cr.DataVencimento) <= 3 THEN 2
                        ELSE 3
                    END,
                    cr.DataVencimento, 
                    c.Nome";

            using var da = new SqlDataAdapter(sqlContas, conn);
            da.SelectCommand.Transaction = transaction;
            da.SelectCommand.Parameters.AddWithValue("@LoteId", loteId);

            dadosRelatorio = new DataTable();
            da.Fill(dadosRelatorio);

            // Garantir que temos dados válidos
            if (dadosRelatorio == null || dadosRelatorio.Rows.Count == 0)
            {
                // Criar DataTable vazio com estrutura correta
                dadosRelatorio = new DataTable();
                dadosRelatorio.Columns.Add("NomeCliente", typeof(string));
                dadosRelatorio.Columns.Add("Descricao", typeof(string));
                dadosRelatorio.Columns.Add("Valor", typeof(decimal));
                dadosRelatorio.Columns.Add("DataVencimento", typeof(DateTime));
                dadosRelatorio.Columns.Add("DataRecebimento", typeof(DateTime));
                dadosRelatorio.Columns.Add("Observacoes", typeof(string));
                dadosRelatorio.Columns.Add("Status", typeof(string));
            }

            if (InvokeRequired)
            {
                Invoke(new Action(() => dgvRelatorio.DataSource = dadosRelatorio));
            }
            else
            {
                dgvRelatorio.DataSource = dadosRelatorio;
            }
        }

        private void CarregarTotais(SqlConnection conn, SqlTransaction transaction)
        {
            const string sqlTotais = @"
                SELECT 
                    COUNT(*) as TotalContas,
                    ISNULL(SUM(Valor), 0) as ValorTotal,
                    ISNULL(SUM(CASE WHEN DataRecebimento IS NOT NULL THEN Valor ELSE 0 END), 0) as ValorRecebido,
                    ISNULL(SUM(CASE WHEN DataRecebimento IS NULL THEN Valor ELSE 0 END), 0) as ValorPendente,
                    ISNULL(SUM(CASE WHEN DataRecebimento IS NULL AND DataVencimento < CAST(GETDATE() AS DATE) THEN Valor ELSE 0 END), 0) as ValorAtrasado
                FROM ContasReceber 
                WHERE LoteId = @LoteId";

            using var cmd = new SqlCommand(sqlTotais, conn, transaction);
            cmd.Parameters.AddWithValue("@LoteId", loteId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                totalContas = Convert.ToInt32(reader["TotalContas"]);
                valorTotal = Convert.ToDecimal(reader["ValorTotal"]);
                valorRecebido = Convert.ToDecimal(reader["ValorRecebido"]);
                valorPendente = Convert.ToDecimal(reader["ValorPendente"]);
                valorAtrasado = Convert.ToDecimal(reader["ValorAtrasado"]);

                // Thread safety
                if (InvokeRequired)
                {
                    Invoke(new Action(() => AtualizarLabelsResumo()));
                }
                else
                {
                    AtualizarLabelsResumo();
                }
            }
        }

        private void AtualizarLabelsResumo()
        {
            lblTotalContas.Text = $"Total de Contas: {totalContas}";
            lblValorTotal.Text = $"Valor Total: {valorTotal:C2}";
            lblValorRecebido.Text = $"Valor Recebido: {valorRecebido:C2}";
            lblValorPendente.Text = $"Valor Pendente: {valorPendente:C2}";
            lblValorAtrasado.Text = $"Valor Atrasado: {valorAtrasado:C2}";

            // Colorir labels conforme valores
            lblValorRecebido.ForeColor = Color.Green;
            lblValorPendente.ForeColor = valorPendente > 0 ? Color.Orange : Color.Black;
            lblValorAtrasado.ForeColor = valorAtrasado > 0 ? Color.Red : Color.Black;
        }
        #endregion

        #region Event Handlers
        private async void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Criar PDF temporário para impressão
                string tempFile = Path.Combine(Path.GetTempPath(), $"relatorio_temp_{Guid.NewGuid()}.pdf");

                await Task.Run(() => ExportarParaPDF(tempFile));

                // Abrir com o aplicativo padrão para impressão
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempFile,
                    UseShellExecute = true
                });

                MessageBox.Show("Arquivo PDF aberto. Use Ctrl+P para imprimir.", "Informação",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao preparar impressão: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "Arquivo PDF (*.pdf)|*.pdf|Arquivo CSV (*.csv)|*.csv|Arquivo Excel (*.xlsx)|*.xlsx",
                    Title = "Exportar Relatório",
                    FileName = $"Relatorio_Contas_Receber_Lote_{loteId}_{DateTime.Now:yyyyMMdd_HHmm}"
                };

                if (sfd.ShowDialog() != DialogResult.OK) return;

                Cursor = Cursors.WaitCursor;

                await Task.Run(() =>
                {
                    string extension = Path.GetExtension(sfd.FileName).ToLowerInvariant();

                    switch (extension)
                    {
                        case ".pdf":
                            ExportarParaPDF(sfd.FileName);
                            break;
                        case ".csv":
                            ExportarParaCSV(sfd.FileName);
                            break;
                        case ".xlsx":
                            ExportarParaExcel(sfd.FileName);
                            break;
                        default:
                            throw new NotSupportedException("Formato de arquivo não suportado.");
                    }
                });

                MessageBox.Show($"Relatório exportado com sucesso para:\n{sfd.FileName}", "Sucesso",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region Métodos de Exportação
        private void ExportarParaPDF(string caminhoArquivo)
        {
            Document document = null;
            FileStream fs = null;

            try
            {
                // Verificar se temos dados para exportar
                if (dadosRelatorio == null)
                {
                    throw new Exception("Nenhum dado encontrado para exportar.");
                }

                document = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
                fs = new FileStream(caminhoArquivo, FileMode.Create, FileAccess.Write);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                // Título
                var fonteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16f, BaseColor.BLACK);
                var titulo = new Paragraph($"RELATÓRIO DE CONTAS A RECEBER - LOTE {loteId}", fonteTitulo)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10
                };
                document.Add(titulo);

                // Informações do lote
                var fonteLote = FontFactory.GetFont(FontFactory.HELVETICA, 12f);
                var infoLote = new Paragraph($"Data do Lote: {dataLote:dd/MM/yyyy} | Total de Contas: {totalContas}", fonteLote)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10
                };
                document.Add(infoLote);

                // Data de emissão
                var fonteData = FontFactory.GetFont(FontFactory.HELVETICA, 10f);
                var dataEmissao = new Paragraph($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}", fonteData)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 15
                };
                document.Add(dataEmissao);

                // Resumo financeiro
                AdicionarResumoFinanceiro(document);

                // Espaçamento
                document.Add(new Paragraph(" ", FontFactory.GetFont(FontFactory.HELVETICA, 8f)));

                // Tabela de detalhes - só adicionar se houver dados
                if (dadosRelatorio.Rows.Count > 0)
                {
                    AdicionarTabelaDetalhes(document);
                }
                else
                {
                    var semDados = new Paragraph("Nenhuma conta encontrada para este lote.",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12f))
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingBefore = 20
                    };
                    document.Add(semDados);
                }

                document.Close();
            }
            catch (Exception ex)
            {
                document?.Close();
                throw new Exception($"Erro ao exportar para PDF: {ex.Message}", ex);
            }
            finally
            {
                fs?.Close();
                fs?.Dispose();
            }
        }

        private void AdicionarResumoFinanceiro(Document document)
        {
            var fonteResumo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12f);
            var resumo = new Paragraph("RESUMO FINANCEIRO", fonteResumo)
            {
                SpacingAfter = 10,
                SpacingBefore = 5
            };
            document.Add(resumo);

            var fonteDados = FontFactory.GetFont(FontFactory.HELVETICA, 10f);

            // Criar tabela para o resumo
            var tabelaResumo = new PdfPTable(2) { WidthPercentage = 60 };
            tabelaResumo.SetWidths(new float[] { 3f, 2f });

            // Dados do resumo usando as variáveis de instância
            var dadosResumo = new Dictionary<string, string>
            {
                { "Total de Contas", totalContas.ToString() },
                { "Valor Total", valorTotal.ToString("C2") },
                { "Valor Recebido", valorRecebido.ToString("C2") },
                { "Valor Pendente", valorPendente.ToString("C2") },
                { "Valor Atrasado", valorAtrasado.ToString("C2") }
            };

            foreach (var item in dadosResumo)
            {
                var celulaLabel = new PdfPCell(new Phrase(item.Key + ":", fonteDados))
                {
                    Border = iTextSharp.text.Rectangle.NO_BORDER,
                    PaddingBottom = 5
                };
                tabelaResumo.AddCell(celulaLabel);

                var celulaValor = new PdfPCell(new Phrase(item.Value, fonteDados))
                {
                    Border = iTextSharp.text.Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    PaddingBottom = 5
                };
                tabelaResumo.AddCell(celulaValor);
            }

            document.Add(tabelaResumo);
        }

        private void AdicionarTabelaDetalhes(Document document)
        {
            var fonteDetalhe = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f);
            var detalhe = new Paragraph("DETALHES DAS CONTAS", fonteDetalhe)
            {
                SpacingAfter = 10,
                SpacingBefore = 15
            };
            document.Add(detalhe);

            // Verificar se temos colunas válidas
            if (dadosRelatorio.Columns.Count == 0)
            {
                document.Add(new Paragraph("Erro: estrutura de dados inválida.",
                    FontFactory.GetFont(FontFactory.HELVETICA, 10f)));
                return;
            }

            // Criar tabela com número correto de colunas
            var tabela = new PdfPTable(dadosRelatorio.Columns.Count) { WidthPercentage = 100 };

            // Definir larguras das colunas proporcionalmente
            float[] larguras = { 2.5f, 3f, 1.2f, 1.2f, 1.2f, 1.3f, 2.5f };
            if (larguras.Length == dadosRelatorio.Columns.Count)
            {
                tabela.SetWidths(larguras);
            }

            // Cabeçalhos
            foreach (DataColumn coluna in dadosRelatorio.Columns)
            {
                string headerText = GetHeaderText(coluna.ColumnName);

                var celula = new PdfPCell(new Phrase(headerText,
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9f, BaseColor.WHITE)))
                {
                    BackgroundColor = new BaseColor(64, 64, 64),
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };

                tabela.AddCell(celula);
            }

            // Dados
            foreach (DataRow linha in dadosRelatorio.Rows)
            {
                foreach (DataColumn coluna in dadosRelatorio.Columns)
                {
                    string valor = FormatarValorParaExportacao(linha[coluna], coluna.ColumnName);

                    var celula = new PdfPCell(new Phrase(valor,
                        FontFactory.GetFont(FontFactory.HELVETICA, 8f)))
                    {
                        Padding = 3
                    };

                    // Alinhamento baseado no tipo de dados
                    if (coluna.ColumnName == "Valor")
                        celula.HorizontalAlignment = Element.ALIGN_RIGHT;
                    else if (coluna.ColumnName.Contains("Data") || coluna.ColumnName == "Status")
                        celula.HorizontalAlignment = Element.ALIGN_CENTER;
                    else
                        celula.HorizontalAlignment = Element.ALIGN_LEFT;

                    tabela.AddCell(celula);
                }
            }

            document.Add(tabela);
        }

        private string GetHeaderText(string columnName)
        {
            return columnName switch
            {
                "NomeCliente" => "Cliente",
                "Descricao" => "Descrição",
                "Valor" => "Valor",
                "DataVencimento" => "Vencimento",
                "DataRecebimento" => "Recebimento",
                "Status" => "Status",
                "Observacoes" => "Observações",
                _ => columnName
            };
        }

        private void ExportarParaCSV(string caminhoArquivo)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(caminhoArquivo, false, Encoding.UTF8);

                // Verificar se temos dados
                if (dadosRelatorio == null || dadosRelatorio.Columns.Count == 0)
                {
                    sw.WriteLine("Nenhum dado encontrado para exportar");
                    return;
                }

                // Cabeçalho
                var cabecalhos = new List<string>();
                foreach (DataColumn coluna in dadosRelatorio.Columns)
                {
                    cabecalhos.Add(GetHeaderText(coluna.ColumnName));
                }
                sw.WriteLine(string.Join(";", cabecalhos));

                // Dados
                foreach (DataRow linha in dadosRelatorio.Rows)
                {
                    var valores = new List<string>();

                    foreach (DataColumn coluna in dadosRelatorio.Columns)
                    {
                        string valor = FormatarValorParaExportacao(linha[coluna], coluna.ColumnName);

                        // Escapar para CSV
                        if (valor.Contains(";") || valor.Contains("\"") || valor.Contains("\n") || valor.Contains("\r"))
                        {
                            valor = $"\"{valor.Replace("\"", "\"\"")}\"";
                        }

                        valores.Add(valor);
                    }

                    sw.WriteLine(string.Join(";", valores));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao exportar para CSV: {ex.Message}", ex);
            }
            finally
            {
                sw?.Close();
                sw?.Dispose();
            }
        }

        private void ExportarParaExcel(string caminhoArquivo)
        {
            try
            {
                string tempCsv = Path.ChangeExtension(caminhoArquivo, ".csv");
                ExportarParaCSV(tempCsv);

                if (File.Exists(caminhoArquivo))
                    File.Delete(caminhoArquivo);

                File.Move(tempCsv, caminhoArquivo);

                MessageBox.Show("Arquivo exportado em formato CSV com extensão Excel.\n" +
                              "Para funcionalidade completa do Excel, instale a biblioteca EPPlus.",
                              "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao exportar para Excel: {ex.Message}", ex);
            }
        }

        private string FormatarValorParaExportacao(object valor, string nomeColuna)
        {
            if (valor == null || valor == DBNull.Value)
                return "";

            string valorStr = valor.ToString().Trim();

            if (string.IsNullOrEmpty(valorStr))
                return "";

            return nomeColuna switch
            {
                "Valor" when decimal.TryParse(valorStr, out decimal valorDecimal) =>
                    valorDecimal.ToString("C2"),
                "DataVencimento" or "DataRecebimento" when DateTime.TryParse(valorStr, out DateTime data) =>
                    data.ToString("dd/MM/yyyy"),
                _ => valorStr
            };
        }
        #endregion

        #region Cleanup
        private void LimparRecursos()
        {
            try
            {
                dadosRelatorio?.Dispose();
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LimparRecursos();
            base.OnFormClosing(e);
        }
        #endregion
    }
}