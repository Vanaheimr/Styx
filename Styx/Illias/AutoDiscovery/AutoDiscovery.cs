/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A factory which uses reflection to generate a appropriate
    /// implementation of T for you.
    /// </summary>
    public class AutoDiscovery<TClass> : IEnumerable<TClass>
        where TClass : class
    {

        #region Data

        private readonly ConcurrentDictionary<String, Type>   typeLookup;
        private readonly ConcurrentDictionary<String, TClass> instanceLookup;

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
                return typeLookup.Keys;
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
                           in     typeLookup
                           select (TClass) Activator.CreateInstance(_StringTypePair.Value)!;

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
            => (UInt64) typeLookup.Count;

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

        #region AutoDiscovery(AutoStart, IdentificatorFunc = null)

        /// <summary>
        /// Create a new AutoDiscovery instance. An automatic discovery
        /// can be avoided.
        /// </summary>
        /// <param name="AutoStart">Automatically start the reflection process.</param>
        /// <param name="IdentificatorFunc">A transformation delegate to provide an unique identification for every matching class.</param>
        public AutoDiscovery(Boolean AutoStart, Func<TClass, String>? IdentificatorFunc = null)
        {

            typeLookup     = new ConcurrentDictionary<String, Type>();
            instanceLookup = new ConcurrentDictionary<String, TClass>();

            if (AutoStart)
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
        public void FindAndRegister(Boolean ClearTypeDictionary = true, IEnumerable<String>? Paths = null, IEnumerable<String>? FileExtensions = null, Func<TClass, String>? IdentificatorFunc = null)
        {

            #region Get a list of interesting files

            var concurrentBag = new ConcurrentBag<String>();

            Paths          ??= [ "." ];
            FileExtensions ??= [ ".dll", ".exe" ];

            foreach (var path in Paths)
            {

                Parallel.ForEach(Directory.GetFiles(path), actualFile => {

                    var fileInfo = new FileInfo(actualFile);

                    if (FileExtensions.Contains(fileInfo.Extension))
                        concurrentBag.Add(fileInfo.FullName);

                });

            }

            if (ClearTypeDictionary)
                typeLookup.Clear();

            #endregion

            #region Scan files of implementations of T

            Parallel.ForEach(concurrentBag, actualFile => {

                // Seems to be a mono bug!
                if (actualFile is not null)
                {

                   // Console.WriteLine(_File);

                    try
                    {

                        if (actualFile is not null)
                            foreach (var actualType in Assembly.LoadFrom(actualFile).GetTypes())
                            {

                                if (!actualType.IsAbstract &&
                                     actualType.IsPublic   &&
                                     actualType.IsVisible)
                                {

                                    var actualTypeGetInterfaces = actualType.GetInterfaces();

                                    if (actualTypeGetInterfaces is not null)
                                    {

                                        foreach (var actualInterface in actualTypeGetInterfaces)
                                        {

                                            // The following check is not valid for Bifrost HTTPService interfaces and must be fixed!

                                            if (actualInterface == typeof(TClass))
                                            {

                                                try
                                                {

                                                    var __Id = actualType.Name;

                                                    if (IdentificatorFunc is not null)
                                                    {
                                                        var _T = Activator.CreateInstance(actualType) as TClass;
                                                        if (_T is not null)
                                                            __Id = IdentificatorFunc(_T);
                                                    }

                                                    if (__Id is not null && __Id != String.Empty)
                                                        typeLookup.TryAdd(__Id, actualType);

                                                }

                                                catch (Exception e)
                                                {
                                                    throw new AutoDiscoveryException("Could not activate or register " + typeof(TClass).Name + "-instance '" + actualType.Name + "'!", e);
                                                }

                                            }

                                        }

                                    }

                                }

                            }

                    }

                    catch (BadImageFormatException)
                    { }

                    catch (Exception e)
                    {
                        throw new AutoDiscoveryException("Autodiscovering implementations of interface '" + typeof(TClass).Name + "' within file '" + actualFile + "' failed!", e);
                    }

                }

            });

            #endregion

        }

        #endregion

        #region TryGetInstance(Identificator, out Instance)

        /// <summary>
        /// Attempts to get an instance associated with the identifier.
        /// </summary>
        public Boolean TryGetInstance(String Identificator, out TClass Instance)
        {

            Instance = default;

            if (instanceLookup.TryGetValue(Identificator, out Instance))
                return true;

            if (typeLookup.TryGetValue(Identificator, out Type? _Type))
            {

                try
                {

                    Instance = (TClass) Activator.CreateInstance(_Type)!;

                    // If it fails because of concurrency it does not matter!
                    instanceLookup.TryAdd(Identificator, Instance);

                    return true;

                }
                catch (Exception e)
                {
                    throw new AutoDiscoveryException($"An instance of {typeof(TClass).Name} with identifier '{Identificator}' could not be activated!", e);
                }

            }

            return false;

        }

        #endregion


        #region GetEnumerator()

        public IEnumerator<TClass> GetEnumerator()
        {

            TClass Instance;

            foreach (var Identificator in typeLookup.Keys)
                if (TryGetInstance(Identificator, out Instance))
                    yield return Instance;

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {

            foreach (var Identificator in typeLookup.Keys)
                if (TryGetInstance(Identificator, out TClass Instance))
                    yield return Instance;

        }

        #endregion

    }

}
