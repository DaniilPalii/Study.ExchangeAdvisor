using System;
using System.Collections.Generic;
using ExchangeAdvisor.Domain.Values;
using Microsoft.ML.Data;

namespace ExchangeAdvisor.ML.Internal
{
    internal class ModelInput
    {
        [ColumnName("Year"), LoadColumn(0)]
        public float Year { get; set; }

        [ColumnName("Month"), LoadColumn(1)]
        public float Month { get; set; }

        [ColumnName("Day"), LoadColumn(2)]
        public float Day { get; set; }

        [ColumnName("Absolute day number"), LoadColumn(3)]
        public float AbsoluteDayNumber { get; set; }

        [ColumnName("Day of week"), LoadColumn(4)]
        public float DayOfWeek { get; set; }

        [ColumnName("Day of year"), LoadColumn(5)]
        public float DayOfYear { get; set; }

        [ColumnName("Rate"), LoadColumn(6)]
        public float Rate { get; set; }

        [ColumnName("Base currency"), LoadColumn(7)]
        public string BaseCurrency { get; set; }

        [ColumnName("Comparing currency"), LoadColumn(8)]
        public string ComparingCurrency { get; set; }

        public ModelInput(DateTime rateDay, CurrencyPair currencyPair)
        {
            Year = rateDay.Year;
            Month = rateDay.Month;
            Day = rateDay.Day;
            AbsoluteDayNumber = GetAbsoluteDayNumber(rateDay);
            DayOfWeek = GetDayOfWeekNumber(rateDay);
            DayOfYear = rateDay.DayOfYear;
            BaseCurrency = currencyPair.Base.ToString();
            ComparingCurrency = currencyPair.Comparing.ToString();
        }

        public ModelInput(DateTime rateDay, CurrencyPair currencyPair, float rate)
            : this(rateDay, currencyPair)
        {
            Rate = rate;
        }

        public static int GetDayOfWeekNumber(DateTime day)
        {
            return DayOfWeekNumbers[day.DayOfWeek];
        }

        public static float GetAbsoluteDayNumber(DateTime rateDay)
        {
            return (float)(rateDay - DateTime.MinValue).TotalDays;
        }

        private static readonly IReadOnlyDictionary<DayOfWeek, int> DayOfWeekNumbers
            = new Dictionary<DayOfWeek, int>
            {
                { System.DayOfWeek.Monday, 1 },
                { System.DayOfWeek.Tuesday, 2 },
                { System.DayOfWeek.Wednesday, 3 },
                { System.DayOfWeek.Thursday, 4 },
                { System.DayOfWeek.Friday, 5 },
                { System.DayOfWeek.Saturday, 6 },
                { System.DayOfWeek.Sunday, 7 }
            };
    }
}
