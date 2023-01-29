namespace MarpajarosTPVAPI.Classes
{
    public class APIReturn : System.Attribute
    {
        public System.Type Type {get; set;}
        public APIReturn(System.Type type) {
            this.Type = type;
        }
    }
}