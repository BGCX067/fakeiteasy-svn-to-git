using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legend.Fakes.Configuration;
using Legend.Fakes.Api;
using System.Reflection;

namespace Legend.Fakes.VisualBasic
{
    /// <summary>
    /// Lets you specify options for the next call to a fake object.
    /// </summary>
    public static class ThisCall
    {
        /// <summary>
        /// Specifies options for the next call to the specified fake object. The next call will
        /// be recorded as a call configuration.
        /// </summary>
        /// <typeparam name="TFake">The type of the faked object.</typeparam>
        /// <param name="fake">The faked object to configure.</param>
        /// <returns>A call configuration object.</returns>
        public static IVisualBasicConfiguration<TFake> To<TFake>(TFake fake) where TFake : class
        {
            Guard.IsNotNull(fake, "fake");

            var fakeObject = Fake.GetFakeObject(fake);
            var recordingRule = new RecordingCallRule(fakeObject);
            fakeObject.AddRule(recordingRule);

            return new FakeConfiguration<TFake>(fakeObject, recordingRule.RecordedRule);
        }

        /// <summary>
        /// A call rule that "sits and waits" for the next call, when
        /// that call occurs the recorded rule is added for that call.
        /// </summary>
        private class RecordingCallRule
            : IFakeObjectCallRule
        {
            private FakeObject fakeObject;
            public RecordedCallRule RecordedRule;

            public RecordingCallRule(FakeObject fakeObject)
            {
                this.fakeObject = fakeObject;
                this.RecordedRule = new RecordedCallRule();
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return true;
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                this.RecordedRule.ApplicableToMethod = fakeObjectCall.Method;

                if (this.RecordedRule.IsApplicableToArguments == null)
                {
                    this.CreateArgumentsPredicateFromArguments(fakeObjectCall);
                }

                this.fakeObject.AddRule(this.RecordedRule);

                fakeObjectCall.SetReturnValue(Helpers.GetDefaultValueOfType(fakeObjectCall.Method.ReturnType));
            }

            private void CreateArgumentsPredicateFromArguments(IFakeObjectCall fakeObjectCall)
            {
                this.RecordedRule.IsApplicableToArguments = x => x.SequenceEqual(fakeObjectCall.Arguments);
            }

            public int? NumberOfTimesToCall
            {
                get { return 1; }
            }
        }

        
    }
}