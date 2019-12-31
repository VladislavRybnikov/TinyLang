using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyLang.IDE.Utils.Extensions
{
    public static class CollectionExt
    {
        public static void AddTo<T>(this T val, params ICollection<T>[] collectionArgs) 
        {
            foreach (var collection in collectionArgs) collection.Add(val);
        }

        public static void AddTo<T, TSource, T1, T2>(
            this T val, TSource source,
            Func<TSource, ICollection<T1>> first,
            Func<TSource, ICollection<T2>> second = null) where T : T1, T2
        {
            first(source)?.Add(val);
            second?.Invoke(source)?.Add(val);
        }
    }
}
