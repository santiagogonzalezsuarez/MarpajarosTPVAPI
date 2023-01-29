using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class AdmRolesPermiso
    {
        public int PermisoId { get; set; }
        public int RolId { get; set; }

        public virtual AdmPermiso Permiso { get; set; }
        public virtual AdmRole Rol { get; set; }
    }
}
