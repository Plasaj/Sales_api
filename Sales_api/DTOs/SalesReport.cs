namespace Sales_api.DTOs
{
     public class SalesReport
    {
        public DateTime Date { get; set; }
        public int TotalArticlesSold { get; set; }
        public int TotalUniqueArticlesSold { get; set; }
    }
}