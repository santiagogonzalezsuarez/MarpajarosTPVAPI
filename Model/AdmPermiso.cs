using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class AdmPermiso
    {
        public AdmPermiso()
        {
            AdmRolesPermisos = new HashSet<AdmRolesPermiso>();
        }

        public int Id { get; set; }
        public string Permiso { get; set; }
        public int? ModuloId { get; set; }

        public virtual AdmModulo Modulo { get; set; }
        public virtual ICollection<AdmRolesPermiso> AdmRolesPermisos { get; set; }
    }
}
