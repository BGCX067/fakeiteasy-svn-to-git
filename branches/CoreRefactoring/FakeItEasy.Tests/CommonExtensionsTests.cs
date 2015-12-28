using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class CommonExtensionsTests
    {
        [Test]
        public void Pairwise_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                CommonExtensions.Pairwise(new List<string>(), new List<string>()));
        }

        [Test]
        public void Pairwise_returns_an_enumeral_of_tuples_paired_in_order()
        {
            var strings = new List<string>();
            var ints = new List<int>();

            var result = strings.Pairwise(ints);
        }

        [Test]
        public void Pairwise_should_throw_when_the_number_of_elements_differ_in_the_collections_when_enumerated()
        {
            var strings = new List<string> { "a", "b", "c" };
            var ints = new List<int> { 1, 2 };

            Assert.Throws<InvalidOperationException>(() =>
                {
                    strings.Pairwise(ints).ToList();
                });
        }
    }
}
