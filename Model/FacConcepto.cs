using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class FacConcepto
    {
        public FacConcepto()
        {
            FacComprasConceptos = new HashSet<FacComprasConcepto>();
            FacVentasConceptos = new HashSet<FacVentasConcepto>();
        }

        public int Id { get; set; }
        public string Concepto { get; set; }

        public virtual ICollection<FacComprasConcepto> FacComprasConceptos { get; set; }
        public virtual ICollection<FacVentasConcepto> FacVentasConceptos { get; set; }
    }
}
