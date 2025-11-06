using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reverse.Models
{
    [Table("Materiais")]
    public class Material
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Valorizacao { get; set; }
    }
}
