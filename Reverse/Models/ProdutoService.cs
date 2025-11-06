using Reverse.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reverse.Services
{
    public static class ProdutoService
    {
        public static void AddRange(IEnumerable<Produto> lista)
        {
            using var ctx = new ReverseContext();

            foreach (var p in lista)
            {
                p.DataUltimaAlteracao = DateTime.UtcNow;
                ctx.Produtos.Add(p);
            }

            try
            {
                ctx.Database.Log = sql => System.Diagnostics.Debug.WriteLine(sql);
                ctx.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var sb = new StringBuilder();
                foreach (var entryErrors in dbEx.EntityValidationErrors)
                {
                    sb.AppendLine($"Entidade: {entryErrors.Entry.Entity.GetType().Name}");
                    foreach (var ve in entryErrors.ValidationErrors)
                        sb.AppendLine($"  - Propriedade: {ve.PropertyName} | Erro: {ve.ErrorMessage}");
                }

                System.Diagnostics.Debug.WriteLine("ERROS DE VALIDAÇÃO NO EF:\n" + sb);
                throw;
            }
            catch (Exception ex)
            {
                var msg = new StringBuilder();
                msg.AppendLine("Erro inesperado ao salvar no banco:");
                msg.AppendLine(ex.GetType().FullName + ": " + ex.Message);
                if (ex.InnerException != null)
                    msg.AppendLine("Inner: " + ex.InnerException.Message);
                throw new Exception(msg.ToString(), ex);
            }
        }



        private static string GerarCodigoSintetico(ReverseContext ctx)
        {
            for (int i = 0; i < 5; i++)
            {
                var codigo = "MDL-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpperInvariant();
                bool existe = ctx.Produtos.AsNoTracking().Any(p => p.CodigoBarras == codigo);
                if (!existe) return codigo;
            }
            return "MDL-" + Guid.NewGuid().ToString("N").ToUpperInvariant();
        }

        public static async Task<bool> CreateAsync(Produto produto)
        {
            if (produto == null) throw new ArgumentNullException(nameof(produto));

            using (var ctx = new ReverseContext())
            {
                if (string.IsNullOrWhiteSpace(produto.CodigoBarras))
                {
                    produto.CodigoBarras = GerarCodigoSintetico(ctx);
                }

                bool existe = await ctx.Produtos
                    .AsNoTracking()
                    .AnyAsync(p => p.CodigoBarras == produto.CodigoBarras);

                if (existe) return false;

                produto.DataUltimaAlteracao = DateTime.UtcNow;
                SanitizeDates(produto);
                ctx.Produtos.Add(produto);

                try
                {
                    await ctx.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sql &&
                                                   (sql.Number == 2627 || sql.Number == 2601))
                {
                    return false;
                }
            }
        }


        private static void SanitizeDates(Produto p)
        {
            var min = new DateTime(1753, 1, 1);
            if (p.Emissao < min) p.Emissao = DateTime.Now;
            if (p.DataUltimaAlteracao < min) p.DataUltimaAlteracao = DateTime.UtcNow;
            if (p.DataValidade.HasValue && p.DataValidade.Value < min) p.DataValidade = null;
        }

        public static async Task<bool> UpdateAsync(Produto produto)
        {
            if (produto == null) throw new ArgumentNullException(nameof(produto));

            using (var ctx = new ReverseContext())
            {
                // Busca pelo Id (nova PK)
                var existente = await ctx.Produtos.FindAsync(produto.Id);

                if (existente == null)
                    return false;

                ctx.Entry(existente).CurrentValues.SetValues(produto);

                existente.DataUltimaAlteracao = DateTime.UtcNow;
                SanitizeDates(existente);

                try
                {
                    await ctx.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    var atual = await ctx.Produtos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == produto.Id);

                    throw new ConcurrencyException(
                        "Conflito de edição: este produto foi alterado por outro usuário.",
                        atual
                    );
                }
            }
        }


        public static List<Produto> GetAll()
        {
            using var ctx = new ReverseContext();
            return ctx.Produtos.ToList();
        }

        public static List<Produto> GetFiltered(string cod, DateTime? data, string desc)
        {
            using var ctx = new ReverseContext();
            var q = ctx.Produtos.AsQueryable();

            if (!string.IsNullOrEmpty(cod))
                q = q.Where(p => p.CodigoBarras.Contains(cod));

            if (data.HasValue)
                q = q.Where(p =>
                    DbFunctions.TruncateTime(p.DataUltimaAlteracao) == data.Value.Date);

            if (!string.IsNullOrEmpty(desc))
                q = q.Where(p => p.Descricao.Contains(desc));

            return q.ToList();
        }
    }

    public class ConcurrencyException : Exception
    {
        public Produto CurrentFromDb { get; }
        public ConcurrencyException(string message, Produto currentFromDb = null) : base(message)
        {
            CurrentFromDb = currentFromDb;
        }
    }
}
