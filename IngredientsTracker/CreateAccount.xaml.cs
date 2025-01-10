using IngredientsTracker.Helpers;

namespace IngredientsTracker;

public partial class CreateAccount : ContentPage
{

    public static IServiceProvider ServiceProvider { get; set; }
    private readonly ApiService _api;

    public CreateAccount(ApiService api)
	{
		InitializeComponent();
        _api = api;
	}
}