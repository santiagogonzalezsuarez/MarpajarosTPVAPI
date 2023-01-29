using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class TpvProveedore
    {
        public TpvProveedore()
        {
            FacCompras = new HashSet<FacCompra>();
            TpvArticulosProveedores = new HashSet<TpvArticulosProveedore>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string PaginaWeb { get; set; }
        public string Direccion { get; set; }
        public string Horarios { get; set; }
        public string Observaciones { get; set; }
        public bool? MostrarEnInformeVentas { get; set; }

        public virtual ICollection<FacCompra> FacCompras { get; set; }
        public virtual ICollection<TpvArticulosProveedore> TpvArticulosProveedores { get; set; }
    }
}
