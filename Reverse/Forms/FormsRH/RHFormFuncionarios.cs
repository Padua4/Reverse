using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SeuProjeto
{
    public partial class FormFuncionarios : Form
    {
        private readonly string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=ReverseDB;Integrated Security=True;";
        private int funcionarioSelecionadoId = 0;
        private bool modoInsercao = true;

        public FormFuncionarios(int _usuarioId)
        {
            InitializeComponent();
            ConfigurarControles();
            CarregarLista();
            LimparCampos();
        }

        private void ConfigurarControles()
        {
            dtpDemissao.ShowCheckBox = true;
            dtpDemissao.Checked = false;

            cmbEmpresa.Items.Clear();
            cmbEmpresa.Items.AddRange(new[] { "DLD", "DLG" });

            // Status padrão
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new[] { "Ativo", "Inativo" });

            // DataGridView - seleção, leitura, autoajuste
            dgvLista.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLista.MultiSelect = false;
            dgvLista.ReadOnly = true;
            dgvLista.AutoGenerateColumns = true;

            // Cores e estilos fixos para evitar "texto invisível"
            dgvLista.EnableHeadersVisualStyles = false;
            dgvLista.DefaultCellStyle.ForeColor = Color.Black;
            dgvLista.DefaultCellStyle.BackColor = Color.White;
            dgvLista.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
            dgvLista.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
            dgvLista.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvLista.CellClick -= dgvLista_CellClick;
            dgvLista.CellClick += dgvLista_CellClick;

            dgvLista.DataBindingComplete += (s, e) =>
            {
                if (dgvLista.Columns.Contains("FuncionarioID"))
                    dgvLista.Columns["FuncionarioID"].Visible = false;
            };

            // Evita clique duplo no botão cadastrar
            btnCadastrar.Click -= btnCadastrar_Click;
            btnCadastrar.Click += btnCadastrar_Click;

            btnCancelar.Click += (s, e) => LimparCampos();
            btnAbrirDocs.Click += btnAbrirDocs_Click;

            // Eventos para atualizar tempo de empresa
            dtpAdmissao.ValueChanged += (s, e) => AtualizarTempoEmpresaLabel();
            dtpDemissao.ValueChanged += (s, e) => AtualizarTempoEmpresaLabel();

            // Botão "Novo" (se existir no painel superior)
            var btnNovoFuncionario = pnlHeader.Controls["btnNovoFuncionario"] as Button;
            if (btnNovoFuncionario != null)
                btnNovoFuncionario.Click += (s, e) => LimparCampos();
        }

        private void CarregarLista()
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"
        SELECT
            FuncionarioID,
            Nome,
            Empresa,
            Funcao,
            Status
        FROM Funcionarios
        WHERE Status = @Status
        ORDER BY Nome", con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Status", "Ativo");

                var dt = new DataTable();
                da.Fill(dt);
                dgvLista.DataSource = dt;
                dgvLista.ClearSelection();
            }
        }


        private void dgvLista_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var valor = dgvLista.Rows[e.RowIndex].Cells["FuncionarioID"].Value;

            if (valor == null || valor == DBNull.Value || Convert.ToInt32(valor) == 0)
            {
                funcionarioSelecionadoId = 0;
                modoInsercao = true;
                LimparCampos();
                return;
            }

            funcionarioSelecionadoId = Convert.ToInt32(valor);
            modoInsercao = false;
            CarregarFuncionario(funcionarioSelecionadoId);
        }



        private void CarregarFuncionario(int id)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM Funcionarios WHERE FuncionarioID = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return;

                    txtNome.Text = dr["Nome"].ToString();
                    dtpNascimento.Value = ToDate(dr["DataNascimento"], DateTime.Today);
                    txtRG.Text = dr["RG"].ToString();
                    txtCPF.Text = dr["CPF"].ToString();
                    txtEmail.Text = dr["Email"].ToString();

                    txtCNH.Text = dr["CNH"].ToString();
                    dtpVencCNH.Value = ToDate(dr["VencimentoCNH"], DateTime.Today);
                    dtpVencToxicologico.Value = ToDate(dr["VencimentoExameTox"], DateTime.Today);
                    dtpVencCursos.Value = ToDate(dr["VencimentoCursos"], DateTime.Today);
                    txtASO.Text = dr["ASO"].ToString();

                    txtEnderecoRua.Text = dr["EnderecoRua"].ToString();
                    txtEnderecoBairro.Text = dr["EnderecoBairro"].ToString();
                    txtEnderecoNumero.Text = dr["EnderecoNumero"].ToString();
                    txtEnderecoCEP.Text = dr["EnderecoCEP"].ToString();

                    dtpExp1.Value = ToDate(dr["DataPrimeiraExperiencia"], DateTime.Today);
                    dtpExp2.Value = ToDate(dr["DataSegundaExperiencia"], DateTime.Today);

                    dtpFeriasInicio.Value = ToDate(dr["PeriodoFerias"], DateTime.Today);
                    dtpUltimasFerias.Value = ToDate(dr["UltimasFerias"], DateTime.Today);
                    txtPeriodoAquisitivoFerias.Text = dr["PeriodoAquisitivoFerias"]?.ToString() ?? string.Empty;
                    txtObsFerias.Text = dr["ObservacaoFerias"]?.ToString() ?? string.Empty;

                    txtFuncao.Text = dr["Funcao"]?.ToString() ?? string.Empty;
                    txtCBO.Text = dr["CBO"]?.ToString() ?? string.Empty;
                    txtPIS.Text = dr["Pis"]?.ToString() ?? string.Empty;
                    numSalario.Value = ToDecimal(dr["Salario"]);
                    dtpAdmissao.Value = ToDate(dr["DataAdmissao"], DateTime.Today);

                    if (dr["DataDemissao"] == DBNull.Value)
                    {
                        dtpDemissao.Checked = false;
                        dtpDemissao.Value = DateTime.Today;
                    }
                    else
                    {
                        dtpDemissao.Checked = true;
                        dtpDemissao.Value = Convert.ToDateTime(dr["DataDemissao"]);
                    }

                    cmbStatus.Text = dr["Status"]?.ToString() ?? string.Empty;

                    txtAgencia.Text = dr["AgenciaBancaria"]?.ToString() ?? string.Empty;
                    txtConta.Text = dr["ContaCorrente"]?.ToString() ?? string.Empty;
                    txtPix.Text = dr["ChavePix"]?.ToString() ?? string.Empty;

                    txtLinksDocumentos.Text = dr["Documentos"]?.ToString() ?? string.Empty;
                    cmbEmpresa.Text = dr["Empresa"]?.ToString() ?? string.Empty;
                    rtbObservacoes.Text = dr["ObservacoesGerais"]?.ToString() ?? string.Empty;

                    AtualizarTempoEmpresaLabel();
                }
            }
        }

        private static DateTime ToDate(object dbValue, DateTime fallback)
            => dbValue == DBNull.Value ? fallback : Convert.ToDateTime(dbValue);

        private static decimal ToDecimal(object dbValue)
            => dbValue == DBNull.Value ? 0m : Convert.ToDecimal(dbValue);
        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario(out string mensagemErro))
            {
                MessageBox.Show(mensagemErro, "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCPFUnico(out string erroCPF))
            {
                MessageBox.Show(erroCPF, "CPF Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Se Status = Inativo, exigir motivo
            string motivo = null;
            if (string.Equals(cmbStatus.Text, "Inativo", StringComparison.OrdinalIgnoreCase))
            {
                motivo = PromptMotivoInativacao();
                if (string.IsNullOrWhiteSpace(motivo))
                {
                    MessageBox.Show("Informe o motivo da inativação/demissão.", "Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            if (modoInsercao || funcionarioSelecionadoId == 0)
                                InserirFuncionario(con, transaction, motivo);
                            else
                                AtualizarFuncionario(con, transaction, motivo);

                            transaction.Commit();
                            MessageBox.Show("Funcionário salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                CarregarLista();
                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar funcionário: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private bool ValidarFormulario(out string erro)
        {
            // Regras mínimas (ajuste conforme sua política)
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                erro = "Nome é obrigatório.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCPF.Text))
            {
                erro = "CPF é obrigatório.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(cmbStatus.Text))
            {
                erro = "Selecione o Status (Ativo/Inativo).";
                return false;
            }
            if (cmbStatus.Text == "Inativo" && !dtpDemissao.Checked)
            {
                erro = "Para inativar, marque a Data de demissão.";
                return false;
            }
            erro = null;
            return true;
        }

        private bool ValidarCPFUnico(out string erro)
        {
            erro = null;

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Funcionarios WHERE CPF = @cpf AND (@id = 0 OR FuncionarioID != @id)", con))
            {
                cmd.Parameters.AddWithValue("@cpf", txtCPF.Text.Trim());
                cmd.Parameters.AddWithValue("@id", funcionarioSelecionadoId);

                con.Open();
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    erro = "Já existe um funcionário cadastrado com este CPF.";
                    return false;
                }
            }

            return true;
        }


        private void InserirFuncionario(SqlConnection con, SqlTransaction transaction, string motivoInativacao)
        {
            using (var cmd = new SqlCommand(@"
        INSERT INTO Funcionarios (
            Nome, DataNascimento, RG, CPF, Funcao, DataAdmissao, DataDemissao,
            CBO, Pis, ASO, Salario, Email, DataPrimeiraExperiencia, DataSegundaExperiencia,
            Documentos, AgenciaBancaria, ContaCorrente, ChavePix, ObservacoesGerais,
            PeriodoFerias, UltimasFerias, PeriodoAquisitivoFerias, ObservacaoFerias,
            Status, EnderecoRua, EnderecoBairro, EnderecoNumero, EnderecoCEP, CNH,
            VencimentoCNH, VencimentoExameTox, VencimentoCursos, MotivoInativacaoTemp, Empresa
        )
        VALUES (
            @Nome, @DataNascimento, @RG, @CPF, @Funcao, @DataAdmissao, @DataDemissao,
            @CBO, @Pis, @ASO, @Salario, @Email, @DataPrimeiraExperiencia, @DataSegundaExperiencia,
            @Documentos, @AgenciaBancaria, @ContaCorrente, @ChavePix, @ObservacoesGerais,
            @PeriodoFerias, @UltimasFerias, @PeriodoAquisitivoFerias, @ObservacaoFerias,
            @Status, @EnderecoRua, @EnderecoBairro, @EnderecoNumero, @EnderecoCEP, @CNH,
            @VencimentoCNH, @VencimentoExameTox, @VencimentoCursos, @MotivoInativacaoTemp, @Empresa
        )", con, transaction))
            {
                PreencherParametros(cmd, motivoInativacao);
                cmd.ExecuteNonQuery();
            }
        }

        private void AtualizarFuncionario(SqlConnection con, SqlTransaction transaction, string motivoInativacao)
        {
            using (var cmd = new SqlCommand(@"
        UPDATE Funcionarios SET
            Nome=@Nome, DataNascimento=@DataNascimento, RG=@RG, CPF=@CPF,
            Funcao=@Funcao, DataAdmissao=@DataAdmissao, DataDemissao=@DataDemissao,
            CBO=@CBO, Pis=@Pis, ASO=@ASO, Salario=@Salario, Email=@Email,
            DataPrimeiraExperiencia=@DataPrimeiraExperiencia, DataSegundaExperiencia=@DataSegundaExperiencia,
            Documentos=@Documentos, AgenciaBancaria=@AgenciaBancaria, ContaCorrente=@ContaCorrente,
            ChavePix=@ChavePix, ObservacoesGerais=@ObservacoesGerais, PeriodoFerias=@PeriodoFerias,
            UltimasFerias=@UltimasFerias, PeriodoAquisitivoFerias=@PeriodoAquisitivoFerias,
            ObservacaoFerias=@ObservacaoFerias, Status=@Status, EnderecoRua=@EnderecoRua,
            EnderecoBairro=@EnderecoBairro, EnderecoNumero=@EnderecoNumero, EnderecoCEP=@EnderecoCEP,
            CNH=@CNH, VencimentoCNH=@VencimentoCNH, VencimentoExameTox=@VencimentoExameTox,
            VencimentoCursos=@VencimentoCursos, MotivoInativacaoTemp=@MotivoInativacaoTemp, Empresa=@Empresa
        WHERE FuncionarioID=@id", con, transaction))
            {
                PreencherParametros(cmd, motivoInativacao);
                cmd.Parameters.AddWithValue("@id", funcionarioSelecionadoId);
                cmd.ExecuteNonQuery();
            }
        }
        private void PreencherParametros(SqlCommand cmd, string motivoInativacao)
        {
            // Datas opcionais
            object dataDemissao = dtpDemissao.Checked ? dtpDemissao.Value : (object)DBNull.Value;

            // Tempo de empresa
            string tempoEmpresa = CalcularTempoEmpresa();

            cmd.Parameters.AddWithValue("@Nome", txtNome.Text.Trim());
            cmd.Parameters.AddWithValue("@DataNascimento", dtpNascimento.Value);
            cmd.Parameters.AddWithValue("@RG", txtRG.Text.Trim());
            cmd.Parameters.AddWithValue("@CPF", txtCPF.Text.Trim());
            cmd.Parameters.AddWithValue("@Funcao", txtFuncao.Text.Trim());
            cmd.Parameters.AddWithValue("@DataAdmissao", dtpAdmissao.Value);
            cmd.Parameters.AddWithValue("@DataDemissao", dataDemissao);

            cmd.Parameters.AddWithValue("@CBO", txtCBO.Text.Trim());
            cmd.Parameters.AddWithValue("@Pis", txtPIS.Text.Trim());
            cmd.Parameters.AddWithValue("@ASO", txtASO.Text.Trim());
            cmd.Parameters.AddWithValue("@Salario", numSalario.Value);
            cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

            cmd.Parameters.AddWithValue("@DataPrimeiraExperiencia", dtpExp1.Value);
            cmd.Parameters.AddWithValue("@DataSegundaExperiencia", dtpExp2.Value);

            cmd.Parameters.AddWithValue("@Documentos", txtLinksDocumentos.Text.Trim());
            cmd.Parameters.AddWithValue("@AgenciaBancaria", txtAgencia.Text.Trim());
            cmd.Parameters.AddWithValue("@ContaCorrente", txtConta.Text.Trim());
            cmd.Parameters.AddWithValue("@ChavePix", txtPix.Text.Trim());
            cmd.Parameters.AddWithValue("@ObservacoesGerais", rtbObservacoes.Text);

            cmd.Parameters.AddWithValue("@PeriodoFerias", dtpFeriasInicio.Value);
            cmd.Parameters.AddWithValue("@UltimasFerias", dtpUltimasFerias.Value);
            cmd.Parameters.AddWithValue("@PeriodoAquisitivoFerias", txtPeriodoAquisitivoFerias.Text.Trim());
            cmd.Parameters.AddWithValue("@ObservacaoFerias", txtObsFerias.Text.Trim());

            cmd.Parameters.AddWithValue("@Status", cmbStatus.Text);

            cmd.Parameters.AddWithValue("@EnderecoRua", txtEnderecoRua.Text.Trim());
            cmd.Parameters.AddWithValue("@EnderecoBairro", txtEnderecoBairro.Text.Trim());
            cmd.Parameters.AddWithValue("@EnderecoNumero", txtEnderecoNumero.Text.Trim());
            cmd.Parameters.AddWithValue("@EnderecoCEP", txtEnderecoCEP.Text.Trim());

            cmd.Parameters.AddWithValue("@CNH", txtCNH.Text.Trim());
            cmd.Parameters.AddWithValue("@VencimentoCNH", dtpVencCNH.Value);
            cmd.Parameters.AddWithValue("@VencimentoExameTox", dtpVencToxicologico.Value);
            cmd.Parameters.AddWithValue("@VencimentoCursos", dtpVencCursos.Value);
            cmd.Parameters.AddWithValue("@Empresa", string.IsNullOrWhiteSpace(cmbEmpresa.Text) ? (object)DBNull.Value : cmbEmpresa.Text);

            if (string.Equals(cmbStatus.Text, "Inativo", StringComparison.OrdinalIgnoreCase))
                cmd.Parameters.AddWithValue("@MotivoInativacaoTemp", (object)motivoInativacao ?? DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@MotivoInativacaoTemp", DBNull.Value);
        }
        private void LimparCampos()
        {
            // Dados pessoais
            txtNome.Clear();
            dtpNascimento.Value = DateTime.Today;
            txtRG.Clear();
            txtCPF.Clear();
            txtEmail.Clear();

            // Documentos e vencimentos
            txtCNH.Clear();
            dtpVencCNH.Value = DateTime.Today;
            dtpVencToxicologico.Value = DateTime.Today;
            dtpVencCursos.Value = DateTime.Today;
            txtASO.Clear();

            // Endereço
            txtEnderecoRua.Clear();
            txtEnderecoBairro.Clear();
            txtEnderecoNumero.Clear();
            txtEnderecoCEP.Clear();

            // Experiências
            dtpExp1.Value = DateTime.Today;
            dtpExp2.Value = DateTime.Today;

            // Férias
            dtpFeriasInicio.Value = DateTime.Today;
            dtpUltimasFerias.Value = DateTime.Today;
            txtPeriodoAquisitivoFerias.Clear();
            txtObsFerias.Clear();

            // Profissional
            txtFuncao.Clear();
            txtCBO.Clear();
            txtPIS.Clear();
            numSalario.Value = 0;
            dtpAdmissao.Value = DateTime.Today;
            dtpDemissao.Checked = false;
            cmbStatus.SelectedIndex = cmbStatus.Items.IndexOf("Ativo"); // padrão
            lblTempoEmpresa.Text = "-";

            // Bancário / Pix
            txtAgencia.Clear();
            txtConta.Clear();
            txtPix.Clear();

            // Documentos / Observações
            txtLinksDocumentos.Clear();
            rtbObservacoes.Clear();

            funcionarioSelecionadoId = 0;
            modoInsercao = true;
            txtNome.Focus();
        }

        private void AtualizarTempoEmpresaLabel()
        {
            lblTempoEmpresa.Text = CalcularTempoEmpresa() ?? "-";
        }

        private string CalcularTempoEmpresa()
        {
            DateTime inicio = dtpAdmissao.Value.Date;
            DateTime fim = dtpDemissao.Checked ? dtpDemissao.Value.Date : DateTime.Today;

            if (fim < inicio) return null;

            int anos = fim.Year - inicio.Year;
            int meses = fim.Month - inicio.Month;
            int dias = fim.Day - inicio.Day;

            if (dias < 0)
            {
                meses--;
                var mesAnterior = fim.AddMonths(-1);
                dias += DateTime.DaysInMonth(mesAnterior.Year, mesAnterior.Month);
            }
            if (meses < 0)
            {
                anos--;
                meses += 12;
            }

            return $"{anos} ano(s), {meses} mes(es), {dias} dia(s)";
        }

        private string PromptMotivoInativacao()
        {
            using (var dlg = new Form())
            using (var txt = new TextBox())
            using (var ok = new Button())
            using (var cancelar = new Button())
            using (var lbl = new Label())
            {
                dlg.Text = "Motivo da Inativação";
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.MinimizeBox = false;
                dlg.MaximizeBox = false;
                dlg.Width = 520;
                dlg.Height = 220;
                dlg.AcceptButton = ok;
                dlg.CancelButton = cancelar;

                lbl.Text = "Descreva o motivo:";
                lbl.AutoSize = true;
                lbl.Top = 15;
                lbl.Left = 15;

                txt.Multiline = true;
                txt.Left = 15;
                txt.Top = 40;
                txt.Width = 480;
                txt.Height = 100;

                ok.Text = "OK";
                ok.Left = dlg.Width - 200;
                ok.Top = 150;
                ok.DialogResult = DialogResult.OK;

                cancelar.Text = "Cancelar";
                cancelar.Left = dlg.Width - 110;
                cancelar.Top = 150;
                cancelar.DialogResult = DialogResult.Cancel;

                dlg.Controls.Add(lbl);
                dlg.Controls.Add(txt);
                dlg.Controls.Add(ok);
                dlg.Controls.Add(cancelar);

                return dlg.ShowDialog(this) == DialogResult.OK ? txt.Text.Trim() : null;
            }
        }

        private void btnAbrirDocs_Click(object sender, EventArgs e)
        {
            var texto = txtLinksDocumentos.Text?.Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show("Nenhum link informado.");
                return;
            }

            // Suporta múltiplos links separados por espaço, vírgula, ponto e vírgula ou quebra de linha
            var separadores = new[] { ' ', ';', ',', '\n', '\r', '\t' };
            var links = texto.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

            foreach (var link in links)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = link,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Não foi possível abrir: {link}\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
