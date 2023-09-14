using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sales_dal.Models
{
    //This model represents the db table
    public class Article
    {
        public int Id { get; set; }
        public string? ArticleNumber { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedUTC { get; set; }
    }
}
