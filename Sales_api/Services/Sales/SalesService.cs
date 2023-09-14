using Sales_api.Helpers.Articles;
using Sales_api.Interfaces.Sales;
using sales_dal.Interfaces;
using sales_dal.Models;
using System.Data;
using Dapper;


using static Sales_api.Helpers.Articles.ArticleRequestValidator;


namespace Sales_api.Services.Sales
{

    public class SalesService : ISalesService
    {
        private readonly IDapperContext _context;
        private readonly ILogger<SalesService> _logger;
        private readonly IDbConnection _db;

        public SalesService(IDapperContext context, ILogger<SalesService> logger)
        {
            _context = context;
            _logger = logger;
            _db = _context.CreateConnection();
        }

        public async Task<(bool isSuccess, IEnumerable<Purchase>? purchases, string? ErrorMessage)> GetAllPurchases()
        {
            try
            {
                string sql = $@"SELECT PurchaseId, ArticleNumber, Price, CreatedUTC FROM purchases";

                var purchases = await _db.QueryAsync<Purchase>(sql);

                if (!purchases.Any())
                    return (false, null, "No purchases found");

                return (true, purchases, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool isSucces, Article? article, List<ErrorMessage>? errorMessages)> PostPurchase(Article request)
        {
            try
            {
                var (isSucces, errorMessages) = ValidateRequest(request);

                if (!isSucces)
                    return (false, null, errorMessages);

                string sql = $@"BEGIN TRANSACTION;
                                INSERT INTO purchases (ArticleNumber, Price, CreatedUTC)
                                VALUES (@ArticleNumber, @Price, @CreatedUTC);
                                COMMIT;";

                await _db.ExecuteAsync(sql, request);

                return (true, request, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, null, new List<ErrorMessage> { new ErrorMessage { ErrorMsg = "An error occurred while saving the article." } });
            }
        }


        public void Dispose() => _db.Dispose();
    }
}