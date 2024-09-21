namespace RecipeApp.Views;

public partial class RecipeEditView : ContentPage
{
	public RecipeEditView(RecipeEditViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}