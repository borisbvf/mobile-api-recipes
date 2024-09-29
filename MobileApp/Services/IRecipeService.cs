using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Services;
public interface IRecipeService
{
	public Task<RequestResult<IEnumerable<Recipe>>> GetRecipesAsync();
	public Task<RequestResult> AddRecipeAsync(Recipe recipe);
	public Task<RequestResult> UpdateRecipeAsync(Recipe recipe);
	public Task<RequestResult> DeleteRecipeAsync(int recipeId);
	public Task<RequestResult> SendEmailCode(string email, string serverAddress);
	public Task<RequestResult<string?>> GetAuthToken(string serverAddress, string email, string code);
}
