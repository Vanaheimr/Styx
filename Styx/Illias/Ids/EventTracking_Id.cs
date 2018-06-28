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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// The unique identification for tracking events (EventTrackingId).
    /// </summary>
    public class EventTracking_Id : //IId,
                                    IEquatable<EventTracking_Id>,
                                    IComparable<EventTracking_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        protected readonly String  _Id;

        #endregion

        #region Properties

        #region New

        /// <summary>
        /// Generate a new unique event tracking identification.
        /// </summary>
        public static EventTracking_Id New
        {
            get
            {
                return EventTracking_Id.Parse(Guid.NewGuid().ToString());
            }
        }

        #endregion

        #region Length

        /// <summary>
        /// Returns the length of the identification.
        /// </summary>
        public UInt64 Length
        {
            get
            {
                return (UInt64) _Id.Length;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Generate a new unique identification for tracking events (EventTrackingId)
        /// based on the given string.
        /// </summary>
        private EventTracking_Id(String EventTrackingId)
        {
            _Id  = EventTrackingId.Trim();
        }

        #endregion


        #region Parse(EVSEId)

        /// <summary>
        /// Parse the given string as an event tracking identification.
        /// </summary>
        /// <param name="Text">A text representation of an event tracking identification.</param>
        public static EventTracking_Id Parse(String EventTrackingId)
            => new EventTracking_Id(EventTrackingId);

        #endregion

        #region TryParse(Text, out EventTrackingId)

        /// <summary>
        /// Parse the given string as an event tracking identification.
        /// </summary>
        /// <param name="Text">A text representation of an event tracking identification.</param>
        /// <param name="EventTrackingId">The parsed event tracking identification.</param>
        public static Boolean TryParse(String Text, out EventTracking_Id EventTrackingId)
        {
            try
            {
                EventTrackingId = new EventTracking_Id(Text);
                return true;
            }
            catch (Exception)
            {
                EventTrackingId = null;
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone an EventTrackingId.
        /// </summary>
        public EventTracking_Id Clone
        {
            get
            {
                return new EventTracking_Id(_Id);
            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (EventTrackingId1, EventTrackingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventTrackingId1">An event tracking identification.</param>
        /// <param name="EventTrackingId2">Another event tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EventTracking_Id EventTrackingId1, EventTracking_Id EventTrackingId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(EventTrackingId1, EventTrackingId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) EventTrackingId1 == null) || ((Object) EventTrackingId2 == null))
                return false;

            return EventTrackingId1.Equals(EventTrackingId2);

        }

        #endregion

        #region Operator != (EventTrackingId1, EventTrackingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventTrackingId1">An event tracking identification.</param>
        /// <param name="EventTrackingId2">Another event tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EventTracking_Id EventTrackingId1, EventTracking_Id EventTrackingId2)
            => !(EventTrackingId1 == EventTrackingId2);

        #endregion

        #region Operator <  (EventTrackingId1, EventTrackingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventTrackingId1">An event tracking identification.</param>
        /// <param name="EventTrackingId2">Another event tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EventTracking_Id EventTrackingId1, EventTracking_Id EventTrackingId2)
        {

            if ((Object) EventTrackingId1 == null)
                throw new ArgumentNullException("The given EventTrackingId1 must not be null!");

            return EventTrackingId1.CompareTo(EventTrackingId2) < 0;

        }

        #endregion

        #region Operator <= (EventTrackingId1, EventTrackingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventTrackingId1">An event tracking identification.</param>
        /// <param name="EventTrackingId2">Another event tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EventTracking_Id EventTrackingId1, EventTracking_Id EventTrackingId2)
            => !(EventTrackingId1 > EventTrackingId2);

        #endregion

        #region Operator >  (EventTrackingId1, EventTrackingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventTrackingId1">An event tracking identification.</param>
        /// <param name="EventTrackingId2">Another event tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EventTracking_Id EventTrackingId1, EventTracking_Id EventTrackingId2)
        {

            if ((Object) EventTrackingId1 == null)
                throw new ArgumentNullException("The given EventTrackingId1 must not be null!");

            return EventTrackingId1.CompareTo(EventTrackingId2) > 0;

        }

        #endregion

        #region Operator >= (EventTrackingId1, EventTrackingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventTrackingId1">An event tracking identification.</param>
        /// <param name="EventTrackingId2">Another event tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EventTracking_Id EventTrackingId1, EventTracking_Id EventTrackingId2)
            => !(EventTrackingId1 < EventTrackingId2);

        #endregion

        #endregion

        #region IComparable<EventTracking_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var EventTrackingId = Object as EventTracking_Id;
            if ((Object) EventTrackingId == null)
                throw new ArgumentException("The given object is not an event tracking identification!");

            return CompareTo(EventTrackingId);

        }

        #endregion

        #region CompareTo(EventTrackingId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventTrackingId">An object to compare with.</param>
        public Int32 CompareTo(EventTracking_Id EventTrackingId)
        {

            if ((Object) EventTrackingId == null)
                throw new ArgumentNullException(nameof(EventTrackingId), "The given event tracking identification must not be null!");

            return String.Compare(_Id, EventTrackingId._Id, StringComparison.Ordinal);

        }

        #endregion

        #endregion

        #region IEquatable<EventTrackingId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            var EventTrackingId = Object as EventTracking_Id;
            if ((Object) EventTrackingId == null)
                return false;

            return this.Equals(EventTrackingId);

        }

        #endregion

        #region Equals(EventTrackingId)

        /// <summary>
        /// Compares two EventTrackingIds for equality.
        /// </summary>
        /// <param name="EventTrackingId">A EventTrackingId to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EventTracking_Id EventTrackingId)
        {

            if ((Object) EventTrackingId == null)
                return false;

            return _Id.Equals(EventTrackingId._Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => _Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => _Id;

        #endregion

    }

}
