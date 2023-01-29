using System;
using System.Collections.Generic;
using System.Linq;
using MarpajarosTPVAPI.Model;
using MarpajarosTPVAPI.Context;
using MarpajarosTPVAPI.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Web;
using MarpajarosTPVAPI.Business.BL;

namespace MarpajarosTPVAPI.Business
{
    public class BS
    {

        #region Tablas

        public BL_AdmModulo AdmModulo {get; set;}
        public BL_AdmPermiso AdmPermiso {get; set;}
        public BL_AdmRole AdmRole {get; set;}
        public BL_AdmRolesPermiso AdmRolesPermiso {get; set;}
        public BL_AdmUsuario AdmUsuario {get; set;}
        public BL_AdmUsuariosAcceso AdmUsuariosAcceso {get; set;}
        public BL_TpvVistaProducto TpvVistaProducto {get; set;}
        public BL_TpvArticulosProducto TpvArticulosProducto {get; set;}

        #endregion

        #region Propiedades

        public static IConfiguration configuration = null;
        public List<Action> TasksAfterCommit { get; set; } = new List<Action>();

        private AdmUsuario _user = null;
        public AdmUsuario User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }
        
        private MarpajarosContext _context;
        public bool UsuarioEspecifico = false;

        #endregion

        #region Constructor

        public BS(int? UserId = null)
        {
            ConfigureDatabase();
            InitializeTables();

            if (UserId != null)
            {
                UsuarioEspecifico = true;
            }

            if (UserId == null)
            {
                if (HttpContext.Current.Items.Any(p => (string)p.Key == "AccessToken") && !String.IsNullOrEmpty(HttpContext.Current.Items["AccessToken"]?.ToString())) {
                    var accessToken = HttpContext.Current.Items["AccessToken"]?.ToString();
                    var acceso = this.AdmUsuariosAcceso.getByToken(accessToken);
                    if (acceso != null) {
                        this.User = acceso.AdmUsuarios;
                    }
                } else {
                    if (HttpContext.Current.Request.Cookies.Any(p => p.Key == "SessionToken") && !String.IsNullOrEmpty(HttpContext.Current.Request.Cookies["SessionToken"])) {
                        var accessToken = HttpContext.Current.Request.Cookies["SessionToken"];
                        var acceso = this.AdmUsuariosAcceso.getByToken(accessToken);
                        if (acceso != null) {
                            this.User = acceso.AdmUsuarios;
                        }
                    } else {
                        User = null;
                    }
                }
            } else
            {
                var user = this.AdmUsuario.getById(UserId.Value);
                if (user != null)
                {
                    this.User = user;
                } else
                {
                    this.User = null;
                }
            }

        }

        public BS(string Username) {
            ConfigureDatabase();
            InitializeTables();

            var user = this.AdmUsuario.getByUsername(Username);
            if (user != null) {
                this.User = user;
                UsuarioEspecifico = true;
            } else {
                this.User = null;
            }
        }

        private void ConfigureDatabase()
        {
            if (BS.configuration == null)
            {
                throw new Exception("No se ha establecido una configuración. Utilice BS.configuration = configuration en el fichero Startup.cs para pasar una configuración a la capa Business.");
            }

            // Creamos un nuevo contexto.
            var optionsBuilder = new DbContextOptionsBuilder<MarpajarosContext>();
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("GeneradorExamenes.Presentation.Intranet"));

            this._context = new MarpajarosContext(optionsBuilder.Options);

