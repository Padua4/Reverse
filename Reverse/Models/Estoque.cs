using System;

namespace Reverse.Models
{
    public class Estoque
    {
        public int Id { get; set; }
        public string Material { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? DataSaida { get; set; }
        public decimal Quantidade { get; set; }
        public string Status { get; set; }
        public int ClienteId { get; set; }
        public string Observacao { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public bool EhPeso { get; set; }

        public virtual Cliente Cliente { get; set; }
    }
}
