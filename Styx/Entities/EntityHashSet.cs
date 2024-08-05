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

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Illias.Votes;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public class EntityHashSet<TParentDataStructure, TId, TEntity> : IEnumerable<TEntity>

        where TParentDataStructure: class
        where TEntity:              class, IEntity<TId>
        where TId:                  IId

    {


        #region (class) AddResult

        /// <summary>
        /// The result of an add request.
        /// </summary>
        public class AddResult : AEnitityResult<TEntity, TId>
        {

            #region Properties

            public TEntity?               Entity
                => base.Entity;

            public TParentDataStructure?  ParentDataStructure    { get; internal set; }

            #endregion

            #region Constructor(s)

            public AddResult(TEntity                Entity,
                             CommandResult          Result,
                             EventTracking_Id?      EventTrackingId       = null,
                             IId?                   SenderId              = null,
                             Object?                Sender                = null,
                             TParentDataStructure?  ParentDataStructure   = null,
                             I18NString?            Description           = null,
                             IEnumerable<Warning>?  Warnings              = null,
                             TimeSpan?              Runtime               = null)

                : base(Entity,
                       Result,
                       EventTrackingId,
                       SenderId,
                       Sender,
                       Description,
                       Warnings,
                       Runtime)

            {

                this.ParentDataStructure = ParentDataStructure;

            }

            #endregion


            #region (static) AdminDown    (TEntity, ...)

            public static AddResult

                AdminDown(TEntity                Entity,
                          EventTracking_Id?      EventTrackingId       = null,
                          IId?                   SenderId              = null,
                          Object?                Sender                = null,
                          TParentDataStructure?  ParentDataStructure   = null,
                          I18NString?            Description           = null,
                          IEnumerable<Warning>?  Warnings              = null,
                          TimeSpan?              Runtime               = null)

                    => new (Entity,
                            CommandResult.AdminDown,
                            EventTrackingId,
                            SenderId,
                            Sender,
                            ParentDataStructure,
                            Description,
                            Warnings,
                            Runtime);

            #endregion

            #region (static) NoOperation  (TEntity, ...)

            public static AddResult

                NoOperation(TEntity                Entity,
                            EventTracking_Id?      EventTrackingId       = null,
                            IId?                   SenderId              = null,
                            Object?                Sender                = null,
                            TParentDataStructure?  ParentDataStructure   = null,
                            I18NString?            Description           = null,
                            IEnumerable<Warning>?  Warnings              = null,
                            TimeSpan?              Runtime               = null)

                    => new (Entity,
                            CommandResult.NoOperation,
                            EventTrackingId,
                            SenderId,
                            Sender,
                            ParentDataStructure,
                            Description,
                            Warnings,
                            Runtime);

            #endregion


            #region (static) Enqueued     (TEntity, ...)

            public static AddResult

                Enqueued(TEntity                Entity,
                         EventTracking_Id?      EventTrackingId       = null,
                         IId?                   SenderId              = null,
                         Object?                Sender                = null,
                         TParentDataStructure?  ParentDataStructure   = null,
                         I18NString?            Description           = null,
                         IEnumerable<Warning>?  Warnings              = null,
                         TimeSpan?              Runtime               = null)

                    => new (Entity,
                            CommandResult.Enqueued,
                            EventTrackingId,
                            SenderId,
                            Sender,
                            ParentDataStructure,
                            Description,
                            Warnings,
                            Runtime);

            #endregion

            #region (static) Success      (TEntity, ...)

            public static AddResult

                Success(TEntity                Entity,
                        EventTracking_Id?      EventTrackingId       = null,
                        IId?                   SenderId              = null,
                        Object?                Sender                = null,
                        TParentDataStructure?  ParentDataStructure   = null,
                        I18NString?            Description           = null,
                        IEnumerable<Warning>?  Warnings              = null,
                        TimeSpan?              Runtime               = null)

                    => new (Entity,
                            CommandResult.Success,
                            EventTrackingId,
                            SenderId,
                            Sender,
                            ParentDataStructure,
                            Description,
                            Warnings,
                            Runtime);

            #endregion


            #region (static) ArgumentError(TEntity, Description, ...)

            public static AddResult

                ArgumentError(TEntity                Entity,
                              I18NString             Description,
                              EventTracking_Id?      EventTrackingId       = null,
                              IId?                   SenderId              = null,
                              Object?                Sender                = null,
                              TParentDataStructure?  ParentDataStructure   = null,
                              IEnumerable<Warning>?  Warnings              = null,
                              TimeSpan?              Runtime               = null)

                    => new (Entity,
                            CommandResult.ArgumentError,
                            EventTrackingId,
                            SenderId,
                            Sender,
                            ParentDataStructure,
                            Description,
                            Warnings,
                            Runtime);

            #endregion

            #region (static) Error        (TEntity, Description, ...)

            public static AddResult

                Error(TEntity                Entity,
                      I18NString             Description,
                      EventTracking_Id?      EventTrackingId       = null,
                      IId?                   SenderId              = null,
                      Object?                Sender                = null,
                      TParentDataStructure?  ParentDataStructure   = null,
                      IEnumerable<Warning>?  Warnings              = null,
                      TimeSpan?              Runtime               = null)

                    => new (Entity,
                            CommandResult.Error,
                            EventTrackingId,
                            SenderId,
                            Sender,
                            ParentDataStructure,
                            Description,
                            Warnings,
                            Runtime);

            #endregion

            #region (static) Error        (TEntity, Exception,   ...)

            public static AddResult

                Error(TEntity                Entity,
                      Exception              Exception,
                      EventTracking_Id?      EventTrackingId       = null,
                      IId?                   SenderId              = null,
                      Object?                Sender                = null,
                      TParentDataStructure?  ParentDataStructure   = null,
                      IEnumerable<Warning>?  Warnings              = null,
                      TimeSpan?              Runtime               = null)

                    => new (Entity,
                            CommandResult.Error,
                            EventTrackingId,
                            SenderId,
                            Sender,
                            ParentDataStructure,
                            Exception.Message.ToI18NString(),
                            Warnings,
                            Runtime);

            #endregion

            #region (static) Timeout      (TEntity, Timeout,     ...)

            public static AddResult

                Timeout(TEntity                Entity,
                        TimeSpan               Timeout,
                        EventTracking_Id?      EventTrackingId       = null,
                        IId?                   SenderId              = null,
                        Object?                Sender                = null,
                        TParentDataStructure?  ParentDataStructure   = null,
                        IEnumerable<Warning>?  Warnings              = null,
                        TimeSpan?              Runtime               = null)

                    => new (Entity,
                            CommandResult.Timeout,
                            EventTrackingId,
                            SenderId,
                            Sender,
                            ParentDataStructure,
                            $"Timeout after {Timeout.TotalSeconds} seconds!".ToI18NString(),
                            Warnings,
                            Runtime);

            #endregion

            #region (static) LockTimeout  (TEntity, Timeout,     ...)

            public static AddResult

                LockTimeout(TEntity                Entity,
                            TimeSpan               Timeout,
                            EventTracking_Id?      EventTrackingId       = null,
                            IId?                   SenderId              = null,
                            Object?                Sender                = null,
                            TParentDataStructure?  ParentDataStructure   = null,
                            IEnumerable<Warning>?  Warnings              = null,
                            TimeSpan?              Runtime               = null)

                    => new (Entity,
                            CommandResult.LockTimeout,
                            EventTrackingId,
                            SenderId,
                            Sender,
                            ParentDataStructure,
                            $"Lock timeout after {Timeout.TotalSeconds} seconds!".ToI18NString(),
                            Warnings,
                            Runtime);

            #endregion

        }

        #endregion



        #region Data

        private readonly TParentDataStructure                parentDataStructure;

        private readonly ConcurrentDictionary<TId, TEntity>  lookup;

        #endregion

        #region Events

        #region OnAddition

        private readonly IVotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, Boolean> addition;

        /// <summary>
        /// Called whenever an entity will be or was added.
        /// </summary>
        public IVotingSender<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, Boolean> OnAddition
            => addition;

        #endregion

        #region OnAdditionIfNotExists

        private readonly IVotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, Boolean> additionIfNotExists;

        /// <summary>
        /// Called whenever an entity will be or was added.
        /// </summary>
        public IVotingSender<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, Boolean> OnAdditionIfNotExists
            => additionIfNotExists;

        #endregion

        #region OnAddOrUpdate

        private readonly IVotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, TEntity, Boolean> addOrUpdate;

        /// <summary>
        /// Called whenever an entity will be or was added or updated.
        /// </summary>
        public IVotingSender<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, TEntity, Boolean> OnAddOrUpdate
            => addOrUpdate;

        #endregion

        #region OnUpdate

        private readonly IVotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, TEntity, Boolean> update;

        /// <summary>
        /// Called whenever an entity will be or was updated.
        /// </summary>
        public IVotingSender<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, TEntity, Boolean> OnUpdate
            => update;

        #endregion

        #region OnRemoval

        private readonly IVotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, Boolean> removal;

        /// <summary>
        /// Called whenever an entity will be or was removed.
        /// </summary>
        public IVotingSender<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, Boolean> OnRemoval
            => removal;

        #endregion

        #endregion

        #region Constructor(s)

        public EntityHashSet(TParentDataStructure                                                                                       ParentDataStructure,

                             IVotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity,          Boolean>?  Addition              = null,
                             IVotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity,          Boolean>?  AdditionIfNotExists   = null,
                             IVotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, TEntity, Boolean>?  AddOrUpdate           = null,
                             IVotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, TEntity, Boolean>?  Update                = null,
                             IVotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity,          Boolean>?  Removal               = null)
        {

            this.lookup               = new ConcurrentDictionary<TId, TEntity>();

            this.parentDataStructure  = ParentDataStructure;

            this.addition             = Addition            ?? new VotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity,          Boolean>(() => new VetoVote(), true);
            this.additionIfNotExists  = AdditionIfNotExists ?? new VotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity,          Boolean>(() => new VetoVote(), true);
            this.addOrUpdate          = AddOrUpdate         ?? new VotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, TEntity, Boolean>(() => new VetoVote(), true);
            this.update               = Update              ?? new VotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity, TEntity, Boolean>(() => new VetoVote(), true);
            this.removal              = Removal             ?? new VotingNotificator<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity,          Boolean>(() => new VetoVote(), true);

        }

        #endregion


        #region TryAdd(Entity, ...)

        public AddResult TryAdd(TEntity           Entity,
                                EventTracking_Id  EventTrackingId,
                                User_Id?          CurrentUserId)
        {

            if (addition.SendVoting(Timestamp.Now,
                                    EventTrackingId,
                                    CurrentUserId ?? User_Id.Anonymous,
                                    parentDataStructure,
                                    Entity) &&

                lookup.TryAdd(Entity.Id, Entity))

            {

                addition.SendNotification(Timestamp.Now,
                                          EventTrackingId,
                                          CurrentUserId ?? User_Id.Anonymous,
                                          parentDataStructure,
                                          Entity);

                return AddResult.Success(
                           Entity,
                           EventTrackingId
                       );

            }

            return AddResult.Error(
                       Entity,
                       I18NString.Empty,
                       EventTrackingId
                   );

        }


        /// <summary>
        /// Try to add the given entity to the hashset.
        /// </summary>
        /// <param name="Entity">An entity.</param>
        /// <param name="OnSuccess">A delegate called after adding the entity, but before the notifications are send.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public AddResult TryAdd(TEntity           Entity,
                                Action<TEntity>   OnSuccess,
                                EventTracking_Id  EventTrackingId,
                                User_Id?          CurrentUserId)
        {

            if (addition.SendVoting(Timestamp.Now,
                                    EventTrackingId,
                                    CurrentUserId ?? User_Id.Anonymous,
                                    parentDataStructure,
                                    Entity) &&

                lookup.TryAdd(Entity.Id, Entity))

            {

                OnSuccess?.Invoke(Entity);

                addition.SendNotification(Timestamp.Now,
                                          EventTrackingId,
                                          CurrentUserId ?? User_Id.Anonymous,
                                          parentDataStructure,
                                          Entity);

                return AddResult.Success(Entity,
                                         EventTrackingId);

            }

            return AddResult.Error(
                       Entity,
                       I18NString.Empty,
                       EventTrackingId
                   );

        }

        public AddResult TryAdd(TEntity                    Entity,
                                Action<DateTime, TEntity>  OnSuccess,
                                EventTracking_Id           EventTrackingId,
                                User_Id?                   CurrentUserId)
        {

            if (addition.SendVoting(Timestamp.Now,
                                    EventTrackingId,
                                    CurrentUserId ?? User_Id.Anonymous,
                                    parentDataStructure,
                                    Entity) &&

                lookup.TryAdd(Entity.Id, Entity))

            {

                OnSuccess?.Invoke(Timestamp.Now, Entity);

                addition.SendNotification(Timestamp.Now,
                                          EventTrackingId,
                                          CurrentUserId ?? User_Id.Anonymous,
                                          parentDataStructure,
                                          Entity);

                return AddResult.Success(Entity,
                                         EventTrackingId);

            }

            return AddResult.Error(
                       Entity,
                       I18NString.Empty,
                       EventTrackingId
                   );

        }

        public AddResult TryAdd(TEntity                                                                     Entity,
                                Action<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity>  OnSuccess,
                                EventTracking_Id                                                            EventTrackingId,
                                User_Id?                                                                    CurrentUserId)
        {

            var userId = CurrentUserId ?? User_Id.Anonymous;

            if (addition.SendVoting(Timestamp.Now,
                                    EventTrackingId,
                                    userId,
                                    parentDataStructure,
                                    Entity) &&

                lookup.TryAdd(Entity.Id, Entity))

            {

                OnSuccess?.Invoke(Timestamp.Now,
                                  EventTrackingId,
                                  userId,
                                  parentDataStructure,
                                  Entity);

                addition.SendNotification(Timestamp.Now,
                                          EventTrackingId,
                                          CurrentUserId ?? User_Id.Anonymous,
                                          parentDataStructure,
                                          Entity);

                return AddResult.Success(Entity,
                                         EventTrackingId);

            }

            return AddResult.Error(
                       Entity,
                       I18NString.Empty,
                       EventTrackingId
                   );

        }

        #endregion

        #region TryAdd(Entities, ...)

        public Boolean TryAdd(IEnumerable<TEntity>  Entities,
                              EventTracking_Id      EventTrackingId,
                              User_Id?              CurrentUserId)
        {

            var currentUserId = CurrentUserId ?? User_Id.Anonymous;

            // Only when all are allowed we will go on!
            if (Entities.All(Entity => addition.SendVoting(Timestamp.Now,
                                                           EventTrackingId,
                                                           currentUserId,
                                                           parentDataStructure,
                                                           Entity)))
            {

                foreach (var entity in Entities)
                {

                    lookup.TryAdd(entity.Id, entity);

                    addition.SendNotification(Timestamp.Now,
                                              EventTrackingId,
                                              currentUserId,
                                              parentDataStructure,
                                              entity);

                }

                return true;

            }

            return false;

        }

        public Boolean TryAdd(IEnumerable<TEntity>          Entities,
                              Action<IEnumerable<TEntity>>  OnSuccess,
                              EventTracking_Id              EventTrackingId,
                              User_Id?                      CurrentUserId)
        {

            var currentUserId = CurrentUserId ?? User_Id.Anonymous;

            // Only when all are allowed we will go on!
            if (Entities.All(Entity => addition.SendVoting(Timestamp.Now,
                                                           EventTrackingId,
                                                           currentUserId,
                                                           parentDataStructure,
                                                           Entity)))
            {

                foreach (var entity in Entities)
                {

                    lookup.TryAdd(entity.Id, entity);

                    addition.SendNotification(Timestamp.Now,
                                              EventTrackingId,
                                              currentUserId,
                                              parentDataStructure,
                                              entity);

                }

                OnSuccess?.Invoke(Entities);
                return true;

            }

            return false;

        }

        public Boolean TryAdd(IEnumerable<TEntity>                    Entities,
                              Action<DateTime, IEnumerable<TEntity>>  OnSuccess,
                              EventTracking_Id                        EventTrackingId,
                              User_Id?                                CurrentUserId)
        {

            var currentUserId = CurrentUserId ?? User_Id.Anonymous;

            // Only when all are allowed we will go on!
            if (Entities.All(Entity => addition.SendVoting(Timestamp.Now,
                                                           EventTrackingId,
                                                           currentUserId,
                                                           parentDataStructure,
                                                           Entity)))
            {

                foreach (var entity in Entities)
                {

                    lookup.TryAdd(entity.Id, entity);

                    addition.SendNotification(Timestamp.Now,
                                              EventTrackingId,
                                              currentUserId,
                                              parentDataStructure,
                                              entity);

                }

                OnSuccess?.Invoke(Timestamp.Now, Entities);
                return true;

            }

            return false;

        }

        public Boolean TryAdd(IEnumerable<TEntity>                                          Entities,
                              Action<DateTime, TParentDataStructure, IEnumerable<TEntity>>  OnSuccess,
                              EventTracking_Id                                              EventTrackingId,
                              User_Id?                                                      CurrentUserId)
        {

            var currentUserId = CurrentUserId ?? User_Id.Anonymous;

            // Only when all are allowed we will go on!
            if (Entities.All(Entity => addition.SendVoting(Timestamp.Now,
                                                           EventTrackingId,
                                                           currentUserId,
                                                           parentDataStructure,
                                                           Entity)))
            {

                foreach (var entity in Entities)
                {

                    lookup.TryAdd(entity.Id, entity);

                    addition.SendNotification(Timestamp.Now,
                                              EventTrackingId,
                                              currentUserId,
                                              parentDataStructure,
                                              entity);

                }

                OnSuccess?.Invoke(Timestamp.Now, parentDataStructure, Entities);
                return true;

            }

            return false;

        }

        #endregion


        #region TryAddIfNotExists(Entity, ...)

        public AddResult TryAddIfNotExists(TEntity           Entity,
                                           EventTracking_Id  EventTrackingId,
                                           User_Id?          CurrentUserId)
        {

            if (additionIfNotExists.SendVoting(Timestamp.Now,
                                               EventTrackingId,
                                               CurrentUserId ?? User_Id.Anonymous,
                                               parentDataStructure,
                                               Entity) &&

                lookup.TryAdd(Entity.Id, Entity))

            {

                additionIfNotExists.SendNotification(Timestamp.Now,
                                                     EventTrackingId,
                                                     CurrentUserId ?? User_Id.Anonymous,
                                                     parentDataStructure,
                                                     Entity);

                return AddResult.Success(Entity,
                                         EventTrackingId);

            }

            return AddResult.NoOperation(Entity,
                                         EventTrackingId);

        }


        /// <summary>
        /// Try to add the given entity to the hashset.
        /// </summary>
        /// <param name="Entity">An entity.</param>
        /// <param name="OnSuccess">A delegate called after adding the entity, but before the notifications are send.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public AddResult TryAddIfNotExists(TEntity           Entity,
                                           Action<TEntity>   OnSuccess,
                                           EventTracking_Id  EventTrackingId,
                                           User_Id?          CurrentUserId)
        {

            if (additionIfNotExists.SendVoting(Timestamp.Now,
                                               EventTrackingId,
                                               CurrentUserId ?? User_Id.Anonymous,
                                               parentDataStructure,
                                               Entity) &&

                lookup.TryAdd(Entity.Id, Entity))

            {

                OnSuccess?.Invoke(Entity);

                additionIfNotExists.SendNotification(Timestamp.Now,
                                                     EventTrackingId,
                                                     CurrentUserId ?? User_Id.Anonymous,
                                                     parentDataStructure,
                                                     Entity);

                return AddResult.Success(Entity,
                                         EventTrackingId);

            }

            return AddResult.NoOperation(Entity,
                                         EventTrackingId);

        }

        public AddResult TryAddIfNotExists(TEntity                    Entity,
                                           Action<DateTime, TEntity>  OnSuccess,
                                           EventTracking_Id           EventTrackingId,
                                           User_Id?                   CurrentUserId)
        {

            if (additionIfNotExists.SendVoting(Timestamp.Now,
                                               EventTrackingId,
                                               CurrentUserId ?? User_Id.Anonymous,
                                               parentDataStructure,
                                               Entity) &&

                lookup.TryAdd(Entity.Id, Entity))

            {

                OnSuccess?.Invoke(Timestamp.Now, Entity);

                additionIfNotExists.SendNotification(Timestamp.Now,
                                                     EventTrackingId,
                                                     CurrentUserId ?? User_Id.Anonymous,
                                                     parentDataStructure,
                                                     Entity);

                return AddResult.Success(Entity,
                                         EventTrackingId);

            }

            return AddResult.NoOperation(Entity,
                                         EventTrackingId);

        }

        public AddResult TryAddIfNotExists(TEntity                                                                     Entity,
                                           Action<DateTime, EventTracking_Id, User_Id, TParentDataStructure, TEntity>  OnSuccess,
                                           EventTracking_Id                                                            EventTrackingId,
                                           User_Id?                                                                    CurrentUserId)
        {

            var userId = CurrentUserId ?? User_Id.Anonymous;

            if (additionIfNotExists.SendVoting(Timestamp.Now,
                                               EventTrackingId,
                                               userId,
                                               parentDataStructure,
                                               Entity) &&

                lookup.TryAdd(Entity.Id, Entity))

            {

                OnSuccess?.Invoke(Timestamp.Now,
                                  EventTrackingId,
                                  userId,
                                  parentDataStructure,
                                  Entity);

                additionIfNotExists.SendNotification(Timestamp.Now,
                                                     EventTrackingId,
                                                     CurrentUserId ?? User_Id.Anonymous,
                                                     parentDataStructure,
                                                     Entity);

                return AddResult.Success(Entity,
                                         EventTrackingId);

            }

            return AddResult.NoOperation(Entity,
                                         EventTrackingId);

        }

        #endregion


        #region TryUpdate (Id, NewEntity, OldEntity, ...)

        public Boolean TryUpdate(TId               Id,
                                 TEntity           NewEntity,
                                 TEntity           OldEntity,
                                 EventTracking_Id  EventTrackingId,
                                 User_Id?          CurrentUserId)
        {

            if (update.SendVoting(Timestamp.Now,
                                  EventTrackingId,
                                  CurrentUserId ?? User_Id.Anonymous,
                                  parentDataStructure,
                                  NewEntity,
                                  OldEntity) &&

                lookup.TryUpdate(Id, NewEntity, OldEntity))
            {

                update.SendNotification(Timestamp.Now,
                                        EventTrackingId,
                                        CurrentUserId ?? User_Id.Anonymous,
                                        parentDataStructure,
                                        NewEntity,
                                        OldEntity);

                return true;

            }

            return false;

        }

        #endregion


        #region ContainsId(...)

        public Boolean ContainsId(TId Id)

            => lookup.ContainsKey(Id);

        #endregion

        #region Contains  (...)

        public Boolean Contains(TEntity Entity)
        {

            foreach (var entity in lookup)
            {
                if (entity.Equals(Entity))
                    return true;
            }

            return false;

        }

        #endregion

        #region GetById   (Id)

        public TEntity? GetById(TId Id)
        {

            if (lookup.TryGetValue(Id, out var entity))
                return entity;

            return default;

        }

        #endregion

        #region TryGet    (Id, out Entity)

        public Boolean TryGet(TId Id, [NotNullWhen(true)] out TEntity? Entity)
        {

            if (lookup.TryGetValue(Id, out Entity))
                return true;

            Entity = default;
            return false;

        }

        #endregion


        #region TryRemove(Id, out Entity)

        public Boolean TryRemove(TId               Id,
                                 out TEntity?      Entity,
                                 EventTracking_Id  EventTrackingId,
                                 User_Id?          CurrentUserId)
        {

            if (lookup.TryGetValue(Id, out Entity))
            {

                if (removal.SendVoting(Timestamp.Now,
                                       EventTrackingId,
                                       CurrentUserId ?? User_Id.Anonymous,
                                       parentDataStructure,
                                       Entity) &&

                    lookup.TryRemove(Id, out Entity))
                {

                    removal.SendNotification(Timestamp.Now,
                                             EventTrackingId,
                                             CurrentUserId ?? User_Id.Anonymous,
                                             parentDataStructure,
                                             Entity);

                    return true;

                }

                return false;

            }

            return false;

        }

        #endregion

        #region TryRemove(Entity)

        public Boolean TryRemove(TEntity           Entity,
                                 EventTracking_Id  EventTrackingId,
                                 User_Id?          CurrentUserId)
        {

            if (removal.SendVoting(Timestamp.Now,
                                   EventTrackingId,
                                   CurrentUserId ?? User_Id.Anonymous,
                                   parentDataStructure,
                                   Entity) &&

                lookup.TryRemove(Entity.Id, out _))
            {

                removal.SendNotification(Timestamp.Now,
                                         EventTrackingId,
                                         CurrentUserId ?? User_Id.Anonymous,
                                         parentDataStructure,
                                         Entity);

                return true;

            }

            return false;

        }

        #endregion

        #region Clear()

        public void Clear(EventTracking_Id  EventTrackingId,
                          User_Id?          CurrentUserId)
        {
            lookup.Clear();
        }

        #endregion


        #region IEnumerable<T> Members

        public IEnumerator<TEntity> GetEnumerator()

            => lookup.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()

            => lookup.Values.GetEnumerator();

        #endregion


    }

}
