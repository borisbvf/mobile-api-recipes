using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RecipeApp.Services;

public class SettingService : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}

	private static SettingService? _instance;
	public static SettingService Instance = _instance ?? new SettingService();

	private SettingService()
	{
		theme = AppTheme.Unspecified;
	}

	public void LoadThemeFromPreferences()
	{
		if (Preferences.Default.ContainsKey(Constants.AppThemeKey))
		{
			AppTheme appTheme = (AppTheme)Preferences.Default.Get(Constants.AppThemeKey, (int)AppTheme.Unspecified);
			theme = appTheme;
		}
	}

	private AppTheme theme;
	public AppTheme Theme
	{
		get => theme;
		set
		{
			if (theme != value)
			{
				theme = value;
				OnPropertyChanged();
				Preferences.Default.Set(Constants.AppThemeKey, (int)theme);
			}
		}
	}
}
