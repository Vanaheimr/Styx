/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Hermod <https://www.github.com/Vanaheimr/Hermod>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{
    public interface IInternalData
    {

        JObject CustomData { get; }
        UserDefinedDictionary InternalData { get; }
        Boolean IsEmpty { get; }
        Boolean IsNotEmpty { get; }
        DateTime LastChangeDate { get; set; }


        event OnPropertyChangedDelegate? OnPropertyChanged;

        void DeleteProperty<T>(ref T? FieldToChange, [CallerMemberName] String PropertyName = "");
        Object? GetInternalData(String Key);
        T? GetInternalDataAs<T>(String Key);
        void IfDefined(String Key, Action<Object> ValueDelegate);
        void IfDefinedAs<T>(String Key, Action<T> ValueDelegate);
        Boolean IsDefined(String Key);
        Boolean IsDefined(String Key, Object? Value);
        void PropertyChanged<T>(String PropertyName, T OldValue, T NewValue, Context? DataSource = null, EventTracking_Id ? EventTrackingId = null);
        SetPropertyResult SetInternalData(String Key, Object? NewValue, Object? OldValue = null, Context? DataSource = null, EventTracking_Id? EventTrackingId = null);
        void SetProperty<T>(ref T FieldToChange, T NewValue, Context? DataSource = null, EventTracking_Id? EventTrackingId = null, [CallerMemberName] String PropertyName = "");
        Boolean TryGetInternalData(String Key, out Object? Value);
        Boolean TryGetInternalDataAs<T>(String Key, out T? Value);

    }

}
