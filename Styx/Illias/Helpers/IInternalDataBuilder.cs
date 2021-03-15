using System;

namespace org.GraphDefined.Vanaheimr.Illias
{

    public interface IInternalDataBuilder
    {

        void AddInternalData(string Key, object Value);

        object GetInternalData(string Key);

        T GetInternalDataAs<T>(string Key);

        void IfDefined(string Key, Action<object> ValueDelegate);

        void IfDefinedAs<T>(String Key, Action<T> ValueDelegate);

        bool IsDefined(string Key);

    }

}