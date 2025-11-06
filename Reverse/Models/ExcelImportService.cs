using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using ClosedXML.Excel;
using Reverse.Models;

namespace Reverse.Services
{
    public class ExcelImportService
    {
        public ImportResult ImportProductsFromExcel(string filePath)
        {
            var result = new ImportResult();
            var produtosParaImportar = new List<Produto>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheets.First();
                var headerRow = worksheet.Row(1);
                var columnMap = MapearColunas(headerRow, result);

                if (result.MissingColumns.Any())
                    return result;

                int lastRow = worksheet.LastRowUsed().RowNumber();
                for (int row = 2; row <= lastRow; row++)
                {
                    var errosLinha = new List<string>();
                    var codBarras = worksheet.Cell(row, columnMap["CodigoBarras"]).GetString().Trim();
                    var descricao = worksheet.Cell(row, columnMap["Descricao"]).GetString().Trim();
                    var qtdStr = worksheet.Cell(row, columnMap["QTD"]).GetString().Trim();
                    var vUniStr = worksheet.Cell(row, columnMap["vUni"]).GetString().Trim();

                    if (string.IsNullOrEmpty(codBarras))
                        errosLinha.Add("CodigoBarras vazio");
                    if (string.IsNullOrEmpty(descricao))
                        errosLinha.Add("Descricao vazia");
                    if (!int.TryParse(qtdStr, out int qtd))
                        errosLinha.Add("QTD inválida");
                    if (!decimal.TryParse(vUniStr, out decimal vUni))
                        errosLinha.Add("vUni inválido");

                    if (errosLinha.Any())
                    {
                        result.RowErrors.Add(new RowError
                        {
                            RowNumber = row,
                            ErrorMessage = string.Join("; ", errosLinha)
                        });
                    }
                    else
                    {
                        produtosParaImportar.Add(new Produto
                        {
                            CodigoBarras = codBarras,
                            Descricao = descricao,
                            Quantidade = qtd,
                            ValorUnitario = vUni,
                            DataUltimaAlteracao = DateTime.UtcNow,
                            Flag = FlagType.Importado
                        });
                    }
                }
            }

            if (produtosParaImportar.Any())
            {
                result.DuplicatesInExcel = produtosParaImportar
                    .GroupBy(p => p.CodigoBarras)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                produtosParaImportar = produtosParaImportar
                    .GroupBy(p => p.CodigoBarras)
                    .Select(g => g.First())
                    .ToList();

                var existentes = ProdutoService.GetAll()
                    .Select(p => p.CodigoBarras)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                result.DuplicatesInDatabase = produtosParaImportar
                    .Where(p => existentes.Contains(p.CodigoBarras))
                    .Select(p => p.CodigoBarras)
                    .ToList();

                var paraInserir = produtosParaImportar
                    .Where(p => !existentes.Contains(p.CodigoBarras))
                    .ToList();

                if (paraInserir.Any())
                {
                    foreach (var p in paraInserir)
                    {
                        p.Emissao = DateTime.UtcNow;
                    }
                    try
                    {
                        ProdutoService.AddRange(paraInserir);
                        result.ImportedCount = paraInserir.Count;
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entryErrors in ex.EntityValidationErrors)
                        {
                            var entityName = entryErrors.Entry.Entity.GetType().Name;
                            foreach (var ve in entryErrors.ValidationErrors)
                            {
                                result.RowErrors.Add(new RowError
                                {
                                    RowNumber = 0,
                                    ErrorMessage = $"{entityName}.{ve.PropertyName}: {ve.ErrorMessage}"
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        private Dictionary<string, int> MapearColunas(IXLRow headerRow, ImportResult result)
        {
            var required = new[] { "CodigoBarras", "Descricao", "QTD", "vUni" };
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var col in required)
            {
                var cell = headerRow
                    .CellsUsed()
                    .FirstOrDefault(c =>
                        string.Equals(c.GetString(), col, StringComparison.OrdinalIgnoreCase));

                if (cell == null)
                    result.MissingColumns.Add(col);
                else
                    map[col] = cell.Address.ColumnNumber;
            }

            return map;
        }
    }

    public class ImportResult
    {
        public List<string> MissingColumns { get; } = new();
        public List<RowError> RowErrors { get; } = new();
        public List<string> DuplicatesInExcel { get; set; } = new();
        public List<string> DuplicatesInDatabase { get; set; } = new();
        public int ImportedCount { get; set; }
    }

    public class RowError
    {
        public int RowNumber { get; set; }
        public string ErrorMessage { get; set; }
    }
}
