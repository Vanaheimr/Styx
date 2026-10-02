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

using System.Reflection;
using System.Collections.Concurrent;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI
{

    #region (static class) DefaultStrings

    /// <summary>
    /// Default strings.
    /// </summary>
    //public static class DefaultStrings
    //{
    //    public const EnvironmentKey RemoteSystemId = EnvironmentKey.Parse("remoteSystemId");
    //}

    #endregion


    /// <summary>
    /// A Command Line Interface for executing commands.
    /// </summary>
    public interface ICLI
    {

        #region Properties

     //   ConcurrentDictionary<EnvironmentKey, ConcurrentList<String>>  Environment 
     //   Boolean                                                       QuitCLI     



        /// <summary>
        /// All registered commands.
        /// </summary>
        IEnumerable<ICLICommand>                                      Commands          { get; }

        /// <summary>
        /// The command history.
        /// </summary>
        IEnumerable<String>                                           CommandHistory    { get; }

        Boolean                                                       QuitCLI           { get; set; }


        ConcurrentDictionary<EnvironmentKey, ConcurrentList<String>>  Environment       { get; }

        /// <summary>
        /// What this command line is typed at and written on.
        /// </summary>
        ICLITerminal                                                  Terminal          { get; }

        #endregion



        #region RegisterCLIType    (CLIType)

        /// <summary>
        /// Register all commands of the given CLI type and its assembly.
        /// </summary>
        /// <param name="CLIType">A CLI type.</param>
        void RegisterCLIType(Type CLIType);

        #endregion

        #region RegisterAssemblies(AssembliesWithCLICommands)

        /// <summary>
        /// Register all commands from the given assemblies.
        /// </summary>
        /// <param name="AssembliesWithCLICommands">An array of assemblies to search for commands.</param>
        void RegisterAssemblies(params Assembly[] AssembliesWithCLICommands);

        #endregion

        #region RegisterAssemblies(CLIType, AssembliesWithCLICommands)

        /// <summary>
        /// Register all commands from the given assemblies.
        /// </summary>
        /// <param name="AssembliesWithCLICommands">An array of assemblies to search for commands.</param>
        void RegisterAssemblies(Type CLIType, params Assembly[] AssembliesWithCLICommands);

        #endregion



        #region Suggest(Command)

        Task<SuggestionResponse[]> Suggest(String Command);

        #endregion

        #region Suggest(InputArguments)

        Task<SuggestionResponse[]> Suggest(String[] InputArguments);

        #endregion


        #region Execute(Command, CancellationToken = default)

        Task<String[]> Execute(String Command);

        Task<String[]> Execute(String Command, CancellationToken CancellationToken);

        #endregion

        #region Execute(InputArguments, CancellationToken = default)

        Task<String[]> Execute(String[] InputArguments);

        Task<String[]> Execute(String[] InputArguments, CancellationToken CancellationToken);

        #endregion


        #region WriteBlock(Write)

        /// <summary>
        /// Write to the console without breaking the command line somebody is
        /// typing at that moment: the line is taken off the screen, the block is
        /// written as one piece, and the line is put back with the cursor where
        /// it was.
        /// </summary>
        /// <param name="Write">Whatever writes the block. It is called with the console to itself.</param>
        void WriteBlock(Action Write);

        /// <summary>
        /// Write to this command line's terminal without breaking the command
        /// line somebody is typing at that moment.
        /// </summary>
        /// <param name="Write">Whatever writes the block, on the terminal it is handed, which it has to itself.</param>
        void WriteBlock(Action<ICLITerminal> Write);

        #endregion


    }

}
