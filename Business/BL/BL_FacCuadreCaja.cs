using System.Linq;
using MarpajarosTPVAPI.Model;
using System.Collections.Generic;

namespace MarpajarosTPVAPI.Business.BL
{

    public class BL_FacCuadreCaja: BusinessContext<FacCuadreCaja>
    {

        #region Getters

        public FacCuadreCaja getById(long Id)
        {
            return getAll().Where(p => p.Id == Id).SingleOrDefault();
        }

        public IQueryable<FacCuadreCaja> getByCajaId(int CajaId)
        {
            return getAll().Where(p => p.CajaId == CajaId).AsQueryable();
        }

        #endregion

    }

}