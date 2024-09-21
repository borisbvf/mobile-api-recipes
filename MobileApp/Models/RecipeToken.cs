using System.Text.Json.Serialization;

namespace RecipeApp;
public class RecipeToken
{
	[JsonPropertyName("access_token")]
	public string? AccessToken { get; set; }
	[JsonPropertyName("token_type")]
	public string? TokenType { get; set; }
}
