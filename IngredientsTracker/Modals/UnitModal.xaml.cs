using CommunityToolkit.Maui.Views;
using IngredientsTracker.Data;
using IngredientsTracker.ViewModels;

namespace IngredientsTracker.Modals;

public partial class UnitModal : Popup
{

	StockListVM _vm;
	StockItem _item;

	public UnitModal(StockListVM vm, StockItem item)
	{
		_vm = vm;
		_item = item;
		InitializeComponent();
        BindingContext = _vm;
	}

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }

    private async void OnUnitTapped(object sender, TappedEventArgs e)
    {
        Label label = (Label)sender;
        var tapGesture = (TapGestureRecognizer)label.GestureRecognizers.FirstOrDefault();
        string unit = tapGesture.CommandParameter as string;
        bool success = await _vm.EditStockItem(_item, unit);
        if (success)
        {
            Close();
        }  else
        {
            // Error message
        }
    }
}