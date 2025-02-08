using IngredientsTracker.Helpers;


namespace IngredientsTracker.ViewModels
{
    public partial class StockListVM : BindableObject
    {
        private readonly ApiService _api;

        public StockListVM() { }
        public StockListVM(ApiService api)
        {
            _api = api;
        }
    }
}
