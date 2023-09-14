using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sales_api.Interfaces.Reports;

namespace Sales_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reportsService;

        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [HttpGet]
        [Route("GetNumberArticlesSoldPerDay")]
        public async Task<IActionResult> GetNumberArticlesSoldPerDay(DateTime? reportDate)
        {
            try
            {
                var result = await _reportsService.GetNumberArticlesSoldPerDay(reportDate);

                if (result.isSuccess)
                    return Ok(result.salesReports);

                return NotFound(result.errorMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}, inner exception: {ex.InnerException}");
            }
        }

        [HttpGet]
        [Route("TotalRevenuePerDay")]
        public async Task<IActionResult> GetTotalRevenuePerDay(DateTime? reportDate)
        {
            try
            {
                var result = await _reportsService.GetTotalRevenuePerDay(reportDate);

                if (result.isSuccess)
                    return Ok(result.revenueReports);

                return NotFound(result.errorMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}, inner exception: {ex.InnerException}");
            }
        }

        [HttpGet]
        [Route("GetStatistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var result = await _reportsService.GetStatistics();

                if (result.isSuccess)
                    return Ok(result.statisticsReports);

                return NotFound("No statistics found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}, inner exception: {ex.InnerException}");
            }
        }
    }
}