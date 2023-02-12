using System.Linq;
using MarpajarosTPVAPI.Model;

namespace MarpajarosTPVAPI.Business.BL
{
    public class BL_TpvProveedore: BusinessContext<TpvProveedore>
    {

        #region Getters

        public TpvProveedore getById(int Id)
        {
            return getAll().Where(p => p.Id == Id).SingleOrDefault();
        }

        #endregion

    }
}