using TmoTask.DTO;

namespace TmoTask.Services
{
    public interface IDataService
    {
        public void AggregateSalesData();

        public void GenerateDataLists();

        public List<TopSellerItem> GetTopSellers(string branch);

        public List<string> GetBranches();

    }
}
