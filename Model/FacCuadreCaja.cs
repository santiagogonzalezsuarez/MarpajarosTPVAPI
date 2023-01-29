using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class FacCuadreCaja
    {
        public long Id { get; set; }
        public DateTime? FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal? Descuadre { get; set; }
        public int? CajaId { get; set; }
        public decimal? ImporteApertura { get; set; }
        public decimal? ImporteCierre { get; set; }
    }
}
