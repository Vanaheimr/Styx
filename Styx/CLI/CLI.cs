/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Collections.Concurrent;
using org.GraphDefined.Vanaheimr.Illias;
using System.Text.RegularExpressions;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI
{

    /// <summary>
    /// Holds the result of the autocompletion process.
    /// </summary>
    public class AutoCompleteResult
    {
        public String        ExpandedPath    { get; set; } = String.Empty;
        public List<String>  Candidates      { get; set; } = [];

    }

    public static class AutoComplete
    {

        /// <summary>
        /// Attempts to autocomplete a partial path. Returns the path expanded
        /// to its longest common prefix, and a list of all matching candidates
        /// if there's more than one.
        /// </summary>
        /// <param name="partialPath">The user-typed partial path (may be file or directory).</param>
        public static AutoCompleteResult AutoCompletePath(String partialPath)
        {

            var result = new AutoCompleteResult {
                ExpandedPath = partialPath
            };

            if (String.IsNullOrWhiteSpace(partialPath))
                return result;

            if (partialPath == "~")
                partialPath = Directory.GetCurrentDirectory();

            // Normalize directory separators to backslashes for internal handling
            // (You could use Path.DirectorySeparatorChar, but let's be explicit).
            partialPath = partialPath.Replace('/', '\\');

            // If partialPath points to a directory (e.g., ends with slash), 
            // let's remove it so we can split out the "parent directory" vs. "prefix" in a uniform way.
            var endedWithSeparator = partialPath.EndsWith("\\", StringComparison.Ordinal);
            if (endedWithSeparator && partialPath.Length > 1)
            {
                partialPath = partialPath.TrimEnd('\\');
            }

            // Extract the directory portion that definitely exists and the partial name
            var directoryPart = Path.GetDirectoryName(partialPath);
            if (String.IsNullOrEmpty(directoryPart))
            {
                // If there's no directory part, assume the current directory
                directoryPart = Directory.GetCurrentDirectory();
            }
            else if (!Directory.Exists(directoryPart))
            {
                // Try walking up the partial directory string until we get a valid directory
                directoryPart = FindLongestExistingDirectory(directoryPart);
            }

            // The last part in partialPath, which we will try to match to files/dirs
            var partialName = Path.GetFileName(partialPath);
            if (String.IsNullOrEmpty(partialName) && endedWithSeparator)
            {
                // If partialPath ended with a slash, partialName is empty, so just treat it as
                // "list everything inside directoryPart".
                partialName = String.Empty;
            }


            var candidates = new List<String>();

            try
            {

                candidates.AddRange(
                    new DirectoryInfo(directoryPart).
                        GetFiles("*.*", SearchOption.TopDirectoryOnly).
                        Where   (name     => name.FullName.StartsWith(partialPath, StringComparison.OrdinalIgnoreCase)).
                        Select  (filepath => filepath.FullName)
                );

                candidates.AddRange(
                    new DirectoryInfo(directoryPart).
                        GetDirectories("*.*", SearchOption.TopDirectoryOnly).
                        Where(name => name.FullName.StartsWith(partialPath, StringComparison.OrdinalIgnoreCase)).
                        Select(filepath => filepath.FullName)
                );

            }
            catch (Exception e)
            {
                DebugX.Log(e.Message);
                return result;
            }

            if (candidates.Count == 1)
            {

                var candidate = candidates.First();
                candidates.Clear();

                candidates.AddRange(
                    new DirectoryInfo(candidate).
                        GetFiles("*.*", SearchOption.TopDirectoryOnly).
                        Where   (name     => name.FullName.StartsWith(partialPath, StringComparison.OrdinalIgnoreCase)).
                        Select  (filepath => filepath.FullName)
                );

                candidates.AddRange(
                    new DirectoryInfo(candidate).
                        GetDirectories("*.*", SearchOption.TopDirectoryOnly).
                        Where(name => name.FullName.StartsWith(partialPath, StringComparison.OrdinalIgnoreCase)).
                        Select(filepath => filepath.FullName)
                );

            }


            #region No match

            if (candidates.Count == 0)
                return result;

            #endregion

            #region Exactly one match...

            if (candidates.Count == 1)
            {

                var singleMatch = candidates[0];
                var expanded    = Path.Combine(directoryPart, singleMatch);

                // If it's a directory, append a slash to make it obvious
                if (Directory.Exists(expanded))
                    expanded += "\\";

                result.ExpandedPath = expanded;
                result.Candidates.Add(expanded);

                return result;

            }

            #endregion

            #region ...or multiple matches

            var commonPrefix = FindLongestCommonPrefix(candidates, partialPath);

            // Expand partialPath with the found common prefix
            if (commonPrefix.Length > 0 && commonPrefix.Length > partialName.Length)
            {

                var prefixPath = Path.Combine(directoryPart, commonPrefix);

                if (Directory.Exists(prefixPath))
                    prefixPath += "\\";

                result.ExpandedPath = prefixPath;

            }

            // Provide full paths as well, so the consumer can show them or let the user pick
            result.Candidates = candidates.Select(c => {
                var fullPath = Path.Combine(directoryPart, c);
                if (Directory.Exists(fullPath)) fullPath += "\\";
                return fullPath;
            }).
                                           ToList();

            return result;

            #endregion

        }

        /// <summary>
        /// Finds the longest existing directory by peeling off path segments 
        /// until an existing directory is found, or returns an empty string if none found.
        /// </summary>
        private static String FindLongestExistingDirectory(String path)
        {

            while (!String.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                path = Path.GetDirectoryName(path) ?? String.Empty;
            }

            return String.IsNullOrEmpty(path)
                       ? Directory.GetCurrentDirectory()
                       : path;

        }

        /// <summary>
        /// Finds the longest common prefix of all strings in 'values', starting from 
        /// the existing partialName (so we only extend beyond partialName).
        /// Case-insensitive match is used here for path convenience.
        /// </summary>
        private static String FindLongestCommonPrefix(List<String>  values,
                                                      String        partialName)
        {

            if (values is null || values.Count == 0)
                return partialName;

            // Convert everything to the same case for prefix-finding
            var lowered       = values.Select(v => v.ToLowerInvariant()).ToList();
            var basePartial   = partialName.ToLowerInvariant();

            // We'll accumulate the prefix in commonPrefix
            var commonPrefix  = basePartial;

            // The maximum possible length of the common prefix can't exceed 
            // the length of the shortest candidate.
            var minLength     = lowered.Min(s => s.Length);

            for (var i = commonPrefix.Length; i < minLength; i++)
            {

                // The character to compare for all candidates
                var c = lowered[0][i];

                // Check if all match this character
                if (lowered.Any(s => s[i] != c))
                    break;

                // Append the character
                commonPrefix += c;

            }

            return commonPrefix;

        }

    }


    /// <summary>
    /// A Command Line Interface for executing commands.
    /// </summary>
    public class CLI : ICLI
    {

        #region (static class) DefaultStrings

        /// <summary>
        /// Default strings.
        /// </summary>
        public static class DefaultStrings
        {
            //public const EnvironmentKey RemoteSystemId = EnvironmentKey.Parse("remoteSystemId");
        }

        #endregion


        #region Data

        private readonly  List<ICLICommand>        commands         = [];
        private readonly  List<String>             commandHistory   = [];
        private readonly  CancellationTokenSource  cts              = new();

        #endregion

        #region Properties

        /// <summary>
        /// All registered commands.
        /// </summary>
        public IEnumerable<ICLICommand>                                      Commands
            => commands;

        /// <summary>
        /// The command history.
        /// </summary>
        public IEnumerable<String>                                           CommandHistory
            => commandHistory;

        public Boolean                                                       QuitCLI        { get; set; } = false;


        public ConcurrentDictionary<EnvironmentKey, ConcurrentList<String>>  Environment    { get; }      = [];

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


        #region RegisterCLIType    (CLIType)

        /// <summary>
        /// Register all commands of the given CLI type and its assembly.
        /// </summary>
        /// <param name="CLIType">A CLI type.</param>
        public void RegisterCLIType(Type CLIType)
        {

                commands.AddRange(
                    CLIType.Assembly.
                        GetTypes().
                        Where(type => typeof(ICLICommand).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor(Type.EmptyTypes) is not null).
                        Select(type => (ICLICommand) Activator.CreateInstance(type)!)
                );

                commands.AddRange(
                    CLIType.Assembly.
                        GetTypes().
                        Where(type => typeof(ICLICommand).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor([CLIType]) is not null).
                        Select(type => (ICLICommand) Activator.CreateInstance(type, this)!)
                );

        }

        #endregion

        #region RegisterAssemblies (AssembliesWithCLICommands)

        /// <summary>
        /// Register all commands from the given assemblies.
        /// </summary>
        /// <param name="AssembliesWithCLICommands">An array of assemblies to search for commands.</param>
        public void RegisterAssemblies(params Assembly[] AssembliesWithCLICommands)
        {

            foreach (var assemblyWithCLICommands in AssembliesWithCLICommands.Distinct())
            {

                commands.AddRange(
                    assemblyWithCLICommands.
                        GetTypes().
                        Where(type => typeof(ICLICommand).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor(Type.EmptyTypes) is not null).
                        Select(type => (ICLICommand) Activator.CreateInstance(type)!)
                );

                commands.AddRange(
                    assemblyWithCLICommands.
                        GetTypes().
                        Where(type => typeof(ICLICommand).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor([typeof(CLI)]) is not null).
                        Select(type => (ICLICommand) Activator.CreateInstance(type, this)!)
                );

            }

        }

        #endregion

        #region RegisterAssemblies (CLIType, AssembliesWithCLICommands)

        /// <summary>
        /// Register all commands of the given CLI type and from the given assemblies.
        /// </summary>
        /// <param name="CLIType">A CLI type.</param>
        /// <param name="AssembliesWithCLICommands">An array of assemblies to search for commands.</param>
        public void RegisterAssemblies(Type CLIType, params Assembly[] AssembliesWithCLICommands)
        {

            foreach (var assemblyWithCLICommands in AssembliesWithCLICommands.Distinct())
            {

                commands.AddRange(
                    assemblyWithCLICommands.
                        GetTypes().
                        Where(type => typeof(ICLICommand).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor(Type.EmptyTypes) is not null).
                        Select(type => (ICLICommand) Activator.CreateInstance(type)!)
                );

                commands.AddRange(
                    assemblyWithCLICommands.
                        GetTypes().
                        Where(type => typeof(ICLICommand).IsAssignableFrom(type) &&
                                      !type.IsAbstract &&
                                      !type.IsInterface &&
                                       type.GetConstructor([CLIType]) is not null).
                        Select(type => (ICLICommand) Activator.CreateInstance(type, this)!)
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

            if (Environment.TryGetValue(EnvironmentKey.RemoteSystemId, out var remoteSystemId))
                return $"[{remoteSystemId.First()}] Enter command: ";

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



        public static string[] ParseCommandLine(string input)
        {
            // This pattern matches either:
            // 1) Quoted text: "([^"]*)"
            // 2) Or unquoted segments: ([^\s]+)
            var matches = Regex.Matches(input, "\"([^\"]*)\"|([^\\s]+)");
            var tokens = new List<string>();

            foreach (Match match in matches)
            {
                // If Group[1] is not empty, that's the quoted segment (without the quotes)
                if (!string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    tokens.Add(match.Groups[1].Value);
                }
                else
                {
                    // Otherwise, it's a regular token
                    tokens.Add(match.Groups[2].Value);
                }
            }

            return tokens.ToArray();
        }


        #region Suggest(Command)

        public Task<SuggestionResponse[]> Suggest(String Command)

            => //Suggest(Command.Split(' ', StringSplitOptions.RemoveEmptyEntries));
               Suggest(ParseCommandLine(Command));

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

            => //Execute(Command.Split(' ', StringSplitOptions.RemoveEmptyEntries));
               Execute(ParseCommandLine(Command));

        #endregion

        #region Execute(InputArguments)

        public async Task<String[]> Execute(String[] InputArguments)
        {

            try
            {

                var matchingCommands = commands.Where(c => {

                                                        var s = c.Suggest([InputArguments[0]]).FirstOrDefault()?.Suggestion ?? "";

                                                        return s.Equals    (InputArguments[0],       StringComparison.OrdinalIgnoreCase) ||
                                                               s.StartsWith(InputArguments[0] + " ", StringComparison.OrdinalIgnoreCase);

                                                     }).ToArray();
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

                    var suggestions = await Suggest(ParseCommandLine(new String(input.ToArray())));

                    if (suggestions.Length == 0)
                    {
                    }
                    if (suggestions.Length == 1)
                    {

                        if (suggestions[0].Info == SuggestionInfo.CommandHelp)
                        {
                            ClearCurrentConsoleLine();
                            CommandPrompt(input);
                            Console.WriteLine();
                            Console.WriteLine($"Usage: {suggestions[0].Suggestion}");
                            Console.WriteLine();
                            CommandPrompt(input);
                        }
                        else
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
                        Console.WriteLine("Suggestions:");
                        foreach (var suggestion in suggestions)
                            Console.WriteLine($"   {suggestion.Suggestion}");
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
                    return new Tuple<String[], Boolean>(ParseCommandLine(new String(input.ToArray())), false);
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
