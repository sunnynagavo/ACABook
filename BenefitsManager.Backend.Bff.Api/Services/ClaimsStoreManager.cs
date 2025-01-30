using BenefitsManager.Common.Models;
using Dapr.Client;
using System.Security.Claims;

namespace BenefitsManager.Backend.Bff.Api.Services
{
    /// <summary>
    /// Manages the claims store which implements the IClaimsManager interface for storing claims in the Dapr State store.
    /// </summary>
    public class ClaimsStoreManager : IClaimsManager
    {
        private static string STORE_NAME = "statestore";
        private readonly DaprClient _daprClient;
        private readonly IConfiguration _config;
        private readonly ILogger<ClaimsStoreManager> _logger;
        private readonly List<ClaimCategoryModel> _claimCategoriesList = new List<ClaimCategoryModel>();

        public ClaimsStoreManager(DaprClient daprClient, IConfiguration config, ILogger<ClaimsStoreManager> logger)
        {
            _daprClient = daprClient;
            _config = config;
            _logger = logger;

            GenerateRandomCategories();
        }

        private void GenerateRandomCategories()
        {
            var categories = new List<ClaimCategoryModel>
            {
                new ClaimCategoryModel { CategoryCode = "CAT001", ParentCategoryName = "Clothing and shoes", CategoryName = "Athletic Accessories" },
                new ClaimCategoryModel { CategoryCode = "CAT002", ParentCategoryName = "Clothing and shoes", CategoryName = "Athletic Apparel" },
                new ClaimCategoryModel { CategoryCode = "CAT003", ParentCategoryName = "Fitness Activities", CategoryName = "Gym" },
                new ClaimCategoryModel { CategoryCode = "CAT004", ParentCategoryName = "Fitness Activities", CategoryName = "Fitness Classes" },
                new ClaimCategoryModel { CategoryCode = "CAT005", ParentCategoryName = "Home Office", CategoryName = "Desks and Chairs" }
            };

            _claimCategoriesList.AddRange(categories);

        }

        public async Task<Guid> CreateNewClaimAsync(string merchant, decimal claimedAmount, long purchaseDate, string categoryCode, string description, string receiptPath, UserModel createdBy)
        {
            var randomNo = Random.Shared.Next(1, 6);

            var claimId = Guid.NewGuid();
            var claim = new ClaimModel
            {
                ClaimId = claimId,
                Merchant = merchant,
                ClaimedAmount = claimedAmount,
                PurchaseDate = purchaseDate,
                Category = _claimCategoriesList.First(c => c.CategoryCode == categoryCode),
                Description = description,
                StatusLog = new List<ClaimStatusModel>
                {
                    new ClaimStatusModel
                    {
                        Status = ClaimStatus.Pending,
                        Comment = "",
                        SetBy = createdBy,
                        Ts = DateTimeOffset.Now.ToUnixTimeMilliseconds()
                    }
                },
                CurrentStatus = ClaimStatus.Pending,
                ReceiptPath = receiptPath,
                CreatedBy = createdBy,
                ApprovedAmount = null,
                CreatedOn = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                ModifiedOn = null
            };
            _logger.LogInformation("Save a new claim with id: '{0}' to state store", claim.ClaimId);

            //log entire claim object into logger for debugging
            _logger.LogInformation("Claim object: {0}", claim);

            await _daprClient.SaveStateAsync(STORE_NAME, claimId.ToString(), claim);
            await PublishClaimSavedEvent(claim);
            return claimId;
        }

        private async Task PublishClaimSavedEvent(ClaimModel claim)
        {
            _logger.LogInformation("Publish claim Saved event for claim with Id: '{0}' and Description: '{1}' by: '{2}'",
            claim.ClaimId, claim.Description, claim.CreatedBy);
            await _daprClient.PublishEventAsync("dapr-pubsub-servicebus", "claimsavedtopic", claim);
        }

        public async Task<bool> DeleteClaimAsync(Guid claimId)
        {
            await _daprClient.DeleteStateAsync(STORE_NAME, claimId.ToString());
            return true;
        }

        public async Task<List<ClaimModel>> GetClaimsByCreatorAsync(string userId)
        {
            var state = await _daprClient.GetStateEntryAsync<List<ClaimModel>>(STORE_NAME, userId);
            return state.Value ?? new List<ClaimModel>();
        }

        public async Task<ClaimModel?> GetClaimByIdAsync(Guid claimId)
        {
            var state = await _daprClient.GetStateEntryAsync<ClaimModel>(STORE_NAME, claimId.ToString());
            return state.Value;
        }

        public async Task<bool> UpdateClaimAsync(Guid claimId, string merchant, decimal claimedAmount, long purchaseDate, string categoryCode, string description, string receiptPath)
        {
            var state = await _daprClient.GetStateEntryAsync<ClaimModel>(STORE_NAME, claimId.ToString());
            if (state.Value == null)
            {
                return false;
            }
            state.Value.Merchant = merchant;
            state.Value.ClaimedAmount = claimedAmount;
            state.Value.PurchaseDate = purchaseDate;
            state.Value.Category = _claimCategoriesList.First(c => c.CategoryCode == categoryCode);
            state.Value.Description = description;
            state.Value.ReceiptPath = receiptPath;
            state.Value.ModifiedOn = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            await state.SaveAsync();
            await PublishClaimSavedEvent(state.Value);
            return true;
        }

        public async Task<bool> UpdateClaimStatusAsync(Guid claimId, decimal approvedAmount, ClaimStatus newStatus, string comment, UserModel setBy)
        {
            var state = await _daprClient.GetStateEntryAsync<ClaimModel>(STORE_NAME, claimId.ToString());
            if (state.Value == null)
            {
                return false;
            }
            state.Value.CurrentStatus = newStatus;
            state.Value.ApprovedAmount = approvedAmount;
            state.Value.StatusLog.Add(new ClaimStatusModel
            {
                Status = newStatus,
                Comment = comment,
                SetBy = setBy,
                Ts = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            });
            await state.SaveAsync();
            await PublishClaimSavedEvent(state.Value);
            return true;
        }
    }
}
