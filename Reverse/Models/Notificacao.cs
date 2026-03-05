using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reverse.Models
{
    public class Notificacao
    {
        public int Id { get; set; }
        public int UsuarioRemetenteId { get; set; }
        public int? UsuarioDestinatarioId { get; set; }
        public string Mensagem { get; set; }
        public bool Lida { get; set; }
        public DateTime DataCriacao { get; set; }

        public virtual Usuario Remetente { get; set; }
        public virtual Usuario Destinatario { get; set; }

        [NotMapped]
        public string NomeRemetente { get; set; }

        [NotMapped]
        public string NomeDestinatario { get; set; }
    }
}