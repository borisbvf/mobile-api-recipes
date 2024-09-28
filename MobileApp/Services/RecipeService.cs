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

	private readonly string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8000" : "http://localhost:8000";

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

	public async Task<RequestResult> SendEmailCode(string email)
	{
		string uri = $"{BaseUrl}/owners/code_query?email={email}";
		RequestResult result = new();
		try
		{
			HttpResponseMessage response = await httpClient.PostAsync(uri, null);
			if (response.IsSuccessStatusCode)
			{
				result.IsSuccess = true;
			}
			else
			{
				result.IsSuccess = false;
				int statusCode = (int)response.StatusCode;
				string content = await response.Content.ReadAsStringAsync();
				result.ErrorMessage = $"{LocalizationManager.Instance["ErrCodeWasNotSent"]} [{statusCode} {content}]";
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			result.IsSuccess = false;
			result.ErrorMessage = $"{LocalizationManager.Instance["ErrServerUnavailable"]} [{ex.Message}]";
		}
		return result;
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
