

//namespace org.GraphDefined.Vanaheimr.Illias.Collections
//{

//    public class Tuple_old<T1, T2>(T1 Item1, T2 Item2) : IComparable<Tuple<T1, T2>>,
//                                                     IEquatable <Tuple<T1, T2>>
//    {

//        #region Properties

//        public T1  Item1    { get; } = Item1;

//        public T2  Item2    { get; } = Item2;

//        #endregion


//        public Int32 CompareTo(Tuple<T1, T2>? other)
//        {

//            // A bit stupid!
//            return Item1.GetHashCode().CompareTo(other.Item1.GetHashCode());

//        }



//        public override Int32 GetHashCode()
//        {
//            return Item1.GetHashCode() + 1 ^ Item2.GetHashCode();
//        }

//        public override Boolean Equals(Object? OtherTuple)
//        {

//            if (OtherTuple is null)
//                return false;

//            if (ReferenceEquals(this, OtherTuple))
//                return true;

//            return this.Equals(OtherTuple as Tuple<T1, T2>);

//        }

//        public override Boolean Equals(Tuple<T1, T2> OtherTuple)
//        {

//            return Item1.Equals(OtherTuple.Item1) && Item2.Equals(OtherTuple.Item2);

//        }



//        public static bool operator ==(Tuple<T1, T2> left, Tuple<T1, T2> right)
//        {
//            if (ReferenceEquals(left, null))
//            {
//                return ReferenceEquals(right, null);
//            }

//            return left.Equals(right);
//        }

//        public static bool operator !=(Tuple<T1, T2> left, Tuple<T1, T2> right)
//        {
//            return !(left == right);
//        }

//        public static bool operator <(Tuple<T1, T2> left, Tuple<T1, T2> right)
//        {
//            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
//        }

//        public static bool operator <=(Tuple<T1, T2> left, Tuple<T1, T2> right)
//        {
//            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
//        }

//        public static bool operator >(Tuple<T1, T2> left, Tuple<T1, T2> right)
//        {
//            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
//        }

//        public static bool operator >=(Tuple<T1, T2> left, Tuple<T1, T2> right)
//        {
//            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
//        }

//    }

//}

