using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reverse.Models
{
    public enum FlagType
    {
        Importado = 1,
        MercadoLivre = 2,
        Amazon = 3,
        Variados = 4,
        SemAlteracao = 5
    }

    public class Produto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string CodigoBarras { get; set; }

        [Required]
        [StringLength(300)]
        public string Descricao { get; set; }
        [StringLength(50)]
        public string UsuarioUltimaAlteracao { get; set; }
        [StringLength(50)]
        public string UsuarioCriacao { get; set; }
        public string Modelo { get; set; }
        public decimal ValorUnitario { get; set; }
        public int Quantidade { get; set; }
        public bool Perecivel { get; set; }
        public DateTime? DataValidade { get; set; }
        public DateTime Emissao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public FlagType Flag { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
        public ICollection<ItemPalete> ItensPalete { get; set; }
    }
}