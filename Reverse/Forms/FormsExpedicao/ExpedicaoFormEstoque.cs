using Reverse.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ADGV;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormEstoque : Form
    {
        private BindingList<EstoqueViewModel> _estoqueList;
        private EstoqueViewModel _linhaAtual = null;
        private bool _atualizandoCampos = false;
        private readonly int _usuarioId;
        private BindingSource _bindingSource;

        public ExpedicaoFormEstoque(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            _estoqueList = new BindingList<EstoqueViewModel>();
        }

        private void FormEstoque_Load(object sender, EventArgs e)
        {
            string[] meses = { "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
            "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };
            cmbMes.Items.AddRange(meses);
            cmbMes.SelectedIndex = DateTime.Now.Month - 1;

            for (int ano = 2025; ano <= 2030; ano++)
                cmbAno.Items.Add(ano);
            cmbAno.SelectedItem = DateTime.Now.Year;

            using (var ctx = new ReverseContext())
            {
                var materiais = ctx.Materiais.OrderBy(m => m.Nome).ToList();
                cmbMaterial.DataSource = materiais;
                cmbMaterial.DisplayMember = "Nome";
                cmbMaterial.ValueMember = "Id";
            }

            cmbMaterial.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbMaterial.AutoCompleteSource = AutoCompleteSource.ListItems;

            cmbStatus.Items.AddRange(new string[] { "Aguardando Peso", "Segregado", "Vendido" });

            using (var ctx = new ReverseContext())
            {
                var clientes = ctx.Clientes.OrderBy(c => c.Nome).ToList();
                cmbCliente.DataSource = clientes;
                cmbCliente.DisplayMember = "Nome";
                cmbCliente.ValueMember = "ClienteId";
            }

            cmbCliente.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbCliente.AutoCompleteSource = AutoCompleteSource.ListItems;

            ConfigurarGrid();

            _bindingSource = new BindingSource();
            dgvMaterial.DataSource = _bindingSource;

            dgvMaterial.FilterStringChanged += (s, ev) =>
            {
                _bindingSource.Filter = dgvMaterial.FilterString;
                AtualizarPesoFiltro();
            };
            dgvMaterial.SortStringChanged += (s, ev) =>
            {
                _bindingSource.Sort = dgvMaterial.SortString;
                AtualizarPesoFiltro();
            };

            rbmQuantidade.Checked = true;
            lblPesoQuantidade.Text = "Quantidade";
            txtQuantidade.KeyPress += TxtQuantidade_KeyPressInteiro;
            rbmQuantidade.CheckedChanged += rbmQuantidade_CheckedChanged;
            rbmPeso.CheckedChanged += rbmPeso_CheckedChanged;

            txtQuantidade.Leave += txtQuantidade_Leave;
            cmbMaterial.Leave += (s, ev) => AtualizarCampo_Material();
            cmbStatus.Leave += (s, ev) => AtualizarCampo_Status();
            cmbCliente.Leave += (s, ev) => AtualizarCampo_Cliente();
            txtObs.Leave += (s, ev) => AtualizarCampo_Observacao();

            this.cmbMes.SelectedIndexChanged += new System.EventHandler(this.cmbMes_SelectedIndexChanged);
            this.cmbAno.SelectedIndexChanged += new System.EventHandler(this.cmbAno_SelectedIndexChanged);

            CarregarEstoque();
            LimparCampos();
        }

        private void AtualizarCampo_Material()
        {
            if (_linhaAtual == null || _atualizandoCampos) return;

            try
            {
                string materialAnterior = _linhaAtual.Material;
                string materialNovo = cmbMaterial.SelectedItem is Material mat ? mat.Nome : cmbMaterial.Text;

                if (materialAnterior != materialNovo)
                {
                    _linhaAtual.Material = materialNovo;

                    using (var ctx = new ReverseContext())
                    {
                        var material = ctx.Materiais.FirstOrDefault(m => m.Nome == materialNovo);
                        if (material != null)
                        {
                            _linhaAtual.Valorizacao = material.Valorizacao;
                        }
                    }

                    AtualizarLinhaGrid();
                }
            }
            catch { }
        }

        private void AtualizarCampo_Status()
        {
            if (_linhaAtual == null || _atualizandoCampos) return;

            try
            {
                string statusNovo = cmbStatus.SelectedItem?.ToString() ?? "";

                if (_linhaAtual.Status != statusNovo)
                {
                    _linhaAtual.Status = statusNovo;

                    if (_linhaAtual.Status == "Vendido" && _linhaAtual.DataSaida == null)
                    {
                        _linhaAtual.DataSaida = DateTime.Now;
                    }
                    else if (_linhaAtual.Status != "Vendido")
                    {
                        _linhaAtual.DataSaida = null;
                    }

                    AtualizarLinhaGrid();
                }
            }
            catch { }
        }

        private void AtualizarCampo_Cliente()
        {
            if (_linhaAtual == null || _atualizandoCampos) return;

            try
            {
                string clienteNovo = cmbCliente.Text;

                if (_linhaAtual.ClienteNome != clienteNovo)
                {
                    _linhaAtual.ClienteNome = clienteNovo;
                    AtualizarLinhaGrid();
                }
            }
            catch { }
        }

        private void AtualizarCampo_Observacao()
        {
            if (_linhaAtual == null || _atualizandoCampos) return;

            try
            {
                if (_linhaAtual.Observacao != txtObs.Text)
                {
                    _linhaAtual.Observacao = txtObs.Text;
                    AtualizarLinhaGrid();
                }
            }
            catch { }
        }

        private void AtualizarLinhaGrid()
        {
            if (dgvMaterial.CurrentRow != null)
            {
                int rowIndex = dgvMaterial.CurrentRow.Index;
                dgvMaterial.InvalidateRow(rowIndex);
            }
        }



        private void ConfigurarGrid()
        {
            dgvMaterial.AutoGenerateColumns = false;
            dgvMaterial.Columns.Clear();

            var colMaterial = new DataGridViewTextBoxColumn
            {
                Name = "Material",
                HeaderText = "Material",
                DataPropertyName = "Material",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 9F),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            colMaterial.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colValorizacao = new DataGridViewTextBoxColumn
            {
                Name = "Valorizacao",
                HeaderText = "Valorização",
                DataPropertyName = "Valorizacao",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 11,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Black,
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9F)
                }
            };
            colValorizacao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colDataEntrada = new DataGridViewTextBoxColumn
            {
                Name = "DataEntrada",
                HeaderText = "Data Entrada",
                DataPropertyName = "DataEntrada",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy",
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 9F),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            colDataEntrada.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colDataSaida = new DataGridViewTextBoxColumn
            {
                Name = "DataSaida",
                HeaderText = "Data Saída",
                DataPropertyName = "DataSaida",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy",
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 9F),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            colDataSaida.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colQuantidade = new DataGridViewTextBoxColumn
            {
                Name = "Quantidade",
                HeaderText = "Unidade/Peso",
                DataPropertyName = "Quantidade",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 13,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Black,
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "N2",
                    Font = new Font("Segoe UI", 9F)
                }
            };
            colQuantidade.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colStatus = new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                DataPropertyName = "Status",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 9F),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            colStatus.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colCliente = new DataGridViewTextBoxColumn
            {
                Name = "Cliente",
                HeaderText = "Cliente",
                DataPropertyName = "ClienteNome",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 27,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 9F),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            colCliente.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colObservacao = new DataGridViewTextBoxColumn
            {
                Name = "Observacao",
                HeaderText = "Observação",
                DataPropertyName = "Observacao",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 9F),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            colCliente.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colId = new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "Id",
                DataPropertyName = "Id",
                Visible = false
            };

            dgvMaterial.Columns.AddRange(new DataGridViewColumn[]
            {
                colMaterial, colValorizacao, colDataEntrada, colDataSaida, colQuantidade,
                colStatus, colCliente, colObservacao, colId
            });

            dgvMaterial.AllowUserToAddRows = false;
            dgvMaterial.AllowUserToDeleteRows = false;
            dgvMaterial.ReadOnly = true;
            dgvMaterial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMaterial.MultiSelect = false;
            dgvMaterial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMaterial.RowHeadersWidth = 25;

        }

        private void AtualizarPesoFiltro()
        {
            decimal totalPeso = 0;

            foreach (DataRowView rowView in _bindingSource.List)
            {
                if (rowView["Quantidade"] != DBNull.Value)
                {
                    totalPeso += Convert.ToDecimal(rowView["Quantidade"]);
                }
            }

            lblPesoFiltro.Text = $"Total filtrado: {totalPeso:N3}";
        }


        private void dgvMaterial_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvMaterial.Columns[e.ColumnIndex].Name == "Quantidade" && e.Value != null)
            {
                var item = dgvMaterial.Rows[e.RowIndex].DataBoundItem as EstoqueViewModel;
                if (item != null)
                {
                    if (item.EhPeso)
                        e.Value = ((decimal)e.Value).ToString("N3");
                    else
                        e.Value = ((decimal)e.Value).ToString("N0");
                    e.FormattingApplied = true;
                }
            }

            if (dgvMaterial.Columns[e.ColumnIndex].Name == "Valorizacao" && e.Value != null)
            {
                int val = (int)e.Value;
                if (val >= 1 && val <= 5)
                    e.Value = new string('★', val);
                else
                    e.Value = "-";
                e.FormattingApplied = true;
            }

            if (dgvMaterial.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                string status = e.Value.ToString();

                if (status == "Aguardando Peso")
                {
                    e.CellStyle.BackColor = Color.FromArgb(177, 2, 2);
                    e.CellStyle.ForeColor = Color.White;
                }
                else if (status == "Segregado")
                    e.CellStyle.BackColor = Color.FromArgb(255, 229, 160);
                else if (status == "Vendido")
                {
                    e.CellStyle.BackColor = Color.FromArgb(17, 115, 75);
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }

        private void CarregarEstoque()
        {
            if (cmbMes.SelectedIndex < 0 || cmbAno.SelectedItem == null)
                return;

            int mes = cmbMes.SelectedIndex + 1;
            int ano = (int)cmbAno.SelectedItem;

            try
            {
                using (var ctx = new ReverseContext())
                {
                    var dados = ctx.Estoques
                        .Where(e => e.Mes == mes && e.Ano == ano)
                        .OrderBy(e => e.DataEntrada)
                        .ToList();

                    var nomesMateriais = dados
                        .Where(d => !string.IsNullOrWhiteSpace(d.Material))
                        .Select(d => d.Material)
                        .Distinct()
                        .ToList();

                    var materiais = ctx.Materiais
                        .Where(m => nomesMateriais.Contains(m.Nome))
                        .ToDictionary(m => m.Nome, m => m.Valorizacao);

                    var clienteIds = dados
                        .Where(d => d.ClienteId > 0)
                        .Select(d => d.ClienteId)
                        .Distinct()
                        .ToList();

                    var clientes = ctx.Clientes
                        .Where(c => clienteIds.Contains(c.ClienteId))
                        .ToDictionary(c => c.ClienteId, c => c.Nome);

                    _estoqueList.Clear();

                    foreach (var item in dados)
                    {
                        _estoqueList.Add(new EstoqueViewModel
                        {
                            Id = item.Id,
                            Material = item.Material ?? "",
                            Valorizacao = materiais.ContainsKey(item.Material) ? materiais[item.Material] : 1,
                            DataEntrada = item.DataEntrada,
                            DataSaida = item.DataSaida,
                            Quantidade = item.Quantidade,
                            Status = item.Status ?? "Aguardando Peso",
                            ClienteNome = item.ClienteId > 0 && clientes.ContainsKey(item.ClienteId)
                                          ? clientes[item.ClienteId]
                                          : "",
                            Observacao = item.Observacao ?? "",
                            EhPeso = item.EhPeso
                        });
                    }

                    var dt = new DataTable();
                    dt.Columns.Add("Id", typeof(int));
                    dt.Columns.Add("Material", typeof(string));
                    dt.Columns.Add("Valorizacao", typeof(int));
                    dt.Columns.Add("DataEntrada", typeof(DateTime));
                    dt.Columns.Add("DataSaida", typeof(DateTime));
                    dt.Columns.Add("Quantidade", typeof(decimal));
                    dt.Columns.Add("Status", typeof(string));
                    dt.Columns.Add("ClienteNome", typeof(string));
                    dt.Columns.Add("Observacao", typeof(string));

                    foreach (var item in _estoqueList)
                    {
                        dt.Rows.Add(item.Id, item.Material, item.Valorizacao, item.DataEntrada, item.DataSaida,
                                    item.Quantidade, item.Status, item.ClienteNome, item.Observacao);
                    }

                    _bindingSource.DataSource = dt;
                    AtualizarPesoFiltro();
                }

                LimparCampos();
                _linhaAtual = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar estoque: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rbmQuantidade_CheckedChanged(object sender, EventArgs e)
        {
            if (rbmQuantidade.Checked)
            {
                lblPesoQuantidade.Text = "Unidade";

                txtQuantidade.Text = "";
                txtQuantidade.KeyPress -= TxtQuantidade_KeyPressDecimal;
                txtQuantidade.KeyPress += TxtQuantidade_KeyPressInteiro;

            }
        }

        private void rbmPeso_CheckedChanged(object sender, EventArgs e)
        {
            if (rbmPeso.Checked)
            {
                lblPesoQuantidade.Text = "Peso";

                txtQuantidade.Text = "";
                txtQuantidade.KeyPress -= TxtQuantidade_KeyPressInteiro;
                txtQuantidade.KeyPress += TxtQuantidade_KeyPressDecimal;

            }
        }

        private void TxtQuantidade_KeyPressInteiro(object sender, KeyPressEventArgs e)
        {
            // Só aceita números e backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TxtQuantidade_KeyPressDecimal(object sender, KeyPressEventArgs e)
        {
            // Aceita números, vírgula e backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true;
            }

            // Só permite uma vírgula
            if (e.KeyChar == ',' && (sender as TextBox).Text.Contains(","))
            {
                e.Handled = true;
            }
        }

        private void txtQuantidade_Leave(object sender, EventArgs e)
        {
            if (_linhaAtual == null || _atualizandoCampos) return;

            if (string.IsNullOrWhiteSpace(txtQuantidade.Text))
            {
                if (_linhaAtual.Quantidade != 0)
                {
                    _linhaAtual.Quantidade = 0;
                    AtualizarLinhaGrid();
                }
                return;
            }

            if (decimal.TryParse(txtQuantidade.Text,
                                 NumberStyles.Number,
                                 CultureInfo.CurrentCulture,
                                 out decimal valor))
            {
                decimal valorFinal;

                if (_linhaAtual.EhPeso)
                {
                    valorFinal = Math.Round(valor, 3, MidpointRounding.AwayFromZero);
                    txtQuantidade.Text = valorFinal.ToString("N3");
                }
                else
                {
                    valorFinal = Math.Truncate(valor);
                    txtQuantidade.Text = valorFinal.ToString("N0");
                }

                if (_linhaAtual.Quantidade != valorFinal)
                {
                    _linhaAtual.Quantidade = valorFinal;
                    AtualizarLinhaGrid();
                }
            }
            else
            {
                if (_linhaAtual.Quantidade != 0)
                {
                    _linhaAtual.Quantidade = 0;
                    txtQuantidade.Text = "";
                    AtualizarLinhaGrid();
                }
            }
        }

        private void cmbMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarEstoque();
        }

        private void cmbAno_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarEstoque();
        }

        private void btnNovaLinha_Click(object sender, EventArgs e)
        {
            var novaLinha = new EstoqueViewModel
            {
                Id = 0,
                Material = "",
                DataEntrada = DateTime.Now,
                DataSaida = null,
                Quantidade = 0,
                Status = "Aguardando Peso",
                ClienteNome = "",
                Observacao = "",
                EhPeso = rbmPeso.Checked
            };

            _estoqueList.Add(novaLinha);

            dgvMaterial.ClearSelection();
            int novoIndex = dgvMaterial.Rows.Count - 1;
            dgvMaterial.Rows[novoIndex].Selected = true;
            dgvMaterial.CurrentCell = dgvMaterial.Rows[novoIndex].Cells[0];

            // Carregar nos campos
            _linhaAtual = novaLinha;
            CarregarCamposDaLinhaNovaLinha(novaLinha);
        }

        private void CarregarCamposDaLinhaNovaLinha(EstoqueViewModel estoque)
        {
            _atualizandoCampos = true;

            cmbMaterial.SelectedIndex = -1;

            txtQuantidade.Text = "";

            cmbStatus.SelectedItem = estoque.Status;

            cmbCliente.SelectedIndex = -1;

            txtObs.Text = "";

            _atualizandoCampos = false;

            cmbMaterial.Focus();
        }

        private void btnExcluirLinha_Click(object sender, EventArgs e)
        {
            if (dgvMaterial.CurrentRow == null)
            {
                MessageBox.Show("Selecione uma linha para excluir.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var estoque = dgvMaterial.CurrentRow.DataBoundItem as EstoqueViewModel;
            if (estoque == null) return;

            if (estoque.Id == 0)
            {
                _estoqueList.Remove(estoque);
                _linhaAtual = null;
                LimparCampos();
                return;
            }

            var confirmar = MessageBox.Show("Deseja realmente excluir este registro?",
                "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmar != DialogResult.Yes) return;

            try
            {
                using (var ctx = new ReverseContext())
                {
                    var estoqueDb = ctx.Estoques.FirstOrDefault(e => e.Id == estoque.Id);
                    if (estoqueDb != null)
                    {
                        ctx.Estoques.Remove(estoqueDb);
                        ctx.SaveChanges();
                    }
                }

                MessageBox.Show("Registro excluído com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CarregarEstoque();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvMaterial_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMaterial.CurrentRow == null || dgvMaterial.CurrentRow.DataBoundItem == null)
            {
                _linhaAtual = null;
                return;
            }

            var rowView = dgvMaterial.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null) { _linhaAtual = null; return; }

            int id = rowView.Row.Field<int>("Id");
            var estoque = _estoqueList.FirstOrDefault(x => x.Id == id);
            if (estoque == null) { _linhaAtual = null; return; }

            _linhaAtual = estoque;

            _atualizandoCampos = true;
            rbmPeso.Checked = estoque.EhPeso;
            rbmQuantidade.Checked = !estoque.EhPeso;
            _atualizandoCampos = false;

            CarregarCamposDaLinha(estoque);
        }

        private void CarregarCamposDaLinha(EstoqueViewModel estoque)
        {
            _atualizandoCampos = true;

            if (cmbMaterial.DataSource is List<Material> lista)
            {
                var sel = lista.FirstOrDefault(m => m.Nome == estoque.Material);
                cmbMaterial.SelectedItem = sel;
            }
            else
            {
                cmbMaterial.SelectedItem = estoque.Material;
            }

            if (estoque.Quantidade > 0)
            {
                if (estoque.EhPeso)
                    txtQuantidade.Text = estoque.Quantidade.ToString("N3");
                else
                    txtQuantidade.Text = estoque.Quantidade.ToString("N0");
            }
            else
            {
                txtQuantidade.Text = "";
            }

            cmbStatus.SelectedItem = estoque.Status;

            if (!string.IsNullOrWhiteSpace(estoque.ClienteNome))
                cmbCliente.Text = estoque.ClienteNome;
            else
                cmbCliente.SelectedIndex = -1;

            txtObs.Text = estoque.Observacao;

            _atualizandoCampos = false;
        }

        private void LimparCampos()
        {
            _atualizandoCampos = true;
            cmbMaterial.SelectedIndex = -1;
            txtQuantidade.Clear();
            cmbStatus.SelectedIndex = -1;
            cmbCliente.SelectedIndex = -1;
            txtObs.Clear();
            _atualizandoCampos = false;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (cmbMes.SelectedIndex < 0 || cmbAno.SelectedItem == null)
            {
                MessageBox.Show("Selecione o mês e ano antes de salvar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var ctx = new ReverseContext())
                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        int mes = cmbMes.SelectedIndex + 1;
                        int ano = (int)cmbAno.SelectedItem;

                        var idsExistentes = _estoqueList.Where(x => x.Id > 0).Select(x => x.Id).ToList();
                        var estoquesDb = ctx.Estoques
                            .Where(e => idsExistentes.Contains(e.Id))
                            .ToDictionary(e => e.Id);

                        var nomesClientes = _estoqueList
                            .Where(x => !string.IsNullOrWhiteSpace(x.ClienteNome))
                            .Select(x => x.ClienteNome.Trim())
                            .Distinct()
                            .ToList();

                        var clientesDict = ctx.Clientes
                            .Where(c => nomesClientes.Contains(c.Nome))
                            .ToDictionary(c => c.Nome.Trim(), c => c.ClienteId, StringComparer.OrdinalIgnoreCase);

                        foreach (var item in _estoqueList)
                        {
                            if (string.IsNullOrWhiteSpace(item.Material))
                                continue;

                            Estoque estoque;
                            if (item.Id > 0)
                            {
                                if (!estoquesDb.TryGetValue(item.Id, out estoque))
                                    continue;
                            }
                            else
                            {
                                estoque = new Estoque();
                                ctx.Estoques.Add(estoque);
                            }

                            estoque.Material = item.Material;
                            estoque.DataEntrada = item.DataEntrada;
                            estoque.DataSaida = item.DataSaida;
                            estoque.Quantidade = item.EhPeso
                                ? Math.Round(item.Quantidade, 3, MidpointRounding.AwayFromZero)
                                : Math.Truncate(item.Quantidade);
                            estoque.EhPeso = item.EhPeso;
                            estoque.Status = item.Status ?? "Aguardando Peso";
                            estoque.Observacao = item.Observacao;
                            estoque.Mes = mes;
                            estoque.Ano = ano;

                            if (!string.IsNullOrWhiteSpace(item.ClienteNome) &&
                                clientesDict.TryGetValue(item.ClienteNome.Trim(), out int clienteId))
                            {
                                estoque.ClienteId = clienteId;
                            }
                            else
                            {
                                estoque.ClienteId = 0;
                            }
                        }

                        ctx.SaveChanges();
                        transaction.Commit();

                        MessageBox.Show("Estoque salvo com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        CarregarEstoque();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}\n\nDetalhes: {ex.InnerException?.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public class EstoqueViewModel : INotifyPropertyChanged
        {
            private int _id;
            private string _material;
            private DateTime _dataEntrada;
            private DateTime? _dataSaida;
            private decimal _quantidade;
            private string _status;
            private string _clienteNome;
            private string _observacao;
            public bool EhPeso { get; set; }
            private int _valorizacao;
            public int Valorizacao
            {
                get => _valorizacao;
                set { _valorizacao = value; OnPropertyChanged(nameof(Valorizacao)); }
            }

            public int Id
            {
                get => _id;
                set { _id = value; OnPropertyChanged(nameof(Id)); }
            }

            public string Material
            {
                get => _material;
                set { _material = value; OnPropertyChanged(nameof(Material)); }
            }

            public DateTime DataEntrada
            {
                get => _dataEntrada;
                set { _dataEntrada = value; OnPropertyChanged(nameof(DataEntrada)); }
            }

            public DateTime? DataSaida
            {
                get => _dataSaida;
                set { _dataSaida = value; OnPropertyChanged(nameof(DataSaida)); }
            }

            public decimal Quantidade
            {
                get => _quantidade;
                set { _quantidade = value; OnPropertyChanged(nameof(Quantidade)); }
            }

            public string Status
            {
                get => _status;
                set { _status = value; OnPropertyChanged(nameof(Status)); }
            }

            public string ClienteNome
            {
                get => _clienteNome;
                set { _clienteNome = value; OnPropertyChanged(nameof(ClienteNome)); }
            }

            public string Observacao
            {
                get => _observacao;
                set { _observacao = value; OnPropertyChanged(nameof(Observacao)); }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void btnMaterial_Click(object sender, EventArgs e)
        {
            var materialAtual = _linhaAtual?.Material;

            var formMaterial = new ExpedicaoFormMaterialEstoque(_usuarioId);
            formMaterial.ShowDialog();

            using (var ctx = new ReverseContext())
            {
                var materiais = ctx.Materiais.OrderBy(m => m.Nome).ToList();

                cmbMaterial.DataSource = materiais;
                cmbMaterial.DisplayMember = "Nome";
                cmbMaterial.ValueMember = "Id";

                if (!string.IsNullOrEmpty(materialAtual))
                {
                    var sel = materiais.FirstOrDefault(m => m.Nome == materialAtual);
                    if (sel != null)
                        cmbMaterial.SelectedItem = sel;
                }
            }
        }
    }
}