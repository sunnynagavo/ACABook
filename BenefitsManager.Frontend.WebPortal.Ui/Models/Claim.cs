namespace BenefitsManager.Frontend.WebPortal.Ui.Models
{
    public class Claim
    {
        public required Guid ClaimId { get; set; }
        public required string Merchant { get; set; }
        public decimal ClaimedAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public long PurchaseDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public long CreatedOn { get; set; }
        public long? ModifiedOn { get; set; }
        public required ClaimCategoryModel Category { get; set; }

        public required ClaimStatus CurrentStatus { get; set; } = ClaimStatus.Pending;
    }

    public class ClaimCategoryModel
    {
        public required string CategoryCode { get; set; }
        public required string ParentCategoryName { get; set; }
        public required string CategoryName { get; set; }
    }

    public enum ClaimStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
