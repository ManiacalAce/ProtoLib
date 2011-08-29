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
	/// Description of AuthorBLL.
	/// </summary>

	public class AuthorBLL : IKeyedItem, IDataErrorInfo
	{
		public AuthorBLL()
		{
			BookDetailsIDs = new List<int>();
		}
		
		#region Public properties
		
		public int ItemID
		{
			get;
			internal set;
		}

		public string FirstName
		{
			get;
			set;
		}		
		public string MiddleName
		{
			get;
			set;
		}		
		public string LastName
		{
			get;
			set;
		}		
		public DateTime? DateOfBirth
		{
			get;
			set;
		}		
		public DateTime? DateOfDeath
		{
			get;
			set;
		}		
		public string Nationality
		{
			get;
			set;
		}		
		public string AuthorImage
		{
			get;
			set;
		}		
		
		
		public List<int> BookDetailsIDs
		{
			get;
			private set;
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
				if (!this.IsValid)
					return string.Format("AuthorBLL item(ID:{0}) is in an invalid state", ItemID.ToString());
				else
					return null;
			}
		}	
		
		#endregion //IDataErrorInfo members
		
		#region Validation
		
		static readonly string[] ValidatedProperties = 
		{
			"FirstName",
			"MiddleName",
			"LastName",
			"DateOfBirth",
			"DateOfDeath",
			"Nationality",
			"AuthorImage"
			
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


			case "FirstName":
				error = this.ValidateFirstName();
				break;


			case "MiddleName":
				error = this.ValidateMiddleName();
				break;


			case "LastName":
				error = this.ValidateLastName();
				break;


			case "DateOfBirth":
				error = this.ValidateDateOfBirth();
				break;


			case "DateOfDeath":
				error = this.ValidateDateOfDeath();
				break;


			case "Nationality":
				error = this.ValidateNationality();
				break;


			case "AuthorImage":
				error = this.ValidateAuthorImage();
				break;


                default:
                    Debug.Fail("Unexpected property being validated on Customer: " + propertyName);
                    break;					
			}
			
			return error;			
		}
		
		#region Validation helper methods
		

		private string ValidateFirstName()
		{
			if (string.IsNullOrWhiteSpace(FirstName))
				return "The author's first name can't be empty!";
			
			return null;
		}
		

		private string ValidateMiddleName()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateLastName()
		{
			if (string.IsNullOrWhiteSpace(FirstName))
				return "The author's last name can't be empty!";
			
			return null;
		}
		

		private string ValidateDateOfBirth()
		{
			if (DateOfBirth != null && DateTime.Compare((DateTime)DateOfBirth,
	        	                                              DateTime.Now) > 0)
	        		return "The date of birth can't be in the future!";
	        	
	        return null;
		}
		

		private string ValidateDateOfDeath()
		{
			if (DateOfDeath!= null && DateTime.Compare((DateTime)DateOfDeath,
	        	                                              DateTime.Now) > 0)
	        		return "The date of death can't be in the future! (Or are you planning something sinister?)";
	        	
	        return null;
		}
		

		private string ValidateNationality()
		{
			string err = null;
			
			
			return err;
		}
		

		private string ValidateAuthorImage()
		{
			string err = null;
			
			
			return err;
		}
		

		
		#endregion //Validation helper methods
		
		#endregion //Validation
		
		
		#region Fields
		
		
		#endregion //Fields
	}
	

}
