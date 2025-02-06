namespace TmoTask.DTO
{
    public class AggregatedSalesItem
    {
        public string Seller { get; set; }
        public string YearMonth { get; set; }
        public string Branch { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
    }
}
