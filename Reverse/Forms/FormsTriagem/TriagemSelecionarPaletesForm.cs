using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using Reverse.Models;
using System.Runtime.InteropServices;

namespace Reverse.Forms
{
    public partial class TriagemSelecionarPaletesForm : Form
    {
        public List<int> PaletesSelecionados { get; private set; } = new List<int>();

        public TriagemSelecionarPaletesForm()
        {
            InitializeComponent();
            Load += SelecionarPaletesForm_Load;
        }

        private void SelecionarPaletesForm_Load(object sender, EventArgs e)
        {
            CarregarPaletes();
        }

        private class PaleteListItem
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public override string ToString() => Nome;
        }

        private void CarregarPaletes()
        {
            using (var ctx = new ReverseContext())
            {
                var lista = ctx.Paletes
                    .AsNoTracking()
                    .Include(p => p.Categoria)
                    .Select(p => new
                    {
                        p.Id,
                        p.Numero,
                        CategoriaNome = p.Categoria.Nome
                    })
                    .AsEnumerable()
                    .Select(p => new PaleteListItem
                    {
                        Id = p.Id,
                        Nome = $"Palete {p.Numero} - {p.CategoriaNome}"
                    })
                    .OrderByDescending(p => p.Id)
                    .ToList();

                clbPaletes.Items.Clear();
                foreach (var item in lista)
                    clbPaletes.Items.Add(item, false);
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            PaletesSelecionados = clbPaletes.CheckedItems
                .Cast<PaleteListItem>()
                .Select(x => x.Id)
                .ToList();

            if (PaletesSelecionados.Count == 0)
            {
                MessageBox.Show("Selecione pelo menos uma palete.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}