using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;


namespace Reverse.Forms.FormsRH
{
    public partial class FormCurriculosNovos : Form
    {
        private readonly string connectionString =
        ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;


        private string caminhoCurriculoSelecionado;

        public FormCurriculosNovos(int _usuarioId)
        {
            InitializeComponent();
            ConfigurarEventos();
            CarregarCategorias();
            ConfigurarGrid();
            CarregarGridNovos();
        }

        private void ConfigurarEventos()
        {
            btnSalvar.Click += btnSalvar_Click;
            btnAnexar.Click += btnAnexar_Click;
            btnApto.Click += btnApto_Click;
            btnInapto.Click += btnInapto_Click;
            dgvParticipantes.CellDoubleClick += dgvParticipantes_CellDoubleClick;
        }

        private void CarregarCategorias()
        {
            var categorias = new[]
            {
                "Administrativo",
                "Area Tecnica",
                "Compras",
                "Limpeza",
                "Logistica",
                "Manutenção",
                "Motorista",
                "Produção",
                "Segurança",
                "T.I",
                "Vigilante",
                "Outros"
            };

            cbbCat.Items.Clear();
            cbbCat.Items.AddRange(categorias);
            cbbCat.SelectedIndex = -1;
        }

        private void ConfigurarGrid()
        {
            dgvParticipantes.AutoGenerateColumns = false;
            dgvParticipantes.Columns.Clear();

            // Comportamento e aparência gerais
            dgvParticipantes.AllowUserToAddRows = false;
            dgvParticipantes.AllowUserToDeleteRows = false;
            dgvParticipantes.AllowUserToResizeRows = false;
            dgvParticipantes.MultiSelect = false;
            dgvParticipantes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvParticipantes.RowHeadersVisible = false;
            dgvParticipantes.ScrollBars = ScrollBars.Both;

            // Cabeçalho sempre visível e com estilo definido
            dgvParticipantes.EnableHeadersVisualStyles = false;
            dgvParticipantes.ColumnHeadersVisible = true;
            dgvParticipantes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvParticipantes.ColumnHeadersHeight = 28;
            dgvParticipantes.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvParticipantes.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvParticipantes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvParticipantes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvParticipantes.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            // Estilo das células
            dgvParticipantes.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvParticipantes.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dgvParticipantes.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvParticipantes.DefaultCellStyle.SelectionBackColor = Color.AliceBlue;
            dgvParticipantes.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Modo de ajuste de largura proporcional
            dgvParticipantes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Coluna oculta Id (chave)
            var colId = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                HeaderText = "Id",
                Name = "Id",
                Visible = false
            };
            dgvParticipantes.Columns.Add(colId);

