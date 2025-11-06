using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reverse.Models
{
    public class Permissao
    {
        public int PermissaoId { get; set; }
        public int UsuarioId { get; set; }
        public string FormName { get; set; }
        public bool PodeAcessar { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
