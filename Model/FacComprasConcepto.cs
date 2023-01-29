using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class FacComprasConcepto
    {
        public int ComprasId { get; set; }
        public int ConceptoId { get; set; }
        public decimal? Importe { get; set; }
        public int? Orden { get; set; }

        public virtual FacCompra Compras { get; set; }
        public virtual FacConcepto Concepto { get; set; }
    }
}
