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
    }

    public void SetDish(DishModel dish)
    {
        _dish = dish;
        vm.SetDish(dish);

        DishInfoPage.Title = dish.Name + " Information";

        vm.SearchResultsReady += OnSearchResultsReady;
    }

    private void OnIngredientEntryTextChanged(object sender, EventArgs e)
    {
        _timer?.Dispose();

        // Create a new timer that triggers after the user pauses typing for 1 second
        _timer = new Timer(async _ =>
        {
            // The user paused typing, so do your API call
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
        /**
         * 
         * After selecting the ingredient, it performs another search, therefore the drop down shows again
         * Need to fix this small bug
         * Should be relatively straight forward
         * 
         */ 
        SearchResultsCollectionView.IsVisible = false;
        SearchResultsCollectionView.InputTransparent = true;
    }

    public void IngredientEntryUnfocused(object sender, EventArgs e)
    {
        SearchResultsCollectionView.IsVisible = false;
        SearchResultsCollectionView.InputTransparent = true;
    }

    public void UpdateChosenUnit(object sender, EventArgs e)
    {
        Debug.WriteLine("Perform update");
    }

    private void OnResultTapped(object sender, TappedEventArgs e)
    {

    }
}