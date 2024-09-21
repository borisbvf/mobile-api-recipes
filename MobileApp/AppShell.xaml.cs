using System.Windows.Input;

namespace RecipeApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(Constants.DetailPageRoute, typeof(RecipeDetailView));
		Routing.RegisterRoute(Constants.EditPageRoute, typeof(RecipeEditView));
		Routing.RegisterRoute(Constants.SettingsPageRoute, typeof(SettingsView));
		Routing.RegisterRoute(Constants.AboutPageRoute, typeof(AboutView));

		BindingContext = this;
	}
}
