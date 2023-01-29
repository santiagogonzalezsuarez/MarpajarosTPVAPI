namespace MarpajarosTPVAPI.Business
{
    public interface IBusinessContextPostEvents
    {
        void addInsertEntities();
        void addUpdateEntities();
        void addDeleteEntities();

        void afterInsert();
        void afterUpdate();
        void afterDelete();
    }
}
