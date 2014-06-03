using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Enum
{
    public enum HistoryState
    {
        I, // Identical, no Changed
        A, // Added
        R, // Removed
        M, // Moved to the same level of the tree
        L, // Moved to different level of the tree,
        D, // Different, changed (Inner property change OR children changed)
        P  // Properties changed 
    }
}
