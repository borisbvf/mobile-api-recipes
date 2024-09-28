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

	private async Task<RequestResult<string?>> CheckToken()
	{
		RequestResult<string?> result = new();
		string? token = await SecureStorage.Default.GetAsync(Constants.TokenKey);
		if (token == null || !IsValidToken(token))
		{
			string? email = await SecureStorage.Default.GetAsync(Constants.EmailKey);
			string? code = await SecureStorage.Default.GetAsync(Constants.CodeKey);
			if (email != null && code != null)
			{
				RequestResult<string?> authTokenResult = await GetAuthToken(email, code);
				if (authTokenResult.IsSuccess)
				{
					result.IsSuccess = true;
					result.Data = authTokenResult.Data;
				}
				else
				{
					result.IsSuccess = false;
					result.ErrorMessage = "Something went wrong, auth token is invalid. Please try to logout and login again.";
				}
			}
			else
			{
				result.IsSuccess = false;
				result.ErrorMessage = "Something went wrong, auth token is invalid. Please try to logout and login again.";
			}
		}
		else
		{
			result.IsSuccess = true;
			result.Data = token;
		}
		return result;
	}

	public async Task<RequestResult<IEnumerable<Recipe>>> GetRecipesAsync()
	{
		RequestResult<IEnumerable<Recipe>> result = new();
		string uri = $"{BaseUrl}/recipes/list";
		try
		{
			RequestResult<string?> tokenResult = await CheckToken();
			if (tokenResult.IsSuccess)
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Data);
				HttpResponseMessage response = await httpClient.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					result.IsSuccess = true;
					result.Data = JsonSerializer.Deserialize<List<Recipe>>(content)!;
				}
				else
				{
					result.IsSuccess = false;
					int statusCode = (int)response.StatusCode;
					string content = await response.Content.ReadAsStringAsync();
					result.ErrorMessage = $"[{statusCode} {content}]";
				}
			}
			else
			{
				result.IsSuccess = false;
				result.ErrorMessage = tokenResult.ErrorMessage;
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			result.IsSuccess = false;
			result.ErrorMessage = $"{ex.Message}";
		}
		return result;
	}

	public async Task<RequestResult> AddRecipeAsync(Recipe recipe)
	{
		RequestResult result = new();
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
		return result;
	}

	public async Task<RequestResult> UpdateRecipeAsync(Recipe recipe)
	{
		RequestResult result = new();
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
		return result;
	}

	public async Task<RequestResult> DeleteRecipeAsync(int recipeId)
	{
		RequestResult result = new();
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
		return result;
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

	public async Task<RequestResult<string?>> GetAuthToken(string email, string code)
	{
		string uri = $"{BaseUrl}/owners/token";
		RequestResult<string?> result = new();
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
					result.IsSuccess = true;
					result.Data = token?.AccessToken;
				}
			}
			if (!result.IsSuccess)
			{
				result.ErrorMessage = $"{LocalizationManager.Instance["Failed to retrieve auth token."]}";
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			result.IsSuccess = false;
			result.ErrorMessage = $"{LocalizationManager.Instance["Failed to retrieve auth token."]} [{ex.Message}]";
		}
		return result;
	}
}
