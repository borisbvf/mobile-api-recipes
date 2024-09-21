using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content.Res;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Android.Views;
using System.ComponentModel;

namespace RecipeApp
{
	[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
	public class MainActivity : MauiAppCompatActivity
	{
		protected override void OnCreate(Bundle? savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RefreshStatusBarColor();
			SettingService.Instance.PropertyChanged += OnSettingsPropertyChanged;
		}

		public override void OnConfigurationChanged(Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			if (Build.VERSION.SdkInt < BuildVersionCodes.R)
				return;
			if (SettingService.Instance?.Theme != AppTheme.Unspecified)
				return;
			if (newConfig.IsNightModeActive)
			{
				Window?.SetStatusBarColor(Constants.MainDarkColor.ToAndroid());
				Window?.InsetsController?.SetSystemBarsAppearance(0, (int)WindowInsetsControllerAppearance.LightStatusBars);
			}
			else
			{
				Window?.SetStatusBarColor(Constants.MainLightColor.ToAndroid());
				Window?.InsetsController?.SetSystemBarsAppearance((int)WindowInsetsControllerAppearance.LightStatusBars, (int)WindowInsetsControllerAppearance.LightStatusBars);
			}
		}

		private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(SettingService.Theme))
			{
				RefreshStatusBarColor();
			}
		}

		private void RefreshStatusBarColor()
		{
			if (Build.VERSION.SdkInt < BuildVersionCodes.R)
				return;

			AppTheme? currentTheme = SettingService.Instance?.Theme;
			if (currentTheme == AppTheme.Light || currentTheme == AppTheme.Unspecified && !Resources.Configuration.IsNightModeActive)
				Window?.SetStatusBarColor(Constants.MainLightColor.ToAndroid());
			else if (currentTheme == AppTheme.Dark || currentTheme == AppTheme.Unspecified && Resources.Configuration.IsNightModeActive)
				Window?.SetStatusBarColor(Constants.MainDarkColor.ToAndroid());
		}
	}
}
