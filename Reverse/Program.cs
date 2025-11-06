using PdfSharp.Fonts;
using Reverse.Forms;
using Reverse.Forms.FormsExpedicao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse
{
    internal static class Program
    {

        [STAThread]
        static void Main()
        {
            GlobalFontSettings.FontResolver = new CustomFontResolver();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using var login = new LoginForm();
            if (login.ShowDialog() == DialogResult.OK)
            {
                var main = new MainForm(
                    login.UsuarioLogado,
                    login.SetorLogado
                );

                Application.Run(main);
            }
        }
    }
}


