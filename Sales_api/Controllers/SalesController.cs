using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sales_dal.Models;
using Sales_api.Interfaces.Sales;

namespace Sales_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;

        public SalesController(ISalesService salesService)
        {
            _salesService = salesService;
        }

        [HttpGet]
        [Route("GetAllSales")]
        public async Task<IActionResult> GetAllSales()
        {
            try
            {
                var result = await _salesService.GetAllPurchases();

                if (result.isSuccess)
                    return Ok(result.purchases);

                return NotFound("No purchases found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}, inner exception: {ex.InnerException}");
            }
        }

        [HttpPost]

        public async Task<IActionResult> PostPurchase(Article article)
        {
            try
            {
                var result = await _salesService.PostPurchase(article);

                if (result.isSucces)
                    return Ok("New purchase added.");

                return NotFound(result.errorMessages);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}, inner exception: {ex.InnerException}");
            }
        }
    }
}