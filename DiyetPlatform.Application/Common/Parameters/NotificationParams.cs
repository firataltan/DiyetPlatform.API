namespace DiyetPlatform.Application.Common.Parameters
{
    public class NotificationParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public bool OnlyUnread { get; set; } = false;
        public string OrderBy { get; set; } = "newest"; // "newest" or "oldest"
    }
} 