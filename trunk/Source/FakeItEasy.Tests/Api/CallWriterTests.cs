using FakeItEasy.Api;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using System.Linq;

namespace FakeItEasy.Tests.Api
{
    [TestFixture]
    public class CallWriterTests
    {
        private List<IFakeObjectCall> calls;
        private StringWriter writer;

        [SetUp]
        public void SetUp()
        {
            this.calls = new List<IFakeObjectCall>();
            this.writer = new StringWriter();
        }

        private CallWriter CreateWriter()
        {
            return new CallWriter();
        }

        private void StubCalls(int numberOfCalls)
        {
            for (int i = 0; i < numberOfCalls; i++)
            {
                this.calls.Add(A.Fake<IFakeObjectCall>());
            }
        }

        [Test]
        public void WriteCalls_should_list_the_calls_in_the_calls_collection()
        {
            this.StubCalls(10);

            int callNumber = 1;
            foreach (var call in this.calls)
            {
                var boundCallNumber = callNumber;
                Configure.Fake(call)
                    .CallsTo(x => x.ToString())
                    .Returns(x => "Fake call " + boundCallNumber.ToString());
                callNumber++;
            }

            var writer = this.CreateWriter();
            writer.WriteCalls(0, this.calls, this.writer);

            var message = this.writer.GetStringBuilder().ToString();
            
            Assert.That(message, Text.Contains(@"1.  'Fake call 1'
2.  'Fake call 2'
3.  'Fake call 3'
4.  'Fake call 4'
5.  'Fake call 5'
6.  'Fake call 6'
7.  'Fake call 7'
8.  'Fake call 8'
9.  'Fake call 9'
10. 'Fake call 10'"));
        }

        [Test]
        public void WriteCalls_should_skip_duplicate_calls_in_row()
        {
            this.StubCalls(10);

            foreach (var call in this.calls)
            {
                A.CallTo(() => call.ToString()).Returns("Fake call");
            }

            A.CallTo(() => this.calls[9].ToString()).Returns("Other call");

            var writer = this.CreateWriter();
            writer.WriteCalls(0, this.calls, this.writer);

            var message = this.writer.GetStringBuilder().ToString();

            Assert.That(message, Text.Contains(@"1.  'Fake call' repeated 9 times
...
10. 'Other call'"));
        }

        [Test]
        public void WriteCalls_should_not_skip_duplicate_messages_that_are_not_in_row()
        {
            this.StubCalls(4);

            foreach (var call in this.calls.Where((x, i) => i % 2 == 0))
            {
                A.CallTo(() => call.ToString()).Returns("odd");
            }

            foreach (var call in this.calls.Where((x, i) => i % 2 != 0))
            {
                A.CallTo(() => call.ToString()).Returns("even");
            }

            var writer = this.CreateWriter();
            writer.WriteCalls(0, this.calls, this.writer);
            
            var message =this.writer.GetStringBuilder().ToString();

            Assert.That(message, Text.Contains(@"1.  'odd'
2.  'even'
3.  'odd'
4.  'even'
"));
        }

        [Test]
        public void WriteCalls_should_truncate_calls_list_when_more_than_a_hundred_call_lines_are_printed()
        {
            this.StubCalls(30);

            foreach (var call in this.calls)
            {
                A.CallTo(() => call.ToString()).Returns(x => "Fake call {0}".FormatInvariant(Guid.NewGuid()));
            }

            A.CallTo(() => this.calls[18].ToString()).Returns("Last call");

            var writer = this.CreateWriter();
            writer.WriteCalls(0, this.calls, this.writer);

            var message = this.writer.GetStringBuilder().ToString();

            Assert.That(message, Text.Contains(@"19. 'Last call'
... Found 11 more calls not displayed here."));
        }

        [Test]
        public void WriteCalls_should_indent_calls_by_specified_number_of_characters()
        {
            this.StubCalls(10);

            int callNumber = 1;
            foreach (var call in this.calls)
            {
                var boundCallNumber = callNumber;
                A.CallTo(() => call.ToString()).Returns(x => "Fake call " + boundCallNumber.ToString());
                callNumber++;
            }

            var writer = this.CreateWriter();
            writer.WriteCalls(4, this.calls, this.writer);

            var message = this.writer.GetStringBuilder().ToString();

            Assert.That(message, Text.Contains(@"    1.  'Fake call 1'
    2.  'Fake call 2'
    3.  'Fake call 3'
    4.  'Fake call 4'
    5.  'Fake call 5'
    6.  'Fake call 6'
    7.  'Fake call 7'
    8.  'Fake call 8'
    9.  'Fake call 9'
    10. 'Fake call 10'"));
        }
    }
}
