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
    public class UsuariosController : ControllerBase
    {

        #region Funciones

        [ActionName("getUsuarios")]
        [HttpPost]
        [APIReturn(typeof(Paging<GetUsuariosResult>))]
        public ActionResult GetUsuarios(GetUsuariosRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Usuarios_VisualizarUsuarios() || bs.AdmPermiso.Usuarios_ModificarYEliminarUsuarios())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                IQueryable<AdmUsuario> result = bs.AdmUsuario.getAll();

                // Filtro

                // Búsqueda
                if (request.filtro_Search != null && request.filtro_Search != "")
                {
                    var palabras = request.filtro_Search.Split(" ").Where(p => p.Length > 0).ToList();
                    foreach (var palabra in palabras)
                    {
                        result = result.Where(p => p.Username.Contains(palabra) || p.Nombre.Contains(palabra) || p.Apellidos.Contains(palabra) || p.Rol.Rol.Contains(palabra));
                    }
                }

                // Ordenación
                switch (request.sort)
                {
                    case "Nombre":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.Nombre).ThenByDescending(p => p.Apellidos).ThenByDescending(p => p.Id);
                        } else {
                            result = result.OrderBy(p => p.Nombre).ThenBy(p => p.Apellidos).ThenBy(p => p.Id);
                        }
                        break;
                    case "Apellidos":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.Apellidos).ThenByDescending(p => p.Nombre).ThenByDescending(p => p.Id);
                        } else {
                            result = result.OrderBy(p => p.Apellidos).ThenBy(p => p.Nombre).ThenBy(p => p.Id);
                        }
                        break;
                    case "Rol":
                        if (request.dir == "DESC") {
                            result = result.OrderByDescending(p => p.Rol.Rol).ThenByDescending(p => p.Id);
                        } else {
                            result = result.OrderBy(p => p.Rol.Rol).ThenBy(p => p.Id);
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
                var lista = new List<GetUsuariosResult>();
                var page = (int)(Math.Floor((decimal)request.start / (decimal)request.limit) + 1);
                if (result != null)
                {

                    lista = (from p in result.ToList()
                             select new GetUsuariosResult
                             {
                                Id = p.Id,
                                Username = p.Username,
                                Nombre = p.Nombre,
                                Apellidos = p.Apellidos,
                                Rol = p.Rol.Rol
                             }).ToList();

                }

                // Return
                return ResultClass.WithContent(new Paging<GetUsuariosResult>()
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

        [ActionName("getUsuario")]
        [HttpPost]
        [APIReturn(typeof(UsuarioModel))]
        public ActionResult GetUsuario(GetUsuarioRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Usuarios_VisualizarUsuarios() || bs.AdmPermiso.Usuarios_ModificarYEliminarUsuarios())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Obtenemos el registro
                var result = bs.AdmUsuario.getById(request.Id);

                // Transformación de campos
                var item = new UsuarioModel(){
                    Id = result.Id,
                    Username = result.Username,
                    Nombre = result.Nombre,
                    Apellidos = result.Apellidos,
                    RolId = result.RolId
                };

                // Return
                return ResultClass.WithContent(item);

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("saveUsuario")]
        [HttpPost]
        [APIReturn(typeof(UsuarioModel))]
        public ActionResult SaveUsuario(UsuarioModel request)
        {
            
            try 
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Usuarios_ModificarYEliminarUsuarios())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Obtenemos el registro de base de datos. Si es nuevo lo creamos
                AdmUsuario result = null;
                if (request.Id == 0) {
                    // Si la contraseña no se especifica, damos error
                    if (request.Password == null || request.Password == "")
                        return ResultClass.WithError("Es obligatorio establecer una contraseña para el usuario.");
                    result = new AdmUsuario();
                    bs.AdmUsuario.insert(result);
                } else {
                    result = bs.AdmUsuario.getById(request.Id);
                    if (result == null)
                        return ResultClass.WithError($"No se ha encontrado el usuario con el Id {request.Id}.");
                }

                // Si es el usuario actual y está perdiendo el permiso de administrar roles, error.
                if (bs.User.Id == result.Id) {
                    AdmRole role = bs.AdmRole.getById(request.RolId.GetValueOrDefault(0));
                    if (!(new []{210, 220}.All(p => role.AdmRolesPermisos.Any(q => q.PermisoId == p))))
                        return ResultClass.WithError($"No se puede guardar el cambio porque implicaría que el usuario actual perdiera los permisos para gestionar usuarios.");
                }

                // Validamos que estén rellenos todos los campos obligatorios
                if (String.IsNullOrEmpty(request.Username)) return ResultClass.WithError("El campo Nombre de usuario es obligatorio.");
                if (String.IsNullOrEmpty(request.Nombre)) return ResultClass.WithError("El campo Nombre es obligatorio.");
                if (String.IsNullOrEmpty(request.Apellidos)) return ResultClass.WithError("El campo Apellidos es obligatorio.");
                if (request.RolId == null || request.RolId < 1 || bs.AdmRole.getById(request.RolId.GetValueOrDefault(0)) == null)
                    return ResultClass.WithError("Debe seleccionar un rol.");
                
                // Actualizamos valores.
                result.Username = request.Username;
                result.Nombre = request.Nombre;
                result.Apellidos = request.Apellidos;
                result.RolId = request.RolId;
                if (request.Password != null && request.Password != "") {
                    result.Password = bs.AdmUsuario.getPasswordHash(request.Password);
                }

                // Guardamos los cambios
                bs.save();

                // Construimos y devolvemos el objeto resultante.
                return ResultClass.WithContent(new UsuarioModel() {
                    Id = result.Id,
                    Username = result.Username,
                    Nombre = result.Nombre,
                    Apellidos = result.Apellidos,
                    RolId = result.RolId
                });

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }
            
        }

        [ActionName("deleteUsuarios")]
        [HttpPost]
        [APIReturn(typeof(bool))]
        public ActionResult DeleteUsuarios(DeleteUsuariosRequest request)
        {
            
            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Usuarios_ModificarYEliminarUsuarios()))
                {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Si el Id del usuario actual está contenido en la lista, error.
                if (request.UsuariosIds.Contains(bs.User.Id))
                    return ResultClass.WithError("No se puede eliminar el usuario actual.");
                
                // Eliminamos usuarios
                List<AdmUsuario> usuariosEliminar = bs.AdmUsuario.getAll().Where(p => request.UsuariosIds.Contains(p.Id)).ToList();
                foreach (var usuario in usuariosEliminar)
                {
                    bs.AdmUsuario.delete(usuario);
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

        public class GetUsuariosRequest
        {
            public int start;
            public int limit;
            public string sort;
            public string dir;
            public string filtro_Search;
        }

        public class GetUsuariosResult
        {
            public int Id;
            public string Username;
            public string Nombre;
            public string Apellidos;
            public string Rol;

        }

        public class GetUsuarioRequest
        {
            public int Id;
        }

        public class UsuarioModel
        {
            public int Id;
            public string Username;
            public string Password;
            public string Nombre;
            public string Apellidos;
            public int? RolId;
        }

        public class DeleteUsuariosRequest
        {
            public List<int> UsuariosIds;
        }

        #endregion

    }
}