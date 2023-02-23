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

                var categorias = bs.TpvCategoria.getAll().Select(p => new {
                    Id = p.Id,
                    Categoria = p.Categoria,
                    PadreId = p.CategoriaPadreId
                }).ToList();
                var listaCategorias = new List<CategoriasClass>();
                foreach (var categoria in categorias)
                {
                    var nombreCategoria = "";
                    var categoriaItem = categoria;
                    while (categoriaItem != null) {
                        if (nombreCategoria == "") {
                            nombreCategoria = categoriaItem.Categoria;
                        } else {
                            nombreCategoria = $"{categoriaItem.Categoria} > {nombreCategoria}";
                        }
                        if (categoriaItem.PadreId == null) {
                            categoriaItem = null;
                        } else {
                            categoriaItem = categorias.Where(p => p.Id == categoriaItem.PadreId).FirstOrDefault();
                        }
                    }
                    listaCategorias.Add(new CategoriasClass {
                        Id = categoria.Id,
                        NombreCompleto = nombreCategoria
                    });
                }
                
                IQueryable<TpvArticulo> result = bs.TpvArticulo.getAll();

                // Filtro

                // Proveedor
                int filtro_ProveedorIdInt = 0;
                if (request.filtro_ProveedorId != null && Int32.TryParse(request.filtro_ProveedorId, out filtro_ProveedorIdInt)) {
                    result = result.Where(p => p.TpvArticulosProveedores.Any(q => q.ProveedorId == filtro_ProveedorIdInt));
                }

                // Búsqueda
                if (request.filtro_Search != null && request.filtro_Search != "")
                {
                    var palabras = request.filtro_Search.Split(" ").Where(p => p.Length > 0).ToList();
                    foreach (var palabra in palabras)
                    {
                        var categoriasIdsMatches = listaCategorias.Where(p => p.NombreCompleto.ToLower().Contains(palabra.ToLower())).Select(p => (int?)p.Id).ToList();
                        result = result.Where(p => p.TpvArticulosProveedores.Any(q => q.Proveedor.Nombre.Contains(palabra) || p.TpvArticulosProducto.CodigoBarras.Contains(palabra) || p.TpvArticulosProducto.Producto.Contains(palabra) || p.TpvArticulosProducto.Marca.Contains(palabra) || categoriasIdsMatches.Contains(p.TpvArticulosProducto.CategoriaId)));
                    }
                }

                // Ordenación
                switch (request.sort)
                {
                    case "Categorias":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.TpvArticulosProducto.Categoria).ThenByDescending(p => p.Id);
                        } else {
                            result = result.OrderBy(p => p.TpvArticulosProducto.Categoria).ThenBy(p => p.Id);
                        }
                        break;
                    case "Producto":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.TpvArticulosProducto.Producto).ThenByDescending(p => p.Id);
                        } else {
                            result = result.OrderBy(p => p.TpvArticulosProducto.Producto).ThenBy(p => p.Id);
                        }
                        break;
                    case "Stock":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.Stock).ThenByDescending(p => p.Id);
                        } else {
                            result = result.OrderBy(p => p.Stock).ThenBy(p => p.Id);
                        }
                        break;
                    case "Marca":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.TpvArticulosProducto.Marca).ThenByDescending(p => p.Id);
                        } else {
                            result = result.OrderBy(p => p.TpvArticulosProducto.Marca).ThenBy(p => p.Id);
                        }
                        break;
                    case "Referencias":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.TpvArticulosProveedores.Select(q => q.Referencia).FirstOrDefault()).ThenByDescending(p => p.Id);
                        } else {
                            result = result.OrderBy(p => p.TpvArticulosProveedores.Select(q => q.Referencia).FirstOrDefault()).ThenBy(p => p.Id);
                        }
                        break;
                    case "PrecioCompra":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => (p.TpvArticulosProveedores.Any(q => q.PrecioCompra != null) ? p.TpvArticulosProveedores.Where(q => q.PrecioCompra != null).Min(q => q.PrecioCompra) : 0)).ThenByDescending(p => p.Id);
                        } else {
                            result = result.OrderBy(p => (p.TpvArticulosProveedores.Any(q => q.PrecioCompra != null) ? p.TpvArticulosProveedores.Where(q => q.PrecioCompra != null).Min(q => q.PrecioCompra) : 0)).ThenBy(p => p.Id);
                        }
                        break;
                    case "PrecioVenta":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.PrecioVenta).ThenByDescending(p => p.Id);
                        } else {
                            result = result.OrderBy(p => p.PrecioVenta).ThenBy(p => p.Id);
                        }
                        break;
                    default:
                        result = ReflectionQueryable.OrderByProperty(result, request.sort, request.dir == "DESC", true);
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
                    var listaQuery = (from p in result
                                      select new {
                                          Id = p.Id,
                                          CategoriaId = p.TpvArticulosProducto.CategoriaId,
                                          Producto = p.TpvArticulosProducto.Producto,
                                          Stock = p.Stock,
                                          Marca = p.TpvArticulosProducto.Marca,
                                          Referencia = p.TpvArticulosProveedores.Select(q => q.Referencia),
                                          PrecioCompra = p.TpvArticulosProveedores.Min(q => q.PrecioCompra),
                                          PrecioVenta = p.PrecioVenta
                                      }).ToList();
                    
                    lista = (from p in listaQuery
                             select new GetProductosResult
                             {
                                Id = p.Id,
                                Categorias = listaCategorias.Where(q => q.Id == p.CategoriaId).Select(q => q.NombreCompleto).FirstOrDefault(),
                                Producto = p.Producto,
                                Stock = p.Stock,
                                Marca = p.Marca,
                                Referencia = String.Join(", ", p.Referencia.ToArray()),
                                PrecioCompra = p.PrecioCompra,
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

        [ActionName("actualizarImagen")]
        [HttpPost]
        [APIReturn(typeof(bool))]
        public ActionResult ActualizarImagen(ActualizarImagenRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Productos_ModificarYEliminarProductos())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Obtenemos el registro de base de datos.
                var producto = bs.TpvArticulo.getById(request.Id);
                if (producto == null)
                    return ResultClass.WithError($"Producto no encontrado: {request.Id}");

                var justFileName = new FileInfo(request.fileName).Name; // Quitamos la ruta.
                if (justFileName.Contains(".")) justFileName = justFileName.Substring(0, justFileName.LastIndexOf("."));
                var extension = new FileInfo(request.fileName).Extension;
                var fileInfo = new FileInfo(Path.Combine(env.ContentRootPath, "Uploads/Productos", $"{producto.Id}_{justFileName}{extension}"));
                int counter = 1;
                while (fileInfo.Exists) {
                    counter++;
                    fileInfo = new FileInfo(Path.Combine(env.ContentRootPath, "Uploads/Productos", $"{producto.Id}_{justFileName}{counter}{extension}"));
                }

                if (!fileInfo.Directory.Exists) {
                    fileInfo.Directory.Create();
                }

                // Guardamos la imagen.
                System.IO.File.WriteAllBytes(fileInfo.FullName, Convert.FromBase64String(request.base64Image));
                producto.Imagen = fileInfo.Name;
                bs.save();

                return ResultClass.WithContent(true);

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("eliminarImagen")]
        [HttpPost]
        [APIReturn(typeof(bool))]
        public ActionResult EliminarImagen(EliminarImagenRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Productos_ModificarYEliminarProductos())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Obtenemos el registro de base de datos.
                var producto = bs.TpvArticulo.getById(request.Id);
                if (producto == null)
                    return ResultClass.WithError($"Producto no encontrado: {request.Id}");

                producto.Imagen = null;
                bs.save();

                return ResultClass.WithContent(true);

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

        public class ActualizarImagenRequest
        {
            public int Id;
            public string fileName;
            public string base64Image;
        }

        public class EliminarImagenRequest
        {
            public int Id;
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

        public class CategoriasClass
        {
            public int Id;
            public string NombreCompleto;
        }

        #endregion

    }
}