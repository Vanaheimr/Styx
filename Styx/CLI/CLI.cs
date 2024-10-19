﻿/*
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

using System.Net;
using System.Reflection;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI
{

    public enum SuggestionInfo
    {
        None,
        Complete,
        Prefix,
        Error
    }

    public class SuggestionResponse : IComparable<SuggestionResponse>,
                                      IEquatable<SuggestionResponse>
    {

        #region Properties

        public String          Suggestion    { get; set; } = "";
        public SuggestionInfo  Info          { get; set; }

        #endregion


        private SuggestionResponse(String          Suggestion,
                                   SuggestionInfo  Info)
        {
            this.Suggestion  = Suggestion;
            this.Info        = Info;
        }



        public static SuggestionResponse Complete(String Suggestion)

            => new (Suggestion,
                    SuggestionInfo.Complete);

        public static SuggestionResponse Prefix(String Suggestion)

            => new (Suggestion,
                    SuggestionInfo.Prefix);



        #region Operator overloading

        #region Operator == (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SuggestionResponse SuggestionResponse1,
                                           SuggestionResponse SuggestionResponse2)

            => SuggestionResponse1.Equals(SuggestionResponse2);

        #endregion

        #region Operator != (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SuggestionResponse SuggestionResponse1,
                                           SuggestionResponse SuggestionResponse2)

            => !SuggestionResponse1.Equals(SuggestionResponse2);

        #endregion

        #region Operator <  (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SuggestionResponse SuggestionResponse1,
                                          SuggestionResponse SuggestionResponse2)

            => SuggestionResponse1.CompareTo(SuggestionResponse2) < 0;

        #endregion

        #region Operator <= (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SuggestionResponse SuggestionResponse1,
                                           SuggestionResponse SuggestionResponse2)

            => SuggestionResponse1.CompareTo(SuggestionResponse2) <= 0;

        #endregion

        #region Operator >  (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SuggestionResponse SuggestionResponse1,
                                          SuggestionResponse SuggestionResponse2)

            => SuggestionResponse1.CompareTo(SuggestionResponse2) > 0;

        #endregion

        #region Operator >= (SuggestionResponse1, SuggestionResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SuggestionResponse1">A SuggestionResponse.</param>
        /// <param name="SuggestionResponse2">Another SuggestionResponse.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SuggestionResponse SuggestionResponse1,
                                           SuggestionResponse SuggestionResponse2)

            => SuggestionResponse1.CompareTo(SuggestionResponse2) >= 0;

        #endregion

        #endregion

        #region IComparable<SuggestionResponse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two SuggestionResponses.
        /// </summary>
        /// <param name="Object">A SuggestionResponse to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SuggestionResponse ipSocket
                   ? CompareTo(ipSocket)
                   : throw new ArgumentException("The given object is not a SuggestionResponse!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SuggestionResponse)

        /// <summary>
        /// Compares two SuggestionResponses.
        /// </summary>
        /// <param name="SuggestionResponse">A SuggestionResponse to compare with.</param>
        public Int32 CompareTo(SuggestionResponse? SuggestionResponse)
        {
            return Suggestion.CompareTo(SuggestionResponse?.Suggestion ?? "");
        }

        #endregion

        #endregion

        #region IEquatable<SuggestionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SuggestionResponses for equality.
        /// </summary>
        /// <param name="Object">A SuggestionResponse to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SuggestionResponse component &&
                   Equals(component);

        #endregion

        #region Equals(SuggestionResponse)

        /// <summary>
        /// Compares two SuggestionResponses for equality.
        /// </summary>
        /// <param name="SuggestionResponse">A SuggestionResponse to compare with.</param>
        public Boolean Equals(SuggestionResponse? SuggestionResponse)

            => SuggestionResponse is not null &&
               String.Equals(Suggestion, SuggestionResponse.Suggestion, StringComparison.OrdinalIgnoreCase) &&
               Info.  Equals(SuggestionResponse.Info);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Suggestion.GetHashCode() ^
                       Info.      GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Returns a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Suggestion} [{Info}]";

        #endregion


    }


    /// <summary>
    /// A Command Line Interface for executing commands.
    /// </summary>
    public class CLI
    {

        #region (static class) DefaultStrings

        /// <summary>
        /// Default strings.
        /// </summary>
        public static class DefaultStrings
        {
            public const String RemoteSystemId = "remoteSystemId";
        }

        #endregion




        #region Data

        private readonly  List<ICLICommands>          commands         = [];
        private readonly  List<String>                commandHistory   = [];
        private readonly  CancellationTokenSource     cts              = new();

        public  readonly  Dictionary<String, String>  Environment      = [];
        public            Boolean                     QuitCLI          = false;

        #endregion

        #region Properties

        /// <summary>
        /// All registered commands.
        /// </summary>
        public IEnumerable<ICLICommands> Commands
            => commands;

        /// <summary>
        /// The command history.
        /// </summary>
        public IEnumerable<String> CommandHistory
            => commandHistory;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new command line interface.
        /// </summary>
        /// <param name="AssembliesWithCLICommands">The assemblies to search for commands.</param>
        public CLI(params Assembly[] AssembliesWithCLICommands)
        {

            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                cts.Cancel();
            };

            RegisterAssemblies([ typeof(CLI).Assembly, .. AssembliesWithCLICommands ]);

        }

        #endregion


        #region RegisterAssemblies(AssembliesWithCLICommands)

        /// <summary>
        /// Register all commands from the given assemblies.
        /// </summary>
        /// <param name="AssembliesWithCLICommands">An array of assemblies to search for commands.</param>
        public void RegisterAssemblies(params Assembly[] AssembliesWithCLICommands)
        {

            foreach (var assemblyWithCLICommands in AssembliesWithCLICommands)
            {

                commands.AddRange(
                    assemblyWithCLICommands.
                        GetTypes().
                        Where(type => typeof(ICLICommands).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor(Type.EmptyTypes) is not null).
                        Select(type => (ICLICommands)Activator.CreateInstance(type)!)
                );

                commands.AddRange(
                    assemblyWithCLICommands.
                        GetTypes().
                        Where(type => typeof(ICLICommands).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor([typeof(CLI)]) is not null).
                        Select(type => (ICLICommands)Activator.CreateInstance(type, this)!)
                );

            }

        }

        #endregion


        public async Task Run()
        {
            do
            {

                Prompt();

                var inputArgs = ReadLineWithAutoCompletion(commands, out var cancelled);

                if (inputArgs?.Length > 0 && !cancelled)
                {

                    var responseLines = await Execute(inputArgs);

                    foreach (var responseLine in responseLines)
                        Console.WriteLine(responseLine);

                }

            }
            while (!QuitCLI);
        }


        private String GetPrompt()
        {

            if (Environment.TryGetValue(DefaultStrings.RemoteSystemId, out var remoteSystemId))
                return $"[{remoteSystemId}] Enter command: ";

            return "Enter command: ";

        }


        private void Prompt(String? Text = "")
        {
            Console.Write(GetPrompt() + Text);
        }

        private void CommandPrompt(List<Char> Text)
        {
            Prompt(new String(Text.ToArray()));
        }



        #region Suggest(Command)

        public SuggestionResponse[] Suggest(String Command)

            => Suggest(Command.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        #endregion

        #region Suggest(InputArguments)

        public SuggestionResponse[] Suggest(String[] InputArguments)

            => InputArguments.Length == 0

                     // An empty input suggest all commands...
                   ? [.. commands.SelectMany(c => c.Suggest([""])).Distinct().Order()]

                   : [.. commands.SelectMany(c => c.Suggest(InputArguments)).Distinct().Order()];

        #endregion


        #region Execute(Command)

        public Task<String[]> Execute(String Command)

            => Execute(Command.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        #endregion

        #region Execute(InputArguments)

        public async Task<String[]> Execute(String[] InputArguments)
        {

            try
            {

                var matchingCommands = commands.Where(c => String.Equals(c.Suggest([InputArguments[0]]).FirstOrDefault()?.Suggestion ?? "", InputArguments[0], StringComparison.OrdinalIgnoreCase)).ToArray();
                if (matchingCommands.Length == 1)
                {

                    if (InputArguments.Length > 0 &&
                        !String.Equals(HistoryCommand.CommandName, InputArguments[0], StringComparison.OrdinalIgnoreCase))
                    {

                        var command = String.Join(" ", InputArguments);

                        if (commandHistory.LastOrDefault() != command)
                            commandHistory.Add(command);

                    }

                    return await matchingCommands.First().Execute(InputArguments, cts.Token);

                }
                else
                {
                     return [$"Unknown command: {InputArguments[0]}"];
                }

            }
            catch (OperationCanceledException)
            {
                return ["Command execution cancelled"];
            }

        }

        #endregion


        #region (private static) ClearCurrentConsoleLine()

        private static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(new String(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        #endregion

        #region (private) ReadLineWithAutoCompletion(Commands, out Cancelled)

        private String[] ReadLineWithAutoCompletion(List<ICLICommands> Commands, out Boolean Cancelled)
        {

            var input           = new List<Char>();
            var cursorPosition  = 0;
            var historyIndex    = -1;
            var currentInput    = String.Empty;
            Cancelled           = false;

            while (true)
            {

                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Tab)
                {

                    var suggestions = Suggest(new String(input.ToArray()).Split(' ', StringSplitOptions.RemoveEmptyEntries));

                    if (suggestions.Length == 0)
                    {
                    }
                    if (suggestions.Length == 1)
                    {

                        input.Clear();

                        input.AddRange(suggestions[0].Suggestion ?? "");

                        if (suggestions[0].Info == SuggestionInfo.Complete)
                            input.Add(' ');

                        cursorPosition = input.Count;
                        ClearCurrentConsoleLine();
                        CommandPrompt(input);

                    }
                    else if (suggestions.Length > 1)
                    {

                        var commonPrefix = new String(suggestions.First().Suggestion[..suggestions.Min(s => s.Suggestion.Length)].
                                                                    TakeWhile((c, i) => suggestions.All(s => s?.Suggestion.Length > i && s.Suggestion[i] == c)).ToArray());

                        input.Clear();
                        input.AddRange(commonPrefix);
                        cursorPosition = input.Count;

                        ClearCurrentConsoleLine();
                        CommandPrompt(input);
                        Console.WriteLine();
                        Console.WriteLine("Suggestions: " + String.Join(", ", suggestions.Select(s => s.Suggestion)));
                        Console.WriteLine();
                        CommandPrompt(input);

                    }
                }

                else if (key.Key == ConsoleKey.Home)
                {
                    cursorPosition = 0;
                    Console.SetCursorPosition(GetPrompt().Length + cursorPosition, Console.CursorTop);
                }

                else if (key.Key == ConsoleKey.End)
                {
                    cursorPosition = input.Count;
                    Console.SetCursorPosition(GetPrompt().Length + cursorPosition, Console.CursorTop);
                }

                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return new String(input.ToArray()).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                }

                else if (key.Key == ConsoleKey.Backspace && cursorPosition > 0)
                {
                    input.RemoveAt(cursorPosition - 1);
                    cursorPosition--;
                    ClearCurrentConsoleLine();
                    CommandPrompt(input);
                    Console.SetCursorPosition(GetPrompt().Length + cursorPosition, Console.CursorTop);
                }

                else if (key.Key == ConsoleKey.Delete && cursorPosition < input.Count)
                {
                    input.RemoveAt(cursorPosition);
                    ClearCurrentConsoleLine();
                    CommandPrompt(input);
                    Console.SetCursorPosition(GetPrompt().Length + cursorPosition, Console.CursorTop);
                }

                else if (key.Key == ConsoleKey.LeftArrow && cursorPosition > 0)
                {
                    cursorPosition--;
                    Console.SetCursorPosition(GetPrompt().Length + cursorPosition, Console.CursorTop);
                }

                else if (key.Key == ConsoleKey.RightArrow && cursorPosition < input.Count)
                {
                    cursorPosition++;
                    Console.SetCursorPosition(GetPrompt().Length + cursorPosition, Console.CursorTop);
                }

                else if (key.Key == ConsoleKey.UpArrow)
                {

                    if (historyIndex == -1 && commandHistory.Count > 0)
                    {
                        currentInput = new String(input.ToArray());
                        historyIndex = commandHistory.Count - 1;
                    }
                    else if (historyIndex > 0)
                    {
                        historyIndex--;
                    }

                    if (historyIndex >= 0)
                    {
                        input.Clear();
                        input.AddRange(commandHistory[historyIndex]);
                        cursorPosition = input.Count;
                        ClearCurrentConsoleLine();
                        CommandPrompt(input);
                    }

                }

                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (historyIndex != -1)
                    {

                        historyIndex++;

                        if (historyIndex >= commandHistory.Count)
                        {
                            historyIndex = -1;
                            input.Clear();
                            input.AddRange(currentInput);
                        }
                        else
                        {
                            input.Clear();
                            input.AddRange(commandHistory[historyIndex]);
                        }

                        cursorPosition = input.Count;
                        ClearCurrentConsoleLine();
                        CommandPrompt(input);

                    }
                }

                else if (key.Key == ConsoleKey.Escape)
                {
                    Cancelled = true;
                    Console.WriteLine();
                    return [];
                }

                else if (!char.IsControl(key.KeyChar))
                {
                    input.Insert(cursorPosition, key.KeyChar);
                    cursorPosition++;
                    ClearCurrentConsoleLine();
                    CommandPrompt(input);
                    Console.SetCursorPosition(GetPrompt().Length + cursorPosition, Console.CursorTop);
                }

            }
        }

        #endregion


    }

}
