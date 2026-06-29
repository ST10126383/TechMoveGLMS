using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using TechMoveGLMS.Models;
using Xunit;

namespace TechMoveGLMS.Tests.IntegrationTests
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetContracts_ReturnsSuccessAndData()
        {
            var response = await _client.GetAsync("/api/contracts");
            response.EnsureSuccessStatusCode();

            var contracts = await response.Content.ReadFromJsonAsync<List<Contract>>();

            Assert.NotNull(contracts);
        }
    }
}