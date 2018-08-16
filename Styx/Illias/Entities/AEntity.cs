/*
 * Copyright (c) 2010-2018 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public interface IEntityClass<TClass> : IEquatable<TClass>,
                                            IComparable<TClass>,
                                            IComparable
    { }

    /// <summary>
    /// An abstract entity.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identification.</typeparam>
    /// <typeparam name="TDataSource">The type of the data source.</typeparam>
    public abstract class AEntity<TId, TDataSource> : IEntity<TId>
        where TId : IId
    {

        #region Properties

        /// <summary>
        /// The global unique identification of this entity.
        /// </summary>
        [Mandatory]
        public TId                                   Id                  { get; }

        /// <summary>
        /// The source of this information, e.g. an automatic importer.
        /// </summary>
        [Optional]
        public TDataSource                           DataSource          { get; }

        /// <summary>
        /// A lookup for user-defined properties.
        /// </summary>
        [Optional]
        public ConcurrentDictionary<String, Object>  UserDefined         { get; }

        /// <summary>
        /// The timestamp of the last changes within this ChargingPool.
        /// Can be used as a HTTP ETag.
        /// </summary>
        [Mandatory]
        public DateTime                              LastChange          { get; protected set; }

        #endregion

        #region Events

        public event OnPropertyChangedDelegate OnPropertyChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract entity.
        /// </summary>
        /// <param name="Id">The unique entity identification.</param>
        /// <param name="DataSource">The source of this information, e.g. an automatic importer.</param>
        protected AEntity(TId          Id,
                          TDataSource  DataSource)
        {

            #region Initial checks

            if (EqualityComparer<TId>.Default.Equals(Id, default(TId)))
                throw new ArgumentNullException(nameof(Id), "The given Id must not be null!");

            #endregion

            this.Id           = Id;
            this.DataSource   = DataSource;
            this.LastChange   = DateTime.UtcNow;
            this.UserDefined  = new ConcurrentDictionary<String, Object>();

        }

        #endregion


        #region SetProperty<T>(ref FieldToChange, NewValue, [CallerMemberName])

        /// <summary>
        /// Change the given field and call the OnPropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the field to be changed.</typeparam>
        /// <param name="FieldToChange">A reference to the field to be changed.</param>
        /// <param name="NewValue">The new value of the field to be changed.</param>
        /// <param name="PropertyName">The name of the property to be changed (set by the compiler!)</param>
        public void SetProperty<T>(ref                T       FieldToChange,
                                                      T       NewValue,
                                   [CallerMemberName] String  PropertyName = "")
        {

            if (!EqualityComparer<T>.Default.Equals(FieldToChange, NewValue))
            {

                var OldValue       = FieldToChange;
                    FieldToChange  = NewValue;

                PropertyChanged(PropertyName, OldValue, NewValue);

            }

        }

        #endregion

        #region DeleteProperty<T>(ref FieldToChange, [CallerMemberName])

        /// <summary>
        /// Delete the given field and call the OnPropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the field to be deleted.</typeparam>
        /// <param name="FieldToChange">A reference to the field to be deleted.</param>
        /// <param name="PropertyName">The name of the property to be deleted (set by the compiler!)</param>
        public void DeleteProperty<T>(ref                T       FieldToChange,
                                      [CallerMemberName] String  PropertyName = "")
        {

            if (FieldToChange != null)
            {

                var OldValue       = FieldToChange;
                    FieldToChange  = default(T);

                PropertyChanged(PropertyName, OldValue, default(T));

            }

        }

        #endregion

        #region PropertyChanged<T>(PropertyName, OldValue, NewValue)

        /// <summary>
        /// Notify subscribers that a property has changed.
        /// </summary>
        /// <typeparam name="T">The type of the changed property.</typeparam>
        /// <param name="PropertyName">The name of the changed property.</param>
        /// <param name="OldValue">The old value of the changed property.</param>
        /// <param name="NewValue">The new value of the changed property.</param>
        public void PropertyChanged<T>(String  PropertyName,
                                       T       OldValue,
                                       T       NewValue)
        {

            #region Initial checks

            if (PropertyName == null)
                throw new ArgumentNullException(nameof(PropertyName), "The given property name must not be null!");

            #endregion

            this.LastChange = DateTime.UtcNow;

            OnPropertyChanged?.Invoke(LastChange,
                                      this,
                                      PropertyName,
                                      OldValue,
                                      NewValue);

        }

        #endregion


        #region this[PropertyName]

        /// <summary>
        /// Return the user-defined property for the given property name.
        /// </summary>
        /// <param name="PropertyName">The name of the user-defined property.</param>
        public Object this[String PropertyName]
        {

            get
            {
                return UserDefined[PropertyName];
            }

            set
            {
                UserDefined[PropertyName] = value;
            }

        }

        #endregion

        #region RemoveUserDefinedProperty()

        /// <summary>
        /// Try to remove a user-defined property.
        /// </summary>
        /// <param name="PropertyName"></param>
        public void RemoveUserDefinedProperty(String PropertyName)
        {
            UserDefined.TryRemove(PropertyName, out Object Value);
        }

        #endregion






        public Int32 CompareTo(Object obj)
        {

            if (obj is AEntity<TId, TDataSource>)
                return Id.CompareTo((AEntity<TId, TDataSource>) obj);

            if (obj is TId)
                return Id.CompareTo((TId) obj);

            return -1;

        }

        public Int32 CompareTo(TId OtherId)
            => Id.CompareTo(OtherId);

        public Boolean Equals(TId OtherId)
            => Id.Equals(OtherId);

    }

}
