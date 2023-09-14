using Sales_api.DTOs;
using Sales_api.Interfaces.Reports;
using sales_dal.Interfaces;
using System.Data;
using System.Text;
using Dapper;


namespace Sales_api.Services.Reports
{
    public class ReportsService : IReportsService
    {
        private readonly IDapperContext _context;
        private readonly ILogger<ReportsService> _logger;
        private readonly IDbConnection _db;

        public ReportsService(IDapperContext context, ILogger<ReportsService> logger)
        {
            _context = context;
            _logger = logger;
            _db = _context.CreateConnection();
        }

        public async Task<(bool isSuccess, IEnumerable<SalesReport>? salesReports, string? errorMessage)> GetNumberArticlesSoldPerDay(DateTime? reportDate)
        {
            try
            {
                var queryBuilder = new StringBuilder();

                queryBuilder.AppendLine(@$"SELECT CONVERT(DATE, CreatedUTC) AS Date, 
                                           COUNT(*) AS TotalArticlesSold, 
                                           COUNT(DISTINCT ArticleNumber) AS TotalUniqueArticlesSold 
                                           FROM purchases");

                if (reportDate.HasValue)
                    queryBuilder.AppendLine($"WHERE CONVERT(DATE, CreatedUTC) = @ReportDate");

                queryBuilder.AppendLine("GROUP BY CONVERT(DATE, CreatedUTC)");

                var query = queryBuilder.ToString();

                var salesReports = await _db.QueryAsync<SalesReport>(query, new { ReportDate = reportDate?.Date });

                if (!salesReports.Any())
                    return (false, null, $"No sales reports found for {reportDate?.Date ?? DateTime.Today}");

                return (true, salesReports, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, null, ex.Message.ToString());
            }
        }

        public async Task<(bool isSuccess, IEnumerable<RevenueReport>? revenueReports, string? errorMessage)> GetTotalRevenuePerDay(DateTime? reportDate)
        {
            try
            {
                var querybuilder = new StringBuilder();

                querybuilder.AppendLine($@"SELECT CONVERT(DATE, CreatedUTC) AS Date,
                                           SUM(PRICE) AS Revenue 
                                           FROM purchases");

                if (reportDate.HasValue)
                {
                    querybuilder.AppendLine("WHERE CreatedUTC >= @ReportDate AND CreatedUTC < DATEADD(day, 1, @ReportDate)");
                    querybuilder.AppendLine("GROUP BY CONVERT(DATE, CreatedUTC)");

                    var revenueReports = await _db.QueryAsync<RevenueReport>(querybuilder.ToString(), new { ReportDate = reportDate.Value.Date });

                    if (!revenueReports.Any())
                        return (false, null, $"No revenue reports found for {reportDate.Value.Date}.");

                    return (true, revenueReports, null);
                }
                else
                {
                    querybuilder.AppendLine($"GROUP BY CONVERT(DATE, CreatedUTC)");

                    var revenueReports = await _db.QueryAsync<RevenueReport>(querybuilder.ToString());

                    if (!revenueReports.Any())
                        return (false, null, $"No revenue reports found.");

                    return (true, revenueReports, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, null, ex.Message.ToString());
            }
        }

        public async Task<(bool isSuccess, IEnumerable<StatisticsReport>? statisticsReports, string? errorMessage)> GetStatistics()
        {
            try
            {
                var queryBuilder = new StringBuilder();

                queryBuilder.AppendLine(@$"SELECT ArticleNumber, 
                                           SUM(Price) AS Revenue 
                                           FROM purchases
                                           GROUP BY ArticleNumber");

                var statisticsReport = await _db.QueryAsync<StatisticsReport>(queryBuilder.ToString());

                if (!statisticsReport.Any())
                    return (false, null, $"No statistics found at all.");

                return (true, statisticsReport, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, null, ex.Message.ToString());
            }
        }


        public void Dispose() => _db.Dispose();
    }
}