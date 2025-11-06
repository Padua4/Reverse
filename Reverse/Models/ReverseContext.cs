using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using static Reverse.Forms.FormsExpedicao.ExpedicaoFormEstoque;

namespace Reverse.Models
{
    public class ReverseContext : DbContext
    {
        public ReverseContext() : base("ReverseDB")
        {
            Database.SetInitializer<ReverseContext>(null);
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Palete> Paletes { get; set; }
        public DbSet<ItemPalete> ItensPalete { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Material> Materiais { get; set; }
        public DbSet<Permissao> Permissoes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Permissao>().ToTable("Permissoes");

            modelBuilder.Entity<Produto>().ToTable("Produto");
            modelBuilder.Entity<ItemPalete>().ToTable("ItemPalete");
            modelBuilder.Entity<Palete>().ToTable("Palete");
            modelBuilder.Entity<Estoque>().ToTable("Estoques");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");

            modelBuilder.Entity<Permissao>()
                .Property(p => p.FormName)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Produto>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Produto>()
                .Property(p => p.CodigoBarras)
                .IsOptional()
                .HasMaxLength(50);

            modelBuilder.Entity<Produto>()
                .HasIndex(p => p.CodigoBarras)
                .IsUnique();

            modelBuilder.Entity<ItemPalete>()
                .HasRequired(i => i.Produto)
                .WithMany(p => p.ItensPalete)
                .HasForeignKey(i => i.ProdutoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Palete>()
                .HasMany(p => p.Itens)
                .WithRequired(i => i.Palete)
                .HasForeignKey(i => i.PaleteId)
                .WillCascadeOnDelete(false);
        }
    }
}