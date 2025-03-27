using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorExchangeWPF.Models
{
    internal class LoadDataResponse<T>
    {
        public ObservableCollection<T> listItems { get; set; } = new ObservableCollection<T>();

        public MetaResponse meta { set; get; }

    }
}
