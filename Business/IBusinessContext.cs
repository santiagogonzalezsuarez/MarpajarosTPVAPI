using MarpajarosTPVAPI.Context;
using MarpajarosTPVAPI.Business;

namespace MarpajarosTPVAPI.Business
{
    public interface IBusinessContext
    {
        BS bs { get; set; }
        MarpajarosContext db { get; set; }
    }
}
