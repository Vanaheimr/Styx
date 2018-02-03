
using System.Collections;
using Microsoft.SPOT;
using System;

namespace System
{

    public delegate bool Predicate(object o);

    public delegate void Action();

    public delegate void Action_Object (Object Object);

    public delegate void Action_Int32  (Int32  Integer);
    public delegate void Action_UInt32 (UInt32 Integer);
    public delegate void Action_Int64  (Int64  Integer);
    public delegate void Action_UInt64 (UInt64 Integer);

    public delegate void Action_Int32_Int32   (Int32  Integer1, Int32  Integer2);
    public delegate void Action_UInt32_UInt32 (UInt32 Integer1, UInt32 Integer2);
    public delegate void Action_Int64_Int64   (Int64  Integer1, Int64  Integer2);
    public delegate void Action_UInt64_UInt64 (UInt64 Integer1, UInt64 Integer2);

}