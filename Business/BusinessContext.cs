using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MarpajarosTPVAPI.Context;

namespace MarpajarosTPVAPI.Business
{

    /// <summary>
    /// Clase base para las distintas entidades de base de datos que ofrece funciones básicas para el tratamiento de los objetos de dicha entidad.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public abstract class BusinessContext<T> : IBusinessContext where T: class
    {
        /// <summary>
        /// Obtiene un IQueryable con los registros de esta tabla.
        /// </summary>
        /// <returns>Devuelve un IQueryable de registros al que se le pueden aplicar funciones Lambda de base de datos.</returns>
        public virtual IQueryable<T> getAll() 
        {
            return db.Set<T>();
        }

        /// <summary>
        /// Obtiene un IQueryable de registros de este tipo de base de datos sin almacenarlo en la caché de EntityFramework. Se debe utilizar esta función únicamente si no se planea
        /// modificar los registros. Los registros cargados mediante esta función no se modificarán al realizar un bs.Save() a no ser que se produzca una operación de Tracking a posteriori.
        /// La carga masiva de registros mediante esta función no lastrará el rendimiento a la hora de realizar un bs.Save().
        /// </summary>
        /// <returns>Devuelve un IQueryable de registros al que se le pueden aplicar funciones Lambda de base de datos y cuyos datos no se guardarán al realizar un bs.Save() a no ser que otra función realice Tracking de estos registros.</returns>
        public virtual IQueryable<T> getAllNoTracking()
        {
            return db.Set<T>().AsNoTracking();
        }

        /// <summary>
        /// Crea un nuevo objeto en base de datos y enlaza las propiedades para LazyLoad
        /// </summary>
        public virtual T create()
        {
            var item = db.Set<T>().CreateProxy();
            db.Attach(item);
            return item;
        }

        /// <summary>
        /// Inserta el objeto en base de datos.
        /// </summary>
        /// <param name="objeto">Objeto a insertar.</param>
        public virtual void insert(T objeto)
        {
            db.Set<T>().Add(objeto);
        }

        /// <summary>
        /// Elimina el objeto de base de datos.
        /// </summary>
        /// <param name="objeto">Objeto a eliminar.</param>
        public virtual void delete(T objeto)
        {
            db.Set<T>().Remove(objeto);
        }

        private MarpajarosContext _db;
        /// <summary>
        /// Contexto de EntityFramework para la instancia actual de base de datos.
        /// </summary>
        public MarpajarosContext db
        {
            get
            {
                return _db;
            }
            set
            {
                _db = value;
            }
        }

        private BS _bs;
        /// <summary>
        /// Obtiene un objeto BS con la instancia actual de base de datos.
        /// </summary>
        public BS bs
        {
            get
            {
                return _bs;
            }
            set
            {
                _bs = value;
            }
        }
    }
}
