using Reverse.Forms.FormsAtendimento;
using Reverse.Forms.FormsComercial;
using Reverse.Forms.FormsExpedicao;
using Reverse.Forms.FormsFiscal;
using Reverse.Forms.FormsLogin;
using Reverse.Forms.FormsNotificacao;
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

namespace Reverse.Forms.FormsLogin
{
    public partial class MainFormV2 : Form
    {
        // ─── Campos de identidade do usuário ───────────────────
        private readonly string _usuario;
        private readonly string _setor;
        private readonly int _usuarioId;

        // ─── Rastreamento ──────────────────────────────────────
        private RastreadorAtividades _rastreador;

        // ─── Cache estático de permissões (thread-safe) ────────
        private static readonly object _cacheLock = new object();
        private static int? _cachedUsuarioId = null;
        private static string _cachedUsuarioNome = null;
        private static HashSet<string> _permissoesCache = null;
        private static int? _permissoesCacheUsuarioId = null;

        // ─── Notificações ──────────────────────────────────────
        private Timer _timerNotificacoes;
        private Label _lblBadge;
        private static readonly Color CorSinoNormal = Color.FromArgb(255, 255, 255);
        private static readonly Color CorSinoAlerta = Color.FromArgb(255, 180, 0);
        private const int INTERVALO_NOTIFICACAO_MS = 30_000;

        // ─── Lateral recolhível ────────────────────────────────
        private bool _lateralExpandida = false;
        private const int LARGURA_RECOLHIDA = 50;
        private const int LARGURA_EXPANDIDA = 200;

        // ─── Form filho no pnlConteudo ─────────────────────────
        private Form _formFilhoAtual;

        // ══════════════════════════════════════════════════════
        //  CONSTRUTORES
        // ══════════════════════════════════════════════════════

        public MainFormV2()
        {
            InitializeComponent();
            Rectangle area = Screen.PrimaryScreen.WorkingArea;
            this.Location = area.Location;
            this.Size = area.Size;
        }

        public MainFormV2(string usuario, string setor) : this()
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
                        var obj = ctx.Usuarios
                            .Where(u => u.UsuarioNome == usuario)
                            .Select(u => new { u.Id })
                            .FirstOrDefault();

                        if (obj != null)
                        {
                            _usuarioId = obj.Id;
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

            bool isAdm = _setor.Equals("ADM", StringComparison.OrdinalIgnoreCase);
            btnMensagem.Visible = isAdm;
            btnCDU.Visible = isAdm;

            InicializarRastreamento();
            InicializarSistemaNotificacoes();
            InicializarLateral();
        }

