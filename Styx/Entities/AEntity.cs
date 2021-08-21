/*
 * Copyright (c) 2010-2021 Achim Friedland <achim.friedland@graphdefined.com>
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

using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// The interface for custom data stored with an entity.
    /// </summary>
    public interface ICustomData
    { }

    /// <summary>
    /// The generic interface for custom data stored with an entity.
    /// </summary>
    /// <typeparam name="TCustomData">The type of the custom data.</typeparam>
    public interface ICustomData<TCustomData> : ICustomData
    {

        /// <summary>
        /// Custom data stored within this entity.
        /// </summary>
        TCustomData  CustomData    { get; }

    }


    /// <summary>
    /// The interface of an entity.
    /// </summary>
    public interface IEntity : IComparable
    {

        /// <summary>
        /// The timestamp of the last changes within this entity.
        /// Can e.g. be used as a HTTP ETag.
        /// </summary>
        DateTime  LastChange    { get; }


        event OnPropertyChangedDelegate OnPropertyChanged;

    }

    /// <summary>
    /// The generic interface of an entity.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identification.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TCustomData">The type of the custom data.</typeparam>
    /// <typeparam name="TDataSource">The type of the data source.</typeparam>
    public interface IEntity<TId,
                             TEntity,
                             TCustomData,
                             TDataSource> : IEntityBuilder<TId,
                                                           TEntity,
                                                           TCustomData,
                                                           TDataSource>,
                                            IEquatable<TEntity>,
                                            IComparable<TEntity>

        where TId     : IId
        where TEntity : IHasId<TId>

    {


    }

    /// <summary>
    /// The generic interface of an entity.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identification.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TCustomData">The type of the custom data.</typeparam>
    /// <typeparam name="TDataSource">The type of the data source.</typeparam>
    public interface IEntityBuilder<TId,
                                    TEntity,
                                    TCustomData,
                                    TDataSource> : IEntity,
                                                   ICustomData<TCustomData>

        where TId     : IId
        where TEntity : IHasId<TId>

    {

        /// <summary>
        /// The unique identification of this entity.
        /// </summary>
        TId          Id            { get; }

        /// <summary>
        /// The data source of this entity, e.g. an automatic importer.
        /// </summary>
        TDataSource  DataSource    { get; }


        void CopyAllLinkedDataFrom(TEntity OldEnity);


    }


    /// <summary>
    /// An abstract entity.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identification.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class AEntity<TId,
                                  TEntity> : AEntity<TId,
                                                     TEntity,
                                                     JObject,
                                                     String>,
                                             IHasId<TId>

        where TId     : IId
        where TEntity : IHasId<TId>

    {

        /// <summary>
        /// Create a new abstract entity.
        /// </summary>
        /// <param name="Id">The unique identification of this entity.</param>
        /// <param name="JSONLDContext">The JSON-LD context of this entity.</param>
        /// <param name="CustomData">Custom data stored within this entity.</param>
        /// <param name="DataSource">The source of this information, e.g. an automatic importer.</param>
        /// <param name="LastChange">The timestamp of the last changes within this entity. Can e.g. be used as a HTTP ETag.</param>
        protected AEntity(TId            Id,
                          JSONLDContext  JSONLDContext,
                          JObject        CustomData   = default,
                          String         DataSource   = default,
                          DateTime?      LastChange   = default)

            : base(Id,
                   JSONLDContext,
                   CustomData,
                   DataSource,
                   LastChange)

        { }


        #region (protected) ToJSON(JSONLDContext, ...)

        protected JObject ToJSON(Boolean                 Embedded,
                                 Boolean                 IncludeLastChange,
                                 Boolean                 IncludeCryptoHash,
                                 Func<JObject, JObject>  CustomAEntitySerializer   = null,
                                 params JProperty[]      JSONProperties)
        {

            var JSON = JSONObject.Create(

                           new JProperty[] {

                               new JProperty("@id",                Id.           ToString()),

                               Embedded
                                   ? null
                                   : new JProperty("@context",     JSONLDContext.ToString()),

                               CustomData != default
                                   ? new JProperty("customData",   CustomData)
                                   : null,

                               DataSource.IsNotNullOrEmpty()
                                   ? new JProperty("dataSource",   DataSource)
                                   : null,

                               IncludeLastChange
                                   ? new JProperty("lastChange",   LastChange.   ToIso8601())
                                   : null,

                               IncludeCryptoHash
                                   ? new JProperty("hashValue",    HashValue)
                                   : null

                           }.

                           Concat(JSONProperties)

                       );

            return CustomAEntitySerializer != null
                       ? CustomAEntitySerializer(JSON)
                       : JSON;

        }

        #endregion


    }

    /// <summary>
    /// An abstract entity.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identification.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TCustomData">The type of the custom data.</typeparam>
    /// <typeparam name="TDataSource">The type of the data source.</typeparam>
    public abstract class AEntity<TId,
                                  TEntity,
                                  TCustomData,
                                  TDataSource> : IEntity<TId,
                                                         TEntity,
                                                         TCustomData,
                                                         TDataSource>

        where TId     : IId
        where TEntity : IHasId<TId>

    {

        #region Properties

        /// <summary>
        /// The unique identification of this entity.
        /// </summary>
        [Mandatory]
        public TId            Id               { get; }

        /// <summary>
        /// The JSON-LD context of this entity.
        /// </summary>
        [Mandatory]
        public JSONLDContext  JSONLDContext    { get; }

        /// <summary>
        /// Custom data stored within this entity.
        /// </summary>
        [Optional]
        public TCustomData    CustomData       { get; }

        /// <summary>
        /// The data source of this entity, e.g. an automatic importer.
        /// </summary>
        [Optional]
        public TDataSource    DataSource       { get; }

        /// <summary>
        /// The timestamp of the last changes within this entity.
        /// Can e.g. be used as a HTTP ETag.
        /// </summary>
        [Mandatory]
        public DateTime       LastChange       { get; protected set; }

        /// <summary>
        /// The cryptographic hash value of this entity.
        /// </summary>
        public String         HashValue        { get; protected set; }

        #endregion

        #region Events

        public event OnPropertyChangedDelegate OnPropertyChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract entity.
        /// </summary>
        /// <param name="Id">The unique identification of this entity.</param>
        /// <param name="JSONLDContext">The JSON-LD context of this entity.</param>
        /// <param name="CustomData">Custom data stored within this entity.</param>
        /// <param name="DataSource">The source of this information, e.g. an automatic importer.</param>
        /// <param name="LastChange">The timestamp of the last changes within this entity. Can e.g. be used as a HTTP ETag.</param>
        protected AEntity(TId            Id,
                          JSONLDContext  JSONLDContext,
                          TCustomData    CustomData   = default,
                          TDataSource    DataSource   = default,
                          DateTime?      LastChange   = default)

        {

            if (Id.Equals(default))
                throw new ArgumentNullException(nameof(Id),             "The given identification must not be null or empty!");

            if (JSONLDContext.Equals(default))
                throw new ArgumentNullException(nameof(JSONLDContext),  "The given JSON-LD context must not be null or empty!");

            this.Id             = Id;
            this.JSONLDContext  = JSONLDContext;
            this.CustomData     = CustomData;
            this.DataSource     = DataSource;
            this.LastChange     = LastChange ?? DateTime.UtcNow;

        }

        #endregion


        //#region SetProperty<T>(ref FieldToChange, NewValue, [CallerMemberName])

        ///// <summary>
        ///// Change the given field and call the OnPropertyChanged event.
        ///// </summary>
        ///// <typeparam name="T">The type of the field to be changed.</typeparam>
        ///// <param name="FieldToChange">A reference to the field to be changed.</param>
        ///// <param name="NewValue">The new value of the field to be changed.</param>
        ///// <param name="PropertyName">The name of the property to be changed (set by the compiler!)</param>
        //public void SetProperty<T>(ref                T       FieldToChange,
        //                                              T       NewValue,
        //                           [CallerMemberName] String  PropertyName = "")
        //{

        //    if (!EqualityComparer<T>.Default.Equals(FieldToChange, NewValue))
        //    {

        //        var OldValue       = FieldToChange;
        //            FieldToChange  = NewValue;

        //        PropertyChanged(PropertyName, OldValue, NewValue);

        //    }

        //}

        //#endregion

        //#region DeleteProperty<T>(ref FieldToChange, [CallerMemberName])

        ///// <summary>
        ///// Delete the given field and call the OnPropertyChanged event.
        ///// </summary>
        ///// <typeparam name="T">The type of the field to be deleted.</typeparam>
        ///// <param name="FieldToChange">A reference to the field to be deleted.</param>
        ///// <param name="PropertyName">The name of the property to be deleted (set by the compiler!)</param>
        //public void DeleteProperty<T>(ref                T       FieldToChange,
        //                              [CallerMemberName] String  PropertyName = "")
        //{

        //    if (FieldToChange != null)
        //    {

        //        var OldValue       = FieldToChange;
        //            FieldToChange  = default(T);

        //        PropertyChanged(PropertyName, OldValue, default(T));

        //    }

        //}

        //#endregion

        //#region PropertyChanged<T>(PropertyName, OldValue, NewValue)

        ///// <summary>
        ///// Notify subscribers that a property has changed.
        ///// </summary>
        ///// <typeparam name="T">The type of the changed property.</typeparam>
        ///// <param name="PropertyName">The name of the changed property.</param>
        ///// <param name="OldValue">The old value of the changed property.</param>
        ///// <param name="NewValue">The new value of the changed property.</param>
        //public void PropertyChanged<T>(String  PropertyName,
        //                               T       OldValue,
        //                               T       NewValue)
        //{

        //    #region Initial checks

        //    if (PropertyName == null)
        //        throw new ArgumentNullException(nameof(PropertyName), "The given property name must not be null!");

        //    #endregion

        //    this.LastChange = DateTime.UtcNow;

        //    OnPropertyChanged?.Invoke(LastChange,
        //                              this,
        //                              PropertyName,
        //                              OldValue,
        //                              NewValue);

        //}

        //#endregion


        //#region this[PropertyName]

        ///// <summary>
        ///// Return the user-defined property for the given property name.
        ///// </summary>
        ///// <param name="PropertyName">The name of the user-defined property.</param>
        //public Object this[String PropertyName]
        //{

        //    get
        //    {
        //        return CustomData[PropertyName];
        //    }

        //    set
        //    {
        //        CustomData[PropertyName] = value;
        //    }

        //}

        //#endregion

        //#region RemoveUserDefinedProperty()

        ///// <summary>
        ///// Try to remove a user-defined property.
        ///// </summary>
        ///// <param name="PropertyName"></param>
        //public void RemoveUserDefinedProperty(String PropertyName)
        //{
        //    CustomData.TryRemove(PropertyName, out Object Value);
        //}

        //#endregion


        public abstract void CopyAllLinkedDataFrom(TEntity OldEnity);



        #region ToJSON(Embedded = false, IncludeCryptoHash = false)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public abstract JObject ToJSON(Boolean Embedded           = false,
                                       Boolean IncludeCryptoHash  = false);

        #endregion


        #region CalcHash()

        /// <summary>
        /// Calculate the hash value of this object.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        public void CalcHash()
        {

            using (var SHA256 = new SHA256Managed())
            {

                HashValue = "json:sha256:" +
                            SHA256.ComputeHash(Encoding.Unicode.GetBytes(
                                                   ToJSON  (IncludeCryptoHash: false).
                                                   ToString(Newtonsoft.Json.Formatting.None)
                                               )).
                                   Select(value => String.Format("{0:x2}", value)).
                                   Aggregate();

            }

        }

        #endregion



        public Int32 CompareTo(TId OtherId)
            => Id.CompareTo(OtherId);

        public Boolean Equals(TId OtherId)
            => Id.Equals(OtherId);



        public abstract Boolean Equals   (TEntity other);

        public abstract Int32   CompareTo(TEntity other);

        public abstract Int32   CompareTo(Object  obj);





        #region (class) Builder

        /// <summary>
        /// An abstract builder.
        /// </summary>
        public abstract class Builder : IEntityBuilder<TId,
                                                       TEntity,
                                                       TCustomData,
                                                       TDataSource>
        {

            #region Properties

            /// <summary>
            /// The unique identification of this entity.
            /// </summary>
            [Mandatory]
            public  TId           Id               { get; set; }

            /// <summary>
            /// The JSON-LD context of this entity.
            /// </summary>
            [Mandatory]
            public JSONLDContext  JSONLDContext    { get; set; }

            /// <summary>
            /// Custom data stored within this entity.
            /// </summary>
            [Optional]
            public TCustomData    CustomData       { get; set; }

            /// <summary>
            /// The data source of this entity, e.g. an automatic importer.
            /// </summary>
            [Optional]
            public TDataSource    DataSource       { get; set; }

            /// <summary>
            /// The timestamp of the last changes within this entity.
            /// Can e.g. be used as a HTTP ETag.
            /// </summary>
            [Mandatory]
            public DateTime       LastChange       { get; set; }

            /// <summary>
            /// The cryptographic hash value of this entity.
            /// </summary>
            public String         HashValue        { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new user group builder.
            /// </summary>
            /// <param name="Id">The unique identification of this entity.</param>
            /// <param name="JSONLDContext">The JSON-LD context of this entity.</param>
            /// <param name="CustomData">Custom data stored within this entity.</param>
            /// <param name="DataSource">The source of this information, e.g. an automatic importer.</param>
            /// <param name="LastChange">The timestamp of the last changes within this entity. Can e.g. be used as a HTTP ETag.</param>
            public Builder(TId            Id,
                           JSONLDContext  JSONLDContext,
                           TCustomData    CustomData   = default,
                           TDataSource    DataSource   = default,
                           DateTime?      LastChange   = default)

            {

                this.Id             = Id;
                this.JSONLDContext  = JSONLDContext;
                this.CustomData     = CustomData;
                this.DataSource     = DataSource;
                this.LastChange     = LastChange ?? DateTime.UtcNow;
                this.HashValue      = HashValue;

            }

            #endregion


            public event OnPropertyChangedDelegate OnPropertyChanged;


            public abstract void CopyAllLinkedDataFrom(TEntity OldEnity);




            public Int32 CompareTo(TId OtherId)
                => Id.CompareTo(OtherId);

            public Boolean Equals(TId OtherId)
                => Id.Equals(OtherId);


            //public abstract bool Equals(TEntity other);

            //public abstract int CompareTo(TEntity other);

            public abstract Int32 CompareTo(Object obj);


        }

        #endregion



    }

}
