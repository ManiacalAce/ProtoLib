using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Input;
using ProtoBLL;
using ProtoBLL.BusinessEntities;
using ProtoUI.General;
using ProtoUI.ViewModels;

namespace ProtoUI.ViewModels.Screens
{
	/// <summary>
	/// Description of ScreenViewModel.
	/// </summary>
	public class HomeScreenViewModel : ScreenBaseViewModel
	{
		public HomeScreenViewModel(ProtoBridge bridge, StaffAccountBLL currUser)
			:base(LibraryScreens.HOME, bridge, currUser)
		{
			DisplayName = "Home";
			
			if (currUser == null)
				LoginAnimationDone = false;
			
			if (currUser == null)
			{
				LoginBoxVisible = true;
				AdminButtonsVisible = false;
			}
			else
			{
				LoginBoxVisible = false;
				AdminButtonsVisible = true;
			}
			

			Application.Current.Properties["BorrowLimit"] = 15;
			
			//For testing
//			LoginBoxVisible=false;
//			AdminButtonsVisible = true;
		}
		
		public event EventHandler LoginSucceeded;
		
		#region Bindable properties
		

		
		public string UserName
		{
			get
			{
				return _username;
			}
			
			set
			{
				if (_username == value)
					return;
				
				_username = value;
				base.OnPropertyChanged("UserName");
			}
		}
		
		public string Password
		{
			get {return _password;}
			set
			{
				if (_password == value)
					return;
				
				_password = value;
				base.OnPropertyChanged("Password");
			}
		}
		
		public bool LoginAnimationDone
		{
			get { return _loginAnimDone;}
			set
			{
				if (_loginAnimDone == value)
					return;
				
				_loginAnimDone = value;
				base.OnPropertyChanged("LoginAnimationDone");
			}
		}
		
		public bool LoginBoxVisible
		{
			get { return _loginBoxVisible;}
			set
			{
				if (_loginBoxVisible == value)
					return;
				
				_loginBoxVisible = value;
				base.OnPropertyChanged("LoginBoxVisible");
			}
		}
		
		bool _loginBoxVisible;
		bool _adminButtonsVisible;

		public bool AdminButtonsVisible
		{
			get { return _adminButtonsVisible;}
			set
			{
				if (_adminButtonsVisible == value)
					return;
				
				_adminButtonsVisible = value;
				base.OnPropertyChanged("AdminButtonsVisible");
			}
		}		
		
		#endregion //Bindable properties
		
		
		
		#region Commands
		
		#region not needed?
		/*public ICommand GoToSearchScreenCommand
		{
			get
			{
				if (_goToSearchScreenCmd == null)
				{
					_goToSearchScreenCmd = new RelayCommand(
						(param) =>
						{
							FireScreenTransitionEvent(LibraryScreens.SEARCH);
						});
				}
				return _goToSearchScreenCmd;
			}
			
		}
		
		public ICommand GoToManipBookDetailsScreenCommand
		{
			get
			{
				if (_goToManipBookDetailsScreenCmd == null)
				{
					_goToManipBookDetailsScreenCmd = new RelayCommand(
						(param) =>
						{
							FireScreenTransitionEvent(LibraryScreens.MANIPULATE_RECORDS);
						});
				}
				return _goToManipBookDetailsScreenCmd;
			}				
		}*/
		#endregion //not needed?
		
		public ICommand LoginCommand
		{
			get
			{
				if (_loginCmd == null)
					_loginCmd = new RelayCommand(ExecuteLogin);
				
				return _loginCmd;
			}
		}
		
		private void ExecuteLogin(object o)
		{
			if (!IsLoggedIn)
			{
				StaffAccountBLL user = _Bridge.StaffAccountMgr.GetUserByLoginDetails(UserName, Password);
				
				if (user != null)
				{
					base.PerformLogin(user);
					LoginAnimationDone = true;
					AdminButtonsVisible = true;
					LoginBoxVisible = false;
					LoginSucceeded(this, EventArgs.Empty);
				}
			}
		}
		
		
		public ICommand LogoutInHomeScreenCommand
		{
			get
			{
				if (_logoutInHomeScreenCmd == null)
					_logoutInHomeScreenCmd = new RelayCommand((o) =>
					                                          {
					                                          	LoginAnimationDone = false;
					                                          	AdminButtonsVisible = false;
					                                          	LoginBoxVisible = true;
					                                          	base.LogoutShortcutCommand.Execute(this);
					                                          }
					                                         );
				return _logoutInHomeScreenCmd;
			}
		}
		
		#endregion //Commands
		
		
		#region Private Helpers
		
	
		
		#endregion //Private helpers
		
		#region Fields
		
		//RelayCommand _goToSearchScreenCmd;
		//RelayCommand _goToManipBookDetailsScreenCmd;
		
		RelayCommand _loginCmd;
		RelayCommand _logoutInHomeScreenCmd;
		
		
		string _username;
		string _password;
		
		bool _loginAnimDone;
		#endregion //Fields
	}
}
