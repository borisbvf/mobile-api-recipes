using System.ComponentModel;
using System.Globalization;

namespace RecipeApp;

public partial class App : Application
{
	public App(LoadingView loadingView)
	{
		InitializeComponent();

		MainPage = new AppShell();

		SettingService.Instance.LoadThemeFromPreferences();
		SetTheme();
		SettingService.Instance.PropertyChanged += OnSettingsPropertyChanged;
		
		if (Preferences.Default.ContainsKey(Constants.LanguageKey))
		{
			string lang = Preferences.Default.Get(Constants.LanguageKey, "English");
			CultureInfo culture = lang == "Русский"
				? new CultureInfo("ru")
				: new CultureInfo("en-US");
			LocalizationManager.Instance.SetCulture(culture);
		}
	}

	private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(SettingService.Theme))
		{
			SetTheme();
		}
	}

	private void SetTheme()
	{
		UserAppTheme = SettingService.Instance?.Theme != null
			? SettingService.Instance.Theme
			: AppTheme.Unspecified;
	}
}
