using Reverse.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Reverse
{
    public partial class TriagemPaleteDialog : Form
    {
        public CategoriaPalete CategoriaSelecionada =>
            (CategoriaPalete)cmbCategoria.SelectedValue;

        public TriagemPaleteDialog()
        {
            InitializeComponent();
            btnOk.DialogResult = DialogResult.OK;
            btnCancel.DialogResult = DialogResult.Cancel;
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            var itens = Enum.GetValues(typeof(CategoriaPalete))
                            .Cast<CategoriaPalete>()
                            .Select(c => new
                            {
                                Value = c,
                                Text = c.GetDescription()
                            })
                            .ToList();

            cmbCategoria.DataSource = itens;
            cmbCategoria.ValueMember = "Value";
            cmbCategoria.DisplayMember = "Text";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void PaleteDialog_Load(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
