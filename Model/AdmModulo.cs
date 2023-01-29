using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class AdmModulo
    {
        public AdmModulo()
        {
            AdmPermisos = new HashSet<AdmPermiso>();
        }

        public int Id { get; set; }
        public string Modulo { get; set; }

        public virtual ICollection<AdmPermiso> AdmPermisos { get; set; }
    }
}
