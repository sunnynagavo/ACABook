using BenefitsManager.Backend.Bff.Api.Common;
using BenefitsManager.Backend.Bff.Api.Models;

namespace BenefitsManager.Backend.Bff.Api.Services
{
    public interface IClaimsManager
    {
        Task<List<ClaimModel>> GetClaimsByCreatorAsync(string userId);
        Task<ClaimModel?> GetClaimByIdAsync(Guid claimId);
        Task<Guid> CreateNewClaimAsync(string merchant, decimal claimedAmount, long purchaseDate, string categoryCode, string description, string receiptPath, UserModel createdBy);
        Task<bool> UpdateClaimAsync(Guid claimId, string merchant, decimal claimedAmount, long purchaseDate, string categoryCode, string description, string receiptPath);
        Task<bool> UpdateClaimStatusAsync(Guid claimId, decimal approvedAmount, ClaimStatus newStatus, string comment, UserModel setBy);
        Task<bool> DeleteClaimAsync(Guid claimId);

    }
}