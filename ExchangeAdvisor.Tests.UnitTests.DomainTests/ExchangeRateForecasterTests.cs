using System;
using System.Linq;
using ExchangeAdvisor.Domain.Services.Implementation;
using ExchangeAdvisor.Domain.Values;
using NUnit.Framework;

namespace ExchangeAdvisor.Tests.UnitTests.DomainTests
{
    [TestFixture]
    public class ExchangeRateForecasterTests
    {
        [SetUp]
        public void Setup()
        {
            exchangeRateForecaster = new ExchangeRateForecaster();
        }

        [Test, Theory]
        public void GivenSourceAndFinishDay_WhenForecast_ShouldAppendDaysUntilFinishDayIncluded(ForecastMethod forecastMethod)
        {
            var source = new[]
            {
                new RateOnDay { Day = new DateTime(2019, 1, 1), Rate =  1 }, 
                new RateOnDay { Day = new DateTime(2019, 1, 2), Rate =  2 },
                new RateOnDay { Day = new DateTime(2019, 1, 3), Rate =  3 },
                new RateOnDay { Day = new DateTime(2019, 1, 4), Rate =  4 },
                new RateOnDay { Day = new DateTime(2019, 1, 5), Rate =  5 },
                new RateOnDay { Day = new DateTime(2019, 1, 6), Rate =  6 },
            };
            var forecastFinishDay = new DateTime(2019, 1, 8);
            
            var result = exchangeRateForecaster.Forecast(source, forecastFinishDay, forecastMethod).ToArray();
            
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result[0].Day, Is.EqualTo(new DateTime(2019, 1, 7)));
            Assert.That(result[0].Rate, Is.EqualTo(7).Within(forecastTolerance));
            Assert.That(result[1].Day, Is.EqualTo(new DateTime(2019, 1, 8)));
            Assert.That(result[1].Rate, Is.EqualTo(8).Within(forecastTolerance));
        }

        [Test]
        public void GivenSourceAndFinishDayNotGreaterThanLastSourceDay_WhenForecast_ShouldThrowArgumentException()
        {
            var source = new[]
            {
                new RateOnDay { Day = new DateTime(2019, 1, 1), Rate =  1 }, 
                new RateOnDay { Day = new DateTime(2019, 1, 2), Rate =  2 },
            };
            var forecastFinishDay = new DateTime(2019, 1, 2);
            
            Assert.Throws<ArgumentException>(
                () => exchangeRateForecaster.Forecast(source, forecastFinishDay));
        }

        [Test]
        public void GivenSourceWithSingleDayAndFinishDay_WhenForecast_ShouldThrowArgumentException()
        {
            var source = new[]
            {
                new RateOnDay { Day = new DateTime(2019, 1, 1), Rate =  1 },
            };
            var forecastFinishDay = new DateTime(2019, 2, 2);
            
            Assert.Throws<ArgumentException>(
                () => exchangeRateForecaster.Forecast(source, forecastFinishDay));
        }

        private const double forecastTolerance = 0.0001;
        private ExchangeRateForecaster exchangeRateForecaster;
    }
}