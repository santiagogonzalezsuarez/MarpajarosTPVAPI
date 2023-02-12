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

        #endregion

    }
}