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
        private string id = String.Empty;
        /// <summary>
        /// Not nullable
        /// </summary>
        public string Id
        {
            get { return id; }
            set
            {
                if(value != null && !value.Equals(id))
                {
                    id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<XmlItem> children;

    }
}
