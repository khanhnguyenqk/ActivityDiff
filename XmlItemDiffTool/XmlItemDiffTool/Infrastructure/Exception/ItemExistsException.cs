using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ItemExistsException : Exception
    {
        public object Duplicate { get; private set; }
        public ItemExistsException(object item) :
            base(@"Item already exists.")
        {
            Duplicate = item;
        }
    }
}
