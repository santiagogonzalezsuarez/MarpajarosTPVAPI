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
    public class ModulosController : ControllerBase
    {

        #region Funciones

        [ActionName("getAllModulos")]
        [HttpPost]
        [APIReturn(typeof(List<GetAllModulosResult>))]
        public ActionResult GetAllModulos(GetAllModulosRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Roles_ModificarYEliminarRoles())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                IQueryable<AdmModulo> result = bs.AdmModulo.getAll();

                // Ordenación
                result = result.OrderBy(p => p.Id);

                // Transformación de campos
                var lista = new List<GetAllModulosResult>();
                if (result != null)
                {

                    lista = (from p in result.ToList()
                             select new GetAllModulosResult
                             {
                                Id = p.Id,
                                Modulo = p.Modulo,
                                Permisos = p.AdmPermisos.Select(q => new GetAllModulosPermisosResult
                                {
                                    Id = q.Id,
                                    Permiso = q.Permiso
                                }).ToList()
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

        #endregion

        #region Clases

        public class GetAllModulosRequest
        {
        }

        public class GetAllModulosResult
        {
            public int Id;
            public string Modulo;
            public List<GetAllModulosPermisosResult> Permisos;
        }

        public class GetAllModulosPermisosResult
        {
            public int Id;
            public string Permiso;
        }

        #endregion

    }
}