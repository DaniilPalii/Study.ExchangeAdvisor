﻿using System;
using System.Collections.Generic;

namespace ExchangeAdvisor.Domain.Values.Rate
{
    public class RateForecast : RateCollectionBase
    {
        public RateForecastMetadata Metadata { get; }

        public override CurrencyPair CurrencyPair => Metadata.CurrencyPair;

        public DateTime CreationDay => Metadata.CreationDay;

        public string Description
        {
            get => Metadata.Description;
            set => Metadata.Description = value;
        }

        public RateForecast(
            IEnumerable<Values.Rate.Rate> rates,
            CurrencyPair currencyPair,
            DateTime creationDay,
            string description)
                : this(rates, currencyPair, creationDay)
        {
            Description = description;
        }
        
        public RateForecast(IEnumerable<Values.Rate.Rate> rates, CurrencyPair currencyPair, DateTime creationDay)
            : this(rates, new RateForecastMetadata(creationDay, currencyPair))
        { }

        public RateForecast(IEnumerable<Values.Rate.Rate> rates, RateForecastMetadata metadata) : base(rates)
        {
            Metadata = metadata;
        }
    }
}
