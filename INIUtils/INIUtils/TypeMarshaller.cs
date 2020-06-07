using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace INIUtils
{
    public abstract class TypeMarshaller<T>
    {
        public abstract bool TryPack(T value, out string result);
        public abstract bool TryUnpack(string packed, out T result);
    }
}
