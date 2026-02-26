using Reverse.Forms.FormsExpedicao;
using Reverse.Models;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reverse.Forms.FormsComercial
{
    public partial class ComercialFormVenda : Form
    {
        private readonly int _usuarioId;
        private ReverseContext _ctx;
        private BindingList<ItemLista> _listaItens;

        public ComercialFormVenda(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            _listaItens = new BindingList<ItemLista>();
        }

        private void ComercialFormVenda_Load(object sender, EventArgs e)
        {
            _ctx?.Dispose();
            _ctx = new ReverseContext();

            CarregarClientes();
            ConfigurarAutoComplete();
            ConfigurarFormaPagamento();

            rbCIF.Checked = true;

            ConfigurarGridPaletes();
            ConfigurarGridMateriais();
            ConfigurarGridLista();

            CarregarPaletesDisponiveis();
            CarregarMateriaisDisponiveis();

            AplicarEstiloVisual(dgvPalete);
            AplicarEstiloVisual(dgvMaterial);
            AplicarEstiloVisual(dgvLista);

            dgvLista.CellBeginEdit += dgvLista_CellBeginEdit;
            dgvLista.CellEndEdit += dgvLista_CellEndEdit;
            dgvLista.CellValidating += dgvLista_CellValidating;
            dgvLista.CellFormatting += DgvLista_CellFormatting;

            dgvLista.DataSource = _listaItens;

            LimparCampos();
            AtualizarValorTotal();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _ctx?.Dispose();
        }

        #region Configuração Inicial

        private void ConfigurarAutoComplete()
        {
            cmbCliente.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbCliente.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void ConfigurarFormaPagamento()
        {
            cmbFormaPagamento.Items.AddRange(new string[]
            {
                "Pix",
                "Cartão de Crédito",
                "Cartão de Débito",
                "Cheque",
                "Dinheiro",
                "Outros"
            });
        }

        private void CarregarClientes()
        {
            var clientes = _ctx.Clientes
                .OrderBy(c => c.Nome)
                .ToList();

            cmbCliente.DataSource = clientes;
            cmbCliente.DisplayMember = "Nome";
            cmbCliente.ValueMember = "ClienteId";
            cmbCliente.SelectedIndex = -1;
        }

        #endregion

        #region Configuração dos DataGridViews

        private void ConfigurarGridPaletes()
        {
            dgvPalete.AutoGenerateColumns = false;
            dgvPalete.Columns.Clear();

            dgvPalete.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaleteId",
                DataPropertyName = "PaleteId",
                Visible = false
            });

            dgvPalete.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NumeroPalete",
                HeaderText = "Palete",
                DataPropertyName = "NumeroPalete",
                Width = 70,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                }
            });

            dgvPalete.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                HeaderText = "Categoria",
                DataPropertyName = "Categoria",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvPalete.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QtdItens",
                HeaderText = "Qtd Itens",
                DataPropertyName = "QtdItens",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvPalete.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ValorTotal",
                HeaderText = "Valor Total",
                DataPropertyName = "ValorTotal",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvPalete.AllowUserToAddRows = false;
            dgvPalete.AllowUserToDeleteRows = false;
            dgvPalete.ReadOnly = true;
            dgvPalete.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPalete.MultiSelect = false;
        }

        private void ConfigurarGridMateriais()
        {
            dgvMaterial.AutoGenerateColumns = false;
            dgvMaterial.Columns.Clear();

            dgvMaterial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EstoqueId",
                DataPropertyName = "EstoqueId",
                Visible = false
            });

            dgvMaterial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Material",
                HeaderText = "Material",
                DataPropertyName = "Material",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvMaterial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Quantidade",
                HeaderText = "Quantidade",
                DataPropertyName = "Quantidade",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N3",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvMaterial.AllowUserToAddRows = false;
            dgvMaterial.AllowUserToDeleteRows = false;
            dgvMaterial.ReadOnly = true;
            dgvMaterial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMaterial.MultiSelect = false;
        }

        private void ConfigurarGridLista()
        {
            dgvLista.AutoGenerateColumns = false;
            dgvLista.Columns.Clear();

            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                DataPropertyName = "Id",
                Visible = false
            });

            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Tipo",
                HeaderText = "Tipo",
                DataPropertyName = "Tipo",
                Width = 80,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                }
            });

            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Descricao",
                HeaderText = "Descrição",
                DataPropertyName = "Descricao",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = false,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            });

            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QuantidadeNumerica",
                HeaderText = "Quantidade",
                DataPropertyName = "QuantidadeNumerica",
                Width = 100,
                ReadOnly = false,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N3",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ValorMercado",
                HeaderText = "Valor Mercado",
                DataPropertyName = "ValorMercado",
                Width = 120,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = Color.LightGray
                }
            });

            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ValorVendaIndividual",
                HeaderText = "Valor Venda Individual",
                DataPropertyName = "ValorVendaIndividual",
                Width = 150,
                ReadOnly = false,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalIndividual",
                HeaderText = "Total Individual",
                DataPropertyName = "TotalIndividual",
                Width = 130,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = Color.LightYellow,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                }
            });

            dgvLista.AllowUserToAddRows = false;
            dgvLista.AllowUserToDeleteRows = false;
            dgvLista.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvLista.MultiSelect = false;
        }

        #endregion

        #region Eventos da Grid de Lista

        private void dgvLista_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvLista.Rows[e.RowIndex].DataBoundItem is ItemLista item)
            {
                string coluna = dgvLista.Columns[e.ColumnIndex].Name;

                if (coluna == "Descricao" && item.Tipo != "PRODUTO")
                {
                    e.Cancel = true;
                    return;
                }

                if (coluna == "QuantidadeNumerica" && item.Tipo != "PRODUTO")
                {
                    e.Cancel = true;
                    return;
                }

                if (coluna == "ValorMercado")
                {
                    e.Cancel = true;
                    return;
                }

                if (coluna == "TotalIndividual")
                {
                    e.Cancel = true;
                    return;
                }

                if (coluna == "ValorVendaIndividual")
                {
                    var celula = dgvLista.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    if (celula.Value != null && celula.Value != DBNull.Value)
                    {
                        decimal valor = Convert.ToDecimal(celula.Value);
                        celula.Value = valor;
                    }
                }
            }
        }

        private void dgvLista_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string coluna = dgvLista.Columns[e.ColumnIndex].Name;

            if (coluna == "QuantidadeNumerica")
            {
                string valorTexto = e.FormattedValue.ToString().Replace("R$", "").Replace(".", "").Replace(" ", "").Trim();

                if (!decimal.TryParse(valorTexto, out decimal valor) || valor <= 0)
                {
                    MessageBox.Show("Por favor, insira um valor numérico válido maior que zero.",
                        "Valor Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }

            if (coluna == "ValorVendaIndividual")
            {
                string valorTexto = e.FormattedValue.ToString()
                    .Replace("R$", "")
                    .Replace(".", "")
                    .Replace(" ", "")
                    .Trim();

                if (!decimal.TryParse(valorTexto, out decimal valor) || valor < 0)
                {
                    MessageBox.Show("Por favor, insira apenas números (exemplo: 1500 ou 1500,50).",
                        "Valor Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }

        private void dgvLista_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvLista.Rows[e.RowIndex].DataBoundItem is ItemLista item)
            {
                string coluna = dgvLista.Columns[e.ColumnIndex].Name;

                if (coluna == "QuantidadeNumerica" || coluna == "ValorVendaIndividual")
                {
                    item.TotalIndividual = Math.Round(item.QuantidadeNumerica * item.ValorVendaIndividual, 2);
                    dgvLista.Refresh();
                    AtualizarValorTotal();
                }
            }
        }

        private void DgvLista_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvLista.Rows[e.RowIndex].DataBoundItem is ItemLista item)
            {
                if (dgvLista.Columns[e.ColumnIndex].Name == "Tipo" && e.Value != null)
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
        }

        #endregion

        #region Estilo Visual

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

        private void CarregarPaletesDisponiveis()
        {
            try
            {
                var paletes = _ctx.Database.SqlQuery<PaleteDisponivel>(@"
                    SELECT 
                        p.Id AS PaleteId,
                        p.Numero AS NumeroPalete,
                        cp.Nome AS Categoria,
                        ISNULL(SUM(ip.Quantidade * ip.ValorUnitario), 0) AS ValorTotal,
                        COUNT(ip.Id) AS QtdItens
                    FROM Palete p
                    INNER JOIN CategoriaPalete cp ON p.CategoriaId = cp.Id
                    LEFT JOIN ItemPalete ip ON p.Id = ip.PaleteId
                    WHERE p.Status = 2
                    GROUP BY p.Id, p.Numero, cp.Nome
                    ORDER BY p.Numero DESC
                ").ToList();

                dgvPalete.DataSource = paletes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar paletes: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarMateriaisDisponiveis()
        {
            try
            {
                var materiais = _ctx.Database.SqlQuery<MaterialDisponivel>(@"
                    SELECT 
                        Id AS EstoqueId,
                        Material,
                        Quantidade
                    FROM Estoques
                    WHERE Status = 'Segregado' 
                      AND Quantidade > 0
                      AND DataSaida IS NULL
                    ORDER BY Material, DataEntrada
                ").ToList();

                dgvMaterial.DataSource = materiais;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar materiais: {ex.Message}\n\nDetalhes: {ex.InnerException?.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNovoItem_Click(object sender, EventArgs e)
        {
            var novoItem = new ItemLista
            {
                Id = 0,
                Tipo = "PRODUTO",
                Descricao = "",
                QuantidadeNumerica = 1,
                ValorMercado = 0,
                ValorVendaIndividual = 0,
                TotalIndividual = 0
            };

            _listaItens.Add(novoItem);
            dgvLista.Refresh();
            AtualizarValorTotal();
        }

        #endregion

        #region Atualização de Totais

        private void AtualizarValorTotal()
        {
            decimal total = Math.Round(_listaItens.Sum(item => item.TotalIndividual), 2);
            lblValorTotal.Text = $"Valor Total: {total:C2}";
        }

        #endregion

        #region Botões de Ação

        private void btnNovo_Click(object sender, EventArgs e)
        {
            HabilitarCampos(true);
            LimparCampos();
            cmbCliente.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Deseja realmente cancelar? Todos os dados não salvos serão perdidos.",
                "Confirmar Cancelamento",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LimparCampos();
                HabilitarCampos(false);
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            try
            {
                using (var transaction = _ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var resultado = CriarVenda();
                        int vendaId = resultado.VendaId;
                        int numeroPedido = resultado.NumeroPedido;

                        if (vendaId <= 0)
                        {
                            throw new Exception("Erro ao criar venda. ID inválido.");
                        }

                        AdicionarItensDaLista(vendaId);

                        transaction.Commit();

                        MessageBox.Show(
                            $"Venda realizada com sucesso!\nNúmero do Pedido: #{numeroPedido}",
                            "Sucesso",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        CarregarPaletesDisponiveis();
                        CarregarMateriaisDisponiveis();
                        LimparCampos();
                        HabilitarCampos(false);
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
                var erro = ex;
                string detalhes = "";

                while (erro != null)
                {
                    detalhes += $"Mensagem: {erro.Message}\n\n";
                    detalhes += $"StackTrace: {erro.StackTrace}\n\n";
                    erro = erro.InnerException;
                }

                MessageBox.Show(
                    $"Erro ao salvar venda:\n\n{detalhes}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }

        #endregion

        #region Validação e Salvamento

        private bool ValidarCampos()
        {
            if (cmbCliente.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione um cliente.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCliente.Focus();
                return false;
            }

            if (cmbFormaPagamento.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione uma forma de pagamento.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbFormaPagamento.Focus();
                return false;
            }

            if (!rbCIF.Checked && !rbFOB.Checked)
            {
                MessageBox.Show("Selecione a modalidade de frete.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_listaItens.Count == 0)
            {
                MessageBox.Show("Adicione ao menos um item (palete ou material) à lista.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            foreach (var item in _listaItens)
            {
                if (item.QuantidadeNumerica <= 0)
                {
                    MessageBox.Show($"O item '{item.Descricao}' está com quantidade inválida.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (item.ValorVendaIndividual <= 0)
                {
                    MessageBox.Show($"O item '{item.Descricao}' está com Valor Venda Individual inválido.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (item.Tipo == "PRODUTO" && string.IsNullOrWhiteSpace(item.Descricao))
                {
                    MessageBox.Show("A descrição do produto não pode estar vazia.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private VendaResultado CriarVenda()
        {
            int clienteId = (int)cmbCliente.SelectedValue;
            string formaPagamento = cmbFormaPagamento.Text;
            string modalidadeFrete = rbCIF.Checked ? "CIF" : "FOB";

            decimal valorTotal = Math.Round(_listaItens.Sum(i => i.TotalIndividual), 2);

            decimal valorCustoTotal = Math.Round(_listaItens.Where(i => i.Tipo == "PALETE").Sum(i => i.ValorMercado), 2);

            decimal percentualDesconto = 0;

            if (valorCustoTotal > 0 && valorTotal < valorCustoTotal)
            {
                var calculado = ((valorCustoTotal - valorTotal) / valorCustoTotal) * 100;
                calculado = Math.Round(calculado, 2);

                if (calculado < 0) calculado = 0;
                if (calculado > 99.99M) calculado = 99.99M;

                percentualDesconto = calculado;
            }

            var maxPedido = _ctx.Database.SqlQuery<int?>(
                "SELECT ISNULL(MAX(NumeroPedido), 0) FROM Vendas WITH (UPDLOCK, ROWLOCK)"
            ).FirstOrDefault() ?? 0;

            int numeroPedido = maxPedido + 1;

            var resultado = _ctx.Database.SqlQuery<VendaResultado>(@"
                INSERT INTO Vendas (
                    NumeroPedido, ClienteId, FormaPagamento, ModalidadeFrete,
                    ValorTotal, PercentualDesconto,
                    UsuarioId, StatusVenda, DataVenda, DataCriacao
                )
                OUTPUT INSERTED.VendaId, INSERTED.NumeroPedido
                VALUES (
                    @p0, @p1, @p2, @p3, @p4, @p5, @p6, 'Ativo', GETDATE(), GETDATE()
                )",
                numeroPedido,
                clienteId,
                formaPagamento,
                modalidadeFrete,
                valorTotal,
                percentualDesconto,
                (object)_usuarioId ?? DBNull.Value
            ).FirstOrDefault();

            if (resultado == null || resultado.VendaId <= 0)
            {
                throw new Exception("Falha ao recuperar ID da venda criada.");
            }

            return resultado;
        }

        private void AdicionarItensDaLista(int vendaId)
        {
            foreach (var item in _listaItens)
            {
                try
                {
                    if (item.Tipo == "PALETE")
                    {
                        var statusAtual = _ctx.Database.SqlQuery<int?>(
                            "SELECT Status FROM Palete WITH (UPDLOCK) WHERE Id = @p0",
                            item.Id
                        ).FirstOrDefault();

                        if (statusAtual == null || statusAtual != 2)
                        {
                            throw new Exception($"O palete #{item.Id} não está mais disponível para venda.");
                        }

                        _ctx.Database.ExecuteSqlCommand(
                            "UPDATE Palete SET Status = 3 WHERE Id = @p0",
                            item.Id
                        );

                        decimal descontoPalete = 0;
                        if (item.ValorMercado > 0 && item.TotalIndividual < item.ValorMercado)
                        {
                            descontoPalete = Math.Round(item.ValorMercado - item.TotalIndividual, 2);
                        }

                        decimal totalIndividualArredondado = Math.Round(item.TotalIndividual, 2);

                        _ctx.Database.ExecuteSqlCommand(@"
                            INSERT INTO VendaPalete (VendaId, PaleteId, ValorTotal, Desconto)
                            VALUES (@p0, @p1, @p2, @p3)",
                            vendaId,
                            item.Id,
                            totalIndividualArredondado,
                            descontoPalete
                        );
                    }
                    else if (item.Tipo == "MATERIAL")
                    {
                        var qtdResult = _ctx.Database.SqlQuery<decimal?>(
                            "SELECT CAST(Quantidade AS DECIMAL(18,3)) FROM Estoques WHERE Id = @p0",
                            new System.Data.SqlClient.SqlParameter("@p0", item.Id)
                        ).FirstOrDefault();

                        if (!qtdResult.HasValue)
                        {
                            throw new Exception($"Estoque ID {item.Id} não encontrado ou quantidade nula.");
                        }

                        var qtd = qtdResult.Value;

                        if (qtd < item.QuantidadeNumerica)
                        {
                            throw new Exception($"Quantidade insuficiente. Disponível: {qtd:N3} KG");
                        }

                        _ctx.Database.ExecuteSqlCommand(@"
                            UPDATE Estoques 
                            SET Status = 'Vendido',
                                DataSaida = GETDATE()
                            WHERE Id = @p0",
                            item.Id
                        );

                        decimal quantidadeArredondada = Math.Round(item.QuantidadeNumerica, 3);
                        decimal totalIndividualArredondado = Math.Round(item.TotalIndividual, 2);

                        _ctx.Database.ExecuteSqlCommand(@"
                            INSERT INTO VendaMaterial (VendaId, EstoqueId, Quantidade, ValorTotal)
                            VALUES (@p0, @p1, @p2, @p3)",
                            vendaId,
                            item.Id,
                            quantidadeArredondada,
                            totalIndividualArredondado
                        );
                    }
                    else if (item.Tipo == "PRODUTO")
                    {
                        string quantidadeTexto = $"{item.QuantidadeNumerica:N0} un";

                        decimal quantidadeArredondada = Math.Round(item.QuantidadeNumerica, 2);
                        decimal totalIndividualArredondado = Math.Round(item.TotalIndividual, 2);

                        _ctx.Database.ExecuteSqlCommand(@"
                            INSERT INTO VendaProdutoManual 
                            (VendaId, Descricao, Quantidade, QuantidadeDecimal, 
                             ValorCusto, ValorVenda, Desconto, DataCriacao)
                            VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, GETDATE())",
                            vendaId,
                            item.Descricao ?? "",
                            quantidadeTexto,
                            quantidadeArredondada,
                            0,
                            totalIndividualArredondado,
                            0
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ERRO NO ITEM: {item.Descricao}\n\nErro: {ex.Message}\n\nInner: {ex.InnerException?.Message}\n\nStack: {ex.StackTrace}",
                        "ERRO DETALHADO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }
        }

        #endregion

        #region Adicionar/Remover Itens

        private void btnAdicionarPalete_Click(object sender, EventArgs e)
        {
            if (dgvPalete.CurrentRow == null)
            {
                MessageBox.Show("Selecione uma palete.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var palete = dgvPalete.CurrentRow.DataBoundItem as PaleteDisponivel;
            if (palete == null) return;

            if (_listaItens.FirstOrDefault(i => i.Tipo == "PALETE" && i.Id == palete.PaleteId) != null)
            {
                MessageBox.Show("Esta palete já foi adicionada à lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _listaItens.Add(new ItemLista
            {
                Id = palete.PaleteId,
                Tipo = "PALETE",
                Descricao = $"Palete #{palete.NumeroPalete} - {palete.Categoria}",
                QuantidadeNumerica = 1,
                ValorMercado = palete.ValorTotal,
                ValorVendaIndividual = 0,
                TotalIndividual = 0
            });

            AtualizarValorTotal();
        }

        private void btnAdicionarMaterial_Click(object sender, EventArgs e)
        {
            if (dgvMaterial.CurrentRow == null)
            {
                MessageBox.Show("Selecione um material.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var material = dgvMaterial.CurrentRow.DataBoundItem as MaterialDisponivel;
            if (material == null) return;

            if (_listaItens.FirstOrDefault(i => i.Tipo == "MATERIAL" && i.Id == material.EstoqueId) != null)
            {
                MessageBox.Show("Este material já foi adicionado à lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _listaItens.Add(new ItemLista
            {
                Id = material.EstoqueId,
                Tipo = "MATERIAL",
                Descricao = material.Material,
                QuantidadeNumerica = material.Quantidade,
                ValorMercado = 0,
                ValorVendaIndividual = 0,
                TotalIndividual = 0
            });

            AtualizarValorTotal();
        }

        private void btnRemoverItem_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null)
            {
                MessageBox.Show("Selecione um item para remover.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Deseja remover o item selecionado da lista?",
                "Confirmar Remoção",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            if (dgvLista.CurrentRow.DataBoundItem is ItemLista item)
            {
                _listaItens.Remove(item);
            }

            AtualizarValorTotal();
        }

        #endregion

        #region Utilitários

        private void LimparCampos()
        {
            cmbCliente.SelectedIndex = -1;
            cmbFormaPagamento.SelectedIndex = -1;
            rbCIF.Checked = true;

            _listaItens.Clear();
            lblValorTotal.Text = "Valor Total: R$ 0,00";
        }

        private void HabilitarCampos(bool habilitar)
        {
            cmbCliente.Enabled = habilitar;
            cmbFormaPagamento.Enabled = habilitar;
            rbCIF.Enabled = habilitar;
            rbFOB.Enabled = habilitar;

            btnAdicionarPalete.Enabled = habilitar;
            btnAdicionarMaterial.Enabled = habilitar;
            btnNovoItem.Enabled = habilitar;
            btnRemoverItem.Enabled = habilitar;
            btnSalvar.Enabled = habilitar;
        }

        private void btnPedidos_Click(object sender, EventArgs e)
        {
            var FormPedidos = new ComercialFormPedidos();
            FormPedidos.ShowDialog();
        }

        #endregion

        #region Classes Auxiliares

        public class PaleteDisponivel
        {
            public int PaleteId { get; set; }
            public int NumeroPalete { get; set; }
            public string Categoria { get; set; }
            public decimal ValorTotal { get; set; }
            public int QtdItens { get; set; }
        }

        public class VendaProdutoManual
        {
            public int VendaProdutoManualId { get; set; }
            public int VendaId { get; set; }
            public string Descricao { get; set; }
            public string Quantidade { get; set; }
            public decimal? QuantidadeDecimal { get; set; }
            public decimal ValorCusto { get; set; }
            public decimal ValorVenda { get; set; }
            public decimal? Desconto { get; set; }
            public DateTime DataCriacao { get; set; }
        }

        public class MaterialDisponivel
        {
            public int EstoqueId { get; set; }
            public string Material { get; set; }
            public decimal Quantidade { get; set; }
        }

        public class VendaResultado
        {
            public int VendaId { get; set; }
            public int NumeroPedido { get; set; }
        }

        public class MaterialValidacao
        {
            public decimal Quantidade { get; set; }
            public bool TemQuantidade { get; set; }
        }

        public class ItemLista
        {
            public int Id { get; set; }
            public string Tipo { get; set; }
            public string Descricao { get; set; }
            public decimal QuantidadeNumerica { get; set; }
            public decimal ValorMercado { get; set; }
            public decimal ValorVendaIndividual { get; set; }
            public decimal TotalIndividual { get; set; }
        }

        #endregion
    }
}