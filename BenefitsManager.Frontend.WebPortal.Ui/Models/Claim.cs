namespace BenefitsManager.Frontend.WebPortal.Ui.Models
{
    public class ClaimModel
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
        public required string ReceiptPath { get; set; }
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

    public class ClaimAddModel
    {
        public string Merchant { get; set; }
        public decimal ClaimedAmount { get; set; }
        public long PurchaseDate { get; set; }
        public string CategoryCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReceiptPath { get; set; }
        public UserModel CreatedBy { get; set; }
    }

    public class UserModel
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
    }
}
