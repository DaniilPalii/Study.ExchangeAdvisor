using Newtonsoft.Json;

namespace ExchangeAdvisor.Domain.Services.Implementation.Web.Response
{
    public class ErrorResponse
    {
        [JsonProperty(propertyName: "error")]
        public string Message { get; set; }
    }
}