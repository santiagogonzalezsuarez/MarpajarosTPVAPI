using System;
using System.Linq;
using MarpajarosTPVAPI.Business;
using MarpajarosTPVAPI.Model;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace MarpajarosTPVAPI.Business.BL
{
    public class BL_AdmRole: BusinessContext<AdmRole>
    {

        #region Getters

        public AdmRole getById(int Id)
        {
            return getAll().Where(p => p.Id == Id).SingleOrDefault();
        }

        #endregion

    }
}