using Reverse.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace Reverse.Forms.FormsExpedicao
{
    public partial class ExpedicaoFormEstoque : Form
    {
        private BindingList<EstoqueViewModel> _estoqueList;
        private EstoqueViewModel _linhaAtual = null;
        private bool _atualizandoCampos = false;
        private readonly int _usuarioId;

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

            cmbStatus.Items.AddRange(new string[] { "Aguardando Peso", "Segregado", "Vendido" });

            using (var ctx = new ReverseContext())
            {
                var clientes = ctx.Clientes.OrderBy(c => c.Nome).ToList();
                cmbCliente.DataSource = clientes;
                cmbCliente.DisplayMember = "Nome";
                cmbCliente.ValueMember = "ClienteId";
            }

            cmbMaterial.SelectedIndexChanged += (s, ev) => AtualizarLinhaAtual();
            txtQuantidade.TextChanged += (s, ev) => AtualizarLinhaAtual();
            cmbStatus.SelectedIndexChanged += (s, ev) => AtualizarLinhaAtual();
            cmbCliente.SelectedIndexChanged += (s, ev) => AtualizarLinhaAtual();
            txtObs.TextChanged += (s, ev) => AtualizarLinhaAtual();

            ConfigurarGrid();

            rbmQuantidade.Checked = true;
            lblPesoQuantidade.Text = "Quantidade";
            txtQuantidade.KeyPress += TxtQuantidade_KeyPressInteiro;
            rbmQuantidade.CheckedChanged += rbmQuantidade_CheckedChanged;
            rbmPeso.CheckedChanged += rbmPeso_CheckedChanged;
            txtQuantidade.Leave += txtQuantidade_Leave;

            this.cmbMes.SelectedIndexChanged += new System.EventHandler(this.cmbMes_SelectedIndexChanged);
            this.cmbAno.SelectedIndexChanged += new System.EventHandler(this.cmbAno_SelectedIndexChanged);

            CarregarEstoque();
            LimparCampos();
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

                    _estoqueList.Clear();

                    foreach (var item in dados)
                    {
                        string clienteNome = "";
                        if (item.ClienteId > 0)
                        {
                            var cliente = ctx.Clientes.FirstOrDefault(c => c.ClienteId == item.ClienteId);
                            clienteNome = cliente?.Nome ?? "";
                        }

                        int? valorizacao = null;
                        var mat = ctx.Materiais.FirstOrDefault(m => m.Nome == item.Material);
                        if (mat != null)
                            valorizacao = mat.Valorizacao;

                        _estoqueList.Add(new EstoqueViewModel
                        {
                            Id = item.Id,
                            Material = item.Material,
                            Valorizacao = valorizacao ?? 1,
                            DataEntrada = item.DataEntrada,
                            DataSaida = item.DataSaida,
                            Quantidade = item.Quantidade,
                            Status = item.Status,
                            ClienteNome = clienteNome,
                            Observacao = item.Observacao,
                            EhPeso = item.EhPeso
                        });
                    }

                    dgvMaterial.DataSource = null;
                    dgvMaterial.DataSource = _estoqueList;
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
            if (_linhaAtual == null) return;

            if (decimal.TryParse(txtQuantidade.Text,
                                 NumberStyles.Number,
                                 CultureInfo.CurrentCulture,
                                 out decimal valor))
            {
                if (_linhaAtual.EhPeso)
                {
                    valor = Math.Round(valor, 3, MidpointRounding.AwayFromZero);
                    txtQuantidade.Text = valor.ToString("N3");
                }
                else
                {
                    valor = Math.Truncate(valor);
                    txtQuantidade.Text = valor.ToString("N0");
                }

                _linhaAtual.Quantidade = valor;
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

            // Selecionar a nova linha
            dgvMaterial.ClearSelection();
            int novoIndex = dgvMaterial.Rows.Count - 1;
            dgvMaterial.Rows[novoIndex].Selected = true;
            dgvMaterial.CurrentCell = dgvMaterial.Rows[novoIndex].Cells[0];

            // Carregar nos campos
            _linhaAtual = novaLinha;
            CarregarCamposDaLinha(novaLinha);
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

            var estoque = dgvMaterial.CurrentRow.DataBoundItem as EstoqueViewModel;
            if (estoque == null) return;

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

            if (estoque.EhPeso)
                txtQuantidade.Text = estoque.Quantidade.ToString("N3");
            else
                txtQuantidade.Text = estoque.Quantidade.ToString("N0");

            cmbStatus.SelectedItem = estoque.Status;

            if (!string.IsNullOrWhiteSpace(estoque.ClienteNome))
                cmbCliente.Text = estoque.ClienteNome;
            else
                cmbCliente.SelectedIndex = -1;

            txtObs.Text = estoque.Observacao;

            _atualizandoCampos = false;
        }
        private void AtualizarLinhaAtual()
        {
            if (_linhaAtual == null || _atualizandoCampos) return;

            _linhaAtual.EhPeso = rbmPeso.Checked;

            try
            {
                if (cmbMaterial.SelectedItem is Material mat)
                    _linhaAtual.Material = mat.Nome;
                else
                    _linhaAtual.Material = cmbMaterial.Text;

                decimal qtd;
                if (decimal.TryParse(txtQuantidade.Text,
                                     NumberStyles.Number,
                                     CultureInfo.CurrentCulture,
                                     out qtd))
                {
                    if (_linhaAtual.EhPeso)
                    {
                        qtd = Math.Round(qtd, 3, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        qtd = Math.Truncate(qtd);
                    }

                    _linhaAtual.Quantidade = qtd;
                }

                _linhaAtual.Status = cmbStatus.SelectedItem?.ToString() ?? "";
                _linhaAtual.ClienteNome = cmbCliente.Text;
                _linhaAtual.Observacao = txtObs.Text;

                if (_linhaAtual.Status == "Vendido")
                {
                    if (_linhaAtual.DataSaida == null)
                        _linhaAtual.DataSaida = DateTime.Now;
                }
                else
                {
                    _linhaAtual.DataSaida = null;
                }

                dgvMaterial.Refresh();
            }
            catch { }
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
                {
                    int mes = cmbMes.SelectedIndex + 1;
                    int ano = (int)cmbAno.SelectedItem;

                    foreach (var item in _estoqueList)
                    {
                        if (string.IsNullOrWhiteSpace(item.Material))
                            continue;

                        Estoque estoque;
                        if (item.Id > 0)
                        {
                            estoque = ctx.Estoques.FirstOrDefault(x => x.Id == item.Id);
                            if (estoque == null) continue;
                        }
                        else
                        {
                            estoque = new Estoque();
                            ctx.Estoques.Add(estoque);
                        }

                        estoque.Material = item.Material;
                        estoque.DataEntrada = item.DataEntrada;
                        estoque.DataSaida = item.DataSaida;

                        if (item.EhPeso)
                            estoque.Quantidade = Math.Round(item.Quantidade, 3, MidpointRounding.AwayFromZero);
                        else
                            estoque.Quantidade = Math.Truncate(item.Quantidade);

                        estoque.EhPeso = item.EhPeso;
                        estoque.Status = item.Status ?? "Aguardando Peso";
                        estoque.Observacao = item.Observacao;
                        estoque.Mes = mes;
                        estoque.Ano = ano;

                        if (!string.IsNullOrWhiteSpace(item.ClienteNome))
                        {
                            var cliente = ctx.Clientes.FirstOrDefault(c => c.Nome == item.ClienteNome);
                            estoque.ClienteId = cliente?.ClienteId ?? 0;
                        }
                        else
                        {
                            estoque.ClienteId = 0;
                        }
                    }

                    ctx.SaveChanges();
                }

                MessageBox.Show("Estoque salvo com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CarregarEstoque();
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
            var formMaterial = new ExpedicaoFormMaterialEstoque(_usuarioId);
            formMaterial.ShowDialog();

            var materialAtual = _linhaAtual?.Material;

            using (var ctx = new ReverseContext())
            {
                var materiais = ctx.Materiais.OrderBy(m => m.Nome).ToList();
                cmbMaterial.DataSource = materiais;
                cmbMaterial.DisplayMember = "Nome";
                cmbMaterial.ValueMember = "Id";
            }

            if (!string.IsNullOrEmpty(materialAtual))
            {
                var lista = cmbMaterial.DataSource as List<Material>;
                var sel = lista?.FirstOrDefault(m => m.Nome == materialAtual);
                if (sel != null)
                    cmbMaterial.SelectedItem = sel;
            }
        }
    }
}