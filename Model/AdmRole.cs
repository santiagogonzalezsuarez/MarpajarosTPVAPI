using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class AdmRole
    {
        public AdmRole()
        {
            AdmRolesPermisos = new HashSet<AdmRolesPermiso>();
            AdmUsuarios = new HashSet<AdmUsuario>();
        }

        public int Id { get; set; }
        public string Rol { get; set; }

        public virtual ICollection<AdmRolesPermiso> AdmRolesPermisos { get; set; }
        public virtual ICollection<AdmUsuario> AdmUsuarios { get; set; }
    }
}
