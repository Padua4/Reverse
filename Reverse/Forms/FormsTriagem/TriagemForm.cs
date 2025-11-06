using ClosedXML.Excel;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using Reverse.Forms;
using Reverse.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse
{
    public partial class TriagemForm : Form
    {
        private ReverseContext _ctx;
        private readonly int _usuarioId;
        private readonly BindingSource bsProdutos = new BindingSource();
        private readonly System.Windows.Forms.Timer _debounceTimer =
        new System.Windows.Forms.Timer { Interval = 200 };
        private bool _adicionandoItem = false;
        private Palete _paleteAtual;

        public TriagemForm(int usuarioId)
        {
            InitializeComponent();
            LoadProdutos();
            _usuarioId = usuarioId;

            btnAdicionarItem.UseVisualStyleBackColor = false;
            btnRemoverItem.UseVisualStyleBackColor = false;
            btnAtualizarItem.UseVisualStyleBackColor = false;

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;

            Load += TriagemForm_Load;
        }

        private void TriagemForm_Load(object sender, EventArgs e)
        {
            _ctx = new ReverseContext();

            bool aguardandoAtualizacao = false;
            dgvProdutos.AutoGenerateColumns = false;
            dgvItensPalete.AutoGenerateColumns = false;

            if (this.TopLevel)
                this.Bounds = Screen.FromControl(this).WorkingArea;

            FormatGridItensPalete();
            dgvItensPalete.EditingControlShowing += dgvItensPalete_EditingControlShowing;
            FormatGridProdutos();

            dgvProdutos.CellDoubleClick += DgvProdutos_CellDoubleClick;

            dgvProdutos.AutoGenerateColumns = false;
            dgvItensPalete.AutoGenerateColumns = false;
            dgvProdutos.AllowUserToAddRows = false;
            dgvProdutos.AllowUserToDeleteRows = false;
            dgvProdutos.ReadOnly = true;
            dgvProdutos.EditMode = DataGridViewEditMode.EditProgrammatically;
            txtBusca.TextChanged += (s, e2) =>
            {
                _debounceTimer.Stop();
                _debounceTimer.Start();
            };
            txtBusca.KeyDown += async (s, eArgs) =>
            {
                if (eArgs.KeyCode == Keys.Enter && !aguardandoAtualizacao)
                {
                    eArgs.SuppressKeyPress = true;
                    eArgs.Handled = true;
                    aguardandoAtualizacao = true;

                    var gridAtualizada = await EsperarAtualizacaoGridAsync(TimeSpan.FromMilliseconds(2500));

                    if (gridAtualizada && dgvProdutos.Rows.Count > 0)
                        dgvProdutos.Rows[0].Selected = true;

                    await AddItemToPaleteAsync();
                    aguardandoAtualizacao = false;
                }
            };
            _debounceTimer.Tick += (s, e) =>
            {
                _debounceTimer.Stop();
                var termo = txtBusca.Text.Trim();
                LoadProdutos(termo);
                if (dgvProdutos.Rows.Count > 0)
                    dgvProdutos.Rows[0].Selected = true;
            };

            LoadItensDaPalete();
        }

        private Task<bool> EsperarAtualizacaoGridAsync(TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<bool>();

            void handler(object s, EventArgs e)
            {
                dgvProdutos.DataBindingComplete -= handler;
                tcs.TrySetResult(true);
            }

            dgvProdutos.DataBindingComplete += handler;

            var timer = new System.Windows.Forms.Timer { Interval = (int)timeout.TotalMilliseconds };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                dgvProdutos.DataBindingComplete -= handler;
                tcs.TrySetResult(false);
            };
            timer.Start();

            return tcs.Task;
        }


        private async void btnCriarPalete_Click(object sender, EventArgs e)
        {
            using var dlg = new TriagemPaleteDialog();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            int ultimoNumero = await _ctx.Paletes
                                         .Select(p => p.Numero)
                                         .DefaultIfEmpty(0)
                                         .MaxAsync();

            var usuario = _ctx.Usuarios.Find(_usuarioId);

            var novaPalete = new Palete
            {
                Numero = ultimoNumero + 1,
                Categoria = dlg.CategoriaSelecionada,
                DataCriacao = DateTime.Now,
                UsuarioCriacao = usuario?.UsuarioNome,
                Status = 0
            };

            try
            {
                _ctx.Paletes.Add(novaPalete);
                await _ctx.SaveChangesAsync();

                MessageBox.Show("Palete criada com sucesso",
                                "Sucesso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                LoadItensDaPalete();
                AtualizarLabelPaleteAtual();
                AtualizarEstadoBotoes();
            }
            catch
            {
                MessageBox.Show("Erro ao tentar criar a palete",
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btnNovoItem_Click(object sender, EventArgs e)
        {
            using var form = new TriagemProdutoForm(_usuarioId);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            LoadProdutos(txtBusca.Text);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            LoadProdutos(txtBusca.Text);
            if (dgvProdutos.Rows.Count > 0)
                dgvProdutos.Rows[0].Selected = true;
        }

        private async void btnEditarItem_Click(object sender, EventArgs e)
        {
            if (dgvProdutos.CurrentRow?.DataBoundItem is not Produto produtoSelecionado)
            {
                MessageBox.Show("Selecione um produto válido para editar.",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var form = new TriagemProdutoForm(_usuarioId))
            {
                form.ProdutoAtual = produtoSelecionado;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    await SincronizarPrecosDoProdutoAsync(produtoSelecionado.CodigoBarras);
                    RefreshGrids();
                }
            }
        }

        private async Task SincronizarPrecosDoProdutoAsync(string codigoBarras)
        {
            if (string.IsNullOrWhiteSpace(codigoBarras)) return;

            using (var ctx = new ReverseContext())
            {
                var produto = await ctx.Produtos
                    .FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras);

                if (produto == null || produto.ValorUnitario <= 0) return;

                var itens = await ctx.ItensPalete
                    .Where(i => i.ProdutoId == produto.Id)
                    .ToListAsync();

                foreach (var it in itens)
                    it.ValorUnitario = produto.ValorUnitario;

                await ctx.SaveChangesAsync();
            }
        }
        private void RefreshGrids()
        {
            LoadProdutos(txtBusca.Text);
            LoadItensDaPalete();
        }


        private void btnExportarPDF_Click(object sender, EventArgs e)
        {
            if (_paleteAtual == null)
            {
                MessageBox.Show("Nenhuma palete selecionada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int paleteId = _paleteAtual.Id;

            using (var ctx = new ReverseContext())
            {
                var palete = ctx.Paletes
                    .Include(p => p.Itens.Select(i => i.Produto))
                    .AsNoTracking()
                    .FirstOrDefault(p => p.Id == paleteId);

                if (palete == null)
                    return;

                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePdf = Path.Combine(desktop, palete.Nome + ".pdf");

                var document = new PdfDocument();
                document.Info.Title = palete.Nome;
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                GlobalFontSettings.FontResolver = new SystemFontResolver();

                var fontTitle = new XFont("Segoe UI", 14, XFontStyleEx.Bold);
                var fontHead = new XFont("Segoe UI", 10, XFontStyleEx.Bold);
                var fontBody = new XFont("Segoe UI", 9, XFontStyleEx.Regular);

                var tf = new XTextFormatter(gfx);

                double margin = 40;
                double yPos = margin;
                double pageWidth = page.Width.Point;
                double availW = pageWidth - 2 * margin;

                double padH = 3;
                double padV = 2;

                double baseRowH = Math.Ceiling(gfx.MeasureString("Ay", fontBody).Height + (2 * padV));
                double headerH = Math.Ceiling(gfx.MeasureString("AY", fontHead).Height + (2 * padV));

                try
                {
                    gfx.DrawString(
                        $"Criado em: {palete.DataCriacao:dd/MM/yyyy}",
                        fontBody,
                        XBrushes.Black,
                        new XPoint(margin, yPos),
                        XStringFormats.TopLeft);
                    yPos += Math.Max(14, gfx.MeasureString("Ay", fontBody).Height + 4);

                    gfx.DrawString(
                        palete.Nome,
                        fontTitle,
                        XBrushes.DarkSlateBlue,
                        new XPoint(pageWidth / 2, yPos),
                        XStringFormats.TopCenter);
                    yPos += 24;

                    gfx.DrawLine(XPens.DarkSlateBlue,
                                 margin, yPos,
                                 pageWidth - margin, yPos);
                    yPos += 8;

                    double codeW = 110;
                    double qtyW = 50;
                    double unitW = 70;
                    double totalW = 70;
                    double descW = availW - (codeW + qtyW + unitW + totalW);

                    double[] widths = { codeW, descW, qtyW, unitW, totalW };
                    double[] xs = new double[widths.Length];
                    xs[0] = margin;
                    for (int i = 1; i < widths.Length; i++)
                        xs[i] = xs[i - 1] + widths[i - 1];

                    gfx.DrawRectangle(XBrushes.LightGray, xs[0], yPos, availW, headerH);

                    string[] headers = { "Codigo", "Descrição", "Qtd", "Valor Unit.", "Total" };
                    XStringFormat[] headFmt =
                    {
            XStringFormats.CenterLeft,
            XStringFormats.CenterLeft,
            XStringFormats.Center,
            XStringFormats.CenterRight,
            XStringFormats.CenterRight
        };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        var rect = new XRect(xs[i] + padH, yPos + padV, widths[i] - 2 * padH, headerH - 2 * padV);
                        gfx.DrawString(headers[i], fontHead, XBrushes.Black, rect, headFmt[i]);
                    }
                    yPos += headerH;

                    foreach (var item in palete.Itens)
                    {
                        string codigo = "-";
                        if (!string.IsNullOrWhiteSpace(item.CodigoBarras))
                        {
                            codigo = new string(item.CodigoBarras
                                                 .Where(char.IsDigit)
                                                 .ToArray());
                        }

                        string desc = item.Produto?.Descricao ?? "-";
                        string qtd = item.Quantidade.ToString();
                        string unit = item.ValorUnitario.ToString("C2");
                        string total = (item.Quantidade * item.ValorUnitario).ToString("C2");

                        var descLines = WrapText(desc, fontBody, gfx, widths[1] - 2 * padH);
                        double lineH = gfx.MeasureString("Ay", fontBody).Height;
                        double descH = (descLines.Count * lineH) + (2 * padV);

                        double rowH = Math.Max(baseRowH, descH);

                        if (yPos + rowH > page.Height.Point - margin)
                        {
                            gfx.Dispose();
                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            tf = new XTextFormatter(gfx);
                            yPos = margin;
                        }

                        gfx.DrawLine(XPens.Gainsboro, xs[0], yPos, xs[0] + availW, yPos);

                        gfx.DrawString(codigo, fontBody, XBrushes.Black,
                            new XRect(xs[0] + padH, yPos, widths[0] - 2 * padH, rowH), XStringFormats.CenterLeft);

                        var descRect = new XRect(xs[1] + padH, yPos + padV, widths[1] - 2 * padH, rowH - 2 * padV);
                        tf.DrawString(string.Join("\n", descLines), fontBody, XBrushes.Black, descRect, XStringFormats.TopLeft);

                        gfx.DrawString(qtd, fontBody, XBrushes.Black,
                            new XRect(xs[2] + padH, yPos, widths[2] - 2 * padH, rowH), XStringFormats.Center);

                        gfx.DrawString(unit, fontBody, XBrushes.Black,
                            new XRect(xs[3] + padH, yPos, widths[3] - 2 * padH, rowH), XStringFormats.CenterRight);

                        gfx.DrawString(total, fontBody, XBrushes.Black,
                            new XRect(xs[4] + padH, yPos, widths[4] - 2 * padH, rowH), XStringFormats.CenterRight);

                        yPos += rowH;
                    }

                    gfx.DrawLine(XPens.Gainsboro, xs[0], yPos, xs[0] + availW, yPos);

                    decimal totalPalete = palete.Itens.Sum(i => i.Quantidade * i.ValorUnitario);
                    int totalItens = palete.Itens.Sum(i => i.Quantidade);

                    yPos += 8;
                    gfx.DrawLine(XPens.DarkSlateBlue, xs[0], yPos, xs[0] + availW, yPos);
                    yPos += 4;

                    var footerRect = new XRect(xs[0], yPos, availW, baseRowH);
                    gfx.DrawString(
                        $"Valor total: {totalPalete:C2}",
                        fontHead,
                        XBrushes.DarkSlateBlue,
                        footerRect,
                        XStringFormats.CenterRight);

                    yPos += 20;
                    var footerRect2 = new XRect(xs[0], yPos, availW, baseRowH);
                    gfx.DrawString(
                        $"Total de itens: {totalItens}",
                        fontHead,
                        XBrushes.DarkSlateBlue,
                        footerRect2,
                        XStringFormats.CenterRight);
                }
                finally
                {
                    gfx.Dispose();
                }

                document.Save(filePdf);
                MessageBox.Show(
                    $"PDF gerado em:\n{filePdf}",
                    "Exportação Concluida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                List<string> WrapText(string text, XFont font, XGraphics g, double maxWidth)
                {
                    if (string.IsNullOrWhiteSpace(text))
                        return new List<string> { "-" };

                    var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var lines = new List<string>();
                    var current = "";

                    foreach (var w in words)
                    {
                        var test = string.IsNullOrEmpty(current) ? w : current + " " + w;
                        var size = g.MeasureString(test, font).Width;
                        if (size <= maxWidth)
                        {
                            current = test;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(current))
                            {
                                var temp = w;
                                while (g.MeasureString(temp, font).Width > maxWidth && temp.Length > 1)
                                {
                                    int cut = temp.Length - 1;
                                    while (cut > 1 && g.MeasureString(temp.Substring(0, cut), font).Width > maxWidth)
                                        cut--;
                                    lines.Add(temp.Substring(0, cut));
                                    temp = temp.Substring(cut);
                                }
                                current = temp;
                            }
                            else
                            {
                                lines.Add(current);
                                current = w;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(current))
                        lines.Add(current);

                    return lines;
                }
            }
        }

        private void AtualizarEstadoBotoes()
        {
            if (_paleteAtual == null)
            {
                btnAdicionarItem.Enabled = false;
                btnRemoverItem.Enabled = false;
                btnAtualizarItem.Enabled = false;
                btnFinalizado.Enabled = false;
                dgvItensPalete.ReadOnly = true;
            }
            else if (_paleteAtual.Status == 2)
            {
                btnAdicionarItem.Enabled = false;
                btnRemoverItem.Enabled = false;
                btnAtualizarItem.Enabled = false;
                btnFinalizado.Enabled = false;
                dgvItensPalete.ReadOnly = true;
            }
            else
            {
                btnAdicionarItem.Enabled = true;
                btnRemoverItem.Enabled = true;
                btnAtualizarItem.Enabled = true;
                btnFinalizado.Enabled = true;
                dgvItensPalete.ReadOnly = false;
            }

            Color corAtivo = Color.White;
            Color corInativo = Color.Gray;

            btnAdicionarItem.ForeColor = btnAdicionarItem.Enabled ? corAtivo : corInativo;
            btnRemoverItem.ForeColor = btnRemoverItem.Enabled ? corAtivo : corInativo;
            btnAtualizarItem.ForeColor = btnAtualizarItem.Enabled ? corAtivo : corInativo;
        }

        private async void btnAdicionarItem_Click(object sender, EventArgs e)
        {
            await AddItemToPaleteAsync();
        }


        private async Task<bool> ProdutoJaExisteNaPaleteAsync(int paleteId, int produtoId)
        {
            using (var ctx = new ReverseContext())
            {
                return await ctx.ItensPalete
                    .AnyAsync(i => i.PaleteId == paleteId && i.ProdutoId == produtoId);
            }
        }

        private async Task AddItemToPaleteAsync()
        {
            // 🔹 Evita chamadas concorrentes
            if (_adicionandoItem) return;
            _adicionandoItem = true;

            try
            {
                // 🔹 Bloqueia se não houver palete ou se já estiver finalizada
                if (_paleteAtual == null || _paleteAtual.Status == 2)
                {
                    MessageBox.Show("Não é possível adicionar itens em uma palete finalizada.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var ctx = new ReverseContext())
                {
                    int paleteId = _paleteAtual.Id;

                    var palete = ctx.Paletes.FirstOrDefault(p => p.Id == paleteId);
                    if (palete != null && palete.Status == 0)
                    {
                        palete.Status = 1;
                        await ctx.SaveChangesAsync();

                        _paleteAtual.Status = 1;
                        AtualizarLabelPaleteAtual();
                        AtualizarEstadoBotoes();
                    }
                }

                if (dgvProdutos.CurrentRow == null) return;

                var produto = dgvProdutos.CurrentRow.DataBoundItem as Produto;
                if (produto == null) return;

                // 🔹 Validações de produto
                if (produto.Flag == FlagType.Importado)
                {
                    MessageBox.Show("Esse produto foi importado recentemente...", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (produto.Perecivel && produto.DataValidade.HasValue)
                {
                    var venc = produto.DataValidade.Value.Date;
                    if (venc < DateTime.Today)
                    {
                        MessageBox.Show($"O produto {produto.Descricao} venceu em {venc:d}.",
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (venc <= DateTime.Today.AddMonths(1))
                    {
                        var result = MessageBox.Show(
                            $"Atenção: {produto.Descricao} vence em {venc:d}.\n\nContinuar?",
                            "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result != DialogResult.Yes) return;
                    }
                }

                if (produto.Flag == FlagType.SemAlteracao)
                {
                    MessageBox.Show(
                        "Esse produto não foi atualizado há mais de um mês!\n" +
                        "Atualize o preço e altere a flag antes de adicioná-lo à palete.",
                        "Produto bloqueado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                int paleteIdAtual = _paleteAtual.Id;
                int produtoId = produto.Id;

                if (await ProdutoJaExisteNaPaleteAsync(paleteIdAtual, produtoId))
                {
                    MessageBox.Show("Produto já cadastrado na palete.\nUse o botão Atualizar...",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadItensDaPalete(produtoId.ToString());
                    return;
                }

                using (var ctx = new ReverseContext())
                {
                    var item = new ItemPalete
                    {
                        PaleteId = paleteIdAtual,
                        ProdutoId = produtoId,
                        CodigoBarras = string.IsNullOrWhiteSpace(produto.CodigoBarras) ? null : produto.CodigoBarras,
                        Quantidade = 1,
                        ValorUnitario = produto.ValorUnitario,
                        Flag = FlagType.SemAlteracao
                    };

                    ctx.ItensPalete.Add(item);
                    await ctx.SaveChangesAsync();
                }

                LoadItensDaPalete(produtoId.ToString());
                txtBusca.Clear();
                txtBusca.Focus();
            }
            finally
            {
                _adicionandoItem = false;
            }
        }

        private async void btnFinalizado_Click(object sender, EventArgs e)
        {
            if (_paleteAtual == null)
            {
                MessageBox.Show("Nenhuma palete selecionada.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "Deseja realmente finalizar a palete?",
                "Confirmação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            using (var ctx = new ReverseContext())
            {
                var palete = ctx.Paletes.FirstOrDefault(p => p.Id == _paleteAtual.Id);
                if (palete != null)
                {
                    palete.Status = 2;
                    var usuario = ctx.Usuarios.Find(_usuarioId);
                    palete.UsuarioFinalizacao = usuario?.UsuarioNome;
                    palete.DataFinalizacao = DateTime.Now;
                    await ctx.SaveChangesAsync();

                    _paleteAtual.Status = 2;
                    _paleteAtual.UsuarioFinalizacao = usuario?.UsuarioNome;
                    _paleteAtual.DataFinalizacao = DateTime.Now;

                    AtualizarLabelPaleteAtual();
                    AtualizarEstadoBotoes();
                }
            }

            MessageBox.Show("Palete finalizada com sucesso!", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            AtualizarLabelPaleteAtual();
        }

        private async void btnRemoverItem_Click(object sender, EventArgs e)
        {
            if (dgvItensPalete.CurrentRow?.DataBoundItem is not ItemPalete itemSelecionado)
            {
                MessageBox.Show("Selecione um item válido para remover.",
                    "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_paleteAtual == null)
            {
                MessageBox.Show("Nenhuma palete selecionada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int paleteId = _paleteAtual.Id;

            var confirmar = MessageBox.Show(
                $"Remover '{itemSelecionado.DescricaoProduto}' da palete?",
                "Confirmar Remoção", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                using (var ctx = new ReverseContext())
                {
                    var entity = await ctx.ItensPalete
                        .FirstOrDefaultAsync(i =>
                            i.PaleteId == paleteId &&
                            i.CodigoBarras == itemSelecionado.CodigoBarras);

                    if (entity == null)
                    {
                        MessageBox.Show("Item já não existe. A lista será atualizada.",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        ctx.ItensPalete.Remove(entity);
                        await ctx.SaveChangesAsync();
                    }
                }
            }
            catch (System.Data.Entity.Core.OptimisticConcurrencyException)
            {
                MessageBox.Show("O item foi alterado/removido por outro usuário. Atualizando a lista.",
                     "Concorrência", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao remover item: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LoadItensDaPalete();
        }

        private void SalvarQuantidadeLinha(int rowIndex)
        {
            if (rowIndex < 0 || dgvItensPalete.Rows[rowIndex].DataBoundItem is not ItemPalete itemSelecionado)
                return;

            using (var ctx = new ReverseContext())
            {
                var itemBanco = ctx.ItensPalete.FirstOrDefault(ip => ip.CodigoBarras == itemSelecionado.CodigoBarras
                                                                  && ip.PaleteId == _paleteAtual.Id);
                if (itemBanco != null)
                {
                    itemBanco.Quantidade = itemSelecionado.Quantidade;
                    ctx.SaveChanges();
                }
            }

            LoadItensDaPalete(itemSelecionado.CodigoBarras);
        }

        private void dgvItensPalete_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox tb
                && dgvItensPalete.CurrentCell?.OwningColumn?.Name == "colItemQtd")
            {
                tb.PreviewKeyDown -= Tb_PreviewKeyDown;
                tb.PreviewKeyDown += Tb_PreviewKeyDown;
            }
        }

        private void Tb_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.IsInputKey = true;
                var tb = sender as TextBox;

                dgvItensPalete.EndEdit(DataGridViewDataErrorContexts.Commit);

                if (dgvItensPalete.CurrentCell != null)
                {
                    SalvarQuantidadeLinha(dgvItensPalete.CurrentCell.RowIndex);
                }

                var ke = new KeyEventArgs(Keys.Enter);
                ke.SuppressKeyPress = true;
            }
        }

        private void btnAtualizarItem_Click(object sender, EventArgs e)
        {
            if (dgvItensPalete.CurrentRow?.DataBoundItem is not ItemPalete itemSelecionado)
            {
                MessageBox.Show("Selecione um item válido para atualizar.",
                    "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvItensPalete.CurrentRow != null)
            {
                SalvarQuantidadeLinha(dgvItensPalete.CurrentRow.Index);
            }

            using (var ctx = new ReverseContext())
            {
                var itemBanco = ctx.ItensPalete
                    .Include(ip => ip.Produto)
                    .FirstOrDefault(ip => ip.Id == itemSelecionado.Id);

                if (itemBanco == null)
                {
                    MessageBox.Show("O item não foi encontrado no banco de dados.",
                         "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                itemBanco.Quantidade = itemSelecionado.Quantidade;

                try
                {
                    ctx.SaveChanges();
                }
                catch (System.Data.Entity.Core.OptimisticConcurrencyException)
                {
                    MessageBox.Show("O item foi alterado por outro usuário. Atualize a lista e tente novamente.",
                            "Concorrência", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            LoadItensDaPalete();
        }

        private void LoadItensDaPalete(string produtoIdParaFocar = null)
        {
            if (_paleteAtual == null) return;

            using (var ctx = new ReverseContext())
            {
                var dados = ctx.ItensPalete
                    .Include(i => i.Produto)
                    .Where(i => i.PaleteId == _paleteAtual.Id)
                    .OrderBy(i => i.Id)
                    .AsNoTracking()
                    .ToList();

                dgvItensPalete.DataSource = dados;

                decimal totalPalete = dados.Sum(i => i.Quantidade * i.ValorUnitario);
                lblTotalPalete.Text = $"Total: {totalPalete:C2}";

                if (!string.IsNullOrEmpty(produtoIdParaFocar))
                {
                    var row = dgvItensPalete.Rows
                        .Cast<DataGridViewRow>()
                        .FirstOrDefault(r => (r.DataBoundItem as ItemPalete)?.ProdutoId.ToString() == produtoIdParaFocar);

                    if (row != null)
                    {
                        row.Selected = true;
                        dgvItensPalete.FirstDisplayedScrollingRowIndex = row.Index;
                    }
                }
            }
        }

        private void LoadProdutos(string filtro = "")
        {
            using var ctx = new ReverseContext();
            var query = ctx.Produtos.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = (System.Data.Entity.Infrastructure.DbQuery<Produto>)query.Where(p =>
                    p.Descricao.Contains(filtro) ||
                    (p.CodigoBarras != null && p.CodigoBarras.Contains(filtro)));
            }

            query = (System.Data.Entity.Infrastructure.DbQuery<Produto>)(chkUltimosPrimeiro.Checked
                ? query.OrderByDescending(p => p.DataUltimaAlteracao)
                : query.OrderBy(p => p.Descricao));

            var lista = query.ToList();

            foreach (var prod in lista)
            {
                if (prod.DataUltimaAlteracao <= DateTime.Today.AddMonths(-1))
                    prod.Flag = FlagType.SemAlteracao;
            }

            dgvProdutos.DataSource = lista;
        }

        private void chkUltimosPrimeiro_CheckedChanged(object sender, EventArgs e)
        {
            LoadProdutos(txtBusca.Text);
        }

        private Color GetFlagColor(FlagType flag) => flag switch
        {
            FlagType.Importado => Color.Black,
            FlagType.MercadoLivre => Color.Gold,
            FlagType.Amazon => Color.DodgerBlue,
            FlagType.Variados => Color.LightGray,
            FlagType.SemAlteracao => Color.White,
            _ => Color.Transparent
        };

        private void FormatGridProdutos()
        {
            dgvProdutos.AutoGenerateColumns = false;
            dgvProdutos.Columns.Clear();

            dgvProdutos.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dgvProdutos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvProdutos.DefaultCellStyle.ForeColor = Color.Black;

            dgvProdutos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CodigoBarras",
                HeaderText = "Codigo de Barras",
                Name = "colProdCodigoBarras",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvProdutos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Descricao",
                HeaderText = "Descrição",
                Name = "colProdDescricao",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            });

            dgvProdutos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ValorUnitario",
                HeaderText = "Valor",
                Name = "colProdValor",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvProdutos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Flag",
                HeaderText = "Flag",
                Name = "colProdFlag",
                Width = 80
            });

            dgvProdutos.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Vencimento",
                Name = "colProdVencimento",
                ReadOnly = true,
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvProdutos.CellFormatting += (sender, e) =>
            {
                if (e.RowIndex < 0) return;

                var grid = (DataGridView)sender;
                var colName = grid.Columns[e.ColumnIndex].Name;

                if (grid.Rows[e.RowIndex].DataBoundItem is Produto prod)
                {
                    if (colName == "colProdCodigoBarras")
                    {
                        e.Value = string.IsNullOrWhiteSpace(prod.CodigoBarras) ? "" : prod.CodigoBarras;
                        e.FormattingApplied = true;
                    }
                    else if (colName == "colProdFlag")
                    {
                        e.Value = string.Empty;
                        e.CellStyle.BackColor = GetFlagColor(prod.Flag);
                        e.FormattingApplied = true;
                    }
                    else if (colName == "colProdVencimento")
                    {
                        PreencherStatusVencimento(prod.DataValidade, e);
                    }
                }
            };
        }

        private void DgvProdutos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (_paleteAtual == null || _paleteAtual.Status == 2)
                return;

            _ = AddItemToPaleteAsync();
        }

        private void DgvProdutos_FlagFormatting(object sender,
                                                DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProdutos.Columns[e.ColumnIndex].Name != "colProdFlag")
                return;

            if (dgvProdutos.Rows[e.RowIndex].DataBoundItem is Produto prod)
            {
                e.Value = "";
                e.CellStyle.BackColor = GetFlagColor(prod.Flag);
            }
        }

        private void DgvProdutos_CellFormatting(object sender,
                                                DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProdutos.Columns[e.ColumnIndex].Name != "colProdVencimento")
                return;

            if (dgvProdutos.Rows[e.RowIndex].DataBoundItem is Produto prod)
                PreencherStatusVencimento(prod.DataValidade, e);
        }

        private void FormatGridItensPalete()
        {
            dgvItensPalete.AutoGenerateColumns = false;
            dgvItensPalete.Columns.Clear();
            dgvItensPalete.RowTemplate.Height = 24;

            dgvItensPalete.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            dgvItensPalete.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvItensPalete.DefaultCellStyle.ForeColor = Color.Black;

            var colDesc = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Descricao",
                HeaderText = "Descrição",
                Name = "colItemDescricao",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 55,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            };
            dgvItensPalete.Columns.Add(colDesc);

            var colBarras = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CodigoBarras",
                HeaderText = "Codigo de Barras",
                Name = "colItemCodigoBarras",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 20,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            dgvItensPalete.Columns.Add(colBarras);

            var colQtd = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantidade",
                HeaderText = "Qtd",
                Name = "colItemQtd",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            dgvItensPalete.Columns.Add(colQtd);

            var colValor = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ValorUnitario",
                HeaderText = "Valor Unit.",
                Name = "colItemValorUnit",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            };
            dgvItensPalete.Columns.Add(colValor);

            var colVenc = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Vencimento",
                HeaderText = "Vencimento",
                Name = "colItemVencimento",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 5,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            dgvItensPalete.Columns.Add(colVenc);

            foreach (DataGridViewColumn c in dgvItensPalete.Columns)
                c.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvItensPalete.MultiSelect = false;
            dgvItensPalete.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvItensPalete.CellFormatting -= dgvItensPalete_CellFormatting;
            dgvItensPalete.CellFormatting += dgvItensPalete_CellFormatting;
            dgvItensPalete.DataError -= dgvItensPalete_DataError;
            dgvItensPalete.DataError += dgvItensPalete_DataError;
        }

        private void dgvItensPalete_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var grid = (DataGridView)sender;
            var col = grid.Columns[e.ColumnIndex].Name;
            var item = grid.Rows[e.RowIndex].DataBoundItem as ItemPalete;

            if (item == null) return;

            if (col == "colItemCodigoBarras")
            {
                if (string.IsNullOrWhiteSpace(item.CodigoBarras) || item.CodigoBarras.StartsWith("MDL"))
                    e.Value = "-";
                else
                    e.Value = item.CodigoBarras;

                e.FormattingApplied = true;
            }
            else if (col == "colItemDescricao")
            {
                e.Value = item.Produto?.Descricao ?? "-";
                e.FormattingApplied = true;
            }
            else if (col == "colItemVencimento")
            {
                var dataVal = item.Produto?.DataValidade;
                if (dataVal == null)
                {
                    e.Value = "-";
                    e.FormattingApplied = true;
                    return;
                }

                var dias = (dataVal.Value.Date - DateTime.Today).TotalDays;

                if (dias < 0)
                {
                    e.Value = $"Vencido em {dataVal.Value:dd/MM/yyyy}";
                    e.CellStyle.ForeColor = Color.Red;
                }
                else if (dias <= 30)
                {
                    e.Value = $"A vencer ({dataVal.Value:dd/MM/yyyy})";
                    e.CellStyle.ForeColor = Color.Orange;
                }
                else
                {
                    e.Value = $"Válido até {dataVal.Value:dd/MM/yyyy}";
                    e.CellStyle.ForeColor = Color.Green;
                }

                e.FormattingApplied = true;
            }
        }

        private void dgvItensPalete_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void PreencherStatusVencimento(DateTime? dataVal, DataGridViewCellFormattingEventArgs e)
        {
            if (dataVal == null)
            {
                e.Value = "-";
                return;
            }

            var dias = (dataVal.Value - DateTime.Today).TotalDays;
            if (dias < 0)
            {
                e.Value = "Vencido";
                e.CellStyle.ForeColor = Color.Red;
            }
            else if (dias <= 30)
            {
                e.Value = "A vencer";
                e.CellStyle.ForeColor = Color.Orange;
            }
            else
            {
                e.Value = "Valido!";
                e.CellStyle.ForeColor = Color.Green;
            }
        }

        private void AtualizarLabelPaleteAtual()
        {
            if (_paleteAtual == null)
            {
                lblPaleteAtual.Text = "Nenhuma palete selecionada";
                lblPaleteAtual.ForeColor = Color.Black;
                return;
            }

            string statusTexto;
            Color statusCor;

            switch (_paleteAtual.Status)
            {
                case 0:
                    statusTexto = "Aberto";
                    statusCor = Color.LightSkyBlue;
                    break;
                case 1:
                    statusTexto = "Em andamento";
                    statusCor = Color.Orange;
                    break;
                case 2:
                    statusTexto = "Finalizado";
                    statusCor = Color.Tomato;
                    break;
                case 3:
                    statusTexto = "Vendido";
                    statusCor = Color.Blue;
                    break;
                default:
                    statusTexto = "Desconhecido";
                    statusCor = Color.Gray;
                    break;
            }

            lblPaleteAtual.Text = $"{_paleteAtual.Nome} - {statusTexto}";
            lblPaleteAtual.ForeColor = statusCor;
        }

        public class SystemFontResolver : IFontResolver
        {
            private static string FontsFolder => Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

            public byte[] GetFont(string faceName)
            {
                string path = faceName switch
                {
                    "SegoeUI#" => Path.Combine(FontsFolder, "segoeui.ttf"),
                    "SegoeUI-Bold#" => Path.Combine(FontsFolder, "segoeuib.ttf"),
                    "SegoeUI-Italic#" => Path.Combine(FontsFolder, "segoeuii.ttf"),
                    "SegoeUI-BoldItalic#" => Path.Combine(FontsFolder, "segoeuiz.ttf"),
                    _ => Path.Combine(FontsFolder, "segoeui.ttf"),
                };

                if (!File.Exists(path))
                {
                    if (faceName == "SegoeUI-Bold#" && File.Exists(Path.Combine(FontsFolder, "segoeui.ttf")))
                        path = Path.Combine(FontsFolder, "segoeui.ttf");
                    else if (faceName == "SegoeUI-Italic#" && File.Exists(Path.Combine(FontsFolder, "segoeui.ttf")))
                        path = Path.Combine(FontsFolder, "segoeui.ttf");
                    else if (faceName == "SegoeUI-BoldItalic#" && File.Exists(Path.Combine(FontsFolder, "segoeuib.ttf")))
                        path = Path.Combine(FontsFolder, "segoeuib.ttf");
                }

                return File.ReadAllBytes(path);
            }

            public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {
                if (string.Equals(familyName, "Segoe UI", StringComparison.OrdinalIgnoreCase))
                {
                    if (isBold && isItalic) return new FontResolverInfo("SegoeUI-BoldItalic#");
                    if (isBold) return new FontResolverInfo("SegoeUI-Bold#");
                    if (isItalic) return new FontResolverInfo("SegoeUI-Italic#");
                    return new FontResolverInfo("SegoeUI#");
                }

                return new FontResolverInfo("SegoeUI#");
            }
        }


        private void txtBusca_TextChanged(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private CancellationTokenSource _cts;
        private async void DebounceTimer_Tick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            try
            {
                await AtualizarGridProdutosAsync(txtBusca.Text.Trim(), _cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task AtualizarGridProdutosAsync(string filtro, CancellationToken ct)
        {
            IQueryable<Produto> query = _ctx.Produtos.AsNoTracking();

            if (!string.IsNullOrEmpty(filtro))
                query = query.Where(p =>
                    p.CodigoBarras.Contains(filtro) ||
                    p.Descricao.Contains(filtro));

            var lista = await query
                .OrderBy(p => p.Descricao)
                .ToListAsync(ct);

            bsProdutos.DataSource = lista;
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (_paleteAtual == null)
                return;
            int paleteId = _paleteAtual.Id;

            using (var ctx = new ReverseContext())
            {
                var palete = ctx.Paletes
                    .Include(p => p.Itens.Select(i => i.Produto))
                    .AsNoTracking()
                    .FirstOrDefault(p => p.Id == paleteId);

                if (palete == null)
                    return;

                decimal totalPalete = palete.Itens.Sum(i => i.Quantidade * i.ValorUnitario);
                int totalItens = palete.Itens.Sum(i => i.Quantidade);

                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileXlsx = Path.Combine(desktop, palete.Nome + ".xlsx");

                try
                {
                    using var wb = new XLWorkbook();
                    var ws = wb.Worksheets.Add("Palete");

                    int row = 1;

                    ws.Range(row, 1, row, 6).Merge();
                    ws.Cell(row, 1).Value = $"Valor total da palete: R$ {totalPalete:N2}";
                    ws.Cell(row, 1).Style
                        .Font.SetBold()
                        .Font.SetFontColor(XLColor.White)
                        .Fill.SetBackgroundColor(XLColor.DarkBlue)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    ws.Row(row).Height = 25;
                    row++;

                    ws.Range(row, 1, row, 6).Merge();
                    ws.Cell(row, 1).Value = $"Total de itens: {totalItens}";
                    ws.Cell(row, 1).Style
                        .Font.SetBold()
                        .Font.SetFontColor(XLColor.White)
                        .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    ws.Row(row).Height = 22;
                    row += 2;

                    string[] headers = { "Origem", "Barras", "Descrição", "Qtd", "Valor Unit.", "Total" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cell(row, i + 1).Value = headers[i];
                        ws.Cell(row, i + 1).Style
                            .Font.SetBold()
                            .Fill.SetBackgroundColor(XLColor.SteelBlue)
                            .Font.SetFontColor(XLColor.White)
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    }
                    row++;

                    foreach (var item in palete.Itens)
                    {
                        ws.Cell(row, 1).Value = palete.Numero;

                        string codigo = "-";
                        if (!string.IsNullOrWhiteSpace(item.CodigoBarras))
                            codigo = new string(item.CodigoBarras.Where(char.IsDigit).ToArray());
                        ws.Cell(row, 2).Value = codigo;

                        ws.Cell(row, 3).Value = item.Produto?.Descricao ?? "-";
                        ws.Cell(row, 4).Value = item.Quantidade;
                        ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0";
                        ws.Cell(row, 5).Value = item.ValorUnitario;
                        ws.Cell(row, 5).Style.NumberFormat.Format = "R$ #,##0.00";
                        ws.Cell(row, 6).Value = item.Quantidade * item.ValorUnitario;
                        ws.Cell(row, 6).Style.NumberFormat.Format = "R$ #,##0.00";

                        if ((row - 4) % 2 == 0)
                            ws.Range(row, 1, row, 6).Style.Fill.SetBackgroundColor(XLColor.LightSteelBlue);
                        else
                            ws.Range(row, 1, row, 6).Style.Fill.SetBackgroundColor(XLColor.White);

                        ws.Range(row, 1, row, 6).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                        row++;
                    }

                    ws.Range(4, 1, row - 1, 6).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                    ws.Columns().AdjustToContents();
                    ws.Column(3).Width = Math.Max(ws.Column(3).Width, 40);

                    wb.SaveAs(fileXlsx);

                    MessageBox.Show($"Excel gerado em:\n{fileXlsx}",
                        "Exportação Concluida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao exportar para Excel: " + ex.Message,
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSelecionarPalete_Click(object sender, EventArgs e)
        {
            using (var selector = new Reverse.Forms.FormsTriagem.TriagemPaleteSelectorForm())
            {
                if (selector.ShowDialog() == DialogResult.OK && selector.PaleteSelecionada != null)
                {
                    using (var ctx = new ReverseContext())
                    {
                        _paleteAtual = ctx.Paletes
                            .Include(p => p.Itens.Select(i => i.Produto))
                            .FirstOrDefault(p => p.Id == selector.PaleteSelecionada.Id);
                    }

                    if (_paleteAtual == null)
                    {
                        MessageBox.Show("Não foi possível carregar a palete selecionada.", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    LoadItensDaPalete();
                    AtualizarLabelPaleteAtual();
                    AtualizarEstadoBotoes();
                }
            }
        }
    }
}