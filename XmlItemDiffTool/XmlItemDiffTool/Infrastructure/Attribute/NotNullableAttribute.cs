using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Attribute
{
    /// <summary>
    /// Says that a property is not allowed to be null
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotNullableAttribute : System.Attribute
    {
    }
}
