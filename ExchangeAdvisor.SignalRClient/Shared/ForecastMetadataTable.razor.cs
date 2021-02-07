using System;
using System.Collections.Generic;
using ExchangeAdvisor.SignalRClient.ViewModels;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ExchangeAdvisor.SignalRClient.Shared
{
    public partial class ForecastMetadataTable : ComponentBase
    {
        [Parameter]
        public IReadOnlyCollection<ForecastMetadataTableRowViewModel> Data { get; set; }

        [Parameter]
        public EventCallback<IReadOnlyCollection<ForecastMetadataTableRowViewModel>> DataChanged { get; set; }

        [Parameter]
        public EventCallback SelectionChanged { get; set; }

        public void RowSelectHandler(ForecastMetadataTableRowViewModel obj)
        {
            Console.WriteLine("123");
        }
    }
}