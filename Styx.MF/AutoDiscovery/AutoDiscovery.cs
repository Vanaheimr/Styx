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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

#endregion

namespace de.ahzf.Illias.Commons
{

    /// <summary>
    /// A factory which uses reflection to generate a apropriate
    /// implementation of T for you.
    /// </summary>
    public class AutoDiscovery<TClass> : IEnumerable<TClass>
        where TClass : class
    {

        #region Data

        private readonly ConcurrentDictionary<String, Type>   _TypeLookup;
        private readonly ConcurrentDictionary<String, TClass> _InstanceLookup;

        #endregion

        #region Properties

        #region SearchingFor

        /// <summary>
        /// Returns the Name of the interface T.
        /// </summary>
        public String SearchingFor
        {
            get
            {
                return typeof(TClass).Name;
            }
        }

        #endregion

        #region RegisteredNames

        /// <summary>
        /// Returns an enumeration of the names of all registered types of T.
        /// </summary>
        public IEnumerable<String> RegisteredNames
        {
            get
            {
                return _TypeLookup.Keys;
            }
        }

        #endregion

        #region RegisteredTypes

        /// <summary>
        /// Returns an enumeration of activated instances of all registered types of T.
        /// </summary>
        public IEnumerable<TClass> RegisteredTypes
        {
            get
            {

                try
                {

                    return from   _StringTypePair
                           in     _TypeLookup
                           select (TClass) Activator.CreateInstance(_StringTypePair.Value);

                }
                catch (Exception e)
                {
                    throw new AutoDiscoveryException("Could not create instance! " + e);
                }

            }
        }

        #endregion

        #region Count

        /// <summary>
        /// Returns the number of registered implementations of the interface T.
        /// </summary>
        public UInt64 Count
        {
            get
            {
                return (UInt64) _TypeLookup.LongCount();
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region AutoDiscovery()

        /// <summary>
        /// Create a new AutoDiscovery instance and start the discovery.
        /// </summary>
        public AutoDiscovery()
            : this(true)
        { }

        #endregion

        #region AutoDiscovery(Autostart, IdentificatorFunc = null)

        /// <summary>
        /// Create a new AutoDiscovery instance. An automatic discovery
        /// can be avoided.
        /// </summary>
        /// <param name="Autostart">Automatically start the reflection process.</param>
        /// <param name="IdentificatorFunc">A transformation delegate to provide an unique identification for every matching class.</param>
        public AutoDiscovery(Boolean Autostart, Func<TClass, String> IdentificatorFunc = null)
        {

            _TypeLookup     = new ConcurrentDictionary<String, Type>();
            _InstanceLookup = new ConcurrentDictionary<String, TClass>();
            
            if (Autostart)
                FindAndRegister(IdentificatorFunc: IdentificatorFunc);

        }

        #endregion

        #endregion


        #region FindAndRegister(ClearTypeDictionary = true, Paths = null, FileExtensions = null, IdentificatorFunc = null)

        /// <summary>
        /// Searches all matching files at the given paths for classes implementing the interface &lt;T&gt;.
        /// </summary>
        /// <param name="ClearTypeDictionary">Clears the TypeDictionary before adding new implementations.</param>
        /// <param name="Paths">An enumeration of paths to search for implementations.</param>
        /// <param name="FileExtensions">A enumeration of file extensions for filtering.</param>
        /// <param name="IdentificatorFunc">A transformation delegate to provide an unique identification for every matching class.</param>
        public void FindAndRegister(Boolean ClearTypeDictionary = true, IEnumerable<String> Paths = null, IEnumerable<String> FileExtensions = null, Func<TClass, String> IdentificatorFunc = null)
        {

            #region Get a list of interesting files

            var _ConcurrentBag = new ConcurrentBag<String>();

            if (Paths == null)
                Paths = new List<String> { "." };

            if (FileExtensions == null)
                FileExtensions = new List<String> { ".dll", ".exe" };

            foreach (var _Path in Paths)
            {

                Parallel.ForEach(Directory.GetFiles(_Path), _ActualFile =>
                {

                    var _FileInfo = new FileInfo(_ActualFile);

                    if (FileExtensions.Contains(_FileInfo.Extension))
                        _ConcurrentBag.Add(_FileInfo.FullName);

                });

            }

            if (ClearTypeDictionary)
                _TypeLookup.Clear();

            #endregion

            #region Scan files of implementations of T

            Parallel.ForEach(_ConcurrentBag, _File =>
            {

                // Seems to be a mono bug!
                if (_File != null)
                {

                    Console.WriteLine(_File);

                    try
                    {

                        if (_File != null)
                            foreach (var _ActualType in Assembly.LoadFrom(_File).GetTypes())
                            {

                                if (!_ActualType.IsAbstract &&
                                     _ActualType.IsPublic   &&
                                     _ActualType.IsVisible)
                                {

                                    var _ActualTypeGetInterfaces = _ActualType.GetInterfaces();

                                    if (_ActualTypeGetInterfaces != null)
                                    {

                                        foreach (var _Interface in _ActualTypeGetInterfaces)
                                        {

                                            if (_Interface == typeof(TClass))
                                            {

                                                try
                                                {

                                                    var __Id = _ActualType.Name;

                                                    if (IdentificatorFunc != null)
                                                    {
                                                        var _T = Activator.CreateInstance(_ActualType) as TClass;
                                                        if (_T != null)
                                                            __Id = IdentificatorFunc(_T);
                                                    }

                                                    if (__Id != null && __Id != String.Empty)
                                                        _TypeLookup.TryAdd(__Id, _ActualType);

                                                }

                                                catch (Exception e)
                                                {
                                                    throw new AutoDiscoveryException("Could not activate or register " + typeof(TClass).Name + "-instance '" + _ActualType.Name + "'!", e);
                                                }

                                            }

                                        }

                                    }

                                }

                            }

                    }

                    catch (Exception e)
                    {
                        throw new AutoDiscoveryException("Autodiscovering implementations of interface '" + typeof(TClass).Name + "' within file '" + _File + "' failed!", e);
                    }

                }

            });

            #endregion

        }

        #endregion

        #region TryGetInstance(Identificator, out Instance)

        /// <summary>
        /// Attempts to get an instance associated with the identificator.
        /// </summary>
        public Boolean TryGetInstance(String Identificator, out TClass Instance)
        {
            
            if (_InstanceLookup.TryGetValue(Identificator, out Instance))
                return true;

            Type _Type = null;
            if (_TypeLookup.TryGetValue(Identificator, out _Type))
            {

                try
                {

                    Instance = (TClass) Activator.CreateInstance(_Type);

                    // If it fails because of concurrency it does not matter!
                    _InstanceLookup.TryAdd(Identificator, Instance);

                    return true;

                }
                catch (Exception e)
                {
                    throw new AutoDiscoveryException("An instance of " + typeof(TClass).Name + " with identificator '" + Identificator + "' could not be activated!", e);
                }

            }

            return false;

        }

        #endregion


        #region GetEnumerator()

        public IEnumerator<TClass> GetEnumerator()
        {

            TClass Instance;

            foreach (var Identificator in _TypeLookup.Keys)
                if (TryGetInstance(Identificator, out Instance))
                    yield return Instance;

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {

            TClass Instance;

            foreach (var Identificator in _TypeLookup.Keys)
                if (TryGetInstance(Identificator, out Instance))
                    yield return Instance;

        }

        #endregion

    }

}
