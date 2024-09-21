using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Maui.Storage;

namespace RecipeApp.Services;

public class RecipeService : IRecipeService
{
	private HttpClient httpClient;

	public RecipeService()
	{
		httpClient = new();
	}

	private const string BaseUrl = "http://localhost:8000";

	private bool IsValidToken(string token)
	{
		try
		{
			JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(token);
			return jwtSecurityToken.ValidTo > DateTime.UtcNow;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private async Task<string> GetToken()
	{
		string? token = await SecureStorage.Default.GetAsync(Constants.TokenKey);

		if (token == null || !IsValidToken(token))
		{
			string? email = await SecureStorage.Default.GetAsync(Constants.EmailKey);
			string? code = await SecureStorage.Default.GetAsync(Constants.CodeKey);
			if (email != null && code != null)
			{
				token = await GetAuthToken(email, code);
			}
			else
			{
				token = null;
			}
		}
		if (token == null)
		{
			throw new InvalidTokenException("Something went wrong, auth token is invalid. Please try to logout and login again.");
		}
		return token;
	}

	public async Task<IEnumerable<Recipe>> GetRecipesAsync()
	{
		List<Recipe> list = new List<Recipe>();
		/*
		//-------------------------------------------------------------------------------------------
		list.Add(new Recipe { Id = 1, Name = "Pancakes", Description = "Just old good pancakes", Content = "We will need some dow, sugar and salt." });
		list.Add(new Recipe { Id = 2, Name = "Verguni", Description = "Yummy thing to eat", Content = "Yougurt is neccesary" });
		list.Add(new Recipe { Id = 3, Name = "Dougnuts", Description = "Fried dough", Content = "Requares a lot of oil" });
		list.Add(new Recipe { Id = 4, Name = "Chocolate chip cookies", Description = "Backed dough", Content = "You will need chocolate chips" });
		list.Add(new Recipe { Id = 5, Name = "Pine-cones cookies", Description = "Chocolae cookies that lok like pine-cones", Content = "You will need some good cocoa" });
		return list;
		//-------------------------------------------------------------------------------------------
		*/
		string uri = $"{BaseUrl}/recipes/list";
		try
		{
			string token = await GetToken();
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = await httpClient.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				string content = await response.Content.ReadAsStringAsync();
				list = JsonSerializer.Deserialize<List<Recipe>>(content)!;
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			throw;
		}
		return list;
	}

	public async Task AddRecipeAsync(Recipe recipe)
	{
		string uri = $"{BaseUrl}/recipes/";
		try
		{
			string? token = await SecureStorage.Default.GetAsync(Constants.TokenKey);
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			string data = JsonSerializer.Serialize(recipe);
			StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await httpClient.PostAsync(uri, content);
			if (response.IsSuccessStatusCode)
			{
				Debug.WriteLine("Recipe successfully created.");
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			throw;
		}
	}

	public async Task UpdateRecipeAsync(Recipe recipe)
	{
		string uri = $"{BaseUrl}/recipes/{recipe.Id}";
		try
		{
			string? token = await SecureStorage.Default.GetAsync(Constants.TokenKey);
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			string data = JsonSerializer.Serialize(recipe);
			StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await httpClient.PutAsync(uri, content);
			if (response.IsSuccessStatusCode)
			{
				Debug.WriteLine("Recipe successfully updated.");
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			throw;
		}
	}

	public async Task DeleteRecipeAsync(int recipeId)
	{
		string uri = $"{BaseUrl}/recipes/{recipeId}";
		try
		{
			string? token = await SecureStorage.Default.GetAsync(Constants.TokenKey);
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			HttpResponseMessage response = await httpClient.DeleteAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				Debug.WriteLine("Recipe successfully deleted.");
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			throw;
		}
	}

	public async Task SendEmailCode(string email)
	{
		string uri = $"{BaseUrl}/owners/code_query?email={email}";
		try
		{
			HttpResponseMessage response = await httpClient.PostAsync(uri, null);
			if (response.IsSuccessStatusCode)
			{
				Debug.WriteLine("Email successfully sent.");
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			throw;
		}
	}

	public async Task<string?> GetAuthToken(string email, string code)
	{
		string uri = $"{BaseUrl}/owners/token";
		RecipeToken? token = null;
		try
		{
			string authStr = $"{email}:{code}";
			string param = Convert.ToBase64String(Encoding.ASCII.GetBytes(authStr));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", param);

			HttpResponseMessage response = await httpClient.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				string content = await response.Content.ReadAsStringAsync();
				token = JsonSerializer.Deserialize<RecipeToken>(content);
				if (token?.AccessToken != null)
				{
					await SecureStorage.Default.SetAsync(Constants.EmailKey, email);
					await SecureStorage.Default.SetAsync(Constants.CodeKey, code);
					await SecureStorage.Default.SetAsync(Constants.TokenKey, token?.AccessToken!);
				}
				Debug.WriteLine("Auth token successfully received.");
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			throw;
		}
		return token?.AccessToken;
	}
}
