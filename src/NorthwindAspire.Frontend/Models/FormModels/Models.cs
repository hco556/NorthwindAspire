namespace NorthwindAspire.Frontend.Models.FormModels
{

    public class BasicInfoModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? CreditHistory { get; set; }
        public string? Country { get; set; }
        public int LoyaltyFactor { get; set; }
        public decimal TotalPurchasesToDate { get; set; }
    }

    public class OrderInfoModel
    {
        public int TotalOrders { get; set; }
        public int RecurringItems { get; set; }
    }

    public class TelemetryInfoModel
    {
        public int NoOfVisitsPerMonth { get; set; }
        public int PercentageOfBuyingToVisit { get; set; }
    }

}
