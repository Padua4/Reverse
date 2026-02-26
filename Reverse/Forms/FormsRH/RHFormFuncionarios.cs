using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Reverse
{
    public partial class RHFormFuncionarios : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private bool editando = false;
        private int funcionarioIDSelecionado = 0;

        public RHFormFuncionarios(int _usuarioId)
        {
            InitializeComponent();
            InicializarFormulario();
        }

        private void RHFormFuncionarios_Load(object sender, EventArgs e)
        {
            BloquearCampos();
            ConfigurarDataGridView();
            CarregarFuncionarios();
            cmbDLDDLG.Items.AddRange(new string[] { "DLD", "DLG" });
        }

        private void InicializarFormulario()
        {
            dtpDemissao.Enabled = false;
            txtCNH.Enabled = false;
            dtpVencimentoCNH.Enabled = false;
            dtpVencimentoToxicologico.Enabled = false;
        }

        private void ConfigurarDataGridView()
        {
            dgvFuncionarios.AutoGenerateColumns = false;
            dgvFuncionarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFuncionarios.MultiSelect = false;
            dgvFuncionarios.AllowUserToAddRows = false;
            dgvFuncionarios.ReadOnly = true;

            dgvFuncionarios.RowHeadersVisible = false;
            dgvFuncionarios.BorderStyle = BorderStyle.FixedSingle;
            dgvFuncionarios.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvFuncionarios.EnableHeadersVisualStyles = false;
            dgvFuncionarios.AllowUserToResizeRows = false;
            dgvFuncionarios.EditMode = DataGridViewEditMode.EditProgrammatically;

            dgvFuncionarios.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvFuncionarios.DefaultCellStyle.ForeColor = Color.Black;
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvFuncionarios.BackgroundColor = Color.FromArgb(250, 250, 252);
            dgvFuncionarios.GridColor = Color.FromArgb(230, 230, 235);

            dgvFuncionarios.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvFuncionarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvFuncionarios.ColumnHeadersHeight = 40;
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 3, 0, 3);

            dgvFuncionarios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            dgvFuncionarios.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgvFuncionarios.RowsDefaultCellStyle.BackColor = Color.White;
            dgvFuncionarios.RowsDefaultCellStyle.ForeColor = Color.Black;

            dgvFuncionarios.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            dgvFuncionarios.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgvFuncionarios.ColumnHeadersDefaultCellStyle.BackColor;
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgvFuncionarios.DefaultCellStyle.Padding = new Padding(3, 5, 3, 5);

            dgvFuncionarios.RowTemplate.Height = 35;
            dgvFuncionarios.RowTemplate.MinimumHeight = 34;

            dgvFuncionarios.Columns.Clear();
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "FuncionarioID", DataPropertyName = "FuncionarioID", Visible = false });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome", DataPropertyName = "Nome", HeaderText = "Nome", Width = 200 });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "RG", DataPropertyName = "RG", HeaderText = "RG", Width = 100 });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "CPF", DataPropertyName = "CPF", HeaderText = "CPF", Width = 120 });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Funcao", DataPropertyName = "Funcao", HeaderText = "Função", Width = 150 });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "TempoEmpresa", DataPropertyName = "TempoEmpresa", HeaderText = "Tempo de Empresa", Width = 130 });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "CBO", DataPropertyName = "CBO", HeaderText = "CBO", Width = 100 });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Salario", DataPropertyName = "Salario", HeaderText = "Salário", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefone", DataPropertyName = "Telefone", HeaderText = "Telefone", Width = 120 });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", DataPropertyName = "Email", HeaderText = "Email", Width = 200 });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "ChavePix", DataPropertyName = "ChavePix", HeaderText = "Chave Pix", Width = 200 });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ativo", DataPropertyName = "Ativo", Visible = false });
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn { Name = "MotivoDesligamento", DataPropertyName = "MotivoDesligamento", Visible = false });

            dgvFuncionarios.CellFormatting += DgvFuncionarios_CellFormatting;

            dgvFuncionarios.CellMouseEnter += DgvFuncionarios_CellMouseEnter;
            dgvFuncionarios.CellMouseLeave += DgvFuncionarios_CellMouseLeave;
            dgvFuncionarios.SelectionChanged += DgvFuncionarios_SelectionChanged;

            dgvFuncionarios.CellClick += dgvFuncionarios_CellClick;
        }

        private void DgvFuncionarios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvFuncionarios.Rows.Count) return;

            DataGridViewRow row = dgvFuncionarios.Rows[e.RowIndex];

            if (row.Cells["Ativo"] != null && row.Cells["Ativo"].Value != null && row.Cells["Ativo"].Value != DBNull.Value)
            {
                bool ativo = Convert.ToBoolean(row.Cells["Ativo"].Value);

                if (!ativo)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(100, 0, 0);
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 150, 150);
                    row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(100, 0, 0);
                }
                else
                {
                    if (e.RowIndex % 2 == 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
                    }
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
                    row.DefaultCellStyle.SelectionForeColor = Color.Black;
                }
            }
        }

        private void DgvFuncionarios_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvFuncionarios.Rows.Count)
            {
                DataGridViewRow row = dgvFuncionarios.Rows[e.RowIndex];

                // Verificar se a coluna "Ativo" existe
                if (row.Cells["Ativo"] != null && row.Cells["Ativo"].Value != null &&
                    row.Cells["Ativo"].Value != DBNull.Value)
                {
                    bool ativo = Convert.ToBoolean(row.Cells["Ativo"].Value);

                    // Aplicar hover apenas para funcionários ativos
                    if (ativo)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                    }
                }
            }
        }

        private void DgvFuncionarios_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvFuncionarios.Rows.Count)
            {

                dgvFuncionarios.InvalidateRow(e.RowIndex);
            }
        }

        private void DgvFuncionarios_SelectionChanged(object sender, EventArgs e)
        {
            dgvFuncionarios.Invalidate();

            if (dgvFuncionarios.SelectedRows.Count > 0 && !editando)
            {
                DataGridViewRow row = dgvFuncionarios.SelectedRows[0];
                if (row.Cells["FuncionarioID"].Value != null)
                {
                    int funcionarioID = Convert.ToInt32(row.Cells["FuncionarioID"].Value);
                    CarregarDadosFuncionario(funcionarioID);
                }
            }
        }

        private void CarregarFuncionarios()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT 
                            FuncionarioID, 
                            Nome, 
                            RG, 
                            CPF, 
                            Funcao, 
                            CASE 
                                WHEN DATEDIFF(DAY, DataAdmissao, ISNULL(DataDemissao, GETDATE())) < 30 THEN 
                                    CASE 
                                        WHEN DATEDIFF(DAY, DataAdmissao, ISNULL(DataDemissao, GETDATE())) = 1 THEN '1 dia'
                                        ELSE CAST(DATEDIFF(DAY, DataAdmissao, ISNULL(DataDemissao, GETDATE())) AS VARCHAR) + ' dias'
                                    END
                                WHEN DATEDIFF(DAY, DataAdmissao, ISNULL(DataDemissao, GETDATE())) < 365 THEN 
                                    CASE 
                                        WHEN DATEDIFF(MONTH, DataAdmissao, ISNULL(DataDemissao, GETDATE())) = 1 THEN '1 mês'
                                        ELSE CAST(DATEDIFF(MONTH, DataAdmissao, ISNULL(DataDemissao, GETDATE())) AS VARCHAR) + ' meses'
                                    END
                                ELSE 
                                    CASE 
                                        WHEN DATEDIFF(YEAR, DataAdmissao, ISNULL(DataDemissao, GETDATE())) = 1 THEN '1 ano'
                                        ELSE CAST(DATEDIFF(YEAR, DataAdmissao, ISNULL(DataDemissao, GETDATE())) AS VARCHAR) + ' anos'
                                    END
                            END AS TempoEmpresa,
                            CBO, 
                            Salario, 
                            Telefone, 
                            Email, 
                            ChavePix, 
                            Ativo, 
                            MotivoDesligamento
                            FROM RHFuncionarios 
                            ORDER BY Ativo DESC, Nome";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvFuncionarios.ClearSelection();
                    dgvFuncionarios.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar funcionários: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            LimparCampos();
            DesbloquearCampos();
            editando = false;
            funcionarioIDSelecionado = 0;
            txtNome.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvFuncionarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um funcionário para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = dgvFuncionarios.SelectedRows[0];
            bool ativo = Convert.ToBoolean(row.Cells["Ativo"].Value);

            if (!ativo)
            {
                MessageBox.Show("Não é possível editar um funcionário demitido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            funcionarioIDSelecionado = Convert.ToInt32(row.Cells["FuncionarioID"].Value);
            CarregarDadosFuncionario(funcionarioIDSelecionado);
            DesbloquearCampos();
            editando = true;

            foreach (Control control in this.Controls)
            {
                if (control is TextBox)
                {
                    control.BackColor = Color.White;
                }
                else if (control is DateTimePicker)
                {
                    control.BackColor = Color.White;
                    control.Enabled = true;
                }
            }
        }

        private void CarregarDadosFuncionario(int funcionarioID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM RHFuncionarios WHERE FuncionarioID = @FuncionarioID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FuncionarioID", funcionarioID);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtNome.Text = reader["Nome"].ToString();
                        dtpNascimento.Value = Convert.ToDateTime(reader["DataNascimento"]);
                        txtEmail.Text = reader["Email"].ToString();
                        txtOBS.Text = reader["ObservacaoGeral"].ToString();
                        txtRG.Text = reader["RG"].ToString();
                        txtCPF.Text = reader["CPF"].ToString();
                        txtPIS.Text = reader["PIS"].ToString();

                        bool possuiCNH = Convert.ToBoolean(reader["PossuiCNH"]);
                        rbCNHSim.Checked = possuiCNH;
                        rbCNHNao.Checked = !possuiCNH;

                        if (possuiCNH)
                        {
                            txtCNH.Text = reader["CNH"].ToString();
                            if (reader["VencimentoCNH"] != DBNull.Value)
                                dtpVencimentoCNH.Value = Convert.ToDateTime(reader["VencimentoCNH"]);
                            if (reader["VencimentoToxicologico"] != DBNull.Value)
                                dtpVencimentoToxicologico.Value = Convert.ToDateTime(reader["VencimentoToxicologico"]);
                        }

                        txtRua.Text = reader["Rua"].ToString();
                        txtNumero.Text = reader["Numero"].ToString();
                        txtBairro.Text = reader["Bairro"].ToString();
                        txtCidade.Text = reader["Cidade"].ToString();
                        txtCEP.Text = reader["CEP"].ToString();
                        dtpAdmissao.Value = Convert.ToDateTime(reader["DataAdmissao"]);

                        if (reader["DataDemissao"] != DBNull.Value)
                            dtpDemissao.Value = Convert.ToDateTime(reader["DataDemissao"]);

                        cmbDLDDLG.Text = reader["DLDDLG"].ToString();
                        txtCBO.Text = reader["CBO"].ToString();
                        txtSalario.Text = reader["Salario"] != DBNull.Value ? reader["Salario"].ToString() : "";
                        txtASO.Text = reader["ASO"].ToString();
                        txtDependentes.Text = reader["Dependentes"] != DBNull.Value ? reader["Dependentes"].ToString() : "";

                        if (reader["PrimeiraExperiencia"] != DBNull.Value)
                            dtpPrimeiraEXP.Value = Convert.ToDateTime(reader["PrimeiraExperiencia"]);
                        if (reader["SegundaExperiencia"] != DBNull.Value)
                            dtpSegundaEXP.Value = Convert.ToDateTime(reader["SegundaExperiencia"]);
                        if (reader["DataInicioFerias"] != DBNull.Value)
                            dtpInicioFerias.Value = Convert.ToDateTime(reader["DataInicioFerias"]);
                        if (reader["DataRetornoFerias"] != DBNull.Value)
                            dtpRetornoFerias.Value = Convert.ToDateTime(reader["DataRetornoFerias"]);
                        if (reader["DataUltimasFerias"] != DBNull.Value)
                            dtpUltimaFerias.Value = Convert.ToDateTime(reader["DataUltimasFerias"]);

                        txtOBSFerias.Text = reader["ObservacaoFerias"].ToString();
                        txtAgenteBancario.Text = reader["AgenteBancario"].ToString();
                        txtCorrente.Text = reader["ContaCorrente"].ToString();
                        txtPIX.Text = reader["ChavePix"].ToString();
                        txtDocumentosExternos.Text = reader["DocumentosExternos"].ToString();
                        txtCursos.Text = reader["Cursos"].ToString();
                        txtTelefone.Text = reader["Telefone"].ToString();
                        txtFuncao.Text = reader["Funcao"].ToString();
                        bool ativo = Convert.ToBoolean(reader["Ativo"]);

                        if (!ativo)
                        {
                            BloquearCamposParaFuncionarioInativo();
                        }
                        else
                        {
                            BloquearCampos();
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados do funcionário: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ValidarCamposObrigatorios())
            {
                MessageBox.Show("Preencha todos os campos obrigatórios (Nome, Data de Nascimento, RG, CPF e Data de Admissão).", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (editando)
            {
                AtualizarFuncionario();
            }
            else
            {
                InserirFuncionario();
            }

            BloquearCampos();
            editando = false;

            foreach (Control control in this.Controls)
            {
                if (control is TextBox)
                {
                    control.BackColor = Color.White;
                }
                else if (control is DateTimePicker)
                {
                    control.BackColor = Color.White;
                }
            }

            CarregarFuncionarios();
        }

        private bool ValidarCamposObrigatorios()
        {
            return !string.IsNullOrWhiteSpace(txtNome.Text) &&
                   !string.IsNullOrWhiteSpace(txtRG.Text) &&
                   !string.IsNullOrWhiteSpace(txtCPF.Text);
        }

        private void InserirFuncionario()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO RHFuncionarios 
                                    (Nome, DataNascimento, Email, ObservacaoGeral, RG, CPF, PIS, PossuiCNH, CNH, 
                                    VencimentoCNH, VencimentoToxicologico, Rua, Numero, Bairro, Cidade, CEP, 
                                    DataAdmissao, DataDemissao, DLDDLG, CBO, Salario, ASO, Dependentes, 
                                    PrimeiraExperiencia, SegundaExperiencia, DataInicioFerias, DataRetornoFerias, 
                                    DataUltimasFerias, ObservacaoFerias, AgenteBancario, ContaCorrente, ChavePix, 
                                    DocumentosExternos, Cursos, Telefone, Funcao, MotivoDesligamento, Ativo, 
                                    DataCadastro, UsuarioCadastro)
                                    VALUES 
                                    (@Nome, @DataNascimento, @Email, @ObservacaoGeral, @RG, @CPF, @PIS, @PossuiCNH, @CNH, 
                                    @VencimentoCNH, @VencimentoToxicologico, @Rua, @Numero, @Bairro, @Cidade, @CEP, 
                                    @DataAdmissao, @DataDemissao, @DLDDLG, @CBO, @Salario, @ASO, @Dependentes, 
                                    @PrimeiraExperiencia, @SegundaExperiencia, @DataInicioFerias, @DataRetornoFerias, 
                                    @DataUltimasFerias, @ObservacaoFerias, @AgenteBancario, @ContaCorrente, @ChavePix, 
                                    @DocumentosExternos, @Cursos, @Telefone, @Funcao, @MotivoDesligamento, @Ativo, 
                                    @DataCadastro, @UsuarioCadastro)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    PreencherParametros(cmd);
                    cmd.Parameters.AddWithValue("@DataCadastro", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UsuarioCadastro", Environment.UserName);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Funcionário cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar funcionário: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarFuncionario()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE RHFuncionarios SET 
                                    Nome = @Nome, DataNascimento = @DataNascimento, Email = @Email, 
                                    ObservacaoGeral = @ObservacaoGeral, RG = @RG, CPF = @CPF, PIS = @PIS, 
                                    PossuiCNH = @PossuiCNH, CNH = @CNH, VencimentoCNH = @VencimentoCNH, 
                                    VencimentoToxicologico = @VencimentoToxicologico, Rua = @Rua, Numero = @Numero, 
                                    Bairro = @Bairro, Cidade = @Cidade, CEP = @CEP, DataAdmissao = @DataAdmissao, 
                                    DataDemissao = @DataDemissao, DLDDLG = @DLDDLG, CBO = @CBO, Salario = @Salario, 
                                    ASO = @ASO, Dependentes = @Dependentes, PrimeiraExperiencia = @PrimeiraExperiencia, 
                                    SegundaExperiencia = @SegundaExperiencia, DataInicioFerias = @DataInicioFerias, 
                                    DataRetornoFerias = @DataRetornoFerias, DataUltimasFerias = @DataUltimasFerias, 
                                    ObservacaoFerias = @ObservacaoFerias, AgenteBancario = @AgenteBancario, 
                                    ContaCorrente = @ContaCorrente, ChavePix = @ChavePix, 
                                    DocumentosExternos = @DocumentosExternos, Cursos = @Cursos, Telefone = @Telefone, 
                                    Funcao = @Funcao, MotivoDesligamento = @MotivoDesligamento, 
                                    DataUltimaAlteracao = @DataUltimaAlteracao, 
                                    UsuarioUltimaAlteracao = @UsuarioUltimaAlteracao
                                    WHERE FuncionarioID = @FuncionarioID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FuncionarioID", funcionarioIDSelecionado);
                    PreencherParametros(cmd);
                    cmd.Parameters.AddWithValue("@DataUltimaAlteracao", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UsuarioUltimaAlteracao", Environment.UserName);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Funcionário atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar funcionário: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PreencherParametros(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Nome", txtNome.Text);
            cmd.Parameters.AddWithValue("@DataNascimento", dtpNascimento.Value);
            cmd.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text);
            cmd.Parameters.AddWithValue("@ObservacaoGeral", string.IsNullOrWhiteSpace(txtOBS.Text) ? (object)DBNull.Value : txtOBS.Text);
            cmd.Parameters.AddWithValue("@RG", txtRG.Text);
            cmd.Parameters.AddWithValue("@CPF", txtCPF.Text);
            cmd.Parameters.AddWithValue("@PIS", string.IsNullOrWhiteSpace(txtPIS.Text) ? (object)DBNull.Value : txtPIS.Text);
            cmd.Parameters.AddWithValue("@PossuiCNH", rbCNHSim.Checked);
            cmd.Parameters.AddWithValue("@CNH", string.IsNullOrWhiteSpace(txtCNH.Text) ? (object)DBNull.Value : txtCNH.Text);
            cmd.Parameters.AddWithValue("@VencimentoCNH", rbCNHSim.Checked && !string.IsNullOrWhiteSpace(txtCNH.Text) ? (object)dtpVencimentoCNH.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@VencimentoToxicologico", rbCNHSim.Checked && !string.IsNullOrWhiteSpace(txtCNH.Text) ? (object)dtpVencimentoToxicologico.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Rua", string.IsNullOrWhiteSpace(txtRua.Text) ? (object)DBNull.Value : txtRua.Text);
            cmd.Parameters.AddWithValue("@Numero", string.IsNullOrWhiteSpace(txtNumero.Text) ? (object)DBNull.Value : txtNumero.Text);
            cmd.Parameters.AddWithValue("@Bairro", string.IsNullOrWhiteSpace(txtBairro.Text) ? (object)DBNull.Value : txtBairro.Text);
            cmd.Parameters.AddWithValue("@Cidade", string.IsNullOrWhiteSpace(txtCidade.Text) ? (object)DBNull.Value : txtCidade.Text);
            cmd.Parameters.AddWithValue("@CEP", string.IsNullOrWhiteSpace(txtCEP.Text) ? (object)DBNull.Value : txtCEP.Text);
            cmd.Parameters.AddWithValue("@DataAdmissao", dtpAdmissao.Value);
            cmd.Parameters.AddWithValue("@DataDemissao", dtpDemissao.Enabled ? (object)dtpDemissao.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@DLDDLG", string.IsNullOrWhiteSpace(cmbDLDDLG.Text) ? (object)DBNull.Value : cmbDLDDLG.Text);
            cmd.Parameters.AddWithValue("@CBO", string.IsNullOrWhiteSpace(txtCBO.Text) ? (object)DBNull.Value : txtCBO.Text);

            decimal salario;
            cmd.Parameters.AddWithValue("@Salario", decimal.TryParse(txtSalario.Text, out salario) ? (object)salario : DBNull.Value);

            cmd.Parameters.AddWithValue("@ASO", string.IsNullOrWhiteSpace(txtASO.Text) ? (object)DBNull.Value : txtASO.Text);

            int dependentes;
            cmd.Parameters.AddWithValue("@Dependentes", int.TryParse(txtDependentes.Text, out dependentes) ? (object)dependentes : DBNull.Value);

            cmd.Parameters.AddWithValue("@PrimeiraExperiencia", (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SegundaExperiencia", (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DataInicioFerias", (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DataRetornoFerias", (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DataUltimasFerias", (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ObservacaoFerias", string.IsNullOrWhiteSpace(txtOBSFerias.Text) ? (object)DBNull.Value : txtOBSFerias.Text);
            cmd.Parameters.AddWithValue("@AgenteBancario", string.IsNullOrWhiteSpace(txtAgenteBancario.Text) ? (object)DBNull.Value : txtAgenteBancario.Text);
            cmd.Parameters.AddWithValue("@ContaCorrente", string.IsNullOrWhiteSpace(txtCorrente.Text) ? (object)DBNull.Value : txtCorrente.Text);
            cmd.Parameters.AddWithValue("@ChavePix", string.IsNullOrWhiteSpace(txtPIX.Text) ? (object)DBNull.Value : txtPIX.Text);
            cmd.Parameters.AddWithValue("@DocumentosExternos", string.IsNullOrWhiteSpace(txtDocumentosExternos.Text) ? (object)DBNull.Value : txtDocumentosExternos.Text);
            cmd.Parameters.AddWithValue("@Cursos", string.IsNullOrWhiteSpace(txtCursos.Text) ? (object)DBNull.Value : txtCursos.Text);
            cmd.Parameters.AddWithValue("@Telefone", string.IsNullOrWhiteSpace(txtTelefone.Text) ? (object)DBNull.Value : txtTelefone.Text);
            cmd.Parameters.AddWithValue("@Funcao", string.IsNullOrWhiteSpace(txtFuncao.Text) ? (object)DBNull.Value : txtFuncao.Text);
            cmd.Parameters.AddWithValue("@MotivoDesligamento", (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Ativo", true);
        }

        private void btnDemissao_Click(object sender, EventArgs e)
        {
            if (dgvFuncionarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um funcionário para demitir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = dgvFuncionarios.SelectedRows[0];
            bool ativo = Convert.ToBoolean(row.Cells["Ativo"].Value);

            if (!ativo)
            {
                MessageBox.Show("Este funcionário já foi demitido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirmacao = MessageBox.Show("Tem certeza que deseja demitir este funcionário?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacao == DialogResult.Yes)
            {
                string motivoDemissao = PromptMotivoDesligamento();

                if (string.IsNullOrWhiteSpace(motivoDemissao))
                {
                    MessageBox.Show("O motivo da demissão é obrigatório.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int funcionarioID = Convert.ToInt32(row.Cells["FuncionarioID"].Value);
                DemitirFuncionario(funcionarioID, motivoDemissao);
                CarregarFuncionarios();
            }
        }

        private void DemitirFuncionario(int funcionarioID, string motivoDemissao)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE RHFuncionarios SET 
                            DataDemissao = @DataDemissao, 
                            MotivoDesligamento = @MotivoDesligamento, 
                            Ativo = 0,
                            DataUltimaAlteracao = @DataUltimaAlteracao,
                            UsuarioUltimaAlteracao = @UsuarioUltimaAlteracao
                            WHERE FuncionarioID = @FuncionarioID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FuncionarioID", funcionarioID);
                    cmd.Parameters.AddWithValue("@DataDemissao", DateTime.Now);
                    cmd.Parameters.AddWithValue("@MotivoDesligamento", motivoDemissao);
                    cmd.Parameters.AddWithValue("@DataUltimaAlteracao", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UsuarioUltimaAlteracao", Environment.UserName);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Funcionário demitido com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CarregarFuncionarios();

                    dgvFuncionarios.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao demitir funcionário: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvFuncionarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvFuncionarios.Rows.Count)
                return;

            DataGridViewRow row = dgvFuncionarios.Rows[e.RowIndex];

            if (row.Cells["Ativo"] == null || row.Cells["Ativo"].Value == null ||
                row.Cells["Ativo"].Value == DBNull.Value)
                return;

            bool ativo = Convert.ToBoolean(row.Cells["Ativo"].Value);

            if (row.Cells["FuncionarioID"].Value != null)
            {
                int funcionarioID = Convert.ToInt32(row.Cells["FuncionarioID"].Value);
                CarregarDadosFuncionario(funcionarioID);

                if (!ativo)
                {
                    string motivoDesligamento = "Não informado";

                    if (row.Cells["MotivoDesligamento"] != null &&
                        row.Cells["MotivoDesligamento"].Value != null &&
                        row.Cells["MotivoDesligamento"].Value != DBNull.Value)
                    {
                        motivoDesligamento = row.Cells["MotivoDesligamento"].Value.ToString();
                    }

                    MessageBox.Show(
                        "Funcionário Demitido\n\n" +
                        $"Motivo:\n{motivoDesligamento}",
                        "Informações do Funcionário",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }

            if (!ativo)
            {
                BloquearCamposParaFuncionarioInativo();
            }
        }

        private void BloquearCamposParaFuncionarioInativo()
        {
            BloquearCampos();

            foreach (Control control in this.Controls)
            {
                if (control is TextBox)
                {
                    control.BackColor = Color.FromArgb(240, 240, 240);
                }
                else if (control is DateTimePicker)
                {
                    control.BackColor = Color.FromArgb(240, 240, 240);
                    control.Enabled = false;
                }
            }
        }

        private void rbCNHSim_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCNHSim.Checked)
            {
                txtCNH.Enabled = true;
                dtpVencimentoCNH.Enabled = true;
                dtpVencimentoToxicologico.Enabled = true;
            }
        }

        private void rbCNHNao_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCNHNao.Checked)
            {
                txtCNH.Enabled = false;
                txtCNH.Clear();
                dtpVencimentoCNH.Enabled = false;
                dtpVencimentoToxicologico.Enabled = false;
            }
        }

        private void LimparCampos()
        {
            txtNome.Clear();
            txtEmail.Clear();
            txtOBS.Clear();
            txtRG.Clear();
            txtCPF.Clear();
            txtPIS.Clear();
            rbCNHNao.Checked = true;
            txtCNH.Clear();
            txtRua.Clear();
            txtNumero.Clear();
            txtBairro.Clear();
            txtCidade.Clear();
            txtCEP.Clear();
            cmbDLDDLG.SelectedIndex = -1;
            txtCBO.Clear();
            txtSalario.Clear();
            txtASO.Clear();
            txtDependentes.Clear();
            txtOBSFerias.Clear();
            txtAgenteBancario.Clear();
            txtCorrente.Clear();
            txtPIX.Clear();
            txtDocumentosExternos.Clear();
            txtCursos.Clear();
            txtTelefone.Clear();
            txtFuncao.Clear();

            dtpNascimento.Value = DateTime.Now;
            dtpAdmissao.Value = DateTime.Now;
            dtpDemissao.Value = DateTime.Now;
            dtpVencimentoCNH.Value = DateTime.Now;
            dtpVencimentoToxicologico.Value = DateTime.Now;
            dtpPrimeiraEXP.Value = DateTime.Now;
            dtpSegundaEXP.Value = DateTime.Now;
            dtpInicioFerias.Value = DateTime.Now;
            dtpRetornoFerias.Value = DateTime.Now;
            dtpUltimaFerias.Value = DateTime.Now;
        }

        private void BloquearCampos()
        {
            txtNome.Enabled = false;
            dtpNascimento.Enabled = false;
            txtEmail.Enabled = false;
            txtOBS.Enabled = false;
            txtRG.Enabled = false;
            txtCPF.Enabled = false;
            txtPIS.Enabled = false;
            rbCNHSim.Enabled = false;
            rbCNHNao.Enabled = false;
            txtCNH.Enabled = false;
            dtpVencimentoCNH.Enabled = false;
            dtpVencimentoToxicologico.Enabled = false;
            txtRua.Enabled = false;
            txtNumero.Enabled = false;
            txtBairro.Enabled = false;
            txtCidade.Enabled = false;
            txtCEP.Enabled = false;
            dtpAdmissao.Enabled = false;
            dtpDemissao.Enabled = false;
            cmbDLDDLG.Enabled = false;
            txtCBO.Enabled = false;
            txtSalario.Enabled = false;
            txtASO.Enabled = false;
            txtDependentes.Enabled = false;
            dtpPrimeiraEXP.Enabled = false;
            dtpSegundaEXP.Enabled = false;
            dtpInicioFerias.Enabled = false;
            dtpRetornoFerias.Enabled = false;
            dtpUltimaFerias.Enabled = false;
            txtOBSFerias.Enabled = false;
            txtAgenteBancario.Enabled = false;
            txtCorrente.Enabled = false;
            txtPIX.Enabled = false;
            txtDocumentosExternos.Enabled = false;
            txtCursos.Enabled = false;
            txtTelefone.Enabled = false;
            txtFuncao.Enabled = false;
        }

        private void DesbloquearCampos()
        {
            txtNome.Enabled = true;
            dtpNascimento.Enabled = true;
            txtEmail.Enabled = true;
            txtOBS.Enabled = true;
            txtRG.Enabled = true;
            txtCPF.Enabled = true;
            txtPIS.Enabled = true;
            rbCNHSim.Enabled = true;
            rbCNHNao.Enabled = true;
            txtRua.Enabled = true;
            txtNumero.Enabled = true;
            txtBairro.Enabled = true;
            txtCidade.Enabled = true;
            txtCEP.Enabled = true;
            dtpAdmissao.Enabled = true;
            cmbDLDDLG.Enabled = true;
            txtCBO.Enabled = true;
            txtSalario.Enabled = true;
            txtASO.Enabled = true;
            txtDependentes.Enabled = true;
            dtpPrimeiraEXP.Enabled = true;
            dtpSegundaEXP.Enabled = true;
            dtpInicioFerias.Enabled = true;
            dtpRetornoFerias.Enabled = true;
            dtpUltimaFerias.Enabled = true;
            txtOBSFerias.Enabled = true;
            txtAgenteBancario.Enabled = true;
            txtCorrente.Enabled = true;
            txtPIX.Enabled = true;
            txtDocumentosExternos.Enabled = true;
            txtCursos.Enabled = true;
            txtTelefone.Enabled = true;
            txtFuncao.Enabled = true;
        }

        private string PromptMotivoDesligamento()
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 230,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Motivo de Demissão",
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false,
                Padding = new Padding(20)
            };

            Label textLabel = new Label()
            {
                Left = 20,
                Top = 20,
                Width = 450,
                Height = 20,
                Text = "Informe o motivo da demissão:"
            };

            TextBox textBox = new TextBox()
            {
                Left = 20,
                Top = 48,
                Width = 450,
                Height = 80,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            Button confirmation = new Button()
            {
                Text = "OK",
                Left = 305,
                Width = 80,
                Top = 148,
                DialogResult = DialogResult.OK
            };

            Button cancel = new Button()
            {
                Text = "Cancelar",
                Left = 393,
                Width = 85,
                Top = 148,
                DialogResult = DialogResult.Cancel
            };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}