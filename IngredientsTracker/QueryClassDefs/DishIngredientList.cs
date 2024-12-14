

namespace IngredientsTracker.QueryClassDefs
{
    public class DishIngredientsList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Amount { get; set; }
        public int DishId { get; set; }
        public int IngredientId { get; set; }
    }
}
