using RecipeApp.Resources.Localization;
using System.ComponentModel;
using System.Globalization;

namespace RecipeApp.Services;
public class LocalizationManager : INotifyPropertyChanged
{
	private LocalizationManager()
	{
		AppResources.Culture = CultureInfo.CurrentCulture;
	}

	public static LocalizationManager Instance = new LocalizationManager();

	public object this[string resourceKey] => AppResources.ResourceManager.GetObject(resourceKey, AppResources.Culture) ?? Array.Empty<byte>();

	public event PropertyChangedEventHandler? PropertyChanged;

	public void SetCulture(CultureInfo culture)
	{
		AppResources.Culture = culture;
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
	}
}
