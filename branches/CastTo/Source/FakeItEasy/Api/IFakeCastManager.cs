using System;
namespace FakeItEasy.Api
{
    public interface IFakeCastManager
    {
        void CastTo(Type interfaceType, object fakedObject);
    }
}
