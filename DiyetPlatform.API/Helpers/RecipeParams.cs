namespace DiyetPlatform.API.Helpers
{
    public class RecipeParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string OrderBy { get; set; } = "created";
        public int? CategoryId { get; set; }
        public int? MinCalories { get; set; }
        public int? MaxCalories { get; set; }
        public int? MaxPrepTime { get; set; }
    }
} 