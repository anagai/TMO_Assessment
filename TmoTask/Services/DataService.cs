using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using TmoTask.DTO;

namespace TmoTask.Services
{

    public sealed class DataService
    {
        //Ensure instantiated only once
        private static readonly Lazy<DataService> _instance =
        new(() => new DataService());

        private static List<AggregatedSalesItem> _aggregatedSalesList;
        private static List<string> _branchList;
        private static readonly object _lock = new();

        public DataService() { }

        public static DataService Instance => _instance.Value;

        public void AggregateSalesData()
        {
            
            lock (_lock) // Ensure this is only run once. Prevents race condition from multiple threads
            {
                if (_aggregatedSalesList == null) // Load only once
                {
                    GenerateDataLists();
                }
            }
        }

        public void GenerateDataLists()
        {
            string dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
            string filePath = Path.Combine(dataDirectory, "orders.csv");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found: {filePath}");

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            }))
            {
                var records = csv.GetRecords<CSVRecord>().ToList();

                _aggregatedSalesList = records
                   .GroupBy(r => new { r.Seller, YearMonth = r.OrderDate.ToString("yyyy-MM"), r.Branch })
                   .Select(group => new AggregatedSalesItem
                   {
                       Seller = group.Key.Seller,
                       YearMonth = group.Key.YearMonth,
                       Branch = group.Key.Branch,
                       TotalSales = group.Sum(s => s.Price),
                       TotalOrders = group.Count()
                   })
                   .GroupBy(item => new { item.YearMonth, item.Branch })
                   .Select(group => group
                       .OrderByDescending(item => item.TotalSales)
                       .First())
                   .OrderBy(r => r.Branch)
                   .ThenBy(r => r.YearMonth)
                   .ToList();

                if(_aggregatedSalesList!=null)
                {
                    _branchList = _aggregatedSalesList
                        .Select(item => item.Branch)
                        .Distinct()
                        .OrderBy(branch => branch)
                        .ToList();
                }

            }
        }

        public List<TopSellerItem> GetTopSellers(string branch)
        {
            if (_aggregatedSalesList == null)
                return new List<TopSellerItem>();

            return _aggregatedSalesList
                .Where(item => item.Branch.Equals(branch, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.YearMonth)
                .Select(item => new TopSellerItem
                {
                    Seller = item.Seller,
                    YearMonth = ConvertToMonth(item.YearMonth),
                    TotalSales = item.TotalSales,
                    TotalOrders = item.TotalOrders
                })
                .ToList();
        }

        public List<string> GetBranches()
        {
            if (_branchList == null)
                return new List<string>();

            return _branchList;
                
        }

        private string ConvertToMonth(string yearMonth)
        {
            var dateTime = DateTime.ParseExact(yearMonth, "yyyy-MM", CultureInfo.InvariantCulture);
            return dateTime.ToString("MMMM", CultureInfo.InvariantCulture);
        }
    }
}
