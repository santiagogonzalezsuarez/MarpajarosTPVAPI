using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarpajarosTPVAPI.Classes
{
    public class Paging<T>
    {
        public List<T> collection;
        public int count;
        public int total;
        public int page;
    }
}
