using System.Linq;
using MarpajarosTPVAPI.Model;
using System.Collections.Generic;

namespace MarpajarosTPVAPI.Business.BL
{

    public class BL_FacVenta: BusinessContext<FacVenta>
    {

        #region Getters

        public FacVenta getById(long Id)
        {
            return getAll().Where(p => p.Id == Id).SingleOrDefault();
        }

        #endregion

    }

}