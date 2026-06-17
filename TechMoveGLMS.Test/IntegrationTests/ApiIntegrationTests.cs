using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using TechMoveGLMS;
using Xunit;

namespace TechMoveGLMS.Tests.IntegrationTests
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<TechMoveGLMS.Program>>
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<TechMoveGLMS.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetContracts_ReturnsSuccessAndData()
        {
            var response = await _client.GetAsync("/api/contracts");

            response.EnsureSuccessStatusCode(); // Throws if not 2xx

            var contracts = await response.Content.ReadFromJsonAsync<List<Contract>>();

            Assert.NotNull(contracts);
            Assert.True(contracts.Count >= 0);
        }

        [Fact]
        public async Task PostServiceRequest_ReturnsCreated()
        {
            var newRequest = new ServiceRequest
            {
                ContractId = 1,                    // Make sure this Contract exists
                Description = "Integration Test Request",
                CostUSD = 1250.00m,
                Status = "Pending"
            };

            var response = await _client.PostAsJsonAsync("/api/serviceRequests", newRequest);

            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        }
    }
}