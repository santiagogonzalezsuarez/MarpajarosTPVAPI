using System.Linq;
using MarpajarosTPVAPI.Model;

namespace MarpajarosTPVAPI.Business.BL
{
    public class BL_AdmUsuariosAcceso: BusinessContext<AdmUsuariosAcceso>
    {

        #region Getters

        public AdmUsuariosAcceso getById(int Id)
        {
            return getAll().Where(p => p.Id == Id).SingleOrDefault();
        }

        public AdmUsuariosAcceso getByToken(string token)
        {
            return getAll().Where(p => p.Token == token).FirstOrDefault();
        }

        #endregion

    }
}