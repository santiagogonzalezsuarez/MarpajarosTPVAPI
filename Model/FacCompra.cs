using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class FacCompra
    {
        public FacCompra()
        {
            FacComprasArticulos = new HashSet<FacComprasArticulo>();
            FacComprasConceptos = new HashSet<FacComprasConcepto>();
        }

        public int Id { get; set; }
        public decimal? ImporteTotal { get; set; }
        public DateTime? Fecha { get; set; }
        public string NumeroFactura { get; set; }
        public int? ProveedorId { get; set; }

        public virtual TpvProveedore Proveedor { get; set; }
        public virtual ICollection<FacComprasArticulo> FacComprasArticulos { get; set; }
        public virtual ICollection<FacComprasConcepto> FacComprasConceptos { get; set; }
    }
}
