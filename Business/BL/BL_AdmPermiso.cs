using System.Linq;
using MarpajarosTPVAPI.Model;
using System.Collections.Generic;

namespace MarpajarosTPVAPI.Business.BL
{

    public class BL_AdmPermiso: BusinessContext<AdmPermiso>
    {

        #region Getters

        public AdmPermiso getById(int Id)
        {
            return getAll().Where(p => p.Id == Id).SingleOrDefault();
        }

        #endregion

        #region Permisos

        public bool Roles_VisualizarRoles(string UserName = null)
        {
            return TienePermisos(110, UserName);
        }

        public bool Roles_ModificarYEliminarRoles(string UserName = null)
        {
            return TienePermisos(120, UserName);
        }

        public bool Usuarios_VisualizarUsuarios(string UserName = null)
        {
            return TienePermisos(210, UserName);
        }

        public bool Usuarios_ModificarYEliminarUsuarios(string UserName = null)
        {
            return TienePermisos(220, UserName);
        }

        public bool Proveedores_AccesoListadoDeProveedores(string UserName = null)
        {
            return TienePermisos(310, UserName);
        }

        public bool Proveedores_ModificarProveedores(string UserName = null)
        {
            return TienePermisos(320, UserName);
        }

        public bool Productos_ModificarCategorias(string UserName = null)
        {
            return TienePermisos(410, UserName);
        }

        public bool Productos_VisualizarProductos(string UserName = null)
        {
            return TienePermisos(420, UserName);
        }

        public bool Productos_ModificarYEliminarProductos(string UserName = null)
        {
            return TienePermisos(430, UserName);
        }

        public bool Compras_RegistrarComprasAlProveedor(string UserName = null)
        {
            return TienePermisos(510, UserName);
        }

        public bool Compras_ModificarComprasAlProveedor(string UserName = null)
        {
            return TienePermisos(520, UserName);
        }

        public bool Ventas_RealizarVentas(string UserName = null)
        {
            return TienePermisos(610, UserName);
        }

        public bool Ventas_ModificarVentas(string UserName = null)
        {
            return TienePermisos(620, UserName);
        }

        public bool Informes_InformeDeVentas(string UserName = null)
        {
            return TienePermisos(710, UserName);
        }

        public bool Configuracion_ConfiguracionPrograma(string UserName = null)
        {
            return TienePermisos(9010, UserName);
        }

        #endregion

        #region Función genérica de permisos

        public bool TienePermisos(int PermisoId, string UserName = null)
        {
            var Permisos = GetPermisos(UserName);
            return Permisos.Contains(PermisoId);
        }

        public List<int>GetPermisos(string UserName = null)
        {
            AdmUsuario user = null;
            if (UserName == null)
            {
                user = bs.User;
            }
            else
            {
                user = bs.AdmUsuario.getByUsername(UserName);
            }
            if (user == null)
            {
                return new List<int>();
            }
            var permisos = user.Rol.AdmRolesPermisos.Select(p => p.PermisoId).ToList();
            return permisos;
        }

        #endregion

    }

}
