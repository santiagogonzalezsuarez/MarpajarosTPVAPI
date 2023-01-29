using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class TpvCategoria
    {
        public TpvCategoria()
        {
            InverseCategoriaPadre = new HashSet<TpvCategoria>();
            TpvArticulosProductos = new HashSet<TpvArticulosProducto>();
        }

        public int Id { get; set; }
        public string Categoria { get; set; }
        public int? CategoriaPadreId { get; set; }
        public int? Orden { get; set; }

        public virtual TpvCategoria CategoriaPadre { get; set; }
        public virtual ICollection<TpvCategoria> InverseCategoriaPadre { get; set; }
        public virtual ICollection<TpvArticulosProducto> TpvArticulosProductos { get; set; }
    }
}
