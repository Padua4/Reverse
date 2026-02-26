using Reverse.Models;
using System;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reverse.Forms.FormsTriagem
{
    public partial class TriagemPaleteSelectorForm : Form
    {
        public Palete PaleteSelecionada { get; private set; }

        public TriagemPaleteSelectorForm()
        {
            InitializeComponent();
            Load += PaleteSelectorForm_Load;
            txtBuscar.TextChanged += TxtBuscar_TextChanged;
            btnAtualizar.Click += BtnAtualizar_Click;

            btnSair.Click += (s, e) => Close();
            btnCancelar.Click += (s, e) => { PaleteSelecionada = null; DialogResult = DialogResult.Cancel; Close(); };
            btnSelecionar.Click += btnSelecionar_Click;
            dgvPaletes.RowPrePaint += DgvPaletes_RowPrePaint;
            dgvPaletes.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) SelecionarPalete();
            };
        }

        private void PaleteSelectorForm_Load(object sender, EventArgs e)
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

        private void BtnAtualizar_Click(object sender, EventArgs e)
        {
            CarregarPaletes();
        }

        private void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            CarregarPaletes(txtBuscar.Text);
        }

        private void SelecionarPalete()
        {
            if (dgvPaletes.CurrentRow?.DataBoundItem is Palete pal)
            {
                PaleteSelecionada = pal;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Selecione uma palete válida.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CarregarPaletes(string filtro = "")
        {
            try
            {
                using (var ctx = new ReverseContext())
                {
                    var query = ctx.Paletes
                        .AsNoTracking()
                        .Include(p => p.Categoria)
                        .Include(p => p.Itens);

                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        int numeroFiltro;
                        bool isNumero = int.TryParse(filtro, out numeroFiltro);

                        if (isNumero)
                        {
                            query = (System.Data.Entity.Infrastructure.DbQuery<Palete>)query
                                .Where(p => p.Numero == numeroFiltro);
                        }
                        else
                        {
                            query = (System.Data.Entity.Infrastructure.DbQuery<Palete>)query
                                .Where(p => p.Categoria.Nome.Contains(filtro));
                        }
                    }

                    var lista = query
                        .OrderByDescending(p => p.DataCriacao)
                        .Take(500)
                        .ToList()
                        .Select(p => new
                        {
                            p.Id,
                            p.Numero,
                            Nome = p.Nome,
                            Status = ObterNomeStatus(p.Status),
                            p.DataCriacao,
                            UsuarioCriacao = FormatarUsuario(p.UsuarioCriacao),
                            QuantidadeItens = p.Itens?.Sum(i => (int?)i.Quantidade) ?? 0,
                            ValorTotal = p.Itens?.Sum(i => (decimal?)(i.Quantidade * i.ValorUnitario)) ?? 0
                        })
                        .ToList();

                    dgvPaletes.DataSource = lista;

                    if (dgvPaletes.Columns.Count > 0)
                    {
                        dgvPaletes.Columns["Id"].Visible = false;
                        dgvPaletes.Columns["Numero"].HeaderText = "Número";
                        dgvPaletes.Columns["Numero"].DefaultCellStyle.Font = new Font(dgvPaletes.Font, FontStyle.Bold);
                        dgvPaletes.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgvPaletes.Columns["Status"].HeaderText = "Status";
                        dgvPaletes.Columns["Status"].DefaultCellStyle.ForeColor = Color.DarkBlue;
                        dgvPaletes.Columns["DataCriacao"].HeaderText = "Data Criação";
                        dgvPaletes.Columns["DataCriacao"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                        dgvPaletes.Columns["UsuarioCriacao"].HeaderText = "Criado por";
                        dgvPaletes.Columns["QuantidadeItens"].HeaderText = "Qtd. Itens";
                        dgvPaletes.Columns["ValorTotal"].HeaderText = "Valor Total";
                        dgvPaletes.Columns["ValorTotal"].DefaultCellStyle.Format = "C2";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar paletes: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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

        private void DgvPaletes_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvPaletes.Rows.Count) return;

            var row = dgvPaletes.Rows[e.RowIndex];
            var status = row.Cells["Status"]?.Value?.ToString() ?? "";

            switch (status)
            {
                case "Aberto":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235);
                    break;
                case "Em andamento":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(239, 246, 255);
                    break;
                case "Finalizado":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244);
                    break;
                case "Vendido":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(245, 243, 255);
                    break;
                default:
                    row.DefaultCellStyle.BackColor = Color.White;
                    break;
            }
        }

        private string FormatarUsuario(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return "-";

            usuario = usuario.Trim().ToLower();
            return char.ToUpper(usuario[0]) + usuario.Substring(1);
        }

        private void btnSelecionar_Click(object sender, EventArgs e)
        {
            if (dgvPaletes.CurrentRow == null)
            {
                MessageBox.Show("Nenhuma palete selecionada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var id = (int)dgvPaletes.CurrentRow.Cells["Id"].Value;

            using (var ctx = new ReverseContext())
            {
                PaleteSelecionada = ctx.Paletes
                    .Include(p => p.Itens.Select(i => i.Produto))
                    .FirstOrDefault(p => p.Id == id);
            }

            if (PaleteSelecionada == null)
            {
                MessageBox.Show("Não foi possível carregar os detalhes da palete.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}