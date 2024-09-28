using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
	private bool isBusy;
	private bool isEnabled;
	private string? title;

	public bool IsBusy
	{
		get => isBusy;
		set
		{
			if (isBusy == value) 
				return;
			isBusy = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(IsEnabled));
		}
	}
	public bool IsEnabled { get => !isBusy; }
	public string Title
	{
		get => title ?? string.Empty;
		set
		{
			if (title == value)
				return;
			title = value;
			OnPropertyChanged();
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
