using Reverse.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.IO;
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
            cmbStatus.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbStatus.AutoCompleteSource = AutoCompleteSource.ListItems;

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
            btnExportarExcel.Click += btnExportarExcel_Click;

            txtQuantidade.Leave += txtQuantidade_Leave;
            cmbMaterial.Leave += (s, ev) => AtualizarCampo_Material();
            cmbStatus.Leave += (s, ev) => AtualizarCampo_Status();
            cmbCliente.Leave += (s, ev) => AtualizarCampo_Cliente();
            txtObs.Leave += (s, ev) => AtualizarCampo_Observacao();

            this.cmbMes.SelectedIndexChanged += new System.EventHandler(this.cmbMes_SelectedIndexChanged);
            this.cmbAno.SelectedIndexChanged += new System.EventHandler(this.cmbAno_SelectedIndexChanged);

            ConfigurarGridMaterialFiltrado();
            dgvMaterialFiltrado.CellFormatting += dgvMaterialFiltrado_CellFormatting;

            lblTotal30.Text = "0,000";
            lblSegregado.Text = "0,000";
            lblVolumes.Text = "0";

            CarregarEstoque();
            LimparCampos();
        }

        private void ConfigurarGridMaterialFiltrado()
        {
            dgvMaterialFiltrado.AutoGenerateColumns = true;
            dgvMaterialFiltrado.ReadOnly = true;
            dgvMaterialFiltrado.AllowUserToAddRows = false;
            dgvMaterialFiltrado.AllowUserToDeleteRows = false;
            dgvMaterialFiltrado.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMaterialFiltrado.MultiSelect = false;
            dgvMaterialFiltrado.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            AplicarEstiloVisualProducao(dgvMaterialFiltrado);
        }

        private void AtualizarCampo_Material()
        {
            if (_linhaAtual == null || _atualizandoCampos) return;

            try
            {
                string materialAnterior = _linhaAtual.Material;
                string materialNovo = cmbMaterial.SelectedItem is Material mat ? mat.Nome : cmbMaterial.Text;

                materialNovo = string.IsNullOrWhiteSpace(materialNovo) ? "" : materialNovo.Trim();

                if (materialAnterior != materialNovo)
                {
                    _linhaAtual.Material = materialNovo;

                    using (var ctx = new ReverseContext())
                    {
                        var material = ctx.Materiais.FirstOrDefault(m =>
                                            string.Equals(m.Nome, materialNovo, StringComparison.OrdinalIgnoreCase));
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
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            };
            colMaterial.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F)
                }
            };
            colValorizacao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            };
            colDataEntrada.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            };
            colDataSaida.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Format = "N2",
                    Font = new Font("Segoe UI", 9F)
                }
            };
            colQuantidade.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            };
            colStatus.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            };
            colCliente.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            };
            colObservacao.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

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

            AplicarEstiloVisualProducao(dgvMaterial);
        }

        private void AplicarEstiloVisualProducao(DataGridView grid)
        {
            grid.BackgroundColor = Color.FromArgb(250, 250, 252);
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(230, 230, 235);

            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
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
                        .OrderByDescending(e => e.Status == "Segregado")
                        .ThenBy(e => e.DataEntrada)
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
                    dt.Columns.Add("EhPeso", typeof(bool));

                    foreach (var item in _estoqueList)
                    {
                        dt.Rows.Add(item.Id, item.Material, item.Valorizacao, item.DataEntrada, item.DataSaida,
                                    item.Quantidade, item.Status, item.ClienteNome, item.Observacao, item.EhPeso);  // Incluir EhPeso
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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TxtQuantidade_KeyPressDecimal(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true;
            }

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
                EhPeso = rbmPeso.Checked,
                Valorizacao = 1
            };

            _estoqueList.Add(novaLinha);

            if (_bindingSource.DataSource is DataTable dt)
            {
                var newRow = dt.NewRow();
                newRow["Id"] = novaLinha.Id;
                newRow["Material"] = novaLinha.Material;
                newRow["Valorizacao"] = novaLinha.Valorizacao;
                newRow["DataEntrada"] = novaLinha.DataEntrada;
                newRow["DataSaida"] = DBNull.Value;
                newRow["Quantidade"] = novaLinha.Quantidade;
                newRow["Status"] = novaLinha.Status;
                newRow["ClienteNome"] = novaLinha.ClienteNome;
                newRow["Observacao"] = novaLinha.Observacao;
                dt.Rows.Add(newRow);

                dgvMaterial.ClearSelection();
                int novoIndex = dgvMaterial.Rows.Count - 1;
                if (novoIndex >= 0)
                {
                    dgvMaterial.Rows[novoIndex].Selected = true;
                    dgvMaterial.CurrentCell = dgvMaterial.Rows[novoIndex].Cells[0];
                }
            }

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

            var rowView = dgvMaterial.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null) return;

            int estoqueId = rowView.Row.Field<int>("Id");
            var estoque = _estoqueList.FirstOrDefault(e => e.Id == estoqueId);
            if (estoque == null) return;

            if (estoque.Id == 0)
            {
                _estoqueList.Remove(estoque);

                if (_bindingSource.DataSource is DataTable dt)
                {
                    rowView.Row.Delete();
                    dt.AcceptChanges();
                }

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
                lblTotal30.Text = "0,000";
                lblSegregado.Text = "0,000";
                lblVolumes.Text = "0";
                dgvMaterialFiltrado.DataSource = null;
                return;
            }

            var rowView = dgvMaterial.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null)
            {
                _linhaAtual = null;
                return;
            }

            int id = rowView.Row.Field<int>("Id");
            var estoque = _estoqueList.FirstOrDefault(x => x.Id == id);
            if (estoque == null)
            {
                _linhaAtual = null;
                return;
            }

            _linhaAtual = estoque;

            _atualizandoCampos = true;
            rbmPeso.Checked = estoque.EhPeso;
            rbmQuantidade.Checked = !estoque.EhPeso;
            _atualizandoCampos = false;

            CarregarCamposDaLinha(estoque);

            _ = AtualizarGridsResumoPorMaterialAsync(estoque.Material);
        }

        private async Task AtualizarGridsResumoPorMaterialAsync(string materialSelecionado)
        {
            if (string.IsNullOrWhiteSpace(materialSelecionado))
            {
                lblTotal30.Text = "0,000";
                lblSegregado.Text = "0,000";
                lblVolumes.Text = "0";
                dgvMaterialFiltrado.DataSource = null;
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    using (var ctx = new ReverseContext())
                    {
                        DateTime dataLimite = DateTime.Now.AddDays(-30);

                        var dados = ctx.Estoques
                            .Where(e => e.Material == materialSelecionado)
                            .GroupBy(e => 1)
                            .Select(g => new
                            {
                                Total30Dias = g.Where(e => e.DataEntrada >= dataLimite)
                                               .Sum(e => (decimal?)e.Quantidade) ?? 0,
                                TotalSegregado = g.Where(e => e.Status == "Segregado")
                                                  .Sum(e => (decimal?)e.Quantidade) ?? 0,
                                Volumes = g.Count(e => e.Status == "Segregado")
                            })
                            .FirstOrDefault();

                        var registros = ctx.Estoques
                            .Where(e => e.Material == materialSelecionado)
                            .OrderByDescending(e => e.DataEntrada)
                            .Select(e => new
                            {
                                e.Id,
                                e.Material,
                                e.DataEntrada,
                                e.DataSaida,
                                e.Quantidade,
                                e.Status,
                                ClienteNome = ctx.Clientes
                                    .Where(c => c.ClienteId == e.ClienteId)
                                    .Select(c => c.Nome)
                                    .FirstOrDefault() ?? "",
                                e.Observacao
                            })
                            .ToList();

                        this.Invoke(new Action(() =>
                        {
                            lblTotal30.Text = (dados?.Total30Dias ?? 0).ToString("N3");
                            lblSegregado.Text = (dados?.TotalSegregado ?? 0).ToString("N3");
                            lblVolumes.Text = (dados?.Volumes ?? 0).ToString("N0");

                            dgvMaterialFiltrado.DataSource = registros;

                            ConfigurarColunasGridFiltrado();
                        }));
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar resumo: {ex.Message}");
            }
        }

        private void dgvMaterialFiltrado_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvMaterialFiltrado.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                string status = e.Value.ToString();

                if (status == "Aguardando Peso")
                {
                    e.CellStyle.BackColor = Color.FromArgb(177, 2, 2);
                    e.CellStyle.ForeColor = Color.White;
                }
                else if (status == "Segregado")
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 229, 160);
                    e.CellStyle.ForeColor = Color.Black;
                }
                else if (status == "Vendido")
                {
                    e.CellStyle.BackColor = Color.FromArgb(17, 115, 75);
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }

        private void ConfigurarColunasGridFiltrado()
        {
            if (dgvMaterialFiltrado.Columns.Contains("Id"))
                dgvMaterialFiltrado.Columns["Id"].Visible = false;

            if (dgvMaterialFiltrado.Columns.Contains("Material"))
                dgvMaterialFiltrado.Columns["Material"].HeaderText = "MATERIAL";

            if (dgvMaterialFiltrado.Columns.Contains("DataEntrada"))
                dgvMaterialFiltrado.Columns["DataEntrada"].HeaderText = "DATA ENTRADA";

            if (dgvMaterialFiltrado.Columns.Contains("DataSaida"))
                dgvMaterialFiltrado.Columns["DataSaida"].HeaderText = "DATA SAÍDA";

            if (dgvMaterialFiltrado.Columns.Contains("Quantidade"))
                dgvMaterialFiltrado.Columns["Quantidade"].HeaderText = "QUANTIDADE";

            if (dgvMaterialFiltrado.Columns.Contains("Status"))
                dgvMaterialFiltrado.Columns["Status"].HeaderText = "STATUS";

            if (dgvMaterialFiltrado.Columns.Contains("ClienteNome"))
                dgvMaterialFiltrado.Columns["ClienteNome"].HeaderText = "CLIENTE";

            if (dgvMaterialFiltrado.Columns.Contains("Observacao"))
                dgvMaterialFiltrado.Columns["Observacao"].HeaderText = "OBSERVAÇÃO";
        }

        private void CarregarCamposDaLinha(EstoqueViewModel estoque)
        {
            _atualizandoCampos = true;

            if (cmbMaterial.DataSource is List<Material> lista)
            {
                var sel = lista.FirstOrDefault(m =>
                    string.Equals(m.Nome, estoque.Material, StringComparison.OrdinalIgnoreCase));
                cmbMaterial.SelectedItem = sel;
            }
            else
            {
                cmbMaterial.Text = estoque.Material;
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

                            estoque.Material = string.IsNullOrWhiteSpace(item.Material)
                                ? ""
                                : item.Material.Trim();

                            estoque.DataEntrada = item.DataEntrada;
                            estoque.DataSaida = item.DataSaida;
                            estoque.Quantidade = item.EhPeso
                                ? Math.Round(item.Quantidade, 3, MidpointRounding.AwayFromZero)
                                : Math.Truncate(item.Quantidade);
                            estoque.EhPeso = item.EhPeso;
                            estoque.Status = item.Status ?? "Aguardando Peso";
                            estoque.Observacao = item.Observacao ?? "";
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

        private async void btnExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bindingSource == null || _bindingSource.Count == 0)
                {
                    MessageBox.Show("Não há dados para exportar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var resultado = MessageBox.Show(
                    "Deseja exportar apenas os dados FILTRADOS?\n\n" +
                    "SIM = Exportar apenas dados filtrados/visíveis\n" +
                    "NÃO = Exportar TODO o estoque do mês",
                    "Opção de Exportação",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Cancel)
                    return;

                bool exportarApenasFiltrados = (resultado == DialogResult.Yes);

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    string mesNome = cmbMes.SelectedItem?.ToString() ?? "Mes";
                    string ano = cmbAno.SelectedItem?.ToString() ?? "Ano";

                    sfd.Filter = "Arquivo Excel (*.xlsx)|*.xlsx";
                    sfd.Title = "Salvar Exportação de Estoque";
                    sfd.FileName = $"Estoque_{mesNome}_{ano}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    btnExportarExcel.Enabled = false;
                    btnExportarExcel.Text = "Exportando...";
                    Cursor = Cursors.WaitCursor;

                    try
                    {
                        await Task.Run(() => ExportarEstoqueParaExcel(sfd.FileName, exportarApenasFiltrados));

                        MessageBox.Show(
                            $"Dados exportados com sucesso!\n\nArquivo: {Path.GetFileName(sfd.FileName)}",
                            "Exportação Concluída",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        var abrirArquivo = MessageBox.Show(
                            "Deseja abrir o arquivo agora?",
                            "Abrir Arquivo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (abrirArquivo == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao exportar: {ex.Message}", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btnExportarExcel.Enabled = true;
                        btnExportarExcel.Text = "Exportar Excel";
                        Cursor = Cursors.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportarEstoqueParaExcel(string caminhoArquivo, bool apenasFiltrados)
        {
            using (var workbook = new XLWorkbook())
            {
                string mesNome = cmbMes.SelectedItem?.ToString() ?? "Mês";
                string ano = cmbAno.SelectedItem?.ToString() ?? "Ano";

                var worksheet = workbook.Worksheets.Add($"Estoque {mesNome} {ano}");

                DataTable dadosExportar = ObterDadosEstoqueParaExportacao(apenasFiltrados);

                if (dadosExportar.Rows.Count == 0)
                {
                    throw new InvalidOperationException("Nenhum dado disponível para exportação.");
                }

                int colIndex = 1;
                var colunasVisiveis = ObterColunasVisiveisEstoque();

                foreach (var coluna in colunasVisiveis)
                {
                    var cell = worksheet.Cell(1, colIndex);
                    cell.Value = coluna.HeaderText;
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(52, 73, 94);
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    colIndex++;
                }

                int rowIndex = 2;
                foreach (DataRow row in dadosExportar.Rows)
                {
                    colIndex = 1;
                    foreach (var coluna in colunasVisiveis)
                    {
                        var cell = worksheet.Cell(rowIndex, colIndex);
                        var valor = row[coluna.Name];

                        if (valor != null && valor != DBNull.Value)
                        {
                            if (coluna.Name == "Valorizacao")
                            {
                                int val = Convert.ToInt32(valor);
                                if (val >= 1 && val <= 5)
                                {
                                    cell.Value = new string('★', val);
                                    cell.Style.Font.FontSize = 14;
                                    cell.Style.Font.FontColor = XLColor.Gold;
                                }
                                else
                                {
                                    cell.Value = "-";
                                }
                            }
                            else if (coluna.Name == "DataEntrada" || coluna.Name == "DataSaida")
                            {
                                if (DateTime.TryParse(valor.ToString(), out DateTime data))
                                {
                                    cell.Value = data;
                                    cell.Style.DateFormat.Format = "dd/MM/yyyy";
                                }
                            }
                            else if (coluna.Name == "Quantidade")
                            {
                                decimal quantidade = Convert.ToDecimal(valor);
                                bool ehPeso = row["EhPeso"] != DBNull.Value && Convert.ToBoolean(row["EhPeso"]);

                                cell.Value = quantidade;

                                if (ehPeso)
                                {
                                    cell.Style.NumberFormat.Format = "#,##0.000";
                                }
                                else
                                {
                                    cell.Style.NumberFormat.Format = "#,##0";
                                }
                            }
                            else if (coluna.Name == "Status")
                            {
                                string status = valor.ToString();
                                cell.Value = status;
                                cell.Style.Font.Bold = true;

                                switch (status)
                                {
                                    case "Aguardando Peso":
                                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(177, 2, 2);
                                        cell.Style.Font.FontColor = XLColor.White;
                                        break;
                                    case "Segregado":
                                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 229, 160);
                                        cell.Style.Font.FontColor = XLColor.Black;
                                        break;
                                    case "Vendido":
                                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(17, 115, 75);
                                        cell.Style.Font.FontColor = XLColor.White;
                                        break;
                                }
                            }
                            else
                            {
                                cell.Value = valor.ToString();
                            }
                        }

                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.OutsideBorderColor = XLColor.Gray;

                        if (rowIndex % 2 == 0 && coluna.Name != "Status")
                        {
                            if (cell.Style.Fill.BackgroundColor == XLColor.NoColor)
                            {
                                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(245, 249, 255);
                            }
                        }

                        colIndex++;
                    }
                    rowIndex++;
                }

                worksheet.Columns().AdjustToContents(5, 50);

                var range = worksheet.Range(1, 1, rowIndex - 1, colunasVisiveis.Count);
                range.SetAutoFilter();

                worksheet.SheetView.FreezeRows(1);

                decimal totalPeso = 0;
                decimal totalSegregado = 0;
                int totalVolumes = 0;

                foreach (DataRow row in dadosExportar.Rows)
                {
                    if (row["Quantidade"] != DBNull.Value)
                    {
                        decimal qtd = Convert.ToDecimal(row["Quantidade"]);
                        totalPeso += qtd;

                        if (row["Status"]?.ToString() == "Segregado")
                        {
                            totalSegregado += qtd;
                            totalVolumes++;
                        }
                    }
                }

                int footerRow = rowIndex + 2;

                worksheet.Cell(footerRow, 1).Value = "RESUMO DO ESTOQUE";
                worksheet.Cell(footerRow, 1).Style.Font.Bold = true;
                worksheet.Cell(footerRow, 1).Style.Font.FontSize = 12;
                worksheet.Cell(footerRow, 1).Style.Font.FontColor = XLColor.FromArgb(52, 73, 94);

                worksheet.Cell(footerRow + 1, 1).Value = $"Total Geral: {totalPeso:N3}";
                worksheet.Cell(footerRow + 1, 1).Style.Font.Bold = true;

                worksheet.Cell(footerRow + 2, 1).Value = $"Total Segregado: {totalSegregado:N3}";
                worksheet.Cell(footerRow + 2, 1).Style.Font.Bold = true;

                worksheet.Cell(footerRow + 3, 1).Value = $"Volumes Segregados: {totalVolumes}";
                worksheet.Cell(footerRow + 3, 1).Style.Font.Bold = true;

                worksheet.Cell(footerRow + 5, 1).Value = $"Exportado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cell(footerRow + 5, 1).Style.Font.Italic = true;
                worksheet.Cell(footerRow + 5, 1).Style.Font.FontColor = XLColor.Gray;

                worksheet.Cell(footerRow + 6, 1).Value = $"Período: {mesNome}/{ano}";
                worksheet.Cell(footerRow + 6, 1).Style.Font.Italic = true;
                worksheet.Cell(footerRow + 6, 1).Style.Font.FontColor = XLColor.Gray;

                if (apenasFiltrados && !string.IsNullOrEmpty(dgvMaterial.FilterString))
                {
                    worksheet.Cell(footerRow + 7, 1).Value = "Filtros aplicados: Sim";
                    worksheet.Cell(footerRow + 7, 1).Style.Font.Italic = true;
                    worksheet.Cell(footerRow + 7, 1).Style.Font.FontColor = XLColor.Gray;
                }

                worksheet.Cell(footerRow + 8, 1).Value = $"Total de registros: {dadosExportar.Rows.Count}";
                worksheet.Cell(footerRow + 8, 1).Style.Font.Bold = true;

                workbook.SaveAs(caminhoArquivo);
            }
        }

        private DataTable ObterDadosEstoqueParaExportacao(bool apenasFiltrados)
        {
            if (!apenasFiltrados)
            {
                if (_bindingSource.DataSource is DataTable dt)
                {
                    return dt.Copy();
                }
            }

            DataTable resultado = (_bindingSource.DataSource as DataTable)?.Clone();

            if (resultado == null)
                return new DataTable();

            foreach (DataRowView rowView in _bindingSource)
            {
                resultado.ImportRow(rowView.Row);
            }

            return resultado;
        }

        private List<DataGridViewColumn> ObterColunasVisiveisEstoque()
        {
            var colunasVisiveis = new List<DataGridViewColumn>();

            foreach (DataGridViewColumn col in dgvMaterial.Columns)
            {
                if (col.Visible && col.Name != "Id" && col.Name != "EhPeso")
                {
                    colunasVisiveis.Add(col);
                }
            }

            return colunasVisiveis.OrderBy(c => c.DisplayIndex).ToList();
        }

    }
}