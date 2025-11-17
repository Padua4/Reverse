using Reverse.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Reverse.Forms.FormsLogin.FormConfigU;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormMaterialEstoque : Form
    {
        private readonly int _usuarioId;
        private BindingList<Material> _materiaisList;

        public ExpedicaoFormMaterialEstoque(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
        }

        private void FormMaterialEstoque_Load(object sender, EventArgs e)
        {
            string formName = this.GetType().Name;

            if (!PermissaoHelper.TemPermissao(_usuarioId, formName))
            {
                MessageBox.Show("Você não tem permissão para acessar este formulário.",
                                "Acesso negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            ConfigurarGrid();
            CarregarMateriais();
        }

        private void ConfigurarGrid()
        {
            dgvMateriais.AutoGenerateColumns = false;
            dgvMateriais.Columns.Clear();

            var colNome = new DataGridViewTextBoxColumn
            {
                Name = "Nome",
                HeaderText = "Nome do Material",
                DataPropertyName = "Nome",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 50
            };

            var colValorizacao = new DataGridViewComboBoxColumn
            {
                Name = "Valorizacao",
                HeaderText = "Valorização",
                DataPropertyName = "Valorizacao",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 50,
                DataSource = new[]
                {
                    new { Valor = 1, Texto = "⭐" },
                    new { Valor = 2, Texto = "⭐⭐" },
                    new { Valor = 3, Texto = "⭐⭐⭐" },
                    new { Valor = 4, Texto = "⭐⭐⭐⭐" },
                    new { Valor = 5, Texto = "⭐⭐⭐⭐⭐" }
                },
                ValueMember = "Valor",
                DisplayMember = "Texto"
            };

            dgvMateriais.Columns.Add(colNome);
            dgvMateriais.Columns.Add(colValorizacao);

            dgvMateriais.DefaultCellStyle.ForeColor = Color.Black;
            dgvMateriais.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgvMateriais.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;

            dgvMateriais.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMateriais.AllowUserToAddRows = false;
            dgvMateriais.AllowUserToDeleteRows = false;
            dgvMateriais.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMateriais.MultiSelect = false;

            dgvMateriais.DataError += (s, ev) =>
            {
                ev.ThrowException = false;
            };
        }

        private void CarregarMateriais()
        {
            using (var ctx = new ReverseContext())
            {
                var materiais = ctx.Materiais
                    .OrderByDescending(m => m.Valorizacao)
                    .ThenBy(m => m.Nome)
                    .ToList();

                _materiaisList = new BindingList<Material>(materiais);
                dgvMateriais.DataSource = _materiaisList;
            }
        }

        private void btnNovaLinha_Click(object sender, EventArgs e)
        {
            _materiaisList.Add(new Material { Nome = "", Valorizacao = 1 });
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvMateriais.CurrentRow == null) return;

            var material = dgvMateriais.CurrentRow.DataBoundItem as Material;
            if (material == null) return;

            var confirmar = MessageBox.Show("Deseja excluir este material?",
                "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes) return;

            try
            {
                using (var ctx = new ReverseContext())
                {
                    if (material.Id > 0)
                    {
                        var matDb = ctx.Materiais.FirstOrDefault(m => m.Id == material.Id);
                        if (matDb != null)
                        {
                            ctx.Materiais.Remove(matDb);
                            ctx.SaveChanges();
                        }
                    }
                }

                _materiaisList.Remove(material);
                MessageBox.Show("Material excluído com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            dgvMateriais.EndEdit();

            var nomesDuplicados = _materiaisList
                .Where(m => !string.IsNullOrWhiteSpace(m.Nome))
                .GroupBy(m => m.Nome.Trim().ToUpper())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (nomesDuplicados.Any())
            {
                MessageBox.Show($"Materiais duplicados encontrados:\n\n{string.Join("\n", nomesDuplicados)}\n\nRemova as duplicatas antes de salvar.",
                    "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var ctx = new ReverseContext())
                {
                    var nomesParaBuscar = _materiaisList
                        .Where(m => !string.IsNullOrWhiteSpace(m.Nome))
                        .Select(m => m.Nome.Trim())
                        .ToList();

                    var materiaisExistentes = ctx.Materiais
                        .Where(m => nomesParaBuscar.Contains(m.Nome))
                        .ToDictionary(m => m.Nome.Trim().ToUpper(), m => m);

                    foreach (var item in _materiaisList)
                    {
                        if (string.IsNullOrWhiteSpace(item.Nome)) continue;

                        string nomeNormalizado = item.Nome.Trim().ToUpper();

                        if (materiaisExistentes.ContainsKey(nomeNormalizado))
                        {
                            var existente = materiaisExistentes[nomeNormalizado];

                            if (existente.Id == item.Id || item.Id == 0)
                            {
                                existente.Nome = item.Nome.Trim();
                                existente.Valorizacao = item.Valorizacao;
                            }
                            else
                            {
                                MessageBox.Show($"Já existe um material com o nome '{item.Nome}'!",
                                    "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                continue;
                            }
                        }
                        else
                        {
                            var novoMaterial = new Material
                            {
                                Nome = item.Nome.Trim(),
                                Valorizacao = item.Valorizacao
                            };
                            ctx.Materiais.Add(novoMaterial);
                            materiaisExistentes[nomeNormalizado] = novoMaterial;
                        }
                    }

                    ctx.SaveChanges();
                }

                MessageBox.Show("Materiais salvos com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CarregarMateriais();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}\n\n{ex.InnerException?.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}