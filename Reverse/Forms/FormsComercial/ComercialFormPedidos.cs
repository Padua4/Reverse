using Reverse.Models;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Reverse.Forms.FormsComercial
{
    public partial class ComercialFormPedidos : Form
    {
        private ReverseContext _ctx;
        private BindingList<ItemPedido> _itensPedido;

        public ComercialFormPedidos()
        {
            InitializeComponent();
            _itensPedido = new BindingList<ItemPedido>();
        }

        private void ComercialFormPedidos_Load(object sender, EventArgs e)
        {
            _ctx = new ReverseContext();

            ConfigurarComboBox();
            ConfigurarDataGridView();
            AplicarEstiloVisual(dgvPedidos);

            CarregarPedidos();
            LimparDetalhes();
        }

        #region Configuração Inicial

        private void ConfigurarComboBox()
        {
            cmbPedidos.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbPedidos.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbPedidos.DropDownStyle = ComboBoxStyle.DropDown;
        }

        private void ConfigurarDataGridView()
        {
            dgvPedidos.AutoGenerateColumns = false;
            dgvPedidos.Columns.Clear();

            dgvPedidos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Tipo",
                HeaderText = "Material/Palete",
                DataPropertyName = "Tipo",
                Width = 150,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                }
            });

            dgvPedidos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Descricao",
                HeaderText = "Descrição",
                DataPropertyName = "Descricao",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            });

            dgvPedidos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Quantidade",
                HeaderText = "Quantidade",
                DataPropertyName = "Quantidade",
                Width = 120,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvPedidos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ValorTotal",
                HeaderText = "Valor Total",
                DataPropertyName = "ValorTotal",
                Width = 120,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvPedidos.AllowUserToAddRows = false;
            dgvPedidos.AllowUserToDeleteRows = false;
            dgvPedidos.ReadOnly = true;
            dgvPedidos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPedidos.MultiSelect = false;

            dgvPedidos.CellFormatting += DgvPedidos_CellFormatting;
            dgvPedidos.DataSource = _itensPedido;
        }

        private void DgvPedidos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPedidos.Columns[e.ColumnIndex].Name == "Tipo" && e.Value != null)
            {
                string tipo = e.Value.ToString();

                if (tipo == "PALETE")
                {
                    e.CellStyle.BackColor = Color.FromArgb(52, 152, 219);
                    e.CellStyle.ForeColor = Color.White;
                }
                else if (tipo == "MATERIAL")
                {
                    e.CellStyle.BackColor = Color.FromArgb(46, 204, 113);
                    e.CellStyle.ForeColor = Color.White;
                }

                else if (tipo == "PRODUTO")
                {
                    e.CellStyle.BackColor = Color.FromArgb(155, 89, 182);
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }

        private void AplicarEstiloVisual(DataGridView grid)
        {
            grid.BackgroundColor = Color.FromArgb(250, 250, 252);
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(230, 230, 235);

            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            grid.RowsDefaultCellStyle.BackColor = Color.White;

            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = grid.ColumnHeadersDefaultCellStyle.BackColor;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            grid.DefaultCellStyle.ForeColor = Color.Black;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.RowTemplate.Height = 36;
        }

        #endregion

        #region Carregar Dados

        private void CarregarPedidos()
        {
            try
            {
                var pedidos = _ctx.Database.SqlQuery<PedidoLista>(@"
                    SELECT 
                        VendaId,
                        NumeroPedido,
                        ClienteId,
                        FormaPagamento,
                        ModalidadeFrete,
                        ValorTotal,
                        PercentualDesconto,
                        UsuarioId,
                        StatusVenda,
                        DataVenda,
                        DataCriacao
                    FROM Vendas
                    ORDER BY NumeroPedido DESC
                ").ToList();

                cmbPedidos.DataSource = pedidos;
                cmbPedidos.DisplayMember = "DisplayText";
                cmbPedidos.ValueMember = "VendaId";
                cmbPedidos.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar pedidos: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarDetalhesPedido(int vendaId)
        {
            try
            {
                var venda = _ctx.Database.SqlQuery<VendaDetalhes>(@"
                    SELECT 
                        v.VendaId,
                        v.NumeroPedido,
                        v.ClienteId,
                        c.Nome AS NomeCliente,
                        v.FormaPagamento,
                        v.ModalidadeFrete,
                        v.ValorTotal,
                        v.PercentualDesconto,
                        v.UsuarioId,
                        u.UsuarioNome AS NomeUsuario,
                        v.StatusVenda,
                        v.DataVenda,
                        v.Observacoes
                    FROM Vendas v
                    INNER JOIN Clientes c ON v.ClienteId = c.ClienteId
                    LEFT JOIN Usuarios u ON v.UsuarioId = u.Id
                    WHERE v.VendaId = @p0
                ", vendaId).FirstOrDefault();

                if (venda == null)
                {
                    LimparDetalhes();
                    return;
                }

                lblCliente.Text = $"Cliente: {venda.NomeCliente}";
                lblDataVenda.Text = $"Data da Venda: {venda.DataVenda:dd/MM/yyyy HH:mm}";
                lblResponsavel.Text = $"Responsável: {venda.NomeUsuario ?? "Não informado"}";

                CarregarItensPedido(vendaId);

                AtualizarValorTotal();

                btnCancelar.Enabled = venda.StatusVenda != "Cancelado";

                if (venda.StatusVenda == "Cancelado")
                {
                    lblCliente.ForeColor = Color.Red;
                    lblDataVenda.ForeColor = Color.Red;
                    lblResponsavel.ForeColor = Color.Red;
                    lblValorTotal.ForeColor = Color.Red;
                }
                else
                {
                    lblCliente.ForeColor = Color.Black;
                    lblDataVenda.ForeColor = Color.Black;
                    lblResponsavel.ForeColor = Color.Black;
                    lblValorTotal.ForeColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar detalhes do pedido: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarItensPedido(int vendaId)
        {
            _itensPedido.Clear();

            try
            {
                var paletes = _ctx.Database.SqlQuery<ItemPedidoQuery>(@"
                    SELECT 
                        'PALETE' AS Tipo,
                        p.Numero AS Numero,
                        cp.Nome AS Categoria,
                        COUNT(ip.Id) AS QtdItens,
                        0.0 AS QuantidadeKg,
                        CAST(CAST(vp.ValorTotal AS FLOAT) AS DECIMAL(18,2)) AS ValorTotal
                    FROM VendaPalete vp
                    INNER JOIN Palete p ON vp.PaleteId = p.Id
                    INNER JOIN CategoriaPalete cp ON p.CategoriaId = cp.Id
                    LEFT JOIN ItemPalete ip ON p.Id = ip.PaleteId
                    WHERE vp.VendaId = @p0
                    GROUP BY p.Numero, cp.Nome, vp.ValorTotal
                ", vendaId).ToList();

                foreach (var palete in paletes)
                {
                    _itensPedido.Add(new ItemPedido
                    {
                        Tipo = "PALETE",
                        Descricao = $"Palete #{palete.Numero} - {palete.Categoria}",
                        Quantidade = $"{palete.QtdItens} itens",
                        ValorTotal = Convert.ToDecimal(palete.ValorTotal)
                    });
                }

                var materiais = _ctx.Database.SqlQuery<ItemPedidoQuery>(@"
                    SELECT 
                        'MATERIAL' AS Tipo,
                        0 AS Numero,
                        e.Material AS Categoria,
                        0 AS QtdItens,
                        CAST(CAST(vm.Quantidade AS FLOAT) AS DECIMAL(18,3)) AS QuantidadeKg,
                        CAST(CAST(vm.ValorTotal AS FLOAT) AS DECIMAL(18,2)) AS ValorTotal
                    FROM VendaMaterial vm
                    INNER JOIN Estoques e ON vm.EstoqueId = e.Id
                    WHERE vm.VendaId = @p0
                ", vendaId).ToList();

                foreach (var material in materiais)
                {
                    _itensPedido.Add(new ItemPedido
                    {
                        Tipo = "MATERIAL",
                        Descricao = material.Categoria,
                        Quantidade = $"{Convert.ToDecimal(material.QuantidadeKg):F3} KG",
                        ValorTotal = Convert.ToDecimal(material.ValorTotal)
                    });
                }

                var produtos = _ctx.Database.SqlQuery<ItemPedidoQuery>(@"
                    SELECT 
                        'PRODUTO' AS Tipo,
                        0 AS Numero,
                        vpm.Descricao AS Categoria,
                        0 AS QtdItens,
                        CAST(CAST(vpm.QuantidadeDecimal AS FLOAT) AS DECIMAL(18,3)) AS QuantidadeKg,
                        CAST(CAST(vpm.ValorVenda AS FLOAT) AS DECIMAL(18,2)) AS ValorTotal
                    FROM VendaProdutoManual vpm
                    WHERE vpm.VendaId = @p0
                ", vendaId).ToList();

                foreach (var produto in produtos)
                {
                    _itensPedido.Add(new ItemPedido
                    {
                        Tipo = "PRODUTO",
                        Descricao = produto.Categoria,
                        Quantidade = produto.QuantidadeKg > 0 ? $"{produto.QuantidadeKg:F0} un" : "1 un",
                        ValorTotal = Convert.ToDecimal(produto.ValorTotal)
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar itens do pedido: {ex.Message}\n\nDetalhes: {ex.InnerException?.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Eventos

        private void cmbPedidos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPedidos.SelectedIndex < 0 || cmbPedidos.SelectedValue == null)
            {
                LimparDetalhes();
                return;
            }

            if (int.TryParse(cmbPedidos.SelectedValue.ToString(), out int vendaId))
            {
                CarregarDetalhesPedido(vendaId);
            }
            else
            {
                LimparDetalhes();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (cmbPedidos.SelectedIndex < 0 || cmbPedidos.SelectedValue == null)
            {
                MessageBox.Show("Selecione um pedido para cancelar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(cmbPedidos.SelectedValue.ToString(), out int vendaId))
            {
                MessageBox.Show("Erro ao identificar o pedido selecionado.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var pedido = cmbPedidos.SelectedItem as PedidoLista;

            if (pedido.StatusVenda == "Cancelado")
            {
                MessageBox.Show("Este pedido já foi cancelado.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var formObservacao = new FormObservacaoCancelamento();
            if (formObservacao.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string observacao = formObservacao.Observacao;

            var result = MessageBox.Show(
                $"Deseja realmente cancelar o pedido #{pedido.NumeroPedido}?\n\n" +
                $"Esta ação irá:\n" +
                $"- Retornar os paletes para o status 'Finalizado'\n" +
                $"- Retornar os materiais para o estoque como 'Segregado'\n" +
                $"- Marcar o pedido como 'Cancelado'",
                "Confirmar Cancelamento",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                using (var transaction = _ctx.Database.BeginTransaction())
                {
                    try
                    {
                        _ctx.Database.ExecuteSqlCommand(@"
                            UPDATE Palete
                            SET Status = 2
                            WHERE Id IN (
                                SELECT PaleteId 
                                FROM VendaPalete 
                                WHERE VendaId = @p0
                            )
                        ", vendaId);

                        _ctx.Database.ExecuteSqlCommand(@"
                            UPDATE e
                            SET e.Quantidade = v.TotalQtd,
                                e.Status = 'Segregado',
                                e.DataSaida = NULL
                            FROM Estoques e
                            INNER JOIN (
                                SELECT EstoqueId, SUM(Quantidade) AS TotalQtd
                                FROM VendaMaterial
                                WHERE VendaId = @p0
                                GROUP BY EstoqueId
                            ) v ON e.Id = v.EstoqueId;
                        ", vendaId);

                        _ctx.Database.ExecuteSqlCommand(@"
                            UPDATE Vendas
                            SET StatusVenda = 'Cancelado',
                                Observacoes = @p1
                            WHERE VendaId = @p0
                        ", vendaId, observacao);

                        transaction.Commit();

                        MessageBox.Show(
                            $"Pedido #{pedido.NumeroPedido} cancelado com sucesso!",
                            "Sucesso",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        CarregarPedidos();
                        CarregarDetalhesPedido(vendaId);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Erro na transação: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao cancelar pedido:\n{ex.Message}\n\n{ex.InnerException?.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnEnviarPedido_Click(object sender, EventArgs e)
        {
            if (cmbPedidos.SelectedIndex < 0 || cmbPedidos.SelectedValue == null)
            {
                MessageBox.Show("Selecione um pedido para exportar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(cmbPedidos.SelectedValue.ToString(), out int vendaId))
            {
                MessageBox.Show("Erro ao identificar o pedido selecionado.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var pedido = cmbPedidos.SelectedItem as PedidoLista;

            try
            {
                var venda = _ctx.Database.SqlQuery<VendaDetalhes>(@"
                    SELECT 
                        v.VendaId, v.NumeroPedido, v.ClienteId,
                        c.Nome AS NomeCliente, c.Endereco, c.Telefone, c.Email, c.CPF_CNPJ,
                        v.FormaPagamento, v.ModalidadeFrete, v.ValorTotal,
                        v.PercentualDesconto, v.UsuarioId,
                        u.UsuarioNome AS NomeUsuario, v.StatusVenda, v.DataVenda
                    FROM Vendas v
                    INNER JOIN Clientes c ON v.ClienteId = c.ClienteId
                    LEFT JOIN Usuarios u ON v.UsuarioId = u.Id
                    WHERE v.VendaId = @p0
                ", vendaId).FirstOrDefault();

                if (venda == null)
                {
                    MessageBox.Show("Erro ao recuperar dados da venda.", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                GerarPDF(venda);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar pedido: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Geração de PDF

        private string FormatarCpfCnpj(string cpfCnpj)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpj))
                return cpfCnpj;

            string numeros = new string(cpfCnpj.Where(char.IsDigit).ToArray());

            if (numeros.Length == 11)
            {
                return Convert.ToUInt64(numeros).ToString(@"000\.000\.000\-00");
            }
            else if (numeros.Length == 14)
            {
                return Convert.ToUInt64(numeros).ToString(@"00\.000\.000\/0000\-00");
            }
            else
            {
                return cpfCnpj;
            }
        }

        private void GerarPDF(VendaDetalhes venda)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PDF Files|*.pdf";
                    saveFileDialog.Title = "Salvar Pedido";
                    saveFileDialog.FileName = $"Pedido_{venda.NumeroPedido}_{DateTime.Now:yyyyMMdd}.pdf";

                    if (saveFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    PdfDocument document = new PdfDocument();
                    document.Info.Title = $"Pedido #{venda.NumeroPedido}";
                    document.Info.Author = "Reverse - Gestão de Pedidos";
                    document.Info.Subject = $"Pedido de Venda #{venda.NumeroPedido}";
                    document.Info.Creator = "Reverse";

                    PdfPage page = document.AddPage();
                    page.Size = PdfSharp.PageSize.A4;
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    XFont fontTitle = new XFont("Times New Roman", 18, XFontStyleEx.Bold);
                    XFont fontSubtitle = new XFont("Times New Roman", 12, XFontStyleEx.Bold);
                    XFont fontNormal = new XFont("Times New Roman", 10, XFontStyleEx.Regular);
                    XFont fontSmall = new XFont("Times New Roman", 9, XFontStyleEx.Regular);
                    XFont fontBold = new XFont("Times New Roman", 10, XFontStyleEx.Bold);

                    double leftMargin = 40;
                    double pageWidth = page.Width.Point;
                    double pageHeight = page.Height.Point;
                    double rightMargin = pageWidth - 40;
                    double contentWidth = rightMargin - leftMargin;

                    double yPosition = 40;

                    // ========== CABEÇALHO ==========
                    gfx.DrawString("REVERSE LOGÍSTICA", fontTitle,
                        XBrushes.DarkBlue, new XRect(0, yPosition, pageWidth, 30),
                        XStringFormats.TopCenter);

                    gfx.DrawString("Sistema de Gestão de Pedidos", new XFont("Times New Roman", 11, XFontStyleEx.Regular),
                        XBrushes.Gray, new XRect(0, yPosition + 22, pageWidth, 20),
                        XStringFormats.TopCenter);

                    yPosition += 50;

                    // Linha divisória
                    gfx.DrawLine(new XPen(XColor.FromArgb(52, 73, 94), 1.5), leftMargin, yPosition, rightMargin, yPosition);
                    yPosition += 20;

                    // ========== INFORMAÇÕES DO PEDIDO ==========
                    double titleHeight = 28;
                    gfx.DrawRectangle(XBrushes.LightGray, leftMargin, yPosition, contentWidth, titleHeight);
                    gfx.DrawString("INFORMAÇÕES DO PEDIDO", fontSubtitle,
                        XBrushes.DarkBlue, new XRect(leftMargin, yPosition + 8, contentWidth, 20),
                        XStringFormats.TopCenter);

                    yPosition += titleHeight + 15;

                    double infoColLabel = leftMargin + 10;
                    double infoColValue = leftMargin + 110;

                    // Primeira linha - Número do Pedido e Status
                    double secondColLabel = leftMargin + contentWidth / 2 + 10;
                    double secondColValue = secondColLabel + 40;

                    gfx.DrawString($"Número do Pedido:", fontBold, XBrushes.Black, infoColLabel, yPosition);
                    gfx.DrawString($"#{venda.NumeroPedido}", fontNormal, XBrushes.Black, infoColValue, yPosition);

                    gfx.DrawString($"Status:", fontBold, XBrushes.Black, secondColLabel, yPosition);
                    gfx.DrawString($"{venda.StatusVenda}", fontNormal, XBrushes.Black, secondColValue, yPosition);
                    yPosition += 22;

                    // Segunda linha - Data e Hora
                    gfx.DrawString($"Data:", fontBold, XBrushes.Black, infoColLabel, yPosition);
                    gfx.DrawString($"{venda.DataVenda:dd/MM/yyyy}", fontNormal, XBrushes.Black, infoColValue, yPosition);

                    gfx.DrawString($"Hora:", fontBold, XBrushes.Black, secondColLabel, yPosition);
                    gfx.DrawString($"{venda.DataVenda:HH:mm}", fontNormal, XBrushes.Black, secondColValue, yPosition);
                    yPosition += 22;

                    // ESPAÇO ENTRE SEÇÕES
                    yPosition += 25;

                    // ========== DADOS DO CLIENTE ==========
                    gfx.DrawRectangle(XBrushes.LightGray, leftMargin, yPosition, contentWidth, titleHeight);
                    gfx.DrawString("DADOS DO CLIENTE", fontSubtitle,
                        XBrushes.DarkBlue, new XRect(leftMargin, yPosition + 8, contentWidth, 20),
                        XStringFormats.TopCenter);

                    yPosition += titleHeight + 15;

                    // MESMA ESTRUTURA DE ALINHAMENTO
                    gfx.DrawString($"Nome/Razão Social:", fontBold, XBrushes.Black, infoColLabel, yPosition);
                    gfx.DrawString($"{venda.NomeCliente}", fontNormal, XBrushes.Black, infoColValue, yPosition);
                    yPosition += 22;

                    if (!string.IsNullOrEmpty(venda.CPF_CNPJ))
                    {
                        string cpfCnpjFormatado = FormatarCpfCnpj(venda.CPF_CNPJ);
                        gfx.DrawString($"CPF/CNPJ:", fontBold, XBrushes.Black, infoColLabel, yPosition);
                        gfx.DrawString($"{cpfCnpjFormatado}", fontNormal, XBrushes.Black, infoColValue, yPosition);
                        yPosition += 22;
                    }

                    if (!string.IsNullOrEmpty(venda.Endereco))
                    {
                        gfx.DrawString($"Endereço:", fontBold, XBrushes.Black, infoColLabel, yPosition);
                        gfx.DrawString($"{venda.Endereco}", fontNormal, XBrushes.Black, infoColValue, yPosition, XStringFormats.Default);
                        yPosition += 22;
                    }

                    if (!string.IsNullOrEmpty(venda.Telefone))
                    {
                        gfx.DrawString($"Telefone:", fontBold, XBrushes.Black, infoColLabel, yPosition);
                        gfx.DrawString($"{venda.Telefone}", fontNormal, XBrushes.Black, infoColValue, yPosition);
                        yPosition += 22;
                    }

                    if (!string.IsNullOrEmpty(venda.Email))
                    {
                        gfx.DrawString($"E-mail:", fontBold, XBrushes.Black, infoColLabel, yPosition);
                        gfx.DrawString($"{venda.Email}", fontNormal, XBrushes.Black, infoColValue, yPosition);
                        yPosition += 22;
                    }

                    // ESPAÇO ENTRE SEÇÕES
                    yPosition += 25;

                    // ========== INFORMAÇÕES DA VENDA ==========
                    gfx.DrawRectangle(XBrushes.LightGray, leftMargin, yPosition, contentWidth, titleHeight);
                    gfx.DrawString("CONDIÇÕES DA VENDA", fontSubtitle,
                        XBrushes.DarkBlue, new XRect(leftMargin, yPosition + 8, contentWidth, 20),
                        XStringFormats.TopCenter);

                    yPosition += titleHeight + 15;

                    gfx.DrawString($"Forma de Pagamento:", fontBold, XBrushes.Black, infoColLabel, yPosition);
                    gfx.DrawString($"{venda.FormaPagamento}", fontNormal, XBrushes.Black, infoColValue, yPosition);
                    yPosition += 22;

                    gfx.DrawString($"Modalidade de Frete:", fontBold, XBrushes.Black, infoColLabel, yPosition);
                    gfx.DrawString($"{venda.ModalidadeFrete}", fontNormal, XBrushes.Black, infoColValue, yPosition);
                    yPosition += 22;

                    gfx.DrawString($"Responsável:", fontBold, XBrushes.Black, infoColLabel, yPosition);
                    gfx.DrawString($"{venda.NomeUsuario ?? "Não informado"}", fontNormal, XBrushes.Black, infoColValue, yPosition);
                    yPosition += 22;

                    // ESPAÇO ANTES DA TABELA
                    yPosition += 30;

                    // ========== ITENS DO PEDIDO ==========
                    gfx.DrawRectangle(XBrushes.LightGray, leftMargin, yPosition, contentWidth, titleHeight);
                    gfx.DrawString("ITENS DO PEDIDO", fontSubtitle,
                        XBrushes.DarkBlue, new XRect(leftMargin, yPosition + 8, contentWidth, 20),
                        XStringFormats.TopCenter);

                    yPosition += titleHeight + 15;

                    // Cabeçalho da tabela
                    XBrush[] headerBrushes = { XBrushes.DarkBlue, XBrushes.DarkBlue, XBrushes.DarkBlue, XBrushes.DarkBlue };

                    double[] columnWidths = { 60, 295, 80, 80 };

                    double[] columnPositions = {
                        leftMargin,                     // Tipo
                        leftMargin + columnWidths[0],   // Descrição  
                        leftMargin + columnWidths[0] + columnWidths[1], // Quantidade
                        leftMargin + columnWidths[0] + columnWidths[1] + columnWidths[2] // Valor
                    };

                    // Verificar se as colunas cabem
                    double tabelaWidth = columnWidths.Sum();

                    yPosition += 5;

                    double headerHeight = 25;
                    // Desenhar cabeçalho da tabela ALINHADO
                    for (int i = 0; i < 4; i++)
                    {
                        gfx.DrawRectangle(headerBrushes[i], columnPositions[i], yPosition, columnWidths[i], headerHeight);
                    }

                    // Texto do cabeçalho CENTRALIZADO
                    gfx.DrawString("Tipo", fontSmall, XBrushes.White,
                        new XRect(columnPositions[0], yPosition, columnWidths[0], headerHeight),
                        XStringFormats.Center);

                    gfx.DrawString("Descrição", fontSmall, XBrushes.White,
                        new XRect(columnPositions[1] + 8, yPosition, columnWidths[1] - 8, headerHeight),
                        XStringFormats.CenterLeft);

                    gfx.DrawString("Quantidade", fontSmall, XBrushes.White,
                        new XRect(columnPositions[2], yPosition, columnWidths[2], headerHeight),
                        XStringFormats.Center);

                    gfx.DrawString("Valor", fontSmall, XBrushes.White,
                        new XRect(columnPositions[3], yPosition, columnWidths[3], headerHeight),
                        XStringFormats.Center);

                    yPosition += headerHeight;

                    // Itens da tabela
                    bool alternate = false;
                    int itemCount = 0;

                    foreach (var item in _itensPedido)
                    {
                        if (yPosition > pageHeight - 150)
                        {
                            page = document.AddPage();
                            page.Size = PdfSharp.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            yPosition = 40;

                            // Redesenhar cabeçalho da tabela na nova página
                            for (int i = 0; i < 4; i++)
                            {
                                gfx.DrawRectangle(headerBrushes[i], columnPositions[i], yPosition, columnWidths[i], headerHeight);
                            }

                            // Redesenhar texto do cabeçalho
                            gfx.DrawString("Tipo", fontSmall, XBrushes.White,
                                new XRect(columnPositions[0], yPosition, columnWidths[0], headerHeight),
                                XStringFormats.Center);

                            gfx.DrawString("Descrição", fontSmall, XBrushes.White,
                                new XRect(columnPositions[1] + 8, yPosition, columnWidths[1] - 8, headerHeight),
                                XStringFormats.CenterLeft);

                            gfx.DrawString("Quantidade", fontSmall, XBrushes.White,
                                new XRect(columnPositions[2], yPosition, columnWidths[2], headerHeight),
                                XStringFormats.Center);

                            gfx.DrawString("Valor", fontSmall, XBrushes.White,
                                new XRect(columnPositions[3], yPosition, columnWidths[3], headerHeight),
                                XStringFormats.Center);

                            yPosition += headerHeight;
                            alternate = false;
                        }

                        XBrush rowBrush = alternate ? XBrushes.AliceBlue : XBrushes.White;
                        alternate = !alternate;

                        double rowHeight = 22;

                        // Desenhar fundo da linha
                        gfx.DrawRectangle(rowBrush, columnPositions[0], yPosition, tabelaWidth, rowHeight);

                        // Desenhar bordas das células
                        for (int i = 0; i < 4; i++)
                        {
                            gfx.DrawRectangle(XPens.LightGray, columnPositions[i], yPosition, columnWidths[i], rowHeight);
                        }

                        // Desenhar conteúdo
                        gfx.DrawString(item.Tipo, fontSmall, XBrushes.Black,
                            new XRect(columnPositions[0], yPosition, columnWidths[0], rowHeight),
                            XStringFormats.Center);

                        gfx.DrawString(item.Descricao, fontSmall, XBrushes.Black,
                            new XRect(columnPositions[1] + 8, yPosition, columnWidths[1] - 8, rowHeight),
                            XStringFormats.CenterLeft);

                        gfx.DrawString(item.Quantidade, fontSmall, XBrushes.Black,
                            new XRect(columnPositions[2], yPosition, columnWidths[2], rowHeight),
                            XStringFormats.Center);

                        gfx.DrawString(item.ValorTotal.ToString("C2"), fontSmall, XBrushes.Black,
                            new XRect(columnPositions[3], yPosition, columnWidths[3], rowHeight),
                            XStringFormats.Center);

                        yPosition += rowHeight;
                        itemCount++;
                    }

                    // Linha divisória após a tabela
                    yPosition += 15;
                    gfx.DrawLine(XPens.Black, leftMargin, yPosition, rightMargin, yPosition);
                    yPosition += 20;

                    // ========== RESUMO FINANCEIRO ==========
                    double financeHeaderHeight = 30;

                    gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(52, 152, 219)), leftMargin, yPosition, contentWidth, financeHeaderHeight);
                    gfx.DrawString("RESUMO FINANCEIRO", fontSubtitle,
                        XBrushes.White, new XRect(leftMargin, yPosition + 10, contentWidth, 20),
                        XStringFormats.TopCenter);

                    yPosition += financeHeaderHeight + 20;

                    decimal totalItens = _itensPedido.Sum(i => i.ValorTotal);

                    double resumoLabelX = columnPositions[2] - 10; // Um pouco antes da coluna "Quantidade"
                    double resumoValueX = columnPositions[3]; // Mesma posição da coluna "Valor"

                    gfx.DrawString($"Subtotal ({itemCount} itens):", fontBold, XBrushes.Black, resumoLabelX, yPosition);
                    gfx.DrawString(totalItens.ToString("C2"), fontNormal, XBrushes.Black, resumoValueX, yPosition);
                    yPosition += 25;

                    if (venda.PercentualDesconto > 0)
                    {
                        decimal valorDesconto = totalItens * (venda.PercentualDesconto / 100);
                        gfx.DrawString($"Desconto ({venda.PercentualDesconto:N2}%):", fontBold, XBrushes.Black, resumoLabelX, yPosition);
                        gfx.DrawString($"-{valorDesconto:C2}", fontNormal, XBrushes.Red, resumoValueX, yPosition);
                        yPosition += 25;
                    }

                    yPosition += 10;
                    gfx.DrawString($"VALOR TOTAL:", new XFont("Times New Roman", 11, XFontStyleEx.Bold),
                        XBrushes.Black, resumoLabelX, yPosition);
                    gfx.DrawString($"{venda.ValorTotal:C2}", new XFont("Times New Roman", 11, XFontStyleEx.Bold),
                        XBrushes.Green, resumoValueX, yPosition);
                    yPosition += 30;

                    // Linha divisória final
                    gfx.DrawLine(new XPen(XColor.FromArgb(52, 73, 94), 1), leftMargin, yPosition, rightMargin, yPosition);
                    yPosition += 20;

                    // ========== RODAPÉ ==========
                    gfx.DrawString($"Documento gerado automaticamente pelo Sistema Reverse", fontSmall,
                        XBrushes.Gray, new XRect(leftMargin, pageHeight - 40, contentWidth, 20),
                        XStringFormats.TopLeft);

                    gfx.DrawString($"Data de emissão: {DateTime.Now:dd/MM/yyyy HH:mm}", fontSmall,
                        XBrushes.Gray, new XRect(leftMargin, pageHeight - 25, contentWidth, 20),
                        XStringFormats.TopLeft);

                    // Salvar PDF
                    document.Save(saveFileDialog.FileName);

                    MessageBox.Show(
                        $"PDF gerado com sucesso!\n\nArquivo salvo em:\n{saveFileDialog.FileName}",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    var resultAbrir = MessageBox.Show(
                        "Deseja abrir o arquivo agora?",
                        "Abrir PDF",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (resultAbrir == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Utilitários

        private void LimparDetalhes()
        {
            lblCliente.Text = "Cliente: -";
            lblDataVenda.Text = "Data da Venda: -";
            lblResponsavel.Text = "Responsável: -";
            lblValorTotal.Text = "Total: R$ 0,00";
            _itensPedido.Clear();
            btnCancelar.Enabled = false;
        }
        private void AtualizarValorTotal()
        {
            if (_itensPedido == null || _itensPedido.Count == 0)
            {
                lblValorTotal.Text = "Total: R$ 0,00";
                return;
            }

            decimal total = _itensPedido.Sum(i => i.ValorTotal);
            lblValorTotal.Text = $"Total: {total:C2}";
        }

        #endregion

        #region Classes Auxiliares

        public class PedidoLista
        {
            public int VendaId { get; set; }
            public int NumeroPedido { get; set; }
            public int ClienteId { get; set; }
            public string FormaPagamento { get; set; }
            public string ModalidadeFrete { get; set; }
            public decimal ValorTotal { get; set; }
            public decimal PercentualDesconto { get; set; }
            public int? UsuarioId { get; set; }
            public string StatusVenda { get; set; }
            public DateTime DataVenda { get; set; }
            public DateTime DataCriacao { get; set; }

            public string DisplayText => $"Pedido #{NumeroPedido} - {StatusVenda}";
        }

        public class VendaDetalhes
        {
            public int VendaId { get; set; }
            public int NumeroPedido { get; set; }
            public int ClienteId { get; set; }
            public string NomeCliente { get; set; }
            public string Endereco { get; set; }
            public string Telefone { get; set; }
            public string Email { get; set; }
            public string CPF_CNPJ { get; set; }
            public string FormaPagamento { get; set; }
            public string ModalidadeFrete { get; set; }
            public decimal ValorTotal { get; set; }
            public decimal PercentualDesconto { get; set; }
            public int? UsuarioId { get; set; }
            public string NomeUsuario { get; set; }
            public string StatusVenda { get; set; }
            public DateTime DataVenda { get; set; }
            public string Observacoes { get; set; }
        }

        public class ItemPedidoQuery
        {
            public string Tipo { get; set; }
            public int Numero { get; set; }
            public string Categoria { get; set; }
            public int QtdItens { get; set; }
            public decimal QuantidadeKg { get; set; }
            public decimal ValorTotal { get; set; }

        }

        public class ItemPedido
        {
            public string Tipo { get; set; }
            public string Descricao { get; set; }
            public string Quantidade { get; set; }
            public decimal ValorTotal { get; set; }
        }

        #endregion
    }

    #region Form de Observação de Cancelamento

    public partial class FormObservacaoCancelamento : Form
    {
        public string Observacao { get; private set; }

        public FormObservacaoCancelamento()
        {
            this.Text = "Observação de Cancelamento";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(500, 300);

            InitializeControls();
        }

        private void InitializeControls()
        {
            Label lblInstrucao = new Label
            {
                Text = "Informe o motivo do cancelamento:",
                Location = new Point(20, 20),
                Size = new Size(440, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            TextBox txtObservacao = new TextBox
            {
                Name = "txtObservacao",
                Location = new Point(20, 50),
                Size = new Size(440, 150),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                MaxLength = 500,
                Font = new Font("Segoe UI", 9F)
            };

            Button btnConfirmar = new Button
            {
                Text = "Confirmar",
                Location = new Point(270, 220),
                Size = new Size(90, 30),
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 9F)
            };

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(370, 220),
                Size = new Size(90, 30),
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 9F)
            };

            btnConfirmar.Click += (s, e) =>
            {
                string obs = this.Controls.Find("txtObservacao", false).FirstOrDefault()?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(obs))
                {
                    MessageBox.Show("Por favor, informe o motivo do cancelamento.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                    return;
                }

                Observacao = obs;
            };

            this.Controls.Add(lblInstrucao);
            this.Controls.Add(txtObservacao);
            this.Controls.Add(btnConfirmar);
            this.Controls.Add(btnCancelar);

            this.AcceptButton = btnConfirmar;
            this.CancelButton = btnCancelar;
        }
    }

    #endregion
}