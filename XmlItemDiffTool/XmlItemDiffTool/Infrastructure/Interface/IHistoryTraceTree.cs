using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Enum;
using Infrastructure.ObjectModel;

namespace Infrastructure.Interface
{
    public interface IHistoryTraceTree
    {
        string Id { get; set; }
        ObservableCollection<HistoryState> HistoryStates { get; set; }
        ObservarableUniqueCollection<IHistoryTraceTree> Children { get; set; }
        IHistoryTraceTree GetNode(IHistoryTraceTree item);
        IHistoryTraceTree GetNode(string id);
    }
}
