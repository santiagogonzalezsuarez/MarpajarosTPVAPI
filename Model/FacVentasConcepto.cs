using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class FacVentasConcepto
    {
        public long VentaId { get; set; }
        public int ConceptoId { get; set; }
        public decimal? Importe { get; set; }
        public int? Orden { get; set; }

        public virtual FacConcepto Concepto { get; set; }
        public virtual FacVenta Venta { get; set; }
    }
}
