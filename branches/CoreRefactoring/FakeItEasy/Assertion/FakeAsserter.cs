namespace FakeItEasy.Assertion
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FakeItEasy.Api;

    internal class FakeAsserter
    {
        private IEnumerable<IFakeObjectCall> calls;

        public delegate FakeAsserter Factory(IEnumerable<IFakeObjectCall> calls);

        public FakeAsserter(IEnumerable<IFakeObjectCall> calls)
        {
            this.calls = calls;
        }

        public virtual void AssertWasCalled(Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription)
        {
            Guard.IsNotNull(callPredicate, "callPredicate");
            Guard.IsNotNull(repeatPredicate, "repeatPredicate");
            Guard.IsNotNull(callDescription, "callDescription");
            Guard.IsNotNull(repeatDescription, "repeatDescription");

            var repeat = this.calls.Count(callPredicate);

            if (!repeatPredicate(repeat))
            {
                var message = CreateExceptionMessage(callDescription, repeatDescription, repeat);

                throw new ExpectationException(message);
            }
        }

        private string CreateExceptionMessage(string callDescription, string repeatDescription, int repeat)
        {
            var messageWriter = new StringWriter();

            messageWriter.WriteLine();
            AppendCallDescription(callDescription, messageWriter);
            this.AppendExpectation(repeatDescription, repeat, messageWriter);
            this.AppendCallList(messageWriter);
            messageWriter.WriteLine();

            return messageWriter.GetStringBuilder().ToString();
        }

        private static void AppendCallDescription(string callDescription, StringWriter writer)
        {
            writer.WriteLine("  Assertion failed for the following call:");
            writer.WriteLine("    '{0}'", callDescription);
        }

        private void AppendExpectation(string repeatDescription, int repeat, StringWriter writer)
        {
            writer.Write("  Expected to find it {0} ", repeatDescription);

            if (this.calls.Any())
            {
                writer.WriteLine("but found it #{0} times among the calls:", repeat);
            }
            else
            {
                writer.WriteLine("but no calls were made to the fake object.");
            }
        }

        private void AppendCallList(StringWriter writer)
        {
            var callDescriptions = new Queue<string>(this.calls.Select(x => x.ToString()));

            int callNumber = 0;
            int lineNumber = 1;
            while (callDescriptions.Count > 0 && lineNumber < 20)
            {
                var description = callDescriptions.Dequeue();
                callNumber++;
                WriteCallLine(writer, callNumber, description);
                
                int repeatOfCurrentCall = 1;

                while (callDescriptions.Count > 0 && string.Equals(description, callDescriptions.Peek(), StringComparison.Ordinal))
                {
                    repeatOfCurrentCall++;
                    callDescriptions.Dequeue();
                    callNumber++;
                }

                if (repeatOfCurrentCall > 1)
                {
                    writer.WriteLine(" repeated {0} times", repeatOfCurrentCall);
                    writer.WriteLine("    ...");
                }
                else
                {
                    writer.WriteLine();
                }

                lineNumber++;
            }

            var nonPrintedCalls = callDescriptions.Count;
            if (nonPrintedCalls > 0)
            {
                writer.WriteLine("    ... Found {0} more calls not displayed here.", nonPrintedCalls);
            }
        }

        private static void WriteCallLine(StringWriter writer, int callNumber, string callDescription)
        {
            writer.Write("    ");

            WriteCallNumber(writer, callNumber);

            writer.Write("'");
            writer.Write(callDescription);
            writer.Write("'");
        }

        private static void WriteCallNumber(StringWriter writer, int callNumber)
        {
            writer.Write(callNumber.ToString());
            writer.Write(". ");

            if (callNumber < 10)
            {
                writer.Write(" ");
            }
        }
    }
}
