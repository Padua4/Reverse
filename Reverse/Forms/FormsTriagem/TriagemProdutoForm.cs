using Reverse.Models;
using Reverse.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms
{
    public partial class TriagemProdutoForm : Form
    {
        public Produto ProdutoAtual { get; set; }
        private bool _isNovo;
        private bool _isSaving;
        private readonly int _usuarioId;

        private class FlagItem
        {
            public FlagType Flag { get; set; }
            public Color Color { get; set; }
            public string Text { get; set; }
            public override string ToString() => Text;
        }

        public TriagemProdutoForm(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            Load += ProdutoForm_Load;

            cmbFlag.DrawMode = DrawMode.OwnerDrawFixed;
            cmbFlag.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFlag.DrawItem += cmbFlag_DrawItem;
            cmbFlag.ItemHeight = 20;
            chkPerecivel.CheckedChanged += chkPerecivel_CheckedChanged;
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect,
        int nTopRect,
        int nRightRect,
        int nBottomRect,
        int nWidthEllipse,
        int nHeightEllipse
        );


        private void ProdutoForm_Load(object sender, EventArgs e)
        {
            if (ProdutoAtual == null)
            {
                _isNovo = true;
                ProdutoAtual = new Produto { Emissao = DateTime.Now };

                using (var ctx = new ReverseContext())
                {
                    var usuario = ctx.Usuarios.Find(_usuarioId);
                    var nomeUsuario = FormatarNomeUsuario(usuario?.UsuarioNome);
                    ProdutoAtual.UsuarioCriacao = nomeUsuario;
                    lblUltimoUsuario.Text = $"Cadastrado por: {nomeUsuario ?? "Desconhecido"}";
                }
            }
            else
            {
                _isNovo = false;
                txtCodigoBarras.Enabled = true;

                var nomeExibicao = FormatarNomeUsuario(
                    ProdutoAtual.UsuarioUltimaAlteracao ??
                    ProdutoAtual.UsuarioCriacao
                ) ?? "Desconhecido";

                lblUltimoUsuario.Text = $"Última alteração por: {nomeExibicao}";
            }

            ConfigurarControles();
            CarregarFlags();
            ConfigurarDataBindings();

            TogglePerecivelControls();
        }

        private string FormatarNomeUsuario(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return null;

            nome = nome.Trim().ToLower();

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nome);
        }

        private void ConfigurarControles()
        {
            nudValor.DecimalPlaces = 2;
            nudValor.Increment = 0.01m;
            nudValor.Minimum = 0m;
            nudValor.Maximum = 1_000_000m;
            nudValor.ThousandsSeparator = true;
        }

        private void CarregarFlags()
        {
            var flags = new List<FlagItem>
            {
                new FlagItem { Flag = FlagType.MercadoLivre, Color = Color.Gold, Text = "Mercado Livre" },
                new FlagItem { Flag = FlagType.Amazon, Color = Color.DodgerBlue, Text = "Amazon" },
                new FlagItem { Flag = FlagType.Variados, Color = Color.LightGray, Text = "Variados" }
            };

            cmbFlag.DataSource = flags;
            cmbFlag.ValueMember = nameof(FlagItem.Flag);
            cmbFlag.DisplayMember = nameof(FlagItem.Text);
            cmbFlag.SelectedValue = ProdutoAtual.Flag;
        }

        private void ConfigurarDataBindings()
        {
            if (!_isNovo && !string.IsNullOrWhiteSpace(ProdutoAtual.CodigoBarras)
                && ProdutoAtual.CodigoBarras.StartsWith("MDL-"))
            {
                txtCodigoBarras.Text = string.Empty;
                chkSemCodigoBarras.Checked = true;
                txtCodigoBarras.Enabled = false;
            }
            else
            {
                txtCodigoBarras.Text = ProdutoAtual.CodigoBarras ?? string.Empty;
            }

            txtDescricao.DataBindings.Add("Text", ProdutoAtual,
                nameof(Produto.Descricao), true, DataSourceUpdateMode.OnPropertyChanged);
            nudValor.DataBindings.Add("Value", ProdutoAtual,
                nameof(Produto.ValorUnitario), true, DataSourceUpdateMode.OnPropertyChanged);
            cmbFlag.DataBindings.Add("SelectedValue", ProdutoAtual,
                nameof(Produto.Flag), true, DataSourceUpdateMode.OnPropertyChanged);
            chkPerecivel.DataBindings.Add("Checked", ProdutoAtual,
                nameof(Produto.Perecivel), true, DataSourceUpdateMode.OnPropertyChanged);
            dtpDataValidade.DataBindings.Add("Value", ProdutoAtual,
                nameof(Produto.DataValidade), true, DataSourceUpdateMode.OnPropertyChanged);

            chkPerecivel.Checked = ProdutoAtual.Perecivel;
            dtpDataValidade.Value = ProdutoAtual.DataValidade ?? DateTime.Today;
            nudValor.Value = ProdutoAtual.ValorUnitario;
        }


        private void chkPerecivel_CheckedChanged(object sender, EventArgs e)
        {
            TogglePerecivelControls();
        }

        private void TogglePerecivelControls()
        {
            bool ativo = chkPerecivel.Checked;
            lblDataValidade.Visible = ativo;
            dtpDataValidade.Visible = ativo;
        }

        private void cmbFlag_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0) return;

            var item = (FlagItem)cmbFlag.Items[e.Index];
            int diameter = 12;
            var circleRect = new Rectangle(e.Bounds.Left + 2, e.Bounds.Top + (e.Bounds.Height - diameter) / 2, diameter, diameter);

            using (var brush = new SolidBrush(item.Color))
                e.Graphics.FillEllipse(brush, circleRect);

            var textPos = new Point(circleRect.Right + 6, e.Bounds.Top + (e.Bounds.Height - e.Font.Height) / 2);
            TextRenderer.DrawText(e.Graphics, item.Text, e.Font, textPos, e.ForeColor);

            e.DrawFocusRectangle();
        }

        private DateTime AjustarData(DateTime data)
        {
            DateTime minimo = new DateTime(1753, 1, 1);
            if (data < minimo)
                return minimo;

            return data;
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            if (_isSaving) return;

            _isSaving = true;
            btnSalvar.Enabled = false;
            btnCancelar.Enabled = false;

            try
            {
                if (cmbFlag.SelectedValue == null)
                {
                    MessageBox.Show("Selecione uma flag válida.", "Atenção",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string codigoBarras = await ValidarCodigoBarrasAsync();
                if (codigoBarras == null && !chkSemCodigoBarras.Checked)
                    return;

                if (string.IsNullOrWhiteSpace(txtDescricao.Text.Trim()))
                {
                    MessageBox.Show("Informe a descrição do produto.", "Atenção",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescricao.Focus();
                    return;
                }

                if (nudValor.Value <= 0)
                {
                    MessageBox.Show("O valor deve ser maior que zero.", "Atenção",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    nudValor.Focus();
                    return;
                }

                if (!ValidarPerecivelEDataValidade())
                    return;

                PrepararProdutoParaSalvar(codigoBarras);

                string nomeUsuario = await ObterNomeUsuarioAsync();
                ProdutoAtual.UsuarioUltimaAlteracao = nomeUsuario;

                if (_isNovo && string.IsNullOrWhiteSpace(ProdutoAtual.UsuarioCriacao))
                    ProdutoAtual.UsuarioCriacao = nomeUsuario;

                AjustarDatasSqlServer();

                bool sucesso = _isNovo
                    ? await ProdutoService.CreateAsync(ProdutoAtual)
                    : await ProdutoService.UpdateAsync(ProdutoAtual);

                if (sucesso)
                {
                    string mensagem = _isNovo ? "cadastrado" : "atualizado";
                    MessageBox.Show($"Produto {mensagem} com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    string mensagem = _isNovo
                        ? "Erro ao cadastrar produto. Verifique os dados e tente novamente."
                        : "Produto não encontrado. Pode ter sido excluído por outro usuário.";
                    MessageBox.Show(mensagem, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (ConcurrencyException)
            {
                MessageBox.Show("Outro usuário alterou este produto enquanto você editava.",
                    "Conflito", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                var detalhe = ex;
                while (detalhe.InnerException != null)
                    detalhe = detalhe.InnerException;

                MessageBox.Show($"Erro: {detalhe.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isSaving = false;
                btnSalvar.Enabled = true;
                btnCancelar.Enabled = true;
            }
        }

        private async Task<string> ValidarCodigoBarrasAsync()
        {
            if (chkSemCodigoBarras.Checked)
            {
                try
                {
                    string codigoGerado = await GerarCodigoMDLAsync();
                    return codigoGerado;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao gerar código MDL: {ex.Message}",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }

            // Validação de código de barras normal
            string cb = txtCodigoBarras.Text.Trim();

            if (string.IsNullOrWhiteSpace(cb))
            {
                MessageBox.Show("Informe o Código de Barras ou marque 'Sem Código de Barras'.",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigoBarras.Focus();
                return null;
            }

            cb = new string(cb.Where(c => !char.IsWhiteSpace(c)).ToArray());

            if (cb.Length < 3 || cb.Length > 20 || !cb.All(char.IsDigit))
            {
                MessageBox.Show("Código de barras inválido. Use apenas dígitos (3 a 20).",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigoBarras.Focus();
                return null;
            }

            // Verificar duplicatas
            if (_isNovo || cb != ProdutoAtual.CodigoBarras)
            {
                using (var ctx = new ReverseContext())
                {
                    bool existe = await ctx.Produtos
                        .AsNoTracking()
                        .AnyAsync(p => p.CodigoBarras == cb && p.Id != ProdutoAtual.Id);

                    if (existe)
                    {
                        MessageBox.Show($"Já existe um produto com o código de barras '{cb}'.",
                            "Código Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCodigoBarras.Focus();
                        return null;
                    }
                }
            }

            return cb;
        }

        private async Task<string> GerarCodigoMDLAsync()
        {
            using (var ctx = new ReverseContext())
            {
                // Para produtos existentes que já são MDL, manter o código
                if (!_isNovo && !string.IsNullOrWhiteSpace(ProdutoAtual.CodigoBarras)
                    && ProdutoAtual.CodigoBarras.StartsWith("MDL-"))
                {
                    return ProdutoAtual.CodigoBarras;
                }

                try
                {
                    // Buscar todos os códigos MDL existentes
                    var codigosMDL = await ctx.Produtos
                        .Where(p => p.CodigoBarras != null && p.CodigoBarras.StartsWith("MDL-"))
                        .Select(p => p.CodigoBarras)
                        .ToListAsync();

                    int proximoNumero = 1;

                    if (codigosMDL.Any())
                    {
                        var numeros = new List<int>();
                        foreach (var codigo in codigosMDL)
                        {
                            if (codigo.Length > 4) // MDL-XXX
                            {
                                string numeroStr = codigo.Substring(4);
                                if (int.TryParse(numeroStr, out int numero))
                                {
                                    numeros.Add(numero);
                                }
                            }
                        }

                        if (numeros.Any())
                        {
                            proximoNumero = numeros.Max() + 1;
                        }
                    }

                    // Formatar com 3 dígitos
                    return $"MDL-{proximoNumero:D3}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro crítico ao gerar código MDL: {ex.Message}\n\nDetalhes: {ex.InnerException?.Message}",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }
        }

        private bool ValidarPerecivelEDataValidade()
        {
            if (!chkPerecivel.Checked)
                return true;

            DateTime venc = AjustarData(dtpDataValidade.Value.Date);

            if (venc < DateTime.Today)
            {
                MessageBox.Show("Não é possível cadastrar um produto já vencido.",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpDataValidade.Focus();
                return false;
            }

            return true;
        }

        private void PrepararProdutoParaSalvar(string codigoBarras)
        {
            ProdutoAtual.CodigoBarras = codigoBarras;
            ProdutoAtual.Descricao = txtDescricao.Text.Trim();
            ProdutoAtual.ValorUnitario = nudValor.Value;
            ProdutoAtual.DataUltimaAlteracao = AjustarData(DateTime.Now);
            ProdutoAtual.Flag = (FlagType)cmbFlag.SelectedValue;

            if (chkPerecivel.Checked)
            {
                ProdutoAtual.Perecivel = true;
                ProdutoAtual.DataValidade = AjustarData(dtpDataValidade.Value.Date);
            }
            else
            {
                ProdutoAtual.Perecivel = false;
                ProdutoAtual.DataValidade = null;
            }
        }

        private async Task<string> ObterNomeUsuarioAsync()
        {
            using (var ctx = new ReverseContext())
            {
                var usuario = await ctx.Usuarios.FindAsync(_usuarioId);
                return FormatarNomeUsuario(usuario?.UsuarioNome);
            }
        }

        private void AjustarDatasSqlServer()
        {
            DateTime minSqlDate = new DateTime(1753, 1, 1);

            if (ProdutoAtual.Emissao < minSqlDate)
                ProdutoAtual.Emissao = DateTime.Now;

            if (ProdutoAtual.DataUltimaAlteracao < minSqlDate)
                ProdutoAtual.DataUltimaAlteracao = DateTime.Now;

            if (ProdutoAtual.DataValidade.HasValue && ProdutoAtual.DataValidade.Value < minSqlDate)
                ProdutoAtual.DataValidade = null;
        }

        private void chkSemCodigoBarras_CheckedChanged(object sender, EventArgs e)
        {
            txtCodigoBarras.Enabled = !chkSemCodigoBarras.Checked;

            if (chkSemCodigoBarras.Checked)
            {
                txtCodigoBarras.Clear();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}