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
            CarregarPaletes();
            btnSair.Click += (s, e) => Close();
            btnCancelar.Click += (s, e) => { PaleteSelecionada = null; DialogResult = DialogResult.Cancel; Close(); };
            btnSelecionar.Click += btnSelecionar_Click;
            dgvPaletes.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) SelecionarPalete();
            };
        }


        private void PaleteSelectorForm_Load(object sender, EventArgs e)
        {
            CarregarPaletes();
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

        private string FormatarUsuario(int? usuarioId)
        {
            if (!usuarioId.HasValue) return "-";

            using (var ctx = new ReverseContext())
            {
                var usuario = ctx.Usuarios.Find(usuarioId.Value);
                return usuario?.UsuarioNome ?? "-";
            }
        }

        private void CarregarPaletes(string filtro = "")
        {
            using (var ctx = new ReverseContext())
            {
                var query = ctx.Paletes.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    query = (System.Data.Entity.Infrastructure.DbQuery<Palete>)query.Where(p =>
                        p.Numero.ToString().Contains(filtro) ||
                        p.Categoria.ToString().Contains(filtro));
                }

                var lista = query
                    .Include(p => p.Itens)
                    .ToList()
                    .Select(p => new
                    {
                        p.Id,
                        p.Numero,
                        Nome = p.Nome,
                        Status = ObterNomeStatus(p.Status),
                        p.DataCriacao,
                        UsuarioCriacao = FormatarUsuario(p.UsuarioCriacao),
                        QuantidadeItens = p.Itens.Sum(i => (int?)i.Quantidade) ?? 0,
                        ValorTotal = p.Itens.Sum(i => (decimal?)i.Quantidade * i.ValorUnitario) ?? 0
                    })
                    .OrderByDescending(p => p.DataCriacao)
                    .ToList();

                dgvPaletes.DataSource = lista;

                if (dgvPaletes.Columns.Contains("Status"))
                {
                    dgvPaletes.Columns["Status"].HeaderText = "Status";
                    dgvPaletes.Columns["Status"].DefaultCellStyle.ForeColor = Color.DarkBlue;
                }

                if (dgvPaletes.Columns.Count > 0)
                {
                    dgvPaletes.Columns["Id"].Visible = false;
                    dgvPaletes.Columns["Numero"].HeaderText = "Número";
                    dgvPaletes.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgvPaletes.Columns["DataCriacao"].HeaderText = "Data Criação";
                    dgvPaletes.Columns["DataCriacao"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvPaletes.Columns["UsuarioCriacao"].HeaderText = "Criado por";
                    dgvPaletes.Columns["QuantidadeItens"].HeaderText = "Qtd. Itens";
                    dgvPaletes.Columns["ValorTotal"].DefaultCellStyle.Format = "C2";

                    dgvPaletes.Columns["Numero"].DefaultCellStyle.Font = new Font(dgvPaletes.Font, FontStyle.Bold);
                }
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
