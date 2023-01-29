using System.Linq;
using MarpajarosTPVAPI.Model;

namespace MarpajarosTPVAPI.Business.BL
{
    public class BL_TpvArticulosProducto: BusinessContext<TpvArticulosProducto>
    {

        #region Getters

        public TpvArticulosProducto getById(int Id)
        {
            return getAll().Where(p => p.ArticuloId == Id).SingleOrDefault();
        }

        #endregion

    }
}