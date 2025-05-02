namespace Jammer.OrderModule.DTOs
{
    public class OverallOrderReport
    {
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal MinimumOrderValue { get; set; }
        public decimal MaximumOrderValue { get; set; }
    }
}
