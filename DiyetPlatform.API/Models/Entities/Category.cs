namespace DiyetPlatform.API.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // İlişkiler
        public ICollection<RecipeCategory> Recipes { get; set; }
    }
}