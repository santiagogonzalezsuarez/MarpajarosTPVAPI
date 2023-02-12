using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using MarpajarosTPVAPI.Classes;
using MarpajarosTPVAPI.Business;
using MarpajarosTPVAPI.Model;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;

namespace MarpajarosTPVAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {

        #region Funciones

        [ActionName("getAllCategorias")]
        [HttpPost]
        [APIReturn(typeof(List<GetAllCategoriasResult>))]
        public ActionResult GetAllCategorias(GetAllCategoriasRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Productos_ModificarYEliminarProductos())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                var categorias = bs.TpvCategoria.getAll().ToList();
                List<TreeItem> result = new List<TreeItem>();
                AddCategoriasRecursivo(result, null, categorias);

                return ResultClass.WithContent(new GetAllCategoriasResult() {
                    Categorias = result
                });

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        #endregion

        #region Util

        private void AddCategoriasRecursivo(List<TreeItem> result, int? PadreId, List<TpvCategoria> categorias) {
            List<TpvCategoria> categoriasAdd;
            if (PadreId == null) {
                categoriasAdd = categorias.Where(p => p.CategoriaPadreId == null).OrderBy(p => p.Orden).ToList();
            } else {
                categoriasAdd = categorias.Where(p => p.CategoriaPadreId == PadreId).OrderBy(p => p.Orden).ToList();
            }
            foreach (var categoria in categoriasAdd) {
                var categoriaResult = new TreeItem() {
                    Id = categoria.Id,
                    Text = categoria.Categoria,
                    Children = new List<TreeItem>()
                };
                AddCategoriasRecursivo(categoriaResult.Children, categoriaResult.Id, categorias);
                result.Add(categoriaResult);
            }
        }

        #endregion

        #region Clases

        public class GetAllCategoriasRequest
        {
        }

        public class GetAllCategoriasResult
        {
            public List<TreeItem> Categorias;
        }

        #endregion

    }
}