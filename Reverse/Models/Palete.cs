using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reverse.Models
{
    [Table("Palete")]
    public partial class Palete
    {
        public string Nome =>
            Categoria != null
                ? $"Palete {Numero} - {Categoria.Nome}"
                : $"Palete {Numero}";

        [Key]
        public int Id { get; set; }

        [Required]
        public int Numero { get; set; }

        [Required]
        [ForeignKey(nameof(Categoria))]
        public int CategoriaId { get; set; }
        public virtual CategoriaPalete Categoria { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        [Required]
        [StringLength(50)]
        public string UsuarioCriacao { get; set; }

        [Required]
        public int Status { get; set; }

        [StringLength(50)]
        public string UsuarioFinalizacao { get; set; }

        public DateTime? DataFinalizacao { get; set; }

        public virtual ICollection<ItemPalete> Itens { get; set; }

        public Palete()
        {
            DataCriacao = DateTime.Now;
            Itens = new HashSet<ItemPalete>();
        }
    }
}