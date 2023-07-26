using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hosihikari.NativeInterop.UnsafeTypes;

public interface IMoveableCppInstance<TSelf> where TSelf : class, ICppInstance<TSelf>
{
    static abstract TSelf ConstructInstanceByMove(move_handle<TSelf> right);
}
