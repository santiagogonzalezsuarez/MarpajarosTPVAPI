using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class TpvAperturasCajon
    {
        public long Id { get; set; }
        public DateTime? Fecha { get; set; }
        public int? UsuarioId { get; set; }

        public virtual AdmUsuario Usuario { get; set; }
    }
}
