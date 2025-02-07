using Microsoft.AspNetCore.Mvc;
using TmoTask.Services;

namespace TmoTask.Controllers
{
    [Route("api")]
    [ApiController]
    public class PerformanceReportController : ControllerBase
    {

        private readonly IDataService _dataService;

        public PerformanceReportController(IDataService dataService)
        {
            _dataService = dataService;
        }

        /*
         * GetTopSellers()
         * 
         * Will pull Top Sellers for a branch from cached aggregated list.
         * Aggregated list was generated upon application start.
         */
        
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

        /*
         * GetBranches()
         * 
         * Will get list of unique branch names from cached branch list.
         * Branch list was pulled from aggregated data upon application start.
         * 
         */

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

        /*
         * ReloadData()
         * 
         * Re-generate aggregated and branch list data.
         *  
         */

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
