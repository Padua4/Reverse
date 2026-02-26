using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reverse.Models
{
    [Table("CategoriaPalete")]
    public class CategoriaPalete
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        [Required]
        public bool Ativo { get; set; }

        public CategoriaPalete()
        {
            DataCriacao = DateTime.Now;
            Ativo = true;
        }
    }
}