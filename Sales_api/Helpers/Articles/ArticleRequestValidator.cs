using sales_dal.Models;
using System.Linq.Expressions;

namespace Sales_api.Helpers.Articles
{
     public class ArticleRequestValidator
    {
        public static (bool isSucces, List<ErrorMessage>? errorMessages) ValidateRequest(Article request)
        {
            var errorMessages = new List<ErrorMessage>();

            if (request == null)
                errorMessages.Add(new ErrorMessage { ErrorMsg = "Request is null" });

            else
            {
                if (string.IsNullOrWhiteSpace(request.ArticleNumber))
                    errorMessages.Add(new ErrorMessage { ErrorMsg = "Article number is invalid." });

                if (string.IsNullOrWhiteSpace(request.Name))
                    errorMessages.Add(new ErrorMessage { ErrorMsg = "Article name is invalid." });

                if (request.Price <= 0)
                    errorMessages.Add(new ErrorMessage { ErrorMsg = "Article price is invalid." });

                if (request.CreatedUTC == DateTime.MinValue)
                    errorMessages.Add(new ErrorMessage { ErrorMsg = "Article createdUtc is invalid." });
            }

            return (!errorMessages.Any(), errorMessages);
        }
    }
}