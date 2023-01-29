using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using MarpajarosTPVAPI.Classes;
using MarpajarosTPVAPI.Business;
using MarpajarosTPVAPI.Model;
using System.Collections.Generic;

namespace MarpajarosTPVAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {

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

        #endregion

    }
}