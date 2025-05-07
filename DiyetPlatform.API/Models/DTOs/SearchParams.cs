namespace DiyetPlatform.API.Models.DTOs
{
    public class SearchParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string SearchType { get; set; } = "all"; // all, users, posts, recipes, dietplans
    }
}