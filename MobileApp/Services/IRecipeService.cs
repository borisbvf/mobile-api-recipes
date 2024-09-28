using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Services;
public interface IRecipeService
{
	public Task<IEnumerable<Recipe>> GetRecipesAsync();
	public Task AddRecipeAsync(Recipe recipe);
	public Task UpdateRecipeAsync(Recipe recipe);
	public Task DeleteRecipeAsync(int recipeId);
	public Task<RequestResult> SendEmailCode(string email);
	public Task<string?> GetAuthToken(string email, string code);
}
