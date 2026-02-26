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

        private Dictionary<string, (string Tipo, string Tratamento)> materiaisTipoTratamento =
            new Dictionary<string, (string Tipo, string Tratamento)>(StringComparer.OrdinalIgnoreCase);

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

            txtCertificado.Leave += txtCertificado_Leave;
        }


        private async void FormBalanco_Load(object sender, EventArgs e)
        {
            await CarregarEmpresasAsync();
            await CarregarMateriaisTipoTratamentoAsync();
            ConfigurarGrids();
            HabilitarCampos(false);

            dtpData.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpData.Format = DateTimePickerFormat.Custom;
            dtpData.CustomFormat = "MM/yyyy";
            dtpData.ShowUpDown = true;

            txtCertificado.ReadOnly = false;
        }

        private async Task CarregarEmpresasAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand(
                    "SELECT ClienteId, Nome, RazaoSocial FROM Clientes ORDER BY Nome",
                    conn);

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

        private async Task CarregarMateriaisTipoTratamentoAsync()
        {
            materiaisTipoTratamento.Clear();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(
                        "SELECT Nome, Tipo, Tratamento FROM ExpMaterialLaudo", conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string nome = reader["Nome"].ToString();
                            string tipo = reader["Tipo"].ToString();
                            string tratamento = reader["Tratamento"].ToString();

                            if (!materiaisTipoTratamento.ContainsKey(nome))
                                materiaisTipoTratamento[nome] = (tipo, tratamento);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar materiais do banco: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ConfigurarGrids()
        {
            #region Configuração da Grid dgvTickets
            dgvTickets.Columns.Clear();
            dgvTickets.AllowUserToAddRows = false;
            dgvTickets.AllowUserToDeleteRows = false;
            dgvTickets.ReadOnly = true;

            var colData = new DataGridViewTextBoxColumn();
            colData.HeaderText = "Data";
            colData.Name = "Data";
            colData.Width = 80;
            colData.DefaultCellStyle.Format = "dd/MM/yyyy";
            colData.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvTickets.Columns.Add(colData);

            var colTicket = new DataGridViewTextBoxColumn();
            colTicket.HeaderText = "Ticket";
            colTicket.Name = "Ticket";
            colTicket.Width = 100;
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
            colPesoTicket.FillWeight = 60;
            colPesoTicket.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvTickets.Columns.Add(colPesoTicket);

            var colVolume = new DataGridViewTextBoxColumn();
            colVolume.HeaderText = "Volume";
            colVolume.Name = "Volume";
            colVolume.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colVolume.FillWeight = 40;
            colVolume.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvTickets.Columns.Add(colVolume);

            dgvTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            AplicarEstiloVisualProducao(dgvTickets);
            #endregion

            #region Configuração da Grid dgvBalanca
            dgvBalanca.Columns.Clear();
            dgvBalanca.AllowUserToAddRows = false;
            dgvBalanca.AllowUserToDeleteRows = false;

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

            AplicarEstiloVisualProducao(dgvBalanca);
            #endregion

            #region Configuração da Grid dgvResumo
            dgvResumo.Columns.Clear();
            dgvResumo.AllowUserToAddRows = false;
            dgvResumo.AllowUserToDeleteRows = false;
            dgvResumo.ReadOnly = true;

            var colMaterialResumo = new DataGridViewTextBoxColumn();
            colMaterialResumo.HeaderText = "Material";
            colMaterialResumo.Name = "Material";
            colMaterialResumo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colMaterialResumo.FillWeight = 60;
            dgvResumo.Columns.Add(colMaterialResumo);

            var colPesoResumo = new DataGridViewTextBoxColumn();
            colPesoResumo.HeaderText = "Peso Total (kg)";
            colPesoResumo.Name = "PesoTotal";
            colPesoResumo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colPesoResumo.FillWeight = 40;
            colPesoResumo.DefaultCellStyle.Format = "N3";
            colPesoResumo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvResumo.Columns.Add(colPesoResumo);

            dgvResumo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            AplicarEstiloVisualProducao(dgvResumo);
            #endregion

            #region Configuração da Grid dgvTotal
            dgvTotal.Columns.Clear();
            dgvTotal.AllowUserToAddRows = false;
            dgvTotal.AllowUserToDeleteRows = false;
            dgvTotal.ReadOnly = true;

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

            AplicarEstiloVisualProducao(dgvTotal);
            #endregion
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

        private void HabilitarCampos(bool habilitar)
        {
            btnCarregar.Enabled = !habilitar;
            cbEmpresa.Enabled = !habilitar;
            dtpData.Enabled = !habilitar;
            txtCertificado.Enabled = habilitar;

            btnSalvar.Enabled = habilitar;
            btnCancelar.Enabled = habilitar;
            btnCriarLinha.Enabled = habilitar;
            btnExcluirLinha.Enabled = habilitar;
            dgvBalanca.Enabled = habilitar;
        }

        private void txtCertificado_Leave(object sender, EventArgs e)
        {
            string certificado = txtCertificado.Text.Trim();

            if (!string.IsNullOrEmpty(certificado))
            {
                if (!ValidarFormatoCertificado(certificado))
                {
                    MessageBox.Show("Formato de certificado inválido!\n\nUse: XXXX-YYYY/SSSS-ZZZZ\nExemplo: 0100-0022/2025-0005",
                        "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCertificado.Focus();
                    txtCertificado.SelectAll();
                }
            }
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

            dgvTickets.Rows.Clear();
            dgvResumo.Rows.Clear();
            txtCertificado.Clear();

            await CarregarTicketsDoMesAsync(clienteIdSelecionado, mesAno);
            await CarregarBalancoSalvoAsync(clienteIdSelecionado, mesAno);

            HabilitarCampos(true);
        }

        private async Task CarregarTicketsDoMesAsync(int clienteId, DateTime mesAno)
        {
            dgvTickets.Rows.Clear();
            dgvResumo.Rows.Clear();

            DateTime inicioMes = mesAno;
            DateTime fimMes = mesAno.AddMonths(1).AddDays(-1);

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sql = @"
                    SELECT 
                        cl.Ticket,
                        cl.Data,
                        ISNULL(cl.Volume, 0) AS Volume,
                        lm.Material,
                        lm.Peso
                    FROM ControleLogistico cl
                    INNER JOIN LancamentosMateriais lm ON cl.Ticket = lm.Ticket
                    WHERE cl.ClienteId = @ClienteId
                        AND cl.Ticket IS NOT NULL
                        AND cl.Data >= @InicioMes
                        AND cl.Data <= @FimMes
                ORDER BY cl.Data, cl.Ticket, lm.Material";

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
                        DateTime data = Convert.ToDateTime(reader["Data"]);
                        string material = reader["Material"].ToString();
                        decimal peso = Convert.ToDecimal(reader["Peso"]);

                        int volume = 0;
                        object volumeObj = reader["Volume"];
                        if (volumeObj != DBNull.Value && volumeObj != null)
                        {
                            if (int.TryParse(volumeObj.ToString(), out int parsedInt))
                            {
                                volume = parsedInt;
                            }
                            else if (decimal.TryParse(volumeObj.ToString(), out decimal parsedDecimal))
                            {
                                volume = (int)parsedDecimal;
                            }
                            else if (double.TryParse(volumeObj.ToString(), out double parsedDouble))
                            {
                                volume = (int)parsedDouble;
                            }
                        }

                        if (ticket != ticketAnterior)
                        {
                            volumeTotalGeral += volume;
                            ticketAnterior = ticket;
                        }

                        pesoTotalGeral += peso;
                        dgvTickets.Rows.Add(data, ticket, material, peso, volume);
                    }
                }

                txtPeso.Text = pesoTotalGeral.ToString("N3");
                txtVolume.Text = volumeTotalGeral.ToString();
                AtualizarPesoRestante();

                GerarResumoMateriais();

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

        private void GerarResumoMateriais()
        {
            dgvResumo.Rows.Clear();

            if (dgvTickets.Rows.Count == 0)
                return;

            var resumo = new Dictionary<string, decimal>();

            foreach (DataGridViewRow row in dgvTickets.Rows)
            {
                if (row.IsNewRow) continue;

                var materialCell = row.Cells["Material"];
                var pesoCell = row.Cells["Peso"];

                if (materialCell.Value == null || pesoCell.Value == null)
                    continue;

                string material = materialCell.Value.ToString().Trim();
                if (string.IsNullOrWhiteSpace(material))
                    continue;

                if (!decimal.TryParse(pesoCell.Value.ToString(), out decimal peso))
                    continue;

                if (resumo.ContainsKey(material))
                {
                    resumo[material] += peso;
                }
                else
                {
                    resumo[material] = peso;
                }
            }

            foreach (var item in resumo.OrderBy(x => x.Key))
            {
                dgvResumo.Rows.Add(item.Key, item.Value);
            }

            if (resumo.Count > 0)
            {
                decimal totalPeso = resumo.Sum(x => x.Value);
                dgvResumo.Rows.Add("TOTAL", totalPeso);

                if (dgvResumo.Rows.Count > 0)
                {
                    var totalRow = dgvResumo.Rows[dgvResumo.Rows.Count - 1];
                    totalRow.DefaultCellStyle.BackColor = Color.FromArgb(180, 210, 255);
                    totalRow.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    totalRow.DefaultCellStyle.ForeColor = Color.FromArgb(0, 60, 120);
                }
            }
        }

        private async Task CarregarBalancoSalvoAsync(int clienteId, DateTime mesAno)
        {
            dgvBalanca.Rows.Clear();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string sqlCertificado = @"
                    SELECT TOP 1 NumeroCertificado 
                    FROM CertificadosEmitidos 
                    WHERE ClienteId = @ClienteId 
                    AND MesAno = @MesAno
                    ORDER BY DataEmissao DESC";

                var cmdCertificado = new SqlCommand(sqlCertificado, conn);
                cmdCertificado.Parameters.AddWithValue("@ClienteId", clienteId);
                cmdCertificado.Parameters.AddWithValue("@MesAno", mesAno);

                var resultado = await cmdCertificado.ExecuteScalarAsync();
                if (resultado != null && resultado != DBNull.Value)
                {
                    txtCertificado.Text = resultado.ToString();
                }
                else
                {
                    txtCertificado.Text = "";
                }

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

            string numeroCertificado = txtCertificado.Text.Trim();
            bool certificadoEditado = false;

            if (!string.IsNullOrEmpty(numeroCertificado))
            {
                if (!ValidarFormatoCertificado(numeroCertificado))
                {
                    MessageBox.Show("Formato de certificado inválido. Use: XXXX-YYYY/SSSS-ZZZZ\nExemplo: 0100-0022/2025-0005",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                certificadoEditado = true;
            }

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

                            if (certificadoEditado)
                            {
                                await SalvarCertificadoEditadoAsync(conn, transaction, clienteIdSelecionado, mesAno, numeroCertificado);
                            }

                            transaction.Commit();

                            MessageBox.Show("Balanço de massa " + (certificadoEditado ? "e certificado " : "") + "salvo com sucesso!", "Sucesso",
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

        private void GerarPDFComNumeroExistente(DadosCertificado dadosParaPDF)
        {
            try
            {
                string caminhoArquivo = GerarPDFCertificado(dadosParaPDF);

                var resultado = MessageBox.Show(
                    $"Certificado gerado com sucesso!\n\nNúmero: {dadosParaPDF.NumeroCertificado}\nCaminho: {caminhoArquivo}\n\nDeseja abrir o arquivo?",
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

        private async Task SalvarCertificadoEditadoAsync(SqlConnection conn, SqlTransaction transaction, int clienteId, DateTime mesAno, string numeroCertificado)
        {
            string sqlVerifica = @"
                SELECT COUNT(*) 
                FROM CertificadosEmitidos 
                WHERE ClienteId = @ClienteId 
                AND MesAno = @MesAno";

            var cmdVerifica = new SqlCommand(sqlVerifica, conn, transaction);
            cmdVerifica.Parameters.AddWithValue("@ClienteId", clienteId);
            cmdVerifica.Parameters.AddWithValue("@MesAno", mesAno);

            int existe = Convert.ToInt32(await cmdVerifica.ExecuteScalarAsync());

            if (existe > 0)
            {
                string sqlUpdate = @"
                    UPDATE CertificadosEmitidos 
                    SET NumeroCertificado = @NumeroCertificado, 
                        DataEmissao = GETDATE()
                    WHERE ClienteId = @ClienteId 
                    AND MesAno = @MesAno";

                var cmdUpdate = new SqlCommand(sqlUpdate, conn, transaction);
                cmdUpdate.Parameters.AddWithValue("@NumeroCertificado", numeroCertificado);
                cmdUpdate.Parameters.AddWithValue("@ClienteId", clienteId);
                cmdUpdate.Parameters.AddWithValue("@MesAno", mesAno);

                await cmdUpdate.ExecuteNonQueryAsync();
            }
            else
            {
                string sqlInsert = @"
                    INSERT INTO CertificadosEmitidos (ClienteId, MesAno, NumeroCertificado, DataEmissao)
                    VALUES (@ClienteId, @MesAno, @NumeroCertificado, GETDATE())";

                var cmdInsert = new SqlCommand(sqlInsert, conn, transaction);
                cmdInsert.Parameters.AddWithValue("@ClienteId", clienteId);
                cmdInsert.Parameters.AddWithValue("@MesAno", mesAno);
                cmdInsert.Parameters.AddWithValue("@NumeroCertificado", numeroCertificado);

                await cmdInsert.ExecuteNonQueryAsync();
            }

            var partes = numeroCertificado.Split('-');
            if (partes.Length == 3)
            {
                var codigo = partes[0];
                var meio = partes[1];
                var seqAno = partes[2];

                var partesMeio = meio.Split('/');
                if (partesMeio.Length == 2)
                {
                    int seqGeral = int.Parse(partesMeio[0]);
                    int ano = int.Parse(partesMeio[1]);
                    int seqAnoAtual = int.Parse(seqAno);

                    string sqlAtualizaCliente = @"
                        UPDATE Clientes 
                        SET CertificadoSequencialGeral = @SeqGeral,
                            CertificadoSequencialAnoAtual = @SeqAnoAtual,
                            CertificadoUltimoAno = @Ano
                        WHERE ClienteId = @ClienteId
                        AND (
                            CertificadoSequencialGeral < @SeqGeral 
                            OR CertificadoUltimoAno < @Ano
                            OR (CertificadoUltimoAno = @Ano AND CertificadoSequencialAnoAtual < @SeqAnoAtual)
                        )";

                    var cmdAtualiza = new SqlCommand(sqlAtualizaCliente, conn, transaction);
                    cmdAtualiza.Parameters.AddWithValue("@SeqGeral", seqGeral);
                    cmdAtualiza.Parameters.AddWithValue("@SeqAnoAtual", seqAnoAtual);
                    cmdAtualiza.Parameters.AddWithValue("@Ano", ano);
                    cmdAtualiza.Parameters.AddWithValue("@ClienteId", clienteId);

                    await cmdAtualiza.ExecuteNonQueryAsync();
                }
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
            dgvResumo.Rows.Clear();
            lblPesoRestante.Text = "Peso Restante: 0,000 kg";
            dgvBalanca.Rows.Clear();
            dgvTotal.Rows.Clear();
            txtPeso.Clear();
            txtVolume.Clear();
            txtCertificado.Clear();
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

            string certificadoExistente = txtCertificado.Text.Trim();

            if (!string.IsNullOrEmpty(certificadoExistente))
            {
                var resultado = MessageBox.Show(
                    $"Já existe um certificado registrado: {certificadoExistente}\n\n" +
                    "Deseja:\n" +
                    "Não: Usar este número (Não altera sequenciais)\n" +
                    "Sim: Gerar novo certificado (Atualiza sequenciais)",
                    "Certificado Existente",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    var dados = await ObterDadosCertificadoAsync();
                    if (dados == null) return;

                    dados.NumeroCertificado = certificadoExistente;
                    GerarPDFComNumeroExistente(dados);
                    return;
                }
                else if (resultado == DialogResult.Cancel)
                {
                    return;
                }
            }

            var dadosCertificado = await ObterDadosCertificadoAsync();
            if (dadosCertificado == null) return;

            linhaJaTemCertificado = await VerificarCertificadoExistenteAsync(dadosCertificado);

            if (!linhaJaTemCertificado)
            {
                isPrimeiraVezCertificado = (dadosCertificado.CertificadoSequencialGeral == 0);

                if (isPrimeiraVezCertificado)
                {
                    string certificadoInicial = SolicitarCertificadoInicial(dadosCertificado.CodigoEmpresa, dadosCertificado.MesAno);
                    if (string.IsNullOrEmpty(certificadoInicial))
                    {
                        return;
                    }

                    if (!ValidarFormatoCertificado(certificadoInicial))
                    {
                        MessageBox.Show("Formato inválido. Use: XXXX-YYYY/ZZZZ-AAAA\nExemplo: 0100-0022/2026-0001",
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    dadosCertificado.NumeroCertificado = certificadoInicial;
                    await AtualizarSequenciaisCertificado(dadosCertificado, certificadoInicial);

                    txtCertificado.Text = certificadoInicial;
                }
                else
                {
                    await IncrementarSequenciaisCertificado(dadosCertificado);

                    txtCertificado.Text = dadosCertificado.NumeroCertificado;
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

                txtCertificado.Text = dadosCertificado.NumeroCertificado;
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

        private string SolicitarCertificadoInicial(string codigoEmpresa, DateTime mesAno)
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
                           "Formato: XXXX-YYYY/ZZZZ-AAAA\n" +
                           "Exemplo: 0100-0022/2026-0001"
                };

                Label labelFormato = new Label()
                {
                    Left = 20,
                    Top = 110,
                    Width = 400,
                    Height = 60,
                    Text = "XXXX = Código da empresa (4 dígitos)\n" +
                           "YYYY = Sequência geral de todos os certificados (0001-9999)\n" +
                           "ZZZZ = Ano do certificado\n" +
                           "AAAA = Sequência do ano (reinicia a cada ano)"
                };

                string codigo4 = (codigoEmpresa ?? "").Trim();
                if (int.TryParse(codigo4, out var codNum))
                    codigo4 = codNum.ToString("D4");
                else
                    codigo4 = codigo4.PadLeft(4, '0');

                int anoCertificado = mesAno.Year;

                TextBox textBox = new TextBox()
                {
                    Left = 20,
                    Top = 180,
                    Width = 400,
                    Text = $"{codigo4}-0001/{anoCertificado}-0001"
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

            if (seqGeral.Length != 4 || !int.TryParse(seqGeral, out var seqGeralVal)) return false;

            if (seqAno.Length != 4 || !int.TryParse(seqAno, out var seqAnoVal)) return false;

            if (ano.Length != 4 || !int.TryParse(ano, out var anoVal)) return false;

            if (seqGeralVal < 1 || seqGeralVal > 9999) return false;
            if (seqAnoVal < 1 || seqAnoVal > 9999) return false;

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
            int anoCertificado = int.Parse(partesMeio[1]);
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
                        cmdCliente.Parameters.AddWithValue("@Ano", anoCertificado);
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
                        dados.CertificadoUltimoAno = anoCertificado;
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
            int anoCertificado = dados.MesAno.Year;

            int novoSeqGeral = dados.CertificadoSequencialGeral + 1;
            int novoSeqAno = dados.CertificadoSequencialAnoAtual + 1;

            if (dados.CertificadoUltimoAno != anoCertificado)
            {
                novoSeqAno = 1;
            }

            if (novoSeqGeral > 9999)
            {
                novoSeqGeral = 1;
            }

            string codigo4 = (dados.CodigoEmpresa ?? "").Trim();
            if (int.TryParse(codigo4, out var codNum))
                codigo4 = codNum.ToString("D4");
            else
                codigo4 = codigo4.PadLeft(4, '0');

            string novoCertificado = $"{codigo4}-{novoSeqGeral:D4}/{anoCertificado}-{novoSeqAno:D4}";

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

                                novoSeqGeral = seqGeralAtual + 1;

                                if (ultimoAno != anoCertificado)
                                {
                                    novoSeqAno = 1;
                                }
                                else
                                {
                                    novoSeqAno = seqAnoAtual + 1;
                                }

                                if (novoSeqGeral > 9999)
                                {
                                    novoSeqGeral = 1;
                                }

                                novoCertificado = $"{codigo4}-{novoSeqGeral:D4}/{anoCertificado}-{novoSeqAno:D4}";
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
                        cmdCliente.Parameters.AddWithValue("@Ano", anoCertificado);
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
                        dados.CertificadoUltimoAno = anoCertificado;
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
                SELECT RazaoSocial, CPF_CNPJ, CodigoEmpresa,
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
                        dados.NomeCliente = reader["RazaoSocial"].ToString();
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

            string mesEmissao = cultura.DateTimeFormat.GetMonthName(DateTime.Now.Month);
            mesEmissao = char.ToUpper(mesEmissao[0]) + mesEmissao.Substring(1);
            string dataAssinatura = $"Araras/SP, {mesEmissao} de {DateTime.Now.Year}";

            double dataX = 390;
            double dataY = blocoY + 92;

            gfx.DrawString(dataAssinatura, fontTexto, XBrushes.Black,
                new XRect(dataX, dataY, largura, 20), XStringFormats.TopLeft);

            string nomeEmpresaLimpo = RemoverCaracteresInvalidosNomeArquivo(dados.NomeCliente);

            string pasta = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string nomeArquivo = $"Certificado_{nomeEmpresaLimpo}_{dados.MesAno:MM_yyyy}.pdf";
            string caminho = Path.Combine(pasta, nomeArquivo);

            document.Save(caminho);
            return caminho;
        }

        private string RemoverCaracteresInvalidosNomeArquivo(string nomeArquivo)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "SemNome";

            char[] caracteresInvalidos = Path.GetInvalidFileNameChars();

            string nomeValido = nomeArquivo;
            foreach (char c in caracteresInvalidos)
            {
                nomeValido = nomeValido.Replace(c, '_');
            }

            nomeValido = System.Text.RegularExpressions.Regex.Replace(nomeValido, @"\s+", " ").Trim();

            if (nomeValido.Length > 100)
                nomeValido = nomeValido.Substring(0, 100);

            return nomeValido;
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

        private void AtualizarComboBoxBalanca()
        {
            if (!dgvBalanca.Columns.Contains("Material")) return;

            var colMaterial = (DataGridViewComboBoxColumn)dgvBalanca.Columns["Material"];

            var valoresSalvos = new Dictionary<int, string>();
            foreach (DataGridViewRow row in dgvBalanca.Rows)
            {
                if (row.IsNewRow) continue;
                valoresSalvos[row.Index] = row.Cells["Material"].Value?.ToString() ?? "";
            }

            colMaterial.DataSource = materiaisTipoTratamento.Keys.OrderBy(k => k).ToList();

            foreach (DataGridViewRow row in dgvBalanca.Rows)
            {
                if (row.IsNewRow) continue;
                if (valoresSalvos.TryGetValue(row.Index, out string valorAnterior) &&
                    !string.IsNullOrWhiteSpace(valorAnterior) &&
                    materiaisTipoTratamento.ContainsKey(valorAnterior))
                {
                    row.Cells["Material"].Value = valorAnterior;
                }
            }

            AgruparPorTipo();
        }

        private async void btnMaterial_Click(object sender, EventArgs e)
        {
            var formMaterial = new ExpedicaoFormNovoMaterial();
            formMaterial.ShowDialog();

            await CarregarMateriaisTipoTratamentoAsync();

            AtualizarComboBoxBalanca();
        }
    }
}