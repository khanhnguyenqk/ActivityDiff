using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlPropertyHistory : NotifyPropertyChangedBase
    {
        private XmlPropertyAbstract originalProperty;   // this is not XmlStringProperty because of the case NULL -> NEW

        public XmlPropertyAbstract OriginalProperty
        {
            get { return originalProperty; }
            set
            {
                if(!value.Equals(originalProperty))
                {
                    originalProperty = value;
                    NotifyPropertyChanged(@"OriginalProperty");
                }
            }
        }

        private object originalValue;
        public object OriginalValue
        {
            get { return originalValue; }
            set
            {
                if(value == null || !value.Equals(originalValue))
                {
                    originalValue = value;
                    NotifyPropertyChanged(@"OriginalValue");
                }
            }
        }

        private object changedValue;
        public object ChangedValue
        {
            get { return changedValue; }
            set
            {
                if(value == null || !value.Equals(changedValue))
                {
                    changedValue = value;
                    NotifyPropertyChanged(@"ChangedValue");
                }
            }
        }

        public override string ToString()
        {
            return String.Format(@"""{0}"" -> ""{1}""", OriginalValue??@"null", ChangedValue??@"null");
        }
    }
}
