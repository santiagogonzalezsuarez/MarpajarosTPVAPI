using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MarpajarosTPVAPI.Business
{
    /// <summary>
    /// Clase base para las distintas entidades de base de datos que ofrece funciones básicas para el tratamiento de los objetos de dicha entidad. 
    /// Adicionalmente permite la ejecución de funciones tras el guardado del contexto de base de datos mediante las funciones afterInsert, afterUpdate
    /// y afterDelete. 
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public abstract class BusinessContextPostEvents<T>: BusinessContext<T>, IBusinessContextPostEvents where T: class
    {
        private List<T> insertEntities = new List<T>();
        private List<T> updateEntities = new List<T>();
        private List<T> deleteEntities = new List<T>();

        void IBusinessContextPostEvents.addInsertEntities()
        {
            db.ChangeTracker.DetectChanges();
            var addedObjects = db.ChangeTracker.Entries<T>().Where(p => p.State == EntityState.Added).Select(p => p.Entity);
            foreach (T objeto in addedObjects)
            {
                insertEntities.Add(objeto);
            }
        }

        void IBusinessContextPostEvents.addUpdateEntities()
        {
            db.ChangeTracker.DetectChanges();
            var modifiedObjects = db.ChangeTracker.Entries<T>().Where(p => p.State == EntityState.Modified).Select(p => p.Entity);
            foreach (T objeto in modifiedObjects)
            {
                updateEntities.Add(objeto);
            }
        }

        void IBusinessContextPostEvents.addDeleteEntities()
        {
            db.ChangeTracker.DetectChanges();
            var deletedObjects = db.ChangeTracker.Entries<T>().Where(p => p.State == EntityState.Deleted).Select(p => p.OriginalValues);
            foreach (PropertyValues objeto in deletedObjects)
            {
                T objValue = (T)objeto.ToObject();
                deleteEntities.Add(objValue);
            }
        }

        void IBusinessContextPostEvents.afterInsert()
        {
            foreach (var objeto in insertEntities)
            {
                afterInsert(objeto);
            }
        }

        void IBusinessContextPostEvents.afterUpdate()
        {
            foreach (var objeto in updateEntities)
            {
                afterUpdate(objeto);
            }
        }

        void IBusinessContextPostEvents.afterDelete()
        {
            foreach (var objeto in deleteEntities)
            {
                afterDelete(objeto);
            }
        }

        /// <summary>
        /// Reemplace este método para definir las acciones a realizar tras insertar un elemento de esta entidad.
        /// </summary>
        /// <param name="objeto">Objeto que se ha insertado.</param>
        public abstract void afterInsert(T objeto);

        /// <summary>
        /// Reemplace este método para definir las acciones a realizar tras modificar un elemento de esta entidad. 
        /// </summary>
        /// <param name="objeto">Objeto resultante tras la modificación.</param>
        public abstract void afterUpdate(T objeto);

        /// <summary>
        /// Reemplace este método para definir las acciones a realizar tras eliminar un elemento de esta entidad.
        /// </summary>
        /// <param name="objeto">Objeto que se ha eliminado.</param>
        public abstract void afterDelete(T objeto);

    }
}
