using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Input;
using ProtoBLL;
using ProtoBLL.BusinessEntities;

namespace ProtoUI.ViewModels.Screens.SearchScreens
{
	/// <summary>
	/// Description of SearchHostScreenViewModel.
	/// </summary>
	public class SearchHostScreenViewModel : ScreenBaseViewModel
	{
		public SearchHostScreenViewModel(ProtoBridge bridge, StaffAccountBLL currUser)
			:base(LibraryScreens.SEARCH, bridge, currUser)
		{
			//_activeSearchScreen = 
		}
		
		#region Current search screen
		
		public CategorySearchViewModel ActiveSearchScreen
		{
			get {return _activeSearchScreen;}
			set
			{
				if (value == _activeSearchScreen)
					return;
				
				_activeSearchScreen = value;
				OnPropertyChanged("ActiveSearchScreen");
			}
		}
		
		#endregion
		
		#region Search-category-bar's commands
		
		public ICommand BookSearchCommand
		{
			get
			{
				if (_bookSearchCommand == null)
				{
					_bookSearchCommand = new RelayCommand((param) =>
					                                      {
					                                      		
					                                      }
					                                     );
				}
				return _bookSearchCommand;
			}
		}
		
		public ICommand AuthorSearchCommand
		{
			get
			{
				if (_authorSearchCommand == null)
				{
					_authorSearchCommand = new RelayCommand((param) =>
					                                      {
					                                      		
					                                      }
					                                     );
				}
				return _authorSearchCommand;
			}
		}
		
		public ICommand PublisherSearchCommand
		{
			get
			{
				if (_publisherSearchCommand == null)
				{
					_publisherSearchCommand = new RelayCommand((param) =>
					                                      {
					                                      		
					                                      }
					                                     );
				}
				return _publisherSearchCommand;
			}
		}
		
		public ICommand MemberSearchCommand
		{
			get
			{
				if (_memberSearchCommand == null)
				{
					_memberSearchCommand = new RelayCommand((param) =>
					                                      {
					                                      		
					                                      }
					                                     );
				}
				return _memberSearchCommand;
			}
		}
		
		public ICommand QuickSearchCommand
		{
			get
			{
				if (_quickSearchCommand == null)
				{
					_quickSearchCommand = new RelayCommand((param) =>
					                                      {
					                                      		
					                                      }
					                                     );
				}
				return _quickSearchCommand;
			}
		}
		
		#endregion
		
		
		#region Fields
		RelayCommand _bookSearchCommand;
		RelayCommand _authorSearchCommand;
		RelayCommand _publisherSearchCommand;
		RelayCommand _memberSearchCommand;
		RelayCommand _quickSearchCommand;
		
		CategorySearchViewModel _activeSearchScreen;
		#endregion
	}
}
