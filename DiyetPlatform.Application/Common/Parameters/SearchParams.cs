namespace DiyetPlatform.Application.Common.Parameters
{
    public class SearchParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        private string _query = string.Empty;

        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string Query
        {
            get => _query;
            set => _query = value?.ToLower() ?? string.Empty;
        }
        public string OrderBy { get; set; } = "createdAt";
        public bool IncludeUsers { get; set; } = true;
        public bool IncludePosts { get; set; } = true;
        public bool IncludeRecipes { get; set; } = true;
        public bool IncludeDietPlans { get; set; } = true;
    }
} 