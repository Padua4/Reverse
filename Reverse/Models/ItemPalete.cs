using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reverse.Models
{
    public class ItemPalete
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Palete))]
        public int PaleteId { get; set; }
        public virtual Palete Palete { get; set; }

        [Required]
        [ForeignKey(nameof(Produto))]
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }

        [StringLength(50)]
        public string CodigoBarras { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        [Required]
        public decimal ValorUnitario { get; set; }

        public FlagType Flag { get; set; }

        public bool AvisoVencimento { get; set; }
        public decimal? AvisoVencimentoValor { get; set; }

        [NotMapped]
        public string DescricaoProduto => Produto?.Descricao;
    }
}