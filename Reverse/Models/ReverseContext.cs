using System.Data.Entity;
using static Reverse.Forms.FormsExpedicao.ExpedicaoFormEstoque;

namespace Reverse.Models
{
    public class ReverseContext : DbContext
    {
        public ReverseContext() : base("ReverseDB")
        {
            Database.SetInitializer<ReverseContext>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Palete> Paletes { get; set; }
        public DbSet<ItemPalete> ItensPalete { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Material> Materiais { get; set; }
        public DbSet<Permissao> Permissoes { get; set; }
        public DbSet<AtividadeUsuario> AtividadesUsuarios { get; set; }
        public DbSet<SessaoUsuario> SessoesUsuarios { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<NotificacaoLida> NotificacoesLidas { get; set; }   
        public DbSet<CategoriaPalete> CategoriasPalete { get; set; }

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
            modelBuilder.Entity<CategoriaPalete>().ToTable("CategoriaPalete");

            modelBuilder.Entity<Permissao>()
                .Property(p => p.FormName)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Produto>().HasKey(p => p.Id);
            modelBuilder.Entity<Produto>()
                .Property(p => p.CodigoBarras)
                .IsOptional().HasMaxLength(50);
            modelBuilder.Entity<Produto>()
                .HasIndex(p => p.CodigoBarras).IsUnique();

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

            modelBuilder.Entity<Palete>()
                .HasRequired(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.CategoriaId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Notificacao>().ToTable("Notificacoes");

            modelBuilder.Entity<Notificacao>()
                .Property(n => n.Mensagem)
                .IsRequired().HasMaxLength(500);

            modelBuilder.Entity<Notificacao>()
                .HasRequired<Usuario>(n => n.Remetente)
                .WithMany()
                .HasForeignKey(n => n.UsuarioRemetenteId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Notificacao>()
                .HasOptional<Usuario>(n => n.Destinatario)
                .WithMany()
                .HasForeignKey(n => n.UsuarioDestinatarioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NotificacaoLida>()
                .ToTable("NotificacoesLidas")
                .HasKey(l => new { l.NotificacaoId, l.UsuarioId });

            modelBuilder.Entity<NotificacaoLida>()
                .HasRequired(l => l.Notificacao)
                .WithMany()
                .HasForeignKey(l => l.NotificacaoId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<NotificacaoLida>()
                .HasRequired(l => l.Usuario)
                .WithMany()
                .HasForeignKey(l => l.UsuarioId)
                .WillCascadeOnDelete(false);
        }
    }
}