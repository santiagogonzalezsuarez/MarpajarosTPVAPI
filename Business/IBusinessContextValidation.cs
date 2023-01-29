namespace MarpajarosTPVAPI.Business
{
    public interface IBusinessContextValidation
    {
        void validateInsert();
        void validateUpdate();
        void validateDelete();
    }
}
