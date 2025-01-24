using IngredientsTracker.Data;
using IngredientsTracker.ViewModels;
using System.Diagnostics;

namespace IngredientsTracker;

public partial class DishInformation : ContentPage
{
    DishInformationVM vm;
    DishModel _dish;

    private Timer _timer;
    private const int pauseTime = 500; // ms

	public DishInformation()
    {
        InitializeComponent();
        vm = App.ServiceProvider.GetService<DishInformationVM>();
        BindingContext = vm;
        vm.SearchResultsReady += OnSearchResultsReady;
    }

    public void SetDish(DishModel dish)
    {
        _dish = dish;
        vm.SetDish(dish);

        DishInfoPage.Title = dish.Name + " Information";
    }

    private void OnIngredientEntryTextChanged(object sender, EventArgs e)
    {
        _timer?.Dispose();

        _timer = new Timer(async _ =>
        {
            await vm.SearchIngredients(); // No need for param, we can use NewIngredient in the VM

        }, null, pauseTime, Timeout.Infinite);
    }

    private async void OnSearchResultsReady(object sender, IEnumerable<IngredientSearchResult> results)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            SearchResultsCollectionView.ItemsSource = vm.SearchResults;
            SearchResultsCollectionView.IsVisible = vm.SearchResults.Count > 0;
            SearchResultsCollectionView.InputTransparent = vm.SearchResults.Count <= 0;
        });
    }

    public void OnResultTapped(object sender, EventArgs e)
    {
        Label label = (Label)sender;
        var tapGesture = (TapGestureRecognizer)label.GestureRecognizers.FirstOrDefault();
        var chosenIngredient = tapGesture.CommandParameter as IngredientSearchResult;
        vm.NewSelectedIngredient = chosenIngredient;
        vm.NewIngredient = chosenIngredient.Name;

        SearchResultsCollectionView.IsVisible = false;
        SearchResultsCollectionView.InputTransparent = true;
        vm.optionSelected = true;
    }

    public void IngredientEntryUnfocused(object sender, EventArgs e)
    {
        SearchResultsCollectionView.IsVisible = false;
        SearchResultsCollectionView.InputTransparent = true;
    }

    public async void SubmitIngredient(object sender, EventArgs e)
    {
        bool success = await vm.SubmitNewIngredient();

        if (!success)
        {
            Debug.WriteLine("Error submitting");
            // error message
        }


    }
}