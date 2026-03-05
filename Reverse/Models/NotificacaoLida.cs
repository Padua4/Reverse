using System;

namespace Reverse.Models
{
    public class NotificacaoLida
    {
        public int NotificacaoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataLeitura { get; set; }

        // Propriedades de navegação
        public virtual Notificacao Notificacao { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}