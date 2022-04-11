using FluentAssertions;
using MLBTheShowSharp.Utils;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using Xunit;

namespace MLBTheShowSharp.Tests.Utils
{
    public class UriHelperTests
    {
        private readonly string baseUrl = "http://some.site";

        [Fact]
        public void WhenUriDoesNotContainParameters_ShouldAddExpectedQueryParameter()
        {
            // Arrange
            var testUri = new Uri(baseUrl);
            var testParameters = new NameValueCollection() { { "pages", "2" } };

            // Act
            var result = testUri.AddOrUpdateQuery(testParameters);
            var queryResult = result.ParseQueryString();

            // Assert
            queryResult.Should().NotBeNull();
            queryResult.Should().BeEquivalentTo(testParameters);
        }

        [Fact]
        public void WhenUriContainsParameter_ShouldUpdateExpectedQueryParameter()
        {
            // Arrange
            var testUrl = $"{baseUrl}?pages=1";
            var testUri = new Uri(testUrl);
            var testParameters = new NameValueCollection() { { "pages", "2" } };

            // Act
            var result = testUri.AddOrUpdateQuery(testParameters);
            var queryResult = result.ParseQueryString();

            // Assert
            queryResult.Should().NotBeNull();
            queryResult.Should().BeEquivalentTo(testParameters);
        }

        [Fact]
        public void WhenUriContainsMultipleParameter_ShouldUpdateExpectedQueryParameter()
        {
            // Arrange
            var testUrl = $"{baseUrl}?pages=1&team=roosters";
            var testUri = new Uri(testUrl);
            var testParameters = new NameValueCollection() { { "pages", "2" } };
            var expectedResult = new NameValueCollection() {
                { "pages", "2" },
                { "team", "rooster" }
            };

            // Act
            var result = testUri.AddOrUpdateQuery(testParameters);
            var queryResult = result.ParseQueryString();

            // Assert
            queryResult.Should().NotBeNull();
            queryResult.Count.Should().Be(2);
            queryResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}
