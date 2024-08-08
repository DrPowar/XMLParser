using System.Runtime.Serialization;

namespace ConsoleApp2.Exceptions
{
    [Serializable]
    internal class InvalidXMLException : Exception
    {
        public InvalidXMLException() : base("The tag provided is invalid.")
        {
        }

        public InvalidXMLException(string? message) : base(message)
        {
        }

        public InvalidXMLException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidXMLException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
