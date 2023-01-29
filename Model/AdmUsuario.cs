using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class AdmUsuario
    {
        public AdmUsuario()
        {
            AdmUsuariosAccesos = new HashSet<AdmUsuariosAcceso>();
            TpvAperturasCajons = new HashSet<TpvAperturasCajon>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? RolId { get; set; }

        public virtual AdmRole Rol { get; set; }
        public virtual ICollection<AdmUsuariosAcceso> AdmUsuariosAccesos { get; set; }
        public virtual ICollection<TpvAperturasCajon> TpvAperturasCajons { get; set; }
    }
}
