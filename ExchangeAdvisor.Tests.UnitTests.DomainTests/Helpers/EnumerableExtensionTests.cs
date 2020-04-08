using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeAdvisor.Domain.Helpers;
using NUnit.Framework;

namespace ExchangeAdvisor.Tests.UnitTests.DomainTests.Helpers
{
    [TestFixture]
    public class EnumerableExtensionTests
    {
        [TestCase(arg: 0, ExpectedResult = true)]
        [TestCase(arg: 1, ExpectedResult = true)]
        [TestCase(arg: 2, ExpectedResult = true)]
        [TestCase(arg: 3, ExpectedResult = false)]
        public bool GivenEnumerableWithMultipleValuesAndExpectedCount_WhenCheckHasAtLeastExpectedCount_ShouldReturnProperResult(
            int expectedCount)
        {
            IEnumerable<int> enumerable = new[] { 1, 2 };

            return enumerable.HasAtLeast(expectedCount);
        }
        
        [TestCase(arg: 0, ExpectedResult = true)]
        [TestCase(arg: 1, ExpectedResult = true)]
        [TestCase(arg: 2, ExpectedResult = false)]
        [TestCase(arg: 3, ExpectedResult = false)]
        public bool GivenEnumerableWithSingleValueAndExpectedCount_WhenCheckHasAtLeastExpectedCount_ShouldReturnProperResult(
            int expectedCount)
        {
            IEnumerable<int> enumerable = new[] { 1 };

            return enumerable.HasAtLeast(expectedCount);
        }
        
        [TestCase(arg: 0, ExpectedResult = true)]
        [TestCase(arg: 1, ExpectedResult = false)]
        public bool GivenEmptyEnumerableAndExpectedCount_WhenCheckHasAtLeastExpectedCount_ShouldReturnProperResult(
            int expectedCount)
        {
            var enumerable = Enumerable.Empty<int>();

            return enumerable.HasAtLeast(expectedCount);
        }
        
        [Test]
        public void GivenEnumerableAndExpectedNegativeCount_WhenCheckHasAtLeastExpectedCount_ShouldReturnProperResult()
        {
            IEnumerable<int> enumerable = new[] { 1, 2 };

            Assert.Throws<ArgumentException>(() => enumerable.HasAtLeast(count: -1));
        }
    }
}