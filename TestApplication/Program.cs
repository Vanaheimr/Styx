using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using de.ahzf.Pipes;

namespace TestApplication
{

    public class Program
    {

        public static void Main(string[] args)
        {

            var _SingleEnumerator = new SingleEnumerator<UInt64>(123);
            var _List             = new List<UInt64>();
                _List.Add(123);
            var _ListEnumerator   = _List.GetEnumerator();

            var _List1 = new List<UInt64>();
            var _List2 = new List<UInt64>();

            while (_SingleEnumerator.MoveNext())
                _List1.Add(_SingleEnumerator.Current);

            while (_ListEnumerator.MoveNext())
                _List2.Add(_ListEnumerator.Current);

        }

    }

}
