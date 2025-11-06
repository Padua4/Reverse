using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Reverse.Forms.FormsFinanceiro
{
    public partial class formLotesReceber : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        public int? LoteSelecionadoId { get; private set; }

        public formLotesReceber()
        {
            InitializeComponent();
            CarregarLotes();
            ConfigurarGrid();

            dgvLotes.CellDoubleClick += dgvLotes_CellDoubleClick;
            btnSelecionar.Click += btnSelecionar_Click;
            btnCancelar.Click += btnCancelar_Click;
        }

        private void ConfigurarGrid()
        {
            dgvLotes.AutoGenerateColumns = false;
            dgvLotes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLotes.MultiSelect = false;
            dgvLotes.ReadOnly = true;
            dgvLotes.AllowUserToResizeRows = false;
            dgvLotes.RowHeadersVisible = false;

            // Melhorar a aparência
            dgvLotes.BackgroundColor = SystemColors.Window;
            dgvLotes.BorderStyle = BorderStyle.Fixed3D;
            dgvLotes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            // Configurar colunas
            dgvLotes.Columns.Clear();

            // Coluna ID (oculta)
            var colId = new DataGridViewTextBoxColumn
            {
                Name = "LoteId",
                DataPropertyName = "LoteId",
                HeaderText = "ID",
                Width = 50,
                Visible = false
            };

            // Coluna Data
            var colData = new DataGridViewTextBoxColumn
            {
                Name = "DataLote",
                DataPropertyName = "DataLote",
                HeaderText = "Data do Lote",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };

            // Coluna Total de Contas
            var colTotalContas = new DataGridViewTextBoxColumn
            {
                Name = "TotalContas",
                DataPropertyName = "TotalContas",
                HeaderText = "Total Contas",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };

            // Coluna Valor Total
            var colValorTotal = new DataGridViewTextBoxColumn
            {
                Name = "ValorTotal",
                DataPropertyName = "ValorTotal",
                HeaderText = "Valor Total",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            };

            // Coluna Valor Recebido
            var colRecebido = new DataGridViewTextBoxColumn
            {
                Name = "ValorRecebido",
                DataPropertyName = "ValorRecebido",
                HeaderText = "Recebido",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    ForeColor = Color.Green
                }
            };

            // Coluna Valor Pendente
            var colPendente = new DataGridViewTextBoxColumn
            {
                Name = "ValorPendente",
                DataPropertyName = "ValorPendente",
                HeaderText = "Pendente",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    ForeColor = Color.Red
                }
            };

            dgvLotes.Columns.AddRange(colId, colData, colTotalContas, colValorTotal, colRecebido, colPendente);

            // Ajustar automaticamente as colunas para preencher o espaço disponível
            dgvLotes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void CarregarLotes()
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Consulta corrigida - usando ContaId em vez de Id
                    string sql = @"
                        SELECT 
                            l.LoteId,
                            l.DataLote,
                            COUNT(cr.ContaId) as TotalContas,
                            ISNULL(SUM(cr.Valor), 0) as ValorTotal,
                            ISNULL(SUM(CASE WHEN cr.DataRecebimento IS NOT NULL THEN cr.Valor ELSE 0 END), 0) as ValorRecebido,
                            ISNULL(SUM(CASE WHEN cr.DataRecebimento IS NULL THEN cr.Valor ELSE 0 END), 0) as ValorPendente
                        FROM LotesContasReceber l
                        LEFT JOIN ContasReceber cr ON l.LoteId = cr.LoteId
                        GROUP BY l.LoteId, l.DataLote
                        ORDER BY l.DataLote DESC";

                    using (var da = new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Verificar se há dados
                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("Nenhum lote encontrado.", "Informação",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        dgvLotes.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar lotes: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSelecionar_Click(object sender, EventArgs e)
        {
            if (dgvLotes.CurrentRow != null && dgvLotes.CurrentRow.Index >= 0)
            {
                LoteSelecionadoId = Convert.ToInt32(dgvLotes.CurrentRow.Cells["LoteId"].Value);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Selecione um lote para continuar.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void dgvLotes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSelecionar_Click(sender, e);
            }
        }

        private void dgvLotes_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Ajustar automaticamente as colunas após o binding dos dados
            foreach (DataGridViewColumn column in dgvLotes.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
    }
}