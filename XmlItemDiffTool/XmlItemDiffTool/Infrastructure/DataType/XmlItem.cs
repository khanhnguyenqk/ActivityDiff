using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Infrastructure.DataType
{
    public class XmlItem : NotifyPropertyChangedBase
    {
        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                if(!value.Equals(id))
                {
                    id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        Test
    }
}
