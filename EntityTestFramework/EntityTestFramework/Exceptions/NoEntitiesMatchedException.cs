using System;

namespace EntityTestFramework.Exceptions
{
    internal class NoEntitiesMatchedException : Exception
    {
        public NoEntitiesMatchedException(string message) : base(message)
        {
        }
    }
}
