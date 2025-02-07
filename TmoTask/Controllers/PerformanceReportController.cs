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

        private readonly IDataService _dataService;

        public PerformanceReportController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("top-sellers")]
        public IActionResult GetTopSellers([FromQuery] string branch)
        {
            try
            {
                if (string.IsNullOrEmpty(branch))
                {
                    return BadRequest("Branch value is required.");
                }
                var result = _dataService.GetTopSellers(branch);
                return Ok(result);
            } catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpGet("branches")]
        public IActionResult GetBranches()
        {

            try
            {
                var result = _dataService.GetBranches();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpGet("reload-data")]
        public IActionResult ReloadData()
        {
            try
            {
                _dataService.GenerateDataLists();
                return Ok("Success");
            } catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
            
        }
    }
}
