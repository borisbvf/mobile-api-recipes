﻿using CommunityToolkit.Maui.Alerts;
using System.Windows.Input;

namespace RecipeApp.ViewModels;
public class LoginViewModel : BaseViewModel
{
	private IRecipeService recipeService;
	public LoginViewModel(IRecipeService recipeService)
	{
		this.recipeService = recipeService;
	}

	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	private string? email;
	public string? Email
	{
		get => email;
		set
		{
			email = value;
			OnPropertyChanged();
		} 
	}

	private string? code;
	public string? Code
	{
		get => code;
		set
		{
			code = value;
			OnPropertyChanged();
		}
	}

	public ICommand GetEmailCodeCommand => new Command(GetEmailCode);
	private async void GetEmailCode(object obj)
	{
		IsBusy = true;
		try
		{
			string? email = obj as string;
			if (email != null)
			{
				RequestResult result = await recipeService.SendEmailCode(email);
				if (result.IsSuccess)
				{
					var toast = Toast.Make(LocalizationManager["NotificationEmailSent"].ToString()!);
					await toast.Show();
				}
				else
				{
					await Application.Current!.MainPage!.DisplayAlert(
						LocalizationManager["Warning"].ToString(),
						result.ErrorMessage,
						LocalizationManager["Ok"].ToString());
				}
			}
			else
			{
				await Application.Current!.MainPage!.DisplayAlert(
					LocalizationManager["Warning"].ToString(),
					LocalizationManager["EmailIsEmptyWarning"].ToString(),
					LocalizationManager["Ok"].ToString());
			}
		}
		finally
		{
			IsBusy = false;
		}
	}

	public ICommand GetAccessTokenCommand => new Command(GetAccessToken);
	private async void GetAccessToken()
	{
		IsBusy = true;
		try
		{
			if (string.IsNullOrEmpty(email))
			{
				await Application.Current!.MainPage!.DisplayAlert(
					LocalizationManager["Warning"].ToString(),
					LocalizationManager["EmailIsEmptyWarning"].ToString(),
					LocalizationManager["Ok"].ToString());
				return;
			}
			if (string.IsNullOrEmpty(code))
			{
				await Application.Current!.MainPage!.DisplayAlert(
					LocalizationManager["Warning"].ToString(),
					LocalizationManager["CodeIsEmptyWarning"].ToString(),
					LocalizationManager["Ok"].ToString());
				return;
			}
			string? token = await recipeService.GetAuthToken(email, code);
			if (token != null)
			{
				await Shell.Current.GoToAsync($"//{Constants.MainPageRoute}");
			}
			else
			{
				await Application.Current!.MainPage!.DisplayAlert(
					LocalizationManager["Warning"].ToString(),
					LocalizationManager["CodeIsWrongWarning"].ToString(),
					LocalizationManager["Ok"].ToString());
			}
		}
		finally
		{
			IsBusy = false;
		}
	}
}
