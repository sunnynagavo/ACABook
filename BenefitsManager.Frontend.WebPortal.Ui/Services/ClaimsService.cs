using BenefitsManager.Frontend.WebPortal.Ui.Models;

namespace BenefitsManager.Frontend.WebPortal.Ui.Services
{
    public class ClaimsService
    {
        private readonly HttpClient _httpClient;

        public ClaimsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ClaimModel>> GetClaimsAsync()
        {
            var claims = await _httpClient.GetFromJsonAsync<List<ClaimModel>>("api/claims/?userId=user3@mail.com");
            if (claims == null)
            {
                return new List<ClaimModel>();
            }
            return claims;
        }

        public async Task<ClaimModel> GetClaimByIdAsync(Guid claimId)
        {
            var claim = await _httpClient.GetFromJsonAsync<ClaimModel>($"api/claims/{claimId}");
            if (claim == null)
            {
                throw new InvalidOperationException("Claim not found.");
            }
            return claim;
        }

        public async Task<HttpResponseMessage> CreateClaimAsync(ClaimAddModel claim)
        {
            return await _httpClient.PostAsJsonAsync("api/claims", claim);
        }

        public async Task UpdateClaimAsync(ClaimModel claim)
        {
            await _httpClient.PutAsJsonAsync($"api/claims/{claim.ClaimId}", claim);
        }

        public async Task DeleteClaimAsync(Guid id)
        {
            await _httpClient.DeleteAsync($"api/claims/{id}");
        }
    }
}
