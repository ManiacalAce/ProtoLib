using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Input;

namespace ProtoUI.ViewModels.Screens.SearchScreens
{
	/// <summary>
	/// An abstract class that decribes common functionality of the
	/// various search categories.
	/// </summary>
	public abstract class CategorySearchViewModel : ViewModelBase
	{
		public CategorySearchViewModel()
		{
		}
		
		public abstract ICommand PerformSearchCommand
		{
			get; 
		}
		
		
	}
}
