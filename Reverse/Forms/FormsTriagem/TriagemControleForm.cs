using Reverse.Forms;
using Reverse.Models;
using Reverse.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ClosedXML.Excel;
using System.Threading.Tasks;

namespace Reverse
{
    public partial class TriagemControleForm : Form
    {
        private readonly int _usuarioId;
        private readonly BindingSource _bsNotas = new BindingSource();

        private static readonly Color CorVencido = Color.Red;
        private static readonly Color CorAvence = Color.Orange;
        private static readonly Color CorValido = Color.Green;

        private readonly BindingSource _bsFiltrados = new BindingSource();
        private readonly ExcelImportService _importService = new ExcelImportService();
        private readonly List<Produto> _listaFiltrada = new List<Produto>();

        private class FlagItem
        {
            public FlagType Flag { get; set; }
            public Color Color { get; set; }
            public string Text { get; set; }
            public override string ToString() => Text;
        }

        public TriagemControleForm(int usuarioId)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            dgvNotas.AutoGenerateColumns = false;
            dgvNotas.DataSource = _bsNotas;

            dgvNotas.CellFormatting += DgvNotas_CellFormatting;
            this.Load += ControleTriagemForm_Load;
            this.Shown += ControleTriagemForm_Shown;
            dgvNotas.DataBindingComplete += (s, e) => FormatGridNotas();

            var flags = new List<FlagItem>
            {
                new FlagItem { Flag = FlagType.Importado,    Color = Color.Black,       Text = "Importado"      },
                new FlagItem { Flag = FlagType.SemAlteracao, Color = Color.White,       Text = "Sem Alteração" },
                new FlagItem { Flag = FlagType.MercadoLivre, Color = Color.Gold,        Text = "Mercado Livre"  },
                new FlagItem { Flag = FlagType.Amazon,       Color = Color.DodgerBlue,  Text = "Amazon"         },
                new FlagItem { Flag = FlagType.Variados,     Color = Color.LightGray,   Text = "Variados"       }
            };

            cmbFiltroFlag.DrawMode = DrawMode.OwnerDrawFixed;
            cmbFiltroFlag.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFiltroFlag.ValueMember = nameof(FlagItem.Flag);
            cmbFiltroFlag.DisplayMember = nameof(FlagItem.Text);
            cmbFiltroFlag.DataSource = flags;
            cmbFiltroFlag.ItemHeight = 20;
            cmbFiltroFlag.DrawItem += cmbFiltroFlag_DrawItem;
            cmbFiltroFlag.SelectedIndex = -1;

            FormatGridNotas();
            dgvNotas.CellDoubleClick += DgvNotas_CellDoubleClick;

            FormatGridFiltrados();
            dgvFiltrados.DataSource = _bsFiltrados;

            dtpFiltroEmissao.Format = DateTimePickerFormat.Custom;
            dtpFiltroEmissao.CustomFormat = " ";
            dtpFiltroEmissao.Value = DateTime.Today;
            chkFiltroEmissao.CheckedChanged += chkFiltroEmissao_CheckedChanged;
        }

