using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelAgencyCommissionSystem.Web.Services.Interfaces;

namespace TravelAgencyCommissionSystem.Web.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("commission-summary")]
        public async Task<IActionResult> GetCommissionSummary(
           [FromQuery] string month,
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10,
           [FromQuery] string sortBy = "name",
           [FromQuery] bool descending = false)
        {
            var result =
                await _reportService.GetCommissionSummaryAsync(
                    month,
                    page,
                    pageSize,
                    sortBy,
                    descending);

            return Ok(result);
        }
    }
}
