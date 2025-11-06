using Reverse.Models;
using Reverse.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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
                    ProdutoAtual.UsuarioCriacao = usuario?.UsuarioNome;
                    lblUltimoUsuario.Text = $"Cadastrado por: {usuario?.UsuarioNome ?? "Desconhecido"}";
                }
            }
            else
            {
                _isNovo = false;
                txtCodigoBarras.Enabled = true;

                lblUltimoUsuario.Text = $"Última alteração por: {ProdutoAtual.UsuarioUltimaAlteracao ?? ProdutoAtual.UsuarioCriacao ?? "Desconhecido"}";
            }


            nudValor.DecimalPlaces = 2;
            nudValor.Increment = 0.01m;
            nudValor.Minimum = 0m;
            nudValor.Maximum = 1_000_000m;
            nudValor.ThousandsSeparator = true;

            var flags = new List<FlagItem>
    {
        new FlagItem { Flag = FlagType.MercadoLivre, Color = Color.Gold, Text = "Mercado Livre" },
        new FlagItem { Flag = FlagType.Amazon,       Color = Color.DodgerBlue, Text = "Amazon" },
        new FlagItem { Flag = FlagType.Variados,     Color = Color.LightGray, Text = "Variados" }
    };
            cmbFlag.DataSource = flags;
            cmbFlag.ValueMember = nameof(FlagItem.Flag);
            cmbFlag.DisplayMember = nameof(FlagItem.Text);

            txtCodigoBarras.DataBindings.Add("Text", ProdutoAtual, nameof(Produto.CodigoBarras), true, DataSourceUpdateMode.OnPropertyChanged);
            txtDescricao.DataBindings.Add("Text", ProdutoAtual, nameof(Produto.Descricao), true, DataSourceUpdateMode.OnPropertyChanged);
            nudValor.DataBindings.Add("Value", ProdutoAtual, nameof(Produto.ValorUnitario), true, DataSourceUpdateMode.OnPropertyChanged);
            cmbFlag.DataBindings.Add("SelectedValue", ProdutoAtual, nameof(Produto.Flag), true, DataSourceUpdateMode.OnPropertyChanged);
            chkPerecivel.DataBindings.Add("Checked", ProdutoAtual, nameof(Produto.Perecivel), true, DataSourceUpdateMode.OnPropertyChanged);
            dtpDataValidade.DataBindings.Add("Value", ProdutoAtual, nameof(Produto.DataValidade), true, DataSourceUpdateMode.OnPropertyChanged);

            chkPerecivel.Checked = ProdutoAtual.Perecivel;
            dtpDataValidade.Value = ProdutoAtual.DataValidade ?? DateTime.Today;

            TogglePerecivelControls();

            cmbFlag.SelectedValue = ProdutoAtual.Flag;
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

            try
            {
                if (cmbFlag.SelectedValue == null)
                {
                    MessageBox.Show("Selecione uma flag válida.", "Atenção",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string cb = txtCodigoBarras.Text.Trim();

                // 🔹 Se for produto modelo (sem código de barras)
                if (chkSemCodigoBarras.Checked)
                {
                    cb = null; // não exige código
                }
                else
                {
                    // 🔹 Só valida se não for modelo
                    if (string.IsNullOrWhiteSpace(cb))
                    {
                        MessageBox.Show("Informe o Código de Barras ou marque 'Sem código de barras'.", "Atenção",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (cb.Length < 3 || cb.Length > 20 || !cb.All(char.IsDigit))
                    {
                        MessageBox.Show("Código de barras inválido. Use apenas dígitos (3 a 20).",
                            "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCodigoBarras.Focus();
                        return;
                    }
                }

                if (nudValor.Value <= 0)
                {
                    MessageBox.Show("O valor deve ser maior que zero.", "Atenção",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (chkPerecivel.Checked)
                {
                    DateTime venc = AjustarData(dtpDataValidade.Value.Date);

                    if (venc < DateTime.Today)
                    {
                        MessageBox.Show("Não é possível cadastrar um produto já vencido.", "Atenção",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    ProdutoAtual.Perecivel = true;
                    ProdutoAtual.DataValidade = venc;
                }
                else
                {
                    ProdutoAtual.Perecivel = false;
                    ProdutoAtual.DataValidade = null;
                }

                ProdutoAtual.CodigoBarras = cb;
                ProdutoAtual.Descricao = txtDescricao.Text.Trim();
                ProdutoAtual.ValorUnitario = nudValor.Value;
                ProdutoAtual.DataUltimaAlteracao = AjustarData(DateTime.Now);
                ProdutoAtual.Flag = (FlagType)cmbFlag.SelectedValue;

                using (var ctx = new ReverseContext())
                {
                    var usuario = ctx.Usuarios.Find(_usuarioId);
                    var nomeUsuario = usuario?.UsuarioNome;

                    ProdutoAtual.UsuarioUltimaAlteracao = nomeUsuario;

                    if (_isNovo && string.IsNullOrWhiteSpace(ProdutoAtual.UsuarioCriacao))
                        ProdutoAtual.UsuarioCriacao = nomeUsuario;
                }

                DateTime minSqlDate = new DateTime(1753, 1, 1);
                if (ProdutoAtual.Emissao < minSqlDate)
                    ProdutoAtual.Emissao = DateTime.Now;
                if (ProdutoAtual.DataUltimaAlteracao < minSqlDate)
                    ProdutoAtual.DataUltimaAlteracao = DateTime.Now;
                if (ProdutoAtual.DataValidade.HasValue && ProdutoAtual.DataValidade.Value < minSqlDate)
                    ProdutoAtual.DataValidade = null;

                bool sucesso;

                if (_isNovo)
                {
                    sucesso = await ProdutoService.CreateAsync(ProdutoAtual);
                    if (sucesso)
                    {
                        MessageBox.Show("Produto cadastrado com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DialogResult = DialogResult.OK;
                        Close();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Já existe um produto com esse código de barras.",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    sucesso = await ProdutoService.UpdateAsync(ProdutoAtual);
                    if (sucesso)
                    {
                        MessageBox.Show("Produto atualizado com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Produto não encontrado. Pode ter sido excluído por outro usuário.",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
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
            }
        }


        private void chkSemCodigo_CheckedChanged(object sender, EventArgs e)
        {
            bool semCodigo = chkSemCodigoBarras.Checked;
            txtCodigoBarras.Enabled = !semCodigo;
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
