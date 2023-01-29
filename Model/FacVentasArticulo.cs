using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class FacVentasArticulo
    {
        public long Id { get; set; }
        public long? VentaId { get; set; }
        public int? ArticuloId { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Importe { get; set; }

        public virtual TpvArticulo Articulo { get; set; }
        public virtual FacVenta Venta { get; set; }
    }
}