            this._context.Database.SetCommandTimeout(new TimeSpan(0, 2, 0));
        }

        private void InitializeTables()
        {

            // Instanciamos todas las propiedades.
            foreach (PropertyInfo Propiedad in this.GetType().GetProperties())
            {

                string[] PropiedadesExcluir = { "User", "Entidad" };
                if (!PropiedadesExcluir.Contains(Propiedad.Name))
                {
                    // Instanciamos la propiedad
                    var constructor = Propiedad.PropertyType.GetConstructor(Type.EmptyTypes);
                    if (constructor != null)
                    {
                        this.GetType().GetProperty(Propiedad.Name).SetValue(this, constructor.Invoke(null), null);
                    }

                    // Si la propiedad hereda de BusinessContext, actualizamos su contexto.
                    var valorPropiedad = this.GetType().GetProperty(Propiedad.Name).GetValue(this, null);
                    if (valorPropiedad is IBusinessContext)
                    {
                        ((IBusinessContext)valorPropiedad).db = _context;
                        ((IBusinessContext)valorPropiedad).bs = this;
                    }
                }
            }

        }

        #endregion

        #region Métodos

        /// <summary>
        /// Añade una acción para ejecutar tras el guardado del contexto de base de datos.
        /// </summary>
        /// <param name="action">Código a ejecutar tras el guardado de base de datos.</param>
        public void ExecuteAfterSaveChanges(Action action)
        {
            TasksAfterCommit.Add(action);
        }

        #endregion

        #region Save

        public void save()
        {
            string[] PropiedadesExcluir = { "User", "Entidad" };

            // Validamos la inserción, modificación y eliminación de elementos. En caso de haber errores, lanzamos una excepción y no guardamos los cambios.
            // Esto nos permitirá controlar casos particulares en distintas entidades.
            foreach (PropertyInfo Propiedad in this.GetType().GetProperties())
            {
                if (!PropiedadesExcluir.Contains(Propiedad.Name))
                {
                    var valorPropiedad = this.GetType().GetProperty(Propiedad.Name).GetValue(this, null);
                    if (valorPropiedad != null)
                    {
                        if (valorPropiedad is IBusinessContextValidation)
                        {
                            // Llamamos a los métodos validateInsert, validateUpdate y validateDelete
                            ((IBusinessContextValidation)valorPropiedad).validateDelete();
                            ((IBusinessContextValidation)valorPropiedad).validateInsert();
                            ((IBusinessContextValidation)valorPropiedad).validateUpdate();
                        }

                        if (valorPropiedad is IBusinessContextPostEvents)
                        {
                            // Llamamos a los métodos que almacenan las entidades insertadas, actualizadas y eliminadas para más adelante llamar a PostEvents
                            ((IBusinessContextPostEvents)valorPropiedad).addInsertEntities();
                            ((IBusinessContextPostEvents)valorPropiedad).addUpdateEntities();
                            ((IBusinessContextPostEvents)valorPropiedad).addDeleteEntities();
                        }
                    }
                }
            }

            try
            {
                _context.SaveChanges();
                // Ejecutamos acciones post-guardado y vaciamos la lista
                var tasksToRun = TasksAfterCommit;
                TasksAfterCommit = new List<Action>();
                foreach (var taskAfterCommit in tasksToRun)
                {
                    taskAfterCommit();
                }
            } catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        throw new Exception(ex.Message + "\n" + ex.InnerException.Message + "\n" + ex.InnerException.InnerException.Message);
                    } else
                    {
                        throw new Exception(ex.Message + "\n" + ex.InnerException.Message);
                    }
                } else
                {
                    throw;
                }
            }

            // Ejecutamos PostEvents
            foreach (PropertyInfo Propiedad in this.GetType().GetProperties())
            {
                if (!PropiedadesExcluir.Contains(Propiedad.Name))
                {
                    var valorPropiedad = this.GetType().GetProperty(Propiedad.Name).GetValue(this, null);
                    if (valorPropiedad != null)
                    {
                        if (valorPropiedad is IBusinessContextPostEvents)
                        {
                            ((IBusinessContextPostEvents)valorPropiedad).afterInsert();
                            ((IBusinessContextPostEvents)valorPropiedad).afterUpdate();
                            ((IBusinessContextPostEvents)valorPropiedad).afterDelete();
                        }
                    }
                }
            }

        }

        public void save(bool commitChanges)
        {
            // Por defecto, 1 minuto de Timeout
            this.save(commitChanges, 60);
        }

        public void save(bool commitChanges, long timeout)
        {
            // Validamos la inserción, modificación y eliminación de elementos. En caso de haber errores, lanzamos una excepción y no guardamos los cambios.
            // Esto nos permitirá controlar casos particulares en distintas entidades.
            string[] PropiedadesExcluir = { "User", "Entidad" };
            foreach (PropertyInfo Propiedad in this.GetType().GetProperties())
            {
                if (!PropiedadesExcluir.Contains(Propiedad.Name))
                {
                    var valorPropiedad = this.GetType().GetProperty(Propiedad.Name).GetValue(this, null);
                    if (valorPropiedad != null)
                    {
                        if (valorPropiedad is IBusinessContextValidation)
                        {
                            // Llamamos a los métodos validateInsert, validateUpdate y validateDelete
                            ((IBusinessContextValidation)valorPropiedad).validateDelete();
                            ((IBusinessContextValidation)valorPropiedad).validateInsert();
                            ((IBusinessContextValidation)valorPropiedad).validateUpdate();
                        }

                        if (valorPropiedad is IBusinessContextPostEvents)
                        {
                            // Llamamos a los métodos que almacenan las entidades insertadas, actualizadas o eliminadas para más adelante llamar a PostEvents
                            ((IBusinessContextPostEvents)valorPropiedad).addInsertEntities();
                            ((IBusinessContextPostEvents)valorPropiedad).addUpdateEntities();
                            ((IBusinessContextPostEvents)valorPropiedad).addDeleteEntities();
                        }
                    }
                }
            }

            try
            {

                using (var transaction = this._context.Database.BeginTransaction())
                {
                    this._context.SaveChanges();
                    if (commitChanges)
                    {
                        transaction.Commit();
                        var tasksToRun = TasksAfterCommit;
                        TasksAfterCommit = new List<Action>();
                        foreach (var taskAfterCommit in tasksToRun)
                        {
                            taskAfterCommit();
                        }
                    } else
                    {
                        transaction.Dispose();
                    }
                }


            } catch (Exception ex)
            {
                try
                {
                    _context.Database.GetDbConnection().Close();
                }
                catch { }

                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        throw new Exception(ex.Message + "\n" + ex.InnerException.Message + "\n" + ex.InnerException.InnerException.Message);
                    } else
                    {
                        throw new Exception(ex.Message + "\n" + ex.InnerException.Message);
                    }
                } else
                {
                    throw;
                }
            }

            // Ejecutamos PostEvents
            foreach (PropertyInfo Propiedad in this.GetType().GetProperties())
            {
                if (!PropiedadesExcluir.Contains(Propiedad.Name))
                {
                    var valorPropiedad = this.GetType().GetProperty(Propiedad.Name).GetValue(this, null);
                    if (valorPropiedad != null)
                    {
                        if (valorPropiedad is IBusinessContextPostEvents)
                        {
                            ((IBusinessContextPostEvents)valorPropiedad).afterInsert();
                            ((IBusinessContextPostEvents)valorPropiedad).afterUpdate();
                            ((IBusinessContextPostEvents)valorPropiedad).afterDelete();
                        }
                    }
                }
            }

        }

        #endregion

    }

}