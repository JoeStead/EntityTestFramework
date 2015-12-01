using System;

namespace EntityTestFramework.Exceptions
{
    internal class EntitiesSavedException : Exception
    {
        public EntitiesSavedException(string message) : base(message)
        {
        }
    }
}
