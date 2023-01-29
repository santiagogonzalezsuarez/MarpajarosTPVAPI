using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using MarpajarosTPVAPI.Classes;
using MarpajarosTPVAPI.Business;
using MarpajarosTPVAPI.Model;
using System.Collections.Generic;

namespace MarpajarosTPVAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        #region Funciones

        [ActionName("login")]
        [HttpPost]
        [APIReturn(typeof(LoginResult))]
        public ActionResult Login(LoginRequest request)
        {

            try 
            {

                var bs = new BS();
                var usuario = bs.AdmUsuario.getByUsername(request.Username);
                if (usuario == null)
                {
                    return ResultClass.WithError("Usuario o contraseña incorrectos.");
                }
                var hashedPassword = bs.AdmUsuario.getPasswordHash(request.Password);
                if (hashedPassword != usuario.Password)
                {
                    return ResultClass.WithError("Usuario o contraseña incorrectos.");
                }

                // Si todo es correcto, creamos un nuevo token
                var acceso = new AdmUsuariosAcceso();
                acceso.Token = GenerateToken() + bs.AdmUsuario.getPasswordHash(usuario.Id.ToString());
                acceso.CreateDate = DateTime.Now;
                acceso.Ipaddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                // Añadimos el acceso al usuario
                usuario.AdmUsuariosAccesos.Add(acceso);

                // Guardamos
                bs.save();

                // Establecemos el token en la Cookie de la sesión.
                Response.Cookies.Append("SessionToken", acceso.Token, new Microsoft.AspNetCore.Http.CookieOptions() {
                    Path = "/",
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });

                // Devolvemos el token
                return ResultClass.WithContent(new LoginResult
                {
                    Username = usuario.Username,
                    Token = acceso.Token,
                    Nombre = usuario.Nombre,
                    Apellidos = usuario.Apellidos,
                    Permisos = usuario.Rol.AdmRolesPermisos.Select(p => p.PermisoId).ToList()
                });

            }
            catch
            {
                return ResultClass.WithError("Usuario o contraseña incorrectos.");
            }

        }

        #endregion

        #region Util

        public string GenerateToken()
        {
            var rnd = new Random();
            var letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = "";
            for (int i = 0; i < 128; ++i)
            {
                result += letters[rnd.Next(letters.Length)];
            }
            return result;
        }

        #endregion

        #region Clases

        public class LoginRequest
        {
            public string Username;
            public string Password;
        }

        public class LoginResult
        {
            public string Username;
            public string Token;
            public string Nombre;
            public string Apellidos;
            public List<int> Permisos = new List<int>();
        }

        #endregion

    }
}