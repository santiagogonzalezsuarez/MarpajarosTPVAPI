using System;
using System.Collections.Generic;

#nullable disable

namespace MarpajarosTPVAPI.Model
{
    public partial class TpvArticulosProducto
    {
        public int ArticuloId { get; set; }
        public string Producto { get; set; }
        public string CodigoBarras { get; set; }
        public int? CategoriaId { get; set; }
        public string Marca { get; set; }
        public string Animales { get; set; }
        public decimal? Iva { get; set; }
        public int? StockMinimo { get; set; }

        public virtual TpvArticulo Articulo { get; set; }
        public virtual TpvCategoria Categoria { get; set; }
    }
}
