using Reverse.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Reverse.Helpers
{
    /// <summary>
    /// Rastreador de atividades OTIMIZADO
    /// - Registra apenas mudanças de formulário (não movimentos constantes)
    /// - Rastreia formulários MDI filhos automaticamente
    /// - Não enche o banco de dados
    /// </summary>
    public class RastreadorAtividades
    {
        private int _idUsuarioAtual;
        private string _nomeUsuarioAtual;
        private string _ultimoFormularioRegistrado;
        private DateTime _ultimoRegistro;
        private const int INTERVALO_MINIMO_MINUTOS = 5;

        public RastreadorAtividades(int idUsuario, string nomeUsuario)
        {
            _idUsuarioAtual = idUsuario;
            _nomeUsuarioAtual = nomeUsuario;
            _ultimoRegistro = DateTime.MinValue;
            _ultimoFormularioRegistrado = string.Empty;
        }

        #region Métodos Públicos

        /// <summary>
        /// Registra a abertura de um formulário (APENAS quando realmente muda de form)
        /// </summary>
        public void RegistrarAberturaFormulario(Form form)
        {
            if (form == null) return;

            try
            {
                string nomeFormulario = ObterNomeFormularioReal(form);

                TimeSpan diferenca = DateTime.Now - _ultimoRegistro;
                bool mesmForm = nomeFormulario == _ultimoFormularioRegistrado;
                bool tempoMinimo = diferenca.TotalMinutes >= INTERVALO_MINIMO_MINUTOS;

                if (mesmForm && !tempoMinimo)
                {
                    // Ignora - mesmo formulário em menos de 5 minutos
                    return;
                }

                // Registrar apenas a mudança de formulário
                RegistrarAtividade(nomeFormulario, "Abriu");

                _ultimoFormularioRegistrado = nomeFormulario;

                // Monitorar formulários MDI filhos se aplicável
                if (form.IsMdiContainer)
                {
                    MonitorarFormulariosMDI(form);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao registrar abertura: {ex.Message}");
            }
        }

        /// <summary>
        /// Inicia uma nova sessão de usuário
        /// </summary>
        public int IniciarSessao()
        {
            try
            {
                using (var ctx = new ReverseContext())
                {
                    // Encerrar sessões anteriores que ficaram abertas
                    var sessoesAbertas = ctx.SessoesUsuarios
                        .Where(s => s.IdUsuario == _idUsuarioAtual && s.StatusSessao == "Ativo")
                        .ToList();

                    foreach (var sessao in sessoesAbertas)
                    {
                        sessao.StatusSessao = "Encerrado";
                        sessao.DataHoraLogout = DateTime.Now;
                    }

                    // Criar nova sessão
                    var novaSessao = new SessaoUsuario
                    {
                        IdUsuario = _idUsuarioAtual,
                        NomeUsuario = _nomeUsuarioAtual,
                        DataHoraLogin = DateTime.Now,
                        StatusSessao = "Ativo",
                        NomeMaquina = Environment.MachineName,
                        EnderecoIP = ObterIP()
                    };

                    ctx.SessoesUsuarios.Add(novaSessao);
                    ctx.SaveChanges();

                    return novaSessao.IdSessao;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao iniciar sessão: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Encerra a sessão do usuário
        /// </summary>
        public void EncerrarSessao()
        {
            try
            {
                using (var ctx = new ReverseContext())
                {
                    var sessoesAtivas = ctx.SessoesUsuarios
                        .Where(s => s.IdUsuario == _idUsuarioAtual && s.StatusSessao == "Ativo")
                        .ToList();

                    foreach (var sessao in sessoesAtivas)
                    {
                        sessao.StatusSessao = "Encerrado";
                        sessao.DataHoraLogout = DateTime.Now;
                    }

                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao encerrar sessão: {ex.Message}");
            }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// OTIMIZAÇÃO 2: Obtém o nome REAL do formulário (filho MDI, não o pai/hub)
        /// </summary>
        private string ObterNomeFormularioReal(Form form)
        {
            // Se for um container MDI (Hub), pega o filho ativo
            if (form.IsMdiContainer && form.ActiveMdiChild != null)
            {
                return form.ActiveMdiChild.GetType().Name;
            }

            // Caso contrário, retorna o próprio form
            return form.GetType().Name;
        }

        /// <summary>
        /// OTIMIZAÇÃO 3: Monitora mudanças de formulários MDI filhos
        /// </summary>
        private void MonitorarFormulariosMDI(Form containerMDI)
        {
            // Adiciona evento para quando um novo formulário MDI filho é ativado
            containerMDI.MdiChildActivate -= ContainerMDI_MdiChildActivate;
            containerMDI.MdiChildActivate += ContainerMDI_MdiChildActivate;
        }

        /// <summary>
        /// Evento disparado quando um formulário MDI filho é ativado
        /// </summary>
        private void ContainerMDI_MdiChildActivate(object sender, EventArgs e)
        {
            Form container = sender as Form;
            if (container?.ActiveMdiChild != null)
            {
                string nomeFormFilho = container.ActiveMdiChild.GetType().Name;

                // Só registra se mudou de form filho
                if (nomeFormFilho != _ultimoFormularioRegistrado)
                {
                    RegistrarAtividade(nomeFormFilho, "Navegou");
                    _ultimoFormularioRegistrado = nomeFormFilho;
                }
            }
        }

        /// <summary>
        /// Registra uma atividade no banco de dados (OTIMIZADO)
        /// </summary>
        private void RegistrarAtividade(string nomeFormulario, string tipoAcao = null, string detalhes = null)
        {
            try
            {
                using (var ctx = new ReverseContext())
                {
                    var atividade = new AtividadeUsuario
                    {
                        IdUsuario = _idUsuarioAtual,
                        NomeUsuario = _nomeUsuarioAtual,
                        NomeFormulario = nomeFormulario,
                        TipoAcao = tipoAcao,
                        Detalhes = detalhes,
                        DataHoraAtividade = DateTime.Now
                    };

                    ctx.AtividadesUsuarios.Add(atividade);
                    ctx.SaveChanges();

                    _ultimoRegistro = DateTime.Now;

                    // Limpar atividades antigas (>90 dias) - executa apenas 1x por dia
                    LimparAtividadesAntigasSeMudarDia(ctx);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao registrar atividade: {ex.Message}");
            }
        }

        /// <summary>
        /// OTIMIZAÇÃO 4: Limpa atividades antigas apenas 1x por dia
        /// </summary>
        private static DateTime _ultimaLimpeza = DateTime.MinValue;
        private void LimparAtividadesAntigasSeMudarDia(ReverseContext ctx)
        {
            try
            {
                // Só executa limpeza 1x por dia
                if (_ultimaLimpeza.Date == DateTime.Today)
                    return;

                var dataLimite = DateTime.Now.AddDays(-90);
                var atividadesAntigas = ctx.AtividadesUsuarios
                    .Where(a => a.DataHoraAtividade < dataLimite)
                    .ToList();

                if (atividadesAntigas.Any())
                {
                    ctx.AtividadesUsuarios.RemoveRange(atividadesAntigas);
                    ctx.SaveChanges();
                    _ultimaLimpeza = DateTime.Today;
                }
            }
            catch
            {
                // Ignora erros na limpeza
            }
        }

        /// <summary>
        /// Obtém o IP local
        /// </summary>
        private string ObterIP()
        {
            try
            {
                var host = System.Net.Dns.GetHostName();
                var ips = System.Net.Dns.GetHostAddresses(host);

                foreach (var ip in ips)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ip.ToString();
                }

                return "Não identificado";
            }
            catch
            {
                return "Não identificado";
            }
        }

        #endregion
    }
}