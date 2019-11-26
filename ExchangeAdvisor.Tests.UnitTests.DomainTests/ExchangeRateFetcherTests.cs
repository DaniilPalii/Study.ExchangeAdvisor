using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Services.Implementation;
using ExchangeAdvisor.Domain.Values;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace ExchangeAdvisor.Tests.UnitTests.DomainTests
{
    [TestFixture]
    public class ExchangeRateFetcherTests
    {
        [SetUp]
        public void Setup()
        {
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            MockResponseMessage();
            
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(m => m.CreateClient("Exchange Rates API"))
                .Returns(new HttpClient(httpMessageHandlerMock.Object));

            exchangeRateFetcher = new ExchangeRateFetcher(httpClientFactoryMock.Object);
        }

        [Test]
        public async Task WhenFetchHistoryAsync_ShouldSendProperRequest()
        {
            await exchangeRateFetcher
                .FetchRateHistoryAsync(
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 1, 31),
                    CurrencySymbol.USD,
                    CurrencySymbol.PLN)
                .ConfigureAwait(false);
            
            httpMessageHandlerMock.Protected()
                .Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(
                        req => req.Method == HttpMethod.Get
                               && req.RequestUri.ToString()
                                    == "https://api.exchangeratesapi.io/history" +
                                        "?start_at=2019-01-01" +
                                        "&end_at=2019-01-31" +
                                        "&base=USD" +
                                        "&symbols=PLN"),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task WhenFetchHistoryAsync_ShouldDeserializeResponseContentProperly()
        {
            MockResponseMessage(
                "{"
                    + @"""rates"":{"
                        + @"""2019-01-03"":{""PLN"":1.111},"
                        + @"""2019-01-02"":{""PLN"":2.222}"
                    + "},"
                    + @"""start_at"":""2019-01-01"","
                    + @"""base"":""USD"","
                    + @"""end_at"":""2019-01-03"""
                + "}");
            
            var rateHistory = (await exchangeRateFetcher
                .FetchRateHistoryAsync(
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 1, 2),
                    CurrencySymbol.USD,
                    CurrencySymbol.PLN)
                .ConfigureAwait(false))
                    .ToArray();
            
            Assert.That(rateHistory.Length, Is.EqualTo(2));
            Assert.That(rateHistory[0].Day, Is.EqualTo(new DateTime(2019, 1, 3)));
            Assert.That(rateHistory[0].Rate, Is.EqualTo(1.111));
            Assert.That(rateHistory[1].Day, Is.EqualTo(new DateTime(2019, 1, 2)));
            Assert.That(rateHistory[1].Rate, Is.EqualTo(2.222));
        }

        private void MockResponseMessage(string messageContent = "")
        {
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(messageContent)
                    })
                .Verifiable();
        }

        private ExchangeRateFetcher exchangeRateFetcher;
        private Mock<IHttpClientFactory> httpClientFactoryMock;
        private Mock<HttpMessageHandler> httpMessageHandlerMock;
    }
}