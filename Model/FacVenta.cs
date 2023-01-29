using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class FacVenta
    {
        public FacVenta()
        {
            FacVentasArticulos = new HashSet<FacVentasArticulo>();
            FacVentasConceptos = new HashSet<FacVentasConcepto>();
        }

        public long Id { get; set; }
        public decimal? ImporteTotal { get; set; }
        public DateTime? Fecha { get; set; }
        public bool? Borrado { get; set; }
        public int? CajaId { get; set; }
        public int? Nticket { get; set; }

        public virtual ICollection<FacVentasArticulo> FacVentasArticulos { get; set; }
        public virtual ICollection<FacVentasConcepto> FacVentasConceptos { get; set; }
    }
}
