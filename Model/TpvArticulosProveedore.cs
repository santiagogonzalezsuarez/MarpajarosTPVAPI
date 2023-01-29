using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class TpvArticulosProveedore
    {
        public int ArticuloId { get; set; }
        public int ProveedorId { get; set; }
        public decimal? PrecioCompra { get; set; }
        public string Referencia { get; set; }
        public string Observaciones { get; set; }
        public decimal? PorcentajeGanancia { get; set; }

        public virtual TpvArticulo Articulo { get; set; }
        public virtual TpvProveedore Proveedor { get; set; }
    }
}
