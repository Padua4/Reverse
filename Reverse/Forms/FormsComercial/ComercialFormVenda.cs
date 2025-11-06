using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms.FormsComercial
{
    public partial class ComercialFormVenda : Form
    {
        private int usuarioId;

        public ComercialFormVenda(int usuarioId)
        {
            this.usuarioId = usuarioId;
            InitializeComponent();
        }
    }
}
