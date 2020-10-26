using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem.Editor
{
    public static class Utils
    {
        private static StringBuilder _printArray;
        public static char[] SplitPattern = { ' ', '\n' };

        static Utils()
        {
            _printArray = new StringBuilder();
        }

        public static void Print(this IEnumerable collection)
        {
            _printArray.Clear();

            _printArray.Append("{ ");
            foreach (var item in collection)
            {
                _printArray.Append(item.ToString() + ", ");
            }
            _printArray.Remove(_printArray.Length - 2, 1);
            _printArray.Append("}");

            Debug.Log(_printArray.ToString());

        }
    }
}
