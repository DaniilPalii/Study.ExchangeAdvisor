using System;
using System.Collections.Generic;
using ExchangeAdvisor.Domain.Values;
using Newtonsoft.Json;

namespace ExchangeAdvisor.Domain.Services.Implementation.Web.Response
{
    public class LatestRateResponse
    {
        [JsonProperty(propertyName: "date")]
        public DateTime? Date { get; set; }

        [JsonProperty(propertyName: "base")]
        public Currency? BaseCurrency { get; set; }
        
        [JsonProperty(propertyName: "rates")]
        public IDictionary<Currency, float> Rates { get; set; }
    }
}