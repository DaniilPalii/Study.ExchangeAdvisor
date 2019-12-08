using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeAdvisor.Domain.Values;
using MathNet.Numerics.Interpolation;

namespace ExchangeAdvisor.Domain.Services.Implementation
{
    public class ExchangeRateForecaster : IExchangeRateForecaster
    {
        public IEnumerable<RateOnDay> Forecast(
            IReadOnlyCollection<RateOnDay> source,
            DateTime predictionFinishDay)
        {
            if (source.Count < 2)
                throw new ArgumentException("Source must has 2 or more values");

            var lastSourceDay = source.Last().Day.Date;
            if (lastSourceDay >= predictionFinishDay.Date)
                throw new ArgumentException("Prediction finish day must be later than last source day");

            var interpolation = Barycentric.InterpolatePolynomialEquidistantSorted(
                source.Select(r => ToDayNumber(r.Day)).ToArray(),
                source.Select(r => r.Rate).ToArray());
            var lastSourceDayNumber = ToDayNumber(lastSourceDay);
            var predictionFinishDayNumber = ToDayNumber(predictionFinishDay);

            return Enumerable.Range(
                    Convert.ToInt32(lastSourceDayNumber) + 1,
                    Convert.ToInt32(predictionFinishDayNumber - lastSourceDayNumber))
                .Select(dayNumber => new RateOnDay
                {
                    Day = ToDay(dayNumber),
                    Rate = interpolation.Interpolate(dayNumber)
                });
        }

        private static double ToDayNumber(DateTime dateTime)
        {
            return (dateTime - DateTime.MinValue).TotalDays;
        }

        private static DateTime ToDay(double dayNumber)
        {
            return DateTime.MinValue.AddDays(dayNumber);
        }
    }
}