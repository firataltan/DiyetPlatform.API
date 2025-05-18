namespace DiyetPlatform.Application.Common.Parameters
{
    public class DietPlanParams
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
        public string OrderBy { get; set; } = "createdAt";
        public string DietType { get; set; } = "all"; // Keto, Vegan, Vegetarian, LowCarb, etc.
        public int? UserId { get; set; }
        public int? DietitianId { get; set; }
        public bool OnlyApproved { get; set; } = false;
        public bool? IsActive { get; set; }
    }
} 