namespace Fare.Library.Models
{
    public class Lookup
    {
        public decimal Discount { get; set; }
        public decimal CompoundDiscount { get; set; }
        public decimal MinAllowedTopUp { get; set; }
        public decimal MaxAllowedTopUp { get; set; }
        public int MaxDailyDiscountCounter { get; set; }
    }
}
