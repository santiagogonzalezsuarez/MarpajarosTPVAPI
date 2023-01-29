using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class TpvVistaProducto
    {
        public int ArticuloId { get; set; }
        public string Categorias { get; set; }
        public string Proveedor { get; set; }
        public string Referencias { get; set; }
        public string CodigoBarras { get; set; }
        public string Producto { get; set; }
        public string Marca { get; set; }
        public decimal? PrecioVenta { get; set; }
        public string ProveedoresIds { get; set; }
        public decimal? Stock { get; set; }
        public int? StockMinimo { get; set; }
    }
}
