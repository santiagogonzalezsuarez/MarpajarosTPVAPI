using System.Linq;
using MarpajarosTPVAPI.Model;

namespace MarpajarosTPVAPI.Business.BL
{
    public class BL_TpvArticulosProveedore: BusinessContext<TpvArticulosProveedore>
    {

        #region Getters

        public IQueryable<TpvArticulosProveedore> getByArticuloId(int ArticuloId)
        {
            return getAll().Where(p => p.ArticuloId == ArticuloId).AsQueryable();
        }

        #endregion

    }
}