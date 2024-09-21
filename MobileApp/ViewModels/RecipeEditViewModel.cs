using System.Windows.Input;

namespace RecipeApp.ViewModels;
[QueryProperty(nameof(Recipe), "Recipe")]
public class RecipeEditViewModel: BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	private IRecipeService recipeService;

	public RecipeEditViewModel(IRecipeService recipeService)
	{
		Title = "";
		this.recipeService = recipeService;
		recipe = new();
	}

	private Recipe? recipe;
	public Recipe? Recipe
	{
		get => recipe;
		set
		{
			if (recipe == value)
				return;
			recipe = value;
			OnPropertyChanged();
		}
	}

	public ICommand SaveCommand => new Command(SaveRecipe);
	private async void SaveRecipe(object obj)
	{
		Recipe? recipe = obj as Recipe;
		if (recipe != null)
		{
			if (string.IsNullOrEmpty(recipe.Name))
			{
				await Shell.Current.DisplayAlert(null, "Recipe's name cannot be empty!", "Ok");
				return;
			}
			if (string.IsNullOrEmpty(recipe.Content))
			{
				await Shell.Current.DisplayAlert(null, "Recipe's content cannot be empty!", "Ok");
				return;
			}
			if (recipe.Id == 0)
			{
				await recipeService.AddRecipeAsync(recipe);
			} else
			{
				await recipeService.UpdateRecipeAsync(recipe);
			}
			await Shell.Current.GoToAsync("..");
		}
	}

	public ICommand CancelCommand => new Command(CancelRecipe);
	private void CancelRecipe()
	{
		Shell.Current.GoToAsync("..");
	}
}
