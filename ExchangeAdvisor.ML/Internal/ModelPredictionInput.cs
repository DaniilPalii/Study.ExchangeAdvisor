using System;
using System.Collections.Generic;

namespace ExchangeAdvisor.ML.Internal
{
    internal class ModelPredictionInput
    {
        public float Year { get; set; }

        public float Month { get; set; }

        public float Day { get; set; }

        public float AbsoluteDayNumber { get; set; }

        public float DayOfWeek { get; set; }

        public float DayOfYear { get; set; }

        public static string[] InputFeatureNames => new[]
        {
            nameof(Year),
            nameof(Month),
            nameof(Day),
            nameof(AbsoluteDayNumber),
            nameof(DayOfWeek),
            nameof(DayOfYear)
        };

        public ModelPredictionInput(DateTime rateDay)
        {
            Year = rateDay.Year;
            Month = rateDay.Month;
            Day = rateDay.Day;
            AbsoluteDayNumber = GetAbsoluteDayNumber(rateDay);
            DayOfWeek = GetDayOfWeekNumber(rateDay);
            DayOfYear = rateDay.DayOfYear;
        }

        public static int GetDayOfWeekNumber(DateTime day) => DayOfWeekNumbers[day.DayOfWeek];

        public static float GetAbsoluteDayNumber(DateTime rateDay) => (float)(rateDay - DateTime.MinValue).TotalDays;

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
