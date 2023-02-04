using System.Linq;
using MarpajarosTPVAPI.Model;

namespace MarpajarosTPVAPI.Business.BL
{
    public class BL_TpvArticulo: BusinessContext<TpvArticulo>
    {

        #region Getters

        public TpvArticulo getById(long Id)
        {
            return getAll().Where(p => p.Id == Id).SingleOrDefault();
        }

        #endregion

    }
}