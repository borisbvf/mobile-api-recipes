using System.Collections.ObjectModel;
using System.Globalization;

namespace RecipeApp.ViewModels;
public class SettingsViewModel : BaseViewModel
{
	private const string LanguageEnglish = "English";
	private const string LanguageRussian = "Русский";

	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	public SettingsViewModel()
	{
		Themes = new()
		{
			new SettingTheme(AppTheme.Light, LocalizationManager["LightTheme"].ToString()!),
			new SettingTheme(AppTheme.Dark, LocalizationManager["DarkTheme"].ToString()!),
			new SettingTheme(AppTheme.Unspecified, LocalizationManager["SystemTheme"].ToString()!)
		};
		selectedTheme = LoadThemeFromSettings();

		Languages = new()
		{
			LanguageEnglish,
			LanguageRussian
		};
		selectedLanguage = LoadLanguageFromSettings();
	}

	private SettingTheme LoadThemeFromSettings()
	{
		AppTheme? current = SettingService.Instance?.Theme;
		SettingTheme? theme = FindByAppTheme(current);
		return theme ?? Themes[2];
	}

	private string LoadLanguageFromSettings()
	{
		string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru"
			? LanguageRussian
			: LanguageEnglish;
		if (Preferences.Default.ContainsKey(Constants.LanguageKey))
		{
			lang = Preferences.Default.Get(Constants.LanguageKey, LanguageEnglish);
		}
		return lang;
	}

	private SettingTheme? FindByAppTheme(AppTheme? appTheme)
	{
		for (int i = 0; i < Themes.Count; i++)
		{
			if (Themes[i].AppTheme == appTheme)
			{
				return Themes[i];
			}
		}
		return null;
	}

	public List<SettingTheme> Themes { get; set; }

	private SettingTheme selectedTheme;
	public SettingTheme SelectedTheme
	{
		get => selectedTheme;
		set
		{
			if (selectedTheme != value)
			{
				selectedTheme = value;
				OnPropertyChanged();
				SettingService.Instance.Theme = selectedTheme.AppTheme;
			}
		}
	}

	public List<string> Languages { get; set; }

	private string selectedLanguage;
	public string SelectedLanguage
	{
		get => selectedLanguage;
		set
		{
			if (selectedLanguage != value)
			{
				selectedLanguage = value;
				OnPropertyChanged();
				CultureInfo culture = selectedLanguage == LanguageRussian
					? new CultureInfo("ru")
					: new CultureInfo("en-US");
				LocalizationManager.Instance.SetCulture(culture);
				Preferences.Default.Set(Constants.LanguageKey, selectedLanguage);
			}
		}
	}
}
