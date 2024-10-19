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

using org.GraphDefined.Vanaheimr.Illias;
using System.Reflection;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI
{

    public class CLI
    {

        private readonly  List<ICLICommands>          commands         = [];
        private readonly  List<String>                commandHistory   = [];
        private readonly  CancellationTokenSource     cts              = new();

        public  readonly  Dictionary<String, String>  Environment      = [];

        public            String                      RemoteSystemId   = "";
        public            Boolean                     QuitCLI          = false;


        public IEnumerable<ICLICommands> Commands
            => commands;

        public IEnumerable<String> CommandHistory
            => commandHistory;


        public CLI()
        {

            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                cts.Cancel();
            };

            RegisterAssemblies(typeof(CLI).Assembly);

        }


        public void RegisterAssemblies(params Assembly[] Assemblies)
        {

            foreach (var assembly in Assemblies)
            {

                commands.AddRange(
                    assembly.
                        GetTypes().
                        Where(type => typeof(ICLICommands).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor(Type.EmptyTypes) is not null).
                        Select(type => (ICLICommands)Activator.CreateInstance(type)!)
                );

                commands.AddRange(
                    assembly.
                        GetTypes().
                        Where(type => typeof(ICLICommands).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor([typeof(CLI)]) is not null).
                        Select(type => (ICLICommands)Activator.CreateInstance(type, this)!)
                );

            }

        }


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

            if (RemoteSystemId.Length == 0)
                return "Enter command: ";


            return $"[{RemoteSystemId}] Enter command: ";

        }


        private void Prompt(String? Text = "")
        {
            Console.Write(GetPrompt() + Text);
        }

        private void CommandPrompt(List<Char> Text)
        {
            Prompt(new String(Text.ToArray()));
        }




        public String[] Suggest(String Command)

            => Suggest(Command.Split(' ', StringSplitOptions.RemoveEmptyEntries));


        public String[] Suggest(String[] Args)

            => Args.Length == 0

                     // An empty input suggest all commands...
                   ? [.. commands.SelectMany(c => c.Suggest([""])).Distinct().Order()]

                   : [.. commands.SelectMany(c => c.Suggest(Args)).Distinct().Order()];



        public Task<String[]> Execute(String Command)

            => Execute(Command.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        public async Task<String[]> Execute(String[] inputArgs)
        {

            try
            {

                var matchingCommands = commands.Where(c => String.Equals(c.Suggest([inputArgs[0]]).FirstOrDefault() ?? "", inputArgs[0], StringComparison.OrdinalIgnoreCase)).ToArray();
                if (matchingCommands.Length == 1)
                {

                    if (inputArgs.Length > 0 &&
                        !String.Equals(HistoryCommand.CommandName, inputArgs[0], StringComparison.OrdinalIgnoreCase))
                    {

                        var command = String.Join(" ", inputArgs);

                        if (commandHistory.LastOrDefault() != command)
                            commandHistory.Add(command);

                    }

                    return await matchingCommands.First().Execute(inputArgs, cts.Token);

                }
                else
                {
                     return [$"Unknown command: {inputArgs[0]}"];
                }

            }
            catch (OperationCanceledException)
            {
                return ["Command execution cancelled"];
            }

        }


        private String[] ReadLineWithAutoCompletion(List<ICLICommands> commands, out Boolean cancelled)
        {

            var input           = new List<Char>();
            var cursorPosition  = 0;
            var historyIndex    = -1;
            var currentInput    = String.Empty;
            cancelled           = false;

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
                        input.AddRange(suggestions[0] + " ");
                        cursorPosition = input.Count;
                        ClearCurrentConsoleLine();
                        CommandPrompt(input);
                    }
                    else if (suggestions.Length > 1)
                    {
                        //if (suggestions.Length == 1)
                        //{
                        //    input.Clear();
                        //    input.AddRange(suggestions[0] + " ");
                        //    cursorPosition = input.Count;
                        //    ClearCurrentConsoleLine();
                        //    CommandPrompt(input);
                        //}
                        // if (suggestions.Length > 1)
                        //{

                            //var suggestions2 = suggestions.Where(s => s.Count() > 0).Select(s => s.AggregateWith(" "));

                            var commonPrefix = new String(suggestions.First()[..suggestions.Min(s => s.Length)].
                                                                      TakeWhile((c, i) => suggestions.All(s => s.Length > i && s[i] == c)).ToArray());

                            input.Clear();
                            input.AddRange(commonPrefix);
                            cursorPosition = input.Count;

                            ClearCurrentConsoleLine();
                            CommandPrompt(input);
                            Console.WriteLine();
                            Console.WriteLine("Suggestions: " + String.Join(", ", suggestions));
                            Console.WriteLine();
                            CommandPrompt(input);

                        //}
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
                    cancelled = true;
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

        private static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(new String(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

    }


}
