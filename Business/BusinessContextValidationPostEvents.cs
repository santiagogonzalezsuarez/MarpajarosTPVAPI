using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MarpajarosTPVAPI.Business
{

    /// <summary>
    /// Clase base para las distintas entidades de base de datos que ofrece funciones básicas para el tratamiento de los objetos de dicha entidad.
    /// Adicionalmente permite la ejecución de funciones tras el guardado del contexto en base de datos mediante las funciones afterInsert, afterUpdate
    /// y afterDelete. También ofrece la posibilidad de validar los valores de los elementos insertados, modificados o eliminados antes de
    /// guardar los cambios en base de datos. Utilizar la función Throw en las funciones de validación permitirá anular el guardado en base de datos
    /// y mantener la base de datos en su estado original.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public abstract class BusinessContextValidationPostEvents<T>: BusinessContext<T>, IBusinessContextValidation, IBusinessContextPostEvents where T: class
    {

        #region Validation

        void IBusinessContextValidation.validateInsert()
        {
            db.ChangeTracker.DetectChanges();
            var addedObjects = db.ChangeTracker.Entries<T>().Where(p => p.State == EntityState.Added).Select(p => p.Entity);
            foreach (T objeto in addedObjects)
            {
                validateInsert(objeto);
            }

        }
        void IBusinessContextValidation.validateUpdate()
        {
            db.ChangeTracker.DetectChanges();
            var modifiedObjects = db.ChangeTracker.Entries<T>().Where(p => p.State == EntityState.Modified).Select(p => new ModifiedEntityClass<T>() { ModifiedObject = p.Entity, OriginalValues = p.OriginalValues });
            foreach (ModifiedEntityClass<T> objeto in modifiedObjects)
            {
                validateUpdate(objeto.ModifiedObject, (T)objeto.OriginalValues.ToObject());
            }
        }

        void IBusinessContextValidation.validateDelete()
        {
            db.ChangeTracker.DetectChanges();
            var deletedObjects = db.ChangeTracker.Entries<T>().Where(p => p.State == EntityState.Deleted).Select(p => new ModifiedEntityClass<T>() { ModifiedObject = p.Entity, OriginalValues = p.OriginalValues });
            foreach (ModifiedEntityClass<T> objeto in deletedObjects)
            {
                validateDelete(objeto.ModifiedObject, (T)objeto.OriginalValues.ToObject());
            }
        }

        /// <summary>
        /// Reemplace este método para definir las validaciones a realizar antes de insertar un registro de esta entidad.
        /// Utilice la función Throw para disparar una excepción y prevenir el guardado en base de datos.
        /// </summary>
        /// <param name="objeto">Objeto que se ha insertado.</param>
        public abstract void validateInsert(T objeto);

        /// <summary>
        /// Reemplace este método para definir las validaciones a realizar antes de modificar un registro de esta entidad.
        /// Utilice la función Throw para disparar una excepción y prevenir el guardado en base de datos.
        /// </summary>
        /// <param name="objeto">Objeto que se ha modificado</param>
        /// <param name="originalValues">Estado previo del objeto.</param>
        public abstract void validateUpdate(T objeto, T originalValues);

        /// <summary>
        /// Reemplace este método para definir las validaciones a realizar antes de eliminar un registro de esta entidad.
        /// Utilice la función Throw para disparar una excepción y prevenir el guardado en base de datos y así evitar su
        /// </summary>
        /// <param name="objeto">Objeto que se ha eliminado. Tenga en cuenta que el objeto eliminado perderá su identificador
        /// principal. Para obtenerlo utilice el parámetro originalValues.</param>
        /// <param name="originalValues">Estado previo del objeto.</param>
        public abstract void validateDelete(T objeto, T originalValues);

        #endregion

        #region PostEvents

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

        #endregion

    }
}
