using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Enum
{
    public enum HistoryState
    {
        N, // No Changed
        A, // Added
        R, // Removed
        M, // Moved to the same level of the tree
        L, // Moved to different level of the tree,
        C  // Changed (Inner property change OR children changed)
    }
}
