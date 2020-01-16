using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeAdvisor.Domain.Values;
using MathNet.Numerics.Interpolation;

namespace ExchangeAdvisor.Domain.Services.Implementation
{
    public class ExtrapolationRateForecaster : IExtrapolationRateForecaster
    {
        public IEnumerable<Rate> Forecast(
            IReadOnlyCollection<Rate> source, 
            DateTime forecastFinishDay,
            ForecastMethod forecastMethod)
        {
            if (source.Count < 2)
                throw new ArgumentException("Source must has 2 or more values");

            var lastSourceValue = source.Last();
            var lastSourceDay = lastSourceValue.Day;
            if (lastSourceDay.Date >= forecastFinishDay.Date)
                throw new ArgumentException("Prediction finish day must be later than last source day");
            
            var createInterpolation = GetInterpolationCreationFunction(forecastMethod);
            var interpolation = createInterpolation(
                source.Select(r => ToDayNumber(r.Day)).ToArray(),
                source.Select(r => r.Value).ToArray());
            var lastSourceDayNumber = ToDayNumber(lastSourceDay);

            return Enumerable.Range(
                    start: Convert.ToInt32(lastSourceDayNumber) + 1,
                    count: Convert.ToInt32(ToDayNumber(forecastFinishDay) - lastSourceDayNumber))
                .Select(n => (dayNumber: n, day: ToDay(n)))
                .Where(d => IsNotWeekend(d.day))
                .Select(d => new Rate(
                    d.day,
                    value: interpolation.Interpolate(d.dayNumber),
                    lastSourceValue.BaseCurrency,
                    lastSourceValue.ComparingCurrency));
        }

        public IEnumerable<Rate> ForecastOnKnownAndUnknownRange(
            IReadOnlyCollection<Rate> source, 
            DateTime forecastFinishDay,
            ForecastMethod forecastMethod)
        {
            if (source.Count < 2)
                throw new ArgumentException("Source must has 2 or more values");

            var createInterpolation = GetInterpolationCreationFunction(forecastMethod);
            var interpolation = createInterpolation(
                source.Select(r => ToDayNumber(r.Day)).ToArray(),
                source.Select(r => r.Value).ToArray());
            var firstSourceDayNumber = ToDayNumber(source.First().Day);
            var lastSourceValue = source.Last();

            return Enumerable.Range(
                    start: Convert.ToInt32(firstSourceDayNumber),
                    count: Convert.ToInt32(ToDayNumber(forecastFinishDay) - firstSourceDayNumber))
                .Select(n => (dayNumber: n, day: ToDay(n)))
                .Select(d => new Rate(
                    d.day,
                    value: interpolation.Interpolate(d.dayNumber),
                    lastSourceValue.BaseCurrency,
                    lastSourceValue.ComparingCurrency));
        }

        private static Func<double[], double[], IInterpolation> GetInterpolationCreationFunction(ForecastMethod forecastMethod)
        {
            switch (forecastMethod)
            {
                case ForecastMethod.BarycentricInterpolatePolynomialEquidistantSorted:
                    return Barycentric.InterpolatePolynomialEquidistantSorted;
                case ForecastMethod.BarycentricInterpolateRationalFloaterHormannSorted:
                    return Barycentric.InterpolateRationalFloaterHormannSorted;
                case ForecastMethod.BulirschStoerRationalInterpolationInterpolateSorted:
                    return BulirschStoerRationalInterpolation.InterpolateSorted;
                case ForecastMethod.CubicSplineInterpolateAkimaSorted:
                    return CubicSpline.InterpolateAkimaSorted;
                case ForecastMethod.CubicSplineInterpolateNaturalSorted:
                    return CubicSpline.InterpolateNaturalSorted;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static double ToDayNumber(DateTime dateTime)
        {
            return (dateTime - DateTime.MinValue).TotalDays;
        }

        private static DateTime ToDay(double dayNumber)
        {
            return DateTime.MinValue.AddDays(dayNumber);
        }

        private static bool IsNotWeekend(DateTime day)
        {
            return day.DayOfWeek != DayOfWeek.Sunday 
                && day.DayOfWeek != DayOfWeek.Saturday;
        }
    }

    public enum ForecastMethod
    {
        BarycentricInterpolatePolynomialEquidistantSorted,
        BarycentricInterpolateRationalFloaterHormannSorted,
        BulirschStoerRationalInterpolationInterpolateSorted,
        CubicSplineInterpolateAkimaSorted,
        CubicSplineInterpolateNaturalSorted
    }
}