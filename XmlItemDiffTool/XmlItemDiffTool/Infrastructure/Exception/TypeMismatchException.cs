using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class TypeMismatchException : Exception
    {
        public TypeMismatchException()
        {
            
        }
        public TypeMismatchException(string msg):base(msg)
        {
             
        }
    }
}
