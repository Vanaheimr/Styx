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
    /// <remarks>
    /// <para>
    /// Typed at on a terminal: the console of this process unless it was given
    /// another one, such as a <see cref="VT100Terminal"/> at the far end of an
    /// SSH session. Every key is read from it and everything is written to it,
    /// so the same editor - Tab, the history, a line too long for its row,
    /// a log written in the middle of it - is the same wherever it is typed at.
    /// </para>
    /// <para>
    /// Ctrl+C stops the command that is running, and only that one: every
    /// command gets a cancellation of its own. There used to be one for the
    /// whole life of the command line, cancelled by the first Ctrl+C and never
    /// made again, after which every command was cancelled before it started.
    /// </para>
    /// </remarks>
    public class CLI : ICLI,
                      IDisposable
    {

        #region (record struct) TypedLine

        /// <summary>
        /// What one line of typing came to: a command, nothing because it was
        /// abandoned, or nothing because no key will ever come again.
        /// </summary>
        private readonly record struct TypedLine(String[]  Arguments,
                                                 Boolean   Abandoned,
                                                 Boolean   EndOfInput);

        #endregion

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

        /// <summary>
        /// The control characters the editor knows by their character, which is
        /// what every terminal agrees on: a console reports Ctrl+A with
        /// ConsoleKey.A, a byte stream knows nothing of ConsoleKey at all.
        /// </summary>
        private const     Char                     CtrlA            = '\x01';
        private const     Char                     CtrlC            = '\x03';
        private const     Char                     CtrlD            = '\x04';
        private const     Char                     CtrlE            = '\x05';

        private readonly  List<ICLICommand>        commands         = [];
        private readonly  List<String>             commandHistory   = [];

        /// <summary>
        /// What this is typed at and written on, and whether it was made here -
        /// the console's is, and is let go of with this.
        /// </summary>
        private readonly  ICLITerminal             terminal;
        private readonly  Boolean                  ownsTerminal;

        /// <summary>
        /// The cancellation of the command running now, for Ctrl+C to cancel;
        /// null while none is.
        /// </summary>
        private           CancellationTokenSource? runningCommand;

        /// <summary>
        /// A key asked for and not yet taken. Kept rather than dropped where
        /// whoever asked stopped waiting - a prompt that was told to end, a
        /// command that finished - because the key it will bring is still
        /// somebody's next key, and belongs to the next prompt.
        /// </summary>
        private           Task<ConsoleKeyInfo?>?   pendingKey;

        /// <summary>
        /// Keys typed while a command ran, read to see whether one of them was
        /// Ctrl+C, and kept for the line after it - as a shell keeps what is
        /// typed ahead.
        /// </summary>
        private readonly  Queue<ConsoleKeyInfo>    typedAhead       = new();

        /// <summary>
        /// Whether the terminal said that no key will ever come again.
        /// </summary>
        private           Boolean                  inputEnded;

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

        /// <summary>
        /// What this command line is typed at and written on.
        /// </summary>
        public ICLITerminal                                                  Terminal
            => terminal;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new command line interface on the console of this process.
        /// </summary>
        /// <param name="AssembliesWithCLICommands">The assemblies to search for commands.</param>
        public CLI(params Assembly[] AssembliesWithCLICommands)

            : this(new SystemConsoleTerminal(),
                   true,
                   AssembliesWithCLICommands)

        { }

        /// <summary>
        /// Create a new command line interface on the given terminal.
        /// </summary>
        /// <param name="Terminal">What the command line is typed at and written on. Whoever made it lets go of it.</param>
        /// <param name="AssembliesWithCLICommands">The assemblies to search for commands.</param>
        public CLI(ICLITerminal       Terminal,
                   params Assembly[]  AssembliesWithCLICommands)

            : this(Terminal,
                   false,
                   AssembliesWithCLICommands)

        { }

        private CLI(ICLITerminal  Terminal,
                    Boolean       OwnsTerminal,
                    Assembly[]    AssembliesWithCLICommands)
        {

            this.terminal      = Terminal;
            this.ownsTerminal  = OwnsTerminal;

            terminal.Interrupted  += OnInterrupted;
            terminal.Resized      += OnResized;

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


        #region Run(CancellationToken = default)

        /// <summary>
        /// Read a command, run it and write what it answered, until 'quit',
        /// Ctrl+D on an empty line, the end of the terminal's input or the
        /// given token.
        /// </summary>
        public Task Run()

            => Run(CancellationToken.None);

        /// <summary>
        /// Read a command, run it and write what it answered, until 'quit',
        /// Ctrl+D on an empty line, the end of the terminal's input or the
        /// given token.
        /// </summary>
        /// <param name="CancellationToken">Ends the command line, and whatever command is running in it.</param>
        public async Task Run(CancellationToken CancellationToken)
        {
            do
            {

                var line = await ReadLineWithAutoCompletion(CancellationToken);

                if (line.EndOfInput)
                    return;

                if (line.Arguments.Length > 0 && !line.Abandoned)
                {

                    var responseLines = await ExecuteTyped(line.Arguments, CancellationToken);

                    if (responseLines.Length > 0)
                        WriteBlock(terminal => {
                            foreach (var responseLine in responseLines)
                                terminal.WriteLine(responseLine);
                        });

                }

            }
            while (!QuitCLI && !CancellationToken.IsCancellationRequested);
        }

        #endregion


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
        ///
        /// The block writes where it likes - System.Console, for the log of a
        /// program at its own console. On any other terminal it would write
        /// beside the command line rather than on it: that is what the overload
        /// handing the block the terminal is for.
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

        #region WriteBlock(Write)

        /// <summary>
        /// Write to this command line's terminal without breaking the command
        /// line somebody is typing at that moment - see <see cref="WriteBlock(Action)"/>.
        /// </summary>
        /// <param name="Write">Whatever writes the block, on the terminal it is handed, which it has to itself.</param>
        public void WriteBlock(Action<ICLITerminal> Write)

            => WriteBlock(() => Write(terminal));

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

            terminal.Write(LineView.Clearing(width) + view.Drawing);

        }

        /// <summary>
        /// How many columns a line may use: as many as the terminal says a row has.
        /// </summary>
        private Int32 LineWidth()

            => Math.Max(1, terminal.Width);

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


        #region Execute(Command, CancellationToken = default)

        public Task<String[]> Execute(String Command)

            => Execute(ParseCommandLine(Command), CancellationToken.None);

        public Task<String[]> Execute(String             Command,
                                      CancellationToken  CancellationToken)

            => Execute(ParseCommandLine(Command), CancellationToken);

        #endregion

        #region Execute(InputArguments, CancellationToken = default)

        public Task<String[]> Execute(String[] InputArguments)

            => Execute(InputArguments, CancellationToken.None);

        /// <summary>
        /// Run the given command line, until it is done, the given token says
        /// stop, or somebody interrupts it at the terminal.
        /// </summary>
        /// <param name="InputArguments">The command line, split into its words.</param>
        /// <param name="CancellationToken">An optional token to cancel the command.</param>
        public async Task<String[]> Execute(String[]           InputArguments,
                                            CancellationToken  CancellationToken)
        {

            // A cancellation of its own, for an interrupt to cancel: this one
            // command, and not whatever runs after it.
            using var thisCommand  = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
            var outerCommand       = Interlocked.Exchange(ref runningCommand, thisCommand);

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

                    return await matchingCommands.First().Execute(InputArguments, thisCommand.Token);

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
            finally
            {
                Interlocked.CompareExchange(ref runningCommand, outerCommand, thisCommand);
            }

        }

        #endregion

        #region (private) ExecuteTyped(Arguments, CancellationToken)

        /// <summary>
        /// Run a command that was typed - and, on a terminal whose Ctrl+C is a
        /// key, go on reading keys while it runs, so that Ctrl+C can stop it.
        /// </summary>
        /// <remarks>
        /// Every other key read meanwhile is kept for the next line, as a shell
        /// keeps what is typed ahead. A terminal that ends while the command runs
        /// cancels it: there is nobody left to read its answer.
        /// </remarks>
        private async Task<String[]> ExecuteTyped(String[]           Arguments,
                                                  CancellationToken  CancellationToken)
        {

            if (!terminal.InterruptsArriveAsKeys)
                return await Execute(Arguments, CancellationToken);

            using var watched  = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
            var running        = Execute(Arguments, watched.Token);

            while (!running.IsCompleted && !inputEnded)
            {

                var key = PendingKey();

                if (await Task.WhenAny(running, key) != key)
                    break;

                pendingKey = null;

                ConsoleKeyInfo? typed;

                try
                {
                    typed = await key;
                }
                catch
                {
                    typed = null;
                }

                if (typed is not ConsoleKeyInfo next)
                {
                    inputEnded = true;
                    await watched.CancelAsync();
                }

                else if (next.KeyChar == CtrlC)
                    await watched.CancelAsync();

                else
                    typedAhead.Enqueue(next);

            }

            return await running;

        }

        #endregion


        #region (private) PendingKey() / NextKey(CancellationToken)

        /// <summary>
        /// The key asked of the terminal and not yet taken, asked for now where
        /// there is none.
        /// </summary>
        private Task<ConsoleKeyInfo?> PendingKey()

            => pendingKey ??= terminal.ReadKeyAsync().AsTask();

        /// <summary>
        /// The next key: one typed ahead while a command ran, or the terminal's
        /// next; null once no key will ever come again.
        /// </summary>
        private async Task<ConsoleKeyInfo?> NextKey(CancellationToken CancellationToken)
        {

            if (typedAhead.TryDequeue(out var typed))
                return typed;

            if (inputEnded)
                return null;

            var key = await PendingKey().WaitAsync(CancellationToken);

            pendingKey = null;

            if (key is null)
                inputEnded = true;

            return key;

        }

        #endregion

        #region (private) OnInterrupted() / OnResized()

        /// <summary>
        /// Stop the command that is running, if one is - Ctrl+C at a console, a
        /// signal from the far end of a connection.
        /// </summary>
        private void OnInterrupted()
        {
            try
            {
                Volatile.Read(ref runningCommand)?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // It ended just now.
            }
        }

        /// <summary>
        /// Draw the line being typed again for the new width: a line that fitted
        /// the old one may not fit this one, and a line that did not may now.
        /// </summary>
        private void OnResized()
        {
            lock (consoleLock)
            {
                if (liveInput is List<Char> input)
                    RedrawLocked(input, liveCursor);
            }
        }

        #endregion


        #region (private) ClearCurrentConsoleLine()

        /// <summary>
        /// Take the command line off the row the cursor is on, and leave the
        /// cursor at the start of that row - without asking the terminal where
        /// that is. See LineView.Clearing.
        /// </summary>
        private void ClearCurrentConsoleLine()

            => terminal.Write(LineView.Clearing(LineWidth()));

        #endregion

        #region (private) ReadLineWithAutoCompletion(CancellationToken)

        /// <summary>
        /// Read one command line, completing it on Tab.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Every write in here goes through Redraw or WriteBlock rather than
        /// straight to the terminal, and the line being typed is published in
        /// liveInput while it is being typed. That is what lets another thread
        /// log something in the middle of it without the two ending up on the
        /// same line - see WriteBlock.
        /// </para>
        /// <para>
        /// Ctrl+C abandons the line, as a shell does, where Ctrl+C is a key at
        /// all - a console takes it as a signal instead. Ctrl+D leaves on an
        /// empty line and deletes the character under the cursor on any other,
        /// and Ctrl+A and Ctrl+E go to the start and the end, all as readline
        /// has it, because that is what the hands of somebody at a terminal do.
        /// </para>
        /// </remarks>
        /// <param name="CancellationToken">Ends the reading: the line is then given up, and nothing more is read.</param>
        private async Task<TypedLine> ReadLineWithAutoCompletion(CancellationToken CancellationToken)
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
            // it is never going to be taken off the screen again. So is an
            // abandoned line, with what abandoned it after it.
            void FinishLine(String After = "")
            {
                lock (consoleLock)
                {

                    liveInput = null;

                    if (After.Length > 0 || !LineView.Of(GetPrompt(), input, input.Count, LineWidth()).ShowsAll)
                    {
                        ClearCurrentConsoleLine();
                        terminal.Write(GetPrompt() + new String(input.ToArray()) + After);
                    }

                    terminal.WriteLine();

                }
            }

            // Leave the line that was typed standing where it is, write
            // something underneath it, and come back to a fresh prompt. Used by
            // Tab, which answers with more than fits on one line.
            void WriteUnder(Action<ICLITerminal> Write)
            {
                WriteBlock(terminal => {
                    terminal.WriteLine(GetPrompt() + new String(input.ToArray()));
                    Write(terminal);
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

                    ConsoleKeyInfo? next;

                    try
                    {
                        next = await NextKey(CancellationToken);
                    }
                    catch (OperationCanceledException) when (CancellationToken.IsCancellationRequested)
                    {
                        FinishLine();
                        return new TypedLine([], false, true);
                    }

                    // No key will ever come again: what was typed so far is not
                    // run - nobody pressed Enter - and the command line ends.
                    if (next is not ConsoleKeyInfo key)
                    {
                        FinishLine();
                        return new TypedLine([], false, true);
                    }

                    if (key.KeyChar == CtrlC)
                    {
                        FinishLine("^C");
                        return new TypedLine([], true, false);
                    }

                    if (key.KeyChar == CtrlD)
                    {

                        if (input.Count == 0)
                        {
                            FinishLine();
                            return new TypedLine([], false, true);
                        }

                        if (cursorPosition < input.Count)
                        {
                            input.RemoveAt(cursorPosition);
                            Redraw(input, cursorPosition);
                        }

                        continue;

                    }

                    if (key.KeyChar == CtrlA)
                    {
                        cursorPosition = 0;
                        Redraw(input, cursorPosition);
                        continue;
                    }

                    if (key.KeyChar == CtrlE)
                    {
                        cursorPosition = input.Count;
                        Redraw(input, cursorPosition);
                        continue;
                    }

                    if (key.Key == ConsoleKey.Tab)
                    {

                        var suggestions = await Suggest(ParseCommandLine(new String(input.ToArray())));

                        if (suggestions.Length == 1)
                        {

                            if (suggestions[0].Info == SuggestionInfo.CommandHelp)
                                WriteUnder(terminal => {
                                    terminal.WriteLine();
                                    terminal.WriteLine($"Usage: {suggestions[0].Suggestion}");
                                    terminal.WriteLine();
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

                            WriteUnder(terminal => {
                                terminal.WriteLine();
                                terminal.WriteLine("Suggestions:");
                                foreach (var suggestion in suggestions)
                                    terminal.WriteLine($"   {suggestion.Suggestion}");
                                terminal.WriteLine();
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
                        return new TypedLine(ParseCommandLine(new String(input.ToArray())), false, false);
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
                        return new TypedLine([], true, false);
                    }

                    else if (!Char.IsControl(key.KeyChar) && key.KeyChar != '\0')
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


        #region Dispose()

        /// <summary>
        /// Let go of the terminal: of its events, and of the terminal itself
        /// where it was made here - the console's, which lets go of Ctrl+C.
        /// </summary>
        public void Dispose()
        {

            terminal.Interrupted  -= OnInterrupted;
            terminal.Resized      -= OnResized;

            if (ownsTerminal && terminal is IDisposable disposable)
                disposable.Dispose();

            GC.SuppressFinalize(this);

        }

        #endregion


    }

}
