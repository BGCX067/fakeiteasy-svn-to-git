namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Api;

    /// <summary>
    /// Responsible for creating fake objects.
    /// </summary>
    internal class FakeObjectFactory
    {
        public delegate FakeObjectFactory Creator();
        private readonly IFakeObjectContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeObjectFactory"/> class.
        /// </summary>
        /// <param name="container">The container to use.</param>
        public FakeObjectFactory(IFakeObjectContainer container)
        {
            this.container = container;    
        }

        /// <summary>
        /// Creates a fake object of the specified type.
        /// </summary>
        /// <param name="typeOfFake">The type of fake object to create.</param>
        /// <param name="argumentsForConstructor">Arguments for the constructor of the faked type, 
        /// null if no arguments should be passed.</param>
        /// <returns>A fake object.</returns>
        public virtual object CreateFake(Type typeOfFake, IEnumerable<object> argumentsForConstructor, bool allowNonProxiedFakes)
        {
            Guard.IsNotNull(typeOfFake, "typeOfFake");

            object result = null;

            if (!allowNonProxiedFakes || !this.container.TryCreateFakeObject(typeOfFake, null, out result))
            {
                if (argumentsForConstructor == null)
                {
                    result = new FakeObject(typeOfFake).Object;
                }
                else
                {
                    result = new FakeObject(typeOfFake, argumentsForConstructor.ToArray()).Object;
                }
            }
            
            return result;
        }
    }
}
