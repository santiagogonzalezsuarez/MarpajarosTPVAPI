using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class AppConfiguracion
    {
        public int Id { get; set; }
        public int? RegistrosPorPagina { get; set; }
        public decimal? Iva { get; set; }
    }
}
