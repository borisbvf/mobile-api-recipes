using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecipeApp.Models;

public class Recipe
{
	[JsonPropertyName("id")]
	public int Id { get; set; }
	[JsonPropertyName("name")]
	public string? Name { get; set; }
	[JsonPropertyName("description")]
	public string? Description { get; set; }
	[JsonPropertyName("content")]
	public string? Content { get; set; }
	[JsonPropertyName("duration")]
	public int? Duration { get; set; }
}
