using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormMotorista : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private int? motoristaAtualId;
        private bool isBindingGrid = false;

        public event Action MotoristasAtualizados;

        public ExpedicaoFormMotorista()
        {
            InitializeComponent();
            this.Load += ExpedicaoFormMotorista_Load;
            dgvMotorista.SelectionChanged += DgvMotorista_SelectionChanged;
        }

        private async void ExpedicaoFormMotorista_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CarregarComboFuncao();
            await CarregarMotoristasAsync();
        }

        private void ConfigurarGrid()
        {
            dgvMotorista.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMotorista.MultiSelect = false;
            dgvMotorista.ReadOnly = true;
            dgvMotorista.AllowUserToAddRows = false;
            dgvMotorista.AllowUserToDeleteRows = false;
            dgvMotorista.AllowUserToResizeRows = false;
            dgvMotorista.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvMotorista.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvMotorista.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dgvMotorista.DefaultCellStyle.ForeColor = Color.Black;
            dgvMotorista.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgvMotorista.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvMotorista.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvMotorista.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
        }

        private void CarregarComboFuncao()
        {
            cmbFuncao.Items.Clear();
            cmbFuncao.Items.AddRange(new string[] { "Motorista", "Ajudante" });
        }

        private async Task CarregarMotoristasAsync()
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"
                        SELECT 
                            MotoristaId,
                            NomeCompleto,
                            NomeInterno,
                            Funcao,
                            CPF,
                            CNH,
                            VencimentoCNH,
                            VencimentoToxicologico,
                            VencimentoCurso
                        FROM Motoristas
                        WHERE Ativo = 1
                        ORDER BY Funcao, NomeInterno";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        var dt = new DataTable();
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            isBindingGrid = true;
                            adapter.Fill(dt);
                        }

                        dgvMotorista.DataSource = dt;
                        ConfigurarColunasGrid();

                        dgvMotorista.ClearSelection();
                        isBindingGrid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar motoristas: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isBindingGrid = false;
            }
        }

        private void ConfigurarColunasGrid()
        {
            if (dgvMotorista.Columns.Contains("MotoristaId"))
                dgvMotorista.Columns["MotoristaId"].Visible = false;

            if (dgvMotorista.Columns.Contains("NomeCompleto"))
            {
                dgvMotorista.Columns["NomeCompleto"].HeaderText = "Nome Completo";
                dgvMotorista.Columns["NomeCompleto"].FillWeight = 150;
            }

            if (dgvMotorista.Columns.Contains("NomeInterno"))
            {
                dgvMotorista.Columns["NomeInterno"].HeaderText = "Nome Interno";
                dgvMotorista.Columns["NomeInterno"].FillWeight = 80;
            }

            if (dgvMotorista.Columns.Contains("Funcao"))
            {
                dgvMotorista.Columns["Funcao"].HeaderText = "Função";
                dgvMotorista.Columns["Funcao"].FillWeight = 70;
            }

            if (dgvMotorista.Columns.Contains("CPF"))
            {
                dgvMotorista.Columns["CPF"].HeaderText = "CPF";
                dgvMotorista.Columns["CPF"].FillWeight = 90;
            }

            if (dgvMotorista.Columns.Contains("CNH"))
            {
                dgvMotorista.Columns["CNH"].HeaderText = "CNH";
                dgvMotorista.Columns["CNH"].FillWeight = 90;
            }

            if (dgvMotorista.Columns.Contains("VencimentoCNH"))
            {
                dgvMotorista.Columns["VencimentoCNH"].HeaderText = "Venc. CNH";
                dgvMotorista.Columns["VencimentoCNH"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvMotorista.Columns["VencimentoCNH"].FillWeight = 80;
            }

            if (dgvMotorista.Columns.Contains("VencimentoToxicologico"))
            {
                dgvMotorista.Columns["VencimentoToxicologico"].HeaderText = "Venc. Toxico";
                dgvMotorista.Columns["VencimentoToxicologico"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvMotorista.Columns["VencimentoToxicologico"].FillWeight = 80;
            }

            if (dgvMotorista.Columns.Contains("VencimentoCurso"))
            {
                dgvMotorista.Columns["VencimentoCurso"].HeaderText = "Venc. Curso";
                dgvMotorista.Columns["VencimentoCurso"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvMotorista.Columns["VencimentoCurso"].FillWeight = 80;
            }

            VerificarVencimentos();
        }

        private void VerificarVencimentos()
        {
            DateTime dataLimite = DateTime.Now.AddDays(30);

            foreach (DataGridViewRow row in dgvMotorista.Rows)
            {
                // Verificar CNH
                if (row.Cells["VencimentoCNH"].Value != DBNull.Value)
                {
                    DateTime vencCNH = Convert.ToDateTime(row.Cells["VencimentoCNH"].Value);
                    if (vencCNH <= dataLimite)
                        row.Cells["VencimentoCNH"].Style.BackColor = Color.LightCoral;
                }

                // Verificar Toxicológico
                if (row.Cells["VencimentoToxicologico"].Value != DBNull.Value)
                {
                    DateTime vencToxico = Convert.ToDateTime(row.Cells["VencimentoToxicologico"].Value);
                    if (vencToxico <= dataLimite)
                        row.Cells["VencimentoToxicologico"].Style.BackColor = Color.LightCoral;
                }

                // Verificar Curso
                if (row.Cells["VencimentoCurso"].Value != DBNull.Value)
                {
                    DateTime vencCurso = Convert.ToDateTime(row.Cells["VencimentoCurso"].Value);
                    if (vencCurso <= dataLimite)
                        row.Cells["VencimentoCurso"].Style.BackColor = Color.LightCoral;
                }
            }
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            LimparCampos();
            motoristaAtualId = null;
            txtNome.Focus();
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    SqlCommand cmd;
                    if (motoristaAtualId.HasValue)
                    {
                        // UPDATE
                        cmd = new SqlCommand(@"
                            UPDATE Motoristas SET
                                NomeCompleto = @NomeCompleto,
                                NomeInterno = @NomeInterno,
                                Funcao = @Funcao,
                                RG = @RG,
                                CPF = @CPF,
                                CNH = @CNH,
                                CategoriaCNH = @CategoriaCNH,
                                VencimentoCNH = @VencimentoCNH,
                                VencimentoToxicologico = @VencimentoToxicologico,
                                VencimentoCurso = @VencimentoCurso,
                                ChavePIX = @ChavePIX,
                                CartaoCredito = @CartaoCredito,
                                DataAlteracao = GETDATE()
                            WHERE MotoristaId = @MotoristaId", conn);

                        cmd.Parameters.AddWithValue("@MotoristaId", motoristaAtualId.Value);
                    }
                    else
                    {
                        // INSERT
                        cmd = new SqlCommand(@"
                            INSERT INTO Motoristas 
                            (NomeCompleto, NomeInterno, Funcao, RG, CPF, CNH, CategoriaCNH,
                             VencimentoCNH, VencimentoToxicologico, VencimentoCurso, 
                             ChavePIX, CartaoCredito, Ativo, DataCadastro)
                            VALUES
                            (@NomeCompleto, @NomeInterno, @Funcao, @RG, @CPF, @CNH, @CategoriaCNH,
                             @VencimentoCNH, @VencimentoToxicologico, @VencimentoCurso,
                             @ChavePIX, @CartaoCredito, 1, GETDATE())", conn);
                    }

                    PreencherParametros(cmd);
                    await cmd.ExecuteNonQueryAsync();
                }

                MessageBox.Show("Registro salvo com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await CarregarMotoristasAsync();
                LimparCampos();
                motoristaAtualId = null;

                // Notifica o form de controle para atualizar as combos
                MotoristasAtualizados?.Invoke();
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                MessageBox.Show("Já existe um motorista/ajudante cadastrado com este Nome Interno!",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Informe o Nome Completo!", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNomeInterno.Text))
            {
                MessageBox.Show("Informe o Nome Interno!", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNomeInterno.Focus();
                return false;
            }

            if (cmbFuncao.SelectedIndex == -1)
            {
                MessageBox.Show("Selecione a Função!", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbFuncao.Focus();
                return false;
            }

            // Validar CPF se preenchido
            if (!string.IsNullOrWhiteSpace(txtCPF.Text) && !ValidarCPF(txtCPF.Text))
            {
                MessageBox.Show("CPF inválido!", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCPF.Focus();
                return false;
            }

            return true;
        }

        private bool ValidarCPF(string cpf)
        {
            cpf = Regex.Replace(cpf, @"[^\d]", "");

            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            bool todosIguais = true;
            for (int i = 1; i < 11 && todosIguais; i++)
                if (cpf[i] != cpf[0])
                    todosIguais = false;

            if (todosIguais)
                return false;

            // Calcula os dígitos verificadores
            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }

        private void PreencherParametros(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@NomeCompleto", txtNome.Text.Trim());
            cmd.Parameters.AddWithValue("@NomeInterno", txtNomeInterno.Text.Trim().ToUpper());
            cmd.Parameters.AddWithValue("@Funcao", cmbFuncao.Text);
            cmd.Parameters.AddWithValue("@RG",
                string.IsNullOrWhiteSpace(txtRG.Text) ? DBNull.Value : (object)txtRG.Text.Trim());
            cmd.Parameters.AddWithValue("@CPF",
                string.IsNullOrWhiteSpace(txtCPF.Text) ? DBNull.Value : (object)txtCPF.Text.Trim());
            cmd.Parameters.AddWithValue("@CNH",
                string.IsNullOrWhiteSpace(txtCNH.Text) ? DBNull.Value : (object)txtCNH.Text.Trim());
            cmd.Parameters.AddWithValue("@CategoriaCNH",
                string.IsNullOrWhiteSpace(txtCategoria.Text) ? DBNull.Value : (object)txtCategoria.Text.Trim());
            cmd.Parameters.AddWithValue("@VencimentoCNH",
                dtpCNH.Checked ? (object)dtpCNH.Value.Date : DBNull.Value);
            cmd.Parameters.AddWithValue("@VencimentoToxicologico",
                dtpToxicologico.Checked ? (object)dtpToxicologico.Value.Date : DBNull.Value);
            cmd.Parameters.AddWithValue("@VencimentoCurso",
                dtpCurso.Checked ? (object)dtpCurso.Value.Date : DBNull.Value);
            cmd.Parameters.AddWithValue("@ChavePIX",
                string.IsNullOrWhiteSpace(txtPIX.Text) ? DBNull.Value : (object)txtPIX.Text.Trim());
            cmd.Parameters.AddWithValue("@CartaoCredito",
                string.IsNullOrWhiteSpace(txtCartao.Text) ? DBNull.Value : (object)txtCartao.Text.Trim());
        }

        private async void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvMotorista.CurrentRow == null)
            {
                MessageBox.Show("Selecione um registro para excluir!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var valorId = dgvMotorista.CurrentRow.Cells["MotoristaId"].Value;
            if (valorId == null || valorId == DBNull.Value)
            {
                MessageBox.Show("Registro inválido!", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = Convert.ToInt32(valorId);
            string nomeInterno = dgvMotorista.CurrentRow.Cells["NomeInterno"].Value.ToString();

            var confirm = MessageBox.Show(
                $"Deseja realmente excluir o registro de '{nomeInterno}'?\n\n" +
                "Esta ação não poderá ser desfeita!",
                "Confirmação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    // Soft delete - marca como inativo
                    var cmd = new SqlCommand(
                        "UPDATE Motoristas SET Ativo = 0, DataAlteracao = GETDATE() WHERE MotoristaId = @Id",
                        conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }

                MessageBox.Show("Registro excluído com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await CarregarMotoristasAsync();
                LimparCampos();
                motoristaAtualId = null;

                // Notifica o form de controle
                MotoristasAtualizados?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DgvMotorista_SelectionChanged(object sender, EventArgs e)
        {
            if (isBindingGrid) return;

            var row = dgvMotorista.CurrentRow;
            if (row == null || row.IsNewRow) return;

            var idValue = row.Cells["MotoristaId"].Value;
            if (idValue == null || idValue == DBNull.Value) return;

            motoristaAtualId = Convert.ToInt32(idValue);
            await CarregarRegistroCompletoAsync(motoristaAtualId.Value);
        }

        private async Task CarregarRegistroCompletoAsync(int id)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string sql = "SELECT * FROM Motoristas WHERE MotoristaId = @Id";
                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            txtNome.Text = reader["NomeCompleto"]?.ToString() ?? "";
                            txtNomeInterno.Text = reader["NomeInterno"]?.ToString() ?? "";
                            cmbFuncao.Text = reader["Funcao"]?.ToString() ?? "";
                            txtRG.Text = reader["RG"]?.ToString() ?? "";
                            txtCPF.Text = reader["CPF"]?.ToString() ?? "";
                            txtCNH.Text = reader["CNH"]?.ToString() ?? "";
                            txtCategoria.Text = reader["CategoriaCNH"]?.ToString() ?? "";

                            if (reader["VencimentoCNH"] != DBNull.Value)
                            {
                                dtpCNH.Value = Convert.ToDateTime(reader["VencimentoCNH"]);
                                dtpCNH.Checked = true;
                            }
                            else
                            {
                                dtpCNH.Checked = false;
                            }

                            if (reader["VencimentoToxicologico"] != DBNull.Value)
                            {
                                dtpToxicologico.Value = Convert.ToDateTime(reader["VencimentoToxicologico"]);
                                dtpToxicologico.Checked = true;
                            }
                            else
                            {
                                dtpToxicologico.Checked = false;
                            }

                            if (reader["VencimentoCurso"] != DBNull.Value)
                            {
                                dtpCurso.Value = Convert.ToDateTime(reader["VencimentoCurso"]);
                                dtpCurso.Checked = true;
                            }
                            else
                            {
                                dtpCurso.Checked = false;
                            }

                            txtPIX.Text = reader["ChavePIX"]?.ToString() ?? "";
                            txtCartao.Text = reader["CartaoCredito"]?.ToString() ?? "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar registro: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimparCampos()
        {
            txtNome.Clear();
            txtNomeInterno.Clear();
            cmbFuncao.SelectedIndex = -1;
            txtRG.Clear();
            txtCPF.Clear();
            txtCNH.Clear();
            txtCategoria.Clear();
            dtpCNH.Checked = false;
            dtpToxicologico.Checked = false;
            dtpCurso.Checked = false;
            txtPIX.Clear();
            txtCartao.Clear();
            motoristaAtualId = null;
        }

        public static DataTable ObterMotoristas(string funcao = null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT MotoristaId, NomeInterno, NomeCompleto, Funcao
                    FROM Motoristas
                    WHERE Ativo = 1";

                if (!string.IsNullOrEmpty(funcao))
                    sql += " AND Funcao = @Funcao";

                sql += " ORDER BY NomeInterno";

                var cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(funcao))
                    cmd.Parameters.AddWithValue("@Funcao", funcao);

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                return dt;
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}