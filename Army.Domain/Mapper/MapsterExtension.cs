using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace Army.Domain.Mapper
{
    public static class MapsterExtension
    {
        public static List<T> MapList<S, T>(this List<S> source)
        {
            List<T> result = new List<T>();
            foreach (var item in source)
            {
                var t = item.Adapt<T>();
                result.Add(t);
            }
            return result;
        }
    }
}
