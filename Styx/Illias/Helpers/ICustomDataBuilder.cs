using System;

namespace org.GraphDefined.Vanaheimr.Illias
{

    public interface ICustomDataBuilder
    {

        void AddCustomData(string Key, object Value);

        object GetCustomData(string Key);

        T GetCustomDataAs<T>(string Key);

        void IfDefined(string Key, Action<object> ValueDelegate);

        void IfDefinedAs<T>(String Key, Action<T> ValueDelegate);

        bool IsDefined(string Key);

    }

}