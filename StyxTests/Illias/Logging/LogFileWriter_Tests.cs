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

using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Illias.Logging;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// What a LogFileWriter does with a line it took: it writes it to its
    /// file, or it tells OnLineNotWritten that it did not - also when it is
    /// disposed with lines still waiting.
    /// </summary>
    /// <remarks>
    /// WWCP_OCPI keeps its database files with one of these, so a line lost
    /// here is a change lost at the next start.
    /// </remarks>
    [TestFixture]
    public class LogFileWriter_Tests
    {

        #region Data

        /// <summary>
        /// How long a step may take before the test fails rather than hangs.
        /// </summary>
        private static readonly TimeSpan StepTimeout = TimeSpan.FromSeconds(10);

        private String directory = "";

        private static String[] Lines(String Name, Int32 Count)

            => [.. Enumerable.Range(1, Count).Select(i => $"{Name} {i}")];

        private static String[] LinesIn(String FileName)

            => File.Exists(FileName)
                   ? File.ReadAllLines(FileName)
                   : [];

        #endregion

        #region Setup/TearDown

        [SetUp]
        public void CreateDirectory()
        {
            directory = Path.Combine(Path.GetTempPath(), "StyxTests", nameof(LogFileWriter_Tests), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
        }

        [TearDown]
        public void DeleteDirectory()
        {
            try
            {
                Directory.Delete(directory, recursive: true);
            }
            catch
            { }
        }

        #endregion


        #region Every_line_handed_over_before_DisposeAsync_is_in_its_file_in_order()

        /// <summary>
        /// A burst of lines for two files, as an import leaves behind, and the
        /// writer disposed right after it: every line is written all the same.
        /// </summary>
        [Test]
        public async Task Every_line_handed_over_before_DisposeAsync_is_in_its_file_in_order()
        {

            var assets   = Path.Combine(directory, "assets.db");
            var parties  = Path.Combine(directory, "remoteParties.db");
            var writer   = new LogFileWriter(10000);

            foreach (var i in Enumerable.Range(1, 1000))
            {
                await writer.EnqueueAsync(assets,  $"asset {i}");
                await writer.EnqueueAsync(parties, $"party {i}");
            }

            await writer.DisposeAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(LinesIn(assets),   Is.EqualTo(Lines("asset", 1000)));
                Assert.That(LinesIn(parties),  Is.EqualTo(Lines("party", 1000)));
            }

        }

        #endregion

        #region DisposeAsync_twice_is_the_same_as_once()

        [Test]
        public async Task DisposeAsync_twice_is_the_same_as_once()
        {

            var file    = Path.Combine(directory, "twice.log");
            var writer  = new LogFileWriter();

            await writer.EnqueueAsync(file, "the only line");

            await writer.DisposeAsync();
            await writer.DisposeAsync();

            Assert.That(LinesIn(file), Is.EqualTo(new[] { "the only line" }));

        }

        #endregion

        #region A_second_DisposeAsync_returns_only_once_the_first_is_done()

        /// <summary>
        /// Whoever disposes the writer a second time may rely on its lines
        /// being written when that returns, just as the first caller may.
        /// </summary>
        [Test]
        public async Task A_second_DisposeAsync_returns_only_once_the_first_is_done()
        {

            var file    = Path.Combine(directory, "held.log");
            var begun   = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var gate    = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            var writer  = new LogFileWriter(
                              1000,
                              TimeSpan.FromMinutes(10),
                              async (fileName, text, cancellationToken) => {
                                  begun.TrySetResult();
                                  await gate.Task.WaitAsync(cancellationToken);
                                  await File.AppendAllTextAsync(fileName, text, cancellationToken);
                              }
                          );

            await writer.EnqueueAsync(file, "the only line");
            await begun.Task.WaitAsync(StepTimeout);

            var first   = writer.DisposeAsync().AsTask();
            var second  = writer.DisposeAsync().AsTask();

            Assert.That(await Task.WhenAny(second, Task.Delay(200)), Is.Not.SameAs(second),
                        "The second DisposeAsync returned while the line was still being written.");

            gate.SetResult();
            await Task.WhenAll(first, second).WaitAsync(StepTimeout);

            Assert.That(LinesIn(file), Is.EqualTo(new[] { "the only line" }));

        }

        #endregion

        #region A_line_its_file_refuses_is_reported_and_the_lines_after_it_are_still_written()

        [Test]
        public async Task A_line_its_file_refuses_is_reported_and_the_lines_after_it_are_still_written()
        {

            var written  = Path.Combine(directory, "written.log");
            var refused  = Path.Combine(directory, "no such directory", "refused.log");
            var reports  = new ConcurrentQueue<(LogFileWriter Sender, String FileName, String Line, Exception Exception)>();
            var writer   = new LogFileWriter();

            writer.OnLineNotWritten += (timestamp, sender, fileName, line, exception) => reports.Enqueue((sender, fileName, line, exception));

            await writer.EnqueueAsync(written, "before");
            await writer.EnqueueAsync(refused, "refused");
            await writer.EnqueueAsync(written, "after");

            await writer.DisposeAsync().AsTask().WaitAsync(StepTimeout);

            Assert.That(LinesIn(written),  Is.EqualTo(new[] { "before", "after" }));
            Assert.That(reports,           Has.Count.EqualTo(1));

            var report = reports.Single();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(report.Sender,     Is.SameAs(writer));
                Assert.That(report.FileName,   Is.EqualTo(refused));
                Assert.That(report.Line,       Is.EqualTo("refused"));
                Assert.That(report.Exception,  Is.InstanceOf<IOException>());
            }

        }

        #endregion

        #region DisposeAsync_gives_up_after_its_bound_and_reports_every_line_it_did_not_write()

        /// <summary>
        /// A file whose write never ends - a disk or a share that no longer
        /// answers - holds the shutdown for DisposeTimeout and no longer, and
        /// every line that is not in the file is said to be not in it.
        /// </summary>
        [Test]
        public async Task DisposeAsync_gives_up_after_its_bound_and_reports_every_line_it_did_not_write()
        {

            var file     = Path.Combine(directory, "unanswered.log");
            var reports  = new ConcurrentQueue<(String FileName, String Line, Exception Exception)>();
            var begun    = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            var writer   = new LogFileWriter(
                               1000,
                               TimeSpan.FromMilliseconds(200),
                               async (fileName, text, cancellationToken) => {
                                   begun.TrySetResult();
                                   await Task.Delay(Timeout.Infinite, cancellationToken);
                               }
                           );

            writer.OnLineNotWritten += (timestamp, sender, fileName, line, exception) => reports.Enqueue((fileName, line, exception));

            foreach (var line in Lines("line", 3))
                await writer.EnqueueAsync(file, line);

            await begun.Task.WaitAsync(StepTimeout);

            await writer.DisposeAsync().AsTask().WaitAsync(StepTimeout);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(reports.Select(report => report.Line),              Is.EqualTo(Lines("line", 3)));
                Assert.That(reports.Select(report => report.FileName),          Is.All.EqualTo(file));
                Assert.That(reports.First().Exception,                          Is.InstanceOf<OperationCanceledException>(),
                            "The line whose write was cut off.");
                Assert.That(reports.Skip(1).Select(report => report.Exception), Is.All.InstanceOf<TimeoutException>(),
                            "The lines whose turn never came.");
            }

        }

        #endregion

        #region DisposeAsync_returns_once_the_lines_are_written_not_when_its_bound_runs_out()

        [Test]
        public async Task DisposeAsync_returns_once_the_lines_are_written_not_when_its_bound_runs_out()
        {

            var file    = Path.Combine(directory, "prompt.log");
            var writer  = new LogFileWriter(1000, TimeSpan.FromHours(1));

            await writer.EnqueueAsync(file, "the only line");

            await writer.DisposeAsync().AsTask().WaitAsync(StepTimeout);

            Assert.That(LinesIn(file), Is.EqualTo(new[] { "the only line" }));

        }

        #endregion

        #region A_subscriber_that_throws_costs_neither_the_other_subscribers_nor_the_lines_after_it()

        [Test]
        public async Task A_subscriber_that_throws_costs_neither_the_other_subscribers_nor_the_lines_after_it()
        {

            var written  = Path.Combine(directory, "written.log");
            var refused  = Path.Combine(directory, "no such directory", "refused.log");
            var reports  = new ConcurrentQueue<String>();
            var writer   = new LogFileWriter();

            writer.OnLineNotWritten += (timestamp, sender, fileName, line, exception) => throw new InvalidOperationException("A subscriber's own mistake.");
            writer.OnLineNotWritten += (timestamp, sender, fileName, line, exception) => reports.Enqueue(line);

            await writer.EnqueueAsync(refused, "refused 1");
            await writer.EnqueueAsync(written, "written");
            await writer.EnqueueAsync(refused, "refused 2");

            await writer.DisposeAsync().AsTask().WaitAsync(StepTimeout);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(LinesIn(written),  Is.EqualTo(new[] { "written" }));
                Assert.That(reports,           Is.EqualTo(new[] { "refused 1", "refused 2" }));
            }

        }

        #endregion

    }

}
