namespace RecipeApp.Views;

public partial class LoadingView : ContentPage
{
	public LoadingView(LoadingViewModel loadingViewModel)
	{
		InitializeComponent();

		BindingContext = loadingViewModel;
		Appearing += async (s, e) =>
		{
			await loadingViewModel.InitAsync();
		};
	}
}