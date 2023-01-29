using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class FacComprasArticulo
    {
        public int Id { get; set; }
        public int? CompraId { get; set; }
        public int? ArticuloId { get; set; }
        public int? Cantidad { get; set; }
        public decimal? Importe { get; set; }

        public virtual TpvArticulo Articulo { get; set; }
        public virtual FacCompra Compra { get; set; }
    }
}
