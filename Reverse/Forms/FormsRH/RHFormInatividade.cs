using iTextSharp.text;
using iTextSharp.text.pdf;
using Reverse.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Font = System.Drawing.Font;

namespace Reverse.Forms.FormsRH
{
    public partial class RHFormInatividade : Form
    {
        private Timer timerAtualizacao;
        private DateTime proximaAtualizacao;
        private const int INTERVALO_ATUALIZACAO_MINUTOS = 5;

        public RHFormInatividade(int _usuarioId)
        {
            InitializeComponent();
            ConfigurarGrid();
            ConfigurarTimer();
        }

        #region Inicialização

        private void RHFormInatividade_Load(object sender, EventArgs e)
        {
            CarregarComboFuncionarios();
            CarregarComboLimite();
            AtualizarDados();
            IniciarContagemProximaAtualizacao();
        }

        private void ConfigurarGrid()
        {
            dgvInatividade.RowHeadersVisible = false;
            dgvInatividade.BorderStyle = BorderStyle.FixedSingle;
            dgvInatividade.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvInatividade.EnableHeadersVisualStyles = false;
            dgvInatividade.MultiSelect = false;
            dgvInatividade.ReadOnly = true;
            dgvInatividade.AllowUserToAddRows = false;
            dgvInatividade.AllowUserToDeleteRows = false;
            dgvInatividade.AllowUserToResizeRows = false;
            dgvInatividade.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvInatividade.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInatividade.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Fontes
            dgvInatividade.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvInatividade.DefaultCellStyle.ForeColor = Color.Black;
            dgvInatividade.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvInatividade.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Cores de fundo
            dgvInatividade.BackgroundColor = Color.FromArgb(250, 250, 252);
            dgvInatividade.GridColor = Color.FromArgb(230, 230, 235);

            // Cabeçalho
            dgvInatividade.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvInatividade.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvInatividade.ColumnHeadersHeight = 40;
            dgvInatividade.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvInatividade.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvInatividade.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 3, 0, 3);

            // Cores das linhas alternadas
            dgvInatividade.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            dgvInatividade.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgvInatividade.RowsDefaultCellStyle.BackColor = Color.White;
            dgvInatividade.RowsDefaultCellStyle.ForeColor = Color.Black;

            // Seleção
            dgvInatividade.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            dgvInatividade.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvInatividade.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgvInatividade.ColumnHeadersDefaultCellStyle.BackColor;
            dgvInatividade.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // Padding
            dgvInatividade.DefaultCellStyle.Padding = new Padding(3, 5, 3, 5);

            // Altura das linhas
            dgvInatividade.RowTemplate.Height = 35;
            dgvInatividade.RowTemplate.MinimumHeight = 34;

            dgvInatividade.AutoGenerateColumns = false;
            dgvInatividade.Columns.Clear();

            // Total de FillWeight = 100

