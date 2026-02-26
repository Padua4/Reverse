using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Reverse.Forms.FormsAtendimento
{
    public partial class AtendimentoFormProtocolo : Form
    {
        private int usuarioId;
        private int? chamadoId;
        private string connectionString;
        private bool isSuporte;
        private bool isNovoChamado;

        private Dictionary<string, string> anexosDisponiveis = new Dictionary<string, string>();

        #region Construtores

        public AtendimentoFormProtocolo(int _usuarioId, string _connectionString)
        {
            InitializeComponent();
            this.usuarioId = _usuarioId;
            this.connectionString = _connectionString;
            this.chamadoId = null;
            this.isNovoChamado = true;

            VerificarSeSuporte();
            ConfigurarFormularioNovo();
            ConfigurarEventosHistorico();
        }

        public AtendimentoFormProtocolo(int _usuarioId, string _connectionString, int _chamadoId)
        {
            InitializeComponent();
            this.usuarioId = _usuarioId;
            this.connectionString = _connectionString;
            this.chamadoId = _chamadoId;
            this.isNovoChamado = false;

            VerificarSeSuporte();
            ConfigurarFormularioExistente();
            ConfigurarEventosHistorico();
        }

        #endregion

        #region Verificação de Permissões

        private void VerificarSeSuporte()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"SELECT COUNT(*) 
                                  FROM Permissoes 
                                  WHERE UsuarioId = @UsuarioId 
                                  AND FormName = 'AtendimentoForm' 
                                  AND PodeAcessar = 1";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    isSuporte = count > 0;
                }
            }
            catch
            {
                isSuporte = false;
            }
        }

        #endregion

        #region Configuração do Formulário

        private void ConfigurarFormularioNovo()
        {
            lblChamado.Text = "Novo Chamado";
            lblStatus.Text = "Aberto";
            lblDataCriacao.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            lblAtribuido.Text = "Não atribuído";

            cmbPrioridade.Items.Clear();
            cmbPrioridade.Items.AddRange(new string[] { "Alta", "Media", "Baixa", "Sugestao" });
            cmbPrioridade.SelectedIndex = 1;
            cmbPrioridade.Enabled = true;

            txtAssunto.Enabled = true;
            txtMensagem.Enabled = true;
            btnEnviar.Enabled = true;
            btnAnexo.Enabled = false;
            btnAtualizar.Enabled = false;

            rtbHistorico.Clear();
            rtbHistorico.ReadOnly = true;

            this.Text = "Novo Chamado";
        }

        private void ConfigurarFormularioExistente()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"SELECT c.ChamadoId, c.UsuarioId, c.Assunto, c.Prioridade, c.Status, 
                                          c.DataCriacao, c.SuporteId,
                                          ISNULL(s.UsuarioNome, 'Não atribuído') AS SuporteNome
                                   FROM Chamados c
                                   LEFT JOIN Usuarios s ON c.SuporteId = s.Id
                                   WHERE c.ChamadoId = @ChamadoId";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ChamadoId", chamadoId.Value);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int criadorId = Convert.ToInt32(reader["UsuarioId"]);

                        if (!isSuporte && criadorId != usuarioId)
                        {
                            MessageBox.Show("Você não tem permissão para visualizar este chamado!", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            reader.Close();
                            this.Close();
                            return;
                        }

                        lblChamado.Text = $"Chamado #{reader["ChamadoId"]}";
                        lblStatus.Text = reader["Status"].ToString();
                        lblDataCriacao.Text = Convert.ToDateTime(reader["DataCriacao"]).ToString("dd/MM/yyyy HH:mm");
                        lblAtribuido.Text = reader["SuporteNome"].ToString();
                        txtAssunto.Text = reader["Assunto"].ToString();

                        cmbPrioridade.Items.Clear();
                        cmbPrioridade.Items.AddRange(new string[] { "Alta", "Media", "Baixa", "Sugestao" });
                        cmbPrioridade.SelectedItem = reader["Prioridade"].ToString();

                        string status = reader["Status"].ToString();

                        if (status == "Finalizado")
                        {
                            cmbPrioridade.Enabled = false;
                            txtAssunto.Enabled = false;
                            txtMensagem.Enabled = false;
                            btnEnviar.Enabled = false;
                            btnAnexo.Enabled = false;
                        }
                        else
                        {
                            txtAssunto.Enabled = false;
                            txtMensagem.Enabled = true;
                            btnEnviar.Enabled = true;
                            btnAnexo.Enabled = true;
                            cmbPrioridade.Enabled = isSuporte;
                        }

                        this.Text = $"Chamado #{reader["ChamadoId"]} - {reader["Assunto"]}";
                    }
                    else
                    {
                        MessageBox.Show("Chamado não encontrado!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        reader.Close();
                        this.Close();
                        return;
                    }

                    reader.Close();
                }

                btnAtualizar.Enabled = true;
                CarregarHistorico();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar chamado: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void ConfigurarEventosHistorico()
        {
            rtbHistorico.LinkClicked += RtbHistorico_LinkClicked;
            rtbHistorico.MouseClick += RtbHistorico_MouseClick;
        }

        #endregion

        #region Carregar Histórico

        private void CarregarHistorico()
        {
            if (!chamadoId.HasValue)
                return;

            try
            {
                rtbHistorico.Clear();
                anexosDisponiveis.Clear();

                List<ItemHistorico> itensHistorico = new List<ItemHistorico>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlMensagens = @"SELECT m.Mensagem, m.DataEnvio, m.TipoUsuario, u.UsuarioNome
                                           FROM ChamadoMensagens m
                                           INNER JOIN Usuarios u ON m.UsuarioId = u.Id
                                           WHERE m.ChamadoId = @ChamadoId";

                    SqlCommand cmdMensagens = new SqlCommand(sqlMensagens, conn);
                    cmdMensagens.Parameters.AddWithValue("@ChamadoId", chamadoId.Value);
                    SqlDataReader readerMensagens = cmdMensagens.ExecuteReader();

                    while (readerMensagens.Read())
                    {
                        itensHistorico.Add(new ItemHistorico
                        {
                            Tipo = "Mensagem",
                            Data = Convert.ToDateTime(readerMensagens["DataEnvio"]),
                            UsuarioNome = readerMensagens["UsuarioNome"].ToString(),
                            Conteudo = readerMensagens["Mensagem"].ToString(),
                            TipoUsuario = readerMensagens["TipoUsuario"].ToString()
                        });
                    }
                    readerMensagens.Close();

                    string sqlAnexos = @"SELECT a.NomeArquivo, a.DataUpload, a.CaminhoArquivo, u.UsuarioNome
                                        FROM ChamadoAnexos a
                                        INNER JOIN Usuarios u ON a.UsuarioId = u.Id
                                        WHERE a.ChamadoId = @ChamadoId";

                    SqlCommand cmdAnexos = new SqlCommand(sqlAnexos, conn);
                    cmdAnexos.Parameters.AddWithValue("@ChamadoId", chamadoId.Value);
                    SqlDataReader readerAnexos = cmdAnexos.ExecuteReader();

                    while (readerAnexos.Read())
                    {
                        itensHistorico.Add(new ItemHistorico
                        {
                            Tipo = "Anexo",
                            Data = Convert.ToDateTime(readerAnexos["DataUpload"]),
                            UsuarioNome = readerAnexos["UsuarioNome"].ToString(),
                            Conteudo = readerAnexos["NomeArquivo"].ToString(),
                            CaminhoArquivo = readerAnexos["CaminhoArquivo"].ToString()
                        });
                    }
                    readerAnexos.Close();
                }

                itensHistorico = itensHistorico.OrderBy(x => x.Data).ToList();

                foreach (var item in itensHistorico)
                {
                    if (item.Tipo == "Mensagem")
                    {
                        AdicionarMensagemHistorico(item.UsuarioNome, item.Conteudo, item.TipoUsuario, item.Data);
                    }
                    else if (item.Tipo == "Anexo")
                    {
                        AdicionarAnexoHistorico(item.UsuarioNome, item.Conteudo, item.Data, item.CaminhoArquivo);
                    }
                }

                rtbHistorico.SelectionStart = rtbHistorico.Text.Length;
                rtbHistorico.ScrollToCaret();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar histórico: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class ItemHistorico
        {
            public string Tipo { get; set; }
            public DateTime Data { get; set; }
            public string UsuarioNome { get; set; }
            public string Conteudo { get; set; }
            public string TipoUsuario { get; set; }
            public string CaminhoArquivo { get; set; }
        }

        private void AdicionarMensagemHistorico(string nomeUsuario, string mensagem, string tipoUsuario, DateTime dataEnvio)
        {
            if (rtbHistorico.Text.Length > 0)
                rtbHistorico.AppendText("\n\n");

            rtbHistorico.SelectionFont = new Font(rtbHistorico.Font, FontStyle.Regular);
            rtbHistorico.SelectionColor = Color.Gray;
            rtbHistorico.AppendText($"[{dataEnvio:dd/MM/yyyy HH:mm}]\n");

            rtbHistorico.SelectionFont = new Font(rtbHistorico.Font, FontStyle.Bold);
            rtbHistorico.SelectionColor = tipoUsuario == "Usuario" ? Color.Blue : Color.Green;
            rtbHistorico.AppendText($"{nomeUsuario}: ");

            rtbHistorico.SelectionFont = new Font(rtbHistorico.Font, FontStyle.Regular);
            rtbHistorico.SelectionColor = Color.Black;
            rtbHistorico.AppendText(mensagem);
        }

        private void AdicionarAnexoHistorico(string nomeUsuario, string nomeArquivo, DateTime dataUpload, string caminhoArquivo)
        {
            if (rtbHistorico.Text.Length > 0)
                rtbHistorico.AppendText("\n\n");

            rtbHistorico.SelectionFont = new Font(rtbHistorico.Font, FontStyle.Regular);
            rtbHistorico.SelectionColor = Color.Gray;
            rtbHistorico.AppendText($"[{dataUpload:dd/MM/yyyy HH:mm}]\n");

            rtbHistorico.SelectionFont = new Font(rtbHistorico.Font, FontStyle.Italic);
            rtbHistorico.SelectionColor = Color.DarkOrange;

            int posicaoInicial = rtbHistorico.TextLength;
            string textoAnexo = $"📎 {nomeUsuario} anexou: {nomeArquivo} [Clique aqui para abrir]";
            rtbHistorico.AppendText(textoAnexo);
            int posicaoFinal = rtbHistorico.TextLength;

            string chaveAnexo = $"{posicaoInicial}-{posicaoFinal}";
            anexosDisponiveis[chaveAnexo] = caminhoArquivo;

            rtbHistorico.Select(posicaoInicial, posicaoFinal - posicaoInicial);
            rtbHistorico.SelectionFont = new Font(rtbHistorico.Font, FontStyle.Italic | FontStyle.Underline);
            rtbHistorico.SelectionColor = Color.DarkOrange;

            rtbHistorico.Select(rtbHistorico.TextLength, 0);
        }

        private void RtbHistorico_MouseClick(object sender, MouseEventArgs e)
        {
            int posicaoClique = rtbHistorico.GetCharIndexFromPosition(e.Location);

            foreach (var anexo in anexosDisponiveis)
            {
                string[] range = anexo.Key.Split('-');
                int inicio = int.Parse(range[0]);
                int fim = int.Parse(range[1]);

                if (posicaoClique >= inicio && posicaoClique <= fim)
                {
                    AbrirAnexo(anexo.Value);
                    return;
                }
            }
        }

        private void RtbHistorico_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(e.LinkText);
            }
            catch { }
        }

        private void AbrirAnexo(string caminhoArquivo)
        {
            try
            {
                if (!File.Exists(caminhoArquivo))
                {
                    DialogResult result = MessageBox.Show(
                        "O arquivo não foi encontrado no caminho salvo.\n\nDeseja procurar o arquivo manualmente?",
                        "Arquivo não encontrado",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.Yes)
                    {
                        using (OpenFileDialog openDialog = new OpenFileDialog())
                        {
                            openDialog.Title = "Localizar arquivo anexo";
                            openDialog.Filter = "Todos os arquivos|*.*";

                            if (openDialog.ShowDialog() == DialogResult.OK)
                            {
                                Process.Start(openDialog.FileName);
                            }
                        }
                    }
                    return;
                }

                Process.Start(caminhoArquivo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir anexo: {ex.Message}\n\nCaminho: {caminhoArquivo}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Eventos de Botões

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if (isNovoChamado)
            {
                CriarNovoChamado();
            }
            else
            {
                EnviarMensagem();
            }
        }

        private void CriarNovoChamado()
        {
            if (string.IsNullOrWhiteSpace(txtAssunto.Text))
            {
                MessageBox.Show("Por favor, informe o assunto do chamado.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAssunto.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMensagem.Text))
            {
                MessageBox.Show("Por favor, informe a mensagem do chamado.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMensagem.Focus();
                return;
            }

            if (cmbPrioridade.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione a prioridade do chamado.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbPrioridade.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string sqlChamado = @"INSERT INTO Chamados (UsuarioId, Assunto, Prioridade, Status, StatusResposta, DataCriacao, DataUltimaAtualizacao)
                                             VALUES (@UsuarioId, @Assunto, @Prioridade, 'Aberto', 'Enviado', GETDATE(), GETDATE());
                                             SELECT SCOPE_IDENTITY();";

                        SqlCommand cmdChamado = new SqlCommand(sqlChamado, conn, transaction);
                        cmdChamado.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        cmdChamado.Parameters.AddWithValue("@Assunto", txtAssunto.Text.Trim());
                        cmdChamado.Parameters.AddWithValue("@Prioridade", cmbPrioridade.SelectedItem.ToString());

                        int novoChamadoId = Convert.ToInt32(cmdChamado.ExecuteScalar());

                        string sqlMensagem = @"INSERT INTO ChamadoMensagens (ChamadoId, UsuarioId, Mensagem, TipoUsuario, DataEnvio)
                                              VALUES (@ChamadoId, @UsuarioId, @Mensagem, 'Usuario', GETDATE());";

                        SqlCommand cmdMensagem = new SqlCommand(sqlMensagem, conn, transaction);
                        cmdMensagem.Parameters.AddWithValue("@ChamadoId", novoChamadoId);
                        cmdMensagem.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        cmdMensagem.Parameters.AddWithValue("@Mensagem", txtMensagem.Text.Trim());
                        cmdMensagem.ExecuteNonQuery();

                        transaction.Commit();

                        MessageBox.Show($"Chamado #{novoChamadoId} criado com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        chamadoId = novoChamadoId;
                        isNovoChamado = false;
                        ConfigurarFormularioExistente();
                        txtMensagem.Clear();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar chamado: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnviarMensagem()
        {
            if (string.IsNullOrWhiteSpace(txtMensagem.Text))
            {
                MessageBox.Show("Por favor, digite uma mensagem.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMensagem.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string tipoUsuario = isSuporte ? "Suporte" : "Usuario";

                        if (isSuporte)
                        {
                            string sqlAtribuir = @"UPDATE Chamados SET SuporteId = @SuporteId, DataUltimaAtualizacao = GETDATE()
                                                  WHERE ChamadoId = @ChamadoId AND (SuporteId IS NULL OR SuporteId != @SuporteId)";

                            SqlCommand cmdAtribuir = new SqlCommand(sqlAtribuir, conn, transaction);
                            cmdAtribuir.Parameters.AddWithValue("@ChamadoId", chamadoId.Value);
                            cmdAtribuir.Parameters.AddWithValue("@SuporteId", usuarioId);
                            cmdAtribuir.ExecuteNonQuery();
                        }

                        string sqlMensagem = @"INSERT INTO ChamadoMensagens (ChamadoId, UsuarioId, Mensagem, TipoUsuario, DataEnvio)
                                              VALUES (@ChamadoId, @UsuarioId, @Mensagem, @TipoUsuario, GETDATE());";

                        SqlCommand cmdMensagem = new SqlCommand(sqlMensagem, conn, transaction);
                        cmdMensagem.Parameters.AddWithValue("@ChamadoId", chamadoId.Value);
                        cmdMensagem.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        cmdMensagem.Parameters.AddWithValue("@Mensagem", txtMensagem.Text.Trim());
                        cmdMensagem.Parameters.AddWithValue("@TipoUsuario", tipoUsuario);
                        cmdMensagem.ExecuteNonQuery();

                        string statusResposta = tipoUsuario == "Usuario" ? "Enviado" : "Respondido";
                        string sqlUpdate = @"UPDATE Chamados SET StatusResposta = @StatusResposta, DataUltimaAtualizacao = GETDATE()
                                            WHERE ChamadoId = @ChamadoId";

                        SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@ChamadoId", chamadoId.Value);
                        cmdUpdate.Parameters.AddWithValue("@StatusResposta", statusResposta);
                        cmdUpdate.ExecuteNonQuery();

                        transaction.Commit();

                        MessageBox.Show("Mensagem enviada com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        txtMensagem.Clear();
                        CarregarHistorico();
                        ConfigurarFormularioExistente();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar mensagem: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            if (!chamadoId.HasValue)
                return;

            ConfigurarFormularioExistente();
            MessageBox.Show("Chamado atualizado!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAnexo_Click(object sender, EventArgs e)
        {
            if (!chamadoId.HasValue)
            {
                MessageBox.Show("Por favor, crie o chamado antes de anexar arquivos.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Selecionar Arquivo";
                openFileDialog.Filter = "Todos os arquivos|*.*|Imagens|*.png;*.jpg;*.jpeg;*.gif;*.bmp|PDFs|*.pdf|Documentos|*.doc;*.docx;*.txt";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = false;

                const long maxFileSize = 10 * 1024 * 1024;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(openFileDialog.FileName);

                        if (fileInfo.Length > maxFileSize)
                        {
                            MessageBox.Show("O arquivo é muito grande. Tamanho máximo: 10MB.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string pastaAnexos = Path.Combine(Application.StartupPath, "Anexos", "Chamados");
                        if (!Directory.Exists(pastaAnexos))
                            Directory.CreateDirectory(pastaAnexos);

                        string nomeUnico = $"{chamadoId}_{DateTime.Now:yyyyMMddHHmmss}_{fileInfo.Name}";
                        string caminhoDestino = Path.Combine(pastaAnexos, nomeUnico);

                        File.Copy(openFileDialog.FileName, caminhoDestino, true);

                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            string sql = @"INSERT INTO ChamadoAnexos (ChamadoId, UsuarioId, NomeArquivo, CaminhoArquivo, TamanhoArquivo, DataUpload)
                                          VALUES (@ChamadoId, @UsuarioId, @NomeArquivo, @CaminhoArquivo, @TamanhoArquivo, GETDATE());";

                            SqlCommand cmd = new SqlCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@ChamadoId", chamadoId.Value);
                            cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                            cmd.Parameters.AddWithValue("@NomeArquivo", fileInfo.Name);
                            cmd.Parameters.AddWithValue("@CaminhoArquivo", caminhoDestino);
                            cmd.Parameters.AddWithValue("@TamanhoArquivo", fileInfo.Length);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show($"Arquivo '{fileInfo.Name}' anexado com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        CarregarHistorico();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao anexar arquivo: {ex.Message}", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void cmbPrioridade_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isNovoChamado && chamadoId.HasValue && isSuporte && cmbPrioridade.Enabled)
            {
                if (cmbPrioridade.SelectedItem != null)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            string sql = @"UPDATE Chamados SET Prioridade = @Prioridade, DataUltimaAtualizacao = GETDATE()
                                          WHERE ChamadoId = @ChamadoId";

                            SqlCommand cmd = new SqlCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@ChamadoId", chamadoId.Value);
                            cmd.Parameters.AddWithValue("@Prioridade", cmbPrioridade.SelectedItem.ToString());

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch { }
                }
            }
        }

        #endregion

        private void AtendimentoFormProtocolo_Load(object sender, EventArgs e)
        {

        }

        private void AtendimentoFormProtocolo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isNovoChamado && !string.IsNullOrWhiteSpace(txtAssunto.Text))
            {
                DialogResult result = MessageBox.Show(
                    "Você tem um chamado não salvo. Deseja realmente sair?",
                    "Confirmação",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}