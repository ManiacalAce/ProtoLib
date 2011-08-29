using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows;
using ProtoBLL;
using ProtoBLL.BusinessEntities;
using ProtoUI.General;
using ProtoUI.ViewModels.Screens;
using ProtoUI.ViewModels.Screens.ManipulateScreens;
using ProtoUI.ViewModels.Screens.SearchScreens;
using ProtoUI.ViewModels.Screens.TransactionScreens;

namespace ProtoUI.ViewModels
{
	/// <summary>
	/// Description of WorkspaceViewModel.
	/// </summary>
	public class WorkspaceViewModel : CloseableViewModel
	{
		public WorkspaceViewModel(ProtoBridge bridge, StaffAccountBLL currUser)
		{						
			_bridge = bridge;
			_currentUser = currUser;
			
			_activeScreen = new HomeScreenViewModel(bridge, currUser);
			_activeScreen.ScreenTransition += OnTransition;
			_activeScreen.LoggedOut += OnLoggedOut;
			_activeScreen.LoggedIn += OnLoggedIn;
			ActiveLayer = _activeScreen;
			PassiveLayer = null;
						
			
			DisplayName = _activeScreen.DisplayName;			

		}
		
		
		public event Action<object> LoggedIn;
		public event Action<object> LoggedOut;
		
		//fire when new VM is loaded and layer transitions can be performed
		public event EventHandler InitiateTransitionFSTInwards;
		public event EventHandler InitiateTransitionFSTOutwards;


		//Might want to move this layering logic into view's codebehind
		#region Layering properties

		public ScreenBaseViewModel ActiveLayer
		{
			get { return _activeLayer;}
			set
			{
				if (value == _activeLayer)
					return;
				
				_activeLayer = value;
				OnPropertyChanged("ActiveLayer");
			}
		}

		public ScreenBaseViewModel PassiveLayer
		{
			get { return _passiveLayer;}
			set
			{
				if (value == _passiveLayer)
					return;
				
				_passiveLayer = value;
				OnPropertyChanged("PassiveLayer");
			}
		}		
		
		#endregion
		
			

		private void OnTransition(TransitionPath path, StaffAccountBLL currUser)
		{
			ScreenBaseViewModel scr = null;
			
			//ALT: Use a Dictionary if this becomes too large
			switch (path.To)
			{
				case LibraryScreens.SEARCH:
					scr = new SearchHostScreenViewModel(_bridge, currUser);					
					break;
				
				case LibraryScreens.HOME:
					scr = new HomeScreenViewModel(_bridge, currUser);
					break;
					
					//FIXME:Add the proper 4 extra manip enum vals & modify 
					//transition stuff accordingly
				case LibraryScreens.MANIPULATE_RECORDS:
					scr = new ManipBookDetailsViewModel(_bridge, currUser);
					break;
					
				case LibraryScreens.TRANSACTIONS_ENTERID:
					scr = new EnterMemberIdViewModel(_bridge, currUser);
					break;
					
				case LibraryScreens.TRANSACTIONS:
					scr = new MemberAccountViewModel(_bridge, currUser);
					break;
					//etc...
			}
			
			

			//Set active and passive layers for animation and inform
			//listener(view) that animation can be performed
			PassiveLayer = _activeScreen;
			PassiveLayer.ScreenTransition -= OnTransition;
			PassiveLayer.LoggedOut -= OnLoggedOut;
			PassiveLayer.LoggedIn -= OnLoggedIn;
			
			_activeScreen = scr;
			ActiveLayer = _activeScreen;
			ActiveLayer.ScreenTransition += OnTransition;
			ActiveLayer.LoggedOut += OnLoggedOut;
			ActiveLayer.LoggedIn += OnLoggedIn;
			
			if (_screenDepthMap[path.To] >= _screenDepthMap[path.From])
				InitiateTransitionFSTInwards(this, EventArgs.Empty);
			else
				InitiateTransitionFSTOutwards(this, EventArgs.Empty);
		}
		
		private void OnLoggedOut(object o)
		{
			
			if (this.LoggedOut != null)
			{
				_currentUser = null;
				LoggedOut(o);
			}
		}
		
		private void OnLoggedIn(object user)
		{
			if (this.LoggedIn != null)
			{
				_currentUser = user as StaffAccountBLL;
				LoggedIn(user);
			}
		}
		
		#region Fields
		ProtoBridge _bridge;
		
		StaffAccountBLL _currentUser;
		
		ScreenBaseViewModel _activeScreen;
		
		ScreenBaseViewModel _activeLayer;
		ScreenBaseViewModel _passiveLayer;		
		#endregion
		
		#region Static Fields
		
		static readonly Dictionary<LibraryScreens, int> _screenDepthMap =
			new Dictionary<LibraryScreens, int>()
		{
			{LibraryScreens.HOME, 1},
			{LibraryScreens.MANIPULATE_RECORDS, 2},
			{LibraryScreens.SEARCH, 2},
			{LibraryScreens.SETTINGS, 2},
			
			{LibraryScreens.MANIPULATE_BOOKDETAILS, 3},
			{LibraryScreens.MANIPULATE_AUTHORS, 3},
			{LibraryScreens.MANIPULATE_PUBLISHERS, 3},
			{LibraryScreens.MANIPULATE_MEMBERS, 3},
			{LibraryScreens.MANIPULATE_STAFF_ACCOUNTS, 3},
			
			{LibraryScreens.TRANSACTIONS_ENTERID, 2},
			{LibraryScreens.TRANSACTIONS, 3}
		};
		
		#endregion
	}
}
