using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class AdmUsuariosAcceso
    {
        public int Id { get; set; }
        public int? AdmUsuariosId { get; set; }
        public string Token { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Ipaddress { get; set; }

        public virtual AdmUsuario AdmUsuarios { get; set; }
    }
}
