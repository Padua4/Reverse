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
using System.Globalization;

namespace Reverse.Forms.FormsTriagem
{
    public partial class TriagemFormPalete : Form
    {
        private int usuarioId;
        private string nomeUsuario;

        private void FormTriagemPalete_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisualProducao(dgvPaletes);
            CarregarPaletes();
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
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            grid.RowsDefaultCellStyle.BackColor = Color.White;

            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = grid.ColumnHeadersDefaultCellStyle.BackColor;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.RowTemplate.Height = 36;
        }

        public TriagemFormPalete(int _usuarioId, string _nomeUsuario)
        {
            InitializeComponent();
            usuarioId = _usuarioId;
            nomeUsuario = FormatarNomeUsuario(_nomeUsuario);
            this.Load += FormTriagemPalete_Load;
            dgvPaletes.SelectionChanged += dgvPaletes_SelectionChanged;
        }

        private string FormatarNomeUsuario(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return string.Empty;

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(nome.Trim().ToLower());
        }

        private void AplicarCoresStatusPersonalizadas()
        {
            foreach (DataGridViewRow row in dgvPaletes.Rows)
            {
                if (row.Cells["Status"]?.Value?.ToString() == "Vendido")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 250);
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 220, 230);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100);
                    row.DefaultCellStyle.SelectionForeColor = Color.FromArgb(80, 80, 80);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    row.DefaultCellStyle.SelectionForeColor = Color.Black;
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
                }
            }
        }

        private async void CarregarPaletes()
        {
            try
            {
                using (var ctx = new ReverseContext())
                {
                    var lista = await ctx.Paletes
                        .AsNoTracking()
                        .Select(p => new
                        {
                            p.Id,
                            p.Numero,
                            p.Categoria,
                            Status = p.Status,
                            p.DataCriacao,
                            QuantidadeItens = p.Itens.Sum(i => (int?)i.Quantidade) ?? 0,
                            ValorTotal = p.Itens.Sum(i => (decimal?)i.Quantidade * i.ValorUnitario) ?? 0,
                            p.UsuarioFinalizacao,
                            p.DataFinalizacao
                        })
                        .OrderByDescending(p => p.DataCriacao)
                        .ToListAsync();

                    var listaFormatada = lista.Select(p => new
                    {
                        p.Id,
                        p.Numero,
                        Nome = $"Palete {p.Numero} - {p.Categoria.Nome}",
                        Status = ObterNomeStatus(p.Status),
                        p.DataCriacao,
                        p.QuantidadeItens,
                        p.ValorTotal,
                        UsuarioFinalizacao = FormatarNomeUsuario(p.UsuarioFinalizacao),
                        p.DataFinalizacao
                    }).ToList();

                    dgvPaletes.DataSource = listaFormatada;

                    ConfigurarColunas();

                    AplicarCoresStatusPersonalizadas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar paletes: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvPaletes_SelectionChanged(object sender, EventArgs e)
        {
            AplicarCoresStatusPersonalizadas();
        }

        private void ConfigurarColunas()
        {
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

                dgvPaletes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvPaletes.Columns["Nome"].FillWeight = 200;
                dgvPaletes.Columns["Status"].FillWeight = 100;
                dgvPaletes.Columns["DataCriacao"].FillWeight = 100;
                dgvPaletes.Columns["QuantidadeItens"].FillWeight = 80;
                dgvPaletes.Columns["ValorTotal"].FillWeight = 100;
                dgvPaletes.Columns["UsuarioFinalizacao"].FillWeight = 120;
                dgvPaletes.Columns["DataFinalizacao"].FillWeight = 120;
            }

            dgvPaletes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPaletes.MultiSelect = false;
            dgvPaletes.AllowUserToAddRows = false;
            dgvPaletes.AllowUserToDeleteRows = false;
            dgvPaletes.ReadOnly = true;
            dgvPaletes.DefaultCellStyle.ForeColor = Color.Black;
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

            try
            {
                using (var ctx = new ReverseContext())
                {
                    var palete = await ctx.Paletes.FirstOrDefaultAsync(p => p.Id == id);
                    if (palete == null)
                    {
                        MessageBox.Show("Palete não encontrada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    palete.Status = novoStatus;

                    if (salvarUsuarioFinalizacao)
                    {
                        palete.UsuarioFinalizacao = nomeUsuario;
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
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao alterar status: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            try
            {
                using (var ctx = new ReverseContext())
                {
                    var palete = await ctx.Paletes.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == id);
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
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir palete: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}