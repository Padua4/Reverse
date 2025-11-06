using Reverse.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms.FormsTriagem
{
    public partial class TriagemFormPalete : Form
    {
        private int usuarioId;

        private void FormTriagemPalete_Load(object sender, EventArgs e)
        {
            CarregarPaletes();
        }

        public TriagemFormPalete(int _usuarioId)
        {
            InitializeComponent();
            usuarioId = _usuarioId;
            this.Load += FormTriagemPalete_Load;
            dgvPaletes.RowPrePaint += dgvPaletes_RowPrePaint;
        }

        private void CarregarPaletes()
        {
            using (var ctx = new ReverseContext())
            {
                var lista = ctx.Paletes
                    .Include(p => p.Itens)
                    .AsNoTracking()
                    .ToList()
                    .Select(p => new
                    {
                        p.Id,
                        p.Numero,
                        Nome = p.Nome ?? $"Palete {p.Numero} - {p.Categoria}",
                        Status = ObterNomeStatus(p.Status),
                        p.DataCriacao,
                        QuantidadeItens = p.Itens.Sum(i => (int?)i.Quantidade) ?? 0,
                        ValorTotal = p.Itens.Sum(i => (decimal?)i.Quantidade * i.ValorUnitario) ?? 0,
                        p.UsuarioFinalizacao,
                        p.DataFinalizacao
                    })
                    .OrderByDescending(p => p.DataCriacao)
                    .ToList();

                dgvPaletes.DataSource = lista;

                if (dgvPaletes.Columns.Count > 0)
                {
                    dgvPaletes.Columns["Id"].Visible = false;
                    dgvPaletes.Columns["Numero"].HeaderText = "Número";
                    dgvPaletes.Columns["Nome"].HeaderText = "Nome";
                    dgvPaletes.Columns["Status"].HeaderText = "Status";
                    dgvPaletes.Columns["DataCriacao"].HeaderText = "Data Criação";
                    dgvPaletes.Columns["DataCriacao"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvPaletes.Columns["QuantidadeItens"].HeaderText = "Qtd. Itens";
                    dgvPaletes.Columns["ValorTotal"].HeaderText = "Valor Total";
                    dgvPaletes.Columns["ValorTotal"].DefaultCellStyle.Format = "C2";
                    dgvPaletes.Columns["UsuarioFinalizacao"].HeaderText = "Finalizado por";
                    dgvPaletes.Columns["DataFinalizacao"].HeaderText = "Data Finalização";
                    dgvPaletes.Columns["DataFinalizacao"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
            }

            dgvPaletes.AutoGenerateColumns = true;
            dgvPaletes.DefaultCellStyle.ForeColor = Color.Black;
            dgvPaletes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPaletes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPaletes.MultiSelect = false;

            if (dgvPaletes.Columns.Count > 0)
            {
                dgvPaletes.Columns["Nome"].FillWeight = 200;
                dgvPaletes.Columns["Status"].FillWeight = 120;
                dgvPaletes.Columns["DataCriacao"].FillWeight = 120;
                dgvPaletes.Columns["UsuarioFinalizacao"].FillWeight = 150;
                dgvPaletes.Columns["DataFinalizacao"].FillWeight = 150;
                dgvPaletes.RowTemplate.Height = 35;
            }
        }

        private void dgvPaletes_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvPaletes.Rows[e.RowIndex];

            if (e.RowIndex % 2 == 0)
            {
                row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                row.DefaultCellStyle.ForeColor = Color.Black;
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.Gainsboro;
                row.DefaultCellStyle.ForeColor = Color.Black;
            }
        }


        private string ObterNomeStatus(int status)
        {
            return status switch
            {
                0 => "Aberto",
                1 => "Em andamento",
                2 => "Finalizado",
                3 => "Vendido",
                _ => "Desconhecido"
            };
        }

        private async void btnAberto_Click(object sender, EventArgs e)
        {
            await AlterarStatusPaleteAsync(0);
        }

        private async void btnEmAndamento_Click(object sender, EventArgs e)
        {
            await AlterarStatusPaleteAsync(1);
        }

        private async void btnFinalizado_Click(object sender, EventArgs e)
        {
            await AlterarStatusPaleteAsync(2, salvarUsuarioFinalizacao: true);
        }

        private async Task AlterarStatusPaleteAsync(int novoStatus, bool salvarUsuarioFinalizacao = false)
        {
            if (dgvPaletes.CurrentRow == null)
            {
                MessageBox.Show("Selecione uma palete válida.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvPaletes.CurrentRow.Cells["Id"].Value;

            using (var ctx = new ReverseContext())
            {
                var palete = ctx.Paletes.FirstOrDefault(p => p.Id == id);
                if (palete == null)
                {
                    MessageBox.Show("Palete não encontrada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                palete.Status = novoStatus;

                if (salvarUsuarioFinalizacao)
                {
                    palete.UsuarioFinalizacao = usuarioId.ToString();
                    palete.DataFinalizacao = DateTime.Now;
                }
                else
                {
                    palete.UsuarioFinalizacao = null;
                    palete.DataFinalizacao = null;
                }

                await ctx.SaveChangesAsync();
            }

            CarregarPaletes();
        }

        private async void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvPaletes.CurrentRow == null)
            {
                MessageBox.Show("Selecione uma palete válida.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvPaletes.CurrentRow.Cells["Id"].Value;

            var confirmar = MessageBox.Show(
                "Deseja realmente excluir esta palete?",
                "Confirmação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes) return;

            using (var ctx = new ReverseContext())
            {
                var palete = ctx.Paletes.Include("Itens").FirstOrDefault(p => p.Id == id);
                if (palete == null)
                {
                    MessageBox.Show("Palete não encontrada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ctx.ItensPalete.RemoveRange(palete.Itens);
                ctx.Paletes.Remove(palete);
                await ctx.SaveChangesAsync();
            }

            CarregarPaletes();
            MessageBox.Show("Palete excluída com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
