using Reverse.Forms.FormsAtendimento;
using Reverse.Forms.FormsComercial;
using Reverse.Forms.FormsExpedicao;
using Reverse.Forms.FormsFiscal;
using Reverse.Forms.FormsLogin;
using Reverse.Forms.FormsProducao;
using Reverse.Forms.FormsRH;
using Reverse.Forms.FormsTriagem;
using Reverse.Models;
using SeuProjeto;
using Reverse.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        private RastreadorAtividades _rastreador;
        private static readonly object _cacheLock = new object();
        private static int? _cachedUsuarioId = null;
        private static string _cachedUsuarioNome = null;
        private static HashSet<string> _permissoesCache = null;
        private static int? _permissoesCacheUsuarioId = null;

        public MainForm()
        {
            InitializeComponent();

            Rectangle areaTrabalho = Screen.PrimaryScreen.WorkingArea;
            this.Location = areaTrabalho.Location;
            this.Size = areaTrabalho.Size;
        }

        public MainForm(string usuario, string setor) : this()
        {
            _usuario = usuario;
            _setor = setor;

            lock (_cacheLock)
            {
                if (_cachedUsuarioNome == usuario && _cachedUsuarioId.HasValue)
                {
                    _usuarioId = _cachedUsuarioId.Value;
                }
                else
                {
                    using (var ctx = new ReverseContext())
                    {
                        var usuarioObj = ctx.Usuarios
                            .Where(u => u.UsuarioNome == usuario)
                            .Select(u => new { u.Id })
                            .FirstOrDefault();

                        if (usuarioObj != null)
                        {
                            _usuarioId = usuarioObj.Id;
                            _cachedUsuarioId = _usuarioId;
                            _cachedUsuarioNome = usuario;
                        }
                        else
                        {
                            MessageBox.Show("Usuário não encontrado no sistema.",
                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                            return;
                        }
                    }
                }
            }
            string saudacao = ObterSaudacao();
            lblGreeting.Text = $"{saudacao}, {_usuario}!";
            btnConfiguracao.Visible = _setor.Equals("ADM", StringComparison.OrdinalIgnoreCase);

            InicializarRastreamento();
        }

        private void InicializarRastreamento()
        {
            try
            {
                _rastreador = new RastreadorAtividades(_usuarioId, _usuario);
                _rastreador.IniciarSessao();
                _rastreador.RegistrarAberturaFormulario(this);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao inicializar rastreamento: {ex.Message}");
            }
        }

        private static string ObterSaudacao()
        {
            int hora = DateTime.Now.Hour;
            if (hora < 12) return "Bom dia";
            if (hora < 18) return "Boa tarde";
            return "Boa noite";
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
            lock (_cacheLock)
            {
                if (_permissoesCacheUsuarioId != _usuarioId || _permissoesCache == null)
                {
                    using (var ctx = new ReverseContext())
                    {
                        _permissoesCache = ctx.Permissoes
                            .Where(p => p.UsuarioId == _usuarioId && p.PodeAcessar)
                            .Select(p => p.FormName)
                            .ToHashSet();
                        _permissoesCacheUsuarioId = _usuarioId;
                    }
                }

                return _permissoesCache.Contains(formName);
            }
        }

        private void AbrirFormComPermissao(string formName, Func<Form> criarForm)
        {
            if (!TemPermissao(formName))
            {
                MessageBox.Show($"Você não tem permissão para acessar {formName.Replace("Form", "").Replace("Hub", "")}.",
                    "Acesso negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var form = criarForm();
            form.StartPosition = FormStartPosition.CenterScreen;
            _rastreador?.RegistrarAberturaFormulario(form);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void btnConfiguracao_Click(object sender, EventArgs e)
        {
            using var config = new FormConfigU();
            config.StartPosition = FormStartPosition.CenterScreen;
            config.ShowDialog();
        }

        private void picTriagem_Click(object sender, EventArgs e) =>
            AbrirFormComPermissao("TriagemFormHub", () => new TriagemFormHub(_usuarioId));

        private void picExp_Click(object sender, EventArgs e) =>
            AbrirFormComPermissao("ExpedicaoFormExpHub", () => new ExpedicaoFormExpHub(_usuarioId));

        private void picRH_Click(object sender, EventArgs e) =>
            AbrirFormComPermissao("RHFormRHHub", () => new RHFormRHHub(_usuarioId));

        private void picFinanceiro_Click(object sender, EventArgs e) =>
            AbrirFormComPermissao("FinanceiroFormFinanceiroHub", () => new FinanceiroFormFinanceiroHub(_usuarioId));

        private void picComercial_Click(object sender, EventArgs e) =>
            AbrirFormComPermissao("ComercialFormComercialHub", () => new ComercialFormComercialHub(_usuarioId));

        private void picFiscal_Click(object sender, EventArgs e) =>
            AbrirFormComPermissao("FiscalFormHub", () => new FiscalFormHub(_usuarioId));

        private void picProducao_Click(object sender, EventArgs e) =>
            AbrirFormComPermissao("ProducaoFormHub", () => new ProducaoFormHub(_usuarioId));
        private void picAtendimento_Click(object sender, EventArgs e) =>
            AbrirFormComPermissao("AtendimentoFormHub", () => new AtendimentoFormHub(_usuarioId));

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _rastreador?.EncerrarSessao();
            base.OnFormClosing(e);
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            _rastreador?.EncerrarSessao();
            this.Close();
        }
    }
}