using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hosihikari.NativeInterop.Unmanaged;

public interface ICppVtable
{
    public static abstract ulong VtableLength { get; }
}
