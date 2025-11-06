using Reverse.Forms.FormsComercial;
using Reverse.Forms.FormsExpedicao;
using Reverse.Forms.FormsLogin;
using Reverse.Forms.FormsRH;
using Reverse.Forms.FormsTriagem;
using Reverse.Models;
using SeuProjeto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Reverse.Forms
{
    public partial class MainForm : Form
    {
        private readonly string _usuario;
        private readonly string _setor;
        private readonly int _usuarioId;
        private static int? _cachedUsuarioId = null;
        private static string _cachedUsuarioNome = null;
        private static HashSet<string> _permissoesCache = null;
        private static int? _usuarioIdCache = null;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(string usuario, string setor)
            : this()
        {
            _usuario = usuario;
            _setor = setor;

            if (_cachedUsuarioNome == usuario && _cachedUsuarioId.HasValue)
            {
                _usuarioId = _cachedUsuarioId.Value;
            }
            else
            {
                using (var ctx = new ReverseContext())
                {
                    var usuarioObj = ctx.Usuarios.FirstOrDefault(u => u.UsuarioNome == usuario);
                    if (usuarioObj != null)
                    {
                        _usuarioId = usuarioObj.Id;
                        _cachedUsuarioId = _usuarioId;
                        _cachedUsuarioNome = usuario;
                    }
                }
            }

            string saudacao = DateTime.Now.Hour < 12 ? "Bom dia" :
                              DateTime.Now.Hour < 18 ? "Boa tarde" : "Boa noite";

            lblGreeting.Text = $"{saudacao}, {_usuario}!";
            btnConfiguracao.Visible = _setor.Equals("ADM", StringComparison.OrdinalIgnoreCase);
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        private void panelTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private bool TemPermissao(string formName)
        {
            if (_usuarioIdCache != _usuarioId || _permissoesCache == null)
            {
                using (var ctx = new ReverseContext())
                {
                    _permissoesCache = ctx.Permissoes
                        .Where(p => p.UsuarioId == _usuarioId && p.PodeAcessar)
                        .Select(p => p.FormName)
                        .ToHashSet();
                    _usuarioIdCache = _usuarioId;
                }
            }

            return _permissoesCache.Contains(formName);
        }

        private void btnConfiguracao_Click(object sender, EventArgs e)
        {
            using var config = new FormConfigU();
            config.StartPosition = FormStartPosition.CenterScreen;
            config.ShowDialog();
        }

        private void picTriagem_Click(object sender, EventArgs e)
        {
            if (!TemPermissao("TriagemFormHub"))
            {
                MessageBox.Show("Você não tem permissão para acessar Triagem.",
                                "Acesso negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var tri = new TriagemFormHub(_usuarioId)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            Hide();
            tri.ShowDialog();
            Show();
        }
        private void picExp_Click(object sender, EventArgs e)
        {
            if (!TemPermissao("ExpedicaoFormExpHub"))
            {
                MessageBox.Show("Você não tem permissão para acessar este módulo.",
                                "Acesso negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var expHub = new ExpedicaoFormExpHub(_usuarioId)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            Hide();
            expHub.ShowDialog();
            Show();
        }

        private void picRH_Click(object sender, EventArgs e)
        {
            if (!TemPermissao("RHFormRHHub"))
            {
                MessageBox.Show("Você não tem permissão para acessar RH.",
                                "Acesso negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var rhHub = new RHFormRHHub(_usuarioId)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            Hide();
            rhHub.ShowDialog();
            Show();
        }
        private void picFinanceiro_Click(object sender, EventArgs e)
        {
            if (!TemPermissao("FinanceiroFormFinanceiroHub"))
            {
                MessageBox.Show("Você não tem permissão para acessar Financeiro.",
                                "Acesso negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var finHub = new FinanceiroFormFinanceiroHub(_usuarioId)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            Hide();
            finHub.ShowDialog();
            Show();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void picComercial_Click(object sender, EventArgs e)
        {
            if (!TemPermissao("ComercialFormComercialHub"))
            {
                MessageBox.Show("Você não tem permissão para acessar Comercial.",
                                "Acesso negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var fincom = new ComercialFormComercialHub(_usuarioId)
            {
                StartPosition = FormStartPosition.CenterScreen
            };
            Hide();
            fincom.ShowDialog();
            Show();
        }
    }
}
