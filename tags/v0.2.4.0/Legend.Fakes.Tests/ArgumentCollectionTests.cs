﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections;

namespace Legend.Fakes.Tests
{
    [TestFixture]
    public class ArgumentCollectionTests
    {
        [Test]
        public void Constructor_is_properly_guarded()
        {
            IsProperlyGuardedConstraint.IsProperlyGuarded(() =>
                new ArgumentCollection(new object[] { "foo", 1 }, new[] {"foo", "bar" }));
        }

        [Test]
        public void Constructor_that_takes_method_is_properly()
        {
            var method = typeof(IFoo).GetMethod("Bar", new[] { typeof(object), typeof(object) });
            IsProperlyGuardedConstraint.IsProperlyGuarded(() =>
                new ArgumentCollection(new object[] { "foo", 1 }, method));
        }

        [Test]
        public void Constructor_that_takes_method_should_set_argument_names()
        {
            var method = typeof(IFoo).GetMethod("Bar", new[] { typeof(object), typeof(object) });

            var arguments = new ArgumentCollection(new object[] { "foo", "bar" }, method);

            Assert.That(arguments.ArgumentNames.SequenceEqual(new string[] { "argument", "argument2" }));
        }

        [Test]
        public void Constructor_should_throw_when_number_of_arguments_does_not_match_number_of_argument_names()
        {
            Assert.Throws<ArgumentException>(() => 
                new ArgumentCollection(new object[] { 1, 2 }, new[] { "first", "second", "third" }));
        }

        [Test]
        public void Get_called_with_index_should_return_argument_typed()
        {
            var arguments = CreateFakeArgumentList("foo", "bar");

            Assert.That(arguments.Get<string>(1), Is.EqualTo("bar"));
        }

        [Test]
        public void Get_called_with_name_should_return_argument_at_position_of_name()
        {
            var arguments = CreateFakeArgumentList(new[] { "foo", "bar" }, 1, 2);

            Assert.That(arguments.Get<int>("bar"), Is.EqualTo(2));
        }

        [Test]
        public void Get_called_with_argument_name_that_does_not_exist_throws_exception()
        {
            var arguments = CreateFakeArgumentList(new[] { "foo", "bar" }, 1, 2);

            Assert.Throws<ArgumentException>(() =>
                arguments.Get<int>("unknown"));
        }

        [Test]
        public void Count_should_return_the_number_of_arguments_in_the_collection()
        {
            var arguments = CreateFakeArgumentList(new[] { "foo", "bar" }, 1, 2);

            Assert.That(arguments.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetEnumerator_through_non_generic_interface_returns_enumerator_that_enumerates_all_arguments()
        {
            var arguments = CreateFakeArgumentList(new[] { "foo", "bar" }, 1, 2);
            var found = new List<object>();

            var enumerator = ((IEnumerable)arguments).GetEnumerator();
            while (enumerator.MoveNext())
            {
                found.Add(enumerator.Current);
            }            

            Assert.That(found, Is.EquivalentTo(new object[] { 1, 2 }));
        }

        private ArgumentCollection CreateFakeArgumentList(string[] argumentNames, params object[] arguments)
        {
            return new ArgumentCollection(arguments, argumentNames);
        }

        private ArgumentCollection CreateFakeArgumentList(params object[] arguments)
        {
            return CreateFakeArgumentList(
                Enumerable.Range(0, arguments.Length).Select(x => x.ToString()).ToArray(),
                arguments);
        }
    }
}
