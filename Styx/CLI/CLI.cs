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

        /// <summary>
        /// The console is one device, and in a program that does anything besides
        /// reading commands it is written to from several threads at once. Every
        /// write below goes through this, so that two of them cannot end up on
        /// the same line.
        /// </summary>
        private readonly  Lock                     consoleLock      = new();

        /// <summary>
        /// The command line as it currently stands on the screen, while one is
        /// being typed, and null while none is. WriteBlock needs both: what to
        /// take away before it writes, and what to put back afterwards.
        /// </summary>
        private           List<Char>?              liveInput;
        private           Int32                    liveCursor;

        /// <summary>
        /// Where the window onto a line too long for the screen starts - see
        /// LineView. Kept from one redraw to the next, so that the text does not
        /// slide about under somebody editing the middle of it.
        /// </summary>
        private           Int32                    liveOffset;

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

                var inputArgs = await ReadLineWithAutoCompletion(commands);

                if (inputArgs.Item1?.Length > 0 && !inputArgs.Item2)
                {

                    var responseLines = await Execute(inputArgs.Item1);

                    if (responseLines.Length > 0)
                        WriteBlock(() => {
                            foreach (var responseLine in responseLines)
                                Console.WriteLine(responseLine);
                        });

                }

            }
            while (!QuitCLI);
        }


        #region WriteBlock(Write)

        /// <summary>
        /// Write to the console without breaking the command line somebody is
        /// typing at that moment.
        /// </summary>
        /// <remarks>
        /// A program that only reads commands does not need this. One that also
        /// logs what it is doing does: the log is written from whichever thread
        /// did the thing, and half a log entry landing in the middle of a
        /// half-typed command costs both of them - the entry is unreadable and
        /// the command has to be typed again.
        ///
        /// So the input line is taken off the screen, the block is written as
        /// one piece, and the line is put back with the cursor where it was.
        /// The person typing sees their command stay put while the log scrolls
        /// past above it, which is what the line was supposed to do all along.
        ///
        /// Everything this class writes goes through here too, which is the
        /// other half of the promise: the lock is what makes "as one piece"
        /// true, and a block that went around it would still interleave.
        /// </remarks>
        /// <param name="Write">Whatever writes the block. It is called with the console to itself.</param>
        public void WriteBlock(Action Write)
        {

            lock (consoleLock)
            {

                var input = liveInput;

                if (input is not null)
                    ClearCurrentConsoleLine();

                Write();

                if (input is not null)
                    RedrawLocked(input, liveCursor);

            }

        }

        #endregion

        #region (protected virtual) GetPrompt()

        /// <summary>
        /// What to put in front of the command being typed. Overridden by a CLI
        /// that has something more useful to say than "Enter command".
        /// </summary>
        protected virtual String GetPrompt()
        {

            if (Environment.TryGetValue(EnvironmentKey.RemoteSystemId, out var remoteSystemId))
                return $"[{remoteSystemId.First()}] Enter command: ";

            return "Enter command: ";

        }

        #endregion


        /// <summary>
        /// Put the prompt and the given input back on the current line, and the
        /// cursor where it was within it. Only ever called with the console lock
        /// held - see WriteBlock.
        /// </summary>
        /// <remarks>
        /// Through LineView, which keeps it to the one row this editor can take
        /// off the screen and put back. Written straight out, a line wider than
        /// the console wrapped onto a second row, and the cursor was then sent
        /// to a column past the edge of the buffer - which threw, and took the
        /// command line with it.
        ///
        /// The cursor is walked back along the row instead of being sent to its
        /// column, because sending it there took Console.CursorTop - which on
        /// Linux waits for whoever is in Console.ReadKey. See LineView.Clearing.
        /// </remarks>
        private void RedrawLocked(List<Char> Input, Int32 CursorPosition)
        {

            liveInput   = Input;
            liveCursor  = CursorPosition;

            var width   = LineWidth();
            var view    = LineView.Of(GetPrompt(), Input, CursorPosition, width, liveOffset);

            liveOffset  = view.Offset;

            Console.Write(LineView.Clearing(width) + view.Drawing);

        }

        /// <summary>
        /// How many columns a line may use: the narrower of the window and the
        /// buffer, because the cursor can only be put where both are.
        /// </summary>
        private static Int32 LineWidth()

            => Math.Max(1, Math.Min(Console.WindowWidth, Console.BufferWidth));

        /// <summary>
        /// The same, for a caller that does not already hold the lock.
        /// </summary>
        private void Redraw(List<Char> Input, Int32 CursorPosition)
        {
            lock (consoleLock)
            {
                RedrawLocked(Input, CursorPosition);
            }
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

        /// <summary>
        /// Take the command line off the row the cursor is on, and leave the
        /// cursor at the start of that row - without asking the console where
        /// that is. See LineView.Clearing.
        /// </summary>
        private static void ClearCurrentConsoleLine()

            => Console.Write(LineView.Clearing(LineWidth()));

        #endregion

        #region (private) ReadLineWithAutoCompletion(Commands)

        /// <summary>
        /// Read one command line, completing it on Tab.
        /// </summary>
        /// <remarks>
        /// Every write in here goes through Redraw or WriteBlock rather than
        /// straight to the console, and the line being typed is published in
        /// liveInput while it is being typed. That is what lets another thread
        /// log something in the middle of it without the two ending up on the
        /// same line - see WriteBlock.
        /// </remarks>
        private async Task<Tuple<String[], Boolean>> ReadLineWithAutoCompletion(List<ICLICommand> Commands)
        {

            var input           = new List<Char>();
            var cursorPosition  = 0;
            var historyIndex    = -1;
            var currentInput    = String.Empty;

            // Finish the line: it is a line of history now rather than something
            // to be put back, so liveInput goes first and the newline second.
            //
            // A line too long for the screen was shown through a window onto
            // it. What stays behind in the scrollback is the whole of it,
            // wrapped like any other text, because that is what was run - and
            // it is never going to be taken off the screen again.
            void FinishLine()
            {
                lock (consoleLock)
                {

                    liveInput = null;

                    if (!LineView.Of(GetPrompt(), input, input.Count, LineWidth()).ShowsAll)
                    {
                        ClearCurrentConsoleLine();
                        Console.Write(GetPrompt() + new String(input.ToArray()));
                    }

                    Console.WriteLine();

                }
            }

            // Leave the line that was typed standing where it is, write
            // something underneath it, and come back to a fresh prompt. Used by
            // Tab, which answers with more than fits on one line.
            void WriteUnder(Action Write)
            {
                WriteBlock(() => {
                    Console.WriteLine(GetPrompt() + new String(input.ToArray()));
                    Write();
                });
            }

            // A new line starts with its window at its start.
            lock (consoleLock)
            {
                liveOffset = 0;
                RedrawLocked(input, cursorPosition);
            }

            try
            {

                while (true)
                {

                    var key = Console.ReadKey(intercept: true);

                    if (key.Key == ConsoleKey.Tab)
                    {

                        var suggestions = await Suggest(ParseCommandLine(new String(input.ToArray())));

                        if (suggestions.Length == 1)
                        {

                            if (suggestions[0].Info == SuggestionInfo.CommandHelp)
                                WriteUnder(() => {
                                    Console.WriteLine();
                                    Console.WriteLine($"Usage: {suggestions[0].Suggestion}");
                                    Console.WriteLine();
                                });

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
                                Redraw(input, cursorPosition);

                            }

                        }

                        else if (suggestions.Length > 1)
                        {

                            var commonPrefix = new String(suggestions.First().Suggestion[..suggestions.Min(s => s.Suggestion.Length)].
                                                                        TakeWhile((c, i) => suggestions.All(s => s?.Suggestion.Length > i && s.Suggestion[i] == c)).ToArray());

                            WriteUnder(() => {
                                Console.WriteLine();
                                Console.WriteLine("Suggestions:");
                                foreach (var suggestion in suggestions)
                                    Console.WriteLine($"   {suggestion.Suggestion}");
                                Console.WriteLine();
                            });

                            input.Clear();
                            input.AddRange(commonPrefix);
                            cursorPosition = input.Count;

                            Redraw(input, cursorPosition);

                        }

                    }

                    else if (key.Key == ConsoleKey.Home)
                    {
                        cursorPosition = 0;
                        Redraw(input, cursorPosition);
                    }

                    else if (key.Key == ConsoleKey.End)
                    {
                        cursorPosition = input.Count;
                        Redraw(input, cursorPosition);
                    }

                    else if (key.Key == ConsoleKey.Enter)
                    {
                        FinishLine();
                        return new Tuple<String[], Boolean>(ParseCommandLine(new String(input.ToArray())), false);
                    }

                    else if (key.Key == ConsoleKey.Backspace && cursorPosition > 0)
                    {
                        input.RemoveAt(cursorPosition - 1);
                        cursorPosition--;
                        Redraw(input, cursorPosition);
                    }

                    else if (key.Key == ConsoleKey.Delete && cursorPosition < input.Count)
                    {
                        input.RemoveAt(cursorPosition);
                        Redraw(input, cursorPosition);
                    }

                    else if (key.Key == ConsoleKey.LeftArrow && cursorPosition > 0)
                    {
                        cursorPosition--;
                        Redraw(input, cursorPosition);
                    }

                    else if (key.Key == ConsoleKey.RightArrow && cursorPosition < input.Count)
                    {
                        cursorPosition++;
                        Redraw(input, cursorPosition);
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
                            Redraw(input, cursorPosition);
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
                            Redraw(input, cursorPosition);

                        }
                    }

                    else if (key.Key == ConsoleKey.Escape)
                    {
                        FinishLine();
                        return new Tuple<String[], Boolean>([], true);
                    }

                    else if (!char.IsControl(key.KeyChar))
                    {
                        input.Insert(cursorPosition, key.KeyChar);
                        cursorPosition++;
                        Redraw(input, cursorPosition);
                    }

                }

            }
            finally
            {
                // However this line ended - returned, or thrown out of - there
                // is no longer a command line on the screen to be put back.
                lock (consoleLock)
                {
                    liveInput = null;
                }
            }

        }

        #endregion


    }

}
