using System.Collections.Generic;
using ExchangeAdvisor.Domain.Values.Rate;
using Microsoft.AspNetCore.Components;

namespace ExchangeAdvisor.SignalRClient.Shared
{
    public partial class ForecastMetadataTable : ComponentBase
    {
        [Parameter]
        public ICollection<RateForecastMetadata> Data { get; set; }

        [Parameter]
        public EventCallback<ICollection<RateForecastMetadata>> DataChanged { get; set; }
    }
}