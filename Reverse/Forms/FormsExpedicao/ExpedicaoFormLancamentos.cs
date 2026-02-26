using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormLancamentos : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private bool isLoadingTicket = false;
        private bool isBindingTickets = false;

        public ExpedicaoFormLancamentos()
        {
            InitializeComponent();
            this.Load += FormLancamento_Load;

            btnCriar.Click += btnCriar_Click;
            btnExcluir.Click += btnExcluir_Click;
            btnCancelar.Click += btnCancelar_Click;
            btnSalvar.Click += btnSalvar_Click;

            dgvLancamentos.CellValueChanged += dgvLancamentos_CellValueChanged;
            dgvLancamentos.RowsRemoved += dgvLancamentos_RowsRemoved;
            dgvLancamentos.CurrentCellDirtyStateChanged += dgvLancamentos_CurrentCellDirtyStateChanged;
            dgvLancamentos.DataError += dgvLancamentos_DataError;
            dgvLancamentos.CellClick += dgvLancamentos_CellClick;
            dgvLancamentos.CellEnter += dgvLancamentos_CellEnter;

            cbTicket.SelectedIndexChanged += cbTicket_SelectedIndexChanged;
        }

        private void AplicarEstiloVisualProducao(DataGridView grid)
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

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            grid.RowsDefaultCellStyle.BackColor = Color.White;

            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = grid.ColumnHeadersDefaultCellStyle.BackColor;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            grid.DefaultCellStyle.ForeColor = Color.Black;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.RowTemplate.Height = 36;
        }

        private void dgvLancamentos_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvLancamentos.IsCurrentCellDirty)
            {
                if (dgvLancamentos.CurrentCell is DataGridViewComboBoxCell)
                {
                    dgvLancamentos.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }

        private void dgvLancamentos_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context == DataGridViewDataErrorContexts.Commit ||
                e.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            {
                e.ThrowException = false;
                e.Cancel = false;
            }
        }

        private async void FormLancamento_Load(object sender, EventArgs e)
        {
            await CarregarEmpresasAsync();
            ConfigurarGrid();
            await CarregarMateriaisComboBoxAsync();
            HabilitarCampos(false);
        }


        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void HabilitarCampos(bool habilitar)
        {
            txtNF.Enabled = habilitar;
            txtMTR.Enabled = habilitar;
            txtVolume.Enabled = false;
            txtPeso.Enabled = false;
            dtpDataLanca.Enabled = false;
            txtObs.Enabled = habilitar;
            btnCriar.Enabled = habilitar;
            btnExcluir.Enabled = habilitar;
            btnSalvar.Enabled = habilitar;
            dgvLancamentos.Enabled = habilitar;
        }

        private async Task CarregarEmpresasAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT ClienteId, Nome FROM Clientes ORDER BY Nome", conn);

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                cbEmpresa.DataSource = dt;
                cbEmpresa.DisplayMember = "Nome";
                cbEmpresa.ValueMember = "ClienteId";
                cbEmpresa.SelectedIndex = -1;
            }

            cbEmpresa.SelectedIndexChanged += async (s, e) =>
            {
                if (cbEmpresa.SelectedValue != null && cbEmpresa.SelectedIndex >= 0)
                {
                    await CarregarTicketsAsync(Convert.ToInt32(cbEmpresa.SelectedValue));
                    HabilitarCampos(false);
                }
                else
                {
                    cbTicket.DataSource = null;
                    LimparCampos();
                    HabilitarCampos(false);
                }
            };
        }

        private async Task CarregarTicketsAsync(int clienteId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "SELECT Ticket FROM ControleLogistico WHERE ClienteId=@ClienteId AND Ticket IS NOT NULL ORDER BY Ticket",
                    conn);
                cmd.Parameters.AddWithValue("@ClienteId", clienteId);

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                isBindingTickets = true;
                cbTicket.DataSource = dt;
                cbTicket.DisplayMember = "Ticket";
                cbTicket.ValueMember = "Ticket";

                if (dt.Rows.Count > 0)
                {
                    cbTicket.SelectedIndex = dt.Rows.Count - 1;

                    var ticketSelecionado = cbTicket.SelectedValue?.ToString();
                    if (!string.IsNullOrWhiteSpace(ticketSelecionado))
                    {
                        await CarregarLancamentosAsync(ticketSelecionado);
                    }
                }
                else
                {
                    cbTicket.SelectedIndex = -1;
                    MessageBox.Show("Esta empresa não possui tickets cadastrados.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                isBindingTickets = false;
            }
        }

        private void ConfigurarGrid()
        {
            dgvLancamentos.Columns.Clear();

            dgvLancamentos.DefaultCellStyle.ForeColor = Color.Black;
            dgvLancamentos.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgvLancamentos.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvLancamentos.AllowUserToAddRows = false;
            dgvLancamentos.AllowUserToDeleteRows = false;
            dgvLancamentos.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvLancamentos.AutoGenerateColumns = false;
            dgvLancamentos.Rows.Clear();

            var colMaterial = new DataGridViewComboBoxColumn
            {
                HeaderText = "Material",
                Name = "Material",
                ValueType = typeof(string),
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 40
            };
            dgvLancamentos.Columns.Add(colMaterial);

            var colPeso = new DataGridViewTextBoxColumn
            {
                HeaderText = "Peso (kg)",
                Name = "Peso",
                ValueType = typeof(decimal),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 25
            };
            colPeso.DefaultCellStyle.Format = "N3";
            dgvLancamentos.Columns.Add(colPeso);

            var colObservacoes = new DataGridViewTextBoxColumn
            {
                HeaderText = "Observações",
                Name = "Observacoes",
                ValueType = typeof(string),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 35
            };
            colObservacoes.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvLancamentos.Columns.Add(colObservacoes);

            dgvLancamentos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLancamentos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvLancamentos.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            AplicarEstiloVisualProducao(dgvLancamentos);
        }

        private async Task CarregarMateriaisComboBoxAsync()
        {
            try
            {
                var materiais = new System.Collections.Generic.List<string>();

                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(
                        "SELECT Nome FROM ExpMaterialLaudo ORDER BY Nome", conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            materiais.Add(reader["Nome"].ToString());
                    }
                }

                var colMaterial = (DataGridViewComboBoxColumn)dgvLancamentos.Columns["Material"];
                colMaterial.DataSource = materiais;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar lista de materiais: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCriar_Click(object sender, EventArgs e)
        {
            string ultimoMaterial = null;
            string ultimaObservacao = null;

            if (dgvLancamentos.Rows.Count > 0)
            {
                var ultimaLinha = dgvLancamentos.Rows[dgvLancamentos.Rows.Count - 1];
                if (ultimaLinha.Cells["Material"].Value != null)
                {
                    ultimoMaterial = ultimaLinha.Cells["Material"].Value.ToString();
                }
                if (ultimaLinha.Cells["Observacoes"].Value != null)
                {
                    ultimaObservacao = ultimaLinha.Cells["Observacoes"].Value.ToString();
                }
            }

            int novaLinhaIndex = dgvLancamentos.Rows.Add();

            if (!string.IsNullOrWhiteSpace(ultimoMaterial))
            {
                dgvLancamentos.Rows[novaLinhaIndex].Cells["Material"].Value = ultimoMaterial;
            }

            if (!string.IsNullOrWhiteSpace(ultimaObservacao))
            {
                dgvLancamentos.Rows[novaLinhaIndex].Cells["Observacoes"].Value = ultimaObservacao;
            }

            dgvLancamentos.CurrentCell = dgvLancamentos.Rows[novaLinhaIndex].Cells["Peso"];
        }

        private void dgvLancamentos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (dgvLancamentos.Columns[e.ColumnIndex].Name == "Material")
                {
                    var celulaAtual = dgvLancamentos.Rows[e.RowIndex].Cells["Material"];

                    if (celulaAtual.Value == null || string.IsNullOrWhiteSpace(celulaAtual.Value.ToString()))
                    {
                        string ultimoMaterial = null;

                        for (int i = e.RowIndex - 1; i >= 0; i--)
                        {
                            if (dgvLancamentos.Rows[i].Cells["Material"].Value != null)
                            {
                                ultimoMaterial = dgvLancamentos.Rows[i].Cells["Material"].Value.ToString();
                                break;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(ultimoMaterial))
                        {
                            celulaAtual.Value = ultimoMaterial;
                        }
                    }
                }
            }
        }

        private void dgvLancamentos_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (dgvLancamentos.Columns[e.ColumnIndex].Name == "Peso")
                {
                    var celulaMaterial = dgvLancamentos.Rows[e.RowIndex].Cells["Material"];

                    if (celulaMaterial.Value == null || string.IsNullOrWhiteSpace(celulaMaterial.Value.ToString()))
                    {
                        string ultimoMaterial = null;

                        for (int i = e.RowIndex - 1; i >= 0; i--)
                        {
                            if (dgvLancamentos.Rows[i].Cells["Material"].Value != null)
                            {
                                ultimoMaterial = dgvLancamentos.Rows[i].Cells["Material"].Value.ToString();
                                break;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(ultimoMaterial))
                        {
                            celulaMaterial.Value = ultimoMaterial;
                        }
                    }
                }
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvLancamentos.CurrentRow == null)
            {
                MessageBox.Show("Nenhuma linha selecionada.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvLancamentos.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Não é possível excluir a linha em branco.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Deseja realmente excluir esta linha?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                dgvLancamentos.Rows.Remove(dgvLancamentos.CurrentRow);
                AtualizarTotais();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos();
            cbEmpresa.SelectedIndex = -1;
            cbTicket.DataSource = null;
            HabilitarCampos(false);
        }

        private void LimparCampos()
        {
            txtNF.Clear();
            txtMTR.Clear();
            txtVolume.Clear();
            txtPeso.Clear();
            txtObs.Clear();
            dgvLancamentos.Rows.Clear();
            lblPeso.Text = "0,000";
            dtpDataLanca.Value = DateTime.Now;
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            if (cbTicket.SelectedValue == null)
            {
                MessageBox.Show("Selecione um ticket.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow row in dgvLancamentos.Rows)
            {
                if (row.IsNewRow) continue;

                if (row.Cells["Material"].Value == null ||
                    string.IsNullOrWhiteSpace(row.Cells["Material"].Value.ToString()))
                {
                    MessageBox.Show("Todas as linhas devem ter um material selecionado.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (row.Cells["Peso"].Value == null ||
                    !decimal.TryParse(row.Cells["Peso"].Value.ToString(), out var peso) ||
                    peso <= 0)
                {
                    MessageBox.Show("Todas as linhas devem ter um peso válido maior que zero.",
                        "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            string ticket = cbTicket.SelectedValue.ToString();
            int volume = dgvLancamentos.Rows.Count;
            decimal pesoTotal = dgvLancamentos.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow && r.Cells["Peso"].Value != null)
                .Sum(r => decimal.TryParse(r.Cells["Peso"].Value.ToString(), out var p) ? p : 0);

            btnSalvar.Enabled = false;
            btnCancelar.Enabled = false;
            Cursor = Cursors.WaitCursor;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            var cmdUpdate = new SqlCommand(@"
                                UPDATE ControleLogistico WITH (UPDLOCK, ROWLOCK)
                                SET NF = @NF, 
                                    MTR = @MTR, 
                                    Volume = @Volume, 
                                    Peso = @Peso, 
                                    Observacoes = @Obs
                                WHERE Ticket = @Ticket", conn, transaction);

                            cmdUpdate.Parameters.AddWithValue("@NF", string.IsNullOrWhiteSpace(txtNF.Text) ? (object)DBNull.Value : txtNF.Text);
                            cmdUpdate.Parameters.AddWithValue("@MTR", string.IsNullOrWhiteSpace(txtMTR.Text) ? (object)DBNull.Value : txtMTR.Text);
                            cmdUpdate.Parameters.AddWithValue("@Volume", volume);
                            cmdUpdate.Parameters.AddWithValue("@Peso", pesoTotal);
                            cmdUpdate.Parameters.AddWithValue("@Obs", string.IsNullOrWhiteSpace(txtObs.Text) ? (object)DBNull.Value : txtObs.Text);
                            cmdUpdate.Parameters.AddWithValue("@Ticket", ticket);

                            int affected = await cmdUpdate.ExecuteNonQueryAsync();

                            if (affected == 0)
                            {
                                throw new Exception("Ticket não encontrado ou foi modificado por outro usuário.");
                            }

                            var cmdDel = new SqlCommand("DELETE FROM LancamentosMateriais WHERE Ticket = @Ticket", conn, transaction);
                            cmdDel.Parameters.AddWithValue("@Ticket", ticket);
                            await cmdDel.ExecuteNonQueryAsync();

                            if (dgvLancamentos.Rows.Count > 0)
                            {
                                var dtMateriais = new DataTable();
                                dtMateriais.Columns.Add("Ticket", typeof(string));
                                dtMateriais.Columns.Add("Material", typeof(string));
                                dtMateriais.Columns.Add("Peso", typeof(decimal));
                                dtMateriais.Columns.Add("Observacoes", typeof(string));

                                foreach (DataGridViewRow row in dgvLancamentos.Rows)
                                {
                                    if (row.IsNewRow) continue;

                                    string material = row.Cells["Material"].Value?.ToString();
                                    if (string.IsNullOrWhiteSpace(material)) continue;

                                    decimal peso = 0;
                                    decimal.TryParse(row.Cells["Peso"].Value?.ToString(), out peso);

                                    string observacoes = row.Cells["Observacoes"].Value?.ToString() ?? string.Empty;

                                    dtMateriais.Rows.Add(ticket, material, peso, observacoes);
                                }

                                using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                                {
                                    bulkCopy.DestinationTableName = "LancamentosMateriais";
                                    bulkCopy.ColumnMappings.Add("Ticket", "Ticket");
                                    bulkCopy.ColumnMappings.Add("Material", "Material");
                                    bulkCopy.ColumnMappings.Add("Peso", "Peso");
                                    bulkCopy.ColumnMappings.Add("Observacoes", "Observacoes");
                                    bulkCopy.BulkCopyTimeout = 30;
                                    bulkCopy.BatchSize = 500;

                                    await bulkCopy.WriteToServerAsync(dtMateriais);
                                }
                            }

                            transaction.Commit();
                            MessageBox.Show("Lançamento salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show($"Erro ao salvar lançamento: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSalvar.Enabled = true;
                btnCancelar.Enabled = true;
                Cursor = Cursors.Default;
            }
        }


        private async Task CarregarLancamentosAsync(string ticket)
        {
            if (string.IsNullOrWhiteSpace(ticket)) return;

            isLoadingTicket = true;

            try
            {
                dgvLancamentos.Rows.Clear();

                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    var cmd = new SqlCommand(@"
                        SELECT 
                            cl.NF, 
                            cl.MTR, 
                            cl.Observacoes, 
                            cl.Data,
                            lm.Material,
                            lm.Peso,
                            lm.Observacoes as MaterialObservacoes  -- ADICIONE ESTA COLUNA
                        FROM ControleLogistico cl
                        LEFT JOIN LancamentosMateriais lm ON cl.Ticket = lm.Ticket
                        WHERE cl.Ticket = @Ticket
                        ORDER BY lm.Material", conn);

                    cmd.Parameters.AddWithValue("@Ticket", ticket);

                    bool headerLoaded = false;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!headerLoaded)
                            {
                                txtNF.Text = reader["NF"]?.ToString() ?? "";
                                txtMTR.Text = reader["MTR"]?.ToString() ?? "";
                                txtObs.Text = reader["Observacoes"]?.ToString() ?? "";

                                if (reader["Data"] != DBNull.Value)
                                {
                                    dtpDataLanca.Value = Convert.ToDateTime(reader["Data"]);
                                }
                                else
                                {
                                    dtpDataLanca.Value = DateTime.Now;
                                }

                                headerLoaded = true;
                            }

                            if (reader["Material"] != DBNull.Value)
                            {
                                dgvLancamentos.Rows.Add(
                                    reader["Material"].ToString(),
                                    Convert.ToDecimal(reader["Peso"]),
                                    reader["MaterialObservacoes"]?.ToString() ?? ""
                                );
                            }
                        }
                    }
                }

                AtualizarTotais();
                HabilitarCampos(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar lançamentos: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isLoadingTicket = false;
            }
        }


        private async void cbTicket_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadingTicket || isBindingTickets) return;

            if (cbTicket.SelectedValue != null && cbTicket.SelectedIndex >= 0)
            {
                await CarregarLancamentosAsync(cbTicket.SelectedValue.ToString());
            }
            else
            {
                LimparCampos();
                HabilitarCampos(false);
            }
        }

        private void dgvLancamentos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && !isLoadingTicket)
            {
                if (dgvLancamentos.Columns[e.ColumnIndex].Name == "Peso")
                {
                    AtualizarTotais();
                }
            }
        }

        private void dgvLancamentos_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (!isLoadingTicket)
            {
                AtualizarTotais();
            }
        }

        private void AtualizarTotais()
        {
            int volume = dgvLancamentos.Rows.Count;
            decimal pesoTotal = 0;

            foreach (DataGridViewRow row in dgvLancamentos.Rows)
            {
                if (row.IsNewRow) continue;

                if (row.Cells["Peso"].Value != null &&
                    decimal.TryParse(row.Cells["Peso"].Value.ToString(), out var p))
                {
                    pesoTotal += p;
                }
            }

            txtVolume.Text = volume.ToString();
            txtPeso.Text = pesoTotal.ToString("N3");
            lblPeso.Text = pesoTotal.ToString("N3") + " kg";
        }

    }
}