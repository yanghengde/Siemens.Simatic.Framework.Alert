using System;
using System.Collections.Generic;
using System.Text;

namespace Siemens.Simatic.Util.Common
{
    public enum FormOperationStatusEnum
    {
        Initialized = 1,
        BeginNew = 2,
        EndNew = 3,
        CanToEdit = 4,
        BeginEdit = 5,
        EndEdit = 6,
        CanToDelete = 7,
        Deleted = 8,
        CanToEditAndDelete = 9,
        Cancelled = 10,
        Searched = 11
    }
}
