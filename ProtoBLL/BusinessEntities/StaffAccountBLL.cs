using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System;
using ProtoBLL.BusinessInterfaces;

namespace ProtoBLL.BusinessEntities
{
	/// <summary>
	/// Description of StaffAccountBLL.
	/// </summary>

	public class StaffAccountBLL : IKeyedItem, IDataErrorInfo
	{
		public StaffAccountBLL()
		{
			_preferences = new StaffAccountBLL.AccountPrefsBLL();
		}
		
		#region Public properties
		
		public int ItemID
		{
			get;
			internal set;
		}

		public string UserName
		{
			get;
			set;
		}		
		public string Password
		{
			get;
			set;
		}		
		public int MemberID
		{
			get;
			set;
		}		
		public int ClearanceLevel
		{
			get;
			set;
		}		
		
		public AccountPrefsBLL Preferences
		{
			get
			{
				return _preferences;
			}
		}
		
		#endregion //Public properties
		
		#region IDataErrorInfo members
		
		string IDataErrorInfo.this[string propertyName]
		{
			get 
			{
				return this.GetValidationError(propertyName);
			}
		}
		
		string IDataErrorInfo.Error 
		{
			get 
			{
				return null;
			}
		}	
		
		#endregion //IDataErrorInfo members
		
		#region Validation
			
		
		static readonly string[] ValidatedProperties = 
		{
			"UserName",
			"Password",
			"MemberID",
			"ClearanceLevel"
			
		};
		
		public bool IsValid
		{
			get
			{
				foreach (string property in ValidatedProperties)
					if (GetValidationError(property) != null)
						return false;
				
				return true;
			}
		}			
		
		string GetValidationError(string propertyName)
		{
			if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
				return null;
			
			string error = null;
			
			
			switch (propertyName)
			{


			case "UserName":
				error = this.ValidateUserName();
				break;


			case "Password":
				error = this.ValidatePassword();
				break;


			case "MemberID":
				error = this.ValidateMemberID();
				break;


			case "ClearanceLevel":
				error = this.ValidateClearanceLevel();
				break;


                default:
                    Debug.Fail("Unexpected property being validated on Customer: " + propertyName);
                    break;					
			}
			
			return error;			
		}
		
		#region Validation helper methods
		

		private string ValidateUserName()
		{
			string err = null;
			
			if (string.IsNullOrWhiteSpace(UserName))
				err = "User name can't be empty";
			
			return err;
		}
		

		private string ValidatePassword()
		{
			string err = null;
			
			if (string.IsNullOrWhiteSpace(Password))
				err = "Password can't be empty";
			
			return err;
		}
		

		private string ValidateMemberID()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateClearanceLevel()
		{
			string err = null;
			
			if (ClearanceLevel < 0)
				err = "An invalid clearance level has been specified";
			
			return err;
		}
		

		
		#endregion //Validation helper methods
		
		#endregion //Validation
		
		#region Fields
		
		AccountPrefsBLL _preferences;
		
		#endregion
		
		#region Nested Types
		

		public class AccountPrefsBLL : IKeyedItem, IDataErrorInfo
		{
			public AccountPrefsBLL()
			{
				//HACK: Set these via view when implementing settings screen
				//And perform validation in outer class
				CacheSize = 5;
				SearchResultsPageSize = 20;
				FullScreenMode = "true";
			}
			
			#region Public properties
			
			public int ItemID
			{
				get;
				internal set;
			}
	
			public int CacheSize
			{
				get;
				set;
			}		
			public int SearchResultsPageSize
			{
				get;
				set;
			}		
			public string FullScreenMode
			{
				get;
				set;
			}		
			
			#endregion //Public properties
			
			#region IDataErrorInfo members
			
			string IDataErrorInfo.this[string propertyName]
			{
				get 
				{
					return this.GetValidationError(propertyName);
				}
			}
			
			string IDataErrorInfo.Error 
			{
				get 
				{
					return null;
				}
			}	
			
			#endregion //IDataErrorInfo members
			
			#region Validation
			
			static readonly string[] ValidatedProperties = 
			{
				"CacheSize",
				"SearchResultsPageSize",
				"FullScreenMode",
				
			};
			
			public bool IsValid
			{
				get
				{
					foreach (string property in ValidatedProperties)
						if (GetValidationError(property) != null)
							return false;
					
					return true;
				}
			}			
			
			string GetValidationError(string propertyName)
			{
				if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
					return null;
				
				string error = null;
				
				
				switch (propertyName)
				{
	
	
				case "CacheSize":
					error = this.ValidateCacheSize();
					break;
	
	
				case "SearchResultsPageSize":
					error = this.ValidateSearchResultsPageSize();
					break;
	
	
				case "FullScreenMode":
					error = this.ValidateFullScreenMode();
					break;
	
	
	                default:
	                    Debug.Fail("Unexpected property being validated on Customer: " + propertyName);
	                    break;					
				}
				
				return error;			
			}
			
			#region Validation helper methods
			
	
			private string ValidateCacheSize()
			{
				string err = null;
				
				if (CacheSize <= 0)
					err = "Cache size must be a positive number";
				
				return err;
			}
			
	
			private string ValidateSearchResultsPageSize()
			{
				string err = null;
				
				if (SearchResultsPageSize <= 0)
					err = "Page size of search results must be a positive number";
				
				return err;
			}
			
	
			private string ValidateFullScreenMode()
			{
				string err = null;
				
				if (FullScreenMode != "true" || FullScreenMode != "false")
					err = "Full screen mode value can only be 'true' or 'false'";
				
				return err;
			}
			
	
			
			#endregion //Validation helper methods
			
			#endregion //Validation
		}
	

			
		#endregion //Nested Types
	}
	

}
