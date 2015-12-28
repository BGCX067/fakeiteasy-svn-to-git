using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Api;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void Creating_fakes_will_use_the_container_of_the_current_scope()
        {
            var container = new DictionaryContainer
            {
                RegisteredTypes = new Dictionary<Type, object>
                {
                    { typeof(string), "fake string" },
                    { typeof(int), 100 }
                }
            };

            using (Fake.CreateScope(container))
            {
                Assert.That(A.Fake<string>(), Is.EqualTo("fake string"));
                Assert.That(A.Fake<int>(), Is.EqualTo(100));
            }
        }

        private class DictionaryContainer
            : IFakeObjectContainer
        {
            public IDictionary<Type, object> RegisteredTypes;

            public bool TryCreateFakeObject(Type typeOfFakeObject, IEnumerable<object> arguments, out object fakeObject)
            {
                return this.RegisteredTypes.TryGetValue(typeOfFakeObject, out fakeObject);
            }
        }
        
    }
}
