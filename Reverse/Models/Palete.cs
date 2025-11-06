using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Reverse.Models
{
    public enum CategoriaPalete
    {
        [Description("Eletronicos (Variados)")]
        Eletronicos_Variados,

        [Description("Eletronicos (127v)")]
        Eletronicos_127v,

        [Description("Eletronicos (220v)")]
        Eletronicos_220v,

        Alimentos,
        Moveis,
        Moda,
        Variados,
        [Description("Simples Conferência")]
        Simples_Conferencia,
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum valor)
        {
            var fi = valor.GetType().GetField(valor.ToString());
            var attr = fi.GetCustomAttributes(typeof(DescriptionAttribute), false)
                         .OfType<DescriptionAttribute>()
                         .FirstOrDefault();
            return attr != null ? attr.Description : valor.ToString();
        }
    }

    [Table("Palete")]
    public partial class Palete
    {
        public string Nome =>
         $"Palete {Numero} - {Categoria.GetDescription()}";

        [Key]
        public int Id { get; set; }

        [Required]
        public int Numero { get; set; }

        [Required]
        public CategoriaPalete Categoria { get; set; }

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
