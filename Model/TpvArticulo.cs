using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class TpvArticulo
    {
        public TpvArticulo()
        {
            FacComprasArticulos = new HashSet<FacComprasArticulo>();
            FacVentasArticulos = new HashSet<FacVentasArticulo>();
            TpvArticulosProveedores = new HashSet<TpvArticulosProveedore>();
            TpvStocks = new HashSet<TpvStock>();
        }

        public int Id { get; set; }
        public decimal? PrecioVenta { get; set; }
        public int? TipoId { get; set; }
        public bool? Borrado { get; set; }

        public virtual TpvTiposArticulo Tipo { get; set; }
        public virtual TpvArticulosAnimale TpvArticulosAnimale { get; set; }
        public virtual TpvArticulosProducto TpvArticulosProducto { get; set; }
        public virtual ICollection<FacComprasArticulo> FacComprasArticulos { get; set; }
        public virtual ICollection<FacVentasArticulo> FacVentasArticulos { get; set; }
        public virtual ICollection<TpvArticulosProveedore> TpvArticulosProveedores { get; set; }
        public virtual ICollection<TpvStock> TpvStocks { get; set; }
    }
}
