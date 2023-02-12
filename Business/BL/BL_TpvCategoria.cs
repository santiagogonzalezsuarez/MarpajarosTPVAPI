using System.Linq;
using MarpajarosTPVAPI.Model;

namespace MarpajarosTPVAPI.Business.BL
{
    public class BL_TpvCategoria: BusinessContext<TpvCategoria>
    {

        #region Getters

        public TpvCategoria getById(int Id)
        {
            return getAll().Where(p => p.Id == Id).SingleOrDefault();
        }

        #endregion

    }
}