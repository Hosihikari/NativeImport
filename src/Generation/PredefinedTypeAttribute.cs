using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hosihikari.NativeInterop.Generation;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class PredefinedTypeAttribute : Attribute
{
}