        private void cmbFiltroFlag_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0) return;

            var item = (FlagItem)cmbFiltroFlag.Items[e.Index];
            int diameter = 12;
            var circleRect = new Rectangle(
                e.Bounds.Left + 2,
                e.Bounds.Top + (e.Bounds.Height - diameter) / 2,
                diameter,
                diameter);

            using (var brush = new SolidBrush(item.Color))
                e.Graphics.FillEllipse(brush, circleRect);

            var textPos = new Point(
                circleRect.Right + 6,
                e.Bounds.Top + (e.Bounds.Height - e.Font.Height) / 2);

            TextRenderer.DrawText(
                e.Graphics,
                item.Text,
                e.Font,
                textPos,
                e.ForeColor);

            e.DrawFocusRectangle();
        }

        private async void ControleTriagemForm_Load(object sender, EventArgs e)
        {
            CriarColunasGrid();
            lblGreeting.Text = $"Bem-vindo ao Controle de Triagem, {_usuarioId}!";
            await CarregarProdutosNoGridAsync();
        }

        private async Task CarregarProdutosNoGridAsync()
        {
            var lista = await Task.Run(() =>
            {
                using (var ctx = new ReverseContext())
                {
                    return ctx.Produtos
                        .AsNoTracking()
                        .Select(p => new
                        {
                            p.CodigoBarras,
                            p.Descricao,
                            p.ValorUnitario,
                            p.DataValidade,
                            p.DataUltimaAlteracao,
                            p.Flag
                        })
                        .ToList()
                        .Select(p => new Produto
                        {
                            CodigoBarras = p.CodigoBarras,
                            Descricao = p.Descricao,
                            ValorUnitario = p.ValorUnitario,
                            DataValidade = p.DataValidade,
                            DataUltimaAlteracao = p.DataUltimaAlteracao,
                            Flag = p.Flag
                        })
                        .ToList();
                }
            });

            dgvNotas.DataSource = lista;
        }

        private void CriarColunasGrid()
        {
            dgvNotas.Columns.Clear();

            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCodigoBarras",
                HeaderText = "Código de Barras",
                DataPropertyName = "CodigoBarras"
            });

            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDescricao",
                HeaderText = "Descrição",
                DataPropertyName = "Descricao"
            });

            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colValorUnitario",
                HeaderText = "ValorUn.",
                DataPropertyName = "ValorUnitario"
            });
            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFlag",
                HeaderText = "Flag",
                DataPropertyName = "Flag"
            });
            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDataValidade",
                HeaderText = "Data de Validade",
                DataPropertyName = "DataValidade"
            });
            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDataUltimaAlteracao",
                HeaderText = "Última Alteração",
                DataPropertyName = "DataUltimaAlteracao",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

        }

        private void AjustarColuna(string nome, int largura)
        {
            if (dgvNotas.Columns.Contains(nome))
            {
                dgvNotas.Columns[nome].Width = largura;
                dgvNotas.Columns[nome].SortMode = DataGridViewColumnSortMode.Programmatic;
            }
        }

        private void FormatGridNotas()
        {
            dgvNotas.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dgvNotas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvNotas.DefaultCellStyle.ForeColor = Color.Black;

            foreach (DataGridViewColumn col in dgvNotas.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Programmatic;
            }

            if (dgvNotas.Columns.Contains("colCodigoBarras"))
            {
                dgvNotas.Columns["colCodigoBarras"].DefaultCellStyle.Font =
                    new Font("Segoe UI", 9F, FontStyle.Bold);
                dgvNotas.Columns["colCodigoBarras"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            AjustarColuna("colCodigoBarras", 120);
            AjustarColuna("colDescricao", 250);
            AjustarColuna("colValorUnitario", 90);
            AjustarColuna("colFlag", 60);
            AjustarColuna("colDataValidade", 110);
            AjustarColuna("colDataUltimaAlteracao", 130);
        }


        private void ControleTriagemForm_Shown(object sender, EventArgs e)
        {
            dgvNotas.AutoResizeRows();
        }


        private void FormatGridFiltrados()
        {
            dgvFiltrados.AutoGenerateColumns = false;
            dgvFiltrados.Columns.Clear();

            dgvFiltrados.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dgvFiltrados.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvFiltrados.DefaultCellStyle.ForeColor = Color.Black;

            dgvFiltrados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CodigoBarras",
                HeaderText = "Código",
                Name = "colF_Codigo",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvFiltrados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Descricao",
                HeaderText = "Descrição",
                Name = "colF_Descricao",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            });

            dgvFiltrados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ValorUnitario",
                HeaderText = "Valor Unit.",
                Name = "colF_Valor",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
        }

        private void DgvNotas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvNotas.Rows[e.RowIndex].IsNewRow)
                return;

            var produto = dgvNotas.Rows[e.RowIndex].DataBoundItem as Produto;
            if (produto == null) return;

            if (dgvNotas.Columns[e.ColumnIndex].Name == "colFlag")
            {
                e.CellStyle.BackColor = GetFlagColor(produto.Flag);
                e.Value = produto.Flag.ToString();
            }
        }

        private Color GetFlagColor(FlagType flag)
        {
            switch (flag)
            {
                case FlagType.Importado: return Color.Black;
                case FlagType.SemAlteracao: return Color.White;
                case FlagType.MercadoLivre: return Color.Gold;
                case FlagType.Amazon: return Color.DodgerBlue;
                case FlagType.Variados: return Color.LightGray;
                default: return CorValido;
            }
        }

        private void DgvNotas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var prod = (Produto)dgvNotas.Rows[e.RowIndex].DataBoundItem;
            if (_listaFiltrada.Any(p => p.CodigoBarras == prod.CodigoBarras))
                return;

            _listaFiltrada.Add(prod);
            _bsFiltrados.DataSource = null;
            _bsFiltrados.DataSource = _listaFiltrada;
            dgvFiltrados.AutoResizeColumns();
            AtualizarTotalFiltrados();
        }

        private async void btnFiltrar_Click(object sender, EventArgs e)
        {
            string filtroDescricao = txtFiltroDescricao.Text?.Trim();
            string filtroCodBarras = txtFiltroCodBarras.Text?.Trim();

            var lista = await Task.Run(() =>
            {
                using (var ctx = new ReverseContext())
                {
                    var query = ctx.Produtos.AsNoTracking();

                    if (chkFiltroEmissao.Checked)
                    {
                        var data = dtpFiltroEmissao.Value.Date;
                        query = (System.Data.Entity.Infrastructure.DbQuery<Produto>)
                            query.Where(p => DbFunctions.TruncateTime(p.DataUltimaAlteracao) == data);
                    }
                    if (!string.IsNullOrEmpty(filtroDescricao))
                        query = (System.Data.Entity.Infrastructure.DbQuery<Produto>)query.Where(p => p.Descricao.Contains(filtroDescricao));

                    if (!string.IsNullOrEmpty(filtroCodBarras))
                        query = (System.Data.Entity.Infrastructure.DbQuery<Produto>)query.Where(p => p.CodigoBarras.Contains(filtroCodBarras));

                    return query
                        .Select(p => new
                        {
                            p.CodigoBarras,
                            p.Descricao,
                            p.ValorUnitario,
                            p.DataValidade,
                            p.DataUltimaAlteracao,
                            p.Flag,
                            p.RowVersion
                        })
                        .ToList()
                        .Select(p => new Produto
                        {
                            CodigoBarras = p.CodigoBarras,
                            Descricao = p.Descricao,
                            ValorUnitario = p.ValorUnitario,
                            DataValidade = p.DataValidade,
                            DataUltimaAlteracao = p.DataUltimaAlteracao,
                            Flag = p.Flag,
                            RowVersion = p.RowVersion
                        })
                        .ToList();
                }
            });

            _listaFiltrada.Clear();
            _listaFiltrada.AddRange(lista);

            _bsFiltrados.DataSource = null;
            _bsFiltrados.DataSource = _listaFiltrada;

            dgvFiltrados.AutoResizeColumns();
            AtualizarTotalFiltrados();
        }


        private void AtualizarTotalFiltrados()
        {
            decimal total = _listaFiltrada.Sum(p => p.ValorUnitario);
            lblTotal.Text = $"Total selecionado: {total:C2}";
        }

        private async void btnLimparFiltro_Click(object sender, EventArgs e)
        {
            txtFiltroCodBarras.Clear();
            txtFiltroDescricao.Clear();
            chkFiltroEmissao.Checked = false;
            dtpFiltroEmissao.Value = DateTime.Today;
            cmbFiltroFlag.SelectedIndex = -1;

            await CarregarProdutosNoGridAsync();

            _listaFiltrada.Clear();
            _bsFiltrados.DataSource = null;
            _bsFiltrados.DataSource = _listaFiltrada;

            dgvFiltrados.AutoResizeColumns();
            AtualizarTotalFiltrados();
        }

        private void chkFiltroEmissao_CheckedChanged(object sender, EventArgs e)
        {
            dtpFiltroEmissao.CustomFormat =
                chkFiltroEmissao.Checked ? "dd/MM/yyyy" : " ";
        }

        private async void btnImportExcel_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "Arquivos Excel (*.xlsx)|*.xlsx",
                Title = "Selecione o arquivo de importação"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            var result = _importService.ImportProductsFromExcel(dlg.FileName);
            var avisos = new List<string>();
            if (result.DuplicatesInExcel.Any())
                avisos.Add("Duplicados no Excel (ignorados): " +
                           string.Join(", ", result.DuplicatesInExcel));
            if (result.DuplicatesInDatabase.Any())
                avisos.Add("Já existentes no banco (ignorados): " +
                           string.Join(", ", result.DuplicatesInDatabase));

            var msg = new StringBuilder();
            if (avisos.Any())
            {
                msg.AppendLine("Importação parcial, com avisos:");
                avisos.ForEach(a => msg.AppendLine(" • " + a));
            }
            msg.AppendLine($"Produtos inseridos: {result.ImportedCount}");

            MessageBox.Show(
                msg.ToString(),
                "Resumo da Importação",
                MessageBoxButtons.OK,
                avisos.Any()
                    ? MessageBoxIcon.Warning
                    : MessageBoxIcon.Information
            );
            await CarregarProdutosNoGridAsync();
        }

        private async void btnEditarProduto_Click(object sender, EventArgs e)
        {
            if (dgvNotas.CurrentRow?.DataBoundItem is not Produto selecionado)
                return;

            using (var ctx = new ReverseContext())
            {
                // Busca o produto no banco com a versão mais atual
                var produtoDb = await ctx.Produtos.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CodigoBarras == selecionado.CodigoBarras);

                if (produtoDb == null)
                {
                    MessageBox.Show(
                        "Produto não encontrado no banco.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    await CarregarProdutosNoGridAsync();
                    return;
                }

                using (var form = new TriagemProdutoForm(_usuarioId)
                {
                    ProdutoAtual = produtoDb,
                    StartPosition = FormStartPosition.CenterParent
                })
                {
                    if (form.ShowDialog() != DialogResult.OK)
                        return;

                    try
                    {
                        await ProdutoService.UpdateAsync(form.ProdutoAtual);
                        await CarregarProdutosNoGridAsync();
                    }
                    catch (ConcurrencyException)
                    {
                        MessageBox.Show(
                            "Alguém alterou este produto enquanto você editava.\n" +
                            "Os dados atuais do banco serão recarregados para revisão.",
                            "Conflito de edição",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        await CarregarProdutosNoGridAsync();
                    }
                }
            }
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            using var formSel = new TriagemSelecionarPaletesForm();
            if (formSel.ShowDialog() != DialogResult.OK)
                return;

            var ids = formSel.PaletesSelecionados;
            if (ids == null || ids.Count == 0)
            {
                MessageBox.Show("Nenhuma palete selecionada.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ExportarExcel(ids);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao exportar: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportarExcel(List<int> idsPaletes)
        {
            // TODO: ajuste o nome do seu DbContext, se necessário
            using (var ctx = new ReverseContext())
            {
                // TODO: ajuste os includes/navegações conforme suas entidades
                var paletes = ctx.Paletes
                    .Include(p => p.Itens.Select(i => i.Produto))
                    .Where(p => idsPaletes.Contains(p.Id))
                    .ToList();

                if (!paletes.Any())
                {
                    MessageBox.Show("Não foram encontradas paletes para exportar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal valorTotal = paletes.Sum(p => p.Itens.Sum(i => i.Quantidade * i.ValorUnitario));
                int totalItens = paletes.Sum(p => p.Itens.Sum(i => i.Quantidade));

                string titulo = idsPaletes.Count > 1
                    ? $"Valor total do lote: R$ {valorTotal:N2}"
                    : $"Valor total da palete: R$ {valorTotal:N2}";

                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string nomeArq = $"Export_Paletes_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                string caminho = Path.Combine(desktop, nomeArq);

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Paletes");

                    int row = 1;

                    // Faixa 1 — Título
                    ws.Range(row, 1, row, 6).Merge();
                    ws.Cell(row, 1).Value = titulo;
                    ws.Cell(row, 1).Style
                        .Font.SetBold()
                        .Font.SetFontColor(XLColor.White)
                        .Fill.SetBackgroundColor(XLColor.DarkBlue)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    ws.Row(row).Height = 25;
                    row++;

                    // Faixa 2 — Total de Itens
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

                    // Cabeçalho
                    string[] headers = { "Palete", "Código", "Descrição", "Qtd", "Valor Unit.", "Total" };
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

                    // Dados
                    foreach (var pal in paletes)
                    {
                        foreach (var item in pal.Itens)
                        {
                            // TODO: ajuste "pal.Nome" se sua entidade usar "Numero" ou outro campo de identificação
                            ws.Cell(row, 1).Value = pal.Nome;
                            ws.Cell(row, 2).Value = item.CodigoBarras ?? "-";
                            ws.Cell(row, 3).Value = item.Produto?.Descricao ?? "-";
                            ws.Cell(row, 4).Value = item.Quantidade;
                            ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0";
                            ws.Cell(row, 5).Value = item.ValorUnitario;
                            ws.Cell(row, 5).Style.NumberFormat.Format = "R$ #,##0.00";
                            ws.Cell(row, 6).Value = item.Quantidade * item.ValorUnitario;
                            ws.Cell(row, 6).Style.NumberFormat.Format = "R$ #,##0.00";

                            // Zebra azul/branco (deslocamento após cabeçalho)
                            if ((row - 4) % 2 == 0)
                                ws.Range(row, 1, row, 6).Style.Fill.SetBackgroundColor(XLColor.LightSteelBlue);
                            else
                                ws.Range(row, 1, row, 6).Style.Fill.SetBackgroundColor(XLColor.White);

                            ws.Range(row, 1, row, 6).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                            row++;
                        }
                    }

                    // Bordas internas da tabela
                    ws.Range(4, 1, row - 1, 6).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                    // Ajustes de coluna
                    ws.Columns().AdjustToContents();
                    ws.Column(3).Width = Math.Max(ws.Column(3).Width, 40);

                    wb.SaveAs(caminho);
                }

                MessageBox.Show($"Excel gerado em:\n{caminho}",
                    "Exportação concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void btnExcluirProduto_Click(object sender, EventArgs e)
        {
            if (dgvNotas.CurrentRow?.DataBoundItem is not Produto prodSel)
                return;

            var resp = MessageBox.Show(
                $"Você tem certeza que deseja deletar o produto:\n{prodSel.Descricao}?",
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (resp != DialogResult.Yes)
                return;

            using (var ctx = new ReverseContext())
            {
                var paletesComProduto = await ctx.Paletes
                    .Where(p => p.Itens.Any(i => i.CodigoBarras == prodSel.CodigoBarras))
                    .Select(p => new { p.Numero, p.Categoria })
                    .ToListAsync();

                if (paletesComProduto.Any())
                {
                    var nomes = paletesComProduto
                        .Select(p => $"Palete {p.Numero} - {p.Categoria.GetDescription()}");

                    MessageBox.Show(
                        "Não é possível excluir este produto porque ele está vinculado às seguintes paletes:\n\n" +
                        string.Join("\n", nomes),
                        "Exclusão bloqueada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var produtoDb = await ctx.Produtos
                    .FirstOrDefaultAsync(p => p.CodigoBarras == prodSel.CodigoBarras);

                if (produtoDb == null)
                {
                    MessageBox.Show(
                        "Produto não encontrado no banco.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    await CarregarProdutosNoGridAsync();
                    return;
                }

                ctx.Produtos.Remove(produtoDb);

                try
                {
                    await ctx.SaveChangesAsync();

                    MessageBox.Show(
                        "Produto excluído com sucesso.",
                        "Exclusão concluída",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await CarregarProdutosNoGridAsync();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    MessageBox.Show(
                        "O produto foi alterado ou removido por outro usuário. A lista será atualizada.",
                        "Conflito de exclusão",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    await CarregarProdutosNoGridAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Erro ao excluir o produto: " + ex.Message,
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void dgvNotas_SelectionChanged(object sender, EventArgs e)
        {
            btnExcluirProduto.Enabled = dgvNotas.CurrentRow != null;
        }
    }
}