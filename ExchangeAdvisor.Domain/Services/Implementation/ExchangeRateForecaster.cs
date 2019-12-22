using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeAdvisor.Domain.Values;
using MathNet.Numerics.Interpolation;

namespace ExchangeAdvisor.Domain.Services.Implementation
{
    public class ExchangeRateForecaster : IExchangeRateForecaster
    {
        public IEnumerable<RateOnDay> Forecast(IReadOnlyCollection<RateOnDay> source, DateTime forecastFinishDay)
        {
            return Forecast(
                source,
                forecastFinishDay,
                ForecastMethod.BarycentricInterpolatePolynomialEquidistantSorted);
        }

        public IEnumerable<RateOnDay> Forecast(
            IReadOnlyCollection<RateOnDay> source, 
            DateTime forecastFinishDay,
            ForecastMethod forecastMethod)
        {
            if (source.Count < 2)
                throw new ArgumentException("Source must has 2 or more values");

            var lastSourceDay = source.Last().Day;
            if (lastSourceDay.Date >= forecastFinishDay.Date)
                throw new ArgumentException("Prediction finish day must be later than last source day");
            
            var createInterpolation = GetInterpolationCreationFunction(forecastMethod);
            var interpolation = createInterpolation(
                source.Select(r => ToDayNumber(r.Day)).ToArray(),
                source.Select(r => r.Rate).ToArray());
            var lastSourceDayNumber = ToDayNumber(lastSourceDay);

            return Enumerable.Range(
                    start: Convert.ToInt32(lastSourceDayNumber) + 1,
                    count: Convert.ToInt32(ToDayNumber(forecastFinishDay) - lastSourceDayNumber))
                .Select(n => (dayNumber: n, day: ToDay(n)))
                .Where(d => IsNotWeekend(d.day))
                .Select(d => new RateOnDay
                {
                    Day = d.day,
                    Rate = interpolation.Interpolate(d.dayNumber)
                });
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
//                case ForecastMethod.PolynomialInterpolationAlgorithmGenerateSamplePoints:
//                    return PolynomialInterpolationAlgorithm.GenerateSamplePoints;
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
        CubicSplineInterpolateNaturalSorted,
//        PolynomialInterpolationAlgorithmGenerateSamplePoints,
    }
}