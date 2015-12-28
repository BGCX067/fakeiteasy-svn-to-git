using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Configuration;
using FakeItEasy.Api;
using System.Reflection;

namespace FakeItEasy.VisualBasic
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
        public static IVisualBasicArgumentsConfiguration<TFake> To<TFake>(TFake fake) where TFake : class
        {
            Guard.IsNotNull(fake, "fake");

            var recordedRule = CreateRecordedRule();
            var fakeObject = Fake.GetFakeObject(fake);
            var recordingRule = CreateRecordingRule<TFake>(recordedRule, fakeObject);

            fakeObject.AddRule(recordingRule);

            return new FakeConfiguration<TFake>(fakeObject, recordedRule);
        }

        private static RecordingCallRule<TFake> CreateRecordingRule<TFake>(RecordedCallRule recordedRule, FakeObject fakeObject) where TFake : class
        {
            var factory = ServiceLocator.Current.Resolve<IRecordingCallRuleFactory>();
            return factory.Create<TFake>(fakeObject, recordedRule);
        }

        private static RecordedCallRule CreateRecordedRule()
        {
            return ServiceLocator.Current.Resolve<RecordedCallRule.Factory>().Invoke();
        }
    }
}