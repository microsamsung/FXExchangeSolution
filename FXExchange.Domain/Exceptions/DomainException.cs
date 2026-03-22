using System.Runtime.Serialization;

namespace FXExchange.Domain.Exceptions
{
    [Serializable]
    public class DomainException : Exception
    {
        public string ErrorCode { get; }

        public DomainException()
        {
        }

        public DomainException(string message)
            : base(message)
        {
        }

        public DomainException(
            string message,
            string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public DomainException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }

        public DomainException(
            string message,
            string errorCode,
            Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        protected DomainException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            ErrorCode =
                info.GetString(nameof(ErrorCode));
        }
       
    }
}