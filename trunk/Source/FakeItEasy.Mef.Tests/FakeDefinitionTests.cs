﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;

namespace FakeItEasy.Mef
{
    [TestFixture]
    public class FakeDefinitionTests
    {
        [Test]
        public void ForType_should_return_the_generic_type_parameter_type()
        {
            var definition = new TestableFakeDefinition();

            Assert.That(definition.ForType, Is.EqualTo(typeof(SomeType)));
        }

        [Test]
        public void CreateFake_should_return_object_from_protected_function()
        {
            var definition = new TestableFakeDefinition() as IFakeDefinition;
            var created = definition.CreateFake();

            Assert.That(created, Is.InstanceOf<SomeType>());
        }

        public class SomeType
        {
        
        }

        public class TestableFakeDefinition : FakeDefinition<SomeType>
        {
            protected override SomeType CreateFake()
            {
                return new SomeType();
            }
        }
    }
}
