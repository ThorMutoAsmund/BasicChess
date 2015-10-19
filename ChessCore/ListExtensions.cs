using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessCore
{
    public static class ListExtensions
    {
        public static T AddTo<T>(this List<T> list, T item)
        {
            list.Add(item);
            return item;
        }
    }
}
