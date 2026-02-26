using Microsoft.Web.WebView2.WinForms;
using System.Reflection;
using PdfiumViewer;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormTickets : Form
    {
        public event Action TicketGerado;
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private int controleLogisticoId;
        private int clienteId;
        private bool isPrimeiraVez = false;

        private bool linhaJaTemTicket = false;

        private DadosControle dadosControle;
        private DadosCliente dadosCliente;
        private DadosVeiculo dadosVeiculo;

        public ExpedicaoFormTickets(int controleLogisticoId)
        {
            InitializeComponent();
            this.controleLogisticoId = controleLogisticoId;

            this.clienteId = 0;
            this.isPrimeiraVez = false;
            this.linhaJaTemTicket = false;
            this.dadosControle = null;
            this.dadosCliente = null;
            this.dadosVeiculo = null;

            this.Load += FormTicket_Load;
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        private void panelTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }


        private PdfiumViewer.PdfViewer pdfViewer;
        private async void FormTicket_Load(object sender, EventArgs e)
        {
            pdfViewer = new PdfViewer();
            pdfViewer.Dock = DockStyle.Fill;
            pnlDireito.Controls.Add(pdfViewer);

            await CarregarDadosAsync();
            PreencherCampos();

            if (!linhaJaTemTicket)
                await VerificarPrimeiraVez();

            string caminhoPreview = GerarPDFPreview();
            MostrarPreviewPDF(caminhoPreview);
        }

        private async System.Threading.Tasks.Task CarregarDadosAsync()
        {
            if (controleLogisticoId <= 0)
            {
                MessageBox.Show("ID de controle inválido!", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sqlControle = @"
                    SELECT 
                        cl.Id, cl.ClienteId, cl.Gerador, cl.Localidade, cl.Data, 
                        cl.Ticket, cl.NF, cl.MTR, cl.Codigo, cl.Lote, cl.Volume, 
                        cl.Observacoes, cl.ModeloVeiculo, cl.VeiculoId, cl.TipoVeiculo,
                        cl.Motorista, cl.Ajudante1, cl.Ajudante2, cl.Servico, 
                        cl.Destino, cl.Peso, cl.StatusLogistica, cl.StatusLaudo,
                        c.CodigoEmpresa, 
                        c.Nome AS NomeCliente,
                        c.RazaoSocial,
                        c.ComplementoEntrega,
                        c.RuaEntrega, c.BairroEntrega, c.NumeroEntrega, 
                        c.MunicipioEntrega, c.EstadoEntrega, c.Telefone, c.ResponsavelComercial,
                        c.CPF_CNPJ, c.NomeContato,
                        ISNULL(c.TicketSequencialGeral, 0) AS TicketSequencialGeral,
                        ISNULL(c.TicketSequencialAnoAtual, 1) AS TicketSequencialAnoAtual,
                        ISNULL(c.TicketUltimoAno, YEAR(GETDATE())) AS TicketUltimoAno,
                        v.Placa, v.Categoria AS CategoriaVeiculo
                    FROM ControleLogistico cl
                    LEFT JOIN Clientes c ON cl.ClienteId = c.ClienteId
                    LEFT JOIN Veiculos v ON cl.VeiculoId = v.VeiculoId
                    WHERE cl.Id = @Id
                    ";

                var cmd = new SqlCommand(sqlControle, conn);
                cmd.Parameters.AddWithValue("@Id", controleLogisticoId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        clienteId = reader["ClienteId"] != DBNull.Value
                            ? Convert.ToInt32(reader["ClienteId"])
                            : 0;

                        dadosControle = new DadosControle
                        {
                            Gerador = reader["Gerador"]?.ToString() ?? "",
                            Data = reader["Data"] != DBNull.Value
                                ? Convert.ToDateTime(reader["Data"])
                                : DateTime.Now,
                            Servico = reader["Servico"]?.ToString() ?? "",
                            Motorista = reader["Motorista"]?.ToString() ?? "",
                            Ajudante1 = reader["Ajudante1"]?.ToString() ?? "",
                            Ajudante2 = reader["Ajudante2"]?.ToString() ?? "",
                            ModeloVeiculo = reader["ModeloVeiculo"]?.ToString() ?? "",
                            Observacoes = reader["Observacoes"]?.ToString() ?? ""
                        };

                        dadosCliente = new DadosCliente
                        {
                            CodigoEmpresa = reader["CodigoEmpresa"]?.ToString() ?? "",
                            Nome = reader["NomeCliente"]?.ToString() ?? "",
                            RazaoSocial = reader["RazaoSocial"]?.ToString() ?? "",
                            ComplementoEntrega = reader["ComplementoEntrega"]?.ToString() ?? "",
                            RuaEntrega = reader["RuaEntrega"]?.ToString() ?? "",
                            BairroEntrega = reader["BairroEntrega"]?.ToString() ?? "",
                            NumeroEntrega = reader["NumeroEntrega"]?.ToString() ?? "",
                            MunicipioEntrega = reader["MunicipioEntrega"]?.ToString() ?? "",
                            EstadoEntrega = reader["EstadoEntrega"]?.ToString() ?? "",
                            Telefone = reader["Telefone"]?.ToString() ?? "",
                            CNPJ = reader["CPF_CNPJ"]?.ToString() ?? "",
                            NomeContato = reader["NomeContato"]?.ToString() ?? "",
                            Responsavel = reader["ResponsavelComercial"]?.ToString() ?? "",
                            TicketSequencialGeral = reader["TicketSequencialGeral"] != DBNull.Value
                                ? Convert.ToInt32(reader["TicketSequencialGeral"])
                                : 0,
                            TicketSequencialAnoAtual = reader["TicketSequencialAnoAtual"] != DBNull.Value
                                ? Convert.ToInt32(reader["TicketSequencialAnoAtual"])
                                : 1,
                            TicketUltimoAno = reader["TicketUltimoAno"] != DBNull.Value
                                ? Convert.ToInt32(reader["TicketUltimoAno"])
                                : DateTime.Now.Year
                        };

                        dadosVeiculo = new DadosVeiculo
                        {
                            Placa = reader["Placa"]?.ToString() ?? "",
                            Categoria = reader["CategoriaVeiculo"]?.ToString() ?? ""
                        };

                        string ticketExistente = reader["Ticket"]?.ToString();
                        linhaJaTemTicket = !string.IsNullOrWhiteSpace(ticketExistente);

                        if (linhaJaTemTicket)
                        {
                            txtTicket.Text = ticketExistente;
                        }

                        if (reader["ClienteId"] == DBNull.Value)
                        {
                            MessageBox.Show("Registro sem ClienteId associado!", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                            return;
                        }

                        clienteId = Convert.ToInt32(reader["ClienteId"]);

                        if (clienteId <= 0)
                        {
                            MessageBox.Show($"ClienteId inválido: {clienteId}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Registro com Id={controleLogisticoId} não encontrado!",
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }
                }
            }
        }
        private void PreencherCampos()
        {
            bool dldEhReceptor = dadosControle.Servico.Contains("COLETA") ||
                                 dadosControle.Servico.Contains("RECEBIMENTO") ||
                                 dadosControle.Servico.Contains("DESCARTE");

            if (dldEhReceptor)
            {
                txtGerador.Text = dadosCliente.Nome;
                txtReceptor.Text = "DLD SOLUÇÕES LOGÍSTICA REVERSA, GESTÃO E RECICLAGEM";
                dtpColeta.Value = dadosControle.Data;
                dtpEntrega.Value = dadosControle.Data;
            }
            else
            {
                txtGerador.Text = "DLD SOLUÇÕES LOGÍSTICA REVERSA, GESTÃO E RECICLAGEM";
                txtReceptor.Text = dadosCliente.Nome;
                dtpColeta.Value = dadosControle.Data;
                dtpEntrega.Value = dadosControle.Data;
            }

            txtRuaCliente.Text = dadosCliente.RuaEntrega;
            txtBairroCliente.Text = dadosCliente.BairroEntrega;
            txtNumeroCliente.Text = dadosCliente.NumeroEntrega;
            txtMunicioCliente.Text = dadosCliente.MunicipioEntrega;
            txtUFCliente.Text = dadosCliente.EstadoEntrega;
            txtTelCliente.Text = dadosCliente.Telefone;

            txtResCliente.Text = !string.IsNullOrWhiteSpace(dadosCliente.Responsavel)
                ? dadosCliente.Responsavel
                : dadosCliente.NomeContato;
        }

        private async Task VerificarPrimeiraVez()
        {
            isPrimeiraVez = (dadosCliente.TicketSequencialGeral == 0 && !linhaJaTemTicket);

            if (isPrimeiraVez)
            {
                txtTicket.ReadOnly = false;
                txtTicket.BackColor = Color.LightYellow;

                string codigo4 = (dadosCliente.CodigoEmpresa ?? "").Trim();
                if (int.TryParse(codigo4, out var codNum))
                    codigo4 = codNum.ToString("D4");
                else
                    codigo4 = codigo4.PadLeft(4, '0');

                txtTicket.Text = $"{codigo4}/0001-1";

                MessageBox.Show(
                    "Este é o primeiro ticket deste cliente no sistema.\n\n" +
                    "Informe o número atual conforme o novo formato.\n\n" +
                    "Formato: CCCC/SSSS-S\n" +
                    "Exemplo: 0188/0100-1\n\n" +
                    "Regras:\n" +
                    "- CCCC: código da empresa com 4 dígitos (zero à esquerda)\n" +
                    "- SSSS: sequência geral 0001..9999\n" +
                    "- S: serial, incrementa quando a sequência volta a 0001",
                    "Primeira Geração de Ticket",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if (!linhaJaTemTicket)
            {
                await GerarNumeroTicketAutomatico();
                txtTicket.ReadOnly = false;
                txtTicket.BackColor = Color.LightYellow;
            }
            else
            {
                txtTicket.ReadOnly = false;
                txtTicket.BackColor = Color.LightYellow;
            }
        }


        private string GerarPDFPreview()
        {
            string pastaTemp = Path.GetTempPath();
            string nomeArquivo = $"Preview_{Guid.NewGuid()}.pdf";
            string caminhoCompleto = Path.Combine(pastaTemp, nomeArquivo);

            return GerarPDFEm(caminhoCompleto, preview: true);
        }
        private void MostrarPreviewPDF(string caminhoArquivo)
        {
            pdfViewer.Document?.Dispose();
            pdfViewer.Document = PdfiumViewer.PdfDocument.Load(caminhoArquivo);
        }

        private async Task GerarNumeroTicketAutomatico()
        {
            int clienteIdLocal = this.clienteId;

            if (clienteIdLocal <= 0)
            {
                throw new InvalidOperationException($"ClienteId inválido: {clienteIdLocal}");
            }

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        var cmdLock = new SqlCommand(@"
                            SELECT TicketSequencialGeral, TicketSequencialAnoAtual
                            FROM Clientes WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
                            WHERE ClienteId = @ClienteId", conn, transaction);

                        cmdLock.Parameters.AddWithValue("@ClienteId", clienteIdLocal);

                        int seqGeralAtual;
                        int serialAtual;

                        using (var reader = await cmdLock.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                            {
                                throw new Exception($"Cliente {clienteIdLocal} não encontrado!");
                            }

                            seqGeralAtual = reader["TicketSequencialGeral"] != DBNull.Value
                                ? Convert.ToInt32(reader["TicketSequencialGeral"])
                                : 0;

                            serialAtual = reader["TicketSequencialAnoAtual"] != DBNull.Value
                                ? Convert.ToInt32(reader["TicketSequencialAnoAtual"])
                                : 1;
                        }

                        int novoSeqGeral = seqGeralAtual + 1;

                        if (novoSeqGeral > 9999)
                        {
                            novoSeqGeral = 1;
                            serialAtual += 1;
                        }

                        string codigo4 = (dadosCliente.CodigoEmpresa ?? "").Trim();
                        if (int.TryParse(codigo4, out var codNum))
                            codigo4 = codNum.ToString("D4");
                        else
                            codigo4 = codigo4.PadLeft(4, '0');

                        txtTicket.Text = $"{codigo4}/{novoSeqGeral:D4}-{serialAtual}";

                        dadosCliente.TicketSequencialGeral = seqGeralAtual;
                        dadosCliente.TicketSequencialAnoAtual = serialAtual;

                        transaction.Commit();

                        System.Diagnostics.Debug.WriteLine(
                            $"✅ TICKET GERADO - Cliente: {clienteIdLocal}, " +
                            $"Ticket: {txtTicket.Text}, " +
                            $"Usuário: {Environment.UserName}, " +
                            $"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private async void btnExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTicket.Text))
                {
                    MessageBox.Show("Número do ticket não pode estar vazio.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarFormatoTicket(txtTicket.Text))
                {
                    MessageBox.Show("Formato inválido. Use: CCCC/SSSS-S\nExemplo: 0011/0001-1",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (linhaJaTemTicket)
                {
                    await AtualizarSequenciaisCliente(txtTicket.Text);
                }
                else
                {
                    if (isPrimeiraVez)
                    {
                        await AtualizarSequenciaisCliente(txtTicket.Text);
                    }
                    else
                    {
                        await IncrementarSequenciais();
                    }

                    txtTicket.ReadOnly = true;
                    txtTicket.BackColor = SystemColors.Control;
                }

                string caminhoArquivo = GerarPDF();
                MostrarPreviewPDF(caminhoArquivo);

                var parentForm = Application.OpenForms["ExpedicaoFormControle"];
                if (parentForm != null && parentForm is ExpedicaoFormControle controleForm)
                {
                    var txtTicketControle = controleForm.Controls.Find("txtTicket", true).FirstOrDefault() as TextBox;
                    if (txtTicketControle != null)
                    {
                        txtTicketControle.Text = txtTicket.Text;
                    }
                }

                var resultado = MessageBox.Show(
                    $"PDF gerado com sucesso!\n\n{caminhoArquivo}\n\nDeseja abrir o arquivo?",
                    "Sucesso",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (resultado == DialogResult.Yes)
                    System.Diagnostics.Process.Start(caminhoArquivo);

                TicketGerado?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarFormatoTicket(string ticket)
        {
            var partesBarra = ticket.Split('/');
            if (partesBarra.Length != 2) return false;

            var codigo = partesBarra[0];
            var resto = partesBarra[1];

            var partesTraco = resto.Split('-');
            if (partesTraco.Length != 2) return false;

            var seq = partesTraco[0];
            var serialStr = partesTraco[1];

            if (codigo.Length != 4 || !int.TryParse(codigo, out _)) return false;
            if (seq.Length != 4 || !int.TryParse(seq, out var seqVal)) return false;
            if (!int.TryParse(serialStr, out var serialVal)) return false;

            if (seqVal < 1 || seqVal > 9999) return false;
            if (serialVal < 1) return false;

            return true;
        }

        private async System.Threading.Tasks.Task AtualizarSequenciaisCliente(string ticketManual)
        {
            // CRÍTICO: Captura IDs no início
            int clienteIdLocal = this.clienteId;
            int controleLogisticoIdLocal = this.controleLogisticoId;

            if (clienteIdLocal <= 0)
            {
                throw new InvalidOperationException($"ClienteId inválido: {clienteIdLocal}");
            }

            if (controleLogisticoIdLocal <= 0)
            {
                throw new InvalidOperationException($"ControleLogisticoId inválido: {controleLogisticoIdLocal}");
            }

            var partesBarra = ticketManual.Split('/');
            var codigo = partesBarra[0];
            var resto = partesBarra[1];
            var partesTraco = resto.Split('-');

            int seqGeral = int.Parse(partesTraco[0]);
            int serial = int.Parse(partesTraco[1]);

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        var cmdLock = new SqlCommand(@"
                SELECT TicketSequencialGeral, TicketSequencialAnoAtual
                FROM Clientes WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
                WHERE ClienteId = @ClienteId", conn, transaction);

                        cmdLock.Parameters.AddWithValue("@ClienteId", clienteIdLocal); // ← LOCAL

                        using (var reader = await cmdLock.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                            {
                                throw new Exception($"Cliente {clienteIdLocal} não encontrado!");
                            }

                            int seqAtualBanco = reader["TicketSequencialGeral"] != DBNull.Value
                                ? Convert.ToInt32(reader["TicketSequencialGeral"])
                                : 0;

                            int serialAtualBanco = reader["TicketSequencialAnoAtual"] != DBNull.Value
                                ? Convert.ToInt32(reader["TicketSequencialAnoAtual"])
                                : 1;

                            if (seqGeral <= seqAtualBanco && !isPrimeiraVez)
                            {
                                reader.Close();
                                throw new Exception(
                                    $"ERRO: O ticket {ticketManual} é MENOR OU IGUAL ao último registrado no sistema.\n\n" +
                                    $"Último ticket no banco: {codigo}/{seqAtualBanco:D4}-{serialAtualBanco}\n" +
                                    $"Ticket tentando salvar: {ticketManual}\n\n" +
                                    $"Não é possível regredir ou repetir a sequência.\n" +
                                    $"O próximo ticket válido seria: {codigo}/{(seqAtualBanco + 1):D4}-{serialAtualBanco}");
                            }
                        }

                        string ticketAntigo = null;
                        var cmdBusca = new SqlCommand(@"
                            SELECT Ticket 
                            FROM ControleLogistico 
                            WHERE Id = @ControleId", conn, transaction);
                                    cmdBusca.Parameters.AddWithValue("@ControleId", controleLogisticoIdLocal); // ← LOCAL

                        var resultado = await cmdBusca.ExecuteScalarAsync();
                        if (resultado != null && resultado != DBNull.Value)
                        {
                            ticketAntigo = resultado.ToString();
                        }

                        var cmdCliente = new SqlCommand(@"
                            UPDATE Clientes 
                            SET TicketSequencialGeral = @SeqGeral,
                                TicketSequencialAnoAtual = @Serial,
                                TicketUltimoAno = YEAR(GETDATE())
                            WHERE ClienteId = @ClienteId", conn, transaction);

                        cmdCliente.Parameters.AddWithValue("@SeqGeral", seqGeral);
                        cmdCliente.Parameters.AddWithValue("@Serial", serial);
                        cmdCliente.Parameters.AddWithValue("@ClienteId", clienteIdLocal); // ← LOCAL

                        int linhasAfetadasCliente = await cmdCliente.ExecuteNonQueryAsync();
                        if (linhasAfetadasCliente == 0)
                            throw new Exception($"Cliente {clienteIdLocal} não foi encontrado para atualizar sequenciais!");

                        var cmdControle = new SqlCommand(@"
                            UPDATE ControleLogistico
                            SET Ticket = @Ticket
                            WHERE Id = @ControleId", conn, transaction);

                        cmdControle.Parameters.AddWithValue("@Ticket", ticketManual);
                        cmdControle.Parameters.AddWithValue("@ControleId", controleLogisticoIdLocal); // ← LOCAL

                        await cmdControle.ExecuteNonQueryAsync();

                        if (!string.IsNullOrWhiteSpace(ticketAntigo) && ticketAntigo != ticketManual)
                        {
                            var cmdLancamentos = new SqlCommand(@"
                                UPDATE LancamentosMateriais 
                                SET Ticket = @TicketNovo 
                                WHERE Ticket = @TicketAntigo", conn, transaction);

                            cmdLancamentos.Parameters.AddWithValue("@TicketAntigo", ticketAntigo);
                            cmdLancamentos.Parameters.AddWithValue("@TicketNovo", ticketManual);

                            int linhasAtualizadas = await cmdLancamentos.ExecuteNonQueryAsync();

                            if (linhasAtualizadas > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"✅ {linhasAtualizadas} lançamento(s) atualizado(s) de '{ticketAntigo}' para '{ticketManual}'");
                            }
                        }

                        transaction.Commit();

                        System.Diagnostics.Debug.WriteLine(
                            $"✅ TICKET ATUALIZADO - Cliente: {clienteIdLocal}, " +
                            $"Ticket: {ticketManual}, " +
                            $"SeqGeral: {seqGeral}, Serial: {serial}, " +
                            $"Usuário: {Environment.UserName}, " +
                            $"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

                        dadosCliente.TicketSequencialGeral = seqGeral;
                        dadosCliente.TicketSequencialAnoAtual = serial;
                        dadosCliente.TicketUltimoAno = DateTime.Now.Year;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        private async System.Threading.Tasks.Task IncrementarSequenciais()
        {
            int clienteIdLocal = this.clienteId;
            int controleLogisticoIdLocal = this.controleLogisticoId;

            if (clienteIdLocal <= 0)
            {
                throw new InvalidOperationException($"ClienteId inválido: {clienteIdLocal}");
            }

            if (controleLogisticoIdLocal <= 0)
            {
                throw new InvalidOperationException($"ControleLogisticoId inválido: {controleLogisticoIdLocal}");
            }

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        var cmdLer = new SqlCommand(@"
                            SELECT TicketSequencialGeral, TicketSequencialAnoAtual
                            FROM Clientes WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
                            WHERE ClienteId = @ClienteId", conn, transaction);

                        cmdLer.Parameters.AddWithValue("@ClienteId", clienteIdLocal); // ← LOCAL

                        int seqGeralAtual;
                        int serialAtual;

                        using (var reader = await cmdLer.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                            {
                                throw new Exception($"Cliente {clienteIdLocal} não encontrado!");
                            }

                            seqGeralAtual = reader["TicketSequencialGeral"] != DBNull.Value
                                ? Convert.ToInt32(reader["TicketSequencialGeral"])
                                : 0;

                            serialAtual = reader["TicketSequencialAnoAtual"] != DBNull.Value
                                ? Convert.ToInt32(reader["TicketSequencialAnoAtual"])
                                : 1;
                        }

                        int novoSeqGeral = seqGeralAtual + 1;

                        if (novoSeqGeral > 9999)
                        {
                            novoSeqGeral = 1;
                            serialAtual += 1;
                        }

                        string codigo4 = (dadosCliente.CodigoEmpresa ?? "").Trim();
                        if (int.TryParse(codigo4, out var codNum))
                            codigo4 = codNum.ToString("D4");
                        else
                            codigo4 = codigo4.PadLeft(4, '0');

                        string novoTicket = $"{codigo4}/{novoSeqGeral:D4}-{serialAtual}";

                        var cmdCliente = new SqlCommand(@"
                            UPDATE Clientes 
                            SET TicketSequencialGeral = @SeqGeral,
                                TicketSequencialAnoAtual = @Serial,
                                TicketUltimoAno = YEAR(GETDATE())
                            WHERE ClienteId = @ClienteId", conn, transaction);

                        cmdCliente.Parameters.AddWithValue("@SeqGeral", novoSeqGeral);
                        cmdCliente.Parameters.AddWithValue("@Serial", serialAtual);
                        cmdCliente.Parameters.AddWithValue("@ClienteId", clienteIdLocal); // ← LOCAL

                        int linhasAfetadasCliente = await cmdCliente.ExecuteNonQueryAsync();

                        if (linhasAfetadasCliente == 0)
                        {
                            throw new Exception($"Cliente {clienteIdLocal} não foi encontrado para atualizar sequenciais!");
                        }

                        var cmdControle = new SqlCommand(@"
                            UPDATE ControleLogistico
                            SET Ticket = @Ticket
                            WHERE Id = @ControleId", conn, transaction);

                        cmdControle.Parameters.AddWithValue("@Ticket", novoTicket);
                        cmdControle.Parameters.AddWithValue("@ControleId", controleLogisticoIdLocal); // ← LOCAL

                        await cmdControle.ExecuteNonQueryAsync();

                        transaction.Commit();

                        System.Diagnostics.Debug.WriteLine(
                            $"✅ TICKET INCREMENTADO - Cliente: {clienteIdLocal}, " +
                            $"Ticket: {novoTicket}, " +
                            $"SeqGeral: {novoSeqGeral}, Serial: {serialAtual}, " +
                            $"Usuário: {Environment.UserName}, " +
                            $"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

                        dadosCliente.TicketSequencialGeral = novoSeqGeral;
                        dadosCliente.TicketSequencialAnoAtual = serialAtual;
                        txtTicket.Text = novoTicket;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private string GerarPDFEm(string caminhoCompleto, bool preview = false)
        {
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            document.Info.Title = preview
                ? $"Pré-visualização Ticket {txtTicket.Text}"
                : $"Ticket {txtTicket.Text}";

            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont fontTitulo = new XFont("LiberationSans", 16, XFontStyleEx.Bold);
            XFont fontSubtitulo = new XFont("LiberationSans", 13, XFontStyleEx.Bold);
            XFont fontNormal = new XFont("LiberationSans", 10, XFontStyleEx.Regular);
            XFont fontBold = new XFont("LiberationSans", 10, XFontStyleEx.Bold);
            XFont fontPequena = new XFont("LiberationSans", 8, XFontStyleEx.Regular);

            double margemEsq = 40;
            double margemTopo = 40;
            double larguraUtil = page.Width.Point - 2 * margemEsq;
            double y = margemTopo;

            gfx.DrawRectangle(new XPen(XColors.Black, 1),
                margemEsq, margemTopo, larguraUtil, page.Height.Point - 80);

            double altCabecalho = 80;
            XRect rectCabecalho = new XRect(margemEsq, y, larguraUtil, altCabecalho);
            gfx.DrawRectangle(new XPen(XColors.Black, 1), XBrushes.White, rectCabecalho);

            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                string resourceName = "Reverse.Resources.logo_dld.jpg";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        XImage logo = XImage.FromStream(stream);
                        double alturaLogo = 60;
                        double larguraLogo = alturaLogo * (logo.PixelWidth / (double)logo.PixelHeight);
                        gfx.DrawImage(logo, margemEsq + 10, y + 10, larguraLogo, alturaLogo);
                    }
                }
            }
            catch { }

            double larguraTicketBox = 180;
            XRect rectTicket = new XRect(margemEsq + larguraUtil - larguraTicketBox - 10, y + 15, larguraTicketBox, 55);
            gfx.DrawRectangle(new XPen(XColors.Black, 2), XBrushes.White, rectTicket);
            gfx.DrawString("TICKET Nº", new XFont("LiberationSans", 12, XFontStyleEx.Bold), XBrushes.Black,
                new XRect(rectTicket.X, rectTicket.Y + 8, rectTicket.Width, 14), XStringFormats.Center);
            gfx.DrawString(txtTicket.Text, new XFont("LiberationSans", 20, XFontStyleEx.Bold), XBrushes.Black,
                new XRect(rectTicket.X, rectTicket.Y + 28, rectTicket.Width, 22), XStringFormats.Center);

            y += altCabecalho + 15;

            double alturaBlocoMateriais = 150;
            XRect rectMateriais = new XRect(margemEsq, y, larguraUtil, alturaBlocoMateriais);
            gfx.DrawRectangle(new XPen(XColors.Black, 1), XBrushes.White, rectMateriais);

            bool mostrarMateriais = !(dadosControle.Servico.ToUpper().Contains("RETIRADA") ||
                                      dadosControle.Servico.ToUpper().Contains("TRANSFERÊNCIA") ||
                                      dadosControle.Servico.ToUpper().Contains("TRANSFERENCIA") ||
                                      dadosControle.Servico.ToUpper().Contains("ENTREGA"));

            if (mostrarMateriais)
            {
                gfx.DrawRectangle(new XPen(XColors.Black, 1), XBrushes.White,
                    margemEsq, y, larguraUtil, 25);
                gfx.DrawString("MATERIAIS TRANSPORTADOS", fontSubtitulo, XBrushes.Black,
                    margemEsq + 10, y + 17);

                string[] col1 = { "Papelão", "Papel", "Plástico", "Apara", "Inox", "Alumínio", "Placa" };
                string[] col2 = { "Metálico", "Cavaco", "Maquinário", "Sucata Eletrônica", "Madeira", "Vidro", "Cabo" };
                string[] col3 = { "Borracha", "Resíduo Industrial", "Tecido", "Bateria", "Isopor", "Misto", "Outros" };

                double colWidth = (larguraUtil - 30) / 3;
                double yMat = y + 40;

                for (int i = 0; i < col1.Length; i++)
                {
                    // Coluna 1
                    gfx.DrawString(col1[i] + ":", fontNormal, XBrushes.Black, margemEsq + 15, yMat);
                    gfx.DrawLine(new XPen(XColors.Black, 0.5),
                        margemEsq + 15 + 60, yMat - 2, margemEsq + colWidth - 10, yMat - 2);

                    // Coluna 2
                    gfx.DrawString(col2[i] + ":", fontNormal, XBrushes.Black, margemEsq + 15 + colWidth, yMat);
                    gfx.DrawLine(new XPen(XColors.Black, 0.5),
                        margemEsq + 15 + colWidth + 100, yMat - 2, margemEsq + 15 + 2 * colWidth - 10, yMat - 2);

                    // Coluna 3
                    if (col3[i] == "Outros")
                    {
                        // Apenas a linha, sem o texto
                        gfx.DrawLine(new XPen(XColors.Black, 0.5),
                            margemEsq + 15 + 2 * colWidth, yMat - 2,
                            margemEsq + larguraUtil - 15, yMat - 2);
                    }
                    else
                    {
                        gfx.DrawString(col3[i] + ":", fontNormal, XBrushes.Black, margemEsq + 15 + 2 * colWidth, yMat);
                        gfx.DrawLine(new XPen(XColors.Black, 0.5),
                            margemEsq + 15 + 2 * colWidth + 100, yMat - 2, margemEsq + larguraUtil - 15, yMat - 2);
                    }

                    yMat += 16;
                }
            }
            else
            {
                gfx.DrawRectangle(new XPen(XColors.Black, 1), XBrushes.White,
                    margemEsq, y, larguraUtil, 25);
                gfx.DrawString("OBSERVAÇÕES", fontSubtitulo, XBrushes.Black,
                    margemEsq + 10, y + 17);

                double yObs = y + 40;
                double limiteInferior = y + alturaBlocoMateriais - 10;

                while (yObs < limiteInferior)
                {
                    gfx.DrawLine(new XPen(XColors.Black, 0.5),
                        margemEsq + 15, yObs, margemEsq + larguraUtil - 15, yObs);
                    yObs += 18;
                }
            }

            bool clienteEhFornecedor =
                dadosControle.Servico.ToUpper().Contains("COLETA") ||
                dadosControle.Servico.ToUpper().Contains("RECEBIMENTO") ||
                dadosControle.Servico.ToUpper().Contains("DESCARTE");

            string tituloCliente = clienteEhFornecedor ? "DADOS DO FORNECEDOR" : "DADOS DO RECEPTOR";
            string tituloDLD = clienteEhFornecedor ? "DADOS DO RECEPTOR" : "DADOS DO FORNECEDOR";

            y += alturaBlocoMateriais + 15;

            double alturaCliente = 160;
            XRect rectCliente = new XRect(margemEsq, y, larguraUtil, alturaCliente);
            gfx.DrawRectangle(new XPen(XColors.Black, 1), XBrushes.White, rectCliente);
            gfx.DrawRectangle(new XPen(XColors.Black, 1), XBrushes.White,
                margemEsq, y, larguraUtil, 25);
            gfx.DrawString(tituloCliente, fontSubtitulo, XBrushes.Black, margemEsq + 10, y + 17);

            double yCliente = y + 40;

            // Razão Social
            gfx.DrawString("Razão Social: ", fontBold, XBrushes.Black, margemEsq + 15, yCliente);
            gfx.DrawString(dadosCliente.RazaoSocial, fontNormal, XBrushes.Black, margemEsq + 95, yCliente);
            yCliente += 18;

            // Nome Fantasia
            gfx.DrawString("Nome Fantasia: ", fontBold, XBrushes.Black, margemEsq + 15, yCliente);
            gfx.DrawString(dadosCliente.Nome, fontNormal, XBrushes.Black, margemEsq + 95, yCliente);
            yCliente += 18;

            // Complemento + CNPJ na mesma linha
            if (!string.IsNullOrWhiteSpace(dadosCliente.ComplementoEntrega) || !string.IsNullOrWhiteSpace(dadosCliente.CNPJ))
            {
                gfx.DrawString("Complemento: ", fontBold, XBrushes.Black, margemEsq + 15, yCliente);
                gfx.DrawString(dadosCliente.ComplementoEntrega, fontNormal, XBrushes.Black, margemEsq + 95, yCliente);

                if (!string.IsNullOrWhiteSpace(dadosCliente.CNPJ))
                {
                    string cnpjFormatado = FormatarCNPJ(dadosCliente.CNPJ);
                    gfx.DrawString("CNPJ: ", fontBold, XBrushes.Black, margemEsq + 350, yCliente);
                    gfx.DrawString(cnpjFormatado, fontNormal, XBrushes.Black, margemEsq + 390, yCliente);
                }

                yCliente += 18;
            }

            gfx.DrawString("Endereço: ", fontBold, XBrushes.Black, margemEsq + 15, yCliente);
            gfx.DrawString($"{txtRuaCliente.Text}, {txtNumeroCliente.Text} - {txtBairroCliente.Text}",
                fontNormal, XBrushes.Black, margemEsq + 95, yCliente);
            yCliente += 18;

            gfx.DrawString("Cidade: ", fontBold, XBrushes.Black, margemEsq + 15, yCliente);
            gfx.DrawString($"{txtMunicioCliente.Text} / {txtUFCliente.Text}", fontNormal, XBrushes.Black, margemEsq + 95, yCliente);
            gfx.DrawString($"Telefone: {txtTelCliente.Text}", fontNormal, XBrushes.Black, margemEsq + 350, yCliente);
            yCliente += 18;

            gfx.DrawString("Responsável: ", fontBold, XBrushes.Black, margemEsq + 15, yCliente);
            gfx.DrawString(txtResCliente.Text, fontNormal, XBrushes.Black, margemEsq + 95, yCliente);
            yCliente += 18;

            gfx.DrawString("Data: ", fontBold, XBrushes.Black, margemEsq + 15, yCliente);
            gfx.DrawString($"{dtpEntrega.Value:dd/MM/yyyy}", fontNormal, XBrushes.Black, margemEsq + 45, yCliente);

            y += alturaCliente + 15;

            // --- Bloco DLD (Receptor ou Fornecedor) ---
            double alturaDLD = 140;
            XRect rectDLD = new XRect(margemEsq, y, larguraUtil, alturaDLD);
            gfx.DrawRectangle(new XPen(XColors.Black, 1), XBrushes.White, rectDLD);
            gfx.DrawRectangle(new XPen(XColors.Black, 1), XBrushes.White,
                margemEsq, y, larguraUtil, 25);
            gfx.DrawString(tituloDLD, fontSubtitulo, XBrushes.Black, margemEsq + 10, y + 17);

            double yDLD = y + 40;
            gfx.DrawString("Empresa: ", fontBold, XBrushes.Black, margemEsq + 15, yDLD);
            gfx.DrawString("DLD SOLUÇÕES LOGÍSTICA REVERSA, GESTÃO E RECICLAGEM", fontNormal, XBrushes.Black, margemEsq + 90, yDLD);
            yDLD += 18;

            gfx.DrawString("Endereço: ", fontBold, XBrushes.Black, margemEsq + 15, yDLD);
            gfx.DrawString("RUA FREDERICO RUEGGER, 301 - JARDIM CANDIDA", fontNormal, XBrushes.Black, margemEsq + 90, yDLD);
            yDLD += 18;

            gfx.DrawString("Município: ", fontBold, XBrushes.Black, margemEsq + 15, yDLD);
            gfx.DrawString("ARARAS / SP", fontNormal, XBrushes.Black, margemEsq + 90, yDLD);
            gfx.DrawString("Telefone: 19 3351-5609", fontNormal, XBrushes.Black, margemEsq + 250, yDLD);
            yDLD += 18;

            gfx.DrawString("Motorista: ", fontBold, XBrushes.Black, margemEsq + 15, yDLD);
            gfx.DrawString(dadosControle.Motorista, fontNormal, XBrushes.Black, margemEsq + 90, yDLD);
            gfx.DrawString("Veículo: " + dadosControle.ModeloVeiculo, fontNormal, XBrushes.Black, margemEsq + 250, yDLD);
            gfx.DrawString("Placa: " + dadosVeiculo.Placa, fontNormal, XBrushes.Black, margemEsq + 400, yDLD);
            yDLD += 18;

            gfx.DrawString("Ajudante(s): ", fontBold, XBrushes.Black, margemEsq + 15, yDLD);
            string ajudantes = dadosControle.Ajudante1;
            if (!string.IsNullOrWhiteSpace(dadosControle.Ajudante2))
                ajudantes += " / " + dadosControle.Ajudante2;
            gfx.DrawString(ajudantes, fontNormal, XBrushes.Black, margemEsq + 90, yDLD);
            yDLD += 18;

            gfx.DrawString("Data: ", fontBold, XBrushes.Black, margemEsq + 15, yDLD);
            gfx.DrawString($"{dtpColeta.Value:dd/MM/yyyy}", fontNormal, XBrushes.Black, margemEsq + 45, yDLD);

            y += alturaDLD + 15;

            // --- Assinaturas ---
            double alturaAssinaturas = 70;
            double espacoEntreAssinaturas = 20;
            double larguraAssinatura = (larguraUtil - espacoEntreAssinaturas) / 2;

            double xAssCliente = margemEsq;
            XRect rectAssCliente = new XRect(xAssCliente, y, larguraAssinatura, alturaAssinaturas);
            gfx.DrawRectangle(new XPen(XColors.Black, 1), XBrushes.White, rectAssCliente);
            gfx.DrawLine(new XPen(XColors.Black, 0.5),
                xAssCliente + 20, y + 45, xAssCliente + larguraAssinatura - 20, y + 45);
            gfx.DrawString("Assinatura do Cliente", fontPequena, XBrushes.Black,
                new XRect(xAssCliente, y + 52, larguraAssinatura, 15), XStringFormats.Center);

            double xAssDLD = margemEsq + larguraAssinatura + espacoEntreAssinaturas;
            XRect rectAssDLD = new XRect(xAssDLD, y, larguraAssinatura, alturaAssinaturas);
            gfx.DrawRectangle(new XPen(XColors.Black, 1), XBrushes.White, rectAssDLD);
            gfx.DrawLine(new XPen(XColors.Black, 0.5),
                xAssDLD + 20, y + 45, xAssDLD + larguraAssinatura - 20, y + 45);
            gfx.DrawString("Assinatura DLD", fontPequena, XBrushes.Black,
                new XRect(xAssDLD, y + 52, larguraAssinatura, 15), XStringFormats.Center);

            y += alturaAssinaturas + 15;

            // --- Observações finais (até o rodapé) ---
            double alturaRestante = page.Height.Point - y - 60;
            XRect rectStatus = new XRect(margemEsq, y, larguraUtil, alturaRestante);
            gfx.DrawRectangle(new XPen(XColors.Black, 1), XBrushes.White, rectStatus);

            double yStatus = y + 25;
            gfx.DrawString("OBSERVAÇÕES:", fontSubtitulo, XBrushes.Black, margemEsq + 30, yStatus);

            yStatus += 25;
            while (yStatus < page.Height.Point - 60)
            {
                gfx.DrawLine(new XPen(XColors.Black, 0.5),
                    margemEsq + 30, yStatus, margemEsq + larguraUtil - 30, yStatus);
                yStatus += 22;
            }

            document.Save(caminhoCompleto);
            return caminhoCompleto;
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                string pastaTemp = Path.GetTempPath();
                string nomeArquivo = $"Ticket_{Guid.NewGuid()}.pdf";
                string caminhoCompleto = Path.Combine(pastaTemp, nomeArquivo);

                GerarPDFEm(caminhoCompleto, preview: false);

                System.Diagnostics.Process.Start(caminhoCompleto);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir para impressão: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GerarPDF()
        {
            string pastaDocumentos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pastaDLD = Path.Combine(pastaDocumentos, "DLD_Tickets");

            if (!Directory.Exists(pastaDLD))
                Directory.CreateDirectory(pastaDLD);

            string nomeArquivo = $"Ticket_{txtTicket.Text.Replace("/", "-").Replace("-", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string caminhoCompleto = Path.Combine(pastaDLD, nomeArquivo);

            return GerarPDFEm(caminhoCompleto, preview: false);
        }

        private string FormatarCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return cnpj;

            // Remove tudo que não for número
            string apenasNumeros = new string(cnpj.Where(char.IsDigit).ToArray());

            // Se tiver exatamente 14 dígitos, formata como CNPJ
            if (apenasNumeros.Length == 14)
            {
                return $"{apenasNumeros.Substring(0, 2)}.{apenasNumeros.Substring(2, 3)}.{apenasNumeros.Substring(5, 3)}/{apenasNumeros.Substring(8, 4)}-{apenasNumeros.Substring(12, 2)}";
            }
            // Se tiver exatamente 11 dígitos, formata como CPF
            else if (apenasNumeros.Length == 11)
            {
                return $"{apenasNumeros.Substring(0, 3)}.{apenasNumeros.Substring(3, 3)}.{apenasNumeros.Substring(6, 3)}-{apenasNumeros.Substring(9, 2)}";
            }

            // Caso contrário, retorna como está
            return cnpj;
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Classes auxiliares
        private class DadosControle
        {
            public string Gerador { get; set; }
            public DateTime Data { get; set; }
            public string Servico { get; set; }
            public string Motorista { get; set; }
            public string Ajudante1 { get; set; }
            public string Ajudante2 { get; set; }
            public string ModeloVeiculo { get; set; }
            public string Observacoes { get; set; }
        }
        private class DadosCliente
        {
            public string CodigoEmpresa { get; set; }
            public string Nome { get; set; }
            public string RazaoSocial { get; set; }
            public string RuaEntrega { get; set; }
            public string BairroEntrega { get; set; }
            public string NumeroEntrega { get; set; }
            public string ComplementoEntrega { get; set; }
            public string MunicipioEntrega { get; set; }
            public string EstadoEntrega { get; set; }
            public string Telefone { get; set; }
            public string CNPJ { get; set; }
            public string NomeContato { get; set; }
            public string Responsavel { get; set; }
            public int TicketSequencialGeral { get; set; }
            public int TicketSequencialAnoAtual { get; set; }
            public int TicketUltimoAno { get; set; }
        }

        private class DadosVeiculo
        {
            public string Placa { get; set; }
            public string Categoria { get; set; }
        }
    }
}