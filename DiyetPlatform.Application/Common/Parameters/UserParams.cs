namespace DiyetPlatform.Application.Common.Parameters
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        private string _search = string.Empty;

        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }
        public string OrderBy { get; set; } = "username";
        public bool IsDietitian { get; set; } = false;
        public string Role { get; set; } = "all";
        public bool OnlyVerified { get; set; } = false;
    }
} 