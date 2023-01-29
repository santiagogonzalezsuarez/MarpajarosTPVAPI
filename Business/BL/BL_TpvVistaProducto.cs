using System.Linq;
using MarpajarosTPVAPI.Model;

namespace MarpajarosTPVAPI.Business.BL
{
    public class BL_TpvVistaProducto: BusinessContext<TpvVistaProducto>
    {

        #region Getters

        public TpvVistaProducto getById(int Id)
        {
            return getAll().Where(p => p.ArticuloId == Id).SingleOrDefault();
        }

        #endregion

    }
}