using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Services.Implementation;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace ExchangeAdvisor.Tests.UnitTests.DomainTests.Services
{
    [TestFixture]
    public class RateFetcherTests
    {
        [SetUp]
        public void Setup()
        {
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            MockResponseMessage();

            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(m => m.CreateClient("Exchange Rates API"))
                .Returns(new HttpClient(httpMessageHandlerMock.Object));

            webRateHistoryFetcher = new WebRateHistoryFetcher(httpClientFactoryMock.Object);
        }

        [Test]
        public async Task WhenFetchHistoryAsync_ShouldSendProperRequest()
        {
            MockResponseMessage(
                "{"
                    + "\"rates\":{"
                    + "\"2019-01-03\":{\"PLN\":1.111},"
                    + "\"2019-01-02\":{\"PLN\":2.222}"
                    + "},"
                    + "\"start_at\":\"2019-01-01\","
                    + "\"base\":\"USD\","
                    + "\"end_at\":\"2019-01-03\""
                    + "}");
            
            await webRateHistoryFetcher
                .FetchAsync(
                    DateRange.From(2019, 1, 1).Until(2019, 1, 31),
                    new CurrencyPair(Currency.USD, Currency.PLN))
                .ConfigureAwait(false);

            httpMessageHandlerMock.Protected()
                .Verify(
                    methodName: "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(
                        req => req.Method == HttpMethod.Get
                            && req.RequestUri.ToString()
                                == "https://api.exchangeratesapi.io/history"
                                   + "?start_at=2019-01-01"
                                   + "&end_at=2019-01-31"
                                   + "&base=USD"
                                   + "&symbols=PLN"),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task WhenFetchHistoryAsync_ShouldDeserializeResponseContentProperly()
        {
            MockResponseMessage(
                "{"
                    + "\"rates\":{"
                    + "\"2019-01-03\":{\"PLN\":1.111},"
                    + "\"2019-01-02\":{\"PLN\":2.222}"
                    + "},"
                    + "\"start_at\":\"2019-01-01\","
                    + "\"base\":\"USD\","
                    + "\"end_at\":\"2019-01-03\""
                    + "}");

            var rateHistory = await webRateHistoryFetcher.FetchAsync(
                DateRange.From(2019, 1, 1).Until(2019, 1, 2),
                new CurrencyPair(Currency.USD, Currency.PLN));

            Assert.That(rateHistory.CurrencyPair, Is.EqualTo(new CurrencyPair(Currency.USD, Currency.PLN)));
            rateHistory.Rates.Should().BeEquivalentTo(
                new Rate(new DateTime(2019, 1, 2), 2.222f),
                new Rate(new DateTime(2019, 1, 3), 1.111f));
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

        private WebRateHistoryFetcher webRateHistoryFetcher;
        private Mock<IHttpClientFactory> httpClientFactoryMock;
        private Mock<HttpMessageHandler> httpMessageHandlerMock;
    }
}