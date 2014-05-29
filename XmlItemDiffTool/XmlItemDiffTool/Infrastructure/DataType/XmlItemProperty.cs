using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infrastructure.DataType
{
    public class XmlItemProperty : NotifyPropertyChangedBase
    {
        private string name = String.Empty;
        /// <summary>
        /// Not Nullable
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if(value != null && !value.Equals(name))
                {
                    value = name;
                    NotifyPropertyChanged();
                }
            }
        }

        private string value = String.Empty;
        public string Value
        {
            get { return value; }
            set
            {
                if(value != null && !value.Equals(value))
                {
                    value = name;
                    NotifyPropertyChanged();
                }
            }
        }

        public override int GetHashCode()
        {
            int ret = Name.GetHashCode();
            ret = ret << 20;
            ret = ret ^ Value.GetHashCode();
            return ret;
        }

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if(obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            XmlItemProperty x = obj as XmlItemProperty;
            if((System.Object)x == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Value.Equals(x.Value) && Name.Equals(x.Name);
        }

        public bool Equals(XmlItemProperty x)
        {
            // If parameter is null return false:
            if((object)x == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Value.Equals(x.Value) && Name.Equals(x.Name);
        }
    }
}
