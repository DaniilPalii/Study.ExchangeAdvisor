using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace ExchangeAdvisor.Tests.UnitTests.DomainTests
{
    public class ExchangeRateFetcherTests
    {
        [SetUp]
        public void Setup()
        {
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()) 
                .ReturnsAsync(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("[ ]")
                    })
                .Verifiable();
            
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(m => m.CreateClient("Exchange Rates API"))
                .Returns(new HttpClient(httpMessageHandlerMock.Object));

            exchangeRateFetcher = new ExchangeRateFetcher(httpClientFactoryMock.Object);
        }

        [Test]
        public async Task FetchHistoryAsync()
        {
            await exchangeRateFetcher
                .FetchHistoryAsync(
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

        private ExchangeRateFetcher exchangeRateFetcher;
        private Mock<IHttpClientFactory> httpClientFactoryMock;
        private Mock<HttpMessageHandler> httpMessageHandlerMock;
    }
}