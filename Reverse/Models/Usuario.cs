using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reverse.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Index("IX_Usuario_UsuarioNome", IsUnique = true)]
        [StringLength(50)]
        public string UsuarioNome { get; set; }

        [Required]
        [Column(TypeName = "varbinary")]
        public byte[] PasswordHash { get; set; }

        [Required]
        [StringLength(50)]
        public string Setor { get; set; }

        [Required]
        public DateTime DataCadastro { get; set; }

        public Usuario()
        {
            DataCadastro = DateTime.UtcNow;
        }
    }
}
