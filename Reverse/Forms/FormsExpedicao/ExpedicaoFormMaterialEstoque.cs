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

            using (var ctx = new ReverseContext())
            {
                var materiais = ctx.Materiais
                    .OrderByDescending(m => m.Valorizacao)
                    .ThenBy(m => m.Nome)
                    .ToList();

                dgvMateriais.DataSource = new BindingList<Material>(materiais);
            }

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

        private void btnNovaLinha_Click(object sender, EventArgs e)
        {
            var lista = dgvMateriais.DataSource as BindingList<Material>;
            lista.Add(new Material { Nome = "", Valorizacao = 1 });
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvMateriais.CurrentRow == null) return;

            var material = dgvMateriais.CurrentRow.DataBoundItem as Material;
            if (material == null) return;

            var confirmar = MessageBox.Show("Deseja excluir este material?",
                "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes) return;

            using (var ctx = new ReverseContext())
            {
                var matDb = ctx.Materiais.FirstOrDefault(m => m.Id == material.Id);
                if (matDb != null)
                {
                    ctx.Materiais.Remove(matDb);
                    ctx.SaveChanges();
                }
            }

            (dgvMateriais.DataSource as BindingList<Material>).Remove(material);
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            dgvMateriais.EndEdit();

            var lista = dgvMateriais.DataSource as BindingList<Material>;

            using (var ctx = new ReverseContext())
            {
                foreach (var item in lista)
                {
                    if (string.IsNullOrWhiteSpace(item.Nome)) continue;

                    Material matDb;
                    if (item.Id > 0)
                    {
                        matDb = ctx.Materiais.FirstOrDefault(m => m.Id == item.Id);
                        if (matDb == null) continue;
                    }
                    else
                    {
                        matDb = new Material();
                        ctx.Materiais.Add(matDb);
                    }

                    matDb.Nome = item.Nome;
                    matDb.Valorizacao = item.Valorizacao;
                }

                ctx.SaveChanges();
            }

            MessageBox.Show("Materiais salvos com sucesso!", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