        // ══════════════════════════════════════════════════════
        //  RASTREAMENTO
        // ══════════════════════════════════════════════════════

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
                System.Diagnostics.Debug.WriteLine($"Erro rastreamento: {ex.Message}");
            }
        }

        // ══════════════════════════════════════════════════════
        //  NOTIFICAÇÕES
        // ══════════════════════════════════════════════════════

        private void InicializarSistemaNotificacoes()
        {
            Point posAbsoluta = btnNotificacao.PointToScreen(Point.Empty);
            Point posNoForm = this.PointToClient(posAbsoluta);

            _lblBadge = new Label
            {
                AutoSize = false,
                Size = new Size(20, 20),
                Text = "",
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Red,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 7, FontStyle.Bold),
                Visible = false,
                Location = new Point(posNoForm.X + btnNotificacao.Width - 10,
                                      posNoForm.Y - 6)
            };

            this.Controls.Add(_lblBadge);
            _lblBadge.BringToFront();

            btnNotificacao.BackColor = CorSinoNormal;
            btnNotificacao.Cursor = Cursors.Hand;

            _timerNotificacoes = new Timer { Interval = INTERVALO_NOTIFICACAO_MS };
            _timerNotificacoes.Tick += (s, e) => AtualizarNotificacoes();
            _timerNotificacoes.Start();

            AtualizarNotificacoes();
        }

        private void AtualizarNotificacoes()
        {
            try
            {
                int qtd;
                using (var ctx = new ReverseContext())
                {
                    qtd = ctx.Notificacoes
                        .Count(n =>
                            (n.UsuarioDestinatarioId == _usuarioId
                             || n.UsuarioDestinatarioId == null)
                            && !ctx.NotificacoesLidas.Any(
                                l => l.NotificacaoId == n.Id
                                  && l.UsuarioId == _usuarioId));
                }
                AtualizarVisualSino(qtd);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar notificações: {ex.Message}");
            }
        }

        private void AtualizarVisualSino(int qtd)
        {
            if (InvokeRequired) { Invoke(new Action<int>(AtualizarVisualSino), qtd); return; }

            if (qtd > 0)
            {
                btnNotificacao.BackColor = CorSinoAlerta;
                _lblBadge.Text = qtd > 99 ? "99+" : qtd.ToString();
                _lblBadge.Visible = true;
            }
            else
            {
                btnNotificacao.BackColor = CorSinoNormal;
                _lblBadge.Visible = false;
            }
        }

        private void btnNotificacao_Click(object sender, EventArgs e)
        {
            _timerNotificacoes.Stop();
            using (var form = new FormNotificacoes(_usuarioId))
            {
                form.NotificacoesAlteradas += (s, ev) => AtualizarNotificacoes();
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
            AtualizarNotificacoes();
            _timerNotificacoes.Start();
        }

        private void btnMensagem_Click(object sender, EventArgs e)
        {
            using var form = new FormMensagem(_usuarioId);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowDialog();
        }

        // ══════════════════════════════════════════════════════
        //  LATERAL RECOLHÍVEL
        // ══════════════════════════════════════════════════════

        private void InicializarLateral()
        {
            btnTriagem.Tag = "Triagem";
            btnExpedicao.Tag = "Expedição";
            btnRH.Tag = "RH";
            btnFinanceiro.Tag = "Financeiro";
            btnComercial.Tag = "Comercial";
            btnFiscal.Tag = "Fiscal";
            btnProducao.Tag = "Produção";
            btnChamado.Tag = "Chamado";
            btnCDU.Tag = "CDU";
            btnMensagem.Tag = "Mensagem";
            btnSair.Tag = "Sair";
            btnNotificacao.Tag = "Notificação";

            foreach (Button btn in new[] {
            btnTriagem, btnExpedicao, btnRH, btnFinanceiro,
            btnComercial, btnFiscal, btnProducao, btnChamado,
            btnCDU, btnMensagem, btnSair, btnNotificacao })
            {
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.ImageAlign = ContentAlignment.MiddleRight;
            }

            AplicarEstadoLateral(false);
        }

        private void btnReverse_Click(object sender, EventArgs e)
        {
            _lateralExpandida = !_lateralExpandida;
            AplicarEstadoLateral(_lateralExpandida);
        }

        private void AplicarEstadoLateral(bool expandida)
        {
            if (expandida)
            {
                tlpLateral.Width = LARGURA_EXPANDIDA;
                btnReverse.Text = "Reverse";

                foreach (Button btn in new[] {
                btnTriagem, btnExpedicao, btnRH, btnFinanceiro,
                btnComercial, btnFiscal, btnProducao, btnChamado,
                btnCDU, btnMensagem, btnSair, btnNotificacao })
                {
                    btn.ImageAlign = ContentAlignment.MiddleRight;
                    btn.Text = (string)btn.Tag;
                }

            }
            else
            {
                tlpLateral.Width = LARGURA_RECOLHIDA;
                btnReverse.Text = "R";

                foreach (Button btn in new[] {
                btnTriagem, btnExpedicao, btnRH, btnFinanceiro,
                btnComercial, btnFiscal, btnProducao, btnChamado,
                btnCDU, btnMensagem, btnSair, btnNotificacao })
                {
                    btn.Text = "";
                    btn.ImageAlign = ContentAlignment.MiddleCenter;
                }
            }
        }

        // ══════════════════════════════════════════════════════
        //  PERMISSÕES
        // ══════════════════════════════════════════════════════

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

        // ══════════════════════════════════════════════════════
        //  ABERTURA DE MÓDULOS NO pnlConteudo
        // ══════════════════════════════════════════════════════

        private void AbrirModulo(string formName, Func<Form> criarForm)
        {
            if (!TemPermissao(formName))
            {
                MessageBox.Show(
                    $"Você não tem permissão para acessar " +
                    $"{formName.Replace("Form", "").Replace("Hub", "")}.",
                    "Acesso negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            pnlConteudo.AutoScroll = true;
            pnlConteudo.AutoScrollMargin = new Size(0, 0);
            pnlConteudo.Padding = new Padding(0);

            if (_formFilhoAtual != null && !_formFilhoAtual.IsDisposed)
            {
                _formFilhoAtual.Close();
                _formFilhoAtual.Dispose();
                _formFilhoAtual = null;
            }

            pnlConteudo.Controls.Clear();

            var formFilho = criarForm();
            _formFilhoAtual = formFilho;

            formFilho.TopLevel = false;
            formFilho.FormBorderStyle = FormBorderStyle.None;
            formFilho.Dock = DockStyle.None;

            pnlConteudo.Controls.Add(formFilho);
            AjustarFormFilho();
            formFilho.Show();

            _rastreador?.RegistrarAberturaFormulario(formFilho);
        }

        private void AjustarFormFilho()
        {
            if (_formFilhoAtual == null) return;

            pnlConteudo.AutoScrollPosition = new Point(0, 0);
            pnlConteudo.AutoScrollMinSize = Size.Empty;

            _formFilhoAtual.Location = Point.Empty;
            _formFilhoAtual.Margin = new Padding(0);
            _formFilhoAtual.Padding = new Padding(0);
            _formFilhoAtual.Size = pnlConteudo.ClientSize;

            pnlConteudo.PerformLayout();
            _formFilhoAtual.PerformLayout();
        }

        // ══════════════════════════════════════════════════════
        //  CLIQUES DOS BOTÕES DE MÓDULO
        // ══════════════════════════════════════════════════════

        private void btnTriagem_Click(object sender, EventArgs e) =>
            AbrirModulo("TriagemFormHub", () => new TriagemFormHub(_usuarioId));

        private void btnExpedicao_Click(object sender, EventArgs e) =>
            AbrirModulo("ExpedicaoFormExpHub", () => new ExpedicaoFormExpHub(_usuarioId));

        private void btnRH_Click(object sender, EventArgs e) =>
            AbrirModulo("RHFormRHHub", () => new RHFormRHHub(_usuarioId));

        private void btnFinanceiro_Click(object sender, EventArgs e) =>
            AbrirModulo("FinanceiroFormFinanceiroHub", () => new FinanceiroFormFinanceiroHub(_usuarioId));

        private void btnComercial_Click(object sender, EventArgs e) =>
            AbrirModulo("ComercialFormComercialHub", () => new ComercialFormComercialHub(_usuarioId));

        private void btnFiscal_Click(object sender, EventArgs e) =>
            AbrirModulo("FiscalFormHub", () => new FiscalFormHub(_usuarioId));

        private void btnProducao_Click(object sender, EventArgs e) =>
            AbrirModulo("ProducaoFormHub", () => new ProducaoFormHub(_usuarioId));

        private void btnChamado_Click(object sender, EventArgs e) =>
            AbrirModulo("AtendimentoFormHub", () => new AtendimentoFormHub(_usuarioId));

        private void btnCDU_Click(object sender, EventArgs e)
        {
            using var config = new FormConfigU();
            config.StartPosition = FormStartPosition.CenterScreen;
            config.ShowDialog();
        }

        // ══════════════════════════════════════════════════════
        //  REDIMENSIONAMENTO
        // ══════════════════════════════════════════════════════

        private void pnlConteudo_Resize(object sender, EventArgs e) =>
            AjustarFormFilho();

        // ══════════════════════════════════════════════════════
        //  SAIR / FECHAR
        // ══════════════════════════════════════════════════════

        private void btnSair_Click(object sender, EventArgs e)
        {
            _rastreador?.EncerrarSessao();
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerNotificacoes?.Stop();
            _timerNotificacoes?.Dispose();

            if (_formFilhoAtual != null && !_formFilhoAtual.IsDisposed)
            {
                _formFilhoAtual.Close();
                _formFilhoAtual.Dispose();
            }

            _rastreador?.EncerrarSessao();
            base.OnFormClosing(e);
        }
    }
}