using System;
using System.Linq;
using ExchangeAdvisor.Domain.Services.Implementation;
using ExchangeAdvisor.Domain.Values;
using NUnit.Framework;

namespace ExchangeAdvisor.Tests.UnitTests.DomainTests
{
    [TestFixture]
    public class RateForecasterTests
    {
        [SetUp]
        public void Setup()
        {
            rateForecaster = new ExtrapolationRateForecaster();
        }

        [Test, Theory]
        public void GivenSourceAndFinishDay_WhenForecast_ShouldAppendDaysUntilFinishDayIncluded(ForecastMethod forecastMethod)
        {
            var source = new[]
            {
                new Rate(new DateTime(2019, 1, 1), value: 1, CurrencySymbol.USD, CurrencySymbol.PLN), 
                new Rate(new DateTime(2019, 1, 2), value: 2, CurrencySymbol.USD, CurrencySymbol.PLN),
                new Rate(new DateTime(2019, 1, 3), value: 3, CurrencySymbol.USD, CurrencySymbol.PLN),
                new Rate(new DateTime(2019, 1, 4), value: 4, CurrencySymbol.USD, CurrencySymbol.PLN),
                new Rate(new DateTime(2019, 1, 5), value: 5, CurrencySymbol.USD, CurrencySymbol.PLN),
                new Rate(new DateTime(2019, 1, 6), value: 6, CurrencySymbol.USD, CurrencySymbol.PLN),
            };
            var forecastFinishDay = new DateTime(2019, 1, 8);
            
            var result = rateForecaster.Forecast(source, forecastFinishDay, forecastMethod).ToArray();
            
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result[0].Day, Is.EqualTo(new DateTime(2019, 1, 7)));
            Assert.That(result[0].Value, Is.EqualTo(7).Within(ForecastTolerance));
            Assert.That(result[1].Day, Is.EqualTo(new DateTime(2019, 1, 8)));
            Assert.That(result[1].Value, Is.EqualTo(8).Within(ForecastTolerance));
        }

        [Test]
        public void GivenSourceAndFinishDayNotGreaterThanLastSourceDay_WhenForecast_ShouldThrowArgumentException()
        {
            var source = new[]
            {
                new Rate(new DateTime(2019, 1, 1), value: 1, CurrencySymbol.USD, CurrencySymbol.PLN),
                new Rate(new DateTime(2019, 1, 2), value: 2, CurrencySymbol.USD, CurrencySymbol.PLN),
            };
            var forecastFinishDay = new DateTime(2019, 1, 2);
            
            Assert.Throws<ArgumentException>(
                () => rateForecaster.Forecast(
                    source,
                    forecastFinishDay,
                    ForecastMethod.BarycentricInterpolatePolynomialEquidistantSorted));
        }

        [Test]
        public void GivenSourceWithSingleDayAndFinishDay_WhenForecast_ShouldThrowArgumentException()
        {
            var source = new[]
            {
                new Rate(new DateTime(2019, 1, 1), value: 1, CurrencySymbol.USD, CurrencySymbol.PLN),
            };
            var forecastFinishDay = new DateTime(2019, 2, 2);
            
            Assert.Throws<ArgumentException>(
                () => rateForecaster.Forecast(
                    source,
                    forecastFinishDay,
                    ForecastMethod.BarycentricInterpolatePolynomialEquidistantSorted));
        }

        private const double ForecastTolerance = 0.0001;
        private ExtrapolationRateForecaster rateForecaster;
    }
}