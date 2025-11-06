using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormVeiculos : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;
        private int? veiculoAtualId;

        public ExpedicaoFormVeiculos()
        {
            InitializeComponent();
            this.Load += FormVeiculos_Load;
            dgvVeiculos.SelectionChanged += dgvVeiculos_SelectionChanged;
            _ = CarregarVeiculosAsync();
        }

        private void FormVeiculos_Load(object sender, EventArgs e)
        {
            dgvVeiculos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVeiculos.MultiSelect = false;
            dgvVeiculos.ReadOnly = true;
            dgvVeiculos.AllowUserToAddRows = false;
            dgvVeiculos.AllowUserToDeleteRows = false;
            dgvVeiculos.AllowUserToResizeRows = false;
            dgvVeiculos.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private async Task CarregarVeiculosAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    SELECT VeiculoId, Categoria, Modelo, Placa, Eixos
                    FROM Veiculos
                    ORDER BY Placa", conn);
                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                dgvVeiculos.DataSource = dt;

                if (dgvVeiculos.Columns.Contains("VeiculoId"))
                    dgvVeiculos.Columns["VeiculoId"].Visible = false;

                dgvVeiculos.Columns["Categoria"].HeaderText = "Categoria";
                dgvVeiculos.Columns["Modelo"].HeaderText = "Modelo";
                dgvVeiculos.Columns["Placa"].HeaderText = "Placa";
                dgvVeiculos.Columns["Eixos"].HeaderText = "Eixos";

                dgvVeiculos.DefaultCellStyle.ForeColor = Color.Black;

                dgvVeiculos.Columns["Categoria"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dgvVeiculos.Columns["Modelo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dgvVeiculos.Columns["Placa"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvVeiculos.Columns["Eixos"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgvVeiculos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
        }

        private async void dgvVeiculos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVeiculos.CurrentRow == null || dgvVeiculos.CurrentRow.IsNewRow)
            {
                LimparCampos();
                veiculoAtualId = null;
                return;
            }

            var valorId = dgvVeiculos.CurrentRow.Cells["VeiculoId"]?.Value;
            if (valorId == null || valorId == DBNull.Value)
            {
                LimparCampos();
                veiculoAtualId = null;
                return;
            }

            veiculoAtualId = Convert.ToInt32(valorId);
            await CarregarDadosVeiculoAsync(veiculoAtualId.Value);
        }

        private async Task CarregarDadosVeiculoAsync(int veiculoId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    SELECT Categoria, Modelo, Marca, Placa, Renavam, TipoCombustivel, 
                           Eixos, Tara, Tacografo, KmPorLitro
                    FROM Veiculos
                    WHERE VeiculoId = @Id", conn);

                cmd.Parameters.AddWithValue("@Id", veiculoId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        txtCategoria.Text = reader["Categoria"]?.ToString() ?? "";
                        txtModelo.Text = reader["Modelo"]?.ToString() ?? "";
                        txtMarca.Text = reader["Marca"]?.ToString() ?? "";
                        txtPlaca.Text = reader["Placa"]?.ToString() ?? "";
                        txtRenavam.Text = reader["Renavam"]?.ToString() ?? "";
                        txtCombustivel.Text = reader["TipoCombustivel"]?.ToString() ?? "";
                        txtEixos.Text = reader["Eixos"]?.ToString() ?? "";
                        txtTara.Text = reader["Tara"]?.ToString() ?? "";
                        if (reader["Tacografo"] != DBNull.Value)
                            dtpTacografo.Value = Convert.ToDateTime(reader["Tacografo"]);
                        else
                            dtpTacografo.Value = DateTime.Today;
                        txtKM.Text = reader["KmPorLitro"]?.ToString() ?? "";
                    }
                }
            }
        }

        private void btnCriar_Click(object sender, EventArgs e)
        {
            LimparCampos();
            veiculoAtualId = null;
            txtCategoria.Focus();
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            if (veiculoAtualId.HasValue)
            {
                await AtualizarVeiculoAsync();
                MessageBox.Show("Veículo atualizado com sucesso!");
            }
            else
            {
                veiculoAtualId = await InserirVeiculoAsync();
                MessageBox.Show("Veículo cadastrado com sucesso!");
            }

            await CarregarVeiculosAsync();

            if (!veiculoAtualId.HasValue)
            {
                dgvVeiculos.ClearSelection();
            }
        }

        private async void btnExcluir_Click(object sender, EventArgs e)
        {
            if (!veiculoAtualId.HasValue)
            {
                MessageBox.Show("Selecione um veículo para excluir.");
                return;
            }

            var confirm = MessageBox.Show("Deseja realmente excluir este veículo?",
                                          "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM Veiculos WHERE VeiculoId=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", veiculoAtualId.Value);
                await cmd.ExecuteNonQueryAsync();
            }

            await CarregarVeiculosAsync();
            LimparCampos();
            MessageBox.Show("Veículo excluído com sucesso!");
        }
        private async Task<int> InserirVeiculoAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    INSERT INTO Veiculos 
                        (Categoria, Modelo, Marca, Placa, Renavam, TipoCombustivel, Eixos, Tara, Tacografo, KmPorLitro)
                    OUTPUT INSERTED.VeiculoId
                    VALUES (@Categoria, @Modelo, @Marca, @Placa, @Renavam, @Combustivel, @Eixos, @Tara, @Tacografo, @KM)", conn);

                PreencherParametros(cmd);
                return (int)await cmd.ExecuteScalarAsync();
            }
        }

        private async Task AtualizarVeiculoAsync()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    UPDATE Veiculos SET 
                        Categoria=@Categoria, Modelo=@Modelo, Marca=@Marca, Placa=@Placa, Renavam=@Renavam,
                        TipoCombustivel=@Combustivel, Eixos=@Eixos, Tara=@Tara, Tacografo=@Tacografo, KmPorLitro=@KM
                    WHERE VeiculoId=@Id", conn);

                PreencherParametros(cmd);
                cmd.Parameters.AddWithValue("@Id", veiculoAtualId.Value);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private void PreencherParametros(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Categoria", txtCategoria.Text.Trim());
            cmd.Parameters.AddWithValue("@Modelo", txtModelo.Text.Trim());
            cmd.Parameters.AddWithValue("@Marca", txtMarca.Text.Trim());
            cmd.Parameters.AddWithValue("@Placa", txtPlaca.Text.Trim());
            cmd.Parameters.AddWithValue("@Renavam", txtRenavam.Text.Trim());
            cmd.Parameters.AddWithValue("@Combustivel", txtCombustivel.Text.Trim());
            cmd.Parameters.AddWithValue("@Eixos", string.IsNullOrWhiteSpace(txtEixos.Text) ? (object)DBNull.Value : int.Parse(txtEixos.Text));
            cmd.Parameters.AddWithValue("@Tara", string.IsNullOrWhiteSpace(txtTara.Text) ? (object)DBNull.Value : decimal.Parse(txtTara.Text));
            cmd.Parameters.AddWithValue("@Tacografo", dtpTacografo.Value);
            cmd.Parameters.AddWithValue("@KM", string.IsNullOrWhiteSpace(txtKM.Text) ? (object)DBNull.Value : decimal.Parse(txtKM.Text));
        }

        private void LimparCampos()
        {
            txtCategoria.Clear();
            txtModelo.Clear();
            txtMarca.Clear();
            txtPlaca.Clear();
            txtRenavam.Clear();
            txtCombustivel.Clear();
            txtEixos.Clear();
            txtTara.Clear();
            dtpTacografo.Value = DateTime.Today;
            txtKM.Clear();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
