using System;
using System.Linq;
using ExchangeAdvisor.Domain.Services.Implementation;
using ExchangeAdvisor.Domain.Values;
using Moq;
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

        [Test]
        public void GivenSourceAndFinishDay_WhenForecast_ShouldAppendDaysUntilFinishDayIncluded()
        {
            var source = new[]
            {
                new RateOnDay { Day = new DateTime(2019, 1, 1), Rate =  1 }, 
                new RateOnDay { Day = new DateTime(2019, 1, 2), Rate =  2 },
            };
            var forecastFinishDay = new DateTime(2019, 1, 4);
            
            var result = exchangeRateForecaster.Forecast(source, forecastFinishDay).ToArray();
            
            Assert.That(result, Is.EquivalentTo(new[]
            {
                new RateOnDay { Day = new DateTime(2019, 1, 3), Rate = 3 },
                new RateOnDay { Day = new DateTime(2019, 1, 4), Rate = 4 },
            }));
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

        private ExchangeRateForecaster exchangeRateForecaster;
    }
}