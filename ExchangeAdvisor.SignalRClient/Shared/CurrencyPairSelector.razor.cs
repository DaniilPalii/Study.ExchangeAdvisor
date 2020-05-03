using System;
using System.Collections.Generic;
using ExchangeAdvisor.Domain.Extensions;
using ExchangeAdvisor.Domain.Values;
using Microsoft.AspNetCore.Components;

namespace ExchangeAdvisor.SignalRClient.Shared
{
    public partial class CurrencyPairSelector : ComponentBase
    {
        [Parameter]
        public CurrencyPair Value
        {
            get
            {
                var baseCurrency = CurrencyExtensions.ToCurrency(BaseCurrencyName);
                var comparingCurrency = CurrencyExtensions.ToCurrency(ComparingCurrencyName);

                return new CurrencyPair(baseCurrency, comparingCurrency);
            }
            set
            {
                BaseCurrencyName = value.Base.ToString();
                ComparingCurrencyName = value.Comparing.ToString();
            }
        }

        [Parameter]
        public EventCallback<CurrencyPair> ValueChanged { get; set; }

        private void HandleDropDownValueChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string> args)
        {
            ValueChanged.InvokeAsync(Value);
        }

        private string BaseCurrencyName { get; set; }
        
        private string ComparingCurrencyName { get; set; }

        private static readonly IReadOnlyCollection<string> CurrencyNames = Enum.GetNames(typeof(Currency));
    }
}