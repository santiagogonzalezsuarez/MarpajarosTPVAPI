using System;
using System.Linq;
using MarpajarosTPVAPI.Business;
using MarpajarosTPVAPI.Model;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace MarpajarosTPVAPI.Business.BL
{
    public class BL_AdmRolesPermiso: BusinessContext<AdmRolesPermiso>
    {

        #region Getters

        public IQueryable<AdmRolesPermiso> getByRolId(int RolId)
        {
            return getAll().Where(p => p.RolId == RolId).AsQueryable();
        }

        #endregion

    }
}