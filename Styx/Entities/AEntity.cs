/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// The interface for custom data stored with an entity.
    /// </summary>
    public interface ICustomData
    { }

    ///// <summary>
    ///// The generic interface for custom data stored with an entity.
    ///// </summary>
    ///// <typeparam name="TCustomData">The type of the custom data.</typeparam>
    //public interface ICustomData<TCustomData> : ICustomData
    //{

    //    /// <summary>
    //    /// Custom data stored within this entity.
    //    /// </summary>
    //    TCustomData?  CustomData    { get; }

    //}


    /// <summary>
    /// The interface of an entity.
    /// </summary>
    public interface IEntity : IInternalData,
                               IComparable
    {

        I18NString  Name               { get; }

        I18NString  Description        { get; }

        String?     DataSource         { get; }


        //event OnPropertyChangedDelegate? OnPropertyChanged;

    }


    /// <summary>
    /// The common generic interface of an entity having one or multiple unique identification(s).
    /// </summary>
    /// <typeparam name="TId">THe type of the unique identificator.</typeparam>
    public interface IEntity<TId> : IEntity, IHasId<TId>

        where TId : IId

    {

    }


    /// <summary>
    /// The generic interface of an entity.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identification.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TCustomData">The type of the custom data.</typeparam>
    /// <typeparam name="TDataSource">The type of the data source.</typeparam>
    public interface IEntity<TId, TEntity> : IEntity,
                                             IHasId<TId>,
                                             IEquatable<TEntity>,
                                             IComparable<TEntity>

        where TId     : IId
        where TEntity : IHasId<TId>

    { }


    /// <summary>
    /// The generic interface of an entity.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identification.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IEntityBuilder<TId, TEntity> : IEntity,
                                                    IHasId<TId>,
                                                    IEquatable<TEntity>,
                                                    IComparable<TEntity>

        where TId     : IId
        where TEntity : IHasId<TId>

    {

        ///// <summary>
        ///// The unique identification of this entity.
        ///// </summary>
        //TId           Id            { get; }

        ///// <summary>
        ///// The data source of this entity, e.g. an automatic importer.
        ///// </summary>
        //TDataSource?  DataSource    { get; }


        void CopyAllLinkedDataFrom(TEntity OldEnity);


    }


    /// <summary>
    /// An abstract entity.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identification.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    //public abstract class AEntity<TId,
    //                              TEntity> : AEntity<TId,
    //                                                 TEntity,
    //                                                 JObject,
    //                                                 String>,
    //                                         IHasId<TId>

    //    where TId     : IId
    //    where TEntity : IHasId<TId>

    //{

    //    /// <summary>
    //    /// Create a new abstract entity.
    //    /// </summary>
    //    /// <param name="Id">The unique identification of this entity.</param>
    //    /// <param name="JSONLDContext">The JSON-LD context of this entity.</param>
    //    /// <param name="CustomData">Custom data stored within this entity.</param>
    //    /// <param name="DataSource">The source of this information, e.g. an automatic importer.</param>
    //    /// <param name="LastChange">The timestamp of the last changes within this entity. Can e.g. be used as a HTTP ETag.</param>
    //    protected AEntity(TId                      Id,
    //                      JSONLDContext            JSONLDContext,
    //                      DateTime?                LastChange   = default,
    //                      IEnumerable<Signature>?  Signatures   = default,
    //                      JObject?                 CustomData   = default,
    //                      String?                  DataSource   = default)

    //        : base(Id,
    //               JSONLDContext,
    //               LastChange,
    //               Signatures,
    //               CustomData,
    //               DataSource)

    //    { }


        #region (protected) ToJSON(JSONLDContext, ...)

        //protected JObject ToJSON(Boolean                  Embedded,
        //                         Boolean                  IncludeLastChange,
        //                         Func<JObject, JObject>?  CustomAEntitySerializer   = null,
        //                         params JProperty?[]      JSONProperties)
        //{

        //    var JSON = JSONObject.Create(

        //                   new JProperty?[] {

        //                       new JProperty("@id",                    Id.            ToString()),

        //                       Embedded
        //                           ? null
        //                           : new JProperty("@context",         JSONLDContext. ToString()),

        //                       CustomData is not null
        //                           ? new JProperty("customData",       CustomData)
        //                           : null,

        //                       DataSource.IsNotNullOrEmpty()
        //                           ? new JProperty("dataSource",       DataSource)
        //                           : null,

        //                       IncludeLastChange
        //                           ? new JProperty("lastChangeDate",   LastChange.ToIso8601())
        //                           : null

        //                   }.

        //                   Concat(JSONProperties)

        //               );

        //    return CustomAEntitySerializer is not null
        //               ? CustomAEntitySerializer(JSON)
        //               : JSON;

        //}

        #endregion


    //}

    /// <summary>
    /// An abstract entity.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identification.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class AEntity<TId, TEntity> : AInternalData,
                                                  IEntity<TId, TEntity>

        where TId     : IId
        where TEntity : IHasId<TId>

    {

        #region Properties

        /// <summary>
        /// The unique identification of this entity.
        /// </summary>
        [Mandatory]
        public TId                      Id               { get; }

        /// <summary>
        /// The multi-language name of this entity.
        /// </summary>
        [Optional]
        public I18NString               Name             { get; }

        /// <summary>
        /// The multi-language description of this entity.
        /// </summary>
        [Optional]
        public I18NString               Description      { get; }

        /// <summary>
        /// The JSON-LD context of this entity.
        /// </summary>
        [Mandatory]
        public JSONLDContext            JSONLDContext    { get; }

        ///// <summary>
        ///// The timestamp of the last changes within this entity.
        ///// Can e.g. be used as a HTTP ETag.
        ///// </summary>
        //[Mandatory]
        //public DateTime                 LastChange   { get; protected set; }

        /// <summary>
        /// All signatures of this blog posting.
        /// </summary>
        [Optional]
        public IEnumerable<Signature>?  Signatures       { get; }

        ///// <summary>
        ///// Custom data stored within this entity.
        ///// </summary>
        //[Optional]
        //public TCustomData?             CustomData       { get; }

        /// <summary>
        /// The data source of this entity, e.g. an automatic importer.
        /// </summary>
        [Optional]
        public String?             DataSource       { get; }

        #endregion

        #region Events

        public event OnPropertyChangedDelegate?  OnPropertyChanged;

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
        protected AEntity(TId                      Id,
                          JSONLDContext            JSONLDContext,
                          I18NString?              Name           = null,
                          I18NString?              Description    = null,
                          IEnumerable<Signature>?  Signatures     = default,
                          JObject?                 CustomData     = null,
                          UserDefinedDictionary?   InternalData   = null,
                          DateTime?                LastChange     = null,
                          String?                  DataSource     = default)

            : base(CustomData,
                   InternalData,
                   LastChange)

        {

            if (Id.Equals(default))
                throw new ArgumentNullException(nameof(Id),             "The given identification must not be null or empty!");

            if (JSONLDContext.Equals(default))
                throw new ArgumentNullException(nameof(JSONLDContext),  "The given JSON-LD context must not be null or empty!");

            this.Id              = Id;
            this.JSONLDContext   = JSONLDContext;
            this.Name            = Name        ?? I18NString.Empty;
            this.Description     = Description ?? I18NString.Empty;
            this.Signatures      = Signatures;
            this.DataSource      = DataSource;

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

        //    this.LastChange = Timestamp.Now;

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



        #region ToJSON(Embedded = false)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        public abstract JObject ToJSON(Boolean Embedded = false);

        #endregion

        #region (protected) ToJSON(JSONLDContext, ...)

        protected JObject ToJSON(Boolean                  Embedded,
                                 Boolean                  IncludeLastChange,
                                 Func<JObject, JObject>?  CustomAEntitySerializer   = null,
                                 params JProperty?[]      JSONProperties)
        {

            var JSON = JSONObject.Create(

                           new JProperty?[] {

                                     new JProperty("@id",          Id.            ToString()),

                               Embedded
                                   ? null
                                   : new JProperty("@context",     JSONLDContext. ToString()),

                               CustomData is not null
                                   ? new JProperty("customData",   CustomData)
                                   : null,

                               DataSource.IsNotNullOrEmpty()
                                   ? new JProperty("dataSource",   DataSource)
                                   : null,

                               IncludeLastChange
                                   ? new JProperty("lastChange",   LastChangeDate.ToIso8601())
                                   : null

                           }.

                           Concat(JSONProperties)

                       );

            return CustomAEntitySerializer is not null
                       ? CustomAEntitySerializer(JSON)
                       : JSON;

        }

        #endregion


        #region CalcHash()

        ///// <summary>
        ///// Calculate the hash value of this object.
        ///// </summary>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        //public void CalcHash()
        //{

        //    using (var SHA256 = new SHA256Managed())
        //    {

        //        HashValue = "json:sha256:" +
        //                    SHA256.ComputeHash(Encoding.Unicode.GetBytes(
        //                                           ToJSON  (IncludeCryptoHash: false).
        //                                           ToString(Newtonsoft.Json.Formatting.None)
        //                                       )).
        //                           Select(value => String.Format("{0:x2}", value)).
        //                           Aggregate();

        //    }

        //}

        #endregion



        public Int32 CompareTo(TId OtherId)
            => Id.CompareTo(OtherId);

        public Boolean Equals(TId OtherId)
            => Id.Equals(OtherId);



        public abstract Boolean Equals   (TEntity? other);

        public abstract Int32   CompareTo(TEntity? other);

        public abstract Int32   CompareTo(Object?  obj);



        public ComparizionResult CompareWith(AEntity<TId, TEntity> Entity)
        {

            var Added    = new List<ComparizionResult.PropertyWithValue>();
            var Updated  = new List<ComparizionResult.PropertyWithValues>();
            var Removed  = new List<ComparizionResult.PropertyWithValue>();

            var propertyInfos = GetType().GetProperties().
                                          Where(info => info.CustomAttributes.SafeAny() &&
                                                        info.CustomAttributes.Any(attr => attr.AttributeType == typeof(MandatoryAttribute) ||
                                                                                          attr.AttributeType == typeof(OptionalAttribute))).
                                          ToArray();

            foreach (var pinfo in propertyInfos)
            {

                var getter1 = Entity.GetType().InvokeMember(pinfo.Name, System.Reflection.BindingFlags.GetProperty, null, Entity, Array.Empty<Object>());
                var getter2 = this.  GetType().InvokeMember(pinfo.Name, System.Reflection.BindingFlags.GetProperty, null, this,   Array.Empty<Object>());

                if (getter1 is null && getter2 is null)
                { }

                else if (getter1 is null)
                    Added.  Add(new ComparizionResult.PropertyWithValue (pinfo.Name, getter2));

                else if (getter2 is null)
                    Removed.Add(new ComparizionResult.PropertyWithValue (pinfo.Name, getter1));

                else if (!getter1.Equals(getter2))
                    Updated.Add(new ComparizionResult.PropertyWithValues(pinfo.Name, getter1, getter2));

            }

            return new ComparizionResult(Added,
                                         Updated,
                                         Removed);

        }




        #region (class) Builder

        /// <summary>
        /// An abstract builder.
        /// </summary>
        public abstract new class Builder : AInternalData.Builder,
                                            IEntityBuilder<TId, TEntity>
        {

            #region Properties

            /// <summary>
            /// The unique identification of this entity.
            /// </summary>
            [Mandatory]
            public  TId                Id               { get; set; }

            /// <summary>
            /// The multi-language name of this entity.
            /// </summary>
            [Optional]
            public I18NString?         Name             { get; set; }

            /// <summary>
            /// The multi-language description of this entity.
            /// </summary>
            [Optional]
            public I18NString?         Description      { get; set; }

            /// <summary>
            /// The JSON-LD context of this entity.
            /// </summary>
            [Mandatory]
            public JSONLDContext       JSONLDContext    { get; set; }

            ///// <summary>
            ///// The timestamp of the last changes within this entity.
            ///// Can e.g. be used as a HTTP ETag.
            ///// </summary>
            //[Mandatory]
            //public DateTime            LastChange   { get; set; }

            /// <summary>
            /// All signatures of this blog posting.
            /// </summary>
            [Optional]
            public HashSet<Signature>  Signatures       { get; }

            ///// <summary>
            ///// Custom data stored within this entity.
            ///// </summary>
            //[Optional]
            //public TCustomData?        CustomData       { get; set; }

            /// <summary>
            /// The data source of this entity, e.g. an automatic importer.
            /// </summary>
            [Optional]
            public String?        DataSource       { get; set; }

            #endregion

            #region Events

            public event OnPropertyChangedDelegate?  OnPropertyChanged;

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
            public Builder(TId                      Id,
                           JSONLDContext            JSONLDContext,
                           DateTime?                LastChange     = default,
                           IEnumerable<Signature>?  Signatures     = default,
                           JObject?                 CustomData     = null,
                           UserDefinedDictionary?   InternalData   = null,
                           String?                  DataSource     = default)

                : base(CustomData,
                       InternalData,
                       LastChange)

            {

                this.Id              = Id;
                this.JSONLDContext   = JSONLDContext;
                this.Signatures      = Signatures is not null
                                           ? new HashSet<Signature>(Signatures)
                                           : new HashSet<Signature>();
                this.DataSource      = DataSource;

            }

            #endregion


            public abstract void CopyAllLinkedDataFrom(TEntity OldEnity);



            public Int32 CompareTo(TId OtherId)
                => Id.CompareTo(OtherId);

            public Boolean Equals(TId OtherId)
                => Id.Equals(OtherId);


            public abstract Boolean Equals   (TEntity? other);

            public abstract Int32   CompareTo(TEntity? other);

            public abstract Int32   CompareTo(Object?  obj);


        }

        #endregion



    }

}
