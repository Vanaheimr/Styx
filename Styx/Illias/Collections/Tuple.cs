using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.GraphDefined.Vanaheimr.Illias.Collections
{

    public class Tuple<T1, T2> : IComparable<Tuple<T1, T2>>,
                                 IEquatable <Tuple<T1, T2>>
    {

        #region Properties

        #region Item1

        private readonly T1 _Item1;

        public T1 Item1
        {
            get
            {
                return _Item1;
            }
        }

        #endregion

        #region Item2

        private readonly T2 _Item2;

        public T2 Item2
        {
            get
            {
                return _Item2;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        public Tuple(T1 Item1, T2 Item2)
        {
            this._Item1  = Item1;
            this._Item2  = Item2;
        }

        #endregion



        public int CompareTo(Tuple<T1, T2> other)
        {

            // A bit stupid!
            return Item1.GetHashCode().CompareTo(other.Item1.GetHashCode());

        }

        public bool Equals(Tuple<T1, T2> other)
        {

            if (Item1.Equals(other.Item1))
                return false;

            return (Item2.Equals(other.Item2));

        }



        public override Int32 GetHashCode()
        {
            return Item1.GetHashCode() + 1 ^ Item2.GetHashCode();
        }

    }

}

