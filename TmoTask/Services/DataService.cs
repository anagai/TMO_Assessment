using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.Net.NetworkInformation;

namespace TmoTask.Services
{

    public class CSVRecord
    {
        public string Seller { get; set; }
        public string Product { get; set; }
        public decimal Price { get; set; }
        public DateTime OrderDate { get; set; }
        public string Branch { get; set; }
    }

    public class AggregatedSalesItem
    {
        public string Seller { get; set; }
        public string Month { get; set; }
        public string Branch { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
    }

    public sealed class DataService
    {
        //Ensure instantiated only once
        private static readonly Lazy<DataService> _instance =
        new(() => new DataService());

        private static List<AggregatedSalesItem> _aggregatedSalesList;
        private static List<AggregatedSalesItem> _topSellersList;
        private static readonly object _lock = new();

        public DataService() { }

        public static DataService Instance => _instance.Value;

        public void AggregateSalesData(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found: {filePath}");

            lock (_lock) // Ensure this is only run once. Prevents race condition from multiple threads
            {
                if (_aggregatedSalesList == null) // Load only once
                {
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true
                    }))
                    {
                        var records = csv.GetRecords<CSVRecord>().ToList();

                        // aggregate by seller, month and branch. Order by branch, month TotalSales in desc order
                        _aggregatedSalesList = records
                            .GroupBy(r => new { r.Seller, MonthYear = r.OrderDate.ToString("MMMM"), r.Branch })
                            .Select(group => new AggregatedSalesItem
                            {
                                Seller = group.Key.Seller,
                                Month = group.Key.MonthYear,
                                Branch = group.Key.Branch,
                                TotalSales = group.Sum(s => s.Price),
                                TotalOrders = group.Count()
                            })
                            .OrderBy(r => r.Branch)
                            .ThenBy(r => r.Month)
                            .ThenByDescending(r => r.TotalSales)
                            .ToList();

                        _topSellersList = _aggregatedSalesList
                            .GroupBy(item => item.Month)
                            .Select(group => group
                                .OrderByDescending(item => item.TotalSales)
                                .Select(item => new AggregatedSalesItem
                                {
                                    Seller = item.Seller,
                                    Month = item.Month,
                                    TotalSales = item.TotalSales,
                                    TotalOrders = item.TotalOrders,
                                    Branch = item.Branch
                                })
                                .First())
                            .ToList();
                        var topSellersByBranch = _topSellersList
                            .Where()
                    }
                }
            }
        }

        public List<AggregatedSalesItem> GetAggregatedSales()
        {
            return _aggregatedSalesList ?? new List<AggregatedSalesItem>();
        }

        public List<AggregatedSalesItem> GetTopSellers()
        {
            return _topSellersList ?? new List<AggregatedSalesItem>();
        }
    }
}
