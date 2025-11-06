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

            cbTicket.SelectedIndexChanged += cbTicket_SelectedIndexChanged;
        }

        private async void FormLancamento_Load(object sender, EventArgs e)
        {
            await CarregarEmpresasAsync();
            ConfigurarGrid();

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

                cbTicket.DataSource = dt;
                cbTicket.DisplayMember = "Ticket";
                cbTicket.ValueMember = "Ticket";

                if (dt.Rows.Count > 0)
                {
                    cbTicket.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("Esta empresa não possui tickets cadastrados.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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
            dgvLancamentos.Rows.Clear();

            var colMaterial = new DataGridViewComboBoxColumn();
            colMaterial.HeaderText = "Material";
            colMaterial.Name = "Material";
            colMaterial.DataSource = new string[]
            {
                "FERRO", "PAPELÃO", "PLASTICO", "APARA", "INOX FERROSO", "INOX NÃO FERROSO",
                "ALUMINIO BLOCO LIMPO", "ALUMINIO BLOCO SUJO", "PLACA VERDE", "PLACA MARROM",
                "COBRE", "APARA AMARELA", "ISOPOR", "COBRE MISTO", "CAVACO DE FERRO", "CAVACO DE ALUMINIO",
                "CABO", "CABO MISTO", "PICADEIRA", "BIGBAG", "MOTOR", "TRANSFORMADOR", "RESISTENCIA",
                "SERVIDOR", "MAQUINAS", "TABLET", "CELULAR", "MÓDULO", "PAINEL ELETRICO", "EQUIPAMENTO MÉDICO",
                "SUCATA ELETRONICA", "MISTO", "VIDRO", "MADEIRA", "METAIS", "SUCATA VARIADA", "LATÃO",
                "RADIADORES", "BATERIA", "PILHA", "RAÇÃO / FRALDA / OUTROS", "ALIMENTO / BEBIDA / OUTROS",
                "CAVACO DE PLÁSTICO", "TECIDOS", "PAPEL", "LAMPADA", "RESIDUO INDUSTRIAL", "BORRACHA",
                "EVA (ETILENO ACETATO DE VINILA)"
            };
            colMaterial.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colMaterial.FillWeight = 60;
            dgvLancamentos.Columns.Add(colMaterial);

            var colPeso = new DataGridViewTextBoxColumn();
            colPeso.HeaderText = "Peso (kg)";
            colPeso.Name = "Peso";
            colPeso.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colPeso.FillWeight = 40;
            colPeso.DefaultCellStyle.Format = "N3";
            dgvLancamentos.Columns.Add(colPeso);

            dgvLancamentos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLancamentos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvLancamentos.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        }

        private void btnCriar_Click(object sender, EventArgs e)
        {
            dgvLancamentos.Rows.Add();
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
                MessageBox.Show("Selecione um ticket.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvLancamentos.Rows.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um material na grid.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow row in dgvLancamentos.Rows)
            {
                if (row.IsNewRow) continue;

                if (row.Cells["Material"].Value == null || string.IsNullOrWhiteSpace(row.Cells["Material"].Value.ToString()))
                {
                    MessageBox.Show("Todas as linhas devem ter um material selecionado.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (row.Cells["Peso"].Value == null || !decimal.TryParse(row.Cells["Peso"].Value.ToString(), out var peso) || peso <= 0)
                {
                    MessageBox.Show("Todas as linhas devem ter um peso válido maior que zero.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            string ticket = cbTicket.SelectedValue.ToString();

            int volume = dgvLancamentos.Rows.Count;
            decimal pesoTotal = dgvLancamentos.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow && r.Cells["Peso"].Value != null)
                .Sum(r => decimal.TryParse(r.Cells["Peso"].Value.ToString(), out var p) ? p : 0);

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            var cmdUpdate = new SqlCommand(@"
                                UPDATE ControleLogistico
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

                            await cmdUpdate.ExecuteNonQueryAsync();

                            var cmdDel = new SqlCommand("DELETE FROM LancamentosMateriais WHERE Ticket = @Ticket", conn, transaction);
                            cmdDel.Parameters.AddWithValue("@Ticket", ticket);
                            await cmdDel.ExecuteNonQueryAsync();

                            foreach (DataGridViewRow row in dgvLancamentos.Rows)
                            {
                                if (row.IsNewRow) continue;

                                string material = row.Cells["Material"].Value?.ToString();
                                if (string.IsNullOrWhiteSpace(material)) continue;

                                decimal peso = 0;
                                decimal.TryParse(row.Cells["Peso"].Value?.ToString(), out peso);

                                var cmdIns = new SqlCommand(@"
                                    INSERT INTO LancamentosMateriais (Ticket, Material, Peso)
                                    VALUES (@Ticket, @Material, @Peso)", conn, transaction);

                                cmdIns.Parameters.AddWithValue("@Ticket", ticket);
                                cmdIns.Parameters.AddWithValue("@Material", material);
                                cmdIns.Parameters.AddWithValue("@Peso", peso);
                                await cmdIns.ExecuteNonQueryAsync();
                            }

                            transaction.Commit();
                            MessageBox.Show("Lançamento salvo com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show($"Erro ao salvar lançamento: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    var cmdTicket = new SqlCommand(@"
                        SELECT NF, MTR, Observacoes, Data 
                        FROM ControleLogistico 
                        WHERE Ticket = @Ticket", conn);
                    cmdTicket.Parameters.AddWithValue("@Ticket", ticket);

                    using (var reader = await cmdTicket.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
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
                        }
                    }

                    var cmdMateriais = new SqlCommand(
                        "SELECT Material, Peso FROM LancamentosMateriais WHERE Ticket = @Ticket ORDER BY Material",
                        conn);
                    cmdMateriais.Parameters.AddWithValue("@Ticket", ticket);

                    using (var reader = await cmdMateriais.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            dgvLancamentos.Rows.Add(
                                reader["Material"].ToString(),
                                Convert.ToDecimal(reader["Peso"])
                            );
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
            if (isLoadingTicket) return;

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
            if (e.RowIndex >= 0 && !isLoadingTicket)
            {
                AtualizarTotais();
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