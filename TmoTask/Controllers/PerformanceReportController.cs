using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TmoTask.Services;

namespace TmoTask.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PerformanceReportController : ControllerBase
    {

        private readonly DataService _dataService;

        public PerformanceReportController(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("top-sellers")]
        public IActionResult GetTopSellers([FromQuery] string branch)
        {
            var result = _dataService.GetTopSellers(branch);
            return Ok(result);
        }

        [HttpGet("branches")]
        public IActionResult GetBranches()
        {
            var result = _dataService.GetBranches();
            return Ok(result);
        }

        [HttpGet("reload-data")]
        public IActionResult ReloadData()
        {
            _dataService.GenerateDataLists();
            return Ok();
        }
    }
}
