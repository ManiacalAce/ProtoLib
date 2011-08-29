using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Input;

namespace ProtoUI.ViewModels.Screens.SearchScreens
{
	/// <summary>
	/// Description of BookSearchViewModel.
	/// </summary>
	public class BookSearchViewModel : CategorySearchViewModel
	{
		public BookSearchViewModel()
		{
		}
		
		#region Search implementation
		public override ICommand PerformSearchCommand
		{
			get
			{
				if (_performSearchCmd == null)
					_performSearchCmd = new RelayCommand(ExecuteSearch, CanSearch);
				
				return _performSearchCmd;
			}
		}
		
		public void ExecuteSearch(object param)
		{
			//TODO: implement ExecuteSearch
		}
		
		public bool CanSearch(object param)
		{
			//TODO: implement CanSearch 
			return false;
		}
		#endregion
		
		
		
		#region Fields
		RelayCommand _performSearchCmd;
		#endregion
	}
}
