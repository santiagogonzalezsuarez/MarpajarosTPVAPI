using System;
using System.Linq;
using MarpajarosTPVAPI.Business;
using MarpajarosTPVAPI.Model;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace MarpajarosTPVAPI.Business.BL
{
    public class BL_AdmUsuario: BusinessContext<AdmUsuario>
    {

        #region Getters

        public AdmUsuario getById(int Id)
        {
            return getAll().Where(p => p.Id == Id).SingleOrDefault();
        }

        public AdmUsuario getByUsername(string Username)
        {
            return getAll().Where(p => p.Username == Username).FirstOrDefault();
        }

        #endregion

        #region Password

        public string getPasswordHash(string Password)
        {
            string result;
            HMACSHA1 sha1 = new HMACSHA1();
            sha1.Key = Encoding.UTF8.GetBytes(BS.configuration.GetValue<string>("UserHashToken"));
            byte[] hashValue = sha1.ComputeHash(Encoding.UTF8.GetBytes(Password));
            result = BitConverter.ToString(hashValue).Replace("-", "");
            return result;
        }

        #endregion

    }
}