﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace RecipeApp
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.UseMauiCommunityToolkit()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif

			builder.Services.AddSingleton<IRecipeService, RecipeService>();
			builder.Services.AddTransient<LoadingViewModel>();
			builder.Services.AddTransient<LoadingView>();
			builder.Services.AddTransient<RecipeListViewModel>();
			builder.Services.AddTransient<RecipeListView>();
			builder.Services.AddTransient<RecipeDetailViewModel>();
			builder.Services.AddTransient<RecipeDetailView>();
			builder.Services.AddTransient<RecipeEditViewModel>();
			builder.Services.AddTransient<RecipeEditView>();
			builder.Services.AddTransient<LoginViewModel>();
			builder.Services.AddTransient<LoginView>();
			builder.Services.AddTransient<AboutViewModel>();
			builder.Services.AddTransient<AboutView>();
			builder.Services.AddTransient<SettingsViewModel>();
			builder.Services.AddTransient<SettingsView>();

			return builder.Build();
		}
	}
}
