/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI
{

    /// <summary>
    /// Extension methods for environment keys.
    /// </summary>
    public static class EnvironmentKeyExtensions
    {

        /// <summary>
        /// Indicates whether this environment key is null or empty.
        /// </summary>
        /// <param name="EnvironmentKey">An environment key.</param>
        public static Boolean IsNullOrEmpty(this EnvironmentKey? EnvironmentKey)
            => !EnvironmentKey.HasValue || EnvironmentKey.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this environment key is null or empty.
        /// </summary>
        /// <param name="EnvironmentKey">An environment key.</param>
        public static Boolean IsNotNullOrEmpty(this EnvironmentKey? EnvironmentKey)
            => EnvironmentKey.HasValue && EnvironmentKey.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An environment key.
    /// </summary>
    public readonly struct EnvironmentKey : IId,
                                            IEquatable<EnvironmentKey>,
                                            IComparable<EnvironmentKey>
    {

        #region Data

        private readonly static Dictionary<String, EnvironmentKey>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                              InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this environment key is null or empty.
        /// </summary>
        public readonly Boolean                    IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this environment key is NOT null or empty.
        /// </summary>
        public readonly Boolean                    IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the environment key.
        /// </summary>
        public readonly UInt64                     Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered environment keys.
        /// </summary>
        public static IEnumerable<EnvironmentKey>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new environment key based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an environment key.</param>
        public EnvironmentKey(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static EnvironmentKey Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new EnvironmentKey(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an environment key.
        /// </summary>
        /// <param name="Text">A text representation of an environment key.</param>
        public static EnvironmentKey Parse(String Text)
        {

            if (TryParse(Text, out var environmentKey))
                return environmentKey;

            throw new ArgumentException($"Invalid text representation of an environment key: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an environment key.
        /// </summary>
        /// <param name="Text">A text representation of an environment key.</param>
        public static EnvironmentKey? TryParse(String Text)
        {

            if (TryParse(Text, out var environmentKey))
                return environmentKey;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EnvironmentKey)

        /// <summary>
        /// Try to parse the given text as an environment key.
        /// </summary>
        /// <param name="Text">A text representation of an environment key.</param>
        /// <param name="EnvironmentKey">The parsed environment key.</param>
        public static Boolean TryParse(String Text, out EnvironmentKey EnvironmentKey)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out EnvironmentKey))
                    EnvironmentKey = Register(Text);

                return true;

            }

            EnvironmentKey = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this environment key.
        /// </summary>
        public EnvironmentKey Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The environment key for a remote system identification.
        /// </summary>
        public static EnvironmentKey  RemoteSystemId             { get; }
            = Register("remoteSystemId");

        /// <summary>
        /// The environment key for a remote system OCPP version.
        /// </summary>
        public static EnvironmentKey  RemoteSystemOCPPVersion    { get; }
            = Register("remoteSystemOCPPVersion");

        #endregion


        #region Operator overloading

        #region Operator == (EnvironmentKey1, EnvironmentKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentKey1">An environment key.</param>
        /// <param name="EnvironmentKey2">Another environment key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnvironmentKey EnvironmentKey1,
                                           EnvironmentKey EnvironmentKey2)

            => EnvironmentKey1.Equals(EnvironmentKey2);

        #endregion

        #region Operator != (EnvironmentKey1, EnvironmentKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentKey1">An environment key.</param>
        /// <param name="EnvironmentKey2">Another environment key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnvironmentKey EnvironmentKey1,
                                           EnvironmentKey EnvironmentKey2)

            => !EnvironmentKey1.Equals(EnvironmentKey2);

        #endregion

        #region Operator <  (EnvironmentKey1, EnvironmentKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentKey1">An environment key.</param>
        /// <param name="EnvironmentKey2">Another environment key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnvironmentKey EnvironmentKey1,
                                          EnvironmentKey EnvironmentKey2)

            => EnvironmentKey1.CompareTo(EnvironmentKey2) < 0;

        #endregion

        #region Operator <= (EnvironmentKey1, EnvironmentKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentKey1">An environment key.</param>
        /// <param name="EnvironmentKey2">Another environment key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnvironmentKey EnvironmentKey1,
                                           EnvironmentKey EnvironmentKey2)

            => EnvironmentKey1.CompareTo(EnvironmentKey2) <= 0;

        #endregion

        #region Operator >  (EnvironmentKey1, EnvironmentKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentKey1">An environment key.</param>
        /// <param name="EnvironmentKey2">Another environment key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnvironmentKey EnvironmentKey1,
                                          EnvironmentKey EnvironmentKey2)

            => EnvironmentKey1.CompareTo(EnvironmentKey2) > 0;

        #endregion

        #region Operator >= (EnvironmentKey1, EnvironmentKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentKey1">An environment key.</param>
        /// <param name="EnvironmentKey2">Another environment key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnvironmentKey EnvironmentKey1,
                                           EnvironmentKey EnvironmentKey2)

            => EnvironmentKey1.CompareTo(EnvironmentKey2) >= 0;

        #endregion

        #endregion

        #region IComparable<EnvironmentKey> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two environment keys.
        /// </summary>
        /// <param name="Object">An environment key to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EnvironmentKey environmentKey
                   ? CompareTo(environmentKey)
                   : throw new ArgumentException("The given object is not an environment key!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnvironmentKey)

        /// <summary>
        /// Compares two environment keys.
        /// </summary>
        /// <param name="EnvironmentKey">An environment key to compare with.</param>
        public Int32 CompareTo(EnvironmentKey EnvironmentKey)

            => String.Compare(InternalId,
                              EnvironmentKey.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EnvironmentKey> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two environment keys for equality.
        /// </summary>
        /// <param name="Object">An environment key to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EnvironmentKey environmentKey &&
                   Equals(environmentKey);

        #endregion

        #region Equals(EnvironmentKey)

        /// <summary>
        /// Compares two environment keys for equality.
        /// </summary>
        /// <param name="EnvironmentKey">An environment key to compare with.</param>
        public Boolean Equals(EnvironmentKey EnvironmentKey)

            => String.Equals(InternalId,
                             EnvironmentKey.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
