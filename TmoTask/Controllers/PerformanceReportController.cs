using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TmoTask.Services;

namespace TmoTask.Controllers
{
    [Route("top-sellers")]
    [ApiController]
    public class PerformanceReportController : ControllerBase
    {

        private readonly DataService _dataService;

        public PerformanceReportController(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = _dataService.GetTopSellers();
            return Ok(result);
        }
    }
}
