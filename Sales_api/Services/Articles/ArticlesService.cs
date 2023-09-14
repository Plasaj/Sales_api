using Dapper;
using Sales_api.Interfaces.Articles;
using sales_dal.Interfaces;
using sales_dal.Models;
using System.Collections;
using System.Data;
using Sales_api.Helpers.Articles;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data.Common;
using static Sales_api.Helpers.Articles.ArticleRequestValidator;

namespace Sales_api.Services.Articles
{
  
    public class ArticlesService : IArticlesService
    {
        private readonly IDapperContext _context;
        private readonly ILogger<ArticlesService> _logger;
        private readonly IDbConnection _db;

        public ArticlesService(IDapperContext context, ILogger<ArticlesService> logger)
        {
            _context = context;
            _logger = logger;
            _db = _context.CreateConnection();
        }

        public async Task<(bool isSuccess, IEnumerable<Article>? articles, string? errorMessage)> GetArticlesAsync()
        {
            try
            {
                string sql = $@"SELECT Id, ArticleNumber, Name, Price, CreatedUTC FROM articles";

                var articles = await _db.QueryAsync<Article>(sql);

                if (!articles.Any())
                    return (false, null, "No articles found.");

                return (true, articles, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool isSuccess, Article? article, string? errorMessage)> GetArticleByIdAsync(int id)
        {
            try
            {

                string sql = $@"SELECT Id, ArticleNumber, Name, Price, CreatedUTC 
                                FROM articles WHERE Id = @Id";

                var result = await _db.QuerySingleOrDefaultAsync<Article>(sql, new { Id = id });

                if (result == null)
                    return (false, null, "No article found.");

                return (true, result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool isSuccess, List<ErrorMessage>? errorMessages)> PostArticleAsync(Article request)
        {
            try
            {
                var (isSucces, errorMessages) = ValidateRequest(request);

                if (!isSucces)
                    return (false, errorMessages);

                string sql = $@"BEGIN TRANSACTION;
                                INSERT INTO articles (ArticleNumber, Name, Price, CreatedUTC)
                                VALUES (@ArticleNumber, @Name, @Price, @CreatedUTC);
                                COMMIT;";

                await _db.ExecuteAsync(sql, request);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving the article to the database.");
                return (false, new List<ErrorMessage> { new ErrorMessage { ErrorMsg = "An error occurred while saving the article." } });
            }
        }

        public async Task<(bool isSuccess, List<ErrorMessage>? errorMessages)> UpdateArticleAsync(Article request)
        {
            try
            {
                var (isSucces, errorMessages) = ValidateRequest(request);

                if (!isSucces)
                    return (false, errorMessages);

                // Check if the Id exists in the database
                var article = await _db.QuerySingleOrDefaultAsync<Article>("SELECT * FROM articles WHERE Id = @Id", new { request.Id });

                if (article == null)
                    return (false, new List<ErrorMessage> { new ErrorMessage { ErrorMsg = $"Article with id {request.Id} not found" } });

                string sql = @"BEGIN TRANSACTION;
                               UPDATE articles
                               SET ArticleNumber = @ArticleNumber, Name = @Name, Price = @Price, CreatedUTC = @CreatedUTC
                               WHERE Id = @Id;
                               COMMIT;";

                var parameters = new { request.ArticleNumber, request.Name, request.Price, request.CreatedUTC, request.Id };

                await _db.ExecuteAsync(sql, parameters);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, new List<ErrorMessage> { new ErrorMessage { ErrorMsg = $"An error occurred while updating the article." } });
            }
        }

        public async Task<(bool isSuccess, string? errorMessage)> DeleteArticleAsync(int id)
        {
            try
            {
                if (id == 0)
                    return (false, "Requested id is 0.");

                // Check if the Id exists in the database
                var article = await _db.QuerySingleOrDefaultAsync<Article>("SELECT * FROM articles WHERE Id = @Id", new { Id = id });

                if (article == null)
                    return (false, $"Article with id {id} not found");

                string sql = $@"BEGIN TRANSACTION;
                                DELETE FROM articles WHERE Id = @Id
                                COMMIT;";

                await _db.ExecuteAsync(sql, new { Id = id });

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }

        public void Dispose() => _db?.Dispose();
    }
}