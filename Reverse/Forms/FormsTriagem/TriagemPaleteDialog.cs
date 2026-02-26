using Reverse.Models;
using System;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reverse
{
    public partial class TriagemPaleteDialog : Form
    {
        private CategoriaPalete _categoriaSelecionada;

        public CategoriaPalete CategoriaSelecionada => _categoriaSelecionada;

        public TriagemPaleteDialog()
        {
            InitializeComponent();

            btnOk.DialogResult = DialogResult.None; // Mudado para None para validar antes
            btnCancel.DialogResult = DialogResult.Cancel;
            this.CancelButton = btnCancel;

            // Configurar eventos
            Load += TriagemPaleteDialog_Load;
            btnOk.Click += BtnOk_Click;
            btnNovaCategoria.Click += BtnNovaCategoria_Click;
            dgvPaleteCriar.CellDoubleClick += DgvPaleteCriar_CellDoubleClick;
        }

        private void TriagemPaleteDialog_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CarregarCategorias();
        }

        private void ConfigurarGrid()
        {
            dgvPaleteCriar.AutoGenerateColumns = false;
            dgvPaleteCriar.AllowUserToAddRows = false;
            dgvPaleteCriar.AllowUserToDeleteRows = false;
            dgvPaleteCriar.ReadOnly = true;
            dgvPaleteCriar.MultiSelect = false;
            dgvPaleteCriar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Estilo visual
            dgvPaleteCriar.BackgroundColor = Color.FromArgb(250, 250, 252);
            dgvPaleteCriar.BorderStyle = BorderStyle.FixedSingle;
            dgvPaleteCriar.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvPaleteCriar.GridColor = Color.FromArgb(230, 230, 235);

            dgvPaleteCriar.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvPaleteCriar.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvPaleteCriar.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPaleteCriar.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvPaleteCriar.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvPaleteCriar.ColumnHeadersHeight = 40;

            dgvPaleteCriar.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            dgvPaleteCriar.RowsDefaultCellStyle.BackColor = Color.White;
            dgvPaleteCriar.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            dgvPaleteCriar.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvPaleteCriar.EnableHeadersVisualStyles = false;
            dgvPaleteCriar.RowHeadersVisible = false;
            dgvPaleteCriar.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvPaleteCriar.RowTemplate.Height = 36;

            // Colunas
            dgvPaleteCriar.Columns.Clear();

            dgvPaleteCriar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                DataPropertyName = "Id",
                Visible = false
            });

            dgvPaleteCriar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nome",
                DataPropertyName = "Nome",
                HeaderText = "Categoria da Palete",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular)
                }
            });
        }

        private void CarregarCategorias()
        {
            try
            {
                using (var ctx = new ReverseContext())
                {
                    // Otimização: AsNoTracking para leitura rápida
                    var categorias = ctx.Set<CategoriaPalete>()
                        .AsNoTracking()
                        .Where(c => c.Ativo)
                        .OrderBy(c => c.Nome)
                        .ToList();

                    dgvPaleteCriar.DataSource = categorias;

                    // Selecionar primeira linha automaticamente
                    if (dgvPaleteCriar.Rows.Count > 0)
                    {
                        dgvPaleteCriar.Rows[0].Selected = true;
                        dgvPaleteCriar.CurrentCell = dgvPaleteCriar.Rows[0].Cells["Nome"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar categorias: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnNovaCategoria_Click(object sender, EventArgs e)
        {
            using (var frmInput = new Form())
            {
                frmInput.Text = "Nova Categoria";
                frmInput.Size = new Size(400, 150);
                frmInput.StartPosition = FormStartPosition.CenterParent;
                frmInput.FormBorderStyle = FormBorderStyle.FixedDialog;
                frmInput.MaximizeBox = false;
                frmInput.MinimizeBox = false;

                var lblPrompt = new Label
                {
                    Text = "Digite o nome da nova categoria:",
                    Location = new Point(20, 20),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9F)
                };

                var txtCategoria = new TextBox
                {
                    Location = new Point(20, 45),
                    Size = new Size(340, 25),
                    Font = new Font("Segoe UI", 10F),
                    MaxLength = 100
                };

                var btnConfirmar = new Button
                {
                    Text = "Confirmar",
                    Location = new Point(185, 80),
                    Size = new Size(85, 30),
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(52, 73, 94),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                var btnCancelarNovo = new Button
                {
                    Text = "Cancelar",
                    Location = new Point(275, 80),
                    Size = new Size(85, 30),
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(189, 195, 199),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                frmInput.Controls.AddRange(new Control[] { lblPrompt, txtCategoria, btnConfirmar, btnCancelarNovo });
                frmInput.AcceptButton = btnConfirmar;
                frmInput.CancelButton = btnCancelarNovo;

                txtCategoria.Focus();

                if (frmInput.ShowDialog() == DialogResult.OK)
                {
                    string nomeCategoria = txtCategoria.Text.Trim();

                    if (string.IsNullOrWhiteSpace(nomeCategoria))
                    {
                        MessageBox.Show("O nome da categoria não pode estar vazio.",
                            "Aviso",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        using (var ctx = new ReverseContext())
                        {
                            // Verificar se já existe
                            bool jaExiste = ctx.Set<CategoriaPalete>()
                                .Any(c => c.Nome.ToLower() == nomeCategoria.ToLower());

                            if (jaExiste)
                            {
                                MessageBox.Show("Já existe uma categoria com este nome.",
                                    "Aviso",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                                return;
                            }

                            var novaCategoria = new CategoriaPalete
                            {
                                Nome = nomeCategoria,
                                DataCriacao = DateTime.Now,
                                Ativo = true
                            };

                            ctx.Set<CategoriaPalete>().Add(novaCategoria);
                            ctx.SaveChanges();

                            MessageBox.Show("Categoria criada com sucesso!",
                                "Sucesso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            // Recarregar grid e selecionar a nova categoria
                            CarregarCategorias();

                            // Selecionar a categoria recém-criada
                            foreach (DataGridViewRow row in dgvPaleteCriar.Rows)
                            {
                                if (row.Cells["Nome"].Value?.ToString() == nomeCategoria)
                                {
                                    row.Selected = true;
                                    dgvPaleteCriar.CurrentCell = row.Cells["Nome"];
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao criar categoria: {ex.Message}",
                            "Erro",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (dgvPaleteCriar.CurrentRow == null)
            {
                MessageBox.Show("Selecione uma categoria antes de continuar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var categoriaId = (int)dgvPaleteCriar.CurrentRow.Cells["Id"].Value;

            using (var ctx = new ReverseContext())
            {
                _categoriaSelecionada = ctx.Set<CategoriaPalete>()
                    .AsNoTracking()
                    .FirstOrDefault(c => c.Id == categoriaId);
            }

            if (_categoriaSelecionada == null)
            {
                MessageBox.Show("Erro ao carregar a categoria selecionada.",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void DgvPaleteCriar_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnOk_Click(sender, EventArgs.Empty);
            }
        }
    }
}