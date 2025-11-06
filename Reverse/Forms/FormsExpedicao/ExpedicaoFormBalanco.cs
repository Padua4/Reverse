using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp.Pdf;
using System.Reflection;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormBalanco : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private bool isLoadingData = false;
        private int clienteIdSelecionado = 0;

        private readonly Dictionary<string, (string Tipo, string Tratamento)> materiaisTipoTratamento = new Dictionary<string, (string, string)>
        {
            { "ALIMENTO / BEBIDA / OUTROS", ("ALIMENTO / BEBIDA / OUTROS", "COMPOSTAGEM") },
            { "ALUMINIO BLOCO LIMPO", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "ALUMINIO BLOCO SUJO", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "APARA", ("POLÍMEROS", "RECICLAGEM") },
            { "APARA AMARELA", ("POLÍMEROS", "RECICLAGEM") },
            { "BATERIA", ("BATERIAS / PILHAS", "RECICLAGEM") },
            { "BIGBAG", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "BORRACHA", ("BORRACHA", "RECICLAGEM") },
            { "CABO", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "CABO MISTO", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "CAVACO DE ALUMINIO", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "CAVACO DE FERRO", ("METÁLICO", "INDUSTRIALIZAÇÃO") },
            { "CAVACO DE PLÁSTICO", ("POLÍMEROS", "RECICLAGEM") },
            { "CELULAR", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "COBRE", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "COBRE MISTO", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "EQUIPAMENTO MÉDICO", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "EVA (ETILENO ACETATO DE VINILA)", ("POLÍMEROS", "RECICLAGEM") },
            { "FERRO", ("METÁLICO", "INDUSTRIALIZAÇÃO") },
            { "INOX FERROSO", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "INOX NÃO FERROSO", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "ISOPOR", ("ISOPOR", "RECICLAGEM") },
            { "LAMPADA", ("SÓLIDO AMORFO (VIDRO)", "RECICLAGEM") },
            { "LATÃO", ("METÁLICO", "INDUSTRIALIZAÇÃO") },
            { "MADEIRA", ("MADEIRA", "RECICLAGEM") },
            { "MAQUINAS", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "METAIS", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "MISTO", ("OUTRAS FRAÇÕES NÃO ESPECIFICADAS", "TRIAGEM, TRANSBORDO E COPROCESSAMENTO") },
            { "MÓDULO", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "MOTOR", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "PAPEL", ("PAPEL / PAPELÃO", "RECICLAGEM") },
            { "PAPELÃO", ("PAPEL / PAPELÃO", "RECICLAGEM") },
            { "PAINEL ELETRICO", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "PICADEIRA", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "PILHA", ("BATERIAS / PILHAS", "COPROCESSAMENTO") },
            { "PLACA MARROM", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "PLACA VERDE", ("METAL NOBRE", "INDUSTRIALIZAÇÃO") },
            { "PLASTICO", ("POLÍMEROS", "RECICLAGEM") },
            { "RADIADORES", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "RAÇÃO / FRALDA / OUTROS", ("RAÇÃO / FRALDA / OUTROS", "DOAÇÕES") },
            { "RESISTENCIA", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "RESIDUO INDUSTRIAL", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "SERVIDOR", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "SUCATA ELETRONICA", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "SUCATA VARIADA", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "TABLET", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "TECIDOS", ("SUCATA TÊXTIL", "COPROCESSAMENTO") },
            { "TRANSFORMADOR", ("NÃO ESPECIFICADO", "NÃO ESPECIFICADO") },
            { "VIDRO", ("SÓLIDO AMORFO (VIDRO)", "RECICLAGEM") }
        };

        public ExpedicaoFormBalanco(int _usuarioId)
        {
            InitializeComponent();
            this.Load += FormBalanco_Load;

            btnCarregar.Click += btnCarregar_Click;
            btnSalvar.Click += btnSalvar_Click;
            btnCancelar.Click += btnCancelar_Click;
            btnLancamentos.Click += btnLancamentos_Click;
            btnCertificado.Click += btnCertificado_Click;

            dgvBalanca.CellValueChanged += dgvBalanca_CellValueChanged;
        }


        private async void FormBalanco_Load(object sender, EventArgs e)
        {
            await CarregarEmpresasAsync();
            ConfigurarGrids();
            HabilitarCampos(false);

            dtpData.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpData.Format = DateTimePickerFormat.Custom;
            dtpData.CustomFormat = "MM/yyyy";
            dtpData.ShowUpDown = true;
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
        }

        private void ConfigurarGrids()
        {
            dgvTickets.Columns.Clear();
            dgvTickets.AllowUserToAddRows = false;
            dgvTickets.AllowUserToDeleteRows = false;
            dgvTickets.ReadOnly = true;
            dgvTickets.DefaultCellStyle.ForeColor = Color.Black;
            dgvTickets.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgvTickets.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvTickets.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dgvTickets.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            dgvTickets.RowsDefaultCellStyle.BackColor = Color.FromArgb(230, 240, 255);
            dgvTickets.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(200, 220, 240);

            var colTicket = new DataGridViewTextBoxColumn();
            colTicket.HeaderText = "Ticket";
            colTicket.Name = "Ticket";
            colTicket.Width = 120;
            colTicket.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvTickets.Columns.Add(colTicket);

            var colMaterialTicket = new DataGridViewTextBoxColumn();
            colMaterialTicket.HeaderText = "Material";
            colMaterialTicket.Name = "Material";
            colMaterialTicket.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colMaterialTicket.FillWeight = 150;
            dgvTickets.Columns.Add(colMaterialTicket);

            var colPesoTicket = new DataGridViewTextBoxColumn();
            colPesoTicket.HeaderText = "Peso (kg)";
            colPesoTicket.Name = "Peso";
            colPesoTicket.DefaultCellStyle.Format = "N3";
            colPesoTicket.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colPesoTicket.FillWeight = 80;
            dgvTickets.Columns.Add(colPesoTicket);

            var colVolume = new DataGridViewTextBoxColumn();
            colVolume.HeaderText = "Volume";
            colVolume.Name = "Volume";
            colVolume.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colVolume.FillWeight = 60;
            dgvTickets.Columns.Add(colVolume);

            dgvTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvBalanca.Columns.Clear();
            dgvBalanca.AllowUserToAddRows = false;
            dgvBalanca.AllowUserToDeleteRows = false;
            dgvBalanca.DefaultCellStyle.ForeColor = Color.Black;
            dgvBalanca.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgvBalanca.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvBalanca.RowsDefaultCellStyle.BackColor = Color.FromArgb(230, 240, 255);
            dgvBalanca.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(200, 220, 240);

            var colId = new DataGridViewTextBoxColumn();
            colId.HeaderText = "Id";
            colId.Name = "Id";
            colId.Visible = false;
            dgvBalanca.Columns.Add(colId);

            var colMaterialBal = new DataGridViewComboBoxColumn();
            colMaterialBal.HeaderText = "Material";
            colMaterialBal.Name = "Material";
            colMaterialBal.DataSource = materiaisTipoTratamento.Keys.OrderBy(k => k).ToArray();
            colMaterialBal.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colMaterialBal.FillWeight = 60;
            dgvBalanca.Columns.Add(colMaterialBal);

            var colPesoBal = new DataGridViewTextBoxColumn();
            colPesoBal.HeaderText = "Peso (kg)";
            colPesoBal.Name = "Peso";
            colPesoBal.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colPesoBal.FillWeight = 40;
            colPesoBal.DefaultCellStyle.Format = "N3";
            dgvBalanca.Columns.Add(colPesoBal);

            dgvBalanca.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBalanca.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvBalanca.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            dgvTotal.Columns.Clear();
            dgvTotal.AllowUserToAddRows = false;
            dgvTotal.AllowUserToDeleteRows = false;
            dgvTotal.ReadOnly = true;
            dgvTotal.DefaultCellStyle.ForeColor = Color.Black;
            dgvTotal.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgvTotal.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvTotal.RowsDefaultCellStyle.BackColor = Color.FromArgb(230, 240, 255);
            dgvTotal.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(200, 220, 240);

            var colTipoTotal = new DataGridViewTextBoxColumn();
            colTipoTotal.HeaderText = "Tipo";
            colTipoTotal.Name = "Tipo";
            colTipoTotal.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTipoTotal.FillWeight = 60;
            dgvTotal.Columns.Add(colTipoTotal);

            var colPesoTotal = new DataGridViewTextBoxColumn();
            colPesoTotal.HeaderText = "Peso Total (kg)";
            colPesoTotal.Name = "PesoTotal";
            colPesoTotal.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colPesoTotal.FillWeight = 40;
            colPesoTotal.DefaultCellStyle.Format = "N3";
            colPesoTotal.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            colPesoTotal.DefaultCellStyle.BackColor = Color.LightYellow;
            dgvTotal.Columns.Add(colPesoTotal);

            dgvTotal.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTotal.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvTotal.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        }

        private void HabilitarCampos(bool habilitar)
        {
            btnCarregar.Enabled = !habilitar;
            cbEmpresa.Enabled = !habilitar;
            dtpData.Enabled = !habilitar;

            btnSalvar.Enabled = habilitar;
            btnCancelar.Enabled = habilitar;
            btnCriarLinha.Enabled = habilitar;
            btnExcluirLinha.Enabled = habilitar;
            dgvBalanca.Enabled = habilitar;
        }

        private async void btnCarregar_Click(object sender, EventArgs e)
        {
            if (cbEmpresa.SelectedValue == null)
            {
                MessageBox.Show("Selecione uma empresa.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clienteIdSelecionado = Convert.ToInt32(cbEmpresa.SelectedValue);
            DateTime mesAno = new DateTime(dtpData.Value.Year, dtpData.Value.Month, 1);

            await CarregarTicketsDoMesAsync(clienteIdSelecionado, mesAno);
            await CarregarBalancoSalvoAsync(clienteIdSelecionado, mesAno);

            HabilitarCampos(true);
        }

        private async Task CarregarTicketsDoMesAsync(int clienteId, DateTime mesAno)
        {
            dgvTickets.Rows.Clear();

            DateTime inicioMes = mesAno;
            DateTime fimMes = mesAno.AddMonths(1).AddDays(-1);

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
            SELECT 
                cl.Ticket,
                ISNULL(cl.Volume, 0) AS Volume,
                lm.Material,
                lm.Peso
                FROM ControleLogistico cl
                INNER JOIN LancamentosMateriais lm ON cl.Ticket = lm.Ticket
                WHERE cl.ClienteId = @ClienteId
                  AND cl.Ticket IS NOT NULL
                  AND cl.Data >= @InicioMes
                  AND cl.Data <= @FimMes
                ORDER BY cl.Ticket, lm.Material";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClienteId", clienteId);
                cmd.Parameters.AddWithValue("@InicioMes", inicioMes);
                cmd.Parameters.AddWithValue("@FimMes", fimMes);

                decimal pesoTotalGeral = 0;
                int volumeTotalGeral = 0;
                string ticketAnterior = "";

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string ticket = reader["Ticket"].ToString();
                        string material = reader["Material"].ToString();
                        decimal peso = Convert.ToDecimal(reader["Peso"]);
                        int volume = Convert.ToInt32(reader["Volume"]);

                        if (ticket != ticketAnterior)
                        {
                            volumeTotalGeral += volume;
                            ticketAnterior = ticket;
                        }

                        pesoTotalGeral += peso;
                        dgvTickets.Rows.Add(ticket, material, peso, volume);
                    }
                }

                txtPeso.Text = pesoTotalGeral.ToString("N3");
                txtVolume.Text = volumeTotalGeral.ToString();
                AtualizarPesoRestante();

                if (dgvTickets.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Nenhum ticket com materiais lançados foi encontrado neste período.\n\n" +
                        "Por favor, faça os lançamentos primeiro no formulário de Lançamentos.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private async Task CarregarBalancoSalvoAsync(int clienteId, DateTime mesAno)
        {
            dgvBalanca.Rows.Clear();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                    SELECT Id, Material, Peso, Tipo
                    FROM BalancoMassa
                    WHERE ClienteId = @ClienteId
                      AND MesAno = @MesAno
                    ORDER BY Material";
                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClienteId", clienteId);
                cmd.Parameters.AddWithValue("@MesAno", mesAno);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string material = reader["Material"].ToString();
                        decimal peso = Convert.ToDecimal(reader["Peso"]);

                        if (!materiaisTipoTratamento.ContainsKey(material))
                            material = "";

                        dgvBalanca.Rows.Add(id, material, peso);
                    }
                }
            }

            if (dgvBalanca.Rows.Count > 0)
            {
                AgruparPorTipo();
            }
        }

        private void btnCriarLinha_Click(object sender, EventArgs e)
        {
            dgvBalanca.Rows.Add(null, "", 0);
        }
        private async void btnExcluirLinha_Click(object sender, EventArgs e)
        {
            if (dgvBalanca.CurrentRow == null)
            {
                MessageBox.Show("Nenhuma linha selecionada.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvBalanca.CurrentRow;

            if (row.IsNewRow ||
                (row.Cells["Id"].Value == null &&
                 string.IsNullOrWhiteSpace(row.Cells["Material"].Value?.ToString()) &&
                 string.IsNullOrWhiteSpace(row.Cells["Peso"].Value?.ToString())))
            {
                dgvBalanca.Rows.Remove(row);
                AgruparPorTipo();
                return;
            }

            if (row.Cells["Id"].Value == null || row.Cells["Id"].Value == DBNull.Value)
            {
                dgvBalanca.Rows.Remove(row);
                AgruparPorTipo();
                return;
            }

            var confirm = MessageBox.Show("Deseja realmente excluir esta linha também do banco de dados?",
                "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                int id = Convert.ToInt32(row.Cells["Id"].Value);

                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    var cmd = new SqlCommand("DELETE FROM BalancoMassa WHERE Id = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows > 0)
                    {
                        dgvBalanca.Rows.Remove(row);
                        AgruparPorTipo();
                        MessageBox.Show("Linha excluída com sucesso do banco e da grade.",
                            "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nenhum registro correspondente foi encontrado no banco.",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvBalanca_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isLoadingData || e.RowIndex < 0) return;

            if (e.ColumnIndex == dgvBalanca.Columns["Material"].Index ||
                e.ColumnIndex == dgvBalanca.Columns["Peso"].Index)
            {
                AgruparPorTipo();
            }
        }


        private void AgruparPorTipo()
        {
            isLoadingData = true;

            var agrupamento = new Dictionary<string, decimal>();

            foreach (DataGridViewRow row in dgvBalanca.Rows)
            {
                if (row.IsNewRow) continue;

                var material = row.Cells["Material"].Value?.ToString();
                var pesoStr = row.Cells["Peso"].Value?.ToString();

                if (string.IsNullOrWhiteSpace(material)) continue;
                if (!decimal.TryParse(pesoStr, out var peso)) continue;

                if (materiaisTipoTratamento.ContainsKey(material))
                {
                    var tipo = materiaisTipoTratamento[material].Tipo;

                    if (agrupamento.ContainsKey(tipo))
                        agrupamento[tipo] += peso;
                    else
                        agrupamento[tipo] = peso;
                }
            }

            dgvTotal.Rows.Clear();
            foreach (var kvp in agrupamento)
            {
                dgvTotal.Rows.Add(kvp.Key, kvp.Value);
            }
            AtualizarPesoRestante();
            isLoadingData = false;
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            if (clienteIdSelecionado == 0)
            {
                MessageBox.Show("Nenhuma empresa selecionada.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvBalanca.Rows.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um material no balanço.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow row in dgvBalanca.Rows)
            {
                if (row.IsNewRow) continue;

                if (row.Cells["Material"].Value == null || string.IsNullOrWhiteSpace(row.Cells["Material"].Value.ToString()))
                {
                    MessageBox.Show("Todas as linhas devem ter um material selecionado.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(row.Cells["Peso"].Value?.ToString(), out var peso) || peso <= 0)
                {
                    MessageBox.Show("Todas as linhas devem ter um peso válido maior que zero.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            DateTime mesAno = new DateTime(dtpData.Value.Year, dtpData.Value.Month, 1);

            var dadosParaInserir = new List<(string Material, decimal Peso, string Tipo, string Tratamento)>();

            foreach (DataGridViewRow row in dgvBalanca.Rows)
            {
                if (row.IsNewRow) continue;

                string material = row.Cells["Material"].Value.ToString();
                decimal peso = Convert.ToDecimal(row.Cells["Peso"].Value);
                string tipo = materiaisTipoTratamento.ContainsKey(material)
                    ? materiaisTipoTratamento[material].Tipo
                    : "NÃO ESPECIFICADO";
                string tratamento = materiaisTipoTratamento.ContainsKey(material)
                    ? materiaisTipoTratamento[material].Tratamento
                    : "NÃO ESPECIFICADO";

                dadosParaInserir.Add((material, peso, tipo, tratamento));
            }

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            var cmdDel = new SqlCommand(@"
                        DELETE FROM BalancoMassa 
                        WHERE ClienteId = @ClienteId AND MesAno = @MesAno",
                                conn, transaction);
                            cmdDel.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);
                            cmdDel.Parameters.AddWithValue("@MesAno", mesAno);
                            await cmdDel.ExecuteNonQueryAsync();

                            if (dadosParaInserir.Count > 0)
                            {
                                const int batchSize = 100;
                                for (int i = 0; i < dadosParaInserir.Count; i += batchSize)
                                {
                                    var batch = dadosParaInserir.Skip(i).Take(batchSize).ToList();

                                    var valores = new List<string>();
                                    var cmdIns = new SqlCommand { Connection = conn, Transaction = transaction };

                                    for (int j = 0; j < batch.Count; j++)
                                    {
                                        var item = batch[j];
                                        valores.Add($"(@ClienteId, @MesAno, @Material{j}, @Peso{j}, @Tipo{j}, @Tratamento{j})");

                                        cmdIns.Parameters.AddWithValue($"@Material{j}", item.Material);
                                        cmdIns.Parameters.AddWithValue($"@Peso{j}", item.Peso);
                                        cmdIns.Parameters.AddWithValue($"@Tipo{j}", item.Tipo);
                                        cmdIns.Parameters.AddWithValue($"@Tratamento{j}", item.Tratamento);
                                    }

                                    cmdIns.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);
                                    cmdIns.Parameters.AddWithValue("@MesAno", mesAno);

                                    cmdIns.CommandText = $@"
                                INSERT INTO BalancoMassa (ClienteId, MesAno, Material, Peso, Tipo, Tratamento)
                                VALUES {string.Join(",", valores)}";

                                    await cmdIns.ExecuteNonQueryAsync();
                                }
                            }

                            transaction.Commit();
                            MessageBox.Show("Balanço de massa salvo com sucesso!", "Sucesso",
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
                MessageBox.Show($"Erro ao salvar balanço: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarPesoRestante()
        {
            decimal pesoTotalTickets = 0;
            decimal.TryParse(txtPeso.Text, out pesoTotalTickets);

            decimal pesoLancado = dgvBalanca.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow && r.Cells["Peso"].Value != null)
                .Sum(r => decimal.TryParse(r.Cells["Peso"].Value.ToString(), out var p) ? p : 0);

            decimal pesoRestante = pesoTotalTickets - pesoLancado;

            lblPesoRestante.Text = $"Peso Restante: {pesoRestante:N3} kg";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            dgvTickets.Rows.Clear();
            lblPesoRestante.Text = "Peso Restante: 0,000 kg";
            dgvBalanca.Rows.Clear();
            dgvTotal.Rows.Clear();
            txtPeso.Clear();
            txtVolume.Clear();
            cbEmpresa.SelectedIndex = -1;
            clienteIdSelecionado = 0;
            HabilitarCampos(false);
        }

        private class DadosCertificado
        {
            public string NomeCliente { get; set; }
            public string CNPJCliente { get; set; }
            public string CodigoEmpresa { get; set; }
            public decimal PesoTotal { get; set; }
            public List<string> Ticket { get; set; }
            public List<string> NF { get; set; }
            public List<string> MTR { get; set; }
            public DateTime MesAno { get; set; }
            public string NumeroCertificado { get; set; }
            public int CertificadoSequencialGeral { get; set; }
            public int CertificadoSequencialAnoAtual { get; set; }
            public int CertificadoUltimoAno { get; set; }
        }

        private bool linhaJaTemCertificado = false;
        private bool isPrimeiraVezCertificado = false;

        private async void btnCertificado_Click(object sender, EventArgs e)
        {
            if (clienteIdSelecionado == 0)
            {
                MessageBox.Show("Carregue os dados de uma empresa primeiro.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvBalanca.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Não há balanço de massa cadastrado.\n\n" +
                    "Por favor, faça o balanço de massa e salve antes de gerar o certificado.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var dadosCertificado = await ObterDadosCertificadoAsync();
            if (dadosCertificado == null) return;

            linhaJaTemCertificado = await VerificarCertificadoExistenteAsync(dadosCertificado);

            if (!linhaJaTemCertificado)
            {
                isPrimeiraVezCertificado = (dadosCertificado.CertificadoSequencialGeral == 0);

                if (isPrimeiraVezCertificado)
                {
                    string certificadoInicial = SolicitarCertificadoInicial(dadosCertificado.CodigoEmpresa);
                    if (string.IsNullOrEmpty(certificadoInicial))
                    {
                        return;
                    }

                    if (!ValidarFormatoCertificado(certificadoInicial))
                    {
                        MessageBox.Show("Formato inválido. Use: XXXX-YYY/SSSS-ZZZ\nExemplo: 0100-022/2025-005",
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    dadosCertificado.NumeroCertificado = certificadoInicial;
                    await AtualizarSequenciaisCertificado(dadosCertificado, certificadoInicial);
                }
                else
                {
                    await IncrementarSequenciaisCertificado(dadosCertificado);
                }
            }
            else
            {
                MessageBox.Show(
                    $"Já existe um certificado para este período:\n{dadosCertificado.NumeroCertificado}\n\n" +
                    "O PDF será regenerado com o mesmo número.",
                    "Certificado Existente",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            try
            {
                string caminhoArquivo = GerarPDFCertificado(dadosCertificado);

                var resultado = MessageBox.Show(
                    $"Certificado gerado com sucesso!\n\n{caminhoArquivo}\n\nDeseja abrir o arquivo?",
                    "Sucesso",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (resultado == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(caminhoArquivo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar certificado: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<bool> VerificarCertificadoExistenteAsync(DadosCertificado dados)
        {
            DateTime mesAno = dados.MesAno;

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
            SELECT TOP 1 NumeroCertificado
            FROM CertificadosEmitidos
            WHERE ClienteId = @ClienteId
              AND MesAno = @MesAno";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);
                cmd.Parameters.AddWithValue("@MesAno", mesAno);

                var resultado = await cmd.ExecuteScalarAsync();

                if (resultado != null && resultado != DBNull.Value)
                {
                    dados.NumeroCertificado = resultado.ToString();
                    return true;
                }

                return false;
            }
        }

        private string SolicitarCertificadoInicial(string codigoEmpresa)
        {
            using (Form promptForm = new Form())
            {
                promptForm.Width = 450;
                promptForm.Height = 280;
                promptForm.Text = "Primeira Geração de Certificado";
                promptForm.StartPosition = FormStartPosition.CenterParent;
                promptForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                promptForm.MaximizeBox = false;
                promptForm.MinimizeBox = false;

                Label textLabel = new Label()
                {
                    Left = 20,
                    Top = 20,
                    Width = 400,
                    Height = 80,
                    Text = "Este é o primeiro certificado deste cliente no sistema.\n\n" +
                           "Informe o número atual conforme o formato abaixo.\n\n" +
                           "Formato: XXXX-YYY/SSSS-ZZZ\n" +
                           "Exemplo: 0100-022/2025-005"
                };

                Label labelFormato = new Label()
                {
                    Left = 20,
                    Top = 110,
                    Width = 400,
                    Height = 60,
                    Text = "XXXX = Código da empresa (4 dígitos)\n" +
                           "YYY = Sequência geral de todos os certificados (001-999)\n" +
                           "SSSS = Ano atual\n" +
                           "ZZZ = Sequência do ano (reinicia a cada ano)"
                };

                string codigo4 = (codigoEmpresa ?? "").Trim();
                if (int.TryParse(codigo4, out var codNum))
                    codigo4 = codNum.ToString("D4");
                else
                    codigo4 = codigo4.PadLeft(4, '0');

                TextBox textBox = new TextBox()
                {
                    Left = 20,
                    Top = 180,
                    Width = 400,
                    Text = $"{codigo4}-001/{DateTime.Now.Year}-001"
                };

                Button confirmation = new Button()
                {
                    Text = "OK",
                    Left = 250,
                    Width = 80,
                    Top = 215,
                    DialogResult = DialogResult.OK
                };

                Button cancelButton = new Button()
                {
                    Text = "Cancelar",
                    Left = 340,
                    Width = 80,
                    Top = 215,
                    DialogResult = DialogResult.Cancel
                };

                confirmation.Click += (sender2, e2) => { promptForm.Close(); };
                cancelButton.Click += (sender2, e2) => { promptForm.Close(); };

                promptForm.Controls.Add(textLabel);
                promptForm.Controls.Add(labelFormato);
                promptForm.Controls.Add(textBox);
                promptForm.Controls.Add(confirmation);
                promptForm.Controls.Add(cancelButton);
                promptForm.AcceptButton = confirmation;
                promptForm.CancelButton = cancelButton;

                if (promptForm.ShowDialog() == DialogResult.OK)
                {
                    return textBox.Text.Trim();
                }

                return null;
            }
        }
        private bool ValidarFormatoCertificado(string certificado)
        {
            var partes = certificado.Split('-');
            if (partes.Length != 3) return false;

            var codigo = partes[0];
            var meio = partes[1];
            var seqAno = partes[2];

            if (codigo.Length != 4 || !int.TryParse(codigo, out _)) return false;

            var partesMeio = meio.Split('/');
            if (partesMeio.Length != 2) return false;

            var seqGeral = partesMeio[0];
            var ano = partesMeio[1];

            if (seqGeral.Length != 3 || !int.TryParse(seqGeral, out var seqGeralVal)) return false;
            if (ano.Length != 4 || !int.TryParse(ano, out var anoVal)) return false;
            if (seqAno.Length != 3 || !int.TryParse(seqAno, out var seqAnoVal)) return false;

            if (seqGeralVal < 1 || seqGeralVal > 999) return false;
            if (seqAnoVal < 1 || seqAnoVal > 999) return false;

            return true;
        }

        private async Task AtualizarSequenciaisCertificado(DadosCertificado dados, string certificadoManual)
        {
            var partes = certificadoManual.Split('-');
            var codigo = partes[0];
            var meio = partes[1];
            var seqAno = partes[2];

            var partesMeio = meio.Split('/');
            int seqGeral = int.Parse(partesMeio[0]);
            int ano = int.Parse(partesMeio[1]);
            int seqAnoAtual = int.Parse(seqAno);

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var cmdCliente = new SqlCommand(@"
                    UPDATE Clientes 
                    SET CertificadoSequencialGeral = @SeqGeral,
                        CertificadoSequencialAnoAtual = @SeqAnoAtual,
                        CertificadoUltimoAno = @Ano
                    WHERE ClienteId = @ClienteId", conn, transaction);

                        cmdCliente.Parameters.AddWithValue("@SeqGeral", seqGeral);
                        cmdCliente.Parameters.AddWithValue("@SeqAnoAtual", seqAnoAtual);
                        cmdCliente.Parameters.AddWithValue("@Ano", ano);
                        cmdCliente.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);

                        int linhasAfetadas = await cmdCliente.ExecuteNonQueryAsync();

                        if (linhasAfetadas == 0)
                        {
                            throw new Exception($"Cliente {clienteIdSelecionado} não foi encontrado para atualizar sequenciais!");
                        }

                        var cmdCertificado = new SqlCommand(@"
                    INSERT INTO CertificadosEmitidos (ClienteId, MesAno, NumeroCertificado, DataEmissao)
                    VALUES (@ClienteId, @MesAno, @NumeroCertificado, GETDATE())", conn, transaction);

                        cmdCertificado.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);
                        cmdCertificado.Parameters.AddWithValue("@MesAno", dados.MesAno);
                        cmdCertificado.Parameters.AddWithValue("@NumeroCertificado", certificadoManual);

                        await cmdCertificado.ExecuteNonQueryAsync();

                        transaction.Commit();

                        dados.CertificadoSequencialGeral = seqGeral;
                        dados.CertificadoSequencialAnoAtual = seqAnoAtual;
                        dados.CertificadoUltimoAno = ano;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private async Task IncrementarSequenciaisCertificado(DadosCertificado dados)
        {
            int novoSeqGeral = dados.CertificadoSequencialGeral + 1;
            int novoSeqAno = dados.CertificadoSequencialAnoAtual + 1;
            int anoAtual = DateTime.Now.Year;

            if (dados.CertificadoUltimoAno != anoAtual)
            {
                novoSeqAno = 1;
            }

            if (novoSeqGeral > 999)
            {
                novoSeqGeral = 1;
            }

            string codigo4 = (dados.CodigoEmpresa ?? "").Trim();
            if (int.TryParse(codigo4, out var codNum))
                codigo4 = codNum.ToString("D4");
            else
                codigo4 = codigo4.PadLeft(4, '0');

            string novoCertificado = $"{codigo4}-{novoSeqGeral:D3}/{anoAtual}-{novoSeqAno:D3}";

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        var cmdLer = new SqlCommand(@"
                    SELECT CertificadoSequencialGeral, CertificadoSequencialAnoAtual, CertificadoUltimoAno
                    FROM Clientes WITH (UPDLOCK, ROWLOCK)
                    WHERE ClienteId = @ClienteId", conn, transaction);
                        cmdLer.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);

                        using (var reader = await cmdLer.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int seqGeralAtual = Convert.ToInt32(reader["CertificadoSequencialGeral"]);
                                int seqAnoAtual = Convert.ToInt32(reader["CertificadoSequencialAnoAtual"]);
                                int ultimoAno = Convert.ToInt32(reader["CertificadoUltimoAno"]);

                                // Recalcular com valores atualizados
                                novoSeqGeral = seqGeralAtual + 1;
                                novoSeqAno = seqAnoAtual + 1;

                                if (ultimoAno != anoAtual)
                                {
                                    novoSeqAno = 1;
                                }

                                if (novoSeqGeral > 999)
                                {
                                    novoSeqGeral = 1;
                                }

                                novoCertificado = $"{codigo4}-{novoSeqGeral:D3}/{anoAtual}-{novoSeqAno:D3}";
                            }
                        }

                        var cmdCliente = new SqlCommand(@"
                    UPDATE Clientes 
                    SET CertificadoSequencialGeral = @SeqGeral,
                        CertificadoSequencialAnoAtual = @SeqAnoAtual,
                        CertificadoUltimoAno = @Ano
                    WHERE ClienteId = @ClienteId", conn, transaction);

                        cmdCliente.Parameters.AddWithValue("@SeqGeral", novoSeqGeral);
                        cmdCliente.Parameters.AddWithValue("@SeqAnoAtual", novoSeqAno);
                        cmdCliente.Parameters.AddWithValue("@Ano", anoAtual);
                        cmdCliente.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);

                        int linhasAfetadas = await cmdCliente.ExecuteNonQueryAsync();

                        if (linhasAfetadas == 0)
                        {
                            throw new Exception($"Cliente {clienteIdSelecionado} não foi encontrado para atualizar sequenciais!");
                        }

                        var cmdCertificado = new SqlCommand(@"
                    INSERT INTO CertificadosEmitidos (ClienteId, MesAno, NumeroCertificado, DataEmissao)
                    VALUES (@ClienteId, @MesAno, @NumeroCertificado, GETDATE())", conn, transaction);

                        cmdCertificado.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);
                        cmdCertificado.Parameters.AddWithValue("@MesAno", dados.MesAno);
                        cmdCertificado.Parameters.AddWithValue("@NumeroCertificado", novoCertificado);

                        await cmdCertificado.ExecuteNonQueryAsync();

                        transaction.Commit();

                        dados.NumeroCertificado = novoCertificado;
                        dados.CertificadoSequencialGeral = novoSeqGeral;
                        dados.CertificadoSequencialAnoAtual = novoSeqAno;
                        dados.CertificadoUltimoAno = anoAtual;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private async Task<DadosCertificado> ObterDadosCertificadoAsync()
        {
            DateTime mesAno = new DateTime(dtpData.Value.Year, dtpData.Value.Month, 1);
            DateTime inicioMes = mesAno;
            DateTime fimMes = mesAno.AddMonths(1).AddDays(-1);

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sqlCliente = @"
            SELECT Nome, CPF_CNPJ, CodigoEmpresa,
                   ISNULL(CertificadoSequencialGeral, 0) AS CertificadoSequencialGeral,
                   ISNULL(CertificadoSequencialAnoAtual, 0) AS CertificadoSequencialAnoAtual,
                   ISNULL(CertificadoUltimoAno, YEAR(GETDATE())) AS CertificadoUltimoAno
            FROM Clientes
            WHERE ClienteId = @ClienteId";

                var cmdCliente = new SqlCommand(sqlCliente, conn);
                cmdCliente.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);

                var dados = new DadosCertificado();

                using (var reader = await cmdCliente.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        dados.NomeCliente = reader["Nome"].ToString();
                        dados.CNPJCliente = reader["CPF_CNPJ"]?.ToString() ?? "";
                        dados.CodigoEmpresa = reader["CodigoEmpresa"]?.ToString() ?? "000";
                        dados.CertificadoSequencialGeral = reader["CertificadoSequencialGeral"] != DBNull.Value
                            ? Convert.ToInt32(reader["CertificadoSequencialGeral"])
                            : 0;
                        dados.CertificadoSequencialAnoAtual = reader["CertificadoSequencialAnoAtual"] != DBNull.Value
                            ? Convert.ToInt32(reader["CertificadoSequencialAnoAtual"])
                            : 0;
                        dados.CertificadoUltimoAno = reader["CertificadoUltimoAno"] != DBNull.Value
                            ? Convert.ToInt32(reader["CertificadoUltimoAno"])
                            : DateTime.Now.Year;
                    }
                    else
                    {
                        MessageBox.Show("Cliente não encontrado.", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }

                string sqlTickets = @"
                    SELECT DISTINCT Ticket
                    FROM ControleLogistico
                    WHERE ClienteId = @ClienteId
                      AND Ticket IS NOT NULL
                      AND Data >= @InicioMes
                      AND Data <= @FimMes
                    ORDER BY Ticket";

                var cmdTickets = new SqlCommand(sqlTickets, conn);
                cmdTickets.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);
                cmdTickets.Parameters.AddWithValue("@InicioMes", inicioMes);
                cmdTickets.Parameters.AddWithValue("@FimMes", fimMes);

                var tickets = new List<string>();
                using (var reader = await cmdTickets.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tickets.Add(reader["Ticket"].ToString());
                    }
                }

                if (tickets.Count == 0)
                {
                    MessageBox.Show("Nenhum ticket encontrado para este período.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
                }

                dados.Ticket = tickets;

                string sqlBalanco = @"
                    SELECT SUM(Peso) AS PesoTotal
                    FROM BalancoMassa
                    WHERE ClienteId = @ClienteId
                    AND MesAno = @MesAno";

                var cmdBalanco = new SqlCommand(sqlBalanco, conn);
                cmdBalanco.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);
                cmdBalanco.Parameters.AddWithValue("@MesAno", mesAno);

                var pesoTotal = await cmdBalanco.ExecuteScalarAsync();
                dados.PesoTotal = pesoTotal != DBNull.Value ? Convert.ToDecimal(pesoTotal) : 0;

                if (dados.PesoTotal == 0)
                {
                    MessageBox.Show(
                        "Peso total zerado no balanço de massa.\n\n" +
                        "Verifique se o balanço está correto.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return null;
                }

                string sqlNFe = @"
                    SELECT DISTINCT NF
                    FROM ControleLogistico
                    WHERE ClienteId = @ClienteId
                      AND Ticket IS NOT NULL
                      AND NF IS NOT NULL
                      AND NF <> ''
                      AND Data >= @InicioMes
                      AND Data <= @FimMes
                    ORDER BY NF";

                var cmdNFe = new SqlCommand(sqlNFe, conn);
                cmdNFe.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);
                cmdNFe.Parameters.AddWithValue("@InicioMes", inicioMes);
                cmdNFe.Parameters.AddWithValue("@FimMes", fimMes);

                var nfes = new List<string>();
                using (var reader = await cmdNFe.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        nfes.Add(reader["NF"].ToString());
                    }
                }
                dados.NF = nfes;

                string sqlMTR = @"
                    SELECT DISTINCT MTR
                    FROM ControleLogistico
                    WHERE ClienteId = @ClienteId
                      AND Ticket IS NOT NULL
                      AND MTR IS NOT NULL
                      AND MTR <> ''
                      AND Data >= @InicioMes
                      AND Data <= @FimMes
                    ORDER BY MTR";

                var cmdMTR = new SqlCommand(sqlMTR, conn);
                cmdMTR.Parameters.AddWithValue("@ClienteId", clienteIdSelecionado);
                cmdMTR.Parameters.AddWithValue("@InicioMes", inicioMes);
                cmdMTR.Parameters.AddWithValue("@FimMes", fimMes);

                var mtrs = new List<string>();
                using (var reader = await cmdMTR.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        mtrs.Add(reader["MTR"].ToString());
                    }
                }
                dados.MTR = mtrs;
                dados.MesAno = mesAno;

                return dados;
            }
        }

        private string GerarPDFCertificado(DadosCertificado dados)
        { 
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            page.Orientation = PdfSharp.PageOrientation.Landscape;

            XGraphics gfx = XGraphics.FromPdfPage(page);

            double pageWidth = page.Width.Point;
            double pageHeight = page.Height.Point;

            XFont fontTexto = new XFont("Times New Roman", 14.5, XFontStyleEx.Regular);
            XFont fontNegrito = new XFont("Times New Roman", 14.5, XFontStyleEx.Bold);

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("Reverse.Resources.CertificadoTemplate.png"))
            {
                if (stream != null)
                {
                    using (XImage template = XImage.FromStream(stream))
                    {
                        gfx.DrawImage(template, 0, 0, pageWidth, pageHeight);
                    }
                }
                else
                {
                    MessageBox.Show("Template não encontrado no recurso embutido.");
                }
            }

            double textoX = 155;
            double textoY = 150;
            double largura = pageWidth - textoX - 10;

            string cnpjFormatado = dados.CNPJCliente;
            if (!string.IsNullOrWhiteSpace(cnpjFormatado) && cnpjFormatado.Length == 14)
            {
                cnpjFormatado = $"{cnpjFormatado.Substring(0, 2)}.{cnpjFormatado.Substring(2, 3)}.{cnpjFormatado.Substring(5, 3)}/{cnpjFormatado.Substring(8, 4)}-{cnpjFormatado.Substring(12, 2)}";
            }

            var cultura = new System.Globalization.CultureInfo("pt-BR");
            string mesExtenso = cultura.DateTimeFormat.GetMonthName(dados.MesAno.Month).ToUpper();
            string mesAnoFormatado = $"{mesExtenso}/{dados.MesAno.Year}";

                  var fragmentos = new List<(string texto, XFont fonte)>
                {
                    ("Certificamos que a empresa ", fontTexto),
                    (dados.NomeCliente.ToUpper(), fontNegrito),
                    (", inscrita no CNPJ nº ", fontTexto),
                    (cnpjFormatado, fontNegrito),
                    (", encaminhou à empresa DLD Soluções em Logística Reversa, Gestão e Reciclagem LTDA, no mês de ", fontTexto),
                    (mesAnoFormatado, fontNegrito),
                    (" a quantidade de ", fontTexto),
                    ($"{dados.PesoTotal:N3} kg", fontNegrito),
                    (" de descartes obsoletos.", fontTexto)
                };

            double currentY = textoY;
            DesenharTextoJustificado(gfx, fragmentos, textoX, currentY, largura, 20, fontTexto);

            double blocoY = textoY + 140;

            string labelCertificado = "Certificado: ";
            XSize sizeLabelCert = gfx.MeasureString(labelCertificado, fontTexto);
            gfx.DrawString(labelCertificado, fontTexto, XBrushes.Black, textoX, blocoY);
            gfx.DrawString(dados.NumeroCertificado, fontNegrito, XBrushes.Black, textoX + sizeLabelCert.Width, blocoY);

            string labelTicket = "Ticket: ";
            XSize sizeLabelTicket = gfx.MeasureString(labelTicket, fontTexto);
            gfx.DrawString(labelTicket, fontTexto, XBrushes.Black, textoX, blocoY + 25);

            List<string> ticketsStr = dados.Ticket ?? new List<string>();
            double ticketX = textoX + sizeLabelTicket.Width;
            double ticketY = blocoY + 25;
            double larguraDisponivel = largura - sizeLabelTicket.Width;
            string linhaAtual = "";
            double espacamentoTickets = 20;

            for (int i = 0; i < ticketsStr.Count; i++)
            {
                string ticketAtual = ticketsStr[i];
                string testeString = string.IsNullOrEmpty(linhaAtual) ? ticketAtual : linhaAtual + " - " + ticketAtual;
                XSize tamanhoTeste = gfx.MeasureString(testeString, fontNegrito);

                if (tamanhoTeste.Width > larguraDisponivel && !string.IsNullOrEmpty(linhaAtual))
                {
                    gfx.DrawString(linhaAtual, fontNegrito, XBrushes.Black, ticketX, ticketY);
                    ticketY += espacamentoTickets;
                    linhaAtual = ticketAtual;
                }
                else
                {
                    linhaAtual = testeString;
                }
            }

            if (!string.IsNullOrEmpty(linhaAtual))
            {
                gfx.DrawString(linhaAtual, fontNegrito, XBrushes.Black, ticketX, ticketY);
                ticketY += espacamentoTickets;
            }

            double linhaY = ticketY + 5;

            if (dados.MTR != null && dados.MTR.Any())
            {
                string labelMTR = "MTR: ";
                XSize sizeLabelMTR = gfx.MeasureString(labelMTR, fontTexto);
                string mtrsStr = string.Join(" - ", dados.MTR);
                gfx.DrawString(labelMTR, fontTexto, XBrushes.Black, textoX, linhaY);
                gfx.DrawString(mtrsStr, fontNegrito, XBrushes.Black, textoX + sizeLabelMTR.Width, linhaY);
                linhaY += 25;
            }

            if (dados.NF != null && dados.NF.Any())
            {
                string labelNFe = "NFe: ";
                XSize sizeLabelNFe = gfx.MeasureString(labelNFe, fontTexto);
                string nfesStr = string.Join(" - ", dados.NF);
                gfx.DrawString(labelNFe, fontTexto, XBrushes.Black, textoX, linhaY);
                gfx.DrawString(nfesStr, fontNegrito, XBrushes.Black, textoX + sizeLabelNFe.Width, linhaY);
                linhaY += 25;
            }

            string mesEmissao = cultura.DateTimeFormat.GetMonthName(dados.MesAno.Month);
            mesEmissao = char.ToUpper(mesEmissao[0]) + mesEmissao.Substring(1);
            string dataAssinatura = $"Araras/SP, {mesEmissao} de {dados.MesAno.Year}";

            double dataX = 390;
            double dataY = blocoY + 92;

            gfx.DrawString(dataAssinatura, fontTexto, XBrushes.Black,
                new XRect(dataX, dataY, largura, 20), XStringFormats.TopLeft);

            string pasta = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string nomeArquivo = $"Certificado_{dados.NomeCliente}_{dados.MesAno:MM_yyyy}.pdf";
            string caminho = Path.Combine(pasta, nomeArquivo);

            document.Save(caminho);
            return caminho;
        }
        private void DesenharTextoJustificado(XGraphics gfx, List<(string texto, XFont fonte)> fragmentos,
            double x, double y, double largura, double espacamentoLinha, XFont fontePadrao)
        {
            var linhas = QuebrarEmLinhas(gfx, fragmentos, largura);

            foreach (var linha in linhas)
            {
                bool isUltimaLinha = (linha == linhas.Last());
                DesenharLinha(gfx, linha, x, y, largura, isUltimaLinha);
                y += espacamentoLinha;
            }
        }

        private List<List<(string texto, XFont fonte)>> QuebrarEmLinhas(XGraphics gfx,
            List<(string texto, XFont fonte)> fragmentos, double larguraMaxima)
        {
            var linhas = new List<List<(string texto, XFont fonte)>>();
            var linhaAtual = new List<(string texto, XFont fonte)>();
            double larguraAtual = 0;

            foreach (var fragmento in fragmentos)
            {
                var palavras = fragmento.texto.Split(new[] { ' ' }, StringSplitOptions.None);

                for (int i = 0; i < palavras.Length; i++)
                {
                    string palavra = palavras[i];
                    if (i < palavras.Length - 1) palavra += " ";

                    double larguraPalavra = gfx.MeasureString(palavra, fragmento.fonte).Width;

                    if (larguraAtual + larguraPalavra > larguraMaxima && linhaAtual.Count > 0)
                    {
                        linhas.Add(new List<(string texto, XFont fonte)>(linhaAtual));
                        linhaAtual.Clear();
                        larguraAtual = 0;
                    }

                    linhaAtual.Add((palavra, fragmento.fonte));
                    larguraAtual += larguraPalavra;
                }
            }

            if (linhaAtual.Count > 0)
            {
                linhas.Add(linhaAtual);
            }

            return linhas;
        }

        private void DesenharLinha(XGraphics gfx, List<(string texto, XFont fonte)> fragmentos,
            double x, double y, double larguraMaxima, bool isUltimaLinha)
        {
            double larguraTotal = 0;
            int totalEspacos = 0;

            foreach (var frag in fragmentos)
            {
                larguraTotal += gfx.MeasureString(frag.texto, frag.fonte).Width;
                totalEspacos += frag.texto.Count(c => c == ' ');
            }

            double posicaoAtualX = x;

            if (isUltimaLinha || totalEspacos == 0)
            {
                foreach (var frag in fragmentos)
                {
                    gfx.DrawString(frag.texto, frag.fonte, XBrushes.Black, posicaoAtualX, y);
                    posicaoAtualX += gfx.MeasureString(frag.texto, frag.fonte).Width;
                }
                return;
            }

            double espacoExtra = (larguraMaxima - larguraTotal) / totalEspacos;

            foreach (var frag in fragmentos)
            {
                string textoAtual = frag.texto;

                var partes = textoAtual.Split(' ');
                for (int i = 0; i < partes.Length; i++)
                {
                    if (!string.IsNullOrEmpty(partes[i]))
                    {
                        gfx.DrawString(partes[i], frag.fonte, XBrushes.Black, posicaoAtualX, y);
                        posicaoAtualX += gfx.MeasureString(partes[i], frag.fonte).Width;
                    }

                    if (i < partes.Length - 1)
                    {
                        double larguraEspaco = gfx.MeasureString(" ", frag.fonte).Width;
                        posicaoAtualX += larguraEspaco + espacoExtra;
                    }
                }
            }
        }

        private void btnLancamentos_Click(object sender, EventArgs e)
        {
            var form = new ExpedicaoFormLancamentos();
            form.ShowDialog();
        }

        private void btnLaudo_Click(object sender, EventArgs e)
        {
            if (cbEmpresa.SelectedValue == null)
            {
                MessageBox.Show("Selecione uma empresa e carregue o balanço antes.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dgvBalanca.Rows.Count == 0)
            {
                MessageBox.Show("Faça o Balanço de massa, exporte o certificado e depois exporte o Laudo!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int clienteId = Convert.ToInt32(cbEmpresa.SelectedValue);
            DateTime mesAno = new DateTime(dtpData.Value.Year, dtpData.Value.Month, 1);
            decimal pesoTotal = 0M;
            decimal.TryParse(txtPeso.Text, out pesoTotal);

            var formLaudo = new ExpedicaoFormLaudo(clienteId, mesAno, pesoTotal);
            formLaudo.ShowDialog();
        }

    }
}