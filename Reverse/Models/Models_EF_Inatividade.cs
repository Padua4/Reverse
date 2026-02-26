using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reverse.Models
{
    /// <summary>
    /// Tabela para registrar atividades dos usuários
    /// </summary>
    [Table("AtividadesUsuarios")]
    public class AtividadeUsuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAtividade { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string NomeUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string NomeFormulario { get; set; }

        [Required]
        public DateTime DataHoraAtividade { get; set; }

        [StringLength(50)]
        public string TipoAcao { get; set; }

        [StringLength(500)]
        public string Detalhes { get; set; }

        // Relacionamento com Usuario
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
    }

    /// <summary>
    /// Tabela para controlar sessões ativas dos usuários
    /// </summary>
    [Table("SessoesUsuarios")]
    public class SessaoUsuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSessao { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string NomeUsuario { get; set; }

        [Required]
        public DateTime DataHoraLogin { get; set; }

        public DateTime? DataHoraLogout { get; set; }

        [Required]
        [StringLength(20)]
        public string StatusSessao { get; set; } // "Ativo" ou "Encerrado"

        [StringLength(100)]
        public string NomeMaquina { get; set; }

        [StringLength(50)]
        public string EnderecoIP { get; set; }

        // Relacionamento com Usuario
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
    }
}