            // Nome do candidato (maior peso)
            var colNome = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NomeCandidato",
                HeaderText = "Nome do candidato",
                Name = "NomeCandidato",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 48,
                MinimumWidth = 150,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft, WrapMode = DataGridViewTriState.False }
            };
            dgvParticipantes.Columns.Add(colNome);

            // Categoria
            var colCategoria = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Categoria",
                HeaderText = "Categoria",
                Name = "Categoria",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 17,
                MinimumWidth = 110
            };
            dgvParticipantes.Columns.Add(colCategoria);

            // Caminho do currículo (médio)
            var colCaminho = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CaminhoCurriculo",
                HeaderText = "Currículo (arquivo)",
                Name = "CaminhoCurriculo",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 20,
                MinimumWidth = 160
            };
            dgvParticipantes.Columns.Add(colCaminho);

            // Data de cadastro (menor peso)
            var colData = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataCadastro",
                HeaderText = "Cadastrado em",
                Name = "DataCadastro",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 15,
                MinimumWidth = 120,
                DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm", Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            dgvParticipantes.Columns.Add(colData);

            // Desabilita ordenação por cabeçalho (opcional)
            foreach (DataGridViewColumn c in dgvParticipantes.Columns)
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void CarregarGridNovos(int limite = 100)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var da = new SqlDataAdapter($@"
            SELECT TOP({limite})
                Id, 
                NomeCandidato, 
                Categoria, 
                CaminhoCurriculo, 
                DataCadastro 
            FROM Curriculos 
            WHERE Status = 'Novo' 
            ORDER BY DataCadastro DESC", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvParticipantes.DataSource = dt;

                    if (dt.Rows.Count >= limite)
                    {
                        lblInfo.Text = $"Mostrando os {limite} registros mais recentes. Total pode ser maior.";
                    }
                    else
                    {
                        lblInfo.Text = $"Total de candidatos novos: {dt.Rows.Count}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar lista: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnexar_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Selecione o currículo do candidato";
                ofd.Filter = "Arquivos suportados|*.jpeg;*.jpg;*.png;*.pdf;*.docx";
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    caminhoCurriculoSelecionado = ofd.FileName;
                    btnAnexar.Text = Path.GetFileName(ofd.FileName);
                    btnAnexar.Tag = ofd.FileName;
                }
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            // Validação adicional de duplicata
            if (!ValidarCandidatoUnico(out string erroDuplicata))
            {
                MessageBox.Show(erroDuplicata, "Candidato já cadastrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new SqlCommand(@"
                        INSERT INTO Curriculos (NomeCandidato, Categoria, CaminhoCurriculo, Status, DataCadastro)
                        VALUES (@nome, @categoria, @caminho, 'Novo', @dataCadastro);", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@nome", txtNome.Text.Trim());
                                cmd.Parameters.AddWithValue("@categoria", cbbCat.SelectedItem.ToString());
                                cmd.Parameters.AddWithValue("@caminho", caminhoCurriculoSelecionado);
                                cmd.Parameters.AddWithValue("@dataCadastro", DateTime.Now);

                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            LimparCampos();
                            CarregarGridNovos();
                            MessageBox.Show("Candidato cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show($"Erro ao salvar candidato: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCandidatoUnico(out string erro)
        {
            erro = null;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"
            SELECT COUNT(*) FROM Curriculos 
            WHERE UPPER(LTRIM(RTRIM(NomeCandidato))) = UPPER(LTRIM(RTRIM(@nome))) 
            AND Status IN ('Novo', 'Apto')", conn))
                {
                    cmd.Parameters.AddWithValue("@nome", txtNome.Text.Trim());

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        erro = "Já existe um candidato ativo com este nome.";
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                erro = $"Erro ao validar duplicata: {ex.Message}";
                return false;
            }
        }

        private bool ValidarCampos()
        {
            // Validar nome
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Informe o nome do candidato.", "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return false;
            }

            if (txtNome.Text.Trim().Length < 2)
            {
                MessageBox.Show("Nome deve ter pelo menos 2 caracteres.", "Nome inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return false;
            }

            // Validar categoria
            if (cbbCat.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione a categoria.", "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbbCat.DroppedDown = true;
                return false;
            }

            // Validar arquivo
            var caminho = caminhoCurriculoSelecionado ?? btnAnexar.Tag as string;
            if (string.IsNullOrWhiteSpace(caminho))
            {
                MessageBox.Show("Anexe o currículo do candidato.", "Arquivo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnAnexar.Focus();
                return false;
            }

            var extensoesPermitidas = new[] { ".jpeg", ".jpg", ".png", ".pdf", ".docx" };
            var ext = Path.GetExtension(caminho)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !extensoesPermitidas.Contains(ext))
            {
                MessageBox.Show("Tipo de arquivo não suportado.\nFormatos aceitos: JPEG, JPG, PNG, PDF ou DOCX.", "Arquivo inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!File.Exists(caminho))
            {
                MessageBox.Show("O arquivo do currículo não foi encontrado.\nVerifique se o arquivo ainda existe no local selecionado.", "Arquivo não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnAnexar.Focus();
                return false;
            }

            var fileInfo = new FileInfo(caminho);
            if (fileInfo.Length > 10 * 1024 * 1024)
            {
                MessageBox.Show("O arquivo é muito grande. Tamanho máximo permitido: 10MB.", "Arquivo muito grande", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void LimparCampos()
        {
            txtNome.Clear();
            cbbCat.SelectedIndex = -1;
            caminhoCurriculoSelecionado = null;
            btnAnexar.Tag = null;
            btnAnexar.Text = "Anexar";
        }

        private void dgvParticipantes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var caminho = dgvParticipantes.Rows[e.RowIndex].Cells["CaminhoCurriculo"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(caminho))
            {
                MessageBox.Show("Caminho do arquivo não disponível.", "Arquivo não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(caminho))
            {
                var nomeCandidato = dgvParticipantes.Rows[e.RowIndex].Cells["NomeCandidato"].Value?.ToString() ?? "Desconhecido";
                MessageBox.Show($"Arquivo não encontrado para o candidato '{nomeCandidato}'.\n\nCaminho: {caminho}\n\nO arquivo pode ter sido movido ou excluído.", "Arquivo não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = caminho,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir o arquivo:\n{ex.Message}\n\nCaminho: {caminho}", "Erro ao abrir arquivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnApto_Click(object sender, EventArgs e)
        {
            AtualizarStatusSelecionado("Apto");
        }

        private void btnInapto_Click(object sender, EventArgs e)
        {
            AtualizarStatusSelecionado("Inapto");
        }

        private void AtualizarStatusSelecionado(string novoStatus)
        {
            if (dgvParticipantes.CurrentRow == null)
            {
                MessageBox.Show("Selecione um candidato na lista.", "Seleção necessária", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idObj = dgvParticipantes.CurrentRow.Cells["Id"].Value;
            if (idObj == null || !int.TryParse(idObj.ToString(), out int id))
            {
                MessageBox.Show("Registro inválido selecionado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var nomeCandidato = dgvParticipantes.CurrentRow.Cells["NomeCandidato"].Value?.ToString() ?? "Não informado";

            var confirmacao = MessageBox.Show(
                $"Confirma alterar o status do candidato '{nomeCandidato}' para '{novoStatus}'?",
                "Confirmar alteração",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacao != DialogResult.Yes)
                return;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new SqlCommand(@"
                        UPDATE Curriculos
                        SET Status = @status,
                            Apto = @apto,
                            Situacao = @situacao,
                            DataAtualizacao = @dataAtualizacao
                        WHERE Id = @id", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@status", novoStatus);
                                cmd.Parameters.AddWithValue("@id", id);
                                cmd.Parameters.AddWithValue("@dataAtualizacao", DateTime.Now);

                                if (novoStatus.Equals("Apto", StringComparison.OrdinalIgnoreCase))
                                {
                                    cmd.Parameters.AddWithValue("@apto", 1);
                                    cmd.Parameters.AddWithValue("@situacao", "Apto");
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@apto", 0);
                                    cmd.Parameters.AddWithValue("@situacao", "Inapto");
                                }

                                var afetados = cmd.ExecuteNonQuery();

                                if (afetados > 0)
                                {
                                    transaction.Commit();

                                    CarregarGridNovos();
                                    MessageBox.Show($"Candidato '{nomeCandidato}' marcado como {novoStatus}.", "Atualizado",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    transaction.Rollback();
                                    MessageBox.Show("Nenhum registro foi atualizado. O candidato pode ter sido alterado por outro usuário.", "Aviso",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
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
                MessageBox.Show($"Erro ao atualizar status: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnMin_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
    }
}
