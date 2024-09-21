using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RecipeApp.ViewModels;

public class RecipeListViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	public ObservableCollection<Recipe> Recipes { get; } = new();
	
	private IRecipeService recipeService;

	public RecipeListViewModel(IRecipeService recipeService)
	{
		this.recipeService = recipeService;
	}

	private bool isRefreshing;
	public bool IsRefreshing
	{
		get => isRefreshing;
		set
		{
			if (isRefreshing != value)
			{
				isRefreshing = value;
				OnPropertyChanged();
			}
		}
	}

	public ICommand GetRecipesCommand => new Command(GetRecipesAsync);
	public async void GetRecipesAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			IEnumerable<Recipe> data = await recipeService.GetRecipesAsync();
			if (Recipes.Count > 0)
				Recipes.Clear();
			foreach (Recipe recipe in data)
				Recipes.Add(recipe);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Unable to get recipes: {ex.Message}");
			await Shell.Current.DisplayAlert(
				LocalizationManager["Error"].ToString(),
				ex.Message,
				LocalizationManager["Ok"].ToString());
		}
		finally
		{
			IsBusy = false;
			IsRefreshing = false;
		}
	}

	public ICommand GoToDetailsCommand => new Command(GoToDetails);
	private void GoToDetails(object obj)
	{
		Recipe? recipe = obj as Recipe;
		if (recipe != null)
		{
			var navParameter = new Dictionary<string, object> { { nameof(Recipe), recipe } };
			Shell.Current.GoToAsync(Constants.DetailPageRoute, navParameter);
		}
	}

	public ICommand AddRecipeCommand => new Command(AddRecipe);
	private void AddRecipe()
	{
		var navParameter = new Dictionary<string, object> { { nameof(Recipe), new Recipe() } };
		Shell.Current.GoToAsync(Constants.EditPageRoute, navParameter);
	}

	public ICommand LogoutCommand => new Command(Logout);
	private async void Logout()
	{
		SecureStorage.Default.Remove(Constants.TokenKey);
		await Shell.Current.GoToAsync($"//{Constants.LoginPageRoute}");
	}

	public ICommand SettingsCommand => new Command(ShowSettings);
	private async void ShowSettings()
	{
		await Shell.Current.GoToAsync($"{Constants.SettingsPageRoute}");
	}
}