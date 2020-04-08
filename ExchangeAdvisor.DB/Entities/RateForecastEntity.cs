using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.DB.Entities
{
    public class RateForecastEntity : EntityBase
    {
        public ICollection<ForecastRateEntity> Rates { get; set; }

        [Column(TypeName = ColumnTypeName.Currency)]
        public Currency BaseCurrency { get; set; }

        [Column(TypeName = ColumnTypeName.Currency)]
        public Currency ComparingCurrency { get; set; }

        public DateTime CreationDay { get; set; }

        public string Description { get; set; }

        public RateForecastEntity() { }

        public RateForecastEntity(RateForecast forecast)
        {
            Rates = forecast.Rates.Select(r => new ForecastRateEntity(r)).ToArray();
            BaseCurrency = forecast.CurrencyPair.Base;
            ComparingCurrency = forecast.CurrencyPair.Comparing;
            CreationDay = forecast.CreationDay;
            Description = forecast.Description;
        }

        public RateForecast ToRateForecast()
        {
            var rates = Rates.Select(r => r.ToRate());
            var metadata = ToRateForecastMetadata();

            return new RateForecast(rates, metadata);
        }

        public RateForecastMetadata ToRateForecastMetadata()
        {
            var currencyPair = new CurrencyPair(BaseCurrency, ComparingCurrency);

            return new RateForecastMetadata(CreationDay, currencyPair, Description);
        }
    }
}