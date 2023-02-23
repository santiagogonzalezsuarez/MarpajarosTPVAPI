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
    public class ProveedoresController : ControllerBase
    {

        #region Funciones

        [ActionName("getProveedoresCombo")]
        [HttpPost]
        [APIReturn(typeof(List<GetProveedoresComboResult>))]
        public ActionResult GetProveedoresCombo(GetProveedoresComboRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Productos_ModificarYEliminarProductos())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                IQueryable<TpvProveedore> result = bs.TpvProveedore.getAll();

                // Ordenación
                result = result.OrderBy(p => p.Nombre);

                // Transformación de campos
                var lista = new List<GetProveedoresComboResult>();
                if (result != null)
                {

                    lista = (from p in result.ToList()
                             select new GetProveedoresComboResult
                             {
                                Id = p.Id,
                                Proveedor = p.Nombre
                             }).ToList();

                }

                // Return
                return ResultClass.WithContent(lista);

            }
            catch (Exception ex) {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("getProveedores")]
        [HttpPost]
        [APIReturn(typeof(Paging<GetProveedoresResult>))]
        public ActionResult GetProveedores(GetProveedoresRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Proveedores_AccesoListadoDeProveedores() || bs.AdmPermiso.Proveedores_ModificarProveedores())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                IQueryable<TpvProveedore> result = bs.TpvProveedore.getAll();

                // Filtro

                // Búsqueda
                if (request.filtro_Search != null && request.filtro_Search != "")
                {
                    var palabras = request.filtro_Search.Split(" ").Where(p => p.Length > 0).ToList();
                    foreach (var palabra in palabras)
                    {
                        result = result.Where(p => p.Nombre.Contains(palabra) || p.Telefono.Contains(palabra) || p.Email.Contains(palabra) || p.PaginaWeb.Contains(palabra));
                    }
                }

                // Ordenación
                switch (request.sort)
                {
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
                var lista = new List<GetProveedoresResult>();
                var page = (int)(Math.Floor((decimal)request.start / (decimal)request.limit) + 1);
                if (result != null)
                {

                    lista = (from p in result.ToList()
                            select new GetProveedoresResult
                            {
                                Id = p.Id,
                                Nombre = p.Nombre,
                                Telefono = p.Telefono,
                                Email = p.Email,
                                PaginaWeb = p.PaginaWeb
                            }).ToList();

                }

                // Return
                return ResultClass.WithContent(new Paging<GetProveedoresResult>()
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

        [ActionName("getProveedor")]
        [HttpPost]
        [APIReturn(typeof(ProveedorModel))]
        public ActionResult GetProveedor(GetProveedorRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Proveedores_AccesoListadoDeProveedores() || bs.AdmPermiso.Proveedores_ModificarProveedores())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Obtenemos el registro
                var result = bs.TpvProveedore.getById(request.Id);

                // Transformación de campos
                var item = new ProveedorModel(){
                    Id = result.Id,
                    Nombre = result.Nombre,
                    Telefono = result.Telefono,
                    Email = result.Email,
                    PaginaWeb = result.PaginaWeb,
                    Direccion = result.Direccion,
                    Horarios = result.Horarios,
                    Observaciones = result.Observaciones,
                    MostrarEnInformeVentas = result.MostrarEnInformeVentas.GetValueOrDefault(false)
                };

                // Return
                return ResultClass.WithContent(item);

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("saveProveedor")]
        [HttpPost]
        [APIReturn(typeof(ProveedorModel))]
        public ActionResult SaveProveedor(ProveedorModel request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Proveedores_ModificarProveedores())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Obtenemos el registro de base de datos. Si es nuevo lo creamos.
                TpvProveedore result = null;
                if (request.Id == 0) {
                    result = new TpvProveedore();
                    bs.TpvProveedore.insert(result);
                } else {
                    result = bs.TpvProveedore.getById(request.Id);
                    if (result == null)
                        return ResultClass.WithError($"No se ha encontrado el proveedor con el Id {request.Id}.");
                }

                // Validamos que estén rellenos todos los campos obligatorios.
                if (String.IsNullOrEmpty(request.Nombre)) return ResultClass.WithError("El campo Nombre del proveedor es obligatorio.");

                // Actualizamos valores.
                result.Nombre = request.Nombre;
                result.Telefono = request.Telefono;
                result.Email = request.Email;
                result.PaginaWeb = request.PaginaWeb;
                result.Direccion = request.Direccion;
                result.Horarios = request.Horarios;
                result.Observaciones = request.Observaciones;
                result.MostrarEnInformeVentas = request.MostrarEnInformeVentas;

                // Guardamos los cambios.
                bs.save();

                // Construimos y devolvemos el objeto resultante.
                return ResultClass.WithContent(new ProveedorModel() {
                    Id = result.Id,
                    Nombre = result.Nombre,
                    Telefono = result.Telefono,
                    Email = result.Email,
                    PaginaWeb = result.PaginaWeb,
                    Direccion = result.Direccion,
                    Horarios = result.Horarios,
                    Observaciones = result.Observaciones,
                    MostrarEnInformeVentas = result.MostrarEnInformeVentas.GetValueOrDefault(false)
                });

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("deleteProveedores")]
        [HttpPost]
        [APIReturn(typeof(bool))]
        public ActionResult DeleteProveedores(DeleteProveedoresRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Proveedores_ModificarProveedores()))
                {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Eliminamos proveedores.
                List<TpvProveedore> proveedoresEliminar = bs.TpvProveedore.getAll().Where(p => request.ProveedoresIds.Contains(p.Id)).ToList();
                foreach (var proveedor in proveedoresEliminar) 
                {
                    if (proveedor.TpvArticulosProveedores.Any()) {
                        var producto = proveedor.TpvArticulosProveedores.FirstOrDefault().Articulo?.TpvArticulosProducto?.Producto;
                        return ResultClass.WithError($"El proveedor {proveedor.Nombre} no se puede eliminar porque el producto {producto} lo tiene asociado.");
                    }
                }
                foreach (var proveedor in proveedoresEliminar)
                {
                    bs.TpvProveedore.delete(proveedor);
                }

                // Guardamos
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

        public class GetProveedoresComboRequest
        {
        }

        public class GetProveedoresComboResult
        {
            public int Id;
            public string Proveedor;
        }

        public class GetProveedoresRequest
        {
            public int start;
            public int limit;
            public string sort;
            public string dir;
            public string filtro_Search;
        }

        public class GetProveedoresResult
        {
            public int Id;
            public string Nombre;
            public string Telefono;
            public string Email;
            public string PaginaWeb;
        }

        public class GetProveedorRequest
        {
            public int Id;
        }

        public class ProveedorModel
        {
            public int Id;
            public string Nombre;
            public string Telefono;
            public string Email;
            public string PaginaWeb;
            public string Direccion;
            public string Horarios;
            public string Observaciones;
            public bool MostrarEnInformeVentas;
        }

        public class DeleteProveedoresRequest
        {
            public List<int> ProveedoresIds;
        }

        #endregion

    }
}