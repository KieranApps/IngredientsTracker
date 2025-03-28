using CommunityToolkit.Maui.Views;
using IngredientsTracker.Data;
using IngredientsTracker.Modals;
using IngredientsTracker.ViewModels;
using System.Diagnostics;

namespace IngredientsTracker;

public partial class StockList : ContentPage
{
    StockListVM vm;

    private Timer _timer;
    private const int pauseTime = 500; // ms

    public StockList()
	{
		InitializeComponent();
        vm = App.ServiceProvider.GetService<StockListVM>();
        BindingContext = vm;
        vm.SearchResultsReady += OnSearchResultsReady;
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

    private void OnIngredientEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        _timer?.Dispose();

        _timer = new Timer(async _ =>
        {
            await vm.SearchIngredients(); // No need for param, we can use NewIngredient in the VM

        }, null, pauseTime, Timeout.Infinite);
    }

    private async void SubmitIngredient(object sender, EventArgs e)
    {
        bool success = await vm.SubmitNewIngredient();

        if (!success)
        {
            Debug.WriteLine("Error submitting");
            // error message
        }
    }

    private void OnResultTapped(object sender, TappedEventArgs e)
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

    private void IngredientEntryUnfocused(object sender, FocusEventArgs e)
    {
        SearchResultsCollectionView.IsVisible = false;
        SearchResultsCollectionView.InputTransparent = true;
    }

    private async void AmountChanged(object sender, FocusEventArgs e)
    {
        Entry entry = (Entry)sender;
        StockItem item = entry.ReturnCommandParameter as StockItem;
        bool success = await vm.EditStockItem(item);

        if (!success) 
        {
            // Error message
        }

    }

    private async void OpenUnitModal(object sender, TappedEventArgs e)
    {
        Label label = (Label)sender;
        var tapGesture = (TapGestureRecognizer)label.GestureRecognizers.FirstOrDefault();
        var item = tapGesture.CommandParameter as StockItem;
        var popup = new UnitModal(vm, item);
        await this.ShowPopupAsync(popup);
    }
}