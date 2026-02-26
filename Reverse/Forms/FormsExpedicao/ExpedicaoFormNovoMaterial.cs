using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormNovoMaterial : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;

        public ExpedicaoFormNovoMaterial()
        {
            InitializeComponent();

            this.Load += FormNovoMaterial_Load;
            btnMaterial.Click += btnMaterial_Click;
            btnTipo.Click += btnTipo_Click;
            btnTratamento.Click += btnTratamento_Click;
            btnExcluir.Click += btnExcluir_Click;
            btnSalvar.Click += btnSalvar_Click;
            btnSair.Click += btnSair_Click;
        }

        private async void FormNovoMaterial_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            await CarregarDadosAsync();
        }

        private void ConfigurarGrid()
        {
            dgvMaterial.Columns.Clear();
            dgvMaterial.AllowUserToAddRows = false;
            dgvMaterial.AllowUserToDeleteRows = false;
            dgvMaterial.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvMaterial.AutoGenerateColumns = false;

            // Coluna oculta para controlar se o registro é novo (0) ou existente (ID real)
            var colId = new DataGridViewTextBoxColumn
            {
                Name = "MaterialId",
                Visible = false
            };
            dgvMaterial.Columns.Add(colId);

            // Coluna: Nome do material
            var colNome = new DataGridViewTextBoxColumn
            {
                Name = "Nome",
                HeaderText = "Material",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 40
            };
            dgvMaterial.Columns.Add(colNome);

            // Coluna: Tipo (ComboBox)
            var colTipo = new DataGridViewComboBoxColumn
            {
                Name = "Tipo",
                HeaderText = "Tipo",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 30
            };
            dgvMaterial.Columns.Add(colTipo);

            // Coluna: Tratamento (ComboBox)
            var colTratamento = new DataGridViewComboBoxColumn
            {
                Name = "Tratamento",
                HeaderText = "Tratamento",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 30
            };
            dgvMaterial.Columns.Add(colTratamento);

            // Confirma edição do ComboBox imediatamente ao selecionar
            dgvMaterial.CurrentCellDirtyStateChanged += (s, ev) =>
            {
                if (dgvMaterial.IsCurrentCellDirty && dgvMaterial.CurrentCell is DataGridViewComboBoxCell)
                    dgvMaterial.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            // Suprime erros de binding de ComboBox (ex.: valor ainda não carregado)
            dgvMaterial.DataError += (s, ev) =>
            {
                ev.ThrowException = false;
                ev.Cancel = false;
            };

            AplicarEstiloVisual(dgvMaterial);
        }

        private async Task CarregarDadosAsync()
        {
            await CarregarTiposAsync();
            await CarregarTratamentosAsync();
            await CarregarMateriaisAsync();
        }

        private async Task CarregarTiposAsync()
        {
            var tipos = new List<string>();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(
                    "SELECT Descricao FROM ExpTipoMaterial ORDER BY Descricao", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        tipos.Add(reader["Descricao"].ToString());
                }
            }

            var colTipo = (DataGridViewComboBoxColumn)dgvMaterial.Columns["Tipo"];
            colTipo.DataSource = new List<string>(tipos);
        }

        private async Task CarregarTratamentosAsync()
        {
            var tratamentos = new List<string>();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(
                    "SELECT Descricao FROM ExpTratamentoMaterial ORDER BY Descricao", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        tratamentos.Add(reader["Descricao"].ToString());
                }
            }

            var colTratamento = (DataGridViewComboBoxColumn)dgvMaterial.Columns["Tratamento"];
            colTratamento.DataSource = new List<string>(tratamentos);
        }

        private async Task CarregarMateriaisAsync()
        {
            dgvMaterial.Rows.Clear();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(
                    "SELECT MaterialId, Nome, Tipo, Tratamento FROM ExpMaterialLaudo ORDER BY Nome", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int idx = dgvMaterial.Rows.Add();
                        var row = dgvMaterial.Rows[idx];

                        row.Cells["MaterialId"].Value = reader["MaterialId"].ToString();
                        row.Cells["Nome"].Value = reader["Nome"].ToString();
                        row.Cells["Tipo"].Value = reader["Tipo"].ToString();
                        row.Cells["Tratamento"].Value = reader["Tratamento"].ToString();
                    }
                }
            }
        }

        private void btnMaterial_Click(object sender, EventArgs e)
        {
            int idx = dgvMaterial.Rows.Add();
            dgvMaterial.Rows[idx].Cells["MaterialId"].Value = "0";

            dgvMaterial.CurrentCell = dgvMaterial.Rows[idx].Cells["Nome"];
            dgvMaterial.BeginEdit(true);
        }

        private async void btnTipo_Click(object sender, EventArgs e)
        {
            string novoTipo = MostrarInputBox(
                "Novo Tipo",
                "Digite o nome do novo tipo de material:");

            if (string.IsNullOrWhiteSpace(novoTipo)) return;

            novoTipo = novoTipo.Trim().ToUpper();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(@"
                        IF NOT EXISTS (SELECT 1 FROM ExpTipoMaterial WHERE Descricao = @Desc)
                            INSERT INTO ExpTipoMaterial (Descricao) VALUES (@Desc)", conn))
                    {
                        cmd.Parameters.AddWithValue("@Desc", novoTipo);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // Recarrega o ComboBox de Tipo em todas as linhas da grid
                await CarregarTiposAsync();

                MessageBox.Show($"Tipo '{novoTipo}' adicionado com sucesso!",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar tipo: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnTratamento_Click(object sender, EventArgs e)
        {
            string novoTratamento = MostrarInputBox(
                "Novo Tratamento",
                "Digite o nome do novo tratamento:");

            if (string.IsNullOrWhiteSpace(novoTratamento)) return;

            novoTratamento = novoTratamento.Trim().ToUpper();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(@"
                        IF NOT EXISTS (SELECT 1 FROM ExpTratamentoMaterial WHERE Descricao = @Desc)
                            INSERT INTO ExpTratamentoMaterial (Descricao) VALUES (@Desc)", conn))
                    {
                        cmd.Parameters.AddWithValue("@Desc", novoTratamento);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                await CarregarTratamentosAsync();

                MessageBox.Show($"Tratamento '{novoTratamento}' adicionado com sucesso!",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar tratamento: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvMaterial.CurrentRow == null || dgvMaterial.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Selecione um material para excluir.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var linhaAtual = dgvMaterial.CurrentRow;
            string idStr = linhaAtual.Cells["MaterialId"].Value?.ToString() ?? "0";
            string nome = linhaAtual.Cells["Nome"].Value?.ToString() ?? "";

            // Linha ainda não foi salva: remove só da grid, sem ir ao banco
            if (idStr == "0")
            {
                dgvMaterial.Rows.Remove(linhaAtual);
                return;
            }

            int materialId = int.Parse(idStr);

            try
            {
                // Verifica se o material já está em uso em algum lançamento
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmdCheck = new SqlCommand(
                        "SELECT COUNT(1) FROM LancamentosMateriais WHERE Material = @Nome", conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@Nome", nome);
                        int usos = (int)await cmdCheck.ExecuteScalarAsync();

                        if (usos > 0)
                        {
                            MessageBox.Show(
                                $"O material '{nome}' não pode ser excluído pois está vinculado a {usos} lançamento(s).\n\n" +
                                "Para removê-lo, primeiro exclua ou altere os lançamentos que o utilizam.",
                                "Exclusão bloqueada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                var confirmacao = MessageBox.Show(
                    $"Deseja excluir permanentemente o material '{nome}'?\n\nEssa ação não pode ser desfeita.",
                    "Confirmação de exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacao != DialogResult.Yes) return;

                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmdDel = new SqlCommand(
                        "DELETE FROM ExpMaterialLaudo WHERE MaterialId = @Id", conn))
                    {
                        cmdDel.Parameters.AddWithValue("@Id", materialId);
                        await cmdDel.ExecuteNonQueryAsync();
                    }
                }

                dgvMaterial.Rows.Remove(linhaAtual);
                MessageBox.Show("Material excluído com sucesso!",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir material: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            // ── Validação ──────────────────────────────────────────────
            foreach (DataGridViewRow row in dgvMaterial.Rows)
            {
                if (row.IsNewRow) continue;

                string nome = row.Cells["Nome"].Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(nome))
                {
                    MessageBox.Show("Todas as linhas devem ter o nome do material preenchido.",
                        "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvMaterial.CurrentCell = row.Cells["Nome"];
                    return;
                }

                if (string.IsNullOrWhiteSpace(row.Cells["Tipo"].Value?.ToString()))
                {
                    MessageBox.Show($"O material '{nome}' precisa ter um Tipo selecionado.",
                        "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvMaterial.CurrentCell = row.Cells["Tipo"];
                    return;
                }

                if (string.IsNullOrWhiteSpace(row.Cells["Tratamento"].Value?.ToString()))
                {
                    MessageBox.Show($"O material '{nome}' precisa ter um Tratamento selecionado.",
                        "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvMaterial.CurrentCell = row.Cells["Tratamento"];
                    return;
                }
            }

            btnSalvar.Enabled = false;
            Cursor = Cursors.WaitCursor;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (DataGridViewRow row in dgvMaterial.Rows)
                            {
                                if (row.IsNewRow) continue;

                                string idStr = row.Cells["MaterialId"].Value?.ToString() ?? "0";
                                int id = int.TryParse(idStr, out var pid) ? pid : 0;
                                string nome = row.Cells["Nome"].Value.ToString().Trim().ToUpper();
                                string tipo = row.Cells["Tipo"].Value.ToString();
                                string trat = row.Cells["Tratamento"].Value.ToString();

                                if (id == 0)
                                {
                                    // ── INSERT ───────────────────────
                                    using (var cmdIns = new SqlCommand(@"
                                        INSERT INTO ExpMaterialLaudo (Nome, Tipo, Tratamento)
                                        VALUES (@Nome, @Tipo, @Tratamento);
                                        SELECT SCOPE_IDENTITY();", conn, transaction))
                                    {
                                        cmdIns.Parameters.AddWithValue("@Nome", nome);
                                        cmdIns.Parameters.AddWithValue("@Tipo", tipo);
                                        cmdIns.Parameters.AddWithValue("@Tratamento", trat);

                                        var novoId = await cmdIns.ExecuteScalarAsync();
                                        row.Cells["MaterialId"].Value = novoId.ToString();
                                        row.Cells["Nome"].Value = nome;
                                    }
                                }
                                else
                                {
                                    // ── UPDATE ───────────────────────
                                    using (var cmdUpd = new SqlCommand(@"
                                        UPDATE ExpMaterialLaudo
                                        SET Nome = @Nome, Tipo = @Tipo, Tratamento = @Tratamento
                                        WHERE MaterialId = @Id", conn, transaction))
                                    {
                                        cmdUpd.Parameters.AddWithValue("@Nome", nome);
                                        cmdUpd.Parameters.AddWithValue("@Tipo", tipo);
                                        cmdUpd.Parameters.AddWithValue("@Tratamento", trat);
                                        cmdUpd.Parameters.AddWithValue("@Id", id);

                                        await cmdUpd.ExecuteNonQueryAsync();
                                        row.Cells["Nome"].Value = nome;
                                    }
                                }
                            }

                            transaction.Commit();
                            MessageBox.Show("Materiais salvos com sucesso!",
                                "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar materiais: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSalvar.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void btnSair_Click(object sender, EventArgs e) => this.Close();

        private string MostrarInputBox(string titulo, string mensagem)
        {
            using (var form = new Form())
            {
                form.Text = titulo;
                form.Size = new Size(420, 165);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var lbl = new Label
                {
                    Text = mensagem,
                    Left = 12,
                    Top = 15,
                    Width = 388,
                    Height = 28,
                    Font = new Font("Segoe UI", 9F)
                };

                var txt = new TextBox
                {
                    Left = 12,
                    Top = 48,
                    Width = 388,
                    Font = new Font("Segoe UI", 9F)
                };

                var btnOk = new Button
                {
                    Text = "OK",
                    Left = 220,
                    Top = 85,
                    Width = 85,
                    Height = 30,
                    DialogResult = DialogResult.OK,
                    Font = new Font("Segoe UI", 9F)
                };

                var btnCancel = new Button
                {
                    Text = "Cancelar",
                    Left = 315,
                    Top = 85,
                    Width = 85,
                    Height = 30,
                    DialogResult = DialogResult.Cancel,
                    Font = new Font("Segoe UI", 9F)
                };

                form.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                return form.ShowDialog(this) == DialogResult.OK ? txt.Text : null;
            }
        }

        private void AplicarEstiloVisual(DataGridView grid)
        {
            grid.BackgroundColor = Color.FromArgb(250, 250, 252);
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(230, 230, 235);

            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;

            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = grid.ColumnHeadersDefaultCellStyle.BackColor;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            grid.RowsDefaultCellStyle.BackColor = Color.White;

            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.DefaultCellStyle.ForeColor = Color.Black;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);

            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 36;
        }
    }
}