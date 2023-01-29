using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class TpvStock
    {
        public long Id { get; set; }
        public DateTime? Fecha { get; set; }
        public int? Stock { get; set; }
        public int? ArticuloId { get; set; }
        public decimal? StockAnterior { get; set; }

        public virtual TpvArticulo Articulo { get; set; }
    }
}
