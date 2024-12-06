using BenefitsManager.Frontend.WebPortal.Ui.Models;
using Dapr.Client;

namespace BenefitsManager.Frontend.WebPortal.Ui.Services
{
    public class ClaimServices
    {
        private readonly HttpClient _httpClient;
        private readonly DaprClient _daprClient;

        public ClaimServices(HttpClient httpClient, DaprClient daprClient)
        {
            _httpClient = httpClient;
            _daprClient = daprClient;
        }

        public async Task<List<Claim>> GetClaimsAsync()
        {
            return await _daprClient.InvokeMethodAsync<List<Claim>>(
                HttpMethod.Get,
                "BenefitsManager-Backend-Bff-Api",
                "api/claims/?userId=user3@mail.com"
            );
        }

        public async Task<Claim> GetClaimByIdAsync(int id)
        {
            return await _daprClient.InvokeMethodAsync<Claim>(
                HttpMethod.Get,
                "BenefitsManager-Backend-Bff-Api",
                $"api/claims/{id}"
            );
        }

        public async Task CreateClaimAsync(Claim claim)
        {
            await _daprClient.InvokeMethodAsync(
                HttpMethod.Post,
                "BenefitsManager-Backend-Bff-Api",
                "api/claims",
                claim
            );
        }

        public async Task UpdateClaimAsync(Claim claim)
        {
            await _daprClient.InvokeMethodAsync(
                HttpMethod.Put,
                "BenefitsManager-Backend-Bff-Api",
                $"api/claims/{claim}",
                claim
            );
        }

        public async Task DeleteClaimAsync(Guid id)
        {
            await _daprClient.InvokeMethodAsync(
                HttpMethod.Delete,
                "BenefitsManager-Backend-Bff-Api",
                $"api/claims/{id}"
            );
        }
    }
}
