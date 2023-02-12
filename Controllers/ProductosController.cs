using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using MarpajarosTPVAPI.Classes;
using MarpajarosTPVAPI.Business;
using MarpajarosTPVAPI.Model;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MarpajarosTPVAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {

        private IWebHostEnvironment env;
        public ProductosController(IWebHostEnvironment _env) {
            this.env = _env;
        }

        #region Funciones

        [ActionName("getProductos")]
        [HttpPost]
        [APIReturn(typeof(Paging<GetProductosResult>))]
        public ActionResult GetProductos(GetProductosRequest request)
        {

            try 
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Productos_VisualizarProductos() || bs.AdmPermiso.Productos_ModificarYEliminarProductos())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                IQueryable<TpvVistaProducto> result = bs.TpvVistaProducto.getAll();

                // Filtro

                // Proveedor
                int filtro_ProveedorIdInt = 0;
                if (request.filtro_ProveedorId != null && Int32.TryParse(request.filtro_ProveedorId, out filtro_ProveedorIdInt)) {
                    string proveedorFiltroString = $"#{filtro_ProveedorIdInt}#";
                    result = result.Where(p => p.ProveedoresIds.Contains(proveedorFiltroString));
                }

                // Búsqueda
                if (request.filtro_Search != null && request.filtro_Search != "")
                {
                    var palabras = request.filtro_Search.Split(" ").Where(p => p.Length > 0).ToList();
                    foreach (var palabra in palabras)
                    {
                        result = result.Where(p => p.Proveedor.Contains(palabra) || p.Referencias.Contains(palabra) || p.CodigoBarras.Contains(palabra) || p.Producto.Contains(palabra) || p.Categorias.Contains(palabra) || p.Marca.Contains(palabra));
                    }
                }

                // Ordenación
                switch (request.sort)
                {
                    case "Categorias":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.Categorias).ThenByDescending(p => p.ArticuloId);
                        } else {
                            result = result.OrderBy(p => p.Categorias).ThenBy(p => p.ArticuloId);
                        }
                        break;
                    case "Producto":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.Producto).ThenByDescending(p => p.ArticuloId);
                        } else {
                            result = result.OrderBy(p => p.Producto).ThenBy(p => p.ArticuloId);
                        }
                        break;
                    case "Stock":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.Stock).ThenByDescending(p => p.ArticuloId);
                        } else {
                            result = result.OrderBy(p => p.Stock).ThenBy(p => p.ArticuloId);
                        }
                        break;
                    case "Marca":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.Marca).ThenByDescending(p => p.ArticuloId);
                        } else {
                            result = result.OrderBy(p => p.Marca).ThenBy(p => p.ArticuloId);
                        }
                        break;
                    case "Referencias":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.Referencias).ThenByDescending(p => p.ArticuloId);
                        } else {
                            result = result.OrderBy(p => p.Referencias).ThenBy(p => p.ArticuloId);
                        }
                        break;
                    case "PrecioVenta":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.PrecioVenta).ThenByDescending(p => p.ArticuloId);
                        } else {
                            result = result.OrderBy(p => p.PrecioVenta).ThenBy(p => p.ArticuloId);
                        }
                        break;
                    default:
                        result = ReflectionQueryable.OrderByProperty(result, request.sort, request.dir == "DESC", false);
                        break;
                }

                // Paginación
                int total = 0;
                if (result != null)
                {
                    total = result.Count();
                    result = result.Skip(request.start).Take(request.limit);
                }

                // Transformación de campos
                var lista = new List<GetProductosResult>();
                var page = (int)(Math.Floor((decimal)request.start / (decimal)request.limit) + 1);
                if (result != null)
                {

                    lista = (from p in result.ToList()
                            select new GetProductosResult
                            {
                                Id = p.ArticuloId,
                                Categorias = p.Categorias,
                                Producto = p.Producto,
                                Stock = p.Stock,
                                Marca = p.Marca,
                                Referencia = p.Referencias,
                                PrecioCompra = bs.TpvArticulosProducto.getById(p.ArticuloId).Articulo.TpvArticulosProveedores.Min(q => q.PrecioCompra),
                                PrecioVenta = p.PrecioVenta
                            }).ToList();

                }

                // Return
                return ResultClass.WithContent(new Paging<GetProductosResult>()
                {
                    collection = lista,
                    count = lista.Count(),
                    page = page,
                    total = total
                });

            }
            catch (Exception ex) 
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("getImagen")]
        [HttpPost]
        public ActionResult GetImagen(GetImagenProductoRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Productos_VisualizarProductos() || bs.AdmPermiso.Productos_ModificarYEliminarProductos())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                var producto = bs.TpvArticulo.getById(request.ProductoId);
                if (producto == null) {
                    return ResultClass.NotFound("Producto no encontrado.");
                }
                if (producto.Imagen == null) {
                    return ResultClass.NotFound("El producto no tiene una imagen asociada.");
                }

                var fileInfo = new FileInfo(Path.Combine(env.ContentRootPath, "Uploads/Productos", producto.Imagen));
                if (fileInfo.Exists) {
                    return ResultClass.WithFile(fileInfo.FullName);
                } else {
                    return ResultClass.WithError("El archivo no existe.");
                }

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("getProducto")]
        [HttpPost]
        [APIReturn(typeof(ProductoModel))]
        public ActionResult GetProducto(GetProductoRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Productos_VisualizarProductos() || bs.AdmPermiso.Productos_ModificarYEliminarProductos())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Obtenemos el registro
                var result = bs.TpvArticulo.getById(request.Id);

                // Transformación de campos
                var item = new ProductoModel(){
                    Id = result.Id,
                    Producto = result.TpvArticulosProducto.Producto,
                    CodigoBarras = result.TpvArticulosProducto.CodigoBarras,
                    IVA = result.TpvArticulosProducto.Iva,
                    PrecioVenta = result.PrecioVenta,
                    CategoriaId = result.TpvArticulosProducto.CategoriaId,
                    Marca = result.TpvArticulosProducto.Marca,
                    Animales = result.TpvArticulosProducto.Animales,
                    StockMinimo = result.TpvArticulosProducto.StockMinimo,
                    Proveedores = result.TpvArticulosProveedores.Select(p => new ProductoProveedorModel(){
                        ProveedorId = p.ProveedorId,
                        Referencia = p.Referencia,
                        PrecioCompra = p.PrecioCompra,
                        PorcentajeGanancia = p.PorcentajeGanancia,
                        Observaciones = p.Observaciones
                    }).ToList()
                };

                // Return
                return ResultClass.WithContent(item);

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("saveProducto")]
        [HttpPost]
        [APIReturn(typeof(ProductoModel))]
        public ActionResult SaveProducto(ProductoModel request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Productos_ModificarYEliminarProductos())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Obtenemos el registro de base de datos. Si es nuevo lo creamos
                TpvArticulo result = null;
                if (request.Id == 0) {
                    result = new TpvArticulo();
                    bs.TpvArticulo.insert(result);
                } else {
                    result = bs.TpvArticulo.getById(request.Id);
                    if (result == null)
                        return ResultClass.WithError($"No se ha encontrado el producto con el Id {request.Id}.");
                }

                // Validamos que estén rellenos todos los campos obligatorios.
                if (String.IsNullOrEmpty(request.Producto)) return ResultClass.WithError("El campo Nombre del producto es obligatorio.");
                if (request.CategoriaId == null || request.CategoriaId < 1) return ResultClass.WithError("El campo Categoría es obligatorio.");
                var categoria = bs.TpvCategoria.getById(request.CategoriaId.GetValueOrDefault(0));
                if (categoria == null) return ResultClass.WithError("La categoría seleccionada no es válida.");
                if (request.PrecioVenta == null) return ResultClass.WithError("El precio de venta tiene que ser numérico.");
                if (request.IVA == null) return ResultClass.WithError("Debe seleccionar un IVA.");
                if (request.Proveedores != null && request.Proveedores.GroupBy(p => p.ProveedorId).Any(p => p.Count() > 1)) return ResultClass.WithError("Existen proveedores duplicados. Solo puede haber una línea de proveedor por cada proveedor del producto. Elimine los sobrantes.");
                
                if (result.TpvArticulosProducto == null) {
                    var tpvArticuloProducto = new TpvArticulosProducto();
                    result.TpvArticulosProducto = tpvArticuloProducto;
                }

                // Actualizamos valores.
                result.TpvArticulosProducto.Producto = request.Producto;
                result.TpvArticulosProducto.CodigoBarras = request.CodigoBarras;
                result.TpvArticulosProducto.Iva = request.IVA;
                result.PrecioVenta = request.PrecioVenta;
                result.TpvArticulosProducto.CategoriaId = request.CategoriaId;
                result.TpvArticulosProducto.Marca = request.Marca;
                result.TpvArticulosProducto.Animales = request.Animales;
                result.TpvArticulosProducto.StockMinimo = request.StockMinimo;

                // Proveedores a eliminar.
                var proveedoresIds = request.Proveedores.Select(p => p.ProveedorId).ToList();
                var proveedoresEliminar = result.TpvArticulosProveedores.Where(p => !proveedoresIds.Contains(p.ProveedorId)).ToList();
                foreach (var proveedor in proveedoresEliminar)
                {
                    bs.TpvArticulosProveedore.delete(proveedor);
                }

                // Añadimos / modificamos proveedores
                foreach (var proveedor in request.Proveedores)
                {
                    var proveedorBD = result.TpvArticulosProveedores.Where(p => p.ProveedorId == proveedor.ProveedorId).FirstOrDefault();
                    if (proveedorBD == null) {
                        proveedorBD = new TpvArticulosProveedore();
                        result.TpvArticulosProveedores.Add(proveedorBD);
                    }
                    proveedorBD.ProveedorId = proveedor.ProveedorId.GetValueOrDefault(0);
                    proveedorBD.Referencia = proveedor.Referencia;
                    proveedorBD.PrecioCompra = proveedor.PrecioCompra;
                    proveedorBD.PorcentajeGanancia = proveedor.PorcentajeGanancia;
                    proveedorBD.Observaciones = proveedor.Observaciones;
                }

                // Guardamos los cambios.
                bs.save();

                // Construimos y devolvemos el objeto resultante.
                return ResultClass.WithContent(new ProductoModel() {
                    Id = result.Id,
                    Producto = result.TpvArticulosProducto.Producto,
                    CodigoBarras = result.TpvArticulosProducto.CodigoBarras,
                    IVA = result.TpvArticulosProducto.Iva,
                    PrecioVenta = result.PrecioVenta,
                    CategoriaId = result.TpvArticulosProducto.CategoriaId,
                    Marca = result.TpvArticulosProducto.Animales,
                    StockMinimo = result.TpvArticulosProducto.StockMinimo,
                    Proveedores = result.TpvArticulosProveedores.Select(p => new ProductoProveedorModel(){
                        ProveedorId = p.ProveedorId,
                        Referencia = p.Referencia,
                        PrecioCompra = p.PrecioCompra,
                        PorcentajeGanancia = p.PorcentajeGanancia,
                        Observaciones = p.Observaciones
                    }).ToList()
                });

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("deleteProductos")]
        [HttpPost]
        [APIReturn(typeof(bool))]
        public ActionResult DeleteProductos(DeleteProductosRequest request)
        {
        
            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Productos_ModificarYEliminarProductos()))
                {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Eliminamos productos
                List<TpvArticulo> productosEliminar = bs.TpvArticulo.getAll().Where(p => request.ProductosIds.Contains(p.Id)).ToList();
                foreach (var producto in productosEliminar)
                {
                    producto.Borrado = true;
                }

                // Guardamos.
                bs.save();
                return ResultClass.WithContent(true);

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        #endregion

        #region Clases

        public class GetProductosRequest
        {
            public int start;
            public int limit;
            public string sort;
            public string dir;
            public string filtro_Search;
            public string filtro_ProveedorId;
        }

        public class GetProductosResult
        {
            public int Id;
            public string Categorias;
            public string Producto;
            public decimal? Stock;
            public string Marca;
            public string Referencia;
            public decimal? PrecioCompra;
            public decimal? PrecioVenta;

        }

        public class GetImagenProductoRequest
        {
            public int ProductoId;
        }

        public class GetProductoRequest
        {
            public int Id;
        }

        public class ProductoModel
        {
            public int Id;
            public string Producto;
            public string CodigoBarras;
            public decimal? IVA;
            public decimal? PrecioVenta;
            public int? CategoriaId;
            public string Marca;
            public string Animales;
            public int? StockMinimo;
            public List<ProductoProveedorModel> Proveedores;
        }

        public class ProductoProveedorModel
        {
            public int? ProveedorId;
            public string Referencia;
            public decimal? PrecioCompra;
            public decimal? PorcentajeGanancia;
            public string Observaciones;
        }

        public class DeleteProductosRequest
        {
            public List<int> ProductosIds;
        }

        #endregion

    }
}