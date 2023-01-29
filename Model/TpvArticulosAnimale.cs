using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class TpvArticulosAnimale
    {
        public int ArticuloId { get; set; }
        public string ReferenciaNombre { get; set; }
        public string Especie { get; set; }
        public string Variedad { get; set; }
        public int? AnoNacimiento { get; set; }
        public string Nidentificacion { get; set; }
        public DateTime? FechaAdquisicion { get; set; }
        public string Procedencia { get; set; }
        public string ControlVeterinario { get; set; }
        public bool? Vendido { get; set; }

        public virtual TpvArticulo Articulo { get; set; }
    }
}
