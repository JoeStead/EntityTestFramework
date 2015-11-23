using System;

namespace EntityTestFramework.Exceptions
{
    internal class EntitiesMatchedException : Exception
    {
        public EntitiesMatchedException(string message) : base(message)
        {
        }
    }
}
