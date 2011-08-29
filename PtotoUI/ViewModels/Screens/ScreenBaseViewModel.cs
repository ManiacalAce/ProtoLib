using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Input;
using ProtoBLL;
using ProtoBLL.BusinessEntities;
using ProtoUI.General;

namespace ProtoUI.ViewModels.Screens
{
	/// <summary>
	/// Description of ScreenBaseViewModel.
	/// </summary>
	public abstract class ScreenBaseViewModel : ViewModelBase		
	{
		public ScreenBaseViewModel(LibraryScreens screenType, ProtoBridge bridge,
		                           StaffAccountBLL currUser)
		{
			ScreenType = screenType;
			_Bridge = bridge;
			_CurrentUser = currUser;
			
			if (_CurrentUser != null)
				IsLoggedIn = true;
			else
				IsLoggedIn = false;
		}	
		
		
		//Use to inform MainWin that a valid account is using the app,
		//so maintain account across all workspaces
		public event Action<object> LoggedIn; //For MainWin?
		
		//Also use this to inform HomeScreen when logged out
		public event Action<object> LoggedOut;		

		public LibraryScreens ScreenType
		{
			get;
			private set;
		}
		
		protected ProtoBridge _Bridge
		{
			get;
			private set;
		}
		
		protected StaffAccountBLL _CurrentUser
		{
			get;
			private set;
		}
		
		protected void PerformLogin(StaffAccountBLL user)
		{
			_CurrentUser = user;
			
			if (user != null)
			{
				IsLoggedIn = true;
				if (LoggedIn != null)
					LoggedIn(user);
			}
			else
				IsLoggedIn = false;
		}
		
		public bool IsLoggedIn
		{
			get
			{
				return _isLoggedIn;
			}
			
			private set
			{
				if (_isLoggedIn == value)
					return;
				
				_isLoggedIn = value;
				base.OnPropertyChanged("IsLoggedIn");
			}
		}		
		
		#region Transition related stuff
		public delegate void TransitionHandler(TransitionPath path, StaffAccountBLL currUser);
		
		public event TransitionHandler ScreenTransition;
		
		//Since events defined in a base class can't be fired from a derived class,
		//this protected method is used to indirectly fire ScreenTransition
		protected void FireScreenTransitionEvent(LibraryScreens to)
		{
			if (ScreenTransition != null)
				ScreenTransition(new TransitionPath(ScreenType, to), _CurrentUser);
		}
		#endregion
		
		//These commands map to the 'shortcuts bar' at the bottom of every
		//screen except the Home screen.
		//HACK: Move these to their own VM with associated View?
		#region Shortcuts commands	
		
		public ICommand HomeShortcutCommand
		{
			get
			{
				if (_homeSCCmd == null)
				{
					_homeSCCmd = new RelayCommand(
						(param) =>
						{
							FireScreenTransitionEvent(LibraryScreens.HOME);
						});
				}
				return _homeSCCmd;
			}
			
		}
		public ICommand SearchShortcutCommand
		{
			get
			{
				if (_searchSCCmd == null)
				{
					_searchSCCmd = new RelayCommand(
						(param) =>
						{
							FireScreenTransitionEvent(LibraryScreens.SEARCH);
						});
				}
				return _searchSCCmd;
			}
					
		}
		public ICommand ManipulateRecordsShortcutCommand
		{
			get
			{
				if (_manipRecsSCCmd == null)
				{
					_manipRecsSCCmd = new RelayCommand(
						(param) =>
						{
							FireScreenTransitionEvent(LibraryScreens.MANIPULATE_RECORDS);
						});
				}
				return _manipRecsSCCmd;
			}
			
		}
		public ICommand TransactionsShortcutCommand
		{
			get
			{
				if (_transactionsSCCmd == null)
				{
					_transactionsSCCmd = new RelayCommand(
						(param) =>
						{
							FireScreenTransitionEvent(LibraryScreens.TRANSACTIONS_ENTERID);
						});
				}
				return _transactionsSCCmd;
			}
		}
		public ICommand SettingsShortcutCommand
		{
			get
			{
				if (_settingsSCCmd == null)
				{
					_settingsSCCmd = new RelayCommand(
						(param) =>
						{
							FireScreenTransitionEvent(LibraryScreens.SETTINGS);
						});
				}
				return _settingsSCCmd;
			}

		}
		public ICommand LogoutShortcutCommand
		{
			get
			{
				if (_logoutSCCmd == null)
				{
					_logoutSCCmd = new RelayCommand(
						(param) =>
						{
							this.PerformLogin(null);
							
							if (LoggedOut != null)
								LoggedOut(_CurrentUser);

							if (ScreenType != LibraryScreens.HOME)
								FireScreenTransitionEvent(LibraryScreens.HOME);
						});
				}
				return _logoutSCCmd;
			}

		}
		public ICommand ExitAppShortcutCommand
		{
			get
			{
				if (_exitAppSCCmd == null)
				{
					_exitAppSCCmd = new RelayCommand(
						(param) =>
						{
							//HACK: Request the main window to close in an MVVMy way.
//							if (RequestAppClose != null)
//								RequestAppClose();
							Application.Current.Shutdown();
							
						});
				}
				return _exitAppSCCmd;
			}

		}
		
		#endregion
		
		#region Fields		
		RelayCommand _homeSCCmd;
		RelayCommand _searchSCCmd;
		RelayCommand _manipRecsSCCmd;
		RelayCommand _transactionsSCCmd;
		RelayCommand _settingsSCCmd;
		RelayCommand _logoutSCCmd;
		RelayCommand _exitAppSCCmd;
		
		bool _isLoggedIn;
		
		#endregion
	}	
	
}
