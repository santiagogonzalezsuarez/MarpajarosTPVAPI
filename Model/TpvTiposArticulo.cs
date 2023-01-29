using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class TpvTiposArticulo
    {
        public TpvTiposArticulo()
        {
            TpvArticulos = new HashSet<TpvArticulo>();
        }

        public int Id { get; set; }
        public string Tipo { get; set; }

        public virtual ICollection<TpvArticulo> TpvArticulos { get; set; }
    }
}
