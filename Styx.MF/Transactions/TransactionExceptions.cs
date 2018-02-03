/*
 * Copyright (c) 2010-2012 Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Illias Commons <http://www.github.com/ahzf/Illias>
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

namespace de.ahzf.Illias.Commons.Transactions
{

    #region TransactionException<TTransactionId, TSystemId>

    /// <summary>
    /// An exception during transaction processing occurred!
    /// </summary>
    /// <typeparam name="TTransactionId">The type of the transaction Id.</typeparam>
    /// <typeparam name="TSystemId">The type of the system Id.</typeparam>
    public class TransactionException<TTransactionId, TSystemId> : Exception

        where TTransactionId : IEquatable<TTransactionId>, IComparable<TTransactionId>, IComparable
        where TSystemId      : IEquatable<TSystemId>,      IComparable<TSystemId>,      IComparable

    {

        /// <summary>
        /// The transaction causing this exception.
        /// </summary>
        protected Transaction<TTransactionId, TSystemId> _Transaction = null;

        /// <summary>
        /// An exception during transaction processing occurred!
        /// </summary>
        /// <param name="Transaction">A transaction.</param>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public TransactionException(Transaction<TTransactionId, TSystemId> Transaction, String Message = null, Exception InnerException = null)
            : base(Message, InnerException)
        {
            _Transaction = Transaction;
        }

    }

    #endregion


    #region CouldNotBeginTransactionException<T>

    /// <summary>
    /// A transaction could not be started.
    /// </summary>
    /// <typeparam name="TTransactionId">The type of the transaction Id.</typeparam>
    /// <typeparam name="TSystemId">The type of the system Id.</typeparam>
    public class CouldNotBeginTransactionException<TTransactionId, TSystemId>
                     : TransactionException<TTransactionId, TSystemId>

        where TTransactionId : IEquatable<TTransactionId>, IComparable<TTransactionId>, IComparable
        where TSystemId      : IEquatable<TSystemId>,      IComparable<TSystemId>,      IComparable

    {

        /// <summary>
        /// A transaction could not be started.
        /// </summary>
        /// <param name="Transaction">A transaction.</param>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public CouldNotBeginTransactionException(Transaction<TTransactionId, TSystemId> Transaction, String Message = null, Exception InnerException = null)
            : base(Transaction, Message, InnerException)
        { }

    }

    #endregion

    #region CouldNotCommitNestedTransactionException<T>

    /// <summary>
    /// A nested transaction could not be committed.
    /// </summary>
    /// <typeparam name="TTransactionId">The type of the transaction Id.</typeparam>
    /// <typeparam name="TSystemId">The type of the system Id.</typeparam>
    public class CouldNotCommitNestedTransactionException<TTransactionId, TSystemId>
                     : TransactionException<TTransactionId, TSystemId>

        where TTransactionId : IEquatable<TTransactionId>, IComparable<TTransactionId>, IComparable
        where TSystemId      : IEquatable<TSystemId>,      IComparable<TSystemId>,      IComparable

    {

        /// <summary>
        /// A nested transaction could not be committed.
        /// </summary>
        /// <param name="Transaction">A transaction.</param>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public CouldNotCommitNestedTransactionException(Transaction<TTransactionId, TSystemId> Transaction, String Message = null, Exception InnerException = null)
            : base(Transaction, Message, InnerException)
        { }

    }

    #endregion

    #region CouldNotRolleBackNestedTransactionException<T>

    /// <summary>
    /// A nested transaction could not be rolled back.
    /// </summary>
    /// <typeparam name="TTransactionId">The type of the transaction Id.</typeparam>
    /// <typeparam name="TSystemId">The type of the system Id.</typeparam>
    public class CouldNotRolleBackNestedTransactionException<TTransactionId, TSystemId>
                     : TransactionException<TTransactionId, TSystemId>
        
        where TTransactionId : IEquatable<TTransactionId>, IComparable<TTransactionId>, IComparable
        where TSystemId      : IEquatable<TSystemId>,      IComparable<TSystemId>,      IComparable

    {

        /// <summary>
        /// A nested transaction could not be rolled back.
        /// </summary>
        /// <param name="Transaction">A transaction.</param>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public CouldNotRolleBackNestedTransactionException(Transaction<TTransactionId, TSystemId> Transaction, String Message = null, Exception InnerException = null)
            : base(Transaction, Message, InnerException)
        { }

    }

    #endregion

    #region TransactionAlreadyCommitedException<T>

    /// <summary>
    /// The transaction was already committed.
    /// </summary>
    /// <typeparam name="TTransactionId">The type of the transaction Id.</typeparam>
    /// <typeparam name="TSystemId">The type of the system Id.</typeparam>
    public class TransactionAlreadyCommitedException<TTransactionId, TSystemId>
                     : TransactionException<TTransactionId, TSystemId>
        
        where TTransactionId : IEquatable<TTransactionId>, IComparable<TTransactionId>, IComparable
        where TSystemId      : IEquatable<TSystemId>,      IComparable<TSystemId>,      IComparable

    {

        /// <summary>
        /// The transaction was already committed.
        /// </summary>
        /// <param name="Transaction">A transaction.</param>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public TransactionAlreadyCommitedException(Transaction<TTransactionId, TSystemId> Transaction, String Message = null, Exception InnerException = null)
            : base(Transaction, Message, InnerException)
        { }

    }

    #endregion

    #region TransactionAlreadyRolledbackException<T>

    /// <summary>
    /// The transaction was already rolled back!
    /// </summary>
    /// <typeparam name="TTransactionId">The type of the transaction Id.</typeparam>
    /// <typeparam name="TSystemId">The type of the system Id.</typeparam>
    public class TransactionAlreadyRolledbackException<TTransactionId, TSystemId>
                     : TransactionException<TTransactionId, TSystemId>
        
        where TTransactionId : IEquatable<TTransactionId>, IComparable<TTransactionId>, IComparable
        where TSystemId      : IEquatable<TSystemId>,      IComparable<TSystemId>,      IComparable

    {

        /// <summary>
        /// The transaction was already rolled back!
        /// </summary>
        /// <param name="Transaction">A transaction.</param>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public TransactionAlreadyRolledbackException(Transaction<TTransactionId, TSystemId> Transaction, String Message = null, Exception InnerException = null)
            : base(Transaction, Message, InnerException)
        { }

    }

    #endregion

    #region TransactionAlreadyRunningException<T>

    /// <summary>
    /// The transaction is already running!
    /// </summary>
    /// <typeparam name="TTransactionId">The type of the transaction Id.</typeparam>
    /// <typeparam name="TSystemId">The type of the system Id.</typeparam>
    public class TransactionAlreadyRunningException<TTransactionId, TSystemId>
                     : TransactionException<TTransactionId, TSystemId>
        
        where TTransactionId : IEquatable<TTransactionId>, IComparable<TTransactionId>, IComparable
        where TSystemId      : IEquatable<TSystemId>,      IComparable<TSystemId>,      IComparable

    {

        /// <summary>
        /// The transaction is already running!
        /// </summary>
        /// <param name="Transaction">A transaction.</param>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public TransactionAlreadyRunningException(Transaction<TTransactionId, TSystemId> Transaction, String Message = null, Exception InnerException = null)
            : base(Transaction, Message, InnerException)
        { }

    }

    #endregion

    #region TransactionCurrentlyCommittingException<T>

    /// <summary>
    /// The transaction is currently committing!
    /// </summary>
    /// <typeparam name="TTransactionId">The type of the transaction Id.</typeparam>
    /// <typeparam name="TSystemId">The type of the system Id.</typeparam>
    public class TransactionCurrentlyCommittingException<TTransactionId, TSystemId>
                     : TransactionException<TTransactionId, TSystemId>
        
        where TTransactionId : IEquatable<TTransactionId>, IComparable<TTransactionId>, IComparable
        where TSystemId      : IEquatable<TSystemId>,      IComparable<TSystemId>,      IComparable

    {

        /// <summary>
        /// The transaction is currently committing!
        /// </summary>
        /// <param name="Transaction">A transaction.</param>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public TransactionCurrentlyCommittingException(Transaction<TTransactionId, TSystemId> Transaction, String Message = null, Exception InnerException = null)
            : base(Transaction, Message, InnerException)
        { }

    }

    #endregion

    #region TransactionCurrentlyRollingBackException<T>

    /// <summary>
    /// The transaction is currently rolling back!
    /// </summary>
    /// <typeparam name="TTransactionId">The type of the transaction Id.</typeparam>
    /// <typeparam name="TSystemId">The type of the system Id.</typeparam>
    public class TransactionCurrentlyRollingBackException<TTransactionId, TSystemId>
                     : TransactionException<TTransactionId, TSystemId>
        
        where TTransactionId : IEquatable<TTransactionId>, IComparable<TTransactionId>, IComparable
        where TSystemId      : IEquatable<TSystemId>,      IComparable<TSystemId>,      IComparable

    {

        /// <summary>
        /// The transaction is currently rolling back!
        /// </summary>
        /// <param name="Transaction">A transaction.</param>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public TransactionCurrentlyRollingBackException(Transaction<TTransactionId, TSystemId> Transaction, String Message = null, Exception InnerException = null)
            : base(Transaction, Message, InnerException)
        { }

    }

    #endregion

}