            // Coluna Usuário (25%)
            dgvInatividade.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUsuario",
                HeaderText = "Usuário",
                DataPropertyName = "Usuario",
                FillWeight = 25
            });

            // Coluna Setor (25%)
            dgvInatividade.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSetor",
                HeaderText = "Formulario Acessado",
                DataPropertyName = "SetorAmigavel",
                FillWeight = 25
            });

            // Coluna Data (15%)
            dgvInatividade.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colData",
                HeaderText = "Data",
                DataPropertyName = "DataFormatada",
                FillWeight = 15
            });

            // Coluna Hora (12%)
            dgvInatividade.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHora",
                HeaderText = "Hora",
                DataPropertyName = "HoraFormatada",
                FillWeight = 12
            });

            // Coluna Tempo Decorrido (15%)
            dgvInatividade.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTempoAtivo",
                HeaderText = "Tempo Decorrido",
                DataPropertyName = "TempoAtivo",
                FillWeight = 15
            });

            // Coluna Status (8%)
            dgvInatividade.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colStatus",
                HeaderText = "Status",
                DataPropertyName = "StatusTexto",
                FillWeight = 8
            });

            // Eventos de hover
            dgvInatividade.CellMouseEnter += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    dgvInatividade.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                    dgvInatividade.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
            };

            dgvInatividade.CellMouseLeave += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (e.RowIndex % 2 == 0)
                    {
                        dgvInatividade.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                        dgvInatividade.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        dgvInatividade.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
                        dgvInatividade.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            };

            // Seleção em negrito
            dgvInatividade.SelectionChanged += (sender, e) =>
            {
                if (dgvInatividade.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvInatividade.Rows)
                    {
                        row.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    dgvInatividade.SelectedRows[0].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    dgvInatividade.SelectedRows[0].DefaultCellStyle.ForeColor = Color.Black;
                }
            };

            dgvInatividade.CellFormatting += DgvInatividade_CellFormatting;
        }

        private void ConfigurarTimer()
        {
            timerAtualizacao = new Timer();
            timerAtualizacao.Interval = 60000;
            timerAtualizacao.Tick += TimerAtualizacao_Tick;
            timerAtualizacao.Start();
        }

        private void CarregarComboFuncionarios()
        {
            cmbFuncionario.Items.Clear();
            cmbFuncionario.Items.Add(new KeyValuePair<int, string>(0, "Todos"));

            var usuarios = ObterUsuarios();
            foreach (var usuario in usuarios)
            {
                cmbFuncionario.Items.Add(usuario);
            }

            cmbFuncionario.DisplayMember = "Value";
            cmbFuncionario.ValueMember = "Key";
            cmbFuncionario.SelectedIndex = 0;
        }

        private void CarregarComboLimite()
        {
            cmbLimite.Items.Clear();
            cmbLimite.Items.Add(new KeyValuePair<int, string>(0, "Todos"));
            cmbLimite.Items.Add(new KeyValuePair<int, string>(5, "5 minutos"));
            cmbLimite.Items.Add(new KeyValuePair<int, string>(10, "10 minutos"));
            cmbLimite.Items.Add(new KeyValuePair<int, string>(30, "30 minutos"));
            cmbLimite.Items.Add(new KeyValuePair<int, string>(60, "1 hora"));
            cmbLimite.Items.Add(new KeyValuePair<int, string>(120, "2 horas"));
            cmbLimite.Items.Add(new KeyValuePair<int, string>(480, "8 horas"));
            cmbLimite.Items.Add(new KeyValuePair<int, string>(1440, "1 dia"));

            cmbLimite.DisplayMember = "Value";
            cmbLimite.ValueMember = "Key";
            cmbLimite.SelectedIndex = 0;
        }

        #endregion

        #region Métodos de Banco de Dados

        private List<KeyValuePair<int, string>> ObterUsuarios()
        {
            var lista = new List<KeyValuePair<int, string>>();

            try
            {
                using (var ctx = new ReverseContext())
                {
                    lista = ctx.Usuarios
                        .OrderBy(u => u.UsuarioNome)
                        .Select(u => new { u.Id, u.UsuarioNome })
                        .AsEnumerable()
                        .Select(u => new KeyValuePair<int, string>(u.Id, u.UsuarioNome))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuários: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }

        private List<MonitoramentoUsuario> ObterMonitoramento(int? idUsuarioFiltro = null, int? minutosLimite = null)
        {
            var lista = new List<MonitoramentoUsuario>();

            try
            {
                using (var ctx = new ReverseContext())
                {
                    var usuariosQuery = ctx.Usuarios.AsQueryable();

                    if (idUsuarioFiltro.HasValue && idUsuarioFiltro.Value > 0)
                    {
                        usuariosQuery = usuariosQuery.Where(u => u.Id == idUsuarioFiltro.Value);
                    }

                    var usuarios = usuariosQuery.ToList();

                    foreach (var usuario in usuarios)
                    {
                        var ultimaAtividade = ctx.AtividadesUsuarios
                            .Where(a => a.IdUsuario == usuario.Id)
                            .OrderByDescending(a => a.DataHoraAtividade)
                            .FirstOrDefault();

                        var nomeFormulario = ultimaAtividade?.NomeFormulario ?? "Nenhum";
                        var setorAmigavel = ConverterNomeFormulario(nomeFormulario);

                        var estaOnline = ctx.SessoesUsuarios
                            .Any(s => s.IdUsuario == usuario.Id && s.StatusSessao == "Ativo");

                        int minutosInativos = 999999;
                        DateTime? dataUltimaAtividade = ultimaAtividade?.DataHoraAtividade;

                        if (dataUltimaAtividade.HasValue)
                        {
                            minutosInativos = (int)(DateTime.Now - dataUltimaAtividade.Value).TotalMinutes;
                        }

                        if (minutosLimite.HasValue && minutosInativos > minutosLimite.Value)
                            continue;

                        var monit = new MonitoramentoUsuario
                        {
                            IdUsuario = usuario.Id,
                            Usuario = usuario.UsuarioNome,
                            SetorAmigavel = setorAmigavel,
                            UltimaAtividade = dataUltimaAtividade,
                            MinutosInativos = minutosInativos,
                            EstaOnline = estaOnline,
                            TempoAtivo = CalcularTempoAtivo(dataUltimaAtividade),
                            Status = DeterminarStatus(minutosInativos, estaOnline)
                        };

                        lista.Add(monit);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao obter dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista.OrderBy(m => m.Usuario).ToList();
        }

        private Dictionary<string, int> ObterEstatisticas()
        {
            var stats = new Dictionary<string, int>
            {
                { "Total", 0 },
                { "Ativos", 0 },
                { "EmAlerta", 0 },
                { "Inativos", 0 }
            };

            try
            {
                using (var ctx = new ReverseContext())
                {
                    var usuarios = ctx.Usuarios.ToList();
                    stats["Total"] = usuarios.Count;

                    foreach (var usuario in usuarios)
                    {
                        var estaOnline = ctx.SessoesUsuarios
                            .Any(s => s.IdUsuario == usuario.Id && s.StatusSessao == "Ativo");

                        if (estaOnline)
                        {
                            stats["Ativos"]++;
                            continue;
                        }

                        var ultimaAtividade = ctx.AtividadesUsuarios
                            .Where(a => a.IdUsuario == usuario.Id)
                            .OrderByDescending(a => a.DataHoraAtividade)
                            .Select(a => a.DataHoraAtividade)
                            .FirstOrDefault();

                        if (ultimaAtividade != DateTime.MinValue)
                        {
                            var diasInativos = (DateTime.Now - ultimaAtividade).TotalDays;

                            if (diasInativos >= 5)
                                stats["Inativos"]++;
                            else if (diasInativos >= 3)
                                stats["EmAlerta"]++;
                        }
                        else
                        {
                            stats["Inativos"]++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter estatísticas: {ex.Message}");
            }

            return stats;
        }

        #endregion

        #region Métodos Auxiliares

        private string ConverterNomeFormulario(string nomeFormulario)
        {
            // Dicionário de conversão
            var conversoes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "MainForm", "Menu Principal" },
                { "TriagemFormHub", "Triagem" },
                { "ExpedicaoFormExpHub", "Expedição" },
                { "RHFormRHHub", "Recursos Humanos" },
                { "FinanceiroFormFinanceiroHub", "Financeiro" },
                { "ComercialFormComercialHub", "Comercial" },
                { "FiscalFormHub", "Fiscal" },
                { "ProducaoFormHub", "Produção" },
                { "AtendimentoFormHub", "Atendimento" },

                { "RHFormInatividade", "RH - Monitoramento" },
                { "RHFormFuncionarios", "RH - Funcionários" },
                { "ExpedicaoFormControle", "Expedição - Controle" },
                { "ExpedicaoFormEstoque", "Expedição - Estoque" },
                { "TriagemFormRegistro", "Triagem - Registro" },
                { "FiscalFormNotas", "Fiscal - Notas" },
                { "FormConfigU", "Configurações" },

                { "Nenhum", "Sistema Inativo" }
            };

            if (conversoes.ContainsKey(nomeFormulario))
            {
                return conversoes[nomeFormulario];
            }

            if (nomeFormulario.StartsWith("RHForm", StringComparison.OrdinalIgnoreCase))
                return "Recursos Humanos";
            if (nomeFormulario.StartsWith("TriagemForm", StringComparison.OrdinalIgnoreCase))
                return "Triagem";
            if (nomeFormulario.StartsWith("ExpedicaoForm", StringComparison.OrdinalIgnoreCase))
                return "Expedição";
            if (nomeFormulario.StartsWith("FinanceiroForm", StringComparison.OrdinalIgnoreCase))
                return "Financeiro";
            if (nomeFormulario.StartsWith("ComercialForm", StringComparison.OrdinalIgnoreCase))
                return "Comercial";
            if (nomeFormulario.StartsWith("FiscalForm", StringComparison.OrdinalIgnoreCase))
                return "Fiscal";
            if (nomeFormulario.StartsWith("ProducaoForm", StringComparison.OrdinalIgnoreCase))
                return "Produção";
            if (nomeFormulario.StartsWith("AtendimentoForm", StringComparison.OrdinalIgnoreCase))
                return "Atendimento";

            return nomeFormulario;
        }

        private string CalcularTempoAtivo(DateTime? ultimaAtividade)
        {
            if (!ultimaAtividade.HasValue)
                return "Nunca conectou";

            var diferenca = DateTime.Now - ultimaAtividade.Value;

            if (diferenca.TotalMinutes < 1)
                return "Agora";
            else if (diferenca.TotalMinutes < 60)
                return $"{(int)diferenca.TotalMinutes} min atrás";
            else if (diferenca.TotalHours < 24)
                return $"{(int)diferenca.TotalHours}h {diferenca.Minutes}min atrás";
            else if (diferenca.TotalDays < 7)
                return $"{(int)diferenca.TotalDays} dia(s) atrás";
            else
                return ultimaAtividade.Value.ToString("dd/MM/yyyy");
        }

        private int DeterminarStatus(int minutosInativos, bool estaOnline)
        {
            if (estaOnline && minutosInativos < 5)
                return 3; // Ativo

            int diasInativos = minutosInativos / (60 * 24);

            if (diasInativos >= 5)
                return 1; // Inativo
            else if (diasInativos >= 3)
                return 2; // Alerta
            else if (estaOnline)
                return 3; // Ativo
            else
                return 0; // Offline
        }

        #endregion

        #region Atualização de Dados

        private void AtualizarDados()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                int? idUsuario = null;
                if (cmbFuncionario.SelectedItem != null)
                {
                    var item = (KeyValuePair<int, string>)cmbFuncionario.SelectedItem;
                    if (item.Key > 0)
                        idUsuario = item.Key;
                }

                int? minutosLimite = null;
                if (cmbLimite.SelectedItem != null)
                {
                    var item = (KeyValuePair<int, string>)cmbLimite.SelectedItem;
                    if (item.Key > 0)
                        minutosLimite = item.Key;
                }

                var dados = ObterMonitoramento(idUsuario, minutosLimite);
                dgvInatividade.DataSource = dados;

                AtualizarEstatisticas();

                lblAtualizado.Text = $"Atualizado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Erro ao atualizar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarEstatisticas()
        {
            try
            {
                var stats = ObterEstatisticas();

                lblTotal.Text = $"Total: {stats["Total"]}";
                lblAtivos.Text = $"Ativos: {stats["Ativos"]}";
                lblEmAlerta.Text = $"Em Alerta: {stats["EmAlerta"]}";
                lblInativos.Text = $"Inativos: {stats["Inativos"]}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar estatísticas: {ex.Message}");
            }
        }

        private void IniciarContagemProximaAtualizacao()
        {
            proximaAtualizacao = DateTime.Now.AddMinutes(INTERVALO_ATUALIZACAO_MINUTOS);
            AtualizarLabelProximaAtualizacao();
        }

        private void AtualizarLabelProximaAtualizacao()
        {
            var tempo = proximaAtualizacao - DateTime.Now;

            if (tempo.TotalSeconds <= 0)
            {
                lblProxAtt.Text = "Atualizando...";
            }
            else
            {
                lblProxAtt.Text = $"{tempo.Minutes:D2}:{tempo.Seconds:D2}";
            }
        }

        #endregion

        #region Eventos

        private void TimerAtualizacao_Tick(object sender, EventArgs e)
        {
            AtualizarLabelProximaAtualizacao();

            if (DateTime.Now >= proximaAtualizacao)
            {
                AtualizarDados();
                IniciarContagemProximaAtualizacao();
            }
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            AtualizarDados();
            IniciarContagemProximaAtualizacao();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExportarPDF();
        }

        private void cmbFuncionario_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarDados();
        }

        private void cmbLimite_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarDados();
        }

        private void DgvInatividade_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvInatividade.Rows[e.RowIndex].DataBoundItem != null)
            {
                var item = (MonitoramentoUsuario)dgvInatividade.Rows[e.RowIndex].DataBoundItem;

                if (dgvInatividade.Columns[e.ColumnIndex].Name == "colStatus")
                {
                    switch (item.Status)
                    {
                        case 3: // Ativo
                            e.CellStyle.ForeColor = Color.FromArgb(34, 139, 34);
                            e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                            break;
                        case 2: // Alerta
                            e.CellStyle.ForeColor = Color.FromArgb(255, 140, 0);
                            e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                            break;
                        case 1: // Inativo
                            e.CellStyle.ForeColor = Color.FromArgb(220, 20, 60);
                            e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                            break;
                        default: // Offline
                            e.CellStyle.ForeColor = Color.Gray;
                            break;
                    }
                }
            }
        }

        #endregion

        #region Exportação PDF

        private void ExportarPDF()
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files|*.pdf",
                    Title = "Salvar Relatório",
                    FileName = $"Relatorio_Inatividade_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                    PdfWriter.GetInstance(doc, new FileStream(saveDialog.FileName, FileMode.Create));

                    doc.Open();

                    var fonteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                    var titulo = new Paragraph("Relatório de Monitoramento de Atividades", fonteTitulo)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 20
                    };
                    doc.Add(titulo);

                    var fonteInfo = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    var info = new Paragraph(
                        $"Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n" +
                        $"Usuário: {((KeyValuePair<int, string>)cmbFuncionario.SelectedItem).Value}\n" +
                        $"Filtro de Tempo: {((KeyValuePair<int, string>)cmbLimite.SelectedItem).Value}\n",
                        fonteInfo
                    )
                    {
                        SpacingAfter = 20
                    };
                    doc.Add(info);

                    var tabelaStats = new PdfPTable(4) { WidthPercentage = 100, SpacingAfter = 20 };
                    var fonteHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);

                    tabelaStats.AddCell(new PdfPCell(new Phrase("Total", fonteHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabelaStats.AddCell(new PdfPCell(new Phrase("Ativos", fonteHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabelaStats.AddCell(new PdfPCell(new Phrase("Em Alerta", fonteHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabelaStats.AddCell(new PdfPCell(new Phrase("Inativos", fonteHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                    var stats = ObterEstatisticas();
                    tabelaStats.AddCell(stats["Total"].ToString());
                    tabelaStats.AddCell(stats["Ativos"].ToString());
                    tabelaStats.AddCell(stats["EmAlerta"].ToString());
                    tabelaStats.AddCell(stats["Inativos"].ToString());

                    doc.Add(tabelaStats);

                    var tabela = new PdfPTable(6) { WidthPercentage = 100 };
                    tabela.SetWidths(new float[] { 3f, 3f, 2f, 1.5f, 2f, 1.5f });

                    tabela.AddCell(new PdfPCell(new Phrase("Usuário", fonteHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabela.AddCell(new PdfPCell(new Phrase("Setor", fonteHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabela.AddCell(new PdfPCell(new Phrase("Data", fonteHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabela.AddCell(new PdfPCell(new Phrase("Hora", fonteHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabela.AddCell(new PdfPCell(new Phrase("Tempo Decorrido", fonteHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tabela.AddCell(new PdfPCell(new Phrase("Status", fonteHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                    var fonteDados = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                    var dados = (List<MonitoramentoUsuario>)dgvInatividade.DataSource;

                    foreach (var item in dados)
                    {
                        tabela.AddCell(new PdfPCell(new Phrase(item.Usuario, fonteDados)));
                        tabela.AddCell(new PdfPCell(new Phrase(item.SetorAmigavel, fonteDados)));
                        tabela.AddCell(new PdfPCell(new Phrase(item.DataFormatada, fonteDados)));
                        tabela.AddCell(new PdfPCell(new Phrase(item.HoraFormatada, fonteDados)));
                        tabela.AddCell(new PdfPCell(new Phrase(item.TempoAtivo, fonteDados)));

                        var cellStatus = new PdfPCell(new Phrase(item.StatusTexto.Replace("●", "").Replace("○", "").Trim(), fonteDados));

                        switch (item.Status)
                        {
                            case 3:
                                cellStatus.BackgroundColor = new BaseColor(144, 238, 144);
                                break;
                            case 2:
                                cellStatus.BackgroundColor = new BaseColor(255, 218, 185);
                                break;
                            case 1:
                                cellStatus.BackgroundColor = new BaseColor(255, 182, 193);
                                break;
                            default:
                                cellStatus.BackgroundColor = BaseColor.LIGHT_GRAY;
                                break;
                        }

                        tabela.AddCell(cellStatus);
                    }

                    doc.Add(tabela);

                    var rodape = new Paragraph(
                        $"\nRelatório gerado pelo sistema Reverse\n© {DateTime.Now.Year} - Todos os direitos reservados",
                        FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY)
                    )
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingBefore = 30
                    };
                    doc.Add(rodape);

                    doc.Close();

                    MessageBox.Show("Relatório exportado com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Cleanup

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (timerAtualizacao != null)
            {
                timerAtualizacao.Stop();
                timerAtualizacao.Dispose();
            }
            base.OnFormClosing(e);
        }

        #endregion

        #region Classes de Modelo (ViewModel)

        public class MonitoramentoUsuario
        {
            private string _usuario;
            public string Usuario
            {
                get => _usuario;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _usuario = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
                    }
                    else
                    {
                        _usuario = value;
                    }
                }
            }
            public int IdUsuario { get; set; }
            public string SetorAmigavel { get; set; }
            public DateTime? UltimaAtividade { get; set; }
            public string DataFormatada
            {
                get
                {
                    if (!UltimaAtividade.HasValue)
                        return "-";
                    return UltimaAtividade.Value.ToString("dd/MM/yyyy");
                }
            }

            public string HoraFormatada
            {
                get
                {
                    if (!UltimaAtividade.HasValue)
                        return "-";
                    return UltimaAtividade.Value.ToString("HH:mm:ss");
                }
            }

            public string TempoAtivo { get; set; }
            public int Status { get; set; }
            public string StatusTexto
            {
                get
                {
                    switch (Status)
                    {
                        case 3: return "● Ativo";
                        case 2: return "● Alerta";
                        case 1: return "● Inativo";
                        default: return "○ Offline";
                    }
                }
            }
            public int MinutosInativos { get; set; }
            public bool EstaOnline { get; set; }
        }

        #endregion
    }
}