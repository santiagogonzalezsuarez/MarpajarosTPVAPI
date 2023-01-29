using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MarpajarosTPVAPI.Business
{
    /// <summary>
    /// ''' Clase base para las distintas entidades de base de datos que ofrece funciones básicas para el tratamiento de los objetos de dicha entidad.
    /// Adicionalmente ofrece la posibilidad de validar los valores de los elementos insertados, modificados o eliminados antes de guardar los cambios 
    /// en base de datos. Utilizar la función Throw en las funciones de validación permitirá anular el guardado en base de datos y mantener la base de
    /// datos en su estado original.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public abstract class BusinessContextValidation<T> : BusinessContext<T>, IBusinessContextValidation where T: class
    {
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

    }

    public class ModifiedEntityClass<T> where T: class
    {
        public T ModifiedObject;
        public PropertyValues OriginalValues;
    }
}
