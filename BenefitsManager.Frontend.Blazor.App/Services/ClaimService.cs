using BenefitsManager.Frontend.Blazor.App.Data;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BenefitsManager.Frontend.Blazor.App.services
{
    public class ClaimService
    {
        private readonly HttpClient _httpClient;

        public ClaimService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Claim>> GetClaimsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Claim>>("/api/claims/?userId=user3@mail.com");
        }

        public async Task<Claim> GetClaimByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Claim>($"/api/claims/{id}");
        }

        public async Task CreateClaimAsync(Claim claim)
        {
            await _httpClient.PostAsJsonAsync("/api/claims", claim);
        }

        public async Task UpdateClaimAsync(Claim claim)
        {
            await _httpClient.PutAsJsonAsync($"/api/claims/{claim.ClaimId}", claim);
        }

        public async Task DeleteClaimAsync(Guid id)
        {
            await _httpClient.DeleteAsync($"/api/claims/{id}");
        }
    }
}
