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

using System.Reflection;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI
{

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

        private readonly  List<ICLICommand>          commands         = [];
        private readonly  List<String>                commandHistory   = [];
        private readonly  CancellationTokenSource     cts              = new();

        public  readonly  Dictionary<String, String>  Environment      = [];
        public            Boolean                     QuitCLI          = false;

        #endregion

        #region Properties

        /// <summary>
        /// All registered commands.
        /// </summary>
        public IEnumerable<ICLICommand> Commands
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
                        Where(type => typeof(ICLICommand).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor(Type.EmptyTypes) is not null).
                        Select(type => (ICLICommand)Activator.CreateInstance(type)!)
                );

                commands.AddRange(
                    assemblyWithCLICommands.
                        GetTypes().
                        Where(type => typeof(ICLICommand).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor([typeof(CLI)]) is not null).
                        Select(type => (ICLICommand)Activator.CreateInstance(type, this)!)
                );

            }

        }

        #endregion


        public async Task Run()
        {
            do
            {

                Prompt();

                var inputArgs = await ReadLineWithAutoCompletion(commands);

                if (inputArgs.Item1?.Length > 0 && !inputArgs.Item2)
                {

                    var responseLines = await Execute(inputArgs.Item1);

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

        public Task<SuggestionResponse[]> Suggest(String Command)

            => Suggest(Command.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        #endregion

        #region Suggest(InputArguments)

        public async Task<SuggestionResponse[]> Suggest(String[] InputArguments)
        {
            try
            {

                return InputArguments.Length == 0

                     // An empty input suggest all commands...
                   ? [.. commands.SelectMany(c => c.Suggest([""])).Distinct().Order()]

                   : [.. commands.SelectMany(c => c.Suggest(InputArguments)).Distinct().Order()];

            }
            catch
            {
                return [];
            }
        }

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
            catch (Exception e)
            {
                return [ e.Message ];
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

        #region (private) ReadLineWithAutoCompletion(Commands)

        private async Task<Tuple<String[], Boolean>> ReadLineWithAutoCompletion(List<ICLICommand> Commands)
        {

            var input           = new List<Char>();
            var cursorPosition  = 0;
            var historyIndex    = -1;
            var currentInput    = String.Empty;

            while (true)
            {

                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Tab)
                {

                    var suggestions = await Suggest(new String(input.ToArray()).Split(' ', StringSplitOptions.RemoveEmptyEntries));

                    if (suggestions.Length == 0)
                    {
                    }
                    if (suggestions.Length == 1)
                    {

                        input.Clear();

                        input.AddRange(suggestions[0].Suggestion ?? "");

                        if (suggestions[0].Info == SuggestionInfo.CommandCompleted ||
                            suggestions[0].Info == SuggestionInfo.ParameterCompleted)
                        {
                            input.Add(' ');
                        }

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
                    return new Tuple<String[], Boolean>(new String(input.ToArray()).Split(' ', StringSplitOptions.RemoveEmptyEntries), false);
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
                    Console.WriteLine();
                    return new Tuple<String[], Boolean>([], true);
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
