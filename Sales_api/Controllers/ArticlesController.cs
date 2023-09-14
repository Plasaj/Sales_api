using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sales_api.Interfaces.Articles;
using Sales_api.Services.Articles;
using sales_dal.Models;

namespace Sales_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticlesService _articleService;

        public ArticlesController(IArticlesService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        [Route("GetAllArticles")]
        public async Task<IActionResult> GetAllArticles()
        {
            try
            {
                var result = await _articleService.GetArticlesAsync();

                if (result.isSuccess)
                    return Ok(result.articles);

                return NotFound("No articles found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}, inner exception: {ex.InnerException}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            try
            {
                var result = await _articleService.GetArticleByIdAsync(id);

                if (result.isSuccess)
                    return Ok(result.article);

                return NotFound($"No article with Id: {id} found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}, inner exception: {ex.InnerException}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostArticle(Article request)
        {
            try
            {
                var result = await _articleService.PostArticleAsync(request);

                if (result.isSuccess)
                    return Ok($"Article added.");

                return NotFound(result.errorMessages);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}, inner exception: {ex.InnerException}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateArticle(Article request)
        {
            try
            {
                var result = await _articleService.UpdateArticleAsync(request);

                if (result.isSuccess)
                    return Ok($"Article updated.");

                return NotFound(result.errorMessages);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}, inner exception: {ex.InnerException}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            try
            {
                var result = await _articleService.DeleteArticleAsync(id);

                if (result.isSuccess)
                    return Ok($"Article deleted.");

                return NotFound(result.errorMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}, inner exception: {ex.InnerException}");
            }
        }
    }
}