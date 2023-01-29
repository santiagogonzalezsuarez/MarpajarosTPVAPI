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
    public class RolesController : ControllerBase
    {

        #region Funciones

        [ActionName("getRoles")]
        [HttpPost]
        [APIReturn(typeof(Paging<GetRolesResult>))]
        public ActionResult GetRoles(GetRolesRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Roles_VisualizarRoles() || bs.AdmPermiso.Roles_ModificarYEliminarRoles())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                IQueryable<AdmRole> result = bs.AdmRole.getAll();

                // Filtro

                // Búsqueda
                if (request.filtro_Search != null && request.filtro_Search != "")
                {
                    var palabras = request.filtro_Search.Split(" ").Where(p => p.Length > 0).ToList();
                    foreach (var palabra in palabras)
                    {
                        result = result.Where(p => p.Rol.Contains(palabra));
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
                var lista = new List<GetRolesResult>();
                var page = (int)(Math.Floor((decimal)request.start / (decimal)request.limit) + 1);
                if (result != null)
                {
                    
                    lista = (from p in result.ToList()
                             select new GetRolesResult 
                             {
                                Id = p.Id,
                                Rol = p.Rol
                             }).ToList();

                }

                // Return
                return ResultClass.WithContent(new Paging<GetRolesResult>()
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

        [ActionName("getRolesCombo")]
        [HttpPost]
        [APIReturn(typeof(List<GetRolesComboResult>))]
        public ActionResult GetRolesCombo(GetRolesComboRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Usuarios_ModificarYEliminarUsuarios())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                IQueryable<AdmRole> result = bs.AdmRole.getAll();

                // Ordenación
                result = result.OrderBy(p => p.Rol);

                // Transformación de campos
                var lista = new List<GetRolesComboResult>();
                if (result != null)
                {
                    
                    lista = (from p in result.ToList()
                             select new GetRolesComboResult
                             {
                                Id = p.Id,
                                Rol = p.Rol
                             }).ToList();

                }

                // Return
                return ResultClass.WithContent(lista);

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("getRol")]
        [HttpPost]
        [APIReturn(typeof(RolModel))]
        public ActionResult GetRol(GetRolRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Roles_VisualizarRoles() || bs.AdmPermiso.Roles_ModificarYEliminarRoles())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Obtenemos el registro
                var result = bs.AdmRole.getById(request.Id);

                // Transformación de campos
                var item = new RolModel(){
                    Id = result.Id,
                    Rol = result.Rol,
                    PermisosIds = result.AdmRolesPermisos.Select(p => p.PermisoId).ToList()
                };

                // Return
                return ResultClass.WithContent(item);

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("saveRol")]
        [HttpPost]
        [APIReturn(typeof(RolModel))]
        public ActionResult SaveRol(RolModel request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Roles_ModificarYEliminarRoles())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Obtenemos el registro de base de datos. Si es nuevo lo creamos
                AdmRole result = null;
                if (request.Id == 0) {
                    result = new AdmRole();
                    bs.AdmRole.insert(result);
                } else {
                    result = bs.AdmRole.getById(request.Id);
                    if (result == null)
                        return ResultClass.WithError($"No se ha encontrado el rol con el Id {request.Id}.");
                }

                // Si el usuario actual tiene asignado este rol, no le permitimos modificarlo para evitar poder perder permisos.
                if (bs.User.RolId == request.Id)
                    return ResultClass.WithError($"No se puede modificar el rol del usuario actual. Si desea realizar cambios, cree un rol temporal, asígnelo a su usuario y a continuación modifique este.");

                // Validamos que estén rellenos todos los campos obligatorios
                if (String.IsNullOrEmpty(request.Rol)) return ResultClass.WithError("El campo Rol es obligatorio.");
                
                // Actualizamos valores
                result.Rol = request.Rol;

                // Eliminamos permisos desmarcados
                var permisosEliminar = result.AdmRolesPermisos.Where(p => !request.PermisosIds.Contains(p.PermisoId)).ToList();
                foreach (var permisoEliminar in permisosEliminar) {
                    bs.AdmRolesPermiso.delete(permisoEliminar);
                }

                // Añadimos o actualizamos
                foreach (var permisoId in request.PermisosIds) {
                    var permiso = result.AdmRolesPermisos.Where(p => p.PermisoId == permisoId).FirstOrDefault();
                    if (permiso == null) {
                        permiso = new AdmRolesPermiso();
                        permiso.PermisoId = permisoId;
                        result.AdmRolesPermisos.Add(permiso);
                    }
                }

                // Guardamos los cambios
                bs.save();

                // Construimos y devolvemos el objeto resultante.
                return ResultClass.WithContent(new RolModel() {
                    Id = result.Id,
                    Rol = result.Rol,
                    PermisosIds = result.AdmRolesPermisos.Select(p => p.PermisoId).ToList()
                });

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("deleteRoles")]
        [HttpPost]
        [APIReturn(typeof(bool))]
        public ActionResult DeleteRoles(DeleteRolesRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Roles_ModificarYEliminarRoles()))
                {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Si alguno de los roles está en uso, damos error.
                if (bs.AdmUsuario.getAll().Any(p => request.RolesIds.Contains((int)p.RolId))) {
                    var usuario = bs.AdmUsuario.getAll().Where(p => request.RolesIds.Contains((int)p.RolId)).FirstOrDefault();
                    return ResultClass.WithError($"No se puede eliminar el rol {usuario.Rol.Rol} porque el usuario {usuario.Nombre} {usuario.Apellidos} lo tiene asignado.");
                }

                // Eliminamos roles
                List<AdmRole> rolesEliminar = bs.AdmRole.getAll().Where(p => request.RolesIds.Contains(p.Id)).ToList();
                foreach (var rol in rolesEliminar)
                {
                    foreach (var rolPermiso in rol.AdmRolesPermisos.ToList()) {
                        bs.AdmRolesPermiso.delete(rolPermiso);
                    }
                    bs.AdmRole.delete(rol);
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

        public class GetRolesRequest
        {
            public int start;
            public int limit;
            public string sort;
            public string dir;
            public string filtro_Search;
        }

        public class GetRolesResult
        {
            public int Id;
            public string Rol;
        }

        public class GetRolesComboRequest
        {
        }

        public class GetRolesComboResult
        {
            public int Id;
            public string Rol;
        }

        public class GetRolRequest
        {
            public int Id;
        }

        public class RolModel
        {
            public int Id;
            public string Rol;
            public List<int> PermisosIds;
        }

        public class DeleteRolesRequest
        {
            public List<int> RolesIds;
        }

        #endregion

    }
}