using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.IDE.Utils.Extensions
{
    public static class EnumerableExt
    {
        public static IEnumerable<T> AsSingle<T>(this T val) { yield return val; }
    }
}
