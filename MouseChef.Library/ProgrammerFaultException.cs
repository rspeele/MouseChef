using System;

namespace MouseChef
{
    /// <summary>
    /// Represents an exception that will only be thrown if the author of the software made a logic mistake.
    /// </summary>
    public class ProgrammerFaultException : Exception
    {
        public ProgrammerFaultException(string message) : base(message)
        {
        }

        public ProgrammerFaultException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
