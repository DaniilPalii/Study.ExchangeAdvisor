using System;

namespace ExchangeAdvisor.Domain.Exceptions
{
    public class ExternalApiException : Exception
    {
        public string ApiName { get; }

        public string Operation { get; }

        public string Reason { get; }

        public ExternalApiException(string apiName, string operation, string reason)
            : base(ToExceptionMessage(apiName, operation, reason))
        {
            ApiName = apiName;
            Operation = operation;
            Reason = reason;
        }

        public ExternalApiException(string apiName, string operation, string reason, Exception innerException)
            : base(ToExceptionMessage(apiName, operation, reason), innerException)
        {
            ApiName = apiName;
            Operation = operation;
            Reason = reason;
        }

        private static string ToExceptionMessage(string apiName, string operation, string reason)
        {
            return $"Error occured in {apiName} while {operation} because {reason}";
        }
    }
}