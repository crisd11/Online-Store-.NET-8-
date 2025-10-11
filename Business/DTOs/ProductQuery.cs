namespace OnlineStore.Core.DTOs
{
    public class ProductQuery
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; } = 1;      // 1-based
        public int PageSize { get; set; } = 12;
        public string? SortBy { get; set; }     // name | price | createdAt
        public string? SortDir { get; set; }    // asc | desc
    }
}